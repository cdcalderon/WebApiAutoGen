using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.DataSync.Factories;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Responses;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Attributes;
using YPrime.Data.Study.Models.Interfaces;
using YPrime.Data.Study.Models.Models.DataSync;
using YPrime.eCOA.DTOLibrary.Utils;

namespace YPrime.BusinessLayer.Repositories
{
    public class DataSyncRepository : BaseRepository, IDataSyncRepository
    {
        private const string EmptyArrayValue = "[]";
        private const string HmacAuthKeyProperty = "HMACAuthSharedKey";
        private const string NotificationServiceUrlProperty = "YPrimeNotificationScheduleUrl";
        private const string NotificationServiceApiKeyProperty = "YPrimeNotificationServiceApiKey";

        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ISqlBuilderFactory _sqlBuilderFactory;
        private readonly IAuthService _authService;
        private readonly IPatientRepository _patientRepository;

        public DataSyncRepository(
            IStudyDbContext db,
            ISoftwareReleaseRepository softwareReleaseRepository,
            IDeviceRepository deviceRepository,
            IPatientStatusService patientStatusService,
            ISqlBuilderFactory sqlBuilderFactory,
            IAuthService authService,
            IPatientRepository patientRepository)
            : base(db)
        {
            _softwareReleaseRepository = softwareReleaseRepository;
            _deviceRepository = deviceRepository;
            _patientStatusService = patientStatusService;
            _sqlBuilderFactory = sqlBuilderFactory;
            _authService = authService;
            _patientRepository = patientRepository;
        }

        public async Task<DataSyncResponse> SyncInitialData(
            Guid deviceId,
            Guid deviceTypeId,
            Guid? siteId,
            Guid? patientId,
            string softwareVersion,
            string configVersion,
            List<dynamic> clientEntries)
        {
            var syncResult = new DataSyncResponse();

            var deviceConfigId = UpdateDeviceInfo(deviceId, siteId, softwareVersion);

            var sqlBuilder = _sqlBuilderFactory.Build(
                patientId,
                deviceId,
                deviceTypeId,
                true);

            syncResult.Tables = await GetTables(
                sqlBuilder,
                clientEntries,
                deviceConfigId);

            syncResult.Environment = ConfigurationManager.AppSettings["AppEnvironment"];
            syncResult.BuilderApiBaseUrl = ConfigurationManager.AppSettings["StudyBuilderApiBaseURL"];
            SetNotificationSyncProperties(syncResult);

            syncResult.Success = true;

            return syncResult;
        }

