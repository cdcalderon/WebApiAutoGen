using System.Web.Http;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class BaseApiController : ApiController
    {
        public string DefaultCultureCode => "en-US";
        protected readonly ISessionService _sessionService;

        protected BaseApiController(ISessionService sessionService)
        {
            _sessionService = sessionService;
            SetSessionServiceState();
        }

        protected void SetSessionServiceState()
        {
            _sessionService.UserConfigurationId = YPrimeSession.Instance.ConfigurationId;
        }
    }
}