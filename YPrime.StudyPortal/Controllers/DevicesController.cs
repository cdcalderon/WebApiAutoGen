using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal.Controllers
{
    public class DevicesController : BaseController
    {
        private readonly IDeviceRepository _DeviceRepository;
        private readonly ISyncLogRepository _SyncLogRepository;
        private readonly string CurrentDeviceIdSessionKey = "CurrentDeviceGUID";

        public DevicesController(IDeviceRepository DeviceRepository,
            ISyncLogRepository SyncLogRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _DeviceRepository = DeviceRepository;
            _SyncLogRepository = SyncLogRepository;
        }

        public Guid? CurrentDeviceId
        {
            get
            {
                return Session[CurrentDeviceIdSessionKey] != null
                    ? (Guid)Session[CurrentDeviceIdSessionKey]
                    : (Guid?)null;
            }
            set { Session[CurrentDeviceIdSessionKey] = value; }
        }

        [FunctionAuthorizationAttribute("CanUseDeviceWidget", "Can use the device widget.", true)]
        public ActionResult Widget()
        {
            ViewData["TotalDevices"] = _DeviceRepository.GetTotalDevicesAtSite(CurrentSiteId, User.Sites);
            ViewData["PhoneDevices"] =
                _DeviceRepository.GetDevicesAtSiteCountByDeviceType(CurrentSiteId, User.Sites, "Phone");
            ViewData["TabletDevices"] =
                _DeviceRepository.GetDevicesAtSiteCountByDeviceType(CurrentSiteId, User.Sites, "Tablet");

            return PartialView();
        }

        [NoDirectAccess]
        [FunctionAuthorization("CanViewDeviceList", "Can view the list of devices in the system.", true)]
        public ActionResult Index()
        {
            SetViewData();

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetDevicesGridData()
        {
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            var siteList = User.Sites.Select(s => s.Id).ToList();

            var devices = (await _DeviceRepository
                .GetAllDevices(siteList))
                .ToList();

            devices.ForEach(x => x.DeviceDrilldownButtonHTML = htmlHelper
                .PrimeActionLink(string.IsNullOrEmpty(x.AssetTag) ? "N/A" : x.AssetTag, "Details", "Devices",
                    new { id = x.Id }).ToHtmlString());

            return Json(devices, JsonRequestBehavior.AllowGet);
        }

        [FunctionAuthorization("CanViewDeviceDetails", "Can view the details page for a device .", true)]
        public async Task<ActionResult> Details(Guid id)
        {
            var siteList = User.Sites.Select(s => s.Id).ToList();
            var device = await _DeviceRepository.GetDevice(id, siteList);
            CurrentDeviceId = id;

            SetViewData();
            return View(device);
        }

        [HttpGet]
        public async Task<ActionResult> GetSyncLogsGridData()
        {
            var syncLogs = await _SyncLogRepository
                .GetSyncLogsByDevice((Guid) CurrentDeviceId, true);

            return Json(syncLogs, JsonRequestBehavior.AllowGet);
        }

        private void SetViewData()
        {
            ViewBag.CurrentSiteName = CurrentSiteDto?.Name ?? "No Site Selected";
        }
    }
}