using System;
using System.Web.Http;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class PatientVisitController : BaseApiController
    {
        private readonly IPatientVisitRepository _patientVisitRepository;

        public PatientVisitController(IPatientVisitRepository patientVisitRepository, ISessionService sessionService) : base(sessionService)
        {
            _patientVisitRepository = patientVisitRepository;
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult GetById([FromUri] Guid[] ids)
        {
            var patientVisits = _patientVisitRepository.GetById(ids, DefaultCultureCode);
            return Ok(patientVisits);
        }
    }
}