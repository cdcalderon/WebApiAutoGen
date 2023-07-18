using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.Shared.Helpers.Data;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.BaseClasses;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Extensions;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.Sites)]
    public class SiteController : ControllerWithConfirmations
    {
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICountryService _countryService;
        private readonly IDeviceRepository _DeviceRepository;       
        private readonly ISiteRepository _SiteRepository;
        private readonly ILanguageService _languageService;
        private readonly IStudySettingService _studySettingService;
        private readonly IUserRepository _userRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;

        public SiteController(
            ICountryService countryService,
            ISiteRepository SiteRepository,
            IConfirmationRepository confirmationRepository,
            IDeviceRepository deviceRepository,
            ILanguageService languageService,
            IStudySettingService studySettingService,
            IUserRepository userRepository,
            ISystemSettingRepository systemSettingRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _countryService = countryService;
            _SiteRepository = SiteRepository;
            _confirmationRepository = confirmationRepository;
            _DeviceRepository = deviceRepository;
            _languageService = languageService;
            _studySettingService = studySettingService;
            _userRepository = userRepository;
            _systemSettingRepository = systemSettingRepository;
        }

        [FunctionAuthorizationAttribute("CanViewSiteManagement", "Ability to View site management page.", true)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: PatientVisits
        [FunctionAuthorizationAttribute("CanAddEditSites", "Ability to add/edit sites", true)]
        [HttpGet]
        public async Task<ActionResult> AddEdit(Guid? Id, int? tabPage)
        {
            var siteDto = new SiteDto();
            if (Id != null)
            {
                siteDto = await _SiteRepository.GetSite((Guid) Id);
            }

            siteDto.IsWebBackupEnabled = false;
            siteDto.SelectedLanguageIds = siteDto.Languages.Select(l => l.Id).ToList();
            if (siteDto.WebBackupExpireDate != null)
            {
                string timeZone = siteDto.TimeZone;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

                siteDto.IsWebBackupEnabled = siteDto.WebBackupExpireDate >= localToday;
                if (!siteDto.IsWebBackupEnabled)
                {
                    siteDto.WebBackupExpireDate = null;
                }
            }

            await SetViewBagForAddEditSite(siteDto, tabPage);

            return View(siteDto);
        }

        private void CanSaveSiteInfo()
        {
            var CanSaveSiteInfo = CurrentStudyRole.SystemActions.FirstOrDefault(x => x.Name == "CanAddEditSites");
            ViewBag.CanSaveSiteInfo = (CanSaveSiteInfo != null);
        }

        [HttpPost]
        public async Task<ActionResult> AddEdit(SiteDto siteDto)
        {
            if (ModelState.IsValid)
            {
                var activated = false;
                if (siteDto.IsWebBackupEnabled && siteDto.WebBackupExpireDate == null)
                {
                    siteDto.WebBackupExpireDate = await _SiteRepository.CalculateWebBackupExpireDate(siteDto.TimeZone);
                }

                if (!siteDto.IsWebBackupEnabled)
                {
                    siteDto.WebBackupExpireDate = null;
                }

                // New Site
                if (siteDto.Id == Guid.Empty)
                {
                    if (_SiteRepository.CheckSiteNumberIsUsed(siteDto.SiteNumber))
                    {
                        ModelState.AddModelError("SiteNumber", "Site number must be unique.");
                        await SetViewBagForAddEditSite(siteDto, 1);
                        return View(siteDto);
                    }

                    if (_SiteRepository.CheckSiteNameIsUsed(siteDto.Name))
                    {
                        ModelState.AddModelError("Name", "Name must be unique.");
                        await SetViewBagForAddEditSite(siteDto, 1);
                        return View(siteDto);
                    }

                    if (siteDto.IsActive)
                    {
                        activated = true;
                    }
                }
                else // Edit Site
                {
                    if (_SiteRepository.CheckSiteNumberIsUsed(siteDto.SiteNumber, siteDto.Id))
                    {
                        ModelState.AddModelError("SiteNumber", "Site number must be unique.");
                        await SetViewBagForAddEditSite(siteDto, 1);
                        return View(siteDto);
                    }

                    if (_SiteRepository.CheckSiteNameIsUsed(siteDto.Name, siteDto.Id))
                    {
                        ModelState.AddModelError("Name", "Name must be unique.");
                        await SetViewBagForAddEditSite(siteDto, 1);
                        return View(siteDto);
                    }

                    var site = await _SiteRepository.GetSite(siteDto.Id);
                    var initiallyActive  = site.IsActive;
                    if (!initiallyActive && siteDto.IsActive)
                    {
                        activated = true;
                    }
                }

                var siteNumberLength = int.Parse(await _studySettingService.GetStringValue("SiteNumberLength"));
                if (siteDto.SiteNumber.Length != siteNumberLength)
                {
                    ModelState.AddModelError("SiteNumber", $"Site number length must be {siteNumberLength} digits.");
                    await SetViewBagForAddEditSite(siteDto, 1);
                    return View(siteDto);
                }

                if (siteDto.SelectedLanguageIds.Count == 0)
                {
                    ModelState.AddModelError("Languages", "At least one language must be selected.");
                    await SetViewBagForAddEditSite(siteDto, 2);
                    return View(siteDto);
                }

                var languages = await _languageService.GetAll();
                siteDto.Languages = new List<LanguageDto>();

                var country = await _countryService.Get(c => c.Id == siteDto.CountryId, siteDto.CountryId);
                siteDto.CountryName = country?.Name;

                foreach (var curLanguage in languages)
                {
                    LanguageDto ll = new LanguageDto
                    {
                        Id = curLanguage.Id,
                        Name = curLanguage.CultureName,
                        Selected = siteDto.SelectedLanguageIds.Contains(curLanguage.Id)
                    };
                    siteDto.Languages.Add(ll);
                }

                // If no site display language is set, set it to default
                SetDefaultSiteDisplayLanguage(siteDto, languages);

                await _SiteRepository.UpsertSite(siteDto);

                // Send site activation alert
                if (activated)
                {
                    var siteActiviationAdditional = new Dictionary<string, string>
                    {
                        {"DateCurrent", DateTime.Now.ToString("dd-MMM-yyyy")}
                    };
                    await _confirmationRepository.SendEmail(EmailTypes.SiteActivation, siteDto, siteActiviationAdditional,
                        User.Id, siteDto.Id);
                }

                // reload user to pick up site changes
                var studyUser = await _userRepository.GetUser(User.Id, User.FirstName, User.LastName);
                YPrimeSession.Instance.CurrentUser = studyUser;
                HttpContext.User = studyUser;

                return await DoConfirmation(
                    siteDto,
                    new Dictionary<string, string> {{"SiteCountry", siteDto.CountryName}},
                    EmailTypes.SiteManagement,
                    siteDto.Id);
            }

            CanSaveSiteInfo();

            await SetViewBagForAddEditSite(siteDto, null);
            return View(siteDto);
        }

        public async Task<ActionResult> SiteComplianceChart(Guid? ChartSiteId)
        {
            var site = ChartSiteId != null && ChartSiteId != Guid.Empty
                ? ExecuteAsyncActionSynchronously(() => _SiteRepository.GetSite((Guid) ChartSiteId))
                : new SiteDto {Name = SelectListHelper.AllSitesText, SiteNumber = "-1"};
            ViewData["SiteCompliance"] = ChartSiteId != null && ChartSiteId != Guid.Empty
                ? await _SiteRepository.GetSiteCompliancePercent((Guid) ChartSiteId)
                : -1;
            ViewData["StudyCompliance"] = await _SiteRepository.GetSiteCompliancePercent(Guid.Empty);
            ViewData["SiteChartSelection"] = SelectListHelper.GetSitesList(User.Sites, ChartSiteId);
            return PartialView(site);
        }

        [FunctionAuthorizationAttribute("CanImportSites", "Ability to Import sites.")]
        public ActionResult Import()
        {
            var fileImport = new FileImport<SiteDto>();
            return View(fileImport);
        }

        public ActionResult DownloadImportTemplate(string delimiter, string extension)
        {
            var fileImport = new FileImport<SiteDto>();
            var memoryStream = fileImport.GetImportTemplate(delimiter);
            return new FileStreamResult(memoryStream, "text/csv")
            {
                FileDownloadName = $"SiteImport{extension}"
            };
        }

        [HttpPost]
        public async Task<ActionResult> Import(List<ImportObject<SiteDto>> importedObjects)
        {
            var sitesImported = 0;
            var errors = new List<string>();
            foreach (var siteDto in importedObjects.Select(s => s.Entity))
            {
                try
                {
                    await _SiteRepository.UpsertSite(siteDto);
                    sitesImported++;
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return Json(new
            {
                Errors = new List<string>(errors),
                SuccessMessage = $"Sucessfully imported {sitesImported} site(s)."
            });
        }

        [HttpPost]
        public async Task<ActionResult> ValidateImport(FileImport<SiteDto> import)
        {
            //Bail if null
            if (import == null)
            {
                throw new Exception();
            }

            //Process
            import.Process();
            import.Sanitize();

            if (import.Errors.Count == 0)
            {
                //Validate Sites
                await _SiteRepository.ValidateSiteImport(import);
            }

            return Json(new
            {
                importedObjects = JsonConvert.SerializeObject(import.ImportedObjects, Formatting.Indented,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}),
                html = UIHelper.RenderRazorViewToString(this, "_ImportValidatonSummary", import)
            });
        }

        [HttpGet]
        public async Task<ActionResult> GetSiteGridData()
        {
            var sites = await _SiteRepository
                .GetSitesForUser(User.Id);

            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            sites.AsQueryable()
                .ToList().ForEach(x =>
                x.EditLinkHTML = htmlHelper.PrimeActionLink(x.SiteNumber, "AddEdit", "Site", new {x.Id}, new { @class = "site-number-link" }, true, true)
                    .ToHtmlString());

            return Json(sites, JsonRequestBehavior.AllowGet);
        }

        private async Task SetViewBagForAddEditSite(
            SiteDto siteDto, 
            int? tabPage)
        {
            int NumDays = await _studySettingService.GetIntValue("WebBackupTabletEnabled", 0);
            string PublicKey = _systemSettingRepository.GetSystemSettingValue("WebBackupTabletPublicKey");

            siteDto.WebBackupAssetTag = _DeviceRepository.GetWebBackupTabletDevice(siteDto.Id)?.AssetTag;

            var allCountries = await _countryService.GetAll();
            var allLanguages = await _languageService.GetAll();

            ViewBag.CountryList = new SelectList(allCountries, "Id", "Name", siteDto.CountryId);
            ViewBag.TabPage = tabPage ?? 1;
            ViewBag.SiteNumberLength = _studySettingService.GetStringValue("SiteNumberLength");
            CanSaveSiteInfo();

            var allTimeZones = new Dictionary<string, string>();
            TimeZoneInfo.GetSystemTimeZones().ToList().ForEach(t => { allTimeZones.Add(t.DisplayName, t.Id); });
            ViewBag.TimeZoneList = new SelectList(allTimeZones, "value", "key");
            ViewBag.LanguageList = allLanguages;
            ViewBag.IsNewSite = (siteDto.Id == Guid.Empty).ToString().ToLower();
            ViewBag.CanEditSiteLanguages = CurrentStudyRole.SystemActions.Any(x => x.Name == "CanEditSiteLanguages");
            ViewBag.CanActivateTabletWebBackup =
                CurrentStudyRole.SystemActions.Any(x => x.Name == "CanActivateTabletWebBackup") && NumDays > 0 &&
                !string.IsNullOrEmpty(PublicKey);
            ViewBag.WebBackupExpireDateString = siteDto.WebBackupExpireDate?.ToString("dd-MMM-yyyy");
            ViewBag.WebBackupDisabled = string.IsNullOrEmpty(siteDto.WebBackupAssetTag) && !siteDto.IsWebBackupEnabled;
            if (ViewBag.WebBackupDisabled)
            {
                siteDto.WebBackupExpireDate = null;
                siteDto.IsWebBackupEnabled = false;
            }

            // Global setting for site-facing translations in the study. If true, the site facing dropdowns will show up in add/edit
            var siteFacingEnabled = await _studySettingService.GetBoolValue("SiteFacingTranslationsEnabled");

            if (siteFacingEnabled)
            {
                SetDefaultSiteDisplayLanguage(siteDto, allLanguages);
            }
            
            ViewBag.SiteFacingTranslationsEnabled = siteFacingEnabled;
        }

        private void SetDefaultSiteDisplayLanguage(SiteDto siteDto, List<Core.BusinessLayer.Models.LanguageModel> languages)
        {
            bool setDefaultLanguageId = false;

            if (siteDto.SiteDisplayLanguageId == Guid.Empty || siteDto.SiteDisplayLanguageId == null)
            {
                setDefaultLanguageId = true;
            }
            else
            {
                // Make sure the language selected is valid and in the list. If not reset the default                
                if (languages.FirstOrDefault(x => x.Id == siteDto.SiteDisplayLanguageId) == null)
                {
                    setDefaultLanguageId = true;
                }               
            }

            if (setDefaultLanguageId)
            {
                siteDto.SiteDisplayLanguageId = Config.Defaults.Languages.English.Id;
            }
        }

        [FunctionAuthorization("SiteBulkUpdate", "Can Site Bulk Update", true)]
        public async Task<ActionResult> SiteBulkUpdate()
        {
            ViewBag.GridName = "SitesGrid";

            var SiteVM = new SiteBulkAssignViewModel();
            var sites = await _SiteRepository.GetAllSiteModels(User.Id);

            SiteVM.Sites = sites.ToList();
            return View(SiteVM);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [FunctionAuthorization("SiteBulkUpdate", "Can Site Bulk Update", true)]
        public async Task<ActionResult> SiteBulkUpdate(SiteBulkAssignViewModel siteData)
        {
            var allSites = await _SiteRepository.GetAllSiteModels(User.Id);
            var oldSiteData = (from site in allSites.ToList()
                select new
                {
                    site.SiteNumber,
                    site.Name,
                    site.IsActive,
                    site.Id
                }).ToList();

            var sites = _SiteRepository.BulkUpdateSites(siteData.Sites);

            var newSiteData = (from site in sites
                select new
                {
                    site.SiteNumber,
                    site.Name,
                    site.IsActive,
                    site.Id
                }).ToList();

            var table = await _confirmationRepository.CreateDeltaTable(
                oldSiteData, 
                newSiteData, 
                c => c.Id,
                GetDictionaryForTable(), 
                YPrimeSession.Instance.GlobalDateFormat,
                CurrentSiteUserCultureCode);

            return DoConfirmation(new Dictionary<string, string> {{"SiteActivationTable", table}},
                EmailTypes.BulkSiteManagement);
        }

        private Dictionary<string, string> GetDictionaryForTable()
        {
            return new Dictionary<string, string>
            {
                {"SiteNumber", "Site Number"},
                {"Name", "Name"},
                {"IsActive", "Active"}
            };
        }
    }
}