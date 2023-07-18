using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class CareGiverTypeService : ConfigServiceBase<CareGiverTypeModel, Guid>, ICareGiverTypeService
    {
        private const string ConfigEndpoint = "CareGiverType";

        public CareGiverTypeService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public async Task<List<CareGiverTypeModel>> GetAllAlphabetical(Guid? configurationId = null)
        {
            var careGiverTypes = await GetAll(configurationId);

            return careGiverTypes
                .OrderBy(q => q.Name)
                .ToList();
        }
    }
}


