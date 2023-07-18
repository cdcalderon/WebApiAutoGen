using System.Threading.Tasks;
using System.Web.Http;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Exceptions;
using System;

namespace YPrime.API.Controllers
{
    public class BusinessRuleController : BaseApiController
    {
        private readonly IAlarmRepository _alarmRepository;

        public BusinessRuleController(IAlarmRepository alarmRepository, ISessionService sessionService)
            : base(sessionService)
        {
            _alarmRepository = alarmRepository;
        }

        [Route("api/BusinessRule/RunAlarmRule")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> RunAlarmRule(AlarmRuleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IHttpActionResult result;

            try
            {
                var ruleResult = await _alarmRepository.ExecuteAlarmBusinessRule(request);
                result = Ok(ruleResult);
            }
            catch (AlarmNotFoundException ae)
            {
                result = GetBadRequestResult(ae);
            }
            catch(PatientNotFoundException pe)
            {
                result = GetBadRequestResult(pe);
            }
            catch(DeviceNotFoundException de)
            {
                result = GetBadRequestResult(de);
            }
            catch(BusinessRuleException be)
            {
                result = GetBadRequestResult(be);
            }
            catch
            {
                result = InternalServerError();
            }

            return result;
        }

        private IHttpActionResult GetBadRequestResult(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
