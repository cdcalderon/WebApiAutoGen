using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ApiDtos;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.BusinessLayer.Interfaces;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.Utilities.EncryptionLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class DeviceRepository : BaseRepository, IDeviceRepository
    {
        private readonly IDiaryEntryRepository _diaryEntryRepository;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;
        private readonly IConfigurationVersionService _configurationVersionService;

        public DeviceRepository(IStudyDbContext db, IDiaryEntryRepository diaryEntryRepository, ISoftwareReleaseRepository softwareReleaseRepository, IConfigurationVersionService configurationVersionService) : base(db)
        {
            _diaryEntryRepository = diaryEntryRepository;
            _softwareReleaseRepository = softwareReleaseRepository;
            _configurationVersionService = configurationVersionService;
        }

        public void AddUpdateDevice(AddUpdateDeviceDto AddUpdateDevice, ApiRequestResultViewModel result)
        {
            var entity = _db.Devices.SingleOrDefault(d => d.Id == AddUpdateDevice.DeviceId);
            var deviceType = DeviceType.FirstOrDefault<DeviceType>(dt => dt.Name.ToLower() == AddUpdateDevice.DeviceType.ToLower());
            var deviceTypeId = deviceType?.Id ?? DeviceType.Tablet.Id;
            var availableSoftwareRelease = _softwareReleaseRepository.GetLatestSoftwareRelease(AddUpdateDevice.SiteId, deviceTypeId);

            if (availableSoftwareRelease != null)
            {
                if (entity == null)
                {
                    var device = new Device
                    {
                        Id = AddUpdateDevice.DeviceId,
                        SiteId = AddUpdateDevice.SiteId,
                        DeviceTypeId = deviceTypeId,
                        SendDatabase = false,
                        SyncVersion = 0,
                        IsDirty = false,
                        AssetTag = AddUpdateDevice.AssetTag,
                        SoftwareReleaseId = availableSoftwareRelease.Id,
                        LastReportedSoftwareVersionId = availableSoftwareRelease.SoftwareVersionId,
                        LastReportedConfigurationId = availableSoftwareRelease.ConfigurationId,
                    };

                    _db.Devices.Add(device);
                }
                else
                {
                    entity.AssetTag = AddUpdateDevice.AssetTag;
                    entity.SiteId = AddUpdateDevice.SiteId;
                    entity.SendDatabase = false;
                    entity.SyncVersion = 0;
                    entity.IsDirty = false;
                    entity.SoftwareReleaseId = availableSoftwareRelease.Id;
                }

                _db.SaveChanges(entity?.Id.ToString());
                result.WasSuccessful = true;
            }
            else
            {
                result.WasSuccessful = false;
                result.Errors.Add($"no available software release found for device {AddUpdateDevice.AssetTag}.");
            }
        }

        public Device AddDevice(Guid deviceId, Guid? patientId, Guid siteId, Guid deviceTypeId, string assetTag)
        {
            var softwareRelease = _softwareReleaseRepository.GetLatestSoftwareRelease(siteId, deviceTypeId);
            var device = new Device
            {
                Id = deviceId,
                PatientId = patientId,
                SiteId = siteId,
                DeviceTypeId = deviceTypeId,
                SendDatabase = false,
                AssetTag = assetTag,
                SyncVersion = 0,
                IsDirty = false,
                LastReportedSoftwareVersionId = softwareRelease.SoftwareVersionId,
                LastReportedConfigurationId = softwareRelease.ConfigurationId,
                SoftwareReleaseId = softwareRelease.Id,
                LastSyncDate = DateTime.Now
            };

            _db.Devices.Add(device);
            _db.SaveChanges(deviceId.ToString());

            return device;
        }

        public async Task<IOrderedQueryable<DeviceDto>> GetAllDevices(List<Guid> AllowedSites)
        {
            var deviceDtos = new List<DeviceDto>();
            var devices = _db.Devices.Where(d => AllowedSites.Count == 0 || AllowedSites.Contains(d.SiteId.Value)).ToList();
            var configurations = await _configurationVersionService.GetAll();
            foreach (var d in devices)
            {
                var assingedConfiguration = configurations.Find(c => c.Id == d.SoftwareRelease.ConfigurationId);
                var assignedConfigNumber = (assingedConfiguration != null) ? (assingedConfiguration.ConfigurationVersionNumber + "-" + assingedConfiguration.SrdVersion) : "";
                var lastConfiguration = configurations.Find(c => c.Id == d.LastReportedConfigurationId);
                var lastConfigNumber = (lastConfiguration != null) ? (lastConfiguration.ConfigurationVersionNumber + "-" + lastConfiguration.SrdVersion) : "";
                deviceDtos.Add(new DeviceDto
                {
                    Id = d.Id,
                    IMEI1 = d.IMEI1,
                    IMEI2 = d.IMEI2,
                    MACAddress = d.MACAddress,
                    SerialNumber = d.SerialNumber,
                    SiteId = d.SiteId.Value,
                    SiteNumber = d.Site.SiteNumber,
                    SiteName = d.Site.Name,
                    DeviceTypeId = d.DeviceTypeId,
                    DeviceTypeName = d.GetDeviceType()?.Name,
                    SoftwareReleaseName = d.SoftwareRelease.Name,
                    SoftwareReleaseId = d.SoftwareReleaseId,
                    AssignedSoftwareVersionNumber = d.SoftwareRelease.SoftwareVersion.VersionNumber,
                    LastReportedSoftwareVersionNumber = d.LastReportedSoftwareVersion.VersionNumber,
                    LastDataSyncDate = d.LastSyncDate,
                    AssetTag = d.AssetTag,
                    AssignedConfigurationVersionNumber = assignedConfigNumber,
                    LastReportedConfigurationVersionNumber = lastConfigNumber
                });
            }
            return deviceDtos.AsQueryable().OrderBy(d => d.DeviceTypeName);
        }

        public IList<DeviceType> GetAllDeviceTypes()
        {
            return DeviceType.GetAll<DeviceType>().ToList();
        }

        public async Task<DeviceDto> GetDevice(Guid Id, List<Guid> allowedSites)
        {
            DeviceDto result;
            Device device;
            DiaryEntryDto diaryEntry;

            var devices = _db.Devices.Where(d => allowedSites.Count == 0 || allowedSites.Contains(d.SiteId.Value)).ToList();
            device = devices.Single(d => d.Id == Id);

            result = new DeviceDto();
            result.CopyPropertiesFromObject(device);
            var lastConfig = (await _configurationVersionService.GetAll()).FirstOrDefault(x => x.Id == device.LastReportedConfigurationId);
            result.LastReportedConfigurationVersionNumber = ((lastConfig?.ConfigurationVersionNumber ?? "") + "-" + (lastConfig?.SrdVersion ?? "")).TrimEnd('-');
            result.AssignedConfigurationVersionNumber = ((device.SoftwareRelease?.ConfigurationVersion ?? "") + "-" + (device.SoftwareRelease?.SRDVersion ?? "")).TrimEnd('-');
            result.DeviceTypeName = DeviceType.FirstOrDefault<DeviceType>(dt => dt.Id == device.DeviceTypeId).Name;
            result.SiteNumber = device.Site.SiteNumber;
            result.SiteName = device.Site.Name;
            result.LastReportedConfigurationId = device.LastReportedConfigurationId;
            result.LastReportedSoftwareVersionNumber = device.LastReportedSoftwareVersion?.VersionNumber;
            result.SoftwareReleaseId = device.SoftwareReleaseId;
            result.AssignedSoftwareVersionNumber = device.SoftwareRelease.SoftwareVersion.VersionNumber;
            result.MACAddress = device.MACAddress;
            result.SerialNumber = device.SerialNumber;
            result.IMEI1 = device.IMEI1;
            result.IMEI2 = device.IMEI2;

            if (result.SyncLogs.Any())
            {
                result.LastDataSyncDate = result.SyncLogs.OrderByDescending(s => s.SyncDate).First().SyncDate;
            }

            return result;
        }

        public int GetTotalDevicesAtSite(Guid? SiteId, List<SiteDto> AllowedSites)
        {
            return GetDevicesAtSiteCountByDeviceType(SiteId, AllowedSites, null);
        }

        public int GetDevicesAtSiteCountByDeviceType(Guid? siteId, List<SiteDto> allowedSites, string deviceTypeName)
        {
            var allowedSiteIds = new List<Guid>();

            if (allowedSites != null && allowedSites.Any())
            {
                allowedSiteIds.AddRange(allowedSites
                    .Where(s => s.Id != Guid.Empty)
                    .Select(s => s.Id));
            }

            if (siteId.HasValue)
            {
                allowedSiteIds.Add(siteId.Value);
            }

            IQueryable<Device> deviceQuery = _db.Devices;

            if (allowedSiteIds.Any())
            {
                deviceQuery = deviceQuery
                    .Where(d => d.SiteId.HasValue && allowedSiteIds.Contains(d.SiteId.Value));
            }

            if (!string.IsNullOrWhiteSpace(deviceTypeName))
            {
                var deviceTypeId = DeviceType.FirstOrDefault<DeviceType>(dt => dt.Name.ToLower() == deviceTypeName.ToLower()).Id;
                deviceQuery = deviceQuery
                    .Where(d => d.DeviceTypeId == deviceTypeId);
            }

            return deviceQuery.Count();
        }

        public string GenerateToken(string Id)
        {
            string token = "";
            string encryptToken = "";
            Guid deviceId;
            if (Guid.TryParse(Id, out deviceId))
            {
                token = Guid.NewGuid().ToString();
                var E = new PasswordEncrypt(Id);
                encryptToken = E.Encrypt(token);
                var sessions = _db.DeviceSessions.Where(d => d.DeviceId == deviceId && !d.Expired);
                if (sessions.Any())
                {
                    _db.DeviceSessions.RemoveRange(sessions);
                }

                _db.DeviceSessions.Add(new DeviceSession
                {
                    DeviceId = deviceId,
                    Token = token,
                    AvailableCount = 2,
                    Expired = false
                });

                _db.SaveChanges(deviceId.ToString());
            }

            return encryptToken;
        }

        public List<dynamic> GetAdditionalTableData(Guid deviceId, List<dynamic> clientEntries)
        {
            //Check if device needs an update
            var doAdditionalTableSync = _db.Devices.Single(x => x.Id == deviceId);

            if (!doAdditionalTableSync.DoAdditionalTableSync) return clientEntries;

            var additionalTablesToSyncs = _db.AdditionalTablesToSync.Where(x => x.DoSync);

            foreach (var additionalTablesToSync in additionalTablesToSyncs)
            {
                // prevent duplicate tables which will cause a crash
                if (clientEntries.Any(x => x.TableName == additionalTablesToSync.TableName))
                {
                    continue;
                }

                dynamic table = new ExpandoObject();
                table.TableName = additionalTablesToSync.TableName;
                table.Rows = new List<dynamic>();

                // data sync repository expects a jcontainer, not list<dynamic> !
                var jdata = JToken.FromObject(table);

                clientEntries.Add(jdata);
            }

            return clientEntries;
        }

        public bool AuthenticateDevice(string Id, string Token)
        {
            var IsAuthenticated = false;
            Guid deviceId;
            if (Guid.TryParse(Id, out deviceId))
            {
                var session =
                    _db.DeviceSessions.SingleOrDefault(d => d.DeviceId == deviceId && d.Token == Token && !d.Expired);
                if (session != null)
                {
                    IsAuthenticated = true;
                    if (session.AvailableCount == 1)
                    {
                        _db.DeviceSessions.Remove(session);
                    }
                    else
                    {
                        session.AvailableCount = session.AvailableCount - 1;
                    }

                    _db.SaveChanges(deviceId.ToString());
                }
            }

            return IsAuthenticated;
        }

        public Device GetWebBackupTabletDevice(Guid siteId)
        {
            var device = _db.Devices.FirstOrDefault(x =>
                x.AssetTag != null &&
                x.AssetTag != "" &&
                x.DeviceTypeId == DeviceType.Tablet.Id &&
                x.SiteId == siteId);
            return device;
        }

        public Guid? GetDeviceIdForPatient(Guid patientId)
        {
            return _db.Devices.FirstOrDefault(d =>
                (d.DeviceTypeId == DeviceType.Phone.Id || d.DeviceTypeId == DeviceType.BYOD.Id) &&
                 d.PatientId == patientId)
                ?.Id;
        }

        public Device GetWebBackupHandheldDevice(Guid patientId)
        {
            var device = _db.Devices.FirstOrDefault(d =>
                (d.DeviceTypeId == DeviceType.Phone.Id || d.DeviceTypeId == DeviceType.BYOD.Id) &&
                 d.PatientId == patientId);
            return device;
        }

        public Device GetPatientBYODDevice(Guid patientId)
        {
            return _db.Devices.FirstOrDefault(d => d.PatientId == patientId
                    && d.DeviceTypeId == DeviceType.BYOD.Id);
        }

        public async Task<Guid?> GetLastReportedConfigurationForDevice(Guid id)
        {
            var device = await _db.Devices.FindAsync(id);
            return device?.LastReportedConfigurationId;
        }

        public async Task RemoveDevice(Device device)
        {
            if (device != null)
            {
                _db.Devices.Remove(device);

                await _db.SaveChangesAsync(device.Id.ToString());
            }
        }
    }
}