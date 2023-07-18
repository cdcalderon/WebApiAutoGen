using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary.WebBackup;
using YPrime.StudyPortal.Controllers;
using YPrime.StudyPortal.Models;

namespace YPrime.StudyPortal.BaseClasses
{
    public class ControllerWithVisitActivation : BaseController
    {
        protected readonly IPatientVisitRepository _patientVisitRepository;

        protected ControllerWithVisitActivation(
            IPatientVisitRepository patientVisitRepository,
            ISessionService sessionService
            )
            : base(sessionService)
        {
            _patientVisitRepository = patientVisitRepository;
        }

        protected async Task<JsonResult> ActivateVisitLogic(Guid visitId, Guid patientId)
        {
            var ajaxResult = new AjaxResult();

            ajaxResult.Success = await _patientVisitRepository.ActivatePatientVisit(visitId, patientId) != null;
            ajaxResult.Message = "";

            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        protected WebBackupEmailModel ToWebBackupEmailModelDTO(WebBackupEmailWithVisitActivationModel wbewvam)
        {
            return new WebBackupEmailModel
            {
                Id = wbewvam.Id,
                PatientEmail = wbewvam.PatientEmail,
                Subject = wbewvam.Subject,
                EmailContent = wbewvam.EmailContent,
                EmailContentId = wbewvam.EmailContentId,
                WebBackupJwtModel = wbewvam.WebBackupJwtModel
            };
        }
    }
}