using YPrime.Core.BusinessLayer.Interfaces;
using System.Net.Http;
using YPrime.Core.BusinessLayer.Services;
using YPrime.Core.BusinessLayer.Models;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace YPrime.Core.BusinessLayer.Services
{
    public class CountryService : ConfigServiceBase<CountryModel, Guid>, ICountryService
    {
        private const string ConfigEndpoint = "Country";

        public CountryService(
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
