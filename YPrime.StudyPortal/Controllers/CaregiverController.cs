using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Extensions;

namespace YPrime.StudyPortal.Controllers
{
    public class CareGiverController : BaseController
    {
        private readonly IPatientRepository _PatientRepository;
        private readonly ICareGiverTypeService _CareGiverTypeService;
        private readonly IStudySettingService _StudySettingService;
        private readonly ITranslationService _TranslationService;

        public CareGiverController(
            IPatientRepository patientRepository,
            ISessionService sessionService,
            ICareGiverTypeService careGiverTypeService,
            IStudySettingService studySettingService,
            ITranslationService translationService)
            : base(sessionService)
        {
            _PatientRepository = patientRepository;
            _CareGiverTypeService = careGiverTypeService;
            _StudySettingService = studySettingService;
            _TranslationService = translationService;
        }

        // GET: Caregiver
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [FunctionAuthorizationAttribute("CanResetCareGiverPin", "Can reset the pin for a caregiver.")]
        public ActionResult ResetPIN(Guid CareGiverId, bool Update, string TemporaryPin)
        {
            bool success = false;
            if (Update)
            {
                success = _PatientRepository.ResetCareGiverPin(CareGiverId, TemporaryPin);
            }

            ViewBag.Update = true;
            ViewBag.CareGiverName = ExecuteAsyncActionSynchronously(() => _PatientRepository.GetCareGiverTypeName(CareGiverId));
            ViewBag.TemporaryPin = ExecuteAsyncActionSynchronously(() => _PatientRepository.GenerateDefaultPin());
            ViewBag.CareGiverId = CareGiverId;
            ViewBag.Success = success;
            return PartialView();
        }

        [NoDirectAccess]
        [FunctionAuthorizationAttribute("CanViewCareGiverDetails", "Can view caregiver details.", true)]
        public ActionResult DisplayCareGivers(Guid? guid)
        {
            ViewBag.PatientId = guid;
            if (guid == null)
            {
                throw new Exception();
            }

            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> AddCaregiver(Guid PatientId, Guid CaregiverTypeId)
        {
            
            var response = new Dictionary<string, string>();
            var result = await _PatientRepository.InsertCareGiver(PatientId, CaregiverTypeId);

            response.Add("success", result.ToString());

            return Json(response);
        }

        public ActionResult GetCareGiverGrid(Guid PatientId)
        {
            
            var grid = ExecuteAsyncActionSynchronously(() => _PatientRepository
                .GetCareGivers(PatientId))
                .ToList();
            ViewBag.PatientId = PatientId;
            ViewBag.CareGiverTypes = ExecuteAsyncActionSynchronously(() => InflateCareGiverTypeDropdown(grid));
            ViewBag.CaregiverMessage = ExecuteAsyncActionSynchronously(() => GetCaregiverConsentMessage());
            ViewBag.CanCreateCaregiver = User.CanCreateCaregiver();

            return PartialView(grid);
        }

        private async Task<SelectList> InflateCareGiverTypeDropdown(List<CareGiverDto> caregiverList)
        {
            var caregiverTypes = await _CareGiverTypeService.GetAll();
            var caregiverTypesInUse = caregiverList.Select(c => c.CareGiverTypeId);
            var filteredList = caregiverTypes.Where(c => !caregiverTypesInUse.Contains(c.Id));

            return this.GetCareGiverTypeSelectList(filteredList.OrderBy(c => c.Name));
        }

        private async Task<string> GetCaregiverConsentMessage()
        {
            var pinLengthSetting = await _StudySettingService.GetIntValue("PatientPINLength");
            var caregiverTranslationKey = pinLengthSetting == 4 ? "CaregiverPinDefault" : "CaregiverPinDefaultSixDigit";
            var caregiverMessage = await _TranslationService.GetByKey(caregiverTranslationKey);

            return caregiverMessage;
        }
    }
}