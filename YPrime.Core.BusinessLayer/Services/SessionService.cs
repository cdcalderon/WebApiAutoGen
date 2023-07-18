using System;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Services
{
    public class SessionService : ISessionService
    {
        public Guid UserConfigurationId { get; set; }
        public string User { get; set; }
    }
}