        public async Task<DataSyncResponse> SyncClientData(
            Guid deviceId,
            Guid deviceTypeId,
            string softwareVersion,
            string configVersion,
            List<dynamic> clientEntries,
            List<dynamic> auditEntries,
            Guid? devicePatientId)
        {
            var sqlBuilder = _sqlBuilderFactory.Build(
                devicePatientId,
                deviceId,
                deviceTypeId,
                false);

            var syncResponse = new DataSyncResponse();
            var deviceConfigId = new Guid();
            List<DataSyncResponseTable> tablesWithDataToDelete;

            _db.Database.CommandTimeout = 300;
            var trans = _db.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

            try
            {
                tablesWithDataToDelete = await UpdateServerObjectsAsync(clientEntries);
                _db.Devices.Single(x => x.Id == deviceId).DoAdditionalTableSync = false;
                _db.SaveChanges(deviceId.ToString());

                if (auditEntries.Any())
                {
                    _db.SaveDeviceAudits(auditEntries);
                }

                deviceConfigId = UpdateDeviceInfo(deviceId, null, softwareVersion);

                trans.Commit();

                syncResponse.Success = true;
            }
            catch (DbEntityValidationException ex)
            {
                var validationErrors = new StringBuilder();

                foreach (var valError in ex.EntityValidationErrors)
                {
                    foreach (var err in valError.ValidationErrors)
                    {
                        validationErrors.AppendLine(err.ErrorMessage);
                    }
                }

                trans.Rollback();
                throw new ArgumentException(validationErrors.ToString());
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                foreach (var entity in ((DbContext)_db).ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                {
                    ((DbContext)_db).Entry(entity.Entity).State = EntityState.Detached;
                }
            }

            syncResponse.Tables = await GetTables(
                sqlBuilder,
                clientEntries,
                deviceConfigId);

            SetTableDataToDelete(
                syncResponse,
                tablesWithDataToDelete);

            SetNotificationSyncProperties(syncResponse);

            return syncResponse;
        }

        public CheckForUpdateResponse CheckForUpdates(Guid deviceId, string softwareVersion, string configVersion)
        {
            var checkForUpdateResponse = new CheckForUpdateResponse
            {
                PackagePath = null,
                Priority = false,
                ServerName = Environment.MachineName,
                RunSQL = false
            };

            try
            {
                var configUrl = ConfigurationManager.AppSettings["YPrimeConfigBlobURL"];
                configUrl = configUrl?.TrimEnd('/');

                var baseConfigUrl = $"{configUrl}/{DataSyncConstants.ConfigCDNUrlPath}";

                var softwareRelease = _db.Devices.FirstOrDefault(d => d.Id == deviceId).SoftwareRelease;

                // releases should only be sent to device if they are active or if the device does not yet have a release downloaded (initial sync)
                // this prevents a condition where a device does not have an active release but has not yet done a sync and then can't do anything
                // because it doesn't have a config
                if (softwareRelease.IsActive || configVersion == Config.Defaults.ConfigurationVersions.InitialVersion.ConfigurationVersionNumber)
                {
                    if (!string.IsNullOrEmpty(softwareVersion))
                    {
                        var assignedSoftwareVersion = Version.Parse(softwareRelease.SoftwareVersion.VersionNumber);
                        var currentSoftwareVersion = Version.Parse(softwareVersion);
                        if (currentSoftwareVersion < assignedSoftwareVersion)
                        {
                            var assignedVersionString = assignedSoftwareVersion.ToString();
                            var assignedVersion = _db.SoftwareVersions.SingleOrDefault(s => s.VersionNumber == assignedVersionString);
                            checkForUpdateResponse.PackagePath = assignedVersion.PackagePath;
                            checkForUpdateResponse.Priority = softwareRelease.Required;
                        }
                    }

                    if (!string.IsNullOrEmpty(softwareRelease.ConfigurationVersion) && !string.IsNullOrEmpty(configVersion))
                    {
                        var assignedConfigVersion = Version.Parse(softwareRelease.ConfigurationVersion);
                        var currentConfigVersion = Version.Parse(configVersion);

                        if (currentConfigVersion < assignedConfigVersion)
                        {
                            checkForUpdateResponse.ConfigCDNUrl = $"{baseConfigUrl}/{softwareRelease.ConfigurationId.ToString().ToLower()}.json";
                        }
                    }
                }

                var PatientDataPath = System.Configuration.ConfigurationManager.AppSettings["SharePathBase"];
                PatientDataPath = Path.Combine(PatientDataPath, "PatientData");

                // Recurse this and send the device a list of files that it can delete.
                List<string> FilesToDelete = new List<string>();
                if (Directory.Exists(PatientDataPath))
                {
                    foreach (var File in System.IO.Directory.GetFiles(PatientDataPath, "*.mp4", SearchOption.AllDirectories))
                    {
                        var FI = new FileInfo(File);
                        FilesToDelete.Add(FI.Name);
                    }
                }

                checkForUpdateResponse.DeleteFiles = FilesToDelete;
                checkForUpdateResponse.TranslationCDNUrl = $"{baseConfigUrl}/{DataSyncConstants.TranslationCDNUrlPath}";
            }
            catch (Exception e)
            {
                throw new CheckForUpdatesException();
            }

            return checkForUpdateResponse;
        }

        private void SetNotificationSyncProperties(DataSyncResponse syncResponse)
        {
            var url = ConfigurationManager.AppSettings[NotificationServiceUrlProperty];
            var hmacKey = ConfigurationManager.AppSettings[HmacAuthKeyProperty];
            var apiKey = ConfigurationManager.AppSettings[NotificationServiceApiKeyProperty];

            syncResponse.NotificationSchedulerURL = url;
            syncResponse.HmacSigningKey = hmacKey;
            syncResponse.NotificationSchedulerApiKey = apiKey;
        }

        private async Task<List<DataSyncResponseTable>> UpdateServerObjectsAsync(List<dynamic> ClientEntries)
        {
            var tablesWithDataToDelete = new List<DataSyncResponseTable>();

            foreach (var Entry in ClientEntries)
            {
                if (((string)Entry.TableName).StartsWith("ECC"))
                    continue; // Not a sync table but we need to send it down anyway so we'll send the entire table.

                string TypeName = "YPrime.Data.Study.Models." + Entry.TableName + ", YPrime.Data.Study.Models";
                System.Type EntityType = System.Type.GetType(TypeName, true, true);

                if (typeof(IDataSyncObject).IsAssignableFrom(EntityType))
                {

                    List<dynamic> Rows = Entry.Rows.ToObject<List<dynamic>>();
                    var NewObjectsList = Rows.Where(n => n.SyncVersion == 0);
                    if (NewObjectsList.Any())
                    {
                        await ProcessNewClientRecordsAsync(NewObjectsList, EntityType);
                    }

                    var UpdatedObjectsList = Rows.Cast<dynamic>().Where(n => n.SyncVersion > 0 && n.IsDirty == true);
                    if (UpdatedObjectsList.Any())
                    {
                        await ProcessUpdatedRecordsAsync(UpdatedObjectsList, EntityType);
                    }
                    var UnModifiedObjectList = Rows.Cast<dynamic>().Where(n => n.IsDirty == false);

                    var IDListFromDevice = UnModifiedObjectList.Select(n => (Guid)n.Id).AsQueryable();

                    var IDListExistingOnServer = ((((DbContext)_db)
                        .Set(EntityType)) as IQueryable<IDataSyncObject>)
                        .Where(s => IDListFromDevice.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToList();

                    var MissingServerItems = IDListFromDevice.Except(IDListExistingOnServer).ToList();
                    // Put the id's into a form that can be deserialized
                    var Objectlist = MissingServerItems.Select(m => new
                    {
                        Id = m
                    });

                    if (Objectlist.Any())
                    {
                        tablesWithDataToDelete.Add(new DataSyncResponseTable
                        {
                            TableName = Entry.TableName,
                            Rows = EmptyArrayValue,
                            DeleteList = JsonConvert.SerializeObject(Objectlist)
                        });
                    }
                }
            }

            return tablesWithDataToDelete;
        }

        private void SetTableDataToDelete(DataSyncResponse syncResponse, List<DataSyncResponseTable> tablesWithDataToDelete)
        {
            foreach (var tableWithDataToDelete in tablesWithDataToDelete)
            {
                var matchingResponseTable = syncResponse
                    .Tables
                    .FirstOrDefault(t => t.TableName == tableWithDataToDelete.TableName);

                if (matchingResponseTable != null)
                {
                    matchingResponseTable.DeleteList = tableWithDataToDelete.DeleteList;
                }
                else
                {
                    syncResponse.Tables.Add(tableWithDataToDelete);
                }
            }
        }

        private async Task ProcessNewClientRecordsAsync(IEnumerable<dynamic> NewObjects, Type EntityType)
        {
            foreach (var NewObject in NewObjects)
            {
                var EntityObject = NewObject.ToObject(EntityType);
                var Id = Guid.Parse((string)NewObject.Id);
                var ServerObject = ((DbContext)_db).Set(EntityType).Find(Id);
                if (ServerObject == null) // This check was added to stop a sync from failing if the same row was sent up as new again.
                {
                    EntityObject.SyncVersion = 1;
                    EntityObject.IsDirty = false;
                    ((DbContext)_db).Set(EntityType).Add(EntityObject);

                    await HandleNewPatientAsync(EntityObject);
                }
                else
                {
                    // LEts think about how we can fix this that is sending up the same records over and over.
                }
            }
        }

        public async Task HandleNewPatientAsync(object entity)
        {
            if (entity is Patient patient)
            {
                var authUserSignupResponse = await _authService.CreateSubjectAsync(patient.Id, _patientRepository.DecryptPin(patient));
                patient.AuthUserId = authUserSignupResponse.UserId;
            }
        }

        private async Task ProcessUpdatedRecordsAsync(IEnumerable<dynamic> UpdatedObjects, System.Type EntityType)
        {
            var IDList = UpdatedObjects.Select(c =>
                new System.Guid((string)c.Id)).ToList();

            var ServerItems = ((((DbContext)_db)
                        .Set(EntityType))
                    .AsNoTracking() as IQueryable<IDataSyncObject>)
                .Where(s => IDList.Contains(s.Id))
                .ToList();

            // Remove anything getting tracked but unmodified.

            foreach (var UpdatedObject in UpdatedObjects)
            {
                Guid Id = new Guid((string)UpdatedObject.Id);

                var ServerObject = ServerItems.Single(s => s.Id == Id);
                if (UpdatedObject.SyncVersion == ServerObject.SyncVersion) // Updated on the Client but not the server so update the server
                {
                    var EntityObject = UpdatedObject.ToObject(EntityType);

                    // handle updated patient pin
                    await HandleUpdatedPatientPinAsync(EntityObject, ServerObject);

                    // Revert anything from the server that has the NonSyncColumnAttribute attached to it.
                    RevertNonSyncColumnsWithServerValue(EntityObject, ServerObject);
                    ForceSyncDeviceColumnAlways(EntityObject, ServerObject);

                    EntityObject.IsDirty = false;

                    ((DbContext)_db).Set(EntityType).Attach(EntityObject);

                    System.Data.Entity.Infrastructure.DbEntityEntry Entry = ((DbContext)_db).Entry(EntityObject);
                    Entry.State = EntityState.Modified;
                    // update the client object with the new syncversion   ... 
                    UpdateClientObject(UpdatedObject, EntityObject);
                }
                else if (UpdatedObject.SyncVersion < ServerObject.SyncVersion)
                {
                    var EntityObject = UpdatedObject.ToObject(EntityType);
                    ForceSyncDeviceColumnAlways(EntityObject, ServerObject);
                    ((DbContext)_db).Set(EntityType).Attach(ServerObject);
                    System.Data.Entity.Infrastructure.DbEntityEntry Entry = ((DbContext)_db).Entry(ServerObject);
                    Entry.State = EntityState.Modified;
                }
            }
        }

        public async Task HandleUpdatedPatientPinAsync(object entity, IDataSyncObject serverObject)
        {
            if (entity is Patient)
            {
                var patient = entity as Patient;
                var serverPatient = (Patient)serverObject;

                var oldPin = _patientRepository.DecryptPin(serverPatient);
                var newPin = _patientRepository.DecryptPin(patient);

                if (oldPin != newPin)
                {
                    await _authService.ChangePasswordAsync(patient.AuthUserId, newPin);
                }
            }
        }

        private void ForceSyncDeviceColumnAlways(dynamic clientObject, IDataSyncObject serverObject)
        {

            var props = serverObject.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(SyncDeviceColumnAlwaysAttribute)));

            foreach (var prop in props)
            {
                prop.SetValue(serverObject, prop.GetValue(clientObject));
            }
        }

