using System;
using System.Web.Http;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class SiteController : BaseApiController
    {
        private readonly ISiteRepository _siteRepository;

        public SiteController(ISiteRepository siteRepository, ISessionService sessionService) : base(sessionService)
        {
            _siteRepository = siteRepository;
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult GetById([FromUri] Guid[] ids)
        {
            var results = _siteRepository.GetById(ids);
            return Ok(results);
        }
    }
}