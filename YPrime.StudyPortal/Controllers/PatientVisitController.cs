using System;
using System.Linq;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Attributes;
using System.Threading.Tasks;
using YPrime.StudyPortal.Models;
using YPrime.StudyPortal.BaseClasses;

namespace YPrime.StudyPortal.Controllers
{
    public class PatientVisitController : ControllerWithVisitActivation
    {
        private readonly ICareGiverTypeService _careGiverTypeService;

        public PatientVisitController(
            IPatientVisitRepository patientVisitRepository,
            ISessionService sessionService,
            ICareGiverTypeService careGiverTypeService)
            : base(patientVisitRepository, sessionService)
        {
            _careGiverTypeService = careGiverTypeService;
        }

        public async Task<ActionResult> IndexAccordian(Guid PatientId)
        {
            var canAccessTabletWebBackUp = CurrentStudyRole.SystemActions.Any(x => x.Name == "CanAccessTabletWebBackup");
            var canActivateVisitPortal = CurrentStudyRole.SystemActions.Any(x => x.Name == "CanActivateVisitInPortal");

            var summaries = await _patientVisitRepository.GetPatientVisitSummary(
                PatientId,
                canActivateVisitPortal,
                canAccessTabletWebBackUp);

            var caregivers = await _careGiverTypeService.GetAll();
            ViewBag.CareGiverTypes = caregivers.ToDictionary(c => c.Id.ToString(), c => c.Name);
          
            return PartialView("IndexAccordian", summaries);
        }

        [FunctionAuthorization("CanActivateVisitInPortal", "Can Activate Visits in Portal.")]
        public async Task<JsonResult> ActivateVisit(Guid visitId, Guid patientId)
        {
            return await ActivateVisitLogic(visitId, patientId);
        }
    }
}