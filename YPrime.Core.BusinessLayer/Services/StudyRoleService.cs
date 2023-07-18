using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class StudyRoleService : ConfigServiceBase<StudyRoleModel, Guid>, IStudyRoleService
    {
        private const string ConfigEndpoint = "StudyRole";

        public StudyRoleService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public override async Task<List<StudyRoleModel>> GetAll(Guid? configurationId = null)
        {
            var cacheKey = GetCacheKey(ConfigEndpoint, configurationId, null);
            var entities = GetEntityFromCache<List<StudyRoleModel>>(cacheKey);

            if (entities == null)
            {
                entities = await base.GetAll(configurationId);

                _cache.Set(cacheKey, entities, BuildCacheOptions());
            }

            return entities;
        }

        public async Task LoadIntoCache(Guid configurationId)
        {
            await GetAll(configurationId);
        }
    }
}
