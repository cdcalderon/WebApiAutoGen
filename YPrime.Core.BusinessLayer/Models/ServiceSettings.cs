using System;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    public class ServiceSettings : IServiceSettings
    {
        public Guid StudyId { get; set; }

        public string AuthUrl { get; set; }

        public string StudyBuilderAppEnvironment { get; set; }

        public string InventoryAppEnvironment { get; set; }

        public string StudyPortalAppEnvironment { get; set; }

        public int SlidingCacheExpirationSeconds { get; set; }

        public string HMACAuthSharedKey { get; set; }

        public Guid SponsorId { get; set; }

        public string eConsentUrl { get; set; }
    }
}
