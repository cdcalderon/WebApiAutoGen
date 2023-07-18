using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.eCOA.Utilities.Helpers;
using YPrime.Web.E2E.Models.Api;
using Config.Enums;
using System.Text;
using YPrime.Core.BusinessLayer.Extensions;

namespace YPrime.Web.E2E.Data
{
    public class E2ERepository
    {
        private const string WorkingVersionName = "Working";
        private const string InitialSiteName = "Initial Site";
        private const string QuestionnaireId = "ADF2EC95-A7F7-E711-80DD-000D3A1029A9";
        private const string en_USId = "8b55eac8-f1a0-4dde-ba13-b7c2e4486d18";
        private const string de_GermanId = "8b55eac8-f1a0-4dde-ba13-b7c2e4486d16";
        private readonly IStudyDbContext db;
        private readonly E2ESettings e2eSettings;
        private readonly ICountryService countryService;
        private readonly IConfigurationVersionService configurationVersionService;
        private readonly IQuestionnaireService questionnaireService;
        private readonly ISubjectInformationService subjectInformationService;
        private readonly IStudySettingService studySettingService;
        private readonly ISessionService sessionService;
        private readonly IStudyRoleService studyRoleService;
        private readonly ICorrectionTypeService correctionTypeService;
        private readonly ICorrectionWorkflowService correctionWorkflowService;
        private readonly IPatientStatusService patientStatusService;
        private readonly ILanguageService languageService;
        private readonly IVisitService visitService;

        private List<SystemActionStudyRole> _systemActionStudyRoles;
        private Dictionary<Guid, Dictionary<string, Guid>> _siteLanguages = new Dictionary<Guid, Dictionary<string, Guid>>();
        private readonly ApiTestData apiTestData;

        public E2ERepository(
            IStudyDbContext db,
            E2ESettings e2eSettings,
            ICountryService countryService,
            IConfigurationVersionService configurationVersionService,
            IQuestionnaireService questionnaireService,
            ISubjectInformationService subjectInformationService,
            IStudySettingService studySettingService,
            ISessionService sessionService,
            IStudyRoleService studyRoleService,
            ICorrectionTypeService correctionTypeService,
            ICorrectionWorkflowService correctionWorkflowService,
            IPatientStatusService patientStatusService,
            ApiTestData apiTestData,
            ILanguageService languageService,
            IVisitService visitService)
        {

            this.db = db;
            this.countryService = countryService;
            this.configurationVersionService = configurationVersionService;
            this.questionnaireService = questionnaireService;
            this.subjectInformationService = subjectInformationService;
            this.studySettingService = studySettingService;
            this.e2eSettings = e2eSettings;
            this.sessionService = sessionService;
            this.studyRoleService = studyRoleService;
            this.correctionTypeService = correctionTypeService;
            this.correctionWorkflowService = correctionWorkflowService;
            this.patientStatusService = patientStatusService;
            this.apiTestData = apiTestData;
            this.languageService = languageService;
            this.visitService = visitService;
        }



        public void setGridData(string gridData, string tableName)
        {
            switch (tableName)

            {
                case "CareGivers":

                    var name = db.CareGivers.FirstOrDefault();
                    name.IsHandheldTrainingComplete = true;
                    name.IsTabletTrainingComplete = true;
                    name.LockoutEnabled = true;
                    db.CareGivers.AddOrUpdate(name);
                    break;
                case "Subject":

                    var patientname = db.Patients.FirstOrDefault();
                    patientname.IsHandheldTrainingComplete = true;
                    patientname.IsTabletTrainingComplete = true;
                    break;
            }
            db.SaveChanges(null);
        }
        public void AddDevice(string deviceType, string assetTag, string siteName)
        {
            var deviceTypeEnum = DeviceType.FirstOrDefault<DeviceType>(dt => dt.Name == deviceType);
            var site = db.Sites.FirstOrDefault(s => s.Name == siteName);
            var softwareRelease = db.SoftwareReleases.OrderByDescending(s => s.StudyWide && s.IsActive).FirstOrDefault();

            db.Devices.AddOrUpdate(d => d.AssetTag,
                new Device()
                {
                    Id = Guid.NewGuid(),
                    DeviceTypeId = deviceTypeEnum.Id,
                    SiteId = site.Id,
                    AssetTag = assetTag,
                    DoAdditionalTableSync = false,
                    SyncVersion = 1,
                    IsDirty = false,
                    SoftwareReleaseId = softwareRelease.Id,
                    LastReportedConfigurationId = softwareRelease.ConfigurationId,
                    LastReportedSoftwareVersionId = softwareRelease.SoftwareVersionId
                });

            db.SaveChanges(null);
        }

        public Device GetDevice(string assetTag)
        {
            var device = db.Devices.FirstOrDefault(d => d.AssetTag == assetTag);

            return device;
        }

        public Site GetSite(string siteName)
        {
            var site = db.Sites.FirstOrDefault(s => s.Name == siteName);

            return site;
        }

        public Patient AddPatient(
            string patientNumber,
            string siteName,
            int patientStatusTypeId,
            bool forceAdd = false)
        {
            Patient result;

            var site = db.Sites.FirstOrDefault(s => s.Name == siteName);

            var patient = forceAdd ? null : db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber);

            if (patient == null)
            {
                result = new Patient()
                {
                    Id = Guid.NewGuid(),
                    PatientNumber = patientNumber,
                    LanguageId = YPrime.Config.Defaults.Languages.English.Id,
                    SiteId = site.Id,
                    SyncVersion = 1,
                    IsDirty = false,
                    PatientStatusTypeId = patientStatusTypeId,
                    ConfigurationId = GetLatestConfigId(),
                    EnrolledDate = DateTime.Now.Date
                };

                db.Patients.AddOrUpdate(result);
            }
            else
            {
                patient.SiteId = site.Id;

                result = patient;
            }

            db.SaveChanges(null);

            return result;
        }

        public Patient AddPatient(string patientNumber, string siteName = InitialSiteName, bool isActive = true)
        {
            var patientStatusTypeId = isActive
                ? 1
                : 99;

            var result = AddPatient(patientNumber, siteName, patientStatusTypeId);

            return result;
        }

        public void AddMultiplePatient(string count, string patientMappingName, string patientNumber, string siteName = InitialSiteName, bool isActive = true)
        {
            var _patientNumber = patientNumber.Substring(0, patientNumber.Length - 3);
            for (int i = 1; i <= int.Parse(count) && i <= 999; i++)
            {
                var patientNumberIs = (i >= 100 && i <= 999)
                    ? _patientNumber + i.ToString()
                    : ((i < 10)
                    ? _patientNumber + "00" + i.ToString()
                    : _patientNumber + "0" + i.ToString());

                var patientStatusTypeId = isActive
                ? 1
                : 99;
                var patient = AddPatient(patientNumberIs, siteName, patientStatusTypeId);

                var _patientMappingName = new string(patientMappingName.Take(8).ToArray());
                var patientMappingNameIs = _patientMappingName + i.ToString();
                var mapping = new PatientMapping
                {
                    MappingName = patientMappingNameIs,
                    PatientNumber = patientNumber,
                    SiteName = siteName,
                    Patient = patient
                };

                apiTestData.PatientMappings.Add(mapping);
            }
        }

