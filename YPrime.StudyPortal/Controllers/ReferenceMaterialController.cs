using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.ReferenceMaterials)]
    public class ReferenceMaterialController : BaseController
    {
        private readonly IReferenceMaterialRepository _ReferenceMaterialRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IFileService _fileService;

        public ReferenceMaterialController(
            IUserRepository UserRepository,
            IReferenceMaterialRepository ReferenceMaterialRepository,
            IFileService fileService,
            ISessionService sessionService)
            : base(sessionService)
        {
            _UserRepository = UserRepository;
            _ReferenceMaterialRepository = ReferenceMaterialRepository;
            _fileService = fileService;
        }

        [FunctionAuthorization("CanViewReferenceMatieralList", "View patient materials", true)]
        public ActionResult Index()
        {
            var model = _ReferenceMaterialRepository.GetReferenceMaterialTypeWithMaterials();
            var DownloadPermission =
                CurrentStudyRole.SystemActions.FirstOrDefault(x => x.Name == "CanDownloadReferenceMaterials");
            ViewBag.CanDownload = (DownloadPermission != null);
            return View(model);
        }

        [FunctionAuthorization("CanUploadReferenceMaterials", "Upload reference materials.", true)]
        public async Task<ActionResult> Upload()
        {
            var model = new ReferenceMaterialDto
            {
                UserId = User.Id
            };
            await SetViewBag();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FunctionAuthorization("CanCreateReferenceMaterials", "Create Reference materials", true)]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,ReferenceMaterialTypeId,Name,Path,File,FileName")]
            ReferenceMaterialDto referenceMaterialDto)
        {
            //Validate name 
            if (_ReferenceMaterialRepository.ReferenceMaterialNameExists(referenceMaterialDto.Name))
            {
                ModelState.AddModelError("NameExists", $"The name '{referenceMaterialDto.Name}' already exists.");
            }

            //Validate fileName
            if (_ReferenceMaterialRepository.ReferenceMaterialFileNameExists(referenceMaterialDto.File.FileName))
            {
                ModelState.AddModelError("FileNameExists",
                    $"The file '{referenceMaterialDto.File.FileName}' already exists.");
            }

            // File extension is valid
            if (!_ReferenceMaterialRepository.CheckUploadedFileNameExtension(referenceMaterialDto.File.FileName))
            {
                string AllowedExtensions = _ReferenceMaterialRepository.AllowedFileExtensions();
                ModelState.AddModelError("FileTypeNotSupported",
                    $"File '{referenceMaterialDto.File.FileName}' is not supported for upload. (" + AllowedExtensions +
                    " Only)");
            }
            else
            {
                // File MIME type agrees with file extension
                if (!_ReferenceMaterialRepository.CheckUploadedFileMimeContents(referenceMaterialDto.File))
                {
                    ModelState.AddModelError("FileMimeTypeMismatch",
                        $"The file contents for file '{referenceMaterialDto.File.FileName}' do not match the reported MIME Type.");
                }
            }


            referenceMaterialDto.FileName = referenceMaterialDto.File.FileName;

            //Save & create          
            if (ModelState.IsValid)
            {
                _ReferenceMaterialRepository.CreateReferenceMaterial(referenceMaterialDto);

                await _fileService.UploadReferenceMaterial(referenceMaterialDto);
                return Json(
                    new
                    {
                        success = true,
                        responseText = $"Reference Material '{referenceMaterialDto.Name}' was successfully uploaded."
                    }, JsonRequestBehavior.AllowGet);
            }

            var errors = ModelState.GetModelErrors();
            return Json(new {success = false, responseText = string.Join(",", errors.Select(e => e.Value))},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionAuthorization("CanDeleteReferenceMaterials", "Delete Reference Materials", true)]
        public async Task<ActionResult> Delete(Guid referenceMaterialId)
        {
            var referenceMaterial = _ReferenceMaterialRepository.GetReferenceMaterial(referenceMaterialId);
            await _fileService.DeleteReferenceMaterial(referenceMaterial.UserId, referenceMaterial.Id,
                referenceMaterial.FileName);
            _ReferenceMaterialRepository.DeleteReferenceMaterial(referenceMaterial.Id);
            return Json(
                new
                {
                    success = true,
                    responseText = $"Reference Material '{referenceMaterial.Name}' was successfully deleted."
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [FunctionAuthorization("CanDownloadReferenceMaterials", "Download reference materials.", true)]
        public async Task<ActionResult> Download(Guid referenceMaterialId)
        {
            var referenceMaterial = _ReferenceMaterialRepository.GetReferenceMaterial(referenceMaterialId);
            if (referenceMaterial == null)
            {
                 throw new Exception();
            }

            var referenceMaterialFile = await _fileService.GetReferenceMaterial(referenceMaterial);
            var contentDisposition = new ContentDisposition
            {
                FileName = referenceMaterial.FileName,
                Inline = true // Browser will try to open inline
            };
            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());
            return File(referenceMaterialFile, referenceMaterial.ContentType);
        }

        private async Task SetViewBag()
        {
            var referenceMaterialTypes = _ReferenceMaterialRepository.GetReferenceMaterialTypes();
            var DeletePermission =
                CurrentStudyRole.SystemActions.FirstOrDefault(x => x.Name == "CanDeleteReferenceMaterials");

            ViewBag.Username = User.UserName;
            ViewBag.ReferenceMaterialTypeSelectList = new SelectList(referenceMaterialTypes, "Id", "Name", null);
            ViewBag.MenuGroup = "ManageStudy";
            ViewBag.CanDeleteFile = (DeletePermission != null);
        }

        [HttpGet]
        public ActionResult GetReferenceMaterialGridData()
        {
            var referenceMaterials = _ReferenceMaterialRepository
                .GetReferenceMaterials()
                .ToList();

            return Json(referenceMaterials, JsonRequestBehavior.AllowGet);
        }
    }
}