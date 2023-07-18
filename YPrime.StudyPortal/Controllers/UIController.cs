using Config.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.Shared.Helpers.Data;
using YPrime.StudyPortal.Helpers;
using YPrime.StudyPortal.Models;
using YPrime.BusinessLayer.Constants;
using System.Security.Claims;

namespace YPrime.StudyPortal.Controllers
{
    public class UIController : BaseController
    {
        private readonly IAnswerRepository _AnswerRepository;
        private readonly IAuthenticationUserRepository _AuthenticationRepository;
        private readonly ICorrectionRepository _CorrectionRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IReportRepository _ReportRepository;
        private readonly ISiteRepository _SiteRepository;
        private readonly IWebBackupRepository _webBackupRepository;
        private readonly IStudySettingService _studySettingService;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly ICountryService _countryService;

        public UIController(
            ISiteRepository SiteRepository, 
            IAnswerRepository AnswerRepository,
            IReportRepository ReportRepository,
            ICorrectionRepository CorrectionRepository,
            IAuthenticationUserRepository AuthenticationRespository, 
            IDeviceRepository deviceRepository,
            IWebBackupRepository webBackupRepository,
            IStudySettingService studySettingService,
            ISystemSettingRepository systemSettingRepository,
            ISessionService sessionService,
            ICountryService countryService)
            : base(sessionService)
        {
            _SiteRepository = SiteRepository;
            _AnswerRepository = AnswerRepository;
            _ReportRepository = ReportRepository;
            _CorrectionRepository = CorrectionRepository;
            _AuthenticationRepository = AuthenticationRespository;
            _deviceRepository = deviceRepository;
            _webBackupRepository = webBackupRepository;
            _studySettingService = studySettingService;
            _systemSettingRepository = systemSettingRepository;
            _countryService = countryService;
        }

        public ActionResult MainMenu()
        {

            ViewBag.WebBackupDays = ExecuteAsyncActionSynchronously(() => _studySettingService.GetIntValue("WebBackupTabletEnabled", 0));
            ViewBag.PublicKey = _systemSettingRepository.GetSystemSettingValue("WebBackupTabletPublicKey");
            ViewBag.WebBackupAssetTag = "";
            ViewBag.SiteId = "";
            ViewBag.ValidTo = "";
            ViewBag.WebBackupURL = "";

            int NumSites = User.Sites.Count(x =>
                x.WebBackupExpireDate != null && x.WebBackupExpireDate >= x.TimeZone.ToLocalDateTime());
            if (NumSites == 1)
            {
                SiteDto siteDto = User.Sites.First(x =>
                    x.WebBackupExpireDate != null && x.WebBackupExpireDate >= x.TimeZone.ToLocalDateTime());

                var siteId = siteDto.Id;
                var device = _deviceRepository.GetWebBackupTabletDevice(siteId);
                ViewBag.WebBackupAssetTag = device?.AssetTag;
                ViewBag.SiteId = siteId;
                if (device != null)
                {
                    ViewBag.WebBackupURL = _webBackupRepository.GetWebBackupUrl(
                        WebBackupType.TabletClinician,
                        device.Id,
                        device.DeviceTypeId,
                        siteId,
                        ViewBag.WebBackupAssetTag,
                        null,
                        siteDto.TimeZone,
                        null);
                }

                DateTime ValidUntil = (DateTime) siteDto.WebBackupExpireDate;
                ViewBag.ValidTo = "Expires " + ValidUntil.ToString("d-MMM");
            }

            ViewData["MainSiteSelection"] = SelectListHelper.GetSitesList(User.Sites, CurrentSiteId);

   
            ViewData["MainSite"] = CurrentSiteId != null || CurrentSiteId == Guid.Empty
                ? ExecuteAsyncActionSynchronously(() => _SiteRepository.GetSite((Guid) CurrentSiteId))
                : new SiteDto {Name = SelectListHelper.AllSitesText};
            ViewBag.CanAccessTabletWebBackup = CurrentStudyRole == null ? false :
                CurrentStudyRole.SystemActions.Any(x => x.Name == "CanAccessTabletWebBackup") && NumSites > 0;
            ViewBag.NumOfEligibleSites = NumSites;
            ViewBag.SiteUserCultureCode = CurrentSiteUserCultureCode;

            SetBaseViewState();
            return PartialView();
        }

        public ActionResult DCFWidget()
        {
            var userId = User.Id;
            var cultureCode = CurrentSiteUserCultureCode;

            var correctionWorkflows = ExecuteAsyncActionSynchronously(() => _CorrectionRepository.GetCorrectionListForUser(userId, cultureCode));

            return PartialView(correctionWorkflows);
        }

        public async Task<ActionResult> WidgetChart(Guid ReportId)
        {
            var pars = new Dictionary<string, object>();
            pars.Add("Sites", User.Sites);
            pars.Add("SelectedSiteId", CurrentSiteId);

            var chart = await _ReportRepository.GetReportChartData(ReportId, User.Id, pars);
            chart.HideContainer = true;
            return PartialView(chart);
        }

        public async Task<ActionResult> ElectronicSignature(string correctionAction = "", string siteId = "")
        {
            ViewBag.FirstName = User.FirstName;
            ViewBag.LastName = User.LastName;

            if (string.IsNullOrEmpty(correctionAction))
            {
                ViewBag.CorrectionAction = CorrectionActionElectronicSignatureEnum.Pending;
            }
            else if (correctionAction == CorrectionActionEnum.Approved.ToString())
            {
                ViewBag.CorrectionAction = CorrectionActionElectronicSignatureEnum.Approved;
            }
            else if (correctionAction == CorrectionActionEnum.Rejected.ToString())
            {
                ViewBag.CorrectionAction = CorrectionActionElectronicSignatureEnum.Rejected;
            }
            else if (correctionAction == CorrectionActionEnum.NeedsMoreInformation.ToString())
            {
                ViewBag.CorrectionAction = CorrectionActionElectronicSignatureEnum.NeedsMoreInformation;
            }
            else
            {
                ViewBag.CorrectionAction = CorrectionActionElectronicSignatureEnum.Pending;
            }

            if(siteId != string.Empty)
            {
                var siteTime = _SiteRepository.GetSiteLocalTime(Guid.Parse(siteId));
                var site = await _SiteRepository.GetSite(Guid.Parse(siteId));
                var country = await _countryService.Get(site.CountryId);
                if(country.Use12HourTime)
                {
                    ViewBag.CurrentTimeString = siteTime.ToString("dd'-'MMM'-'yyyy hh':'mm tt");
                }
                else
                {
                    ViewBag.CurrentTimeString = siteTime.ToString("dd'-'MMM'-'yyyy HH':'mm");
                }
            }

            return PartialView();
        }

        public ActionResult Loading()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [OutputCache(
            Duration = 100,
            VaryByParam = "Id",
            Location = OutputCacheLocation.ServerAndClient)]
        public async Task<FileResult> GetAnswerImage(Guid Id)
        {
            var resultImage = await _AnswerRepository.GetImage(Id);

            return File(ImageHelper.ImageToByteArray(resultImage), "image/png");
        }
    }
}