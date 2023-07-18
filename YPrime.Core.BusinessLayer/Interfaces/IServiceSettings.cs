using System;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IServiceSettings
    {
        Guid StudyId { get; set; }

        string AuthUrl { get; set; }

        string StudyBuilderAppEnvironment { get; set; }

        string StudyPortalAppEnvironment { get; set; }

        string InventoryAppEnvironment { get; set; }

        int SlidingCacheExpirationSeconds { get; set; }

        string HMACAuthSharedKey { get; set; }

        Guid SponsorId { get; set; }

        string eConsentUrl { get; set; }
    }
}
