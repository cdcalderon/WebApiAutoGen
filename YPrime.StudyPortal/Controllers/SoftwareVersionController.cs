using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.ManageStudy)]
    public class SoftwareVersionController : BaseController
    {
        private readonly ISoftwareVersionRepository _SoftwareVersionRepository;

        public SoftwareVersionController(
            ISoftwareVersionRepository SoftwareVersionRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _SoftwareVersionRepository = SoftwareVersionRepository;
        }

        [FunctionAuthorization("CanViewSoftwareVersionManagement", "Ability to View Software Version Management Page.", true)]
        public ViewResult Index()
        {
            var vm = _SoftwareVersionRepository.GetAllSoftwareVersions()
                .Select(d => new SoftwareVersionDto
                {
                    Id = d.Id,
                    VersionNumber = d.VersionNumber,
                    Version = _SoftwareVersionRepository.convertToVersion(d.VersionNumber),
                    PackagePath = d.PackagePath
                })
                .OrderByDescending(p => p.Version);

            return View("Index", vm);
        }

        public ViewResult Add()
        {
            var softwareVersion = new SoftwareVersionDto();

            return View(softwareVersion);
        }

        [HttpPost]
        public ActionResult Add(SoftwareVersionDto softwareVersion)
        {
            if (ModelState.IsValid)
            {
                if (softwareVersion.Id == new Guid())
                {
                    if (_SoftwareVersionRepository.CheckVersionNumberIsUsed(softwareVersion.VersionNumber))
                    {
                        ModelState.AddModelError("Version Number", "Version Number already exists");
                        return View(softwareVersion);
                    }

                    if (softwareVersion.ApkFile == null || string.IsNullOrWhiteSpace(softwareVersion.ApkFile.FileName))
                    {
                        ModelState.AddModelError("APK File", "Apk File is required");
                        return View(softwareVersion);
                    }

                    if (Path.GetExtension(softwareVersion.ApkFile.FileName).ToUpper() != ".APK")
                    {
                        ModelState.AddModelError("APK File", "Please select a file with .apk extension");
                        return View(softwareVersion);
                    }

                    var newSoftwareVersion = new SoftwareVersion
                    {
                        Id = Guid.NewGuid(),
                        VersionNumber = softwareVersion.VersionNumber,
                        PlatformTypeId = PlatformType.Android.Id
                    };

                    try
                    {
                        Uri websiteURL;

                        if (Request == null)
                        {
                            websiteURL = new Uri("http://yprime.com");
                        }
                        else
                        {
                            websiteURL = new Uri(Request.Url, Url.Content("~"));
                        }

                        newSoftwareVersion.PackagePath =
                            _SoftwareVersionRepository.SaveToCDNFolder(softwareVersion, websiteURL);
                        _SoftwareVersionRepository.AddSoftwareVersion(newSoftwareVersion);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("APK File Failure", ex.Message);
                    }
                }
            }

            return View(softwareVersion);
        }

        [HttpGet]
        public ActionResult GetSoftwareVersionGridData()
        {
            var softwareVersions = _SoftwareVersionRepository.GetAllSoftwareVersions()
                .Select(d => new SoftwareVersionDto
                {
                    Id = d.Id,
                    VersionNumber = d.VersionNumber,
                    Version = _SoftwareVersionRepository.convertToVersion(d.VersionNumber),
                    PackagePath = d.PackagePath
                })
                .OrderByDescending(p => p.Version);

            return Json(softwareVersions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSoftwareVersionIdsAssignedToReleases()
        {
            var versionIdList = _SoftwareVersionRepository.GetSoftwareVersionIdsAssignedToReleases();

            return Json(versionIdList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSoftwareVersionById(Guid softwareVersionId)
        {
            _SoftwareVersionRepository.DeleteSoftwareVersionById(softwareVersionId);

            return Json(new {success = true});
        }
    }
}