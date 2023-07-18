using Hangfire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.ManageStudy)]
    public class ExportController : BaseController
    {
        private readonly IExportRepository _ExportRepository;
        private readonly IPatientRepository _PatientRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IFileService _fileService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ExportController(
            IUserRepository UserRepository,
            IPatientRepository PatientRepository,
            IExportRepository ExportRepository,
            IFileService fileService,
            ISessionService sessionService,
            IBackgroundJobClient backgroundJobClient)
            : base(sessionService)
        {
            _UserRepository = UserRepository;
            _PatientRepository = PatientRepository;
            _ExportRepository = ExportRepository;
            _fileService = fileService;
            _backgroundJobClient = backgroundJobClient;
        }

        [FunctionAuthorization("CanViewExport", "Can View export.")]
        public async Task<ActionResult> Index()
        {
            var exportDto = new ExportDto
            {
                UserId = User.Id
            };
            await SetViewBag(exportDto);
            return View(exportDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FunctionAuthorization("CanCreateExport", "Can create export.")]
        public async Task<ActionResult> Create([Bind(Include =
                "Id,Name,UserId,SiteId,PatientId,PatientStatusTypeID,QuestionnaireTypeId,DiaryStartDate,DiaryEndDate")]
            ExportDto exportDto)
        {
            if (_ExportRepository.ExportExists(exportDto.Name))
            {
                ModelState.AddModelError("ExportExists", $"An export already exists with the name '{exportDto.Name}'");
            }

            if (ModelState.IsValid)
            {
                if (exportDto.SiteId == Guid.Empty)
                {
                    exportDto.SiteId = null;
                }

                if (exportDto.PatientId == Guid.Empty)
                {
                    exportDto.PatientId = null;
                }

                //Save & create archive          
                _ExportRepository.CreateExport(exportDto);
                await SetViewBag(exportDto);
                _backgroundJobClient.Enqueue(() => ExecuteExport(User.Id, exportDto.Id));
            }
            else
            {
                await SetViewBag(exportDto);
            }

            return PartialView("_Create", exportDto);
        }

        [HttpPost]
        public async Task<JsonResult> UpdatePatientSelectList(Guid siteId)
        {
            IEnumerable<PatientDto> patientDtos;

            if (siteId == null || siteId == Guid.Empty)
            {
                patientDtos = await _PatientRepository.GetAllPatients(User.Sites.Select(s => s.Id));
            }
            else
            {
                patientDtos = await _PatientRepository.GetAllPatients(new List<Guid> {siteId});
            }
            
            return Json(SelectListHelper.GetPatientsList(patientDtos.ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [FunctionAuthorization("CanDownloadExportResults", "Can download export results")]
        public async Task<ActionResult> DownloadExport(Guid exportId)
        {
            var export = await _ExportRepository.GetExport(exportId);

            if (export == null)
            {
                 throw new Exception();
            }

            var exportArchive = await _fileService.GetExportArchive(export);
            return File(exportArchive, "application/zip",
                $"{export.Name}-{export.CompletedTime.Value.ToString("yyyy-dd-MM--HH-mm-ss")}.zip");
        }

        [HttpPost]
        [FunctionAuthorization("CanRunExport", "Can run export")]
        public JsonResult RunExport(Guid exportId)
        {
           _backgroundJobClient.Enqueue(() => ExecuteExport(User.Id, exportId));
            return Json("", JsonRequestBehavior.AllowGet);
        }

        private async Task SetViewBag(ExportDto exportDto)
        {
            IEnumerable<PatientDto> patientDtos;

            if (exportDto.SiteId == null || exportDto.SiteId == Guid.Empty)
            {
                var result = await _PatientRepository.GetAllPatients(User.Sites.Select(s => s.Id).ToList());
                patientDtos = result.ToList();
            }
            else
            {
                var result = await _PatientRepository.GetAllPatients(new List<Guid> {(Guid) exportDto.SiteId});
                patientDtos = result.ToList();
            }

            ViewBag.Username = User.UserName;
            ViewBag.SiteSelectList = SelectListHelper.GetSitesList(User.Sites, exportDto.SiteId);
            ViewBag.PatientSelectList = SelectListHelper.GetPatientsList(patientDtos.ToList());
        }

        [AutomaticRetry(Attempts = 5,DelaysInSeconds = new int[] { 900 })]
        [DisableConcurrentExecution(timeoutInSeconds: 600)]
        public async Task ExecuteExport(Guid UserId, Guid exportId)
        {
            var exportFiles = await _ExportRepository.ExecuteExport(exportId);
            await _fileService.CreateExportArchive(UserId, exportId, exportFiles);
        }

        /************************************** 
         * ASPNET MVC GRID
         * *************************************/

        [HttpGet]
        public async Task<ActionResult> GetExportGridData()
        {
            var userId = User.Id;
            var data = await _ExportRepository
                .GetExports(userId);

            data.ToList().ForEach(i => i.ActionButtonsHTML = GetExportGridActionButtons(i));

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string GetExportGridActionButtons(ExportDto export)
        {
            /* Create an instance of Ajax and Html helper... */
            var ajaxHelper =
                new AjaxHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            string exportGridButtonHTML = "";

            if (export.ExportStatus != "In Queue" && export.ExportStatus != "Processing")
            {
                if (export.ExportStatus == "Complete")
                {
                    exportGridButtonHTML =
                        htmlHelper.PrimeActionLink("Download", "DownloadExport", "Export", new {exportId = export.Id},
                            null).ToHtmlString()
                        + " | "
                        + ajaxHelper.RawActionLink("Run Export", "RunExport", "Export", new {exportId = export.Id},
                            new AjaxOptions
                                {HttpMethod = "POST", OnSuccess = "runExportSuccess", OnFailure = "runExportFailure"},
                            new {@class = "data-export"}).ToHtmlString();
                }
                else
                {
                    exportGridButtonHTML = ajaxHelper.RawActionLink("Run Export", "RunExport", "Export",
                        new {exportId = export.Id},
                        new AjaxOptions
                            {HttpMethod = "POST", OnSuccess = "runExportSuccess", OnFailure = "runExportFailure"},
                        new {@class = "data-export"}).ToHtmlString();
                }
            }

            return exportGridButtonHTML;
        }
    }
}