        public void UpdatePatient(string subjectNumber, string subjectStatus, string enrollmentDate, bool handheldTrainingComplete, bool tabletTrainingComplete, string lastDiaryDate, string lastSyncDate)
        {

            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == subjectNumber);
            Guid patientId = patient.Id;
            var patientStatusTypes = patientStatusService.GetAll();
            var patientStatus = patientStatusTypes.Result.FirstOrDefault(i => i.Name == subjectStatus);
            patient.PatientStatusTypeId = patientStatus.Id;
            patient.EnrolledDate = Convert.ToDateTime(enrollmentDate);
            patient.IsHandheldTrainingComplete = handheldTrainingComplete;
            patient.IsTabletTrainingComplete = tabletTrainingComplete;
            db.Patients.AddOrUpdate(patient);
            db.SaveChanges(null);
            if (patientId != null)
            {
                var diaryEntry = db.DiaryEntries.FirstOrDefault(p => p.PatientId == patientId);
                if (diaryEntry != null)
                {
                    diaryEntry.DiaryDate = Convert.ToDateTime(lastDiaryDate);
                    db.DiaryEntries.AddOrUpdate(diaryEntry);
                    var sql = $"UPDATE [dbo].DiaryEntry SET TransmittedTime = '{lastSyncDate}' where PatientId='{patient.Id}'";
                    db.Database.ExecuteSqlCommand(sql);
                    db.SaveChanges(null);
                }
            }
        }

        public void UpdateSubjectNumber(string SubjectNumber, string NewSubjectNumber)
        {
            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == SubjectNumber);
            patient.PatientNumber = NewSubjectNumber;
            db.Patients.AddOrUpdate(patient);
            db.SaveChanges(null);

        }

        public async Task UpdateQuestionnaireTime(Table table)
        {
            var allquestionnaires = await questionnaireService.GetAllInflatedQuestionnaires();
            foreach (var row in table.Rows)
            {
                var subject = row["Subject"];
                var patient = row["Patient"];
                var questionnaireName = row["Forms"];
                var questionnaireStartTime = row["Started Time"];
                var questionnaireCompletionTime = row["Completed Time"];

                var questionnaire = allquestionnaires.Find(q => q.InternalName == questionnaireName);
                var patientId = db.Patients.FirstOrDefault(p => p.PatientNumber == subject).Id;
                var diaryEntry = db.DiaryEntries.FirstOrDefault(s => s.PatientId == patientId && s.QuestionnaireId == questionnaire.Id);
                diaryEntry.DiaryDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
                diaryEntry.StartedTime = Convert.ToDateTime(questionnaireStartTime);
                diaryEntry.CompletedTime = Convert.ToDateTime(questionnaireCompletionTime);
                db.DiaryEntries.AddOrUpdate(diaryEntry);
                db.SaveChanges(null);
            }
        }

        public void RemoveSubjectNumber(string SubjectNumber)
        {
            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == SubjectNumber);
            patient.PatientStatusTypeId = 99;
            db.Patients.AddOrUpdate(patient);
            db.SaveChanges(null);
        }

        public Task<PatientAttribute> AddPatientAttribute(string patientNumber, string attributeName, string attributeValue)
        {
            var patientId = db.Patients.Where(p => p.PatientNumber == patientNumber).Select(p => p.Id).First();

            return AddPatientAttribute(patientId, attributeName, attributeValue);
        }

        public async Task<PatientAttribute> AddPatientAttribute(Guid patientId, string attributeName, string attributeValue)
        {
            var subjectInfos = await GetSubjectInformationModelsForVersion(WorkingVersionName);

            var matchingInfo = subjectInfos
                .First(si => si.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));

            var newAttribute = new PatientAttribute
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                PatientAttributeConfigurationDetailId = matchingInfo.Id
            };

            if (matchingInfo.ChoiceType == DataType.ChoicesAttribute.DisplayName)
            {
                var matchingChoice = matchingInfo.Choices.First(c => c.Name.Equals(attributeValue));
                newAttribute.AttributeValue = matchingChoice.Id.ToString();
            }
            else
            {
                newAttribute.AttributeValue = attributeValue;
            }

            db.PatientAttributes.AddOrUpdate(newAttribute);
            db.SaveChanges(null);

            return newAttribute;
        }

        public PatientVisit AddPatientVisit(Guid patientId, string patientVisitName, string statusTypeName = "Not Started")
        {
            var patientVisit = db.PatientVisits.FirstOrDefault(pv => pv.Notes == patientVisitName);

            if (patientVisit == null)
            {
                var visitId = Guid.Empty.ToString();

                switch (patientVisitName)
                {
                    case var i when patientVisitName.Contains(CommonData.ScreeningVisitName):
                        visitId = CommonData.visitIdScreening;
                        break;
                    case var i when patientVisitName.Contains(CommonData.EnrollmentVisitName):
                        visitId = CommonData.visitIdEnrollment;
                        break;
                    case var i when patientVisitName.Contains(CommonData.TreatmentVisitName):
                        visitId = CommonData.visitIdTreatment;
                        break;
                }

                var statusTypeId = PatientVisitStatusType.NotStarted.Id;
                switch(statusTypeName)
                {
                    case "In Progress":
                        statusTypeId = PatientVisitStatusType.InProgress.Id;
                        break;
                    case "Partial":
                        statusTypeId = PatientVisitStatusType.Partial.Id;
                        break;
                    case "Complete":
                        statusTypeId = PatientVisitStatusType.Complete.Id;
                        break;
                    case "Missed":
                        statusTypeId = PatientVisitStatusType.Missed.Id;
                        break;
                }

                var patientVisit1 = new PatientVisit
                {
                    Id = Guid.NewGuid(),
                    VisitId = Guid.Parse(visitId),
                    PatientId = patientId,
                    Notes = patientVisitName,
                    PatientVisitStatusTypeId = statusTypeId,
                    ProjectedDate = DateTime.Now.AddDays(1),
                    VisitDate = DateTime.Now,
                    IsDirty = true,
                    ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id,
                    OutsideVisitWindow = false,
                    ActivationDate = DateTime.Now,
                    SyncVersion = 1,
                };

                db.PatientVisits.AddOrUpdate(patientVisit1);

                db.SaveChanges(null);
            }

            return patientVisit;
        }

        public void UpdatePatientVisit(string date, string patientVisitName, Guid patientId)
        {
            var patientVisit = db.PatientVisits.FirstOrDefault(pv => 
                pv.PatientId == patientId &&
                pv.Notes == patientVisitName);

            if (patientVisit == null)
            {
                patientVisit = AddPatientVisit(
                    patientId,
                    patientVisitName);
            }

            var activationDate = DateTime.Parse(date);
            patientVisit.ActivationDate = activationDate;

            db.PatientVisits.AddOrUpdate(patientVisit);

            db.SaveChanges(null);
        }

        public async Task AddDataInSoftwareVersion(string versionNumber, string packagePath)
        {
            var SoftwareVersion = db.SoftwareVersions.FirstOrDefault(pv => pv.VersionNumber == versionNumber);
            {
                SoftwareVersion = new SoftwareVersion
                {
                    Id = Guid.NewGuid(),
                    VersionNumber = versionNumber,
                    PackagePath = packagePath,
                    PlatformTypeId = Guid.NewGuid(),
                };
                db.SoftwareVersions.AddOrUpdate(SoftwareVersion);
                await db.SaveChangesAsync(null);
            }
        }

        public async void AssignPatientToDevice(string patientNumber, string assetTag)
        {
            var device = db.Devices.FirstOrDefault(s => s.AssetTag == assetTag);
            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber);

            device.PatientId = patient.Id;

            db.Devices.AddOrUpdate(device);

            db.SaveChanges(null);
        }

        public void SetDeviceLastSyncDate(string assetTag, string syncAction, DateTime? date = null)
        {
            var device = db.Devices.FirstOrDefault(s => s.AssetTag == assetTag);
            var syncLog = db.SyncLogs.FirstOrDefault(s => s.DeviceId == device.Id);
            if (syncLog == null)
            {
                syncLog = new SyncLog
                {
                    Id = Guid.NewGuid(),
                    DeviceId = device.Id,
                    SoftwareVersionId = device.SoftwareRelease.SoftwareVersionId,
                    SyncAction = syncAction,
                    SyncData = $"{{ assetTag : {device.AssetTag} }}",
                    SyncSuccess = true,
                    ConfigurationVersionNumber = device.SoftwareRelease.ConfigurationVersion
                };
            }

            device.LastSyncDate = (date != null) ? date : DateTime.Now;
            syncLog.SyncDate = (DateTime)device.LastSyncDate;
            db.Devices.AddOrUpdate(d => d.AssetTag, device);
            db.SyncLogs.AddOrUpdate(syncLog);
            db.SaveChanges(null);
        }

        public void UnAssignPatientsFromDevices()
        {
            var devices = db.Devices;

            foreach (var device in devices)
            {
                device.PatientId = null;
            }

            db.SaveChanges(null);
        }

        public string GetSiteNumberFromSitesTable(string siteNumber)
        {
            string tableValue = db.Sites.Where(p => p.SiteNumber == siteNumber).Select(p => p.SiteNumber).First().ToString();
            return tableValue;
        }

        public Guid GetSiteLanguageId(Guid siteId, string language)
        {
            Guid languageID = Guid.Empty;

            _siteLanguages.TryGetValue(siteId, out var languages);
            languages?.TryGetValue(language, out languageID);

            return languageID;
        }

        public async Task<Guid> SetLanguageForSite(string language, string siteName)
        {
            Guid languageId = Guid.Empty;
            var siteId = db.Sites.Where(s => s.Name == siteName).Select(p => p.Id).First();

            if (language == "English")
            {
                languageId = Guid.Parse(en_USId);
            }
            else if (language == "German")
            {
                languageId = Guid.Parse(de_GermanId);
            }
            else
            {
                var languages = await GetAllLanguages();
                var sbLanguage = languages.FirstOrDefault(l => l.Name == language);
                languageId = sbLanguage != null ? sbLanguage.Id : Guid.Empty;
            }
            
            var siteLanguage = db.SiteLanguages.FirstOrDefault(s => s.LanguageId == languageId);

            if (siteLanguage == null)
            {
                db.SiteLanguages.AddOrUpdate(
                   new SiteLanguage()
                   {
                       
                       Id = Guid.NewGuid(),
                       SiteId = siteId,
                       LanguageId = languageId,
                   });

                db.SaveChanges(null);

            }

            if (!_siteLanguages.ContainsKey(siteId))
            {
                _siteLanguages[siteId] = new Dictionary<string, Guid>();
            }

            _siteLanguages[siteId][language] = languageId;

            return languageId;
        }

        public string GetActiveValuefromSiteTable(string siteName)
        {
            string tableValue = db.Sites.Where(p => p.Name == siteName).Select(p => p.IsActive).First().ToString();
            return tableValue;
        }

        public void SetIsActivetoValue(string siteNumber, string value)
        {
            var site = db.Sites.FirstOrDefault(s => s.SiteNumber == siteNumber);
            site.IsActive = false;
            db.Sites.AddOrUpdate(site);
            db.SaveChanges(null);
        }

        public async Task CreateReleaseWithVersions(
            string releaseName,
            string softwareVersion,
            string configVersion,
            bool studyWide = true)
        {
            var version = db
                .SoftwareVersions
                .First(v => v.VersionNumber == softwareVersion);

            var configVersions = await configurationVersionService
                .GetAll();

            var selectedConfig = configVersions
                .First(cv => cv.DisplayVersion == configVersion);

            var releaseId = Guid.NewGuid();

            db
                .SoftwareReleases
                .AddOrUpdate(sr => sr.Id,
                    new SoftwareRelease
                    {
                        Id = releaseId,
                        Name = releaseName,
                        StudyWide = studyWide,
                        IsActive = true,
                        SoftwareVersionId = version.Id,
                        ConfigurationId = selectedConfig.Id,
                        ConfigurationVersion = selectedConfig.ConfigurationVersionNumber,
                        SRDVersion = selectedConfig.SrdVersion,
                        DateCreated = DateTime.Now
                    });

            db.SaveChanges(null);
        }

        public async Task SetDeviceSoftwareAndConfigVersions(
            string assetTag,
            string softwareVersion,
            string configVersion)
        {
            var configVersions = await configurationVersionService
                .GetAll();

            var selectedConfig = configVersions
                .First(cv => cv.DisplayVersion == configVersion);

            var release = db
                .SoftwareReleases
                .First(sr =>
                    sr.SoftwareVersion.VersionNumber == softwareVersion &&
                    sr.ConfigurationId == selectedConfig.Id);

            var selectedDevice = db
                .Devices
                .First(d => d.AssetTag == assetTag);

            selectedDevice.SoftwareReleaseId = release.Id;

            db.SaveChanges(null);
        }

        public async Task SetDeviceSoftwareRelease(
            string deviceType,
            string assetTag,
            string releaseName)
        {
            var release = db
                .SoftwareReleases
                .FirstOrDefault(sr => sr.Name == releaseName);

            var selectedDevice = db
                .Devices
                .FirstOrDefault(d => d.AssetTag == assetTag);

            selectedDevice.SoftwareReleaseId = release.Id;
            selectedDevice.LastReportedConfigurationId = release.ConfigurationId;

            db.SaveChanges(null);
        }

        public async Task SetSoftwareReleaseSite(
            string releaseName,
            string siteName)
        {
            var site = await db
                .Sites
                .FirstAsync(s => s.Name == siteName);

            var release = await db
                .SoftwareReleases
                .Include(r => r.Sites)
                .FirstAsync(r => r.Name == releaseName);

            if (!release.Sites.Any(s => s.Id == site.Id))
            {
                release.Sites.Add(site);
                await db.SaveChangesAsync(null);
            }
        }

        public async Task SetSoftwareReleaseCountry(
            string releaseName,
            string countryName)
        {
            var countries = await countryService.GetAll();
            var country = countries.First(c => c.Name == countryName);

            var release = await db
                .SoftwareReleases
                .FirstAsync(r => r.Name == releaseName);

            if (!db.SoftwareReleaseCountry.Any(sc => sc.CountryId == country.Id && sc.SoftwareReleaseId == release.Id))
            {
                var softwareReleaseCountry = new SoftwareReleaseCountry
                {
                    SoftwareReleaseId = release.Id,
                    CountryId = country.Id
                };

                db.SoftwareReleaseCountry.Add(softwareReleaseCountry);
                await db.SaveChangesAsync(null);
            }
        }

        public async Task EnableSoftwareReleaseProperty(
            string releaseName,
            string propertyName)
        {
            var release = await db
                .SoftwareReleases
                .FirstAsync(r => r.Name == releaseName);

            switch (propertyName.ToUpper())
            {
                case "STUDY WIDE":
                    release.StudyWide = true;
                    break;
                default:
                    break;
            }

            await db.SaveChangesAsync(null);
        }

        public async Task AddSite(string siteName, string countryName, string siteNumber)
        {
            var countries = await countryService.GetAll(Config.Defaults.ConfigurationVersions.InitialVersion.Id);
            var country = countries.FirstOrDefault(c => c.Name == countryName);

            if (countryName.Equals("United States"))
            {
                db.Sites.AddOrUpdate(s => s.Name,
                    new Site()
                    {
                        Id = Guid.NewGuid(),
                        CountryId = country.Id,
                        Name = siteName,
                        SiteNumber = siteNumber,
                        PrimaryContact = "ypadmin",
                        City = "Malvern",
                        State = "PA",
                        Address1 = "test Dr.",
                        PhoneNumber = "123-456-7890",
                        Zip = "19355",
                        TimeZone = "Eastern Standard Time",
                        Investigator = "Investigator " + siteName,
                        IsActive = true
                    });
            }

            else if (countryName.Equals("India"))
            {
                db.Sites.AddOrUpdate(s => s.Name,
                 new Site()
                 {
                     Id = Guid.NewGuid(),
                     CountryId = country.Id,
                     Name = siteName,
                     SiteNumber = siteNumber,
                     PrimaryContact = "ypadmin",
                     City = "Delhi",
                     State = "Delhi",
                     Address1 = "test",
                     PhoneNumber = "123-456-7890",
                     Zip = "19355",
                     TimeZone = "Indian Standard Time",
                     Investigator = "Investigator " + siteName,
                     IsActive = true
                 });
            }
            db.SaveChanges(null);

        }

        public async Task EnableWebBackupForSite(int days, string siteNumber)
        {
            var site = db.Sites.FirstOrDefault(s => s.SiteNumber == siteNumber);

            site.WebBackupExpireDate = GetSiteLocalTime(siteNumber).AddDays(days);
            await db.SaveChangesAsync(null);
        }

        public async Task AddUserWithNewRoleToSite(string userName, string roleName, string siteNumber)
        {
            var site = await db.Sites.FirstAsync(s => s.SiteNumber == siteNumber);
            var studyUser = await db.StudyUsers.FirstAsync(u => u.UserName == userName);
            var studyRole = await studyRoleService.GetAll();
            Guid studyRoleId = studyRole.FirstOrDefault(s => s.ShortName == roleName).Id;

            var existingStudyUserRole = await db.StudyUserRoles
                .FirstOrDefaultAsync(sur =>
                    sur.SiteId == site.Id &&
                    sur.StudyUserId == studyUser.Id &&
                    sur.StudyRoleId == studyRoleId);

            if (existingStudyUserRole == null)
            {
                db.StudyUserRoles.Add(new StudyUserRole
                {
                    SiteId = site.Id,
                    StudyUserId = studyUser.Id,
                    StudyRoleId = studyRoleId
                });

                await db.SaveChangesAsync(null);
            }
        }

        public string GetBCCidEmailRecipient(string subject)
        {
            Guid BCCId = Guid.Empty;
            var Id = db.EmailSents.FirstOrDefault(i => i.Subject == subject).Id;
            BCCId = db.EmailRecipients.FirstOrDefault(i => i.EmailSentId == Id).EmailRecipientTypeId;
            return BCCId.ToString();
        }

        public async Task AddSiteWithInactiveStatus(string siteName, string countryName, string siteNumber, string status)
        {
            var countries = await countryService.GetAll(Config.Defaults.ConfigurationVersions.InitialVersion.Id);
            var country = countries.FirstOrDefault(c => c.Name == countryName);

            if (countryName.Equals("United States"))
            {
                db.Sites.AddOrUpdate(s => s.Name,
                    new Site()
                    {
                        Id = Guid.NewGuid(),
                        CountryId = country.Id,
                        Name = siteName,
                        SiteNumber = siteNumber,
                        PrimaryContact = "ypadmin",
                        City = "Malvern",
                        State = "PA",
                        Address1 = "test Dr.",
                        PhoneNumber = "123-456-7890",
                        Zip = "19355",
                        TimeZone = "Eastern Standard Time",
                        Investigator = "Investigator " + siteName,
                        IsActive = false
                    });
            }

            else if (countryName.Equals("India"))
            {
                db.Sites.AddOrUpdate(s => s.Name,
                 new Site()
                 {
                     Id = Guid.NewGuid(),
                     CountryId = country.Id,
                     Name = siteName,
                     SiteNumber = siteNumber,
                     PrimaryContact = "ypadmin",
                     City = "Delhi",
                     State = "Delhi",
                     Address1 = "test",
                     PhoneNumber = "123-456-7890",
                     Zip = "19355",
                     TimeZone = "Indian Standard Time",
                     Investigator = "Investigator " + siteName,
                     IsActive = false
                 });
            }
            db.SaveChanges(null);
        }


        public void UpdateSiteActiveHistory(string siteNumber, string report = null)
        {
            var site = db.Sites.FirstOrDefault(s => s.SiteNumber == siteNumber);
            var id = site.Id;
            var SiteActiveHistory1 = new SiteActiveHistory
            {
                Id = Guid.NewGuid(),
                SiteId = id,
                Previous = false,
                Current = true,
            };
            db.SiteActiveHistory.Add(SiteActiveHistory1);
            var SiteActiveHistory2 = new SiteActiveHistory
            {
                Id = Guid.NewGuid(),
                SiteId = id,
                Previous = false,
                Current = true,
            };
            db.SiteActiveHistory.Add(SiteActiveHistory2);
            var SiteActiveHistory3 = new SiteActiveHistory
            {
                Id = Guid.NewGuid(),
                SiteId = id,
                Previous = true,
                Current = false,
            };
            db.SiteActiveHistory.Add(SiteActiveHistory3);
            db.SaveChanges(null);
            var sql1 = $"UPDATE [dbo].SiteActiveHistory SET ChangeDate = '{DateTime.Now}'";
            db.Database.ExecuteSqlCommand(sql1);
            if (report == "Visit Compliance")
            {
                var sql2 = $"UPDATE [dbo].SiteActiveHistory SET ChangeDate  = '{DateTime.Now.AddDays(-1).Date}' where Id = (select top 1 Id from SiteActiveHistory where Previous = 0 and [Current] = 1)";
                db.Database.ExecuteSqlCommand(sql2);
            }
        }

        public async Task AddUserToSite(string userMappingName, string roleName, string siteNumber)
        {
            var mappingUser = CommonData
                .UserMappings
                .First(u => u.MappingName == userMappingName);

            var site = await db.Sites.FirstAsync(s => s.SiteNumber == siteNumber);
            var studyUser = await db.StudyUsers.FirstAsync(u => u.UserName == mappingUser.Username);
            var studyRole = Config.Defaults.StudyRoles.Defaults.First(sr => sr.Value.ShortName == roleName).Value;

            var existingStudyUserRole = await db.StudyUserRoles
                .FirstOrDefaultAsync(sur =>
                    sur.SiteId == site.Id &&
                    sur.StudyUserId == studyUser.Id &&
                    sur.StudyRoleId == studyRole.Id);

            if (existingStudyUserRole == null)
            {
                db.StudyUserRoles.Add(new StudyUserRole
                {
                    SiteId = site.Id,
                    StudyUserId = studyUser.Id,
                    StudyRoleId = studyRole.Id
                });

                await db.SaveChangesAsync(null);
            }
        }



        public SoftwareVersion AddSoftwareVersion(string version, string packagePath = null)
        {
            var newVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = version,
                PackagePath = packagePath,
                PlatformTypeId = Guid.NewGuid()
            };

            db.SoftwareVersions.AddOrUpdate(
                s => s.VersionNumber,
                newVersion);

            db.SaveChanges(null);

            return newVersion;
        }

        public async Task AddAnswer(string patientNumber, string questionnaireName, string questionName, string answerValue, bool addToExisitingDiaryEntry = false)
        {
            var allquestionnaires = await questionnaireService.GetAllInflatedQuestionnaires();
            var questionnaire = allquestionnaires.Find(q => q.InternalName == questionnaireName);

            var patient = await db
               .Patients
               .FirstAsync(p => p.PatientNumber == patientNumber);

            QuestionChoiceModel choice = null;

            foreach (var p in questionnaire.Pages)
            {
                foreach (var q in p.Questions)
                {
                    var questionText = Regex.Replace(q.QuestionText, "<.*?>", "");
                    questionText = Regex.Replace(questionText, "&nbsp;", " ");
                    if (questionText == questionName)
                    {
                        foreach (var c in q.Choices)
                        {
                            var choiceText = Regex.Replace(c.DisplayText, "<.*?>", "");
                            choiceText = Regex.Replace(choiceText, "&nbsp;", " ");
                            if (choiceText == answerValue)
                            {
                                choice = c;
                                break;
                            }
                        }

                        Device device = default;
                        var patientDevice = patient.Devices.FirstOrDefault();
                        if (patientDevice != null)
                        {
                            device = db.Devices.FirstOrDefault(d => d.Id == patientDevice.Id);
                        }

                        DiaryEntry diaryEntry = null;
                        var diaryEntryDetails = db.DiaryEntries.Where(s => s.PatientId == patient.Id && s.QuestionnaireId == questionnaire.Id).ToList();
                        if (diaryEntryDetails.Count > 0)
                        {
                            foreach (var diary in diaryEntryDetails)
                            {
                                if (!diary.Answers.Select(a => q.QuestionnaireId).Contains(q.Id))
                                {
                                    diaryEntry = diary;
                                }
                            }

                            if(diaryEntry == null && addToExisitingDiaryEntry && diaryEntryDetails.Count == 1)
                            {
                                diaryEntry = diaryEntryDetails.First();
                            }
                        }

                        if (diaryEntry == null)
                        {
                            diaryEntry = new DiaryEntry
                            {
                                Id = Guid.NewGuid()
                            };
                        }

                        diaryEntry.PatientId = patient.Id;
                        diaryEntry.QuestionnaireId = questionnaire.Id;
                        diaryEntry.DataSourceId = DataSource.eCOAApp.Id;
                        diaryEntry.DiaryStatusId = DiaryStatus.Source.Id;
                        diaryEntry.DeviceId = device?.Id;
                        diaryEntry.ConfigurationId = patient.ConfigurationId;
                        diaryEntry.SoftwareVersionNumber = device?.SoftwareRelease?.SoftwareVersion?.VersionNumber;

                        if (q.InputFieldTypeId == InputFieldType.TemperatureSpinner.Id)
                        {
                            AddTemperatureAnswer(diaryEntry, q.Id, answerValue);
                        }
                        else if (q.InputFieldTypeId == InputFieldType.DateTime.Id ||
                            q.InputFieldTypeId == InputFieldType.Date.Id ||

                            q.InputFieldTypeId == InputFieldType.Time.Id ||
                            q.IsNumericValueQuestionType() ||
                            q.InputFieldTypeId == InputFieldType.TextArea.Id)

                        {

                            AddTextAnswer(diaryEntry, q.Id, answerValue);

                        }
                        else
                        {
                            AddChoiceAnswer(diaryEntry, choice, q.Id);
                        }


                        db.DiaryEntries.AddOrUpdate(diaryEntry);
                        db.SaveChanges(null);
                        break;
                    }
                }
            }
        }

        private void AddChoiceAnswer(DiaryEntry diaryEntry, QuestionChoiceModel choice, Guid questionId)
        {
            var choiceId = choice?.Id ?? Guid.Empty;

            if (diaryEntry.Answers == null)
            {
                diaryEntry.Answers = new List<Answer>();
            }

            var matchingAnswer = diaryEntry.Answers.FirstOrDefault(a => a.QuestionId == questionId);

            if (matchingAnswer != null)
            {
                matchingAnswer.ChoiceId = choiceId;
            }
            else
            {
                var newAnswer = new Answer
                {
                    Id = Guid.NewGuid(),
                    DiaryEntryId = diaryEntry.Id,
                    ChoiceId = choiceId,
                    QuestionId = questionId,
                    FreeTextAnswer = choice?.DisplayText
                };

                diaryEntry.Answers.Add(newAnswer);
            }
        }


        private void AddTextAnswer(DiaryEntry diaryEntry, Guid questionId, string value)

        {
            if (diaryEntry.Answers == null)
            {
                diaryEntry.Answers = new List<Answer>();
            }

            var matchingAnswer = diaryEntry.Answers.FirstOrDefault(a => a.QuestionId == questionId);

            if (matchingAnswer != null)
            {
                matchingAnswer.FreeTextAnswer = value;
            }
            else
            {
                var newAnswer = new Answer
                {
                    Id = Guid.NewGuid(),
                    DiaryEntryId = diaryEntry.Id,
                    ChoiceId = null,
                    QuestionId = questionId,
                    FreeTextAnswer = value
                };

                diaryEntry.Answers.Add(newAnswer);
            }
        }

        private void AddTemperatureAnswer(DiaryEntry diaryEntry, Guid questionId, string value)
        {
            diaryEntry.Answers = new List<Answer>();

            var isFahrenheit = value.ToUpper().Contains("F");

            // remove non numeric characters
            value = Regex.Replace(value, "[^0-9.]", "");

            var formattedValue = "0.0000";

            if (isFahrenheit)
            {
                formattedValue = TemperatureConversionHelper.ToCelsius(value).ToString(CommonData.TemperatureDatabaseFormat);
            }
            else
            {
                if (float.TryParse(value, out var parsedValue))
                {
                    formattedValue = parsedValue.ToString(CommonData.TemperatureDatabaseFormat);
                }
            }

            var tempAnswer = new Answer
            {
                Id = Guid.NewGuid(),
                DiaryEntryId = diaryEntry.Id,
                ChoiceId = null,
                QuestionId = questionId,
                FreeTextAnswer = formattedValue
            };

            diaryEntry.Answers.Add(tempAnswer);
        }

        public Guid GetLatestConfigId()
        {
            List<ConfigurationVersion> configurationVersions = Task.Run(async () =>
            {
                return await configurationVersionService.GetAll();
            }).Result;

            var latestConfig = configurationVersions
                .Where(cv => cv.Id != Config.Defaults.ConfigurationVersions.InitialVersion.Id)
                .OrderByDescending(cv => Version.Parse(cv.ConfigurationVersionNumber))
                .First();

            return latestConfig.Id;
        }


        public string getCaregiverForSubject(string subjectNumber)
        {
            Guid subjectId = db.Patients.FirstOrDefault(p => p.PatientNumber == subjectNumber).Id;
            Guid careGiver = db.CareGivers.FirstOrDefault(f => f.PatientId == subjectId).Id;
            return careGiver.ToString();
        }

        public CareGiver AddCaregiverForPatient(
            string caregiverTypeName,
            Guid patientId)
        {
            var caregiver = new CareGiver
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
            };

            db.CareGivers.Add(caregiver);

            db.SaveChanges(null);

            return caregiver;
        }

        public void AddDataCorrectionForPatient(
            string parameter, 
            string patientNumber, 
            string correctionUserMapping = CommonData.DefaultPortalUserMappingName)
        {
            var correctionUser = CommonData
                .UserMappings
                .First(m => m.MappingName == correctionUserMapping);

            var correctionStatusId = db.CorrectionStatuses.FirstOrDefault(i => i.TranslationKey == "Pending").Id;
            var correctionActionId = db.CorrectionActions.FirstOrDefault(i => i.TranslationKey == "PendingCorrection").Id;
            var correctionTypeId = correctionTypeService.GetAll().Result.FirstOrDefault(i => i.Name == "Change subject Information").Id;
            var patientId = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber).Id;
            var siteId = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber).SiteId;
            var configurationId = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber).ConfigurationId;
            var studyUserId = db.StudyUsers.FirstOrDefault(s => s.UserName == correctionUser.Username).Id;
            var patientAttributeConfigurationDetailId = subjectInformationService.GetAll().Result.FirstOrDefault(i => i.Name == parameter).Id;
            var patientAttributeOldData = db.PatientAttributes.FirstOrDefault(p => p.PatientId == patientId && p.PatientAttributeConfigurationDetailId == patientAttributeConfigurationDetailId).AttributeValue;
            string patientAttributeNewData = string.Empty;
            switch (parameter)
            {
                case "Height":
                    int heightAttribute = Convert.ToInt32(patientAttributeOldData.Replace(".00", ""));
                    patientAttributeNewData = (heightAttribute <= 100 && heightAttribute > 10) ? $"0{heightAttribute - 1}" : (heightAttribute - 1).ToString();
                    break;
                case "Weight":
                    int weightAttribute = Convert.ToInt32(patientAttributeOldData.Replace(".00", ""));
                    patientAttributeNewData = (weightAttribute <= 100 && weightAttribute > 10) ? $"0{weightAttribute - 1}" : (weightAttribute - 1).ToString();
                    break;
                case "Gender":
                    var genderChoices = subjectInformationService.GetAll().Result.FirstOrDefault(g => g.Name == "Gender").Choices;
                    Guid maleId = genderChoices.FirstOrDefault(i => i.Name == "Male").Id;
                    Guid femaleId = genderChoices.FirstOrDefault(i => i.Name == "Female").Id;
                    patientAttributeNewData = Guid.Parse(patientAttributeOldData) == maleId ? femaleId.ToString() : maleId.ToString();
                    break;
                case "Date of Birth":
                    patientAttributeNewData = Convert.ToDateTime(patientAttributeOldData).AddDays(-1).ToString();
                    break;
            }
            var correctionRowId = db.PatientAttributes.FirstOrDefault(i => i.PatientAttributeConfigurationDetailId == patientAttributeConfigurationDetailId).Id;
            var correctionId = Guid.NewGuid();
            var correctionWorkflowId = Guid.NewGuid();
            var correctionApprovalDataId = Guid.NewGuid();
            var correctionApprovalDataAdditionalId = Guid.NewGuid();
            var correction = new Correction
            {
                Id = correctionId,
                StartedDate = DateTime.Now,
                CompletedDate = null,
                StartedByUserId = studyUserId,
                CorrectionStatusId = correctionStatusId,
                CorrectionTypeId = correctionTypeId,
                CurrentWorkflowOrder = 1,
                PatientId = patientId,
                SiteId = siteId,
                DiaryEntryId = null,
                ReasonForCorrection = "Test",
                QuestionnaireId = null,
                NoApprovalNeeded = false,
                ConfigurationId = configurationId
            };
            var correctionWorkflow = new CorrectionWorkflow
            {
                Id = correctionWorkflowId,
                ApproverGroupId = Guid.Parse(CommonData.YpApproverGroupID),
                CorrectionId = correctionId,
                WorkflowOrder = 1,
                CorrectionActionId = correctionActionId,
                StudyUserId = studyUserId,
                WorkflowChangedDate = DateTime.Now
            };
            var correctionApprovalData = new CorrectionApprovalData
            {
                Id = correctionApprovalDataId,
                CorrectionId = correctionId,
                RowId = correctionRowId,
                TableName = "PatientAttribute",
                ColumnName = "AttributeValue",
                TranslationKey = parameter,
                OldDataPoint = null,
                NewDataPoint = patientAttributeNewData,
                OldDisplayValue = patientAttributeOldData,
                NewDisplayValue = patientAttributeNewData
            };
            var correctionApprovalDataAdditional = new CorrectionApprovalDataAdditional
            {
                Id = correctionApprovalDataAdditionalId,
                CorrectionApprovalDataId = correctionApprovalDataId,
                ColumnName = null,
                ColumnValue = patientAttributeConfigurationDetailId.ToString(),
                IgnorePropertyUpdate = true
            };
            db.Corrections.Add(correction);
            db.CorrectionWorkflows.Add(correctionWorkflow);
            db.CorrectionApprovalDatas.Add(correctionApprovalData);
            db.CorrectionApprovalDataAdditionals.Add(correctionApprovalDataAdditional);
            db.SaveChanges(null);
        }
        public void AddMultipleDataCorrectionsForPatient(
            Table table, 
            string correctionUserMapping = CommonData.DefaultPortalUserMappingName)
        {
            var correctionUser = CommonData
                .UserMappings
                .First(m => m.MappingName == correctionUserMapping);

            string oldDataPoint = string.Empty;
            string newDataPoint = string.Empty;
            string oldDisplayValue = string.Empty;
            string newDisplayValue = string.Empty;
            var correctionRowId = Guid.Empty;
            var correctionTypeId = Guid.Empty;
            var patientAttributeConfigurationDetailId = Guid.Empty;
            foreach (var row in table.Rows)
            {
                var patientNumber = row["Patient No"];
                var dcfType = row["DCF Type"];
                var dcfStatus = row["DCF Status"];
                var dcfAction = row["DCF Action"];
                var dcfOpenDate = row["DCF Opened Date"];
                var dcfCloseDate = row["DCF Closed Date"];
                var translationKey = row["Translation Key"];
                var correctionReason = row["Reason For Correction"];
                var tableName = row["Table Name"];
                var columnName = row["Column Name"];

                var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber);
                var patientId = patient.Id;
                var diaryEntries = db.DiaryEntries.FirstOrDefault(d => d.PatientId == patientId);
                var patientVisits = db.PatientVisits.FirstOrDefault(v => v.PatientId == patientId);
                var siteId = patient.SiteId;
                var configurationId = patient.ConfigurationId;
                var studyUserId = db.StudyUsers.FirstOrDefault(s => s.UserName == correctionUser.Username).Id;
                var diaryEntryId = Guid.Empty;
                foreach (var correction in CommonData.correctionTypes)
                {

                    if (correction.Key == dcfType)
                    {
                        correctionTypeId = Guid.Parse(correction.Value);
                        break;
                    }
                }
                switch (row["DCF Type"])
                {
                    case CommonData.ChangeSubjectInformation:
                        foreach (var translation in CommonData.translationKeys)
                        {

                            if (translation.Key == translationKey)
                            {
                                patientAttributeConfigurationDetailId = Guid.Parse(translation.Value);
                                break;
                            }
                        }
                        oldDisplayValue = db.PatientAttributes.FirstOrDefault(p => p.PatientId == patientId && p.PatientAttributeConfigurationDetailId == patientAttributeConfigurationDetailId).AttributeValue;
                        switch (translationKey)
                        {
                            case "Height":
                                int heightAttribute = Convert.ToInt32(oldDisplayValue.Replace(".00", ""));
                                newDataPoint = (heightAttribute <= 100 && heightAttribute > 10) ? $"0{heightAttribute - 1}" : (heightAttribute - 1).ToString();
                                break;
                            case "Weight":
                                int weightAttribute = Convert.ToInt32(oldDisplayValue.Replace(".00", ""));
                                newDataPoint = (weightAttribute <= 100 && weightAttribute > 10) ? $"0{weightAttribute - 1}" : (weightAttribute - 1).ToString();
                                break;
                            case "Gender":
                                var genderChoices = subjectInformationService.GetAll().Result.FirstOrDefault(g => g.Name == "Gender").Choices;
                                Guid maleId = genderChoices.FirstOrDefault(i => i.Name == "Male").Id;
                                Guid femaleId = genderChoices.FirstOrDefault(i => i.Name == "Female").Id;
                                newDataPoint = Guid.Parse(oldDisplayValue) == maleId ? femaleId.ToString() : maleId.ToString();
                                break;
                            case "Date of Birth":
                                newDataPoint = Convert.ToDateTime(oldDisplayValue).AddDays(-1).ToString();
                                break;
                        }
                        newDisplayValue = newDataPoint;
                        correctionRowId = db.PatientAttributes.FirstOrDefault(i => i.PatientAttributeConfigurationDetailId == patientAttributeConfigurationDetailId).Id;
                        oldDataPoint = null;
                        break;
                    case CommonData.RemoveASubject:
                        oldDataPoint = db.Patients.FirstOrDefault(a => a.PatientNumber == patientNumber).PatientStatusTypeId.ToString();
                        correctionRowId = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber).Id;
                        newDataPoint = "99";
                        oldDisplayValue = "Screened";
                        newDisplayValue = "Removed";
                        break;
                    case CommonData.MergeSubjects:
                        correctionRowId = db.Patients.FirstOrDefault(p => p.PatientNumber == patientNumber).Id;
                        oldDataPoint = patientNumber;
                        newDataPoint = patientNumber;
                        oldDisplayValue = patientNumber.Split('-')[2].ToString();
                        newDisplayValue = patientNumber.Split('-')[2].ToString();
                        break;
                    case CommonData.ChangeQuestionnaireResponses:
                        diaryEntryId = diaryEntries.Id;
                        correctionRowId = db.Answers.FirstOrDefault(a => a.DiaryEntryId == diaryEntryId).QuestionId;
                        oldDataPoint = db.Answers.FirstOrDefault(a => a.DiaryEntryId == diaryEntryId).ChoiceId.ToString();
                        newDataPoint = CommonData.QuestionResponseMuchBetterId;
                        oldDisplayValue = "Not Better";
                        newDisplayValue = "Much Better";
                        break;
                    case CommonData.ChangeQuestionnaireInformation:
                        diaryEntryId = diaryEntries.Id;
                        var oldDiarydate = diaryEntries.DiaryDate.ToString("dd-MMM-yyyy");
                        var newDiarydate = Convert.ToDateTime(oldDiarydate).AddDays(+1).ToString("dd-MMM-yyyy");
                        correctionRowId = db.DiaryEntries.FirstOrDefault(v => v.PatientId == patientId).Id;
                        oldDataPoint = diaryEntries.DiaryDate.ToString("dd-MMM-yyyy");
                        newDataPoint = Convert.ToDateTime(oldDiarydate).AddDays(+1).ToString("dd-MMM-yyyy");
                        oldDisplayValue = diaryEntries.DiaryDate.ToString("dd-MMM-yyyy");
                        newDisplayValue = Convert.ToDateTime(oldDiarydate).AddDays(+1).ToString("dd-MMM-yyyy");
                        break;
                    case CommonData.AddPaperQuestionnaire:
                        diaryEntryId = diaryEntries.Id;
                        correctionRowId = db.Answers.FirstOrDefault(a => a.DiaryEntryId == diaryEntryId).QuestionId;
                        oldDataPoint = null;
                        newDataPoint = "d066b9e2-1a25-47ad-81e0-04560c045293";
                        oldDisplayValue = null;
                        newDisplayValue = "Much Better";
                        break;
                    case CommonData.ChangeSubjectVisit:
                        correctionRowId = db.PatientVisits.FirstOrDefault(v => v.PatientId == patientId).Id;
                        oldDataPoint = patientVisits.PatientVisitStatusTypeId.ToString();
                        newDataPoint = "2";
                        oldDisplayValue = "Not Started";
                        newDisplayValue = "In Progress";
                        break;
                }

                var correctionStatusId = db.CorrectionStatuses.FirstOrDefault(i => i.TranslationKey == dcfStatus).Id;
                var correctionActionId = db.CorrectionActions.FirstOrDefault(i => i.TranslationKey == dcfAction).Id;
                SetCorrectionTableForDCF(studyUserId, correctionStatusId, correctionTypeId, patientId, siteId, diaryEntryId, configurationId, correctionReason, dcfOpenDate, dcfCloseDate, dcfType);
                SetCorrectionWorkflowTableForDCF(correctionActionId, studyUserId, dcfCloseDate, correctionReason);
                SetCorrectionApprovalDataTableForDCF(correctionRowId, tableName, columnName, translationKey, oldDataPoint, newDataPoint, oldDisplayValue, newDisplayValue, correctionReason);
            }
        }


        public DiaryEntry AddDiaryEntryForPatient(Guid patientId)
        {
            var diaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                QuestionnaireId = Guid.Empty,
                DataSourceId = DataSource.eCOAApp.Id,
                DiaryStatusId = DiaryStatus.Source.Id,
                DiaryDate = DateTime.UtcNow.Date
            };

            db.DiaryEntries.Add(diaryEntry);

            db.SaveChanges(null);

            return diaryEntry;
        }

        public Answer AddAnswerForDiaryEntry(Guid diaryEntryId)
        {
            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                DiaryEntryId = diaryEntryId,
                FreeTextAnswer = $"Answer inserted by {nameof(AddAnswerForDiaryEntry)}"
            };

            db.Answers.Add(answer);

            db.SaveChanges(null);

            return answer;
        }

        public void SetCorrectionTableForDCF(Guid studyUserId, Guid correctionStatusId, Guid correctionTypeId, Guid patientId, Guid siteId, Guid diaryEntryId, Guid configurationId, string correctionReason, string dcfOpenDate, string dcfCloseDate, string dcfType)
        {
            var correctionId = Guid.NewGuid();
            var correction = new Correction
            {
                Id = correctionId,
                StartedByUserId = studyUserId,
                CorrectionStatusId = correctionStatusId,
                CorrectionTypeId = correctionTypeId,
                CurrentWorkflowOrder = 1,
                PatientId = patientId,
                SiteId = siteId,
                NoApprovalNeeded = false,
                ConfigurationId = configurationId,
                ReasonForCorrection = correctionReason
            };
            if (dcfOpenDate.ToString().Contains("Current date") && dcfCloseDate.ToString().Contains("Current date"))
            {
                correction.StartedDate = DateTime.Now;
                correction.CompletedDate = DateTime.Now;
            }
            else if (dcfOpenDate.ToString().Contains("Current date") && dcfCloseDate.ToString().Contains("N/A"))
            {
                correction.StartedDate = DateTime.Now;
                correction.CompletedDate = null;
            }
            else
            {
                correction.StartedDate = Convert.ToDateTime(dcfOpenDate);
                correction.CompletedDate = Convert.ToDateTime(dcfCloseDate);
            }

            switch (dcfType)
            {
                case CommonData.ChangeQuestionnaireResponses:
                    correction.QuestionnaireId = Guid.Parse(CommonData.QuestionnaireFormsId);
                    correction.DiaryEntryId = diaryEntryId;
                    break;
                case CommonData.AddPaperQuestionnaire:
                    correction.QuestionnaireId = Guid.Parse(CommonData.QuestionnaireFormsId);
                    correction.DiaryEntryId = diaryEntryId;
                    break;
                case CommonData.ChangeQuestionnaireInformation:
                    correction.DiaryEntryId = diaryEntryId;
                    break;
                default:
                    correction.QuestionnaireId = null;
                    correction.DiaryEntry = null;
                    break;
            }
            db.Corrections.Add(correction);
            db.SaveChanges(null);
        }

        public void SetCorrectionWorkflowTableForDCF(Guid correctionActionId, Guid studyUserId, string dcfCloseDate, string correctionReason)
        {
            var correctionWorkflowId = Guid.NewGuid();
            var correctionId = db.Corrections.FirstOrDefault(i => i.ReasonForCorrection == correctionReason).Id;
            var correctionWorkflow = new CorrectionWorkflow
            {
                Id = correctionWorkflowId,
                ApproverGroupId = Guid.Parse(CommonData.YpApproverGroupID),
                CorrectionId = correctionId,
                WorkflowOrder = 1,
                CorrectionActionId = correctionActionId,
                StudyUserId = studyUserId,

            };
            if (dcfCloseDate.ToString().Contains("Current date") || dcfCloseDate.ToString().Contains("N/A"))
            {
                correctionWorkflow.WorkflowChangedDate = DateTime.Now;
            }
            else
            {
                correctionWorkflow.WorkflowChangedDate = Convert.ToDateTime(dcfCloseDate);
            }
            db.CorrectionWorkflows.Add(correctionWorkflow);
            db.SaveChanges(null);

        }

        public void SetCorrectionApprovalDataTableForDCF(Guid correctionRowId, string tableName, string columnName, string translationKey, string oldDataPoint, string newDataPoint, string oldDisplayValue, string newDisplayValue, string correctionReason)
        {
            var correctionApprovalDataId = Guid.NewGuid();
            var correctionId = db.Corrections.FirstOrDefault(i => i.ReasonForCorrection == correctionReason).Id;
            var correctionApprovalData = new CorrectionApprovalData
            {
                Id = correctionApprovalDataId,
                CorrectionId = correctionId,
                RowId = correctionRowId,
                TableName = tableName,
                ColumnName = columnName,
                TranslationKey = translationKey,
                OldDataPoint = oldDataPoint,
                NewDataPoint = newDataPoint,
                OldDisplayValue = oldDisplayValue,
                NewDisplayValue = newDisplayValue
            };
            db.CorrectionApprovalDatas.Add(correctionApprovalData);
            db.SaveChanges(null);
        }


        public void SetupSessionService(string user = null)
        {
            List<ConfigurationVersion> configurationVersions = Task.Run(async () =>
            {
                return await configurationVersionService.GetAll();
            }).Result;

            var latestConfig = configurationVersions
                .Where(cv => cv.Id != Config.Defaults.ConfigurationVersions.InitialVersion.Id)
                .OrderByDescending(cv => Version.Parse(cv.ConfigurationVersionNumber))
                .First();

            sessionService.UserConfigurationId = latestConfig.Id;
            sessionService.User = user;
        }

        public int GetSubjectCountInEmailSent(string subject)
        {
            var count = db.EmailSents.Where(x => x.Subject == subject).Select(x => x.Subject).Count();
            return count;
        }

        public List<string> GetEmailIdFromEmailRecipients()
        {
            var listOfEmailRecipients = db.EmailRecipients.Select(x => x.EmailAddress).Distinct().ToList();
            return listOfEmailRecipients;
        }

        public void AddBaseSoftwareRelease()
        {
            var releases = db.SoftwareReleases.ToList();

            var configs = Task.Run(async () =>
            {
                var configVersions = await configurationVersionService.GetAll();
                return configVersions;
            }).Result;

            var latestConfig = configs
                .OrderByDescending(c => Version.Parse(c.ConfigurationVersionNumber))
                .FirstOrDefault();

            if (!releases.Any(r => r.ConfigurationId == latestConfig.Id))
            {
                var versions = db
                    .SoftwareVersions
                    .ToList();

                var latestVersion = versions
                    .OrderByDescending(sv => sv.VersionNumber)
                    .First();

                var newRelease = new SoftwareRelease
                {
                    Id = Guid.NewGuid(),
                    Name = "Study Wide Release",
                    StudyWide = true,
                    IsActive = true,
                    SoftwareVersionId = latestVersion.Id,
                    ConfigurationId = latestConfig.Id,
                    ConfigurationVersion = latestConfig.ConfigurationVersionNumber,
                    SRDVersion = latestConfig.SrdVersion,
                    DateCreated = DateTime.Now
                };

                db.SoftwareReleases.Add(newRelease);

                db.SaveChanges(null);
            }
        }

        public SoftwareRelease AddSoftwareRelease(
            Guid softwareVersionId,
            string fullConfigVersion,
            bool isStudyWide = true)
        {
            var version = db.SoftwareVersions.First(sv => sv.Id == softwareVersionId);

            var configs = Task.Run(async () =>
            {
                var configVersions = await configurationVersionService.GetAll();
                return configVersions;
            }).Result;

            var configVersionParts = fullConfigVersion.Split("-");

            var parsedConfigVersionNumber = configVersionParts.First();
            var parsedSrdNumver = configVersionParts.Last();

            var matchingConfigVersion = configs
                .First(c => c.ConfigurationVersionNumber.Trim() == parsedConfigVersionNumber.Trim());

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                Name = $"Release {version.VersionNumber}",
                StudyWide = isStudyWide,
                IsActive = true,
                SoftwareVersionId = version.Id,
                ConfigurationId = matchingConfigVersion.Id,
                ConfigurationVersion = parsedConfigVersionNumber,
                SRDVersion = parsedSrdNumver,
                DateCreated = DateTime.Now
            };

            db.SoftwareReleases.Add(newRelease);

            db.SaveChanges(null);

            return newRelease;
        }

        public SoftwareRelease GetSoftwareRelease(Guid Id)
        {
            var release = db.SoftwareReleases.FirstOrDefault(s => s.Id == Id);
            return release;
        }

        public void CacheSystemActionStudyRoles()
        {
            db.Database.ExecuteSqlCommand(@"DELETE FROM [dbo].[SystemActionStudyRole]");
            db.Database.ExecuteSqlCommand(@$"INSERT INTO [dbo].[SystemActionStudyRole] (SystemActionId, StudyRoleId, LastModified)
                    SELECT Id, 'DF84A2A4-1BF2-E611-80D8-000D3A1029A9', GETDATE()
                    FROM[dbo].[SystemAction]");

            _systemActionStudyRoles = db.SystemActionStudyRoles.ToList();
        }

        public void ResetSystemActionStudyRoles()
        {
            List<Guid> _systemActions = db.SystemActions.Select(a => a.Id).Distinct().ToList();
            List<Guid> _studyUserRoles = db.StudyUserRoles.Select(r => r.StudyRoleId).Distinct().ToList();
            _systemActionStudyRoles = db.SystemActionStudyRoles.ToList();
            if (_systemActionStudyRoles.Count < (_systemActions.Count * _studyUserRoles.Count))
            {
                db.SystemActionStudyRoles.RemoveRange(db.SystemActionStudyRoles);
                var allRoles = _systemActions.SelectMany(actions => _studyUserRoles, (action, userRole) => new { action, userRole });
                List<SystemActionStudyRole> systemActionStudyRoles = new List<SystemActionStudyRole>();
                foreach (var element in allRoles)
                {
                    systemActionStudyRoles.Add(new SystemActionStudyRole { SystemActionId = element.action, StudyRoleId = element.userRole });
                }
                db.SystemActionStudyRoles.AddRange(systemActionStudyRoles);
                db.SaveChanges(null);
            }
            //db.SystemActionStudyRoles.RemoveRange(db.SystemActionStudyRoles);
            //db.SaveChanges(null);
            //db.SystemActionStudyRoles.AddRange(_systemActionStudyRoles);
            //db.SaveChanges(null);
        }

        public void GiveTestUsersAllSystemActions()
        {
            var allSystemActionIds = db
                .SystemActions
                .Select(a => a.Id)
                .Distinct()
                .ToList();

            foreach (var user in CommonData.UserMappings)
            {
                var matchingStudyUser = db
                    .StudyUsers
                    .FirstOrDefault(su => su.UserName == user.Username);

                if (matchingStudyUser == null)
                {
                    continue;
                }

                var role = db
                    .StudyUserRoles
                    .FirstOrDefault(sur => sur.StudyUserId == matchingStudyUser.Id);

                if (role == null)
                {
                    continue;
                }

                var assignedActionIds = db
                    .SystemActionStudyRoles
                    .Where(sa => sa.StudyRoleId == role.Id)
                    .Select(sa => sa.SystemActionId)
                    .Distinct()
                    .ToList();

                var systemActionStudyRolesToAdd = allSystemActionIds
                    .Where(a => !assignedActionIds.Contains(a))
                    .Select(a => new SystemActionStudyRole
                    {
                        SystemActionId = a,
                        StudyRoleId = role.Id

                    })
                    .ToList();

                db.SystemActionStudyRoles.AddRange(systemActionStudyRolesToAdd);
            }

            db.SaveChanges(null);
        }

        public void CleanupE2EData()
        {
            // ef removals
            db.SyncLogs.RemoveRange(db.SyncLogs);
            db.DeviceDatas.RemoveRange(db.DeviceDatas);
            db.Devices.RemoveRange(db.Devices);
            db.Answers.RemoveRange(db.Answers);
            db.DiaryEntries.RemoveRange(db.DiaryEntries);
            db.Devices.RemoveRange(db.Devices);
            db.CorrectionApprovalDataAdditionals.RemoveRange(db.CorrectionApprovalDataAdditionals);
            db.CorrectionApprovalDatas.RemoveRange(db.CorrectionApprovalDatas);
            db.CorrectionWorkflows.RemoveRange(db.CorrectionWorkflows);
            db.Corrections.RemoveRange(db.Corrections);
            db.CorrectionDiscussions.RemoveRange(db.CorrectionDiscussions);
            db.PatientAttributes.RemoveRange(db.PatientAttributes);
            db.PatientVisits.RemoveRange(db.PatientVisits);
            db.CareGivers.RemoveRange(db.CareGivers);
            db.Patients.RemoveRange(db.Patients);
            db.Exports.RemoveRange(db.Exports);
            db.SoftwareReleaseCountry.RemoveRange(db.SoftwareReleaseCountry);
            db.SoftwareReleaseDeviceTypes.RemoveRange(db.SoftwareReleaseDeviceTypes);
            db.NotificationRequests.RemoveRange(db.NotificationRequests);
            db.SiteActiveHistory.RemoveRange(db.SiteActiveHistory);
            db.AnalyticsReferences.RemoveRange(db.AnalyticsReferences);

            var initialSite = db.Sites.FirstOrDefault(s => s.Name == InitialSiteName);

            if (initialSite != null)
            {
                db.SiteLanguages.RemoveRange(db.SiteLanguages.Where(sl => sl.SiteId != initialSite.Id));
                db.StudyUserRoles.RemoveRange(db.StudyUserRoles.Where(sur => sur.SiteId != initialSite.Id));
            }

            db.EmailContentStudyRoles.RemoveRange(db.EmailContentStudyRoles);
            db.EmailRecipients.RemoveRange(db.EmailRecipients);
            db.EmailSents.RemoveRange(db.EmailSents);

            //db.SoftwareReleases.FirstOrDefault(x => x.Name == "Initial Software Release").IsActive = true;

            db.SaveChanges(null);

            // sql removals
            db.Database.ExecuteSqlCommand($"DELETE FROM [dbo].{nameof(ReferenceMaterial)}");
            db.Database.ExecuteSqlCommand($"DELETE FROM [dbo].[SoftwareReleaseSite];");

            var softwareReleasesToDelete = db.SoftwareReleases
                .Where(sr => sr.Name != "Initial Software Release")
                .ToList();

            if (softwareReleasesToDelete.Any())
            {
                var releaseIds = softwareReleasesToDelete.Select(r => $"'{r.Id}'");
                var releaseSql = $"DELETE FROM [dbo].[{nameof(SoftwareRelease)}] WHERE Id IN ({string.Join(",", releaseIds)})";
                db.Database.ExecuteSqlCommand(releaseSql);
            }

            var versionsToDelete = db.SoftwareVersions
                .Where(sv => sv.VersionNumber != "0.0.0.1")
                .ToList();

            if (versionsToDelete.Any())
            {
                var versionIds = versionsToDelete.Select(r => $"'{r.Id}'");
                var versionSql = $"DELETE FROM [dbo].[{nameof(SoftwareVersion)}] WHERE Id IN ({string.Join(",", versionIds)})";
                db.Database.ExecuteSqlCommand(versionSql);
            }

            db.Database.ExecuteSqlCommand($"DELETE FROM [dbo].{nameof(SiteActiveHistory)}");

            var sitesToDelete = db.Sites
                .Where(s => s.Name != InitialSiteName)
                .ToList();

            sitesToDelete.ForEach(s => s.SoftwareReleases = null);

            if (sitesToDelete.Any())
            {
                var siteIds = sitesToDelete.Select(r => $"'{r.Id}'");
                var siteSql = $"DELETE FROM [dbo].[{nameof(Site)}] WHERE Id IN ({string.Join(",", siteIds)})";
                db.Database.ExecuteSqlCommand(siteSql);
            }

            db.Database.ExecuteSqlCommand($"DELETE FROM [ypaudit].StudyUserRole");

            // Bad test data leaving Id null and messing tests up. 
            db.Database.ExecuteSqlCommand($"DELETE FROM [dbo].StudyUser where Id = '{Guid.Empty}'");

            SeedSoftwareReleaseTable();
        }

        public void SeedSoftwareReleaseTable()
        {
            var versionId = Guid.Parse(CommonData.VersionID);
            if (db.SoftwareVersions.FirstOrDefault(sv => sv.VersionNumber == CommonData.IntialSoftwareReleaseVersionNumber) == null)
            {
                var newVersion = new SoftwareVersion
                {
                    Id = versionId,
                    VersionNumber = CommonData.IntialSoftwareReleaseVersionNumber,
                    PlatformTypeId = Guid.NewGuid()
                };

                db.SoftwareVersions.AddOrUpdate(
                    s => s.VersionNumber,
                    newVersion);

                db.SaveChanges(null);
            }

            else
            {
                versionId = db.SoftwareVersions.FirstOrDefault(sv => sv.VersionNumber == CommonData.IntialSoftwareReleaseVersionNumber).Id;
            }


            if (db.SoftwareReleases.FirstOrDefault(s => s.Name == CommonData.InitialSoftwareRelease) == null)
            {
                var newRelease = new SoftwareRelease
                {
                    Id = Guid.Parse(CommonData.IntialSoftwareReleaseVersionID),
                    SoftwareVersionId = versionId,
                    Name = CommonData.InitialSoftwareRelease,
                    IsActive = true,
                    StudyWide = true,
                    ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id,
                    ConfigurationVersion = Config.Defaults.ConfigurationVersions.InitialVersion.ConfigurationVersionNumber,
                    DateCreated = DateTime.Now
                };
                db.SoftwareReleases.AddOrUpdate(newRelease);
                db.SaveChanges(null);
            }
            else
            {
                db.SoftwareReleases.FirstOrDefault(x => x.Name == "Initial Software Release").IsActive = true;
                db.SaveChanges(null);
            }
        }

        public async Task<Patient> GetPatientById(Guid? patientId)
        {
            var patient = await db
                .Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId);

            return patient;
        }

        public async Task<Guid?> GetPatientId(string patientNumber)
        {
            var patient = await db
                .Patients
                .FirstOrDefaultAsync(p => p.PatientNumber == patientNumber);

            return patient?.Id;
        }

        public async Task<string> GetNextSubjectNumber()
        {
            var fullHighestPatientNumber = await db
                .Patients
                .OrderByDescending(p => p.PatientNumber)
                .Select(p => p.PatientNumber)
                .FirstOrDefaultAsync();

            var numberSegment = fullHighestPatientNumber?.Split('-').Last() ?? string.Empty;

            var result = "001";

            if (int.TryParse(numberSegment, out var parsedNumber))
            {
                result = (parsedNumber + 1).ToString("000");
            }

            return result;
        }

        public async Task<List<SubjectInformationModel>> GetSubjectInformationModelsForVersion(string configVersion)
        {
            var allVersions = await configurationVersionService
                .GetAll();

            var version = configVersion.ToUpper() == WorkingVersionName.ToUpper()
                ? null
                : allVersions.First(v => v.DisplayVersion == configVersion);

            var subjectInfos = await subjectInformationService
                .GetAll(version?.Id);

            return subjectInfos;
        }

        public async Task<CorrectionWorkflowSettingsModel> GetCorrectionWorkflowForVersion(string configVersion, string dcfType)
        {
            var allVersions = await configurationVersionService
                .GetAll();

            var version = configVersion.ToUpper() == WorkingVersionName.ToUpper()
                ? null
                : allVersions.First(v => v.DisplayVersion == configVersion);

            var correctionTypes = await correctionTypeService
                .GetAll(version?.Id);

            CorrectionTypeModel matchingCorrectionType = null;

            foreach (var correctionType in correctionTypes)
            {
                if (correctionType.Name.Trim().ToLower() == dcfType.Trim().ToLower())
                {
                    matchingCorrectionType = correctionType;
                    break;
                }
            }

            var result = await correctionWorkflowService
                .Get(
                    matchingCorrectionType?.Id ?? Guid.Empty,
                    version?.Id);

            return result;
        }

        public List<SystemAction> GetSystemActionIds(ICollection<string> systemActionNames = null)
        {
            var systemActions = systemActionNames == null
                ? db.SystemActions.ToList()
                : db.SystemActions.Where(s => systemActionNames.Contains(s.Name)).ToList();

            return systemActions;
        }

        public async Task<string> GetStudySettingValue(string studySettingName)
        {
            var studySettings = await studySettingService.GetAll();

            var matchingSetting = studySettings.FirstOrDefault(ss => ss.Key == studySettingName);

            return matchingSetting?.Value;
        }

        public async Task<List<PatientStatusModel>> GetPatientStatuses(
            string configVersion = WorkingVersionName)
        {
            var allVersions = await configurationVersionService
                .GetAll();

            var version = configVersion.ToUpper() == WorkingVersionName.ToUpper()
                ? null
                : allVersions.First(v => v.DisplayVersion == configVersion);

            var result = await patientStatusService.GetAll(version?.Id);

            return result;
        }

        public async Task EnablePermissionForRole(string permissionName, string roleName = "YP")
        {
            var roles = await studyRoleService
                .GetAll();

            var selectedRole = roles
                .FirstOrDefault(r => r.ShortName.ToUpper() == roleName.ToUpper());

            var permission = await db
                .SystemActions
                .FirstOrDefaultAsync(sa => sa.Description.ToUpper() == permissionName.ToUpper());

            var rolePermission = await db
                .SystemActionStudyRoles
                .FirstOrDefaultAsync(sasr =>
                    sasr.StudyRoleId == selectedRole.Id &&
                    sasr.SystemActionId == permission.Id);

            if (rolePermission == null)
            {
                var newPermission = new SystemActionStudyRole
                {
                    SystemActionId = permission.Id,
                    StudyRoleId = selectedRole.Id
                };

                db.SystemActionStudyRoles.Add(newPermission);

                await db.SaveChangesAsync(null);
            }
        }

        public async Task DeleteUserFromStudy(string userName, string roleName, string siteNumber)
        {
            var site = await db.Sites.FirstAsync(s => s.SiteNumber == siteNumber);
            var studyUser = await db.StudyUsers.FirstAsync(u => u.UserName == userName);
            var studyRole = await studyRoleService.GetAll();
            Guid studyRoleId = studyRole.FirstOrDefault(s => s.ShortName == roleName).Id;

            var existingStudyUserRole = await db.StudyUserRoles
                .FirstOrDefaultAsync(sur =>
                    sur.SiteId == site.Id &&
                    sur.StudyUserId == studyUser.Id &&
                    sur.StudyRoleId == studyRoleId);

            if (existingStudyUserRole != null)
            {
                db.StudyUserRoles.Remove(existingStudyUserRole);

                await db.SaveChangesAsync(null);
            }
        }

        public async Task DisablePermissionForRole(string permissionName, string roleName = "YP")
        {
            var roles = await studyRoleService
                .GetAll();

            var selectedRole = roles
                .FirstOrDefault(r => r.ShortName.ToUpper() == roleName.ToUpper());

            var permission = await db
                .SystemActions
                .FirstOrDefaultAsync(sa => sa.Description.ToUpper() == permissionName.ToUpper());

            var rolePermission = await db
                .SystemActionStudyRoles
                .FirstOrDefaultAsync(sasr =>
                    sasr.StudyRoleId == selectedRole.Id &&
                    sasr.SystemActionId == permission.Id);

            if (rolePermission != null)
            {
                db.SystemActionStudyRoles.Remove(rolePermission);

                await db.SaveChangesAsync(null);
            }
        }

        public int GetAuditRecord(string modifiedBy, Guid studyUserId, Guid studyRoleId, Guid siteId, string auditAction)
        {
            var sql = $"SELECT COUNT(*) AS TotalRows FROM [ypaudit].StudyUserRole where ModifiedBy = '{modifiedBy}' and StudyUserId = '{studyUserId}' and StudyRoleId = '{studyRoleId}' and SiteId = '{siteId}' and AuditAction = '{auditAction}'";
            var auditRecord = db.Database.SqlQuery<int>(sql).FirstOrDefault();

            return auditRecord;
        }

        public void SetGridData(string tableName)
        {
            switch (tableName)

            {
                case "CareGivers":
                    var name = db.CareGivers.FirstOrDefault();
                    name.IsHandheldTrainingComplete = true;
                    name.IsTabletTrainingComplete = true;
                    name.LockoutEnabled = true;
                    db.CareGivers.AddOrUpdate(name);
                    break;

            }
            db.SaveChanges(null);
        }

        public StudyUser GetStudyUser(string userName)
        {
            var studyUser = db.StudyUsers.FirstOrDefault(su => su.UserName == userName);
            return studyUser;
        }


        public List<string> GetMatchingSessionStudyUserIds()
        {
            if (string.IsNullOrWhiteSpace(sessionService.User))
                return new List<string>();
            return db.StudyUsers.AsNoTracking()
                .Where(su => su.UserName == sessionService.User)
                .Select(su => su.Id.ToString()).ToList();
        }

        public async Task<StudyRoleModel> GetRole(string roleName)
        {
            var roles = await studyRoleService.GetAll();
            var selectedRole = roles.FirstOrDefault(r => r.ShortName.ToUpper() == roleName.ToUpper());

            return selectedRole;
        }

        public async Task<IEnumerable<NotificationRequest>> GetNotificationRequests()
        {
            var requests = await db.NotificationRequests.AsNoTracking().ToListAsync();
            return requests;
        }

        public string GetPatientEnrolledDate(string patientNumber)
        {
            string enrolledDate = db.Patients.Where(p => p.PatientNumber == patientNumber).Select(p => p.EnrolledDate).First().ToString();
            return enrolledDate;
        }

        public CorrectionWorkflow GetLatestCorrectionWorkflow()
        {
            var allCorrectionWorkflows = db.CorrectionWorkflows.AsNoTracking().ToList();
            var correctionWorkflow = allCorrectionWorkflows.LastOrDefault();
            return correctionWorkflow;
        }

        public SyncLog GetLatestSyncLog()
        {
            var allSyncLogs = db.SyncLogs.AsNoTracking().ToList();
            var syncLog = allSyncLogs.LastOrDefault();
            return syncLog;
        }

        public async Task<List<LanguageModel>> GetAllLanguages()
        {
            return await this.languageService.GetAll();
        }

        public string GetExportGridDate(string FieldName)
        {
            string ExportdateTime = "";
            if (FieldName == "Created DateTime")
            {
                ExportdateTime = db.Exports.Select(p => p.CreatedTime).First().ToString();
            }
            if (FieldName == "Completed DateTime")
            {
                ExportdateTime = db.Exports.Select(p => p.CompletedTime).First().ToString();
            }
            if (FieldName == "Started DateTime")
            {
                ExportdateTime = db.Exports.Select(p => p.StartedTime).First().ToString();
            }

            return ExportdateTime;
        }
        public Guid GetStudyRoleID(string roleName)
        {
            var studyRole = Task.Run(async () =>
            {
                return await studyRoleService.GetAll();
            }).Result;

            Guid studyRoleId = studyRole.FirstOrDefault(s => s.Description == roleName).Id;
            return studyRoleId;
        }

        public async Task UpdateDiaryEntry(string patient, string questionnaireName, Dictionary<string, string> data)
        {
            var allquestionnaires = await questionnaireService.GetAllInflatedQuestionnaires();
            var questionnaire = allquestionnaires.Find(q => q.InternalName == questionnaireName);
            var patientMapping = apiTestData.PatientMappings.First(pm => pm.MappingName == patient);
            var diaryEntry = db.DiaryEntries.FirstOrDefault(s => s.PatientId == patientMapping.Patient.Id && s.QuestionnaireId == questionnaire.Id);
            if (data.ContainsKey("Data Source Name"))
            {
                string input = data["Data Source Name"];
                diaryEntry.DataSourceId = input switch
                {
                    "eCOA App" => DataSource.eCOAApp.Id,
                    "Web Diary" => DataSource.WebDiary.Id,
                    _ => DataSource.Paper.Id,
                };
            }
            if (data.ContainsKey("Started Time"))
            {
                diaryEntry.StartedTime = DateTime.Now;
            }
            if (data.ContainsKey("Completed Time"))
            {
                diaryEntry.CompletedTime = DateTime.Now;
            }
            if (data.ContainsKey("Transmitted Time"))
            {
                diaryEntry.TransmittedTime = DateTime.Now;

            }
            if (data.ContainsKey("Diary Date"))
            {
                diaryEntry.DiaryDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            }
            // setting visit id as empty because by default it is setting screening visit as it is present in visitidendpoint.json
            diaryEntry.VisitId = Guid.Empty;
            if (data.ContainsKey("Visit Name"))
            {
                var patientVisitName = data["Visit Name"];
                var patientVisit = db.PatientVisits.FirstOrDefault(pv => pv.Notes == patientVisitName);
                diaryEntry.VisitId = patientVisit.VisitId;
            }
            db.DiaryEntries.AddOrUpdate(diaryEntry);
            db.SaveChanges(null);
        }

        public ReferenceMaterial GetReferenceMaterial(string name)
        {
            var referenceMaterial = db.ReferenceMaterials.FirstOrDefault(su => su.Name == name);
            return referenceMaterial;
        }

        public string GetDataCorrectionNumber(string dcfType)
        {
            var correctionTypeId = correctionTypeService.GetAll().Result.FirstOrDefault(c => c.Name == dcfType).Id;
            string dataCorrectionNumber = db.Corrections.FirstOrDefault(i => i.CorrectionTypeId == correctionTypeId).DataCorrectionNumber.ToString();
            return dataCorrectionNumber;
        }

        public CorrectionApprovalData GetDataCorrectionData(string dcfType)
        {
            var correctionId = Guid.Parse(GetCorrectionId(dcfType));
            return db.CorrectionApprovalDatas.FirstOrDefault(x => x.TableName == nameof(Answer) &&  x.CorrectionId == correctionId);
        }

        public string GetCorrectionId(string dcfType)
        {
            var correctionTypeId = correctionTypeService.GetAll().Result.FirstOrDefault(c => c.Name == dcfType).Id;
            var dataCorrectionId = db.Corrections.FirstOrDefault(i => i.CorrectionTypeId == correctionTypeId).Id.ToString();
            return dataCorrectionId;
        }

        public string GetCorrectionWorkflowId(string dcfType)
        {
            var correctionTypeId = correctionTypeService.GetAll().Result.FirstOrDefault(c => c.Name == dcfType).Id;
            var dataCorrectionId = db.Corrections.FirstOrDefault(i => i.CorrectionTypeId == correctionTypeId).Id;
            string dataCorrectionWorkflowId = db.CorrectionWorkflows.FirstOrDefault(i => i.CorrectionId == dataCorrectionId).Id.ToString();
            return dataCorrectionWorkflowId;
        }

        public EmailRecipient SubjectEmailNotStored(string ExpectedEmailAddress)
        {
            var EmailRecipient = db.EmailRecipients.FirstOrDefault(s => s.EmailAddress == ExpectedEmailAddress);
            return EmailRecipient;
        }

        public async Task<Guid> GetQuestionnaireIDFromName(string questionnaireName)
        {
            var allquestionnaires = await questionnaireService.GetAllInflatedQuestionnaires();
            var questionnaire = allquestionnaires.Find(q => q.InternalName == questionnaireName);
            return questionnaire.Id;
        }

        public async Task<string> GetQuestionIdFromName(string questionnaireName, string questionText)
        {
            var questionnaireId = await GetQuestionnaireIDFromName(questionnaireName);
            var questions = await questionnaireService.GetQuestions(questionnaireId);
            var question = questions.FirstOrDefault(x => x.QuestionText.Contains(questionText));
            return question.Id.ToString();
        }

        public async Task<int> GetQuestionInputFieldType(string questionText, string questionnaireName)
        {
            var questionnaireId = await GetQuestionnaireIDFromName(questionnaireName);
            var questions = await questionnaireService.GetQuestions(questionnaireId);
            var question = questions.FirstOrDefault(x => x.QuestionText.Contains(questionText));
            return question.InputFieldTypeId;
        }

        public bool GetRequiredValueFromSoftwareRelease(string releaseName)
        {
            var release = db
                .SoftwareReleases
                .FirstOrDefault(sr => sr.Name == releaseName);

            var reqId = release.Required;
            return reqId;

        }

        public void DeleteAllConfigurationVersion()
        {
            db.SoftwareReleases.RemoveRange(db.SoftwareReleases);
            db.SoftwareVersions.RemoveRange(db.SoftwareVersions);
            db.SaveChanges(null);
        }

        public void UpdatePatient(Patient patient)
        {
            db.Patients.AddOrUpdate(patient);
            db.SaveChanges(null);
        }

        public void UpdateSubjectStatus(string oldSubjectStatus, string newSubjectStatus, string subjectNumber)
        {
            var subjectStatuses = patientStatusService.GetAll();
            var oldSubjectStatusId = subjectStatuses.Result.FirstOrDefault(n => n.Name == oldSubjectStatus).Id;
            var newSubjectStatusId = subjectStatuses.Result.FirstOrDefault(n => n.Name == newSubjectStatus).Id;
            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == subjectNumber && p.PatientStatusTypeId == oldSubjectStatusId);
            patient.PatientStatusTypeId = newSubjectStatusId;
            db.Patients.AddOrUpdate(patient);
            db.SaveChanges(null);
        }
        public void DeleteSubjectNumber(string subjectNumber)
        {
            var patient = db.Patients.FirstOrDefault(p => p.PatientNumber == subjectNumber);
            db.Patients.Remove(patient);
            db.SaveChanges(null);
        }

        public void UpdateWebBackupExpireDate(string date, string siteNumber)
        {
            var site = db.Sites.FirstOrDefault(s => s.SiteNumber == siteNumber);
            site.WebBackupExpireDate = Convert.ToDateTime(date);
            db.Sites.AddOrUpdate(site);
            db.SaveChanges(null);
        }

        public DateTime GetSiteLocalTime(string siteNumber)
        {
            var site = db.Sites.FirstOrDefault(s => s.SiteNumber == siteNumber);

            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
            DateTime localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            return localToday;
        }

        #region Analytics
        public AnalyticsReference GetAnalytics(string internalName)
        {
            return db.AnalyticsReferences.SingleOrDefault(r => r.InternalName == internalName);
        }
        #endregion
    }
}