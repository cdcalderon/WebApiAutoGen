using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.WebBackup;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.BaseClasses;

namespace YPrime.StudyPortal.Controllers
{
    public class WebBackupController : ControllerWithVisitActivation
    {
        private const string WebBackupEmailFromAddress = "confirmation@yprime.com";
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IWebBackupRepository _webBackupRepository;
        private readonly IStudySettingService _studySettingService;
        private readonly IServiceSettings _serviceSettings;

        public WebBackupController(
            ISiteRepository siteRepository,
            IDeviceRepository deviceRepository,
            IWebBackupRepository webBackupRepository,
            IConfirmationRepository confirmationRepository,
            IStudySettingService studySettingService,
            IPatientVisitRepository patientVisitRepository,
            ISessionService sessionService,
            IServiceSettings serviceSettings)
            : base(patientVisitRepository, sessionService)
        {
            _siteRepository = siteRepository;
            _deviceRepository = deviceRepository;
            _webBackupRepository = webBackupRepository;
            _confirmationRepository = confirmationRepository;
            _studySettingService = studySettingService;
            _serviceSettings = serviceSettings;
        }

        [FunctionAuthorization("CanAccessTabletWebBackup", "Ability to access web backup page (Tablet).", true)]
        public ActionResult Index()
        {
            SetBaseViewState();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetSiteGridData()
        {
            var sites = await _siteRepository
                .GetSitesForUser(User.Id);

            foreach (var site in sites.Where(x => x.WebBackupExpireDate != null && x.WebBackupExpireDate >= x.TimeZone.ToLocalDateTime())
                .ToList())
            {
                var device = _deviceRepository.GetWebBackupTabletDevice(site.Id);
                site.WebBackupAssetTag = device?.AssetTag;
            }

            return Json(sites, JsonRequestBehavior.AllowGet);
        }

        [FunctionAuthorizationAttribute("CanAccessTabletWebBackup", "Ability to access web backup page (Tablet).",
            true)]
        public async Task<ActionResult> WebBackupClinician(Guid siteId)
        {
            SetBaseViewState();

            var hostAddress = Request.UserHostAddress;

            var model = await _webBackupRepository.GetClinicianWebBackupModel(
                siteId,
                hostAddress);

            return View("WebBackupPage", model);
        }

        public ActionResult CreateWebBackupHandheldLink()
        {
            return new RedirectResult(string.Empty);
        }

        [AllowAnonymous]
        public async Task<ActionResult> WebBackupHandheld(string token)
        {
            var hostAddress = Request.UserHostAddress;

            var model = await _webBackupRepository.GetSubjectWebBackupModel(token, hostAddress);

            return View("WebBackupPage", model);
        }

        [HttpPost]
        public async Task<ActionResult> SendWebBackupEmail(WebBackupEmailModel webBackupEmail)
        {
            var response = new Dictionary<string, string>();
            if (ModelState.IsValid)
            {
                response = await SendWebBackupEmailLogic(webBackupEmail);
            }

            return Json(response);
        }

        [FunctionAuthorization("CanActivateVisitInPortal", "Can Activate Visits in Portal.")]
        public async Task<ActionResult> SendWebBackupEmailWithVisitActivation(WebBackupEmailWithVisitActivationModel webBackupEmailWithVisitActivation)
        {
            var response = new Dictionary<string, string>();
            if (ModelState.IsValid)
            {
                var activateVisitResponse = await ActivateVisitLogic(webBackupEmailWithVisitActivation.VisitId, webBackupEmailWithVisitActivation.PatientId);

                response = await SendWebBackupEmailLogic(ToWebBackupEmailModelDTO(webBackupEmailWithVisitActivation));
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult CreateWebBackupEmailBody(WebBackupJwtModel jwtModel)
        {
            var response = new Dictionary<string, string>();

            var emailModel = ExecuteAsyncActionSynchronously(() =>
                _webBackupRepository.CreateWebBackupEmailModel(jwtModel.PatientId, jwtModel.SiteId, jwtModel.WebBackupType, jwtModel.VisitId));

            emailModel.WebBackupJwtModel.CaregiverId = jwtModel.CaregiverId;
            
            var result = ExecuteAsyncActionSynchronously(() => _webBackupRepository.CreateWebBackupEmailBody(
                Request,
                Url,
                emailModel.WebBackupJwtModel));
            
            response.Add("Message", result);
            response.Add("EmailModel", Newtonsoft.Json.JsonConvert.SerializeObject(emailModel));            
            return Json(response);
        }

        private async Task<Dictionary<string, string>> SendWebBackupEmailLogic(WebBackupEmailModel webBackupEmail)
        {
            var response = new Dictionary<string, string>();
            var studyId = new Guid();

            Guid.TryParse(await _studySettingService.GetStringValue("StudyID"), out studyId);

            var email = new SendingEmailModel
            {
                Id = Guid.NewGuid(),
                To = new List<string>
                    {
                        webBackupEmail.PatientEmail
                    },
                Cc = new List<string>(),
                Bcc = new List<string>(),
                ToUsers = new List<Guid>(),
                CcUsers = new List<Guid>(),
                BccUsers = new List<Guid>(),
                From = WebBackupEmailFromAddress,
                Subject = webBackupEmail.Subject,
                Body = webBackupEmail.EmailContent,
                CreatedDate = DateTime.Now,
                Attachments = new Dictionary<string, byte[]>(),
                StudyId = studyId,
                SponsorId = _serviceSettings.SponsorId,
                Environment = _serviceSettings.StudyPortalAppEnvironment
            };

            _confirmationRepository.SaveConfirmation(email.Id, email.Subject, email.Body, string.Empty,
                webBackupEmail.EmailContentId, User.Id, webBackupEmail.WebBackupJwtModel.SiteId,
                new List<EmailRecipient>());
            await _confirmationRepository.SendApiEmail(email);
            response.Add("Success", "true");

            return response;
        }
    }
}