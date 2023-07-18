using System;
using System.Web.Http;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class PatientController : BaseApiController
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository, ISessionService sessionService) : base(sessionService)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult GetById([FromUri] Guid[] ids)
        {
            var patients = _patientRepository.GetPatients(ids);
            return Ok(patients);
        }
    }
}