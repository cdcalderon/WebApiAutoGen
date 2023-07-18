using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models.DataSync;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Data.API;
using YPrime.Web.E2E.Endpoints;
using YPrime.Web.E2E.Enums;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Models.Api;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DataSyncSteps
    {
        private const string HttpClientName = "e2eHttpClient";
        private readonly ApiTestData apiTestData;
        private readonly E2ERepository e2eRepository;
        protected readonly IHttpClientFactory httpClientFactory;
        protected readonly ScenarioContext scenarioContext;
        private readonly E2ESettings _e2eSettings;
        private int _responseStatusCode;

        private string _deviceAssetTag;

        public DataSyncSteps(CheckForUpdatesEndpoint checkForUpdatesEndpoint,
            ApiTestData apiTestData,
            E2ERepository e2eRepository,
            IHttpClientFactory httpClientFactory,
            ScenarioContext scenarioContext,
             E2ESettings e2eSettings)
        {
            this.apiTestData = apiTestData;
            this.e2eRepository = e2eRepository;
            this.httpClientFactory = httpClientFactory;
            this.scenarioContext = scenarioContext;
            _e2eSettings = e2eSettings;
        }

        [Given(@"I update the patient details as follows")]
        public void GivenIUpdateThePatientDetailsAsFollows(Table table)
        {
            var fields = table.CreateSet<SubjectsSet>();
            foreach (var field in fields)
            {
                e2eRepository.UpdatePatient(field.SubjectNumber,
                    field.SubjectStatus,
                    field.EnrollmentDate.ToUpper().Equals("YESTERDAY") ? DateTime.Now.AddDays(-1).Date.ToString() : field.EnrollmentDate,
                    field.HandheldTrainingComplete,
                    field.TabletTrainingComplete,
                    field.LastDiaryDate,
                    field.LastSyncDate);
            }
        }

        [Given(@"I have added ""(.*)"" patients to grid starting with ""(.*)"" number ""(.*)"" and associated with ""(.*)""")]
        
        public void GivenIHaveAddedPatientsToGridStartingWithNumberAndAssociatedWith(
            string count,
            string patientMappingName,
            string patientNumber,
            string siteName)
        {
            e2eRepository.AddMultiplePatient(count, patientMappingName, patientNumber, siteName);
        }

        [Given(@"Patient ""(.*)"" with patient number ""(.*)"" is associated with ""(.*)""")]
        [When(@"Patient ""(.*)"" with patient number ""(.*)"" is associated with ""(.*)""")]
        [Then(@"Patient ""(.*)"" with patient number ""(.*)"" is associated with ""(.*)""")]
        public void PatientWithPatientNumberIsAssociatedWithSite(
            string patientMappingName,
            string patientNumber,
            string siteName)
        {
            var patient = e2eRepository.AddPatient(patientNumber,siteName);

            var mapping = new PatientMapping
            {
                MappingName = patientMappingName,
                PatientNumber = patientNumber,
                SiteName = siteName,
                Patient = patient
            };

            apiTestData.PatientMappings.Add(mapping);
        }

        [Given(@"Patient ""(.*)"" with patient number ""(.*)"" with status inactive is associated with ""(.*)""")]
        public void PatientWithPatientNumberIsAssociatedWithSiteInactivePatient(
            string patientMappingName,
            string patientNumber,
            string siteName)
        {
            var patient = e2eRepository.AddPatient(
                patientNumber,
                siteName,
                false);

            var mapping = new PatientMapping
            {
                MappingName = patientMappingName,
                PatientNumber = patientNumber,
                SiteName = siteName,
                Patient = patient
            };

            apiTestData.PatientMappings.Add(mapping);
        }

        [Given(@"Patient ""(.*)"" with patient number ""(.*)"" is assigned to device ""(.*)""")]
        public void PatientIsAssignedToDevice(
            string patientMappingName,
            string patientNumber,
            string assetTag)
        {
            e2eRepository.AssignPatientToDevice(
                patientNumber,
                assetTag);
        }

        [Given(@"Patient ""(.*)"" with status ""(.*)"" and patient number ""(.*)"" is associated with ""(.*)""")]
        public async Task PatientWithPatientNumberAndStatusIsAssociatedWithSite(
            string patientMappingName,
            string statusName,
            string patientNumber,
            string siteName)
        {
            var statuses = await e2eRepository.GetPatientStatuses();

            PatientStatusModel matchingStatus = null;

            foreach (var status in statuses)
            {
                if (status.Name.Trim().ToUpper() == statusName.Trim().ToUpper())
                {
                    matchingStatus = status;
                    break;
                }
            }

            Assert.IsNotNull(matchingStatus);

            var patient = e2eRepository.AddPatient(
                patientNumber,
                siteName,
                matchingStatus.Id,
                true);

            var mapping = new PatientMapping
            {
                MappingName = patientMappingName,
                PatientNumber = patientNumber,
                SiteName = siteName,
                Patient = patient
            };

            apiTestData.PatientMappings.Add(mapping);
        }

        [Given(@"""(.*)"" Device ""(.*)"" is assigned to Site ""(.*)"" and assigned to latest software release")]
        public void AndDeviceIsAssignedSiteWithLatestSoftwareRelease(string deviceType, string assetTag, string site)
        {
            e2eRepository.AddBaseSoftwareRelease();
            e2eRepository.AddDevice(deviceType, assetTag, site);
        }


        [Given(@"Patient Visit ""(.*)"" is associated with ""(.*)""")]
        public void PatientVisitIsAssociatedWithPatient(string patientVisitMappingName,string patientMappingName)
        {
            var patientId = apiTestData.PatientMappings.FirstOrDefault(p => p.MappingName == patientMappingName).Patient.Id;
            var patientVisit = e2eRepository.AddPatientVisit(patientId, patientVisitMappingName);

            var patientVisitMapping = new PatientVisitMapping
            {
                MappingName = patientVisitMappingName,
                PatientVisit = patientVisit,
            };

            apiTestData.PatientVisitMappings.Add(patientVisitMapping);
        }

        [Given(@"Diary Entry ""(.*)"" is associated with ""(.*)""")]
        public void DiaryEntryIsAssociatedWithPatient(
            string diaryEntryMappingName,
            string patientMappingName)
        {
            var patientMapping = apiTestData
                .PatientMappings.First(pm => pm.MappingName == patientMappingName);

            var diaryEntry = e2eRepository.AddDiaryEntryForPatient(
                patientMapping.Patient.Id);

            var mapping = new DiaryEntryMapping
            {
                MappingName = diaryEntryMappingName,
                PatientMappingName = patientMapping.MappingName,
                DiaryEntry = diaryEntry
            };
            scenarioContext["diaryEntryID"] = diaryEntry.Id.ToString();
            apiTestData.DiaryEntryMappings.Add(mapping);
        }

        [Given(@"Answer ""(.*)"" is associated with ""(.*)""")]
        public void AnswerIsAssociatedWithDiaryEntry(
           string answerMappingName,
           string diaryEntryMappingName)
        {
            var diaryEntryMapping = apiTestData
                .DiaryEntryMappings.First(pm => pm.MappingName == diaryEntryMappingName);

            var answer = e2eRepository.AddAnswerForDiaryEntry(
                diaryEntryMapping.DiaryEntry.Id);

            var mapping = new AnswerMapping
            {
                MappingName = answerMappingName,
                DiaryEntryMappingName = diaryEntryMapping.MappingName,
                Answer = answer
            };

            apiTestData.AnswerMappings.Add(mapping);
        }

        [Given(@"Caregiver ""(.*)"" is associated with ""(.*)""")]
        public void CaregiverIsAssociatedWithPatient(
            string caregiverName,
            string patientMappingName)
        {
            var patientMapping = apiTestData
                .PatientMappings.First(pm => pm.MappingName == patientMappingName);

            e2eRepository.AddCaregiverForPatient(
                caregiverName,
                patientMapping.Patient.Id);
        }

        [Given(@"API request contains")]
        public async Task GivenApiRequest(Table table)
        {
            var syncRequestMap = table.CreateInstance<DataSyncRequestMap>();
            _deviceAssetTag = syncRequestMap.AssetTag;
            var device = e2eRepository.GetDevice(syncRequestMap.AssetTag);
            apiTestData.DataSyncRequest.AssetTag = syncRequestMap.AssetTag;
            apiTestData.DataSyncRequest.DeviceTypeId = Config.Enums.DeviceType
                                                        .GetAll<Config.Enums.DeviceType>()
                                                        .Single(d => d.Name == syncRequestMap.DeviceType).Id;
            if (!string.IsNullOrWhiteSpace(syncRequestMap.Patient))
            {
                apiTestData.DataSyncRequest.PatientId = await e2eRepository.GetPatientId(syncRequestMap.Patient);
            }
            apiTestData.DataSyncRequest.DeviceId = device.Id;
            apiTestData.DataSyncRequest.SoftwareVersion = e2eRepository.GetSoftwareRelease(device.SoftwareReleaseId).SoftwareVersion.VersionNumber;

            var tables = GetTableList(syncRequestMap);

            apiTestData.DataSyncRequest.ClientEntries = BuildClientEntriesFromTableList(tables);
        }

        [Given(@"Check for Updates API request contains")]
        public void GivenCheckForUpdatesApiRequest(Table table)
        {
            var requestMap = table.CreateInstance<CheckForUpdatesRequestMap>();

            var site = e2eRepository.GetSite(requestMap.Site);

            _deviceAssetTag = requestMap.AssetTag;
            var device = e2eRepository.GetDevice(requestMap.AssetTag);

            var deviceTypeId = Config.Enums.DeviceType
                    .GetAll<Config.Enums.DeviceType>()
                    .Single(d => d.Name == requestMap.DeviceType).Id;

            var configVersion = requestMap.ConfigurationVersion.Split("-").First();

            var request = new CheckForUpdatesRequest
            {
                DeviceId = device.Id,
                DeviceTypeId = deviceTypeId,
                AssetTag = requestMap.AssetTag,
                SiteId = site.Id,
                SoftwareVersion = requestMap.SoftwareVersion,
                ConfigVersion = configVersion,
                FreeSpace = 50000
            };

            apiTestData.CheckForUpdatesRequest = request;
        }

        [Given(@"the request is made to ""(.*)"" endpoint")]
        [When(@"the request is made to ""(.*)"" endpoint")]
        public async Task WhenApiRequestIsMade(string endpointMap)
        {
            var endpoint = ApiEndPoint.GetAll<ApiEndPoint>()
                                      .Single(d => d.Name == endpointMap).Id;
            var path = $"DataSync/{endpoint}";
            var client = httpClientFactory.CreateClient(HttpClientName);

            object requestObject;

            if (endpoint == ApiEndPoint.CheckForUpdates.Id)
            {
                requestObject = apiTestData.CheckForUpdatesRequest;
            }
            else
            {
                requestObject = apiTestData.DataSyncRequest;
            }

            var data = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(
                HttpMethod.Post,
                path);
            httpMessage.Content = data;

            var response = await client.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead, new CancellationToken());

            _responseStatusCode = (int)response.StatusCode;

            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            if (endpoint == ApiEndPoint.CheckForUpdates.Id)
            {
                apiTestData.CheckForUpdatesResponse = JsonConvert.DeserializeObject<CheckForUpdatesResponse>(json);
            }
            else
            {
                apiTestData.DataSyncResponse = JsonConvert.DeserializeObject<DataSyncResponse>(json);
            }
        }

        [Then(@"API response contains")]
        public void ThenTheApiResponseContains(Table table)
        {
            var tableNameData = table.Rows;

            List<DataSyncClientEntryModel> responseTables = BuildClientEntriesFromSyncResponse(apiTestData.DataSyncResponse.Tables);

            foreach (var tableNameRow in tableNameData)
            {
                var tableName = tableNameRow.Values.First();

                var matchingResponseTable = responseTables
                    .FirstOrDefault(rt => rt.TableName == tableName);

                Assert.IsNotNull(matchingResponseTable);
            }
        }


        [Then(@"API response does not contain")]
        public void ThenTheApiResponseDoesNotContain(Table table)
        {
            var tableNameData = table.Rows;

            List<DataSyncClientEntryModel> responseTables = BuildClientEntriesFromSyncResponse(apiTestData.DataSyncResponse.Tables);

            foreach (var tableNameRow in tableNameData)
            {
                var tableName = tableNameRow.Values.First();

                var matchingResponseTable = responseTables
                    .FirstOrDefault(rt => rt.TableName.Split(".").Last() == tableName);

                Assert.IsNull(matchingResponseTable);
            }
        }

        [Then(@"""(.*)"" includes ""(.*)""")]
        public void ThenTableIncludesSpecificData(
            string tableName,
            string data)
        {
            var responseTable = AssertResponseTableExists(
                tableName);

            var matchingData = GetMatchingData(
                responseTable,
                data);

            Assert.IsNotNull(matchingData);
        }

        [Then(@"""(.*)"" does not include ""(.*)""")]
        public void ThenTableDoesNotIncludeSpecificData(
            string tableName,
            string data)
        {
            var responseTable = AssertResponseTableExists(
                tableName);

            var matchingData = GetMatchingData(
                responseTable,
                data);

            Assert.IsNull(matchingData);
        }

        [Given(@"The following Software Releases are assigned to the study")]
        public void TheFollowingSoftwareVersionsAreAssignedToTheStudy(Table table)
        {
            var releaseRows = table.CreateSet<SoftwareReleaseAssignmentMap>();

            foreach (var releaseRow in releaseRows)
            {
                var version = e2eRepository.AddSoftwareVersion(
                    releaseRow.Version,
                    releaseRow.PackagePath);

                var release = e2eRepository.AddSoftwareRelease(
                    version.Id,
                    releaseRow.ConfigurationVersion);

                apiTestData.AddedSoftwareVersions.Add(version);
                apiTestData.AddedSoftwareReleases.Add(release);
            }
        }

        [Then(@"The response contains Package Path ""(.*)"" and Configuration Update ""(.*)""")]
        public void CheckForUpdatesResponseHasProperties(string packagePath, string configurationUpdate)
        {
            packagePath = packagePath.ConvertNullStringToNull();
            configurationUpdate = configurationUpdate.ConvertNullStringToNull();

            if (string.IsNullOrWhiteSpace(packagePath))
            {
                packagePath = null;
            }

            Assert.AreEqual(apiTestData.CheckForUpdatesResponse.PackagePath, packagePath);

            if (string.IsNullOrWhiteSpace(configurationUpdate))
            {
                Assert.IsNull(apiTestData.CheckForUpdatesResponse.ConfigCDNUrl);
            }
            else
            {
                var configVersionParts = configurationUpdate
                    .Split("-");

                var configVersionNumber = configVersionParts.First();
                var srdNumber = configVersionParts.Last();

                var matchingRelease = apiTestData.AddedSoftwareReleases
                    .First(sr =>
                        sr.ConfigurationVersion == configVersionNumber &&
                        sr.SRDVersion == srdNumber);

                var responseParsedConfigId = apiTestData
                    .CheckForUpdatesResponse
                    .ConfigCDNUrl
                    ?.Split("/")
                    ?.Last()
                    ?.Replace(".json", string.Empty)
                    ?.ToLower();

                var expectedConfigId = matchingRelease
                    .ConfigurationId
                    .ToString()
                    .ToLower();

                Assert.AreEqual(expectedConfigId, responseParsedConfigId);
            }
        }

        [Then("Sync Logs table has new record for following data")]
        public void SyncLogsTableHasNewRecordForFollowingData(Table table)
        {
            const string defaultUser = "System";
            const string deviceId = "DeviceID";
            const string currentDate = "Current date";
            const string lastModifiedByDatabaseUserNull = "Null";
            const string lastModifiedByDatabaseUserNotNull = "Not null";

            var auditModel = table.CreateSet<AuditModelMap>().FirstOrDefault();

            string auditUserId = default;
            if (auditModel.AuditUserID == deviceId)
            {
                if (!string.IsNullOrWhiteSpace(_deviceAssetTag))
                {
                    var device = e2eRepository.GetDevice(_deviceAssetTag);
                    if (device != null)
                        auditUserId = device.Id.ToString();
                }
                if (string.IsNullOrWhiteSpace(auditUserId))
                    auditUserId = defaultUser;
            }
            else
            {
                auditUserId = auditModel.AuditUserID;
            }

            DateTime lastModified;
            if (auditModel.LastModified == currentDate)
            {
                lastModified = DateTime.UtcNow;
            }
            else
            {
                lastModified = DateTime.Parse(auditModel.LastModified);
            }

            var lastModifiedByDatabaseUser = auditModel.LastModifiedByDatabaseUser?.ToLower();

            var syncLog = e2eRepository.GetLatestSyncLog();

            Assert.IsNotNull(syncLog);
            Assert.AreEqual(syncLog.AuditUserID, auditUserId);
            Assert.AreEqual(syncLog.LastModified.Date, lastModified.Date);
            if (lastModifiedByDatabaseUser == lastModifiedByDatabaseUserNull.ToLower())
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(syncLog.LastModifiedByDatabaseUser));
            }
            else if (lastModifiedByDatabaseUser == lastModifiedByDatabaseUserNotNull.ToLower())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(syncLog.LastModifiedByDatabaseUser));
            }
            else
            {
                Assert.Fail("LastModifiedByDatabaseUser parameter unknown value.");
            }
        }


        [Given(@"API request contains no authentication header and the following data")]
        [Given(@"API request contains authentication header and the following data")]
        public async Task GivenAPIRequestContainsAuthenticationHeaderAndTheFollowingData(Table table)
        {
            var syncRequestMap = table.CreateInstance<DataSyncRequestMap>();
            _deviceAssetTag = syncRequestMap.AssetTag;
            var device = e2eRepository.GetDevice(syncRequestMap.AssetTag);
            apiTestData.DataSyncRequest.AssetTag = syncRequestMap.AssetTag;
            apiTestData.DataSyncRequest.DeviceTypeId = Config.Enums.DeviceType
                                                        .GetAll<Config.Enums.DeviceType>()
                                                        .Single(d => d.Name == syncRequestMap.DeviceType).Id;
            if (!string.IsNullOrWhiteSpace(syncRequestMap.Patient))
            {
                apiTestData.DataSyncRequest.PatientId = await e2eRepository.GetPatientId(syncRequestMap.Patient);
            }
            apiTestData.DataSyncRequest.DeviceId = device.Id;
            apiTestData.DataSyncRequest.SoftwareVersion = e2eRepository.GetSoftwareRelease(device.SoftwareReleaseId).SoftwareVersion.VersionNumber;

            var tables = GetTableList(syncRequestMap);

            apiTestData.DataSyncRequest.ClientEntries = BuildClientEntriesFromTableList(tables);
        }

        [When(@"the request is made to ""([^""]*)"" endpoint with authentication header")]
        public async Task WhenTheRequestIsMadeToEndpointWithAuthenticationHeader(string endpointMap)
        {
            var endpoint = ApiEndPoint.GetAll<ApiEndPoint>()
                                      .Single(d => d.Name == endpointMap).Id;
            var path = $"DataSync/{endpoint}";
            var client = await httpClientFactory.CreateAuthClient(HttpClientName, _e2eSettings);

            object requestObject;

            if (endpoint == ApiEndPoint.CheckForUpdates.Id)
            {
                requestObject = apiTestData.CheckForUpdatesRequest;
            }
            else
            {
                requestObject = apiTestData.DataSyncRequest;
            }

            var data = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(
                HttpMethod.Post,
                path);
            httpMessage.Content = data;

            var response = await client.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead, new CancellationToken());

            _responseStatusCode = (int)response.StatusCode;
        }

        [Then(@"API unsuccessfully response contains (.*)")]
        [Then(@"API successfully response contains (.*)")]
        public void ThenAPISuccessfullyResponseContains(int responseCode)
        {
            Assert.AreEqual(_responseStatusCode, responseCode);
        }

        private object GetMatchingData(
            DataSyncClientEntryModel table,
            string data)
        {
            object result;

            switch (table.TableName)
            {
                case nameof(Patient):
                    var matchingPatientMapping = apiTestData.PatientMappings.FirstOrDefault(pm => pm.MappingName == data);
                    var patients = table.Rows.Select(x => JsonConvert.DeserializeObject<Patient>(x.ToString()));
                    result = patients.FirstOrDefault(p => p.Id == matchingPatientMapping?.Patient.Id);
                    break;
                case nameof(PatientVisit):
                    var matchingPatientVisitMapping = apiTestData.PatientVisitMappings.FirstOrDefault(pm => pm.MappingName == data);
                    var patientVisits = table.Rows.Select(x => JsonConvert.DeserializeObject<PatientVisit>(x.ToString()));
                    result = patientVisits.FirstOrDefault(p => p.Id == matchingPatientVisitMapping?.PatientVisit.Id);
                    break;
                case nameof(DiaryEntry):
                    var matchingDiaryEntryMapping = apiTestData.DiaryEntryMappings.FirstOrDefault(pm => pm.MappingName == data);
                    var diaryEntries = table.Rows.Select(x => JsonConvert.DeserializeObject<DiaryEntry>(x.ToString()));
                    result = diaryEntries.FirstOrDefault(p => p.Id == matchingDiaryEntryMapping?.DiaryEntry.Id);
                    break;
                case nameof(Device):
                    var devices = table.Rows.Select(x => JsonConvert.DeserializeObject<Device>(x.ToString()));
                    result = devices.FirstOrDefault(p => p.Id == apiTestData.DataSyncRequest.DeviceId);
                    break;
                case nameof(Answer):
                    var matchingAnswerMapping = apiTestData.AnswerMappings.FirstOrDefault(pm => pm.MappingName == data);
                    var answers = table.Rows.Select(x => JsonConvert.DeserializeObject<Answer>(x.ToString()));
                    result = answers.FirstOrDefault(p => p.Id == matchingAnswerMapping?.Answer.Id);
                    break;
                case nameof(StudyUser):
                    var studyUsers = table.Rows.Select(x => JsonConvert.DeserializeObject<StudyUser>(x.ToString()));
                    result = studyUsers.FirstOrDefault();
                    break;
                case nameof(StudyUserRole):
                    var studyUserRoles = table.Rows.Select(x => JsonConvert.DeserializeObject<StudyUserRole>(x.ToString()));
                    result = studyUserRoles.FirstOrDefault();
                    break;
                case nameof(Site):
                    var sites = table.Rows.Select(x => JsonConvert.DeserializeObject<Site>(x.ToString()));
                    result = sites.FirstOrDefault();
                    break;
                case nameof(SiteLanguage):
                    var siteLanguages = table.Rows.Select(x => JsonConvert.DeserializeObject<SiteLanguage>(x.ToString()));
                    result = siteLanguages.FirstOrDefault();
                    break;
                case nameof(SystemAction):
                    var systemActions = table.Rows.Select(x => JsonConvert.DeserializeObject<SystemAction>(x.ToString()));
                    result = systemActions.FirstOrDefault();
                    break;
                case nameof(SystemActionStudyRole):
                    var systemActionStudyRole = table.Rows.Select(x => JsonConvert.DeserializeObject<SystemActionStudyRole>(x.ToString()));
                    result = systemActionStudyRole.FirstOrDefault();
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private DataSyncClientEntryModel AssertResponseTableExists(string tableName)
        {
            List<DataSyncClientEntryModel> responseTables = BuildClientEntriesFromSyncResponse(apiTestData.DataSyncResponse.Tables);

            var matchingResponseTable = responseTables
                .FirstOrDefault(rt => rt.TableName.Split(".").Last() == tableName);

            Assert.IsNotNull(matchingResponseTable);

            return matchingResponseTable;
        }

        private List<DataSyncClientEntryModel> BuildClientEntriesFromSyncResponse(dynamic syncResponse)
        {
            var resultTables = new List<DataSyncClientEntryModel>();

            foreach (var syncTable in syncResponse)
            {
                var resultTable = new DataSyncClientEntryModel
                {
                    TableName = syncTable.TableName,
                    Rows = JsonConvert.DeserializeObject<List<object>>(syncTable.Rows)
                };

                resultTables.Add(resultTable);
            }

            return resultTables;
        }

        private List<dynamic> BuildClientEntriesFromTableList(List<string> tableList)
        {
            var resultTables = new List<dynamic>();

            foreach (var syncTable in tableList)
            {
                var resultTable = new DataSyncClientEntryModel
                {
                    TableName = syncTable,
                    Rows = new List<object>()
                };

                resultTables.Add(resultTable);
            }

            return resultTables;
        }

        private List<string> GetTableList(DataSyncRequestMap syncRequestMap)
        {
            var tableList = new List<string>();
            if (syncRequestMap.DeviceType == Config.Enums.DeviceType.BYOD.Name)
            {
                tableList = BYODSyncList.ClientEntries;
            }
            else
            {
                if (string.IsNullOrEmpty(syncRequestMap.Patient))
                {
                    tableList = HandheldInitialSyncList.ClientEntries;
                }
                else
                {
                    tableList = ProvisionalSyncList.ClientEntries;
                }
            }

            return tableList;
        }
    }
}
