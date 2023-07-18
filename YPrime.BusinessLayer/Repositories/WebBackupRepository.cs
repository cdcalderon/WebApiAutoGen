using Config.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TimeZoneConverter;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Helpers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Dtos;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.Repositories
{
    public class WebBackupRepository : BaseRepository, IWebBackupRepository
    {
        private const string WebBackupTitleKey = "WebBackupTitleKey";
        private const string WebBackupBodyKey = "WebBackupBodyKey";
        private const string WebBackupTabletPublicKeyName = "WebBackupTabletPublicKey";
        private const string WebBackupHandheldPublicKeyName = "WebBackupHandheldPublicKey";
        private const string WebBackupExpirationStringFormat = "dd-MMM-yyyy";
        private const string UniversalCoordinatedTimeZone = "Coordinated Universal Time";
        private const string UTCTimeZone = "UTC";
        private const string DefaultCultureName = "en-US";
        public const string WebBackupUrl =
            @"https://appetize.io/embed/<PUBLIC KEY>?device=<DEVICE NAME>&scale=<SCALE>&deviceColor=<DEVICE COLOR>&osVersion=8.1&autoplay=true&orientation=portrait&centered=true";

        public const string TimeZoneDefault = "--TimeZone--";
        private const string WebBackupHandheldEnabledKey = "WebBackupHandheldEnabled";
        private const string WebBackupTabletEnabledKey = "WebBackupTabletEnabled";

        private readonly IDeviceRepository _deviceRepository;
        private readonly IJwtRepository _jwtRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly ITimeZoneRepository _timeZoneRepository;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly IConfigurationSettings _config;
        private readonly IStudySettingService _studySettingService;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;

        public WebBackupRepository(
            IStudyDbContext db,
            ISiteRepository siteRepository,
            ISystemSettingRepository systemSettingRepository,
            ITimeZoneRepository timeZoneRepository,
            IDeviceRepository deviceRepository,
            IJwtRepository jwtRepository,
            ILanguageService languageService,
            ITranslationService translationSerivce,
            IConfigurationSettings config,
            IStudySettingService studySettingService,
            ISoftwareReleaseRepository softwareReleaseRepository)
            : base(db)
        {
            _siteRepository = siteRepository;
            _systemSettingRepository = systemSettingRepository;
            _translationService = translationSerivce;
            _timeZoneRepository = timeZoneRepository;
            _deviceRepository = deviceRepository;
            _jwtRepository = jwtRepository;
            _languageService = languageService;
            _config = config;
            _studySettingService = studySettingService;
            _softwareReleaseRepository = softwareReleaseRepository;
        }

        public async Task<WebBackupModel> GetClinicianWebBackupModel(Guid siteId, string hostAddress)
        {
            const WebBackupType webBackupType = WebBackupType.TabletClinician;

            var site = await _siteRepository.GetSite(siteId);
            var timeZone = site?.TimeZone;

            if (!string.IsNullOrEmpty(hostAddress) && string.IsNullOrEmpty(timeZone))
            {
                timeZone = await GetTimeZoneWithDefault(hostAddress);
            }

            var configId = await GetConfigIdFromSite(site);

            var model = await GetWebBackupModel(
                webBackupType,
                site,
                timeZone,
                configId);

            var isSiteEligible = IsSiteStillEligible(
                timeZone,
                site?.WebBackupExpireDate,
                WebBackupTabletPublicKeyName);

            var device = _deviceRepository.GetWebBackupTabletDevice(siteId);

            if (isSiteEligible && device != null)
            {
                model.CanDoWebBackup = true;
                model.Title = $"Web backup for {device.Site.Name}";
                model.Url = GetWebBackupUrl(
                    webBackupType,
                    device.Id,
                    device.DeviceTypeId,
                    siteId,
                    device.AssetTag,
                    null,
                    timeZone,
                    configId);
            }

            return model;
        }

        public async Task<WebBackupModel> GetSubjectWebBackupModel(string token, string hostAddress)
        {
            var subjectJwtModel = _jwtRepository.Decrypt<WebBackupJwtModel>(HttpUtility.HtmlDecode(token));

            var site = await _siteRepository.GetSite(subjectJwtModel.SiteId);
            var timeZone = site?.TimeZone;

            if (!string.IsNullOrEmpty(hostAddress))
            {
                timeZone = await _timeZoneRepository.GetTimeZoneIdWithDefault(hostAddress, timeZone);
            }

            var configId = await GetConfigIdFromSite(site);

            var model = await GetWebBackupModel(
                subjectJwtModel.WebBackupType,
                site,
                timeZone,
                configId,
                subjectJwtModel.PatientId);

            var device = GetPatientDevice(
                subjectJwtModel.WebBackupType,
                subjectJwtModel.PatientId, subjectJwtModel.SiteId);

            var keyName = subjectJwtModel.WebBackupType == WebBackupType.HandheldPatient
                ? WebBackupHandheldPublicKeyName
                : WebBackupTabletPublicKeyName;

            if (device?.SiteId != null && IsSiteStillEligible(timeZone, subjectJwtModel.ExpirationDate, keyName))
            {
                model.CanDoWebBackup = true;
                model.Url = GetWebBackupUrl(
                    subjectJwtModel.WebBackupType,
                    device.Id,
                    device.DeviceTypeId,
                    (Guid)device.SiteId,
                    device.AssetTag,
                    subjectJwtModel.PatientId,
                    timeZone,
                    configId,
                    subjectJwtModel.VisitId,
                    subjectJwtModel.CaregiverId);
            }

            return model;
        }

        public async Task<WebBackupJwtModel> CreateJwtModel(
            Guid patientId,
            Guid siteId,
            WebBackupType webBackupType,
            Guid? visitId)
        {
            var device = GetPatientDevice(webBackupType, patientId, siteId);
            var cultureName = await GetPatientCultureName(patientId);

            var expirationDate = device?.SiteId != null
                ? await GetExprirationDate(device.SiteId.Value, webBackupType)
                : null;

            WebBackupJwtModel model = null;

            if (expirationDate.HasValue)
            {
                model = new WebBackupJwtModel
                {
                    WebBackupType = webBackupType,
                    VisitId = visitId,
                    SiteId = (Guid)device.SiteId,
                    DeviceId = device.Id,
                    PatientId = patientId,
                    CultureName = cultureName,
                    ExpirationDate = expirationDate.Value
                };
            }

            return model;
        }

        public async Task<WebBackupEmailModel> CreateWebBackupEmailModel(
            Guid patientId,
            Guid siteId,
            WebBackupType webBackupType = WebBackupType.HandheldPatient,
            Guid? visitId = null)
        {
            var result = new WebBackupEmailModel();

            var jwtModel = await CreateJwtModel(
                patientId,
                siteId,
                webBackupType,
                visitId);

            if (jwtModel != null)
            {   
                var studyName = await _studySettingService.GetStringValue("StudyName");

                result = await CreateEmailModel(studyName, patientId);

                result.WebBackupJwtModel = jwtModel;
            }

            return result;
        }

        public async Task<string> CreateWebBackupEmailBody(
            HttpRequestBase request,
            UrlHelper urlHelper,
            WebBackupJwtModel jwtModel)
        {
            var token = _jwtRepository.Encrypt(jwtModel);

            var webBackupUrl = request.Url.GetLeftPart(UriPartial.Authority) +
                               urlHelper.Action(
                                   "WebBackupHandheld",
                                   "WebBackup",
                                   new { token = HttpUtility.HtmlEncode(token) }
                               );

            var formattedExpirationDate = jwtModel.ExpirationDate.ToString(WebBackupExpirationStringFormat);

            var emailData = new Dictionary<string, string>
            {
                {"ExpireDate", formattedExpirationDate},
                {"EncryptedURL", webBackupUrl}
            };

            var patient = _db.Patients.First(p => p.Id == jwtModel.PatientId);

            var bodyTextTranslation = await _translationService.GetByKey(WebBackupBodyKey, null, patient.LanguageId);
            var bodyTextReplaced = new StringBuilder(bodyTextTranslation);
            bodyTextReplaced.CompleteTemplate(emailData);

            return bodyTextReplaced.ToString();
        }

        private async Task<WebBackupEmailModel> CreateEmailModel(string studyName, Guid patientId)
        {
            var emailData = new Dictionary<string, string>
            {
                {"StudyName", studyName}
            };

            var patient = _db.Patients.First(p => p.Id == patientId);
            var subjectTextTranslation = await _translationService.GetByKey(WebBackupTitleKey, null, patient.LanguageId);

            var subjectTextReplaced = new StringBuilder(subjectTextTranslation);
            subjectTextReplaced.CompleteTemplate(emailData);

            var emailModel = new WebBackupEmailModel
            {
                Id = Guid.NewGuid(),
                Subject = subjectTextReplaced.ToString(),
                EmailContentId = EmailTypes.SubjectHandheldWebBackup
            };

            return emailModel;
        }

        private async Task<Guid?> GetConfigIdFromSite(SiteDto site)
        {
            Guid? result = null;

            if (site != null)
            {
                result = await _softwareReleaseRepository.FindLatestConfigurationVersion(
                    new List<Guid> { site.Id },
                    new List<Guid> { site.CountryId });
            }

            return result;
        }

        private async Task<string> GetTimeZoneWithDefault(string hostAddress)
        {
            var currentTimeZone = TimeZone.CurrentTimeZone.StandardName;
            currentTimeZone = currentTimeZone.Replace(UniversalCoordinatedTimeZone, UTCTimeZone);

            var timeZone = await _timeZoneRepository.GetTimeZoneIdWithDefault(
                hostAddress,
                TZConvert.WindowsToIana(currentTimeZone));

            return timeZone;
        }

        private Device GetPatientDevice(WebBackupType webBackupType, Guid patientId, Guid siteId)
        {
            var device = webBackupType == WebBackupType.HandheldPatient
                ? _deviceRepository.GetWebBackupHandheldDevice(patientId)
                : _deviceRepository.GetWebBackupTabletDevice(siteId);

            return device;
        }

        private async Task<WebBackupModel> GetWebBackupModel(
            WebBackupType webBackupType,
            SiteDto site,
            string timeZone,
            Guid? configId,
            Guid? patientId = null)
        {
            Guid? languageId = null;

            if (patientId != null)
            {
                languageId = _db.Patients.First(p => p.Id == patientId).LanguageId;
            }

            var title = webBackupType == WebBackupType.TabletClinician
                ? "Web backup"
                : await _translationService.GetByKey("WebBackupHandheldTitle", configId, languageId);

            title = title.Replace("{SiteName}", site.Name);

            var instruction = webBackupType == WebBackupType.TabletClinician
                ? "Please be aware that reloading this page will refresh the mobile application.  This will cause any in-progress questionnaires to be lost."
                : await _translationService.GetByKey("WebBackupHandheldInstruction", configId, languageId);

            var error = webBackupType == WebBackupType.TabletClinician
                ? "Web Backup not available for this site, please contact the help desk for assistance"
                : await _translationService.GetByKey("WebBackupHandheldError", configId, languageId);

            var deviceWidth = webBackupType == WebBackupType.HandheldPatient
                ? 320
                : 433;

            var deviceHeight = webBackupType == WebBackupType.HandheldPatient
                ? 636
                : 650;

            var model = new WebBackupModel
            {
                CanDoWebBackup = false,
                Title = title,
                TimeZone = timeZone,
                WebBackupInstruction = instruction,
                WebBackupError = error,
                IFrameWidth = deviceWidth,
                IFrameHeight = deviceHeight
            };

            return model;
        }

        private async Task<DateTime?> GetExprirationDate(
            Guid siteId,
            WebBackupType webBackupType)
        {
            DateTime? resultingDate = null;

            var site = await _siteRepository.GetSite(siteId);

            if (webBackupType == WebBackupType.HandheldPatient)
            {
                var webBackupEnabledDays = await GetWebBackupHandheldValue();

                if (webBackupEnabledDays != -1)
                {
                    var timeZone = site?.TimeZone;

                    resultingDate = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow.AddDays(webBackupEnabledDays),
                        TimeZoneInfo.FindSystemTimeZoneById(timeZone));
                }
            }
            else
            {
                if (site?.WebBackupExpireDate != null)
                {
                    resultingDate = site.WebBackupExpireDate.Value;
                }
            }

            return resultingDate;
        }

        private bool IsSiteStillEligible(
            string timeZone,
            DateTime? expirationDate,
            string keyName)
        {
            var isEligible = false;

            if (expirationDate != null)
            {
                var publicKey = _systemSettingRepository.GetSystemSettingValue(keyName);
                var tz = TZConvert.GetTimeZoneInfo(timeZone);
                var localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                isEligible = (expirationDate >= localToday && !string.IsNullOrEmpty(publicKey));
            }

            return isEligible;
        }

        private async Task<string> GetPatientCultureName(Guid patientId)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == patientId);

            var cultureName = string.Empty;
            if (patient != null)
            {
                var language = await _languageService.Get(patient.LanguageId);
                cultureName = language.CultureName;
            }
            return cultureName;
        }

        public async Task<bool> IsWebBackupHandheldEnabled()
        {
            return await GetWebBackupHandheldValue() > 0;
        }

        public async Task<int> GetWebBackupHandheldValue()
        {
            var result = await GetWebBackupDaysValue(WebBackupHandheldEnabledKey);
            return result;
        }

        public async Task<int> GetWebBackupTabletValue()
        {
            var result = await GetWebBackupDaysValue(WebBackupTabletEnabledKey);
            return result;
        }

        public string GetWebBackupUrl(
            WebBackupType webBackupType,
            Guid deviceId,
            Guid deviceTypeId,
            Guid siteId,
            string assetTag,
            Guid? patientId,
            string timeZone,
            Guid? configId,
            Guid? visitId = null,
            Guid? caregiverId = null)
        {
            var isWebBackupTablet = webBackupType.IsTabletType();

            var webBackupsystemSettingKey = isWebBackupTablet
                ? WebBackupTabletPublicKeyName
                : WebBackupHandheldPublicKeyName;

            var webBackupPublicKey = _systemSettingRepository.GetSystemSettingValue(webBackupsystemSettingKey);

            Dictionary<string, string> replacementDict;

            if (isWebBackupTablet)
            {
                replacementDict = new Dictionary<string, string>
                {
                    {"<PUBLIC KEY>", webBackupPublicKey},
                    {"<DEVICE NAME>", "g430"},
                    {"<DEVICE COLOR>", "white"},
                    {"<SCALE>", "50"}
                };
            }
            else
            {
                replacementDict = new Dictionary<string, string>
                {
                    {"<PUBLIC KEY>", webBackupPublicKey},
                    {"<DEVICE NAME>", "d450"},
                    {"<DEVICE COLOR>", "black"},
                    {"<SCALE>", "80"}
                };
            }

            var webBackupParams = new WebBackupParams
            {
                AppEnvironment = _config.YPrimeInventoryEnvironment,
                AssetTag = assetTag,
                BringYourOwnDevice = DeviceType.BYOD.Id == deviceTypeId,
                DeviceId = deviceId,
                SiteBased = webBackupType.IsTabletType(),
                SiteId = siteId,
                StudyAPIBaseURL = _config.StudyApiBaseUrl,
                StudyUploadURL = "",
                IanaTimeZone = timeZone,
                PatientId = patientId ?? new Guid(),
                VisitId = visitId,
                WebBackupType = webBackupType,
                CaregiverId = caregiverId
            };

            var webBackupParamsJSON = JsonConvert.SerializeObject(
                webBackupParams,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            var computedURL =
                $"{CompleteUrl(WebBackupUrl, replacementDict)}&params={HttpUtility.UrlEncode(webBackupParamsJSON)}";
            return computedURL;
        }

        private async Task<int> GetWebBackupDaysValue(string key)
        {
            var value = await _studySettingService.GetStringValue(key);

            int days;

            if (!int.TryParse(value, out days))
            {
                days = -1;
            }

            return days;
        }

        private string CompleteUrl(string url, Dictionary<string, string> replacementPairs)
        {
            var result = url;

            foreach (var key in replacementPairs.Keys)
            {
                result = result.Replace(key, replacementPairs[key]);
            }

            return result;
        }
    }
}