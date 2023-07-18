using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class StudySettingService : ConfigServiceBase<StudySettingModel, string>, IStudySettingService
    {
        public const string ConfigEndpoint = "StudySetting";
        public const string StudyCustomEndpointSuffix = "StudyCustom";

        public StudySettingService(
            IHttpClientFactory httpClientFactory, 
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public async Task<string> GetStringValue(
            string key, 
            Guid? configurationId = null)
        {
            var result = await GetSingleStudyCustomValue(key, configurationId);

            return result?.Value;
        }

        public async Task<int> GetIntValue(
            string key, 
            int defaultValue = 0, 
            Guid? configurationId = null)
        {
            var result = await GetSingleStudyCustomValue(key, configurationId);

            return result?.GetIntValue() ?? defaultValue;
        }

        public async Task<Guid> GetGuidValue(
            string key,
            Guid? configurationId = null)
        {
            var result = await GetSingleStudyCustomValue(key, configurationId);

            return result.GetGuidValue();
        }    
        
        public async Task<bool> GetBoolValue(
            string key, 
            bool defaultValue = false,
            Guid? configurationId = null)
        {
            var result = await GetSingleStudyCustomValue(key, configurationId);

            return result?.GetBoolValue() ?? defaultValue;
        }

        public Task<List<StudyCustomModel>> GetAllStudyCustoms(
            Guid? configurationId = null)
        {
            var cacheKey = GetCacheKey(StudyCustomEndpointSuffix, configurationId, null);
            var resultTask = GetEntityFromCache<Task<List<StudyCustomModel>>>(cacheKey);

            if (resultTask == null)
            {
                resultTask = ProcessGet<List<StudyCustomModel>>($"{_endpoint}/{StudyCustomEndpointSuffix}", configurationId);

                _cache.Set(cacheKey, resultTask, BuildCacheOptions());
            }

            return resultTask;
        }

        private async Task<StudyCustomModel> GetSingleStudyCustomValue(
            string key, 
            Guid? configurationId = null)
        {
            var cacheKey = GetCacheKey(key, configurationId, null);

            var result = GetEntityFromCache<StudyCustomModel>(cacheKey);

            if (result == null)
            {
                var results = await GetAllStudyCustoms(configurationId);
                result = results.FirstOrDefault(sc => sc.Key == key);

                // We got the data we need now store it in the cache.
                if (result != null)
                {
                    _cache.Set(GetCacheKey(result.Key, configurationId, null), result, BuildCacheOptions());
                }
            }

            return result;
        }

        public async Task LoadIntoCache(Guid configurationId)
        {
            var cacheKey = GetCacheKey(ConfigEndpoint, configurationId, null);
            
            if (GetEntityFromCache<string>(cacheKey) == null)
            {
                var results = await GetAllStudyCustoms(configurationId);

                foreach (var result in results)
                {
                    _cache.Set(GetCacheKey(result.Key, configurationId, null), result, BuildCacheOptions());
                }

                _cache.Set(cacheKey, DateTime.Now.ToString(), BuildCacheOptions());
            }
        }
    }
}
