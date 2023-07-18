using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal.Controllers
{
    public class SoftwareReleaseController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IConfigurationVersionService _configurationVersionService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;
        private readonly ISoftwareVersionRepository _softwareVersionRepository;
        private readonly IServiceSettings _serviceSettings;

        public SoftwareReleaseController(
            ICountryService countryService,
            IConfigurationVersionService configurationVersionService,
            ISoftwareReleaseRepository softwareReleaseRepository,
            ISoftwareVersionRepository softwareVersionRepository, 
            IDeviceRepository deviceRepository,
            ISiteRepository siteRepository,
            IServiceSettings serviceSettings,
            ISessionService sessionService)
            : base(sessionService)
        {
            _countryService = countryService;
            _configurationVersionService = configurationVersionService;
            _softwareReleaseRepository = softwareReleaseRepository;
            _softwareVersionRepository = softwareVersionRepository;
            _deviceRepository = deviceRepository;
            _siteRepository = siteRepository;
            _serviceSettings = serviceSettings;
        }

        [FunctionAuthorization("CanViewSoftwareReleases", "View Software Releases", false)]
        public async Task<ViewResult> Index()
        {
            var softwareRelease = await SetupDto();

            return View("Index", softwareRelease);
        }

        [HttpPost]
        public async Task<ActionResult> Confirm(SoftwareReleaseDto softwareRelease)
        {
            var response = new Dictionary<string, string>();

            if (!softwareRelease.StudyWide && softwareRelease.CountryIds == null && softwareRelease.SiteIds == null &&
                (softwareRelease.DeviceTypeIds == null || softwareRelease.DeviceTypeIds.Count == 0))
            {
                response.Add("IsValid", "false");
                response.Add("MessageTitle", "Review");
                response.Add("Message", "Software release must be assigned to at least one device.");
            }
            else
            {
                var message = await _softwareReleaseRepository.GetSoftwareReleaseDeviceConfirmation(softwareRelease);

                response.Add("IsValid", "true");
                response.Add("MessageTitle", "Review Software and Configuration Release");
                response.Add("Message", message);
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SoftwareReleaseDto softwareRelease)
        {
            if (ModelState.IsValid)
            {
                await _softwareReleaseRepository.CreateSoftwareRelease(softwareRelease);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> GetReleaseGridData()
        {
            var releases = await _softwareReleaseRepository.GetSoftwareReleaseGridData();
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());
            releases.ForEach(r =>
                r.ViewDevices = htmlHelper.PrimeActionLink("View Devices", "Index", "Devices").ToHtmlString());

            return Json(releases, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCountryListByDeviceType(List<Guid> deviceTypeIds)
        {
            var result = await _softwareReleaseRepository
                .GetCountryDictionaryByDeviceType(deviceTypeIds);

            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSiteListByCountry(List<Guid> deviceTypeIds, List<Guid> countryIds)
        {
            var result = await _softwareReleaseRepository
                .GetSiteDictionaryByCountry(deviceTypeIds, countryIds);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeactivateSoftwareRelease(Guid softwareReleaseId)
        {
            var result = false;

            if (softwareReleaseId != Guid.Empty)
            {
                result = _softwareReleaseRepository.DeactivateSoftwareRelease(softwareReleaseId);
            }

            return Json(new {success = result}, JsonRequestBehavior.AllowGet);
        }

        private async Task<SoftwareReleaseDto> SetupDto()
        {
            var sites = await _siteRepository.GetAllSites();

            var versionList = _softwareVersionRepository
                .GetAllSoftwareVersions()
                .Select(s => s.VersionNumber)
                .ToList();

            var versionSelectList = new SelectList(versionList.OrderByDescending(v => Version.Parse(v))
                .Select(c => new { Text = c, Value = c }), "Value", "Text");

            var countries = await _countryService.GetAll();

            var configVersions = await _configurationVersionService
                .GetAll();

            if (_serviceSettings.IsProductionEnvironment())
            {
                configVersions = configVersions
                    .Where(cv => cv.ApprovedForProd)
                    .ToList();
            }

            var configSelectList = new SelectList(configVersions.OrderByDescending(c => Version.Parse(c.ConfigurationVersionNumber))
                .Select(c => new { Text = c.DisplayVersion, Value = c.Id }), "Value", "Text");

            var deviceTypes = await _softwareReleaseRepository
                .GetProvisionalDeviceTypesForStudy();

            var deviceTypeSelectList = new SelectList(
                deviceTypes.Select(dt => new { Text = dt.Name, Value = dt.Id }),
                "Value",
                "Text");

            var softwareRelease = new SoftwareReleaseDto
            {
                Countries = countries,
                Sites = sites.ToList(),
                VersionList = versionSelectList,
                ConfigVersionList = configSelectList,
                DeviceTypeList = deviceTypeSelectList
            };

            return softwareRelease;
        }
    }
}