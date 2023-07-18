using IronPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Constants;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Patient;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.BaseClasses;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Extensions;
using YPrime.StudyPortal.Helpers;
using YPrime.StudyPortal.Models;

namespace YPrime.StudyPortal.Controllers
{
    [Authorize]
    [MenuGroup(MenuGroupType.Patients)]
    public class PatientController : ControllerWithConfirmations
    {
        private const int MaximumAllowedSubjectNumberLength = 9;
        private const string ConfirmationEmailFromAddress = "confirmations@yprime.com";
        private readonly IPatientRepository _PatientRepository;
        private readonly IDiaryEntryRepository _DiaryEntryRepository;
        private readonly ISiteRepository _SiteRepository;
        private readonly IReportRepository _ReportRepository;
        private readonly IPatientForEditAdapter _patientForEditAdapter;
        private readonly IConfirmationRepository _ConfirmationRepository;
        private readonly IDeviceRepository _DeviceRepository;
        private readonly IWebBackupRepository _webBackupRepository;
        private readonly ISubjectInformationService _subjectInformationService;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ILanguageService _languageService;
        private readonly IStudySettingService _studySettingService;
        private readonly IRuleService _ruleService;
        private readonly IServiceSettings _serviceSettings;
        private readonly IAuthService _authService;


        private readonly string ViewedPatientsSessionKey = "RecentlyViewedPatients";
        private readonly int MaxRetainedPatientsInSession = 4;
        public PatientController(
            IPatientRepository patientRepository,
            IDiaryEntryRepository DiaryEntryRepository,
            ISiteRepository SiteRepository,
            IReportRepository ReportRepository,
            IConfirmationRepository confirmationRepository,
            IPatientForEditAdapter patientForEditAdapter,
            IDeviceRepository deviceRepository,
            IWebBackupRepository webBackupRepository,
            ISubjectInformationService subjectInformationService,
            ILanguageService languageService,
            ITranslationService translationService,
            IPatientStatusService patientStatusService,
            IStudySettingService studySettingService,
            ISessionService sessionService,
            IRuleService ruleService,
            IServiceSettings serviceSettings,
            IAuthService authService)
            : base(sessionService)
        {
            _PatientRepository = patientRepository;
            _DiaryEntryRepository = DiaryEntryRepository;
            _SiteRepository = SiteRepository;
            _ReportRepository = ReportRepository;
            _translationService = translationService;
            _patientStatusService = patientStatusService;
            _patientForEditAdapter = patientForEditAdapter;
            _ConfirmationRepository = confirmationRepository;
            _languageService = languageService;
            _DeviceRepository = deviceRepository;
            _webBackupRepository = webBackupRepository;
            _subjectInformationService = subjectInformationService;
            _studySettingService = studySettingService;
            _ruleService = ruleService;
            _serviceSettings = serviceSettings;
            _authService = authService;
        }

        //this will control the recently viewed patients
        private List<RecentlyViewedPatient> RecentlyViewedPatientsInSession
        {
            get
            {
                return Session[ViewedPatientsSessionKey] == null
                    ? new List<RecentlyViewedPatient>()
                    : (List<RecentlyViewedPatient>)Session[ViewedPatientsSessionKey];
            }
            set { Session[ViewedPatientsSessionKey] = value; }
        }


        // GET: Patients
        [FunctionAuthorization("CanSeePatientList", "Ability to see the patient list on the main screen", true)]
        public async Task<ActionResult> Index(Guid? MainSiteId)
        {
            var siteList = User.Sites.Select(s => s.Id).ToList();
            if (MainSiteId != null && MainSiteId != Guid.Empty)
            {
                CurrentSiteId = (Guid)MainSiteId;
                CurrentSiteDto = CurrentSiteId != Guid.Empty ? await _SiteRepository.GetSite((Guid)CurrentSiteId) : null;
                ViewBag.SiteSelected = true;
            }
            else
            {
                if (siteList.Count() == 1)
                {
                    var site = siteList.First();
                    CurrentSiteId = site;
                    CurrentSiteDto = CurrentSiteId != Guid.Empty ? await _SiteRepository.GetSite((Guid)CurrentSiteId) : null;
                }
                else
                {
                    CurrentSiteId = null;
                    CurrentSiteDto = null;
                    ViewBag.SiteSelected = false;
                }
            }

            ViewData["ConsentCreationPatientsOnly"] = YPrimeSession.Instance.StudyType?.ToLower()?.Contains("consent");

            ViewData["TotalSubjects"] = await _PatientRepository.GetEnrolledPatientCount(
                CurrentSiteId != Guid.Empty && CurrentSiteId != null
                    ? new List<Guid> { (Guid)CurrentSiteId }
                    : siteList);
            ViewData["MainSiteSelection"] = SelectListHelper.GetSitesList(User.Sites, CurrentSiteId);

            if (User.Sites.Count == 1)
            {
                ViewData["MainSiteObject"] = await _SiteRepository.GetSite(User.Sites.First().Id);
            }
            else
            {
                ViewData["MainSiteObject"] = CurrentSiteId != null ? CurrentSiteDto : null;
            }

            var userSites = await GetUserSites(_SiteRepository);
            ViewData["HasSites"] = userSites.Any();

            var data = await GetPatientDataForGrid(
                null,
                User,
                CurrentSiteId);

            return View(data);
        }

