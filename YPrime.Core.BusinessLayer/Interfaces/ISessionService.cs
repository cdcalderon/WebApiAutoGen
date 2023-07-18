using System;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface ISessionService
    {
        Guid UserConfigurationId { get; set; }
        string User { get; set; }
    }
}
