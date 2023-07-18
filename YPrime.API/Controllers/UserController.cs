using System;
using System.Web.Http;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ISessionService sessionService) : base(sessionService)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IHttpActionResult GetById([FromUri] Guid[] ids)
        {
            return Ok(_userRepository.GetUser(ids, DefaultCultureCode));
        }

        [HttpGet]
        public IHttpActionResult GetSiteUserByRole(Guid siteId, string role)
        {
            var users = _userRepository.GetSiteUserByRole(siteId, role);
            return Ok(users);
        }
    }
}