        public ActionResult RecentlyViewedPatients()
        {
            return PartialView(RecentlyViewedPatientsInSession);
        }

        public ActionResult ClearRecentlyViewedPatients()
        {
            RecentlyViewedPatientsInSession = new List<RecentlyViewedPatient>();

            return PartialView("~/Views/Patient/RecentlyViewedPatients.cshtml", RecentlyViewedPatientsInSession);
        }

        public ActionResult SiteComplianceChart()
        {
            return PartialView();
        }

        public ActionResult GridLinks()
        {
            return PartialView();
        }

        [FunctionAuthorization("CanCreateBringYourOwnDeviceCode",
            "Can create a patient bring your own device code.")]
        public async Task<JsonResult> CreateBYODCode(Guid patientId)
        {
            var ajaxResult = new AjaxResult();

            ajaxResult.Success = false;
            ajaxResult.Message = "";

            var patient = await _PatientRepository.GetPatient(patientId, CurrentSiteUserCultureCode);
            var patientDevice = _DeviceRepository.GetPatientBYODDevice(patient.Id);

            if (patientDevice != null)
            {
                ajaxResult.Success = true;
                ajaxResult.JsonData = @"{ 'PatientId'  : '" + patientId + "' }";

                SetPopUpMessageOnLoad("BYODCodeExisting",
                    Url.Content("~/") + Request.RequestContext.RouteData.Values["controller"] + "/ConfirmationEmail?patientId=" + patientId.ToString(),
                    patientId.ToString());

            }
            else
            {
                string newAssetTag = await CreateBYODCodeForPatient(patientId);

                if (newAssetTag != null)
                {
                    ajaxResult.Success = true;
                    ajaxResult.JsonData = @"{ 'PatientId'  : '" + patientId + "' }";
                    SetPopUpMessageOnLoad("BYODCodeExisting",
                    Url.Content("~/") + Request.RequestContext.RouteData.Values["controller"] + "/ConfirmationEmail?patientId=" + patientId.ToString(),
                    patientId.ToString());
                }
            }


            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> ConfirmationEmail(Guid patientId)
        {
            BYODEmailModel byodEmailModel = await BuildEmailModel(patientId);

            ViewBag.AssetTag = _DeviceRepository.GetPatientBYODDevice(patientId)?.AssetTag;
            ViewBag.Client = byodEmailModel.Sponsor;
            ViewBag.Protocol = await _studySettingService.GetStringValue("Protocol");
            ViewBag.StudyName = YPrimeSession.Instance.StudySettingValues["StudyName"];
            ViewBag.StudyDescription = await _translationService.GetByKey("StudyDescription");

            var AbsolutePath = getBaseUri();
            AbsolutePath = $"{AbsolutePath}Content/Images";
            ViewBag.ImagesPath = AbsolutePath;

            return PartialView(byodEmailModel);
        }

        public async Task<string> ConfirmationEmailViewToStringAsync(BYODEmailModel byodEmailModel)
        {
            var viewName = "ConfirmationEmail";

            var AbsolutePath = getBaseUri();
            AbsolutePath = $"{AbsolutePath}Content/Images";
            byodEmailModel.ConfirmationEmailTranslations = await GetEmailConfrimationTranslations(byodEmailModel.LanguageId);

            ViewData.Model = byodEmailModel;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewCxt = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewCxt.ViewBag.AssetTag = _DeviceRepository.GetPatientBYODDevice(byodEmailModel.PatientId)?.AssetTag;
                viewCxt.ViewBag.Client = byodEmailModel.Sponsor;
                viewCxt.ViewBag.Protocol = await _studySettingService.GetStringValue("Protocol");
                viewCxt.ViewBag.StudyName = YPrimeSession.Instance.StudySettingValues["StudyName"];
                viewCxt.ViewBag.StudyDescription = await _translationService.GetByKey("StudyDescription");

                viewCxt.ViewBag.ImagesPath = AbsolutePath;
                viewResult.View.Render(viewCxt, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }

        private async Task<BYODEmailModel> BuildEmailModel(Guid patientId)
        {
            PatientDto patient = await _PatientRepository.GetPatient(patientId, CurrentSiteUserCultureCode);
            BYODEmailModel byodEmailModel = new BYODEmailModel();

            var languageId = (Guid)patient.LanguageId;
            var language = await _languageService.Get(languageId);

            byodEmailModel.Id = new Guid();
            var sponsor = await _studySettingService.GetStringValue("StudySponsor");
            var enrolmentMessage = HtmlHelperExtensions.TranslationLabel("keyCreateBYODCode", (string)ViewBag.SiteUserCultureCode, false);

            byodEmailModel.ConfirmationEmailTranslations = await GetEmailConfrimationTranslations(languageId);

            byodEmailModel.Subject = $"{sponsor} {patient.PatientNumber} {enrolmentMessage} ";
            byodEmailModel.Sponsor = sponsor;
            byodEmailModel.PatientId = patient.Id;
            byodEmailModel.PatientNumber = patient.PatientNumber;
            byodEmailModel.SiteId = patient.SiteId;
            byodEmailModel.SiteName = patient.Site.Name;
            byodEmailModel.SiteNumber = patient.Site.SiteNumber;
            byodEmailModel.LanguageId = languageId;
            byodEmailModel.IsLanguageRightToLeft = language.IsRightToLeft;
            byodEmailModel.StudyId = await _studySettingService.GetStringValue("StudyID");
            byodEmailModel.EmailContent = await _translationService.GetByKey("ConfirmationEmailBody", _sessionService.UserConfigurationId, languageId);

            byodEmailModel.EmailContentId = Guid.Parse("B04D7B3B-6347-4F5C-9FFE-F9817F4613BA"); //New Seeding.

            byodEmailModel.PatientPin = await _PatientRepository.GenerateDefaultPin();

            return byodEmailModel;
        }

        [HttpPost]
        public async Task<FileStreamResult> GetConfirmationEmail(BYODEmailModel byodEmailModel)
        {
            var stream = await BuildIronPdf(byodEmailModel);
            var filename = $"{byodEmailModel.Subject}.pdf";

            return new FileStreamResult(stream, "application/pdf") { FileDownloadName = filename };
        }

        [HttpPost]
        public async Task<ActionResult> SendConfirmationEmail(BYODEmailModel byodEmailModel)
        {
            var response = new Dictionary<string, string>();
            if (ModelState.IsValid)
            {

                var email = new SendingEmailModel
                {
                    Id = Guid.NewGuid(),
                    To = new List<string>
                    {
                        byodEmailModel.PatientEmail
                    },
                    Cc = new List<string>(),
                    Bcc = new List<string>(),
                    ToUsers = new List<Guid>(),
                    CcUsers = new List<Guid>(),
                    BccUsers = new List<Guid>(),
                    From = ConfirmationEmailFromAddress,
                    Subject = byodEmailModel.Subject,
                    Body = byodEmailModel.EmailContent,
                    CreatedDate = DateTime.Now,
                    Attachments = new Dictionary<string, byte[]>(),
                    StudyId = Guid.Parse(byodEmailModel.StudyId),
                    SponsorId = _serviceSettings.SponsorId,
                    Environment = _serviceSettings.StudyPortalAppEnvironment
                };

                var filename = $"{byodEmailModel.Subject}.pdf";
                var stream = await BuildIronPdf(byodEmailModel);

                email.Attachments.Add(filename, stream.ToArray());

                _ConfirmationRepository.SaveConfirmation(email.Id, email.Subject, email.Body, string.Empty,
                    byodEmailModel.EmailContentId, User.Id, byodEmailModel.SiteId,
                    new List<EmailRecipient>());
                await _ConfirmationRepository.SendApiEmail(email);
                response.Add("Success", "true");
            }

            return Json(response);
        }

        private async Task<BYODEmailTranslationsModel> GetEmailConfrimationTranslations(Guid languageid)
        {
            var BYODEmailTranslationsModel = new BYODEmailTranslationsModel()
            {
                ConfirmationEmailTitle = await _translationService.GetByKey("ConfirmationEmailTitle", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailBodyLine1 = await _translationService.GetByKey("ConfirmationEmailBodyLine1", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailBodyLine2 = await _translationService.GetByKey("ConfirmationEmailBodyLine2", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailBodyLine3 = await _translationService.GetByKey("ConfirmationEmailBodyLine3", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailStudyInformationHeader = await _translationService.GetByKey("ConfirmationEmailStudyInformationHeader", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailStudyInformationSponsor = await _translationService.GetByKey("ConfirmationEmailStudyInformationSponsor", _sessionService.UserConfigurationId, languageid),

                ConfirmationEmailStudyInformationProtocol = await _translationService.GetByKey("ConfirmationEmailStudyInformationProtocol", _sessionService.UserConfigurationId, languageid),

                ConfirmationEmailStudyInformationStudyName = await _translationService.GetByKey("ConfirmationEmailStudyInformationStudyName", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailStudyInformationStudyDescription = await _translationService.GetByKey("ConfirmationEmailStudyInformationStudyDescription", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailYourInformationHeader = await _translationService.GetByKey("ConfirmationEmailYourInformationHeader", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailYourInformationEnrollmentID = await _translationService.GetByKey("ConfirmationEmailYourInformationEnrollmentID", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailYourInformationPatientID = await _translationService.GetByKey("ConfirmationEmailYourInformationPatientID", _sessionService.UserConfigurationId, languageid),

                ConfirmationEmailYourInformationSiteID = await _translationService.GetByKey("ConfirmationEmailYourInformationSiteID", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailDefaultPinCode = await _translationService.GetByKey("ConfirmationEmailDefaultPinCode", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailGettingStartedHeader = await _translationService.GetByKey("ConfirmationEmailGettingStartedHeader", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailGettingStartedStep1 = await _translationService.GetByKey("ConfirmationEmailGettingStartedStep1", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailGettingStartedStep2 = await _translationService.GetByKey("ConfirmationEmailGettingStartedStep2", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailGettingStartedStep3 = await _translationService.GetByKey("ConfirmationEmailGettingStartedStep3", _sessionService.UserConfigurationId, languageid),
                ConfirmationEmailGettingStartedFooter = await _translationService.GetByKey("ConfirmationEmailGettingStartedFooter", _sessionService.UserConfigurationId, languageid)
            };

            return BYODEmailTranslationsModel;
        }

        private async Task<MemoryStream> BuildIronPdf(BYODEmailModel byodEmailModel)
        {
            string viewHtml = await ConfirmationEmailViewToStringAsync(byodEmailModel);

            var fileName = $"{byodEmailModel.Subject}.pdf";

            /* IRON PDF */
            PdfPrintOptions options = new PdfPrintOptions
            {
                CustomCssUrl = "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css", /*IronPdf only supports Bootstrap 4.0.0 https://stackoverflow.com/questions/58152198/ironpdf-ignores-bootstrap-column-grids-when-creating-pdf-from-html*/
                CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen,
                DPI = 300,
                PaperOrientation =
                PdfPrintOptions.PdfPaperOrientation.Portrait,
                PaperSize = PdfPrintOptions.PdfPaperSize.Letter,
                EnableJavaScript = true,
                RenderDelay = 500,
                FitToPaperWidth = true
            };
            options.MarginTop = 5;
            options.MarginBottom = 5;
            options.MarginLeft = 1;
            options.MarginRight = 1;

            var Renderer = new IronPdf.HtmlToPdf(options);

            /**
             * Modify HTML
             */
            // Remove height of .modal-header-ce
            viewHtml = Regex.Replace(viewHtml, @"\.modal-header-ce\s*\{([\S\s]*?)\}", ".modal-header-ce{border: none;}");

            // Set width of ceShadowbox
            viewHtml = Regex.Replace(viewHtml, @"\.ceShadowBox\s*\{([\S\s]*?)\s*width: 378px;\s*\}", ".ceShadowBox{$1}");

            // remove "row" class from tr
            viewHtml = viewHtml.Replace("<tr class=\"row\">", "<tr>");

            // update container
            viewHtml = Regex.Replace(viewHtml, @"#container\s*\{([\S\s]*?)\s*\}", "#container{width: 100%; padding: 0 15px;}");

            // Increase font size
            viewHtml = viewHtml.Replace("font-size: 20px;", "font-size: 24px;");
            viewHtml = viewHtml.Replace("font-size: 16px;", "font-size: 20px;");
            viewHtml = viewHtml.Replace("font-size: 12px;", "font-size: 16px;");
            viewHtml = viewHtml.Replace("font-size: 10px;", "font-size: 12px;");

            //number circles size
            viewHtml = viewHtml.Replace("class=\"circle_number\"", "class=\"circle_number\" style=\"width:30px;height:30px;\"");

            //.ceShadowBox
            viewHtml = viewHtml.Replace("height: 180px", "height: 240px");

            // QR Image bigger
            // Remove height of .modal-header-ce
            viewHtml = Regex.Replace(viewHtml, @"\.ceQRCodes\s*\{([\S\s]*?)\}", ".ceQRCodes{width: 500px; height: auto;}");

            // hide print and e-mail buttons, after that add additional styles
            viewHtml = Regex.Replace(viewHtml, @"\.mainheader\s*\{([\S\s]*?)\}", ".mainheader{width: 1100px; height: auto;} #dvButtons{display:none;} #mainclose{display:none;} html,body{background:white; position:relative;}");

            var PDF = Renderer.RenderHtmlAsPdf(viewHtml);
            PdfDocument ironPdfDoc = PDF.SaveAs(fileName);

            return ironPdfDoc.Stream;
        }

        private string getBaseUri()
        {
            var AbsolutePath = Request.Url.AbsoluteUri;
            var thisControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            AbsolutePath = AbsolutePath.Substring(0, AbsolutePath.IndexOf(thisControllerName));
            return AbsolutePath;
        }


        private async Task<string> CreateBYODCodeForPatient(Guid patientId)
        {
            var numberOfUses = 1000;
            string newAssetTag = null;
            var device = await _PatientRepository.AddBringYourOwnDeviceAssetTag(
                ConfigurationManager.AppSettings["YPrime.PrimeInventoryAPIUrl"],
                this._serviceSettings.InventoryAppEnvironment,
                patientId,
                numberOfUses);

            if (device != null)
            {
                newAssetTag = device.AssetTag;
            }

            return newAssetTag;
        }

        /**************************************
         * ASPNET MVC GRID
         * *************************************/

        public ActionResult GetPatientGrid(bool? showActive, string gridName)
        {
            ViewBag.ResultDisplayCount = DefaultResultDisplayCount;

            // definte these variables outside of the async action in order to have access to the httpcontext
            var user = User;
            var currentSiteId = CurrentSiteId;

            var grid = ExecuteAsyncActionSynchronously(() => GetPatientDataForGrid(
                showActive,
                user,
                currentSiteId));

            ViewBag.ResultCount = grid.Count();
            ViewBag.GridName = gridName;

            return PartialView(grid);
        }

        private async Task<IEnumerable<PatientDto>> GetPatientDataForGrid(
            bool? showActive,
            StudyUserDto user,
            Guid? currentSiteId)
        {
            var siteList = user.Sites.Select(s => s.Id).ToList();

            var allowedSites = !currentSiteId.HasValue
                ? siteList
                : new List<Guid> { currentSiteId.Value };

            var patients = await _PatientRepository.GetAllPatients(allowedSites, showActive).ConfigureAwait(false);

            return patients;
        }

        [FunctionAuthorization("CanEditPatient", "Can edit patient", true)]
        public async Task<ActionResult> Edit(Guid? guid)
        {
            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = await GetPatientForEdit(guid.Value, true);
            if (patient == null)
            {
                throw new Exception();
            }

            RecordPatientView(patient.Id, patient.PatientNumber);

            var byodAvailable = (YPrimeSession.Instance.StudySettingValues["BringYourOwnDeviceAvailable"] ?? "false") == "True";
            if (byodAvailable)
            {
                await _translationService.LoadIntoCache(TranslationConstants.PortalTranslationCategory, _sessionService.UserConfigurationId, patient.LanguageId);
            }

            ViewBag.BYODAvailable = byodAvailable;
            ViewBag.ShowCorrectionLineItems = false;
            ViewBag.ShowCaregiverTab = ShowCaregiverTab();
            ViewBag.ShowSubjectAttributeDetails = User.HasPermission("CanViewPatientDetails");

            return View(patient);
        }

        private bool ShowCaregiverTab()
        {
            var userPermissions = User.CanViewCaregiverTab();
            var settingPermission = false;
            string settingValue;

            if (YPrimeSession.Instance.StudySettingValues.TryGetValue("CaregiversEnabled", out settingValue))
            {
                settingPermission = settingValue == "1";
            }


            var result = userPermissions && settingPermission;

            return result;
        }

        private async Task<PatientForEdit> GetPatientForEdit(Guid patientId, bool includeDcfWorkflows = false)
        {
            var entity = _PatientRepository.GetPatientForEditAsync(patientId).Result;
            PatientForEdit patient = null;

            if (entity != null)
            {
                var webBackupIsEnabled =
                    CurrentStudyRole.SystemActions.Any(x => x.Name == "CreateWebBackupHandheldLink") &&
                    await _webBackupRepository.IsWebBackupHandheldEnabled();

                var deviceId = _DeviceRepository.GetDeviceIdForPatient(patientId);
                var currentUserId = includeDcfWorkflows
                    ? User.Id
                    : (Guid?)null;

                patient = await _patientForEditAdapter.Adapt(entity,
                    CurrentSiteUserCultureCode,
                    webBackupIsEnabled,
                    deviceId,
                    currentUserId);

                if (webBackupIsEnabled)
                {
                    patient.WebBackUpEmail = await _webBackupRepository
                        .CreateWebBackupEmailModel(
                            patientId,
                            patient.SiteId);
                }
            }

            return patient;
        }

        [FunctionAuthorization("CanCreatePatient", "Can create patient", true)]
        public async Task<ActionResult> Create()
        {
            if (CurrentSiteId == null)
            {
                // If they cleared the selected site in another tab, send them back to the index to select again                
                return RedirectToAction("Index");
            }

            var patient = await GetPatientObjectForCreate((Guid)CurrentSiteId);

            return View(patient);
        }

        [FunctionAuthorization("CanCreatePatient", "Can create patient", true)]
        public async Task<ActionResult> CreateForParticipant(Guid siteId, Guid participantId)
        {
            var patient = await GetPatientObjectForCreate(siteId);

            patient.ConsentParticipantId = participantId;

            return View("Create", patient);
        }

        private async Task<PatientDto> GetPatientObjectForCreate(Guid siteId)
        {
            var patient = await _PatientRepository.CreateNewPatientObject(siteId);
            await SetupPatientCreateViewData(patient);

            var elementsToRemove = GetPatientAttributesToRemovedBasedOnRuleEngine(patient);
            patient.PatientAttributes.RemoveAll(elementsToRemove.Contains);

            ViewBag.SiteLanguages = await GetSiteLanguagesForPatient(patient);

            return patient;
        }

        //Summary
        // based on ECOA-4307 (if there is no business rule (as in the business rule id is null),
        // the field should be shown, only if the rule is false it should be hidden)
        public List<PatientAttributeDto> GetPatientAttributesToRemovedBasedOnRuleEngine(PatientDto patient)
        {
            List<PatientAttributeDto> patientAttributeToRemoved = new List<PatientAttributeDto>();
            var currentSiteTime = _SiteRepository.GetSiteLocalTime(patient.SiteId);
            foreach (var attribute in patient?.PatientAttributes.ToList())
            {
                if (attribute?.SubjectInformation?.BusinessRuleId != null)
                {
                    var isValid = _ruleService
                         .ExecuteBusinessRule(attribute.SubjectInformation.BusinessRuleId.Value, patient?.Id, patient?.SiteId,
                         attribute.SubjectInformation.BusinessRuleTrueFalseIndicator, currentSiteTime.DateTime);

                    if (!isValid.ExecutionResult)
                    {
                        patientAttributeToRemoved.Add(attribute);
                    }
                }
            }

            return patientAttributeToRemoved;
        }

        [HttpPost]
        public async Task<ActionResult> Create(PatientDto patient)
        {
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await SetupPatientCreateViewData(patient);

            var site = await _SiteRepository
                .GetSite(patient.SiteId);

            patient.SubjectInformations = await _subjectInformationService
                .GetForCountry(site.CountryId);

            patient.PatientAttributes.
                ForEach(pa =>
                {
                    pa.SubjectInformation = patient
                        .SubjectInformations
                        .First(pacd => pacd.Id == pa.PatientAttributeConfigurationDetailId);
                });

            if (await ValidateNewPatientObject(patient))
            {              
                if (await SaveNewPatient(patient)
                 && await SubmitPatientForConsent(patient))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Unable to complete enrollment. Remove unused patient and device
                    await RollbackPatientCreation(patient);
                }
            }

            ViewBag.SiteLanguages = await GetSiteLanguagesForPatient(patient);
            patient.Site = site;

            return View(patient);
        }

        private Device GetBYODDevice(PatientDto patient)
        {
            Device device = null;
            var isByod = patient.SubjectUsePersonalDevice.HasValue && patient.SubjectUsePersonalDevice.Value;
            if (isByod)
            {
                device = _DeviceRepository.GetPatientBYODDevice(patient.Id);
            }

            return device;
        }

        private async Task RollbackPatientCreation(PatientDto patient)
        {
            await _PatientRepository.RemovePatient(patient.Id);

            var device = GetBYODDevice(patient);
            if (device != null)
            {         
                await _DeviceRepository.RemoveDevice(device);
            }
        }

        private async Task<bool> SubmitPatientForConsent(PatientDto patient)
        {
            if (patient.ConsentParticipantId == null)
            {
                // Patient not eligible for consent, no additional work to do
                return true;
            }

            var result = false;

            try
            {
                // Submit patient for consent
                result = await _authService.UpdateECOALink((Guid)patient.ConsentParticipantId, patient.Id, GetBYODDevice(patient)?.AssetTag);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new System.Web.HttpException($"Submit subject to eConsent failed : {e.Message}"));
            }

            return result;
        }

        private async Task<IEnumerable<LanguageDto>> GetSiteLanguagesForPatient(PatientDto patient)
        {
            return patient.Site?.Languages
                ?? (await _SiteRepository.GetSite(patient.SiteId)).Languages;
        }

        private async Task<bool> SaveNewPatient(PatientDto patient)
        {
            var success = false;

            var subjectUsePersonalDevice = patient.SubjectUsePersonalDevice;
            var patientResponse = await _PatientRepository.InsertUpdatePatient(patient, true, ModelState);

            if (patientResponse.Success)
            {
                var isByod = subjectUsePersonalDevice.GetValueOrDefault();
                if (isByod)
                {
                    await CreateBYODCodeForPatient(patientResponse.PatientId);

                    SetPopUpMessageOnLoad("BYODCodeNew",
                        Url.Content("~/") + Request.RequestContext.RouteData.Values["controller"] + "/ConfirmationEmail?patientId=" + patientResponse.PatientId.ToString(),
                        patientResponse.PatientId.ToString());
                }
                else
                {
                    var transKey = await _studySettingService
                        .GetIntValue(StudySettingConstants.PatientPinLength, 4) == 4
                            ? TranslationKeyTypes.lblPatientAddSuccess
                            : TranslationKeyTypes.lblPatientAddSuccessSixDigit;

                    var newPatientNumber = await _PatientRepository
                        .GeneratePatientNumber(patient.SiteId, patient.PatientNumber);

                    SetPopUpMessageOnLoad(TranslationConstants.Success,
                        string.Format(await _translationService.GetByKey(transKey), newPatientNumber));
                }

                // we need the patient.id(after it is saved) to be used for consent so we need to store it
                patient.Id = patientResponse.PatientId;
                success = true;
            }
            else
            {
                var errorList = new StringBuilder();
                foreach (var error in ModelState.GetModelErrors())
                {
                    errorList.Append(error.Value + "</br>");
                }

                SetPopUpMessageOnLoad(
                    TranslationConstants.Warning,
                    string.Format(errorList.ToString(), patient.PatientNumber));
            }

            return success;
        }

        private async Task<bool> ValidateNewPatientObject(PatientDto patient)
        {
            var success = false;

            var patientNumberLength = await GetPatientNumberLength();
            if (patientNumberLength !=  patient.PatientNumber?.Length)
            {
                var lblPatientNumber = await _translationService
                    .GetByKey(TranslationKeyTypes.lblPatientNumber);

                SetPopUpMessageOnLoad(TranslationConstants.Warning,
                    lblPatientNumber + " field needs to be " + patientNumberLength + "-digits long.");
            }
            else if (patient.LanguageId == null || patient.LanguageId == Guid.Empty)
            {
                SetPopUpMessageOnLoad(
                    await _translationService.GetByKey(TranslationConstants.Warning),
                    await _translationService.GetByKey(TranslationConstants.NoLanguageSelected)
                );
            }
            else
            {
                success = true;
            }

            return success;
        }

        private async Task<int> GetPatientNumberLength()
        {
            var patientNumberLength = int.Parse(await _studySettingService.GetStringValue(StudySettingConstants.PatientNumberLength));

            patientNumberLength = patientNumberLength > MaximumAllowedSubjectNumberLength
                ? MaximumAllowedSubjectNumberLength
                : patientNumberLength;
            return patientNumberLength;
        }

        private async Task SetupPatientCreateViewData(PatientDto patient)
        {
            ViewData["SiteSelection"] =
                SelectListHelper.GetSitesList(await GetUserSites(_SiteRepository), patient.SiteId, false);
            ViewData["PatientGenderSelection"] = GetPatientGenders(patient.PatientGender);
            ViewData["MaxEnteredPatientNumberLength"] =
                int.Parse(await _studySettingService.GetStringValue("PatientNumberLength"));
            ViewData["SubjectUsePersonalDeviceEnabled"] =
                            await _studySettingService.GetStringValue("BringYourOwnDeviceAvailable");
        }

        [NoDirectAccess]
        [FunctionAuthorization("CanViewPatientDetails", "Can view patient details.", true)]
        public async Task<ActionResult> DisplayPatient(Guid? guid)
        {
            //this is the actual display of the patient details
            PatientDto patient;

            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            patient = await _PatientRepository.GetPatient((Guid)guid, CurrentSiteUserCultureCode);
            ViewBag.TemporaryPin = _PatientRepository.GeneratePin();

            if (patient == null)
            {
                 throw new Exception();
            }

            return PartialView(patient);
        }

        [HttpGet]
        public async Task<ActionResult> ViewPatientInformation(Guid? guid)
        {
            //this is the actual display of the patient details
            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = await GetPatientForEdit(guid.Value);

            if (patient == null)
            {
                 throw new Exception();
            }

            return PartialView(patient);
        }

        [HttpGet]
        [FunctionAuthorization("CanEditPatient2", "Ability to edit the details of a patient.", true)]
        public async Task<ActionResult> EditPatientInformation(Guid? guid)
        {
            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = await GetPatientForEdit(guid.Value);

            var patientStatusTypes = await _patientStatusService.GetAll();
            ViewBag.PatientStatusTypeSelection = this.GetPatientStatusTypesSelectList(
                patient.PatientStatusTypeId,
                patientStatusTypes,
                true);

            if (patient == null)
            {
                throw new Exception("Invalid patient data.");
            }

            return PartialView(patient);
        }

        [HttpPost]
        public async Task<ActionResult> EditPatientInformation(Guid id, int patientStatusTypeId)
        {
            bool success = false;

            var patientStatusTypes = await _patientStatusService.GetAll();
            //this is the actual display of the patient details
            ViewBag.PatientStatusTypeSelection = this.GetPatientStatusTypesSelectList(
                patientStatusTypeId,
                patientStatusTypes,
                true);

            ViewBag.ReloadPage = false;

            if (ModelState.IsValid)
            {
                await _PatientRepository.UpdatePatientStatusTypeId(id, patientStatusTypeId);

                ViewBag.Updated = true;
                success = true;
            }

            //insure that the data is fully loaded when returning to the view
            var patient = await GetPatientForEdit(id);
            ViewBag.ReloadPage = true;
            SetPopUpMessageOnLoad("Success",
                await _translationService.GetByKey(TranslationKeyTypes.lblPatient) + " " +
                    patient.PatientNumber + " has been updated successfully.");

            return PartialView(
                "~/Views/Patient/" + (success ? "ViewPatientInformation" : "EditPatientInformation") + ".cshtml",
                patient);
        }

        public ActionResult GetTakenQuestionnaires(Guid patientId)
        {
            return PartialView(patientId);
        }

        public ActionResult EnrolledChart()
        {
            ChartDataObject chart = new ChartDataObject();
            Guid reportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F");
            var pars = new Dictionary<string, object>();
            pars.Add("Sites", User.Sites);

            var userId = User.Id;

            chart = ExecuteAsyncActionSynchronously(() => _ReportRepository.GetReportChartData(reportId, userId, pars));
            chart.HideContainer = true;
            chart.ShowDisplayTitle = false;
            return PartialView(chart);
        }

        private SelectList GetPatientGenders(string SelectedValue)
        {
            SelectList result;

            var genders = new Dictionary<string, string>
            {
                {"M", "Male"},
                {"F", "Female"}
            };

            result = new SelectList(genders.AsEnumerable(), "key", "value");

            return result;
        }


        [HttpGet]
        [FunctionAuthorization("CanResetPatientPin", "Can reset the pin for a patient.")]
        public async Task<ActionResult> ResetPIN(Guid PatientId, bool Update, string TemporaryPin)
        {
            bool success = false;

            if (Update)
            {
                success = await _PatientRepository.ResetPatientPin(PatientId, TemporaryPin);
            }

            ViewBag.Update = true;
            ViewBag.TemporaryPin = await _PatientRepository.GenerateDefaultPin();
            ViewBag.PatientId = PatientId;
            ViewBag.Success = success;

            return PartialView();
        }

        /**************************************
        * ASPNET MVC GRID
        * *************************************/

        public ActionResult GetDiaryGrid(Guid PatientId, Guid? QuestionnaireId, Guid? VisitId)
        {
            var gridName = QuestionnaireId == null ? "allquestionnaires" : QuestionnaireId + "-Grid";

            if (VisitId != null)
            {
                gridName = PatientId + "-Visit" + VisitId + "-Grid";
            }

            ViewBag.GridName = gridName;

            var userRoles = User.Roles;

            var grid = ExecuteAsyncActionSynchronously(() => GetDiaryDataForGrid(
                PatientId,
                QuestionnaireId,
                VisitId,
                userRoles));

            return PartialView(grid);
        }

        private async Task<List<DiaryEntryDto>> GetDiaryDataForGrid(
            Guid PatientId,
            Guid? QuestionnaireId,
            Guid? VisitId,
            List<StudyRoleModel> userRoles)
        {
            List<DiaryEntryDto> entries;

            var isBlinded = false;

            foreach (var role in userRoles)
            {
                isBlinded = role.IsBlinded;

                if (isBlinded)
                {
                    break;
                }
            }

            if (VisitId == null)
            {
                entries = await _DiaryEntryRepository
                    .GetDiaryEntriesInflated(QuestionnaireId, false, null, PatientId);
            }
            else
            {
                entries = await _DiaryEntryRepository
                    .GetAllPatientDiaryEntriesByVisit(PatientId, VisitId, false, isBlinded, CurrentSiteUserCultureCode);
            }

            return entries;
        }


        private void RecordPatientView(Guid patientId, string patientNumber)
        {
            var recentlyViewed = RecentlyViewedPatientsInSession;

            //check if already recorded
            recentlyViewed.RemoveAll(r => r.Id == patientId);

            if (recentlyViewed.Count >= MaxRetainedPatientsInSession)
            {
                recentlyViewed.RemoveAt(0);
            }

            recentlyViewed.Add(new RecentlyViewedPatient { Id = patientId, PatientNumber = patientNumber });
            RecentlyViewedPatientsInSession = recentlyViewed;
        }

        [FunctionAuthorization("CanViewUpcomingVisitsWidget", "Can view Upcoming Visits Widget.", true)]
        public ActionResult UpcomingVisitsWidget()
        {
            var model = new UpcomingPatientVisitsWidget
            {
                Patients = ExecuteAsyncActionSynchronously(() => GetUpComingPatientVisitsGrid()),
            };

            model.Count = model.Patients.Count();

            ViewBag.ResultDisplayCount = DefaultResultDisplayCount;
            return PartialView(model);
        }

        private async Task<IEnumerable<PatientDto>> GetUpComingPatientVisitsGrid()
        {
            var siteList = User.Sites.Select(s => s.Id).ToList();
            var allowedSites = CurrentSiteId == null ? siteList : new List<Guid> { (Guid)CurrentSiteId };
            var intervalInDays = int.Parse(await _studySettingService.GetStringValue("UpcomingPatientVisitWidgetInterval"));

            var patients = await _PatientRepository
                .GetAllPatients(allowedSites);

            return patients;
        }
    }
}