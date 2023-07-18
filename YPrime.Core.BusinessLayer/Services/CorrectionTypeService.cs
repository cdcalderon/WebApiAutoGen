using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class CorrectionTypeService : ConfigServiceBase<CorrectionTypeModel, Guid>, ICorrectionTypeService
    {
        private const string ConfigEndpoint = "CorrectionType";

        public CorrectionTypeService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }
    }
}