        private void RevertNonSyncColumnsWithServerValue(dynamic ClientObject, IDataSyncObject ServerObject)
        {
            var props = ServerObject.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(NonSyncColumnAttribute)));

            foreach (var prop in props)
            {
                prop.SetValue(ClientObject, prop.GetValue(ServerObject));
            }
        }

        private void UpdateClientObject(dynamic ClientObject, dynamic ServerObject)
        {
            ClientObject.SyncVersion = ServerObject.SyncVersion;
            ClientObject.IsDirty = ServerObject.IsDirty;
        }

        public async Task<List<DataSyncResponseTable>> GetTables(
            ISyncSQLBuilder sqlBuilder,
            List<dynamic> clientEntries,
            Guid deviceConfigId)
        {
            var patientStatusTypes = await _patientStatusService.GetAll(deviceConfigId);

            if (patientStatusTypes.Count == 0)
            {
                throw new StudyConfigurationException("patient status types not found.");
            }

            var activePatientStatusIds = string.Join(",", patientStatusTypes.Where(p => p.IsActive).Select(pst => pst.Id));

            var result = new List<DataSyncResponseTable>();

            foreach (var Entry in clientEntries)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                JsonWriter jsonWriter = new JsonTextWriter(sw);

                string TableName = Entry.TableName;

                System.Data.Common.DbCommand command = _db.Database.Connection.CreateCommand();
                if (_db.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    _db.Database.Connection.Open();
                }

                var SQL = sqlBuilder.GetSQLForTable(TableName, activePatientStatusIds);

                if (!string.IsNullOrEmpty(SQL))
                {
                    var responseTable = new DataSyncResponseTable
                    {
                        TableName = Entry.TableName
                    };

                    command.CommandText = SQL;
                    command.CommandType = System.Data.CommandType.Text;

                    var reader = command.ExecuteReader();

                    int fieldcount = reader.FieldCount; // count how many columns are in the row
                    object[] values = new object[fieldcount]; // storage for column values

                    jsonWriter.WriteStartArray();

                    var idColOrdinal = -1;
                    var syncVersionColumnOrdinal = -1;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                        {
                            idColOrdinal = i;
                        }
                        if (reader.GetName(i).Equals("SyncVersion", StringComparison.InvariantCultureIgnoreCase))
                        {
                            syncVersionColumnOrdinal = i;
                        }
                        if ((idColOrdinal != -1) && (syncVersionColumnOrdinal != -1))
                        {
                            break;
                        }
                    }

                    if (Entry.GetType() != typeof(ExpandoObject))
                    {
                        List<dynamic> clientRows = Entry.Rows.ToObject<List<dynamic>>();
                        var idSyncVersion = clientRows.ToDictionary(o => (Guid)o.Id, o => (int)o.SyncVersion);
                        while (reader.Read())
                        {
                            if ((idColOrdinal != -1) && (syncVersionColumnOrdinal != -1))
                            {
                                var Id = Guid.Parse(reader[idColOrdinal].ToString());
                                var SyncVersion = Convert.ToInt16(reader[syncVersionColumnOrdinal]);
                                var ContainsId = idSyncVersion.ContainsKey(Id) && idSyncVersion[Id] >= SyncVersion;
                                if (ContainsId)
                                {
                                    continue;  // No need to send this record back down.
                                }
                            }

                            reader.GetValues(values); // extract the values in each column
                            jsonWriter.WriteStartObject();
                            for (int index = 0; index < fieldcount; index++)
                            { // iterate through all columns
                                jsonWriter.WritePropertyName(reader.GetName(index)); // column name
                                jsonWriter.WriteValue(values[index]); // value in column
                            }
                            jsonWriter.WriteEndObject();
                        }
                    }
                    reader.Close();
                    jsonWriter.WriteEndArray();

                    responseTable.Rows = sb.ToString();
                    result.Add(responseTable);
                }
            }

            _db.Database.Connection.Close();

            return result;
        }

        public void LogDeviceSyncData(string configurationVersion, Guid DeviceId, string SoftwareVersion, string SyncAction, bool syncSuccess, string syncLogMessage, dynamic ClientEntries = null)
        {
            var device = _db.Devices.FirstOrDefault(d => d.Id == DeviceId);

            if (device == null)
            {
                throw new DeviceNotFoundException();
            }

            var Log = new SyncLog() { ConfigurationVersionNumber = configurationVersion, DeviceId = DeviceId, SyncDate = System.DateTime.Now, SyncAction = SyncAction };
            var softwareVersionId = _db.SoftwareVersions.SingleOrDefault(s => s.VersionNumber == SoftwareVersion)?.Id;

            Log.Id = Guid.NewGuid();
            Log.SoftwareVersionId = softwareVersionId;
            Log.SyncSuccess = syncSuccess;
            Log.SyncLogMessage = syncLogMessage;

            var Data = JsonConvert.SerializeObject(ClientEntries);
            Log.SyncData = Data;
            _db.SyncLogs.Add(Log);

            _db.SaveChanges(DeviceId.ToString());
        }

        public void CreateDeviceIfNotExists(
            Guid deviceId,
            Guid deviceTypeId,
            Guid siteId,
            string softwareVersion,
            string assetTag)
        {
            if (DeviceType.FirstOrDefault<DeviceType>(dt => dt.Id == deviceTypeId) == null)
            {
                throw new DeviceTypeNotFoundException();
            }

            var device = _db.Devices.Find(deviceId);
            if (device == null)
            {
                device = _deviceRepository.AddDevice(deviceId, null, siteId, deviceTypeId, assetTag);
                _db.DetatchEntity(device);
            }
        }

        private Guid UpdateDeviceInfo(Guid deviceId, Guid? siteId, string softwareVersion)
        {
            var deviceEntity = _db.Devices.FirstOrDefault(d => d.Id == deviceId);
            var softwareVersionEntity = _db.SoftwareVersions.FirstOrDefault(s => s.VersionNumber == softwareVersion);
            var softwareReleaseEntity = _db.SoftwareReleases.FirstOrDefault(s => s.Id == deviceEntity.SoftwareReleaseId);

            if (softwareVersionEntity != null)
            {
                deviceEntity.LastReportedSoftwareVersionId = softwareVersionEntity.Id;

                if (softwareReleaseEntity != null)
                {
                    deviceEntity.LastReportedConfigurationId = softwareReleaseEntity.ConfigurationId;
                }

                if (siteId != null)
                {
                    deviceEntity.SiteId = siteId;
                }

                deviceEntity.LastSyncDate = DateTime.Now;

                _db.SaveChanges(deviceId.ToString());
            }

            return deviceEntity?.LastReportedConfigurationId ?? Guid.Empty;
        }

        public void AddDeviceData(Guid deviceId, string fob)
        {
            var device = _db.Devices.Find(deviceId);

            if (device == null)
            {
                throw new DeviceNotFoundException();
            }

            var deviceData = new DeviceData()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                Fob = fob
            };

            _db.DeviceDatas.Add(deviceData);
            _db.SaveChanges(deviceId.ToString());
        }
    }
}

