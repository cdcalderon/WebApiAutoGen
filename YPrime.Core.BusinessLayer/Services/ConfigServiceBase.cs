using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public abstract class ConfigServiceBase<T, TID> : IConfigServiceBase<T, TID>
        where T : class, IConfigModel
    {
        private const string ConfigVersionIdHeaderName = "VersionId";
        private const string ConfigHttpClientName = "configHttpClient";
        private const string environmentHeaderName = "environment";
        private readonly string environment;
        private readonly int slidingCacheExpirationSeconds;

        protected readonly string _endpoint;
        protected readonly IHttpClientFactory HttpClientFactory;
        protected readonly IMemoryCache _cache;
        protected readonly ILogger logger;
        protected readonly ISessionService SessionService;
        protected readonly IAuthSettings _authSettings;
        protected readonly IAuthService _authService;

        protected ConfigServiceBase(
            string endpoint,
            IHttpClientFactory httpClientFactory, 
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
        {
            _endpoint = endpoint;
            _cache = cache;
            HttpClientFactory = httpClientFactory;
            SessionService = sessionService;
            logger = ExceptionLogger.CreateLogger();
            environment = serviceSettings.StudyBuilderAppEnvironment;
            slidingCacheExpirationSeconds = serviceSettings.SlidingCacheExpirationSeconds;
            _authSettings = authSettings;
            _authService = authService;
    }

        public virtual async Task<List<T>> GetAll(Guid? configurationId = null)
        {
            /*
            if (_cache.TryGetValue(_endpoint, out List<T> data))
            {
                return data;
            }
            */
            var result = await ProcessGet<List<T>>(_endpoint, configurationId)
                .ConfigureAwait(false);

            /*
            if (result != null && result.Any())
            {
                _cache.Set(_endpoint, result, BuildCacheOptions());
            }
            */
            
            return result ?? new List<T>();
        }

        public virtual async Task<T> Get(TID id, Guid? configurationId = null)
        {
            var result = await ProcessGet<T>($"{_endpoint}/{id}", configurationId)
                .ConfigureAwait(false);         

            return result;
        }

        public virtual async Task<T> Get(Expression<Func<T, bool>> predicate, TID id, Guid? configurationId = null)
        {
            /*
            if (_cache.TryGetValue(_endpoint, out List<T> data))
            {
                var item = data.FirstOrDefault(predicate.Compile());
                
                if (item != null)
                {
                    return item;
                }
            }
            */

            var result = await Get(id, configurationId)
                .ConfigureAwait(false);

            /*
            if (result != null)
            {
                var cacheData = data ?? new List<T>
                {
                    result
                };

                _cache.Set(_endpoint, cacheData, BuildCacheOptions());
            } 
            */

            return result;
        }

        protected virtual MemoryCacheEntryOptions BuildCacheOptions()
        {
            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(slidingCacheExpirationSeconds),
                Priority = CacheItemPriority.Normal
            };

            return cacheExpirationOptions;
        }

        protected virtual async Task<R> ProcessGet<R>(string endpoint, Guid? configurationId = null)
            where R : class
        {
            var client = HttpClientFactory.CreateClient(ConfigHttpClientName);
            R result;
            
            try
            {
                result = await GetConfigObject<R>(
                    client,
                    endpoint,
                    configurationId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.ForContext<ConfigServiceBase<T, TID>>().Error(ex.Message);

                throw;
            }
            finally {
                Log.CloseAndFlush();
            }

            return result;
        }

        protected virtual async Task<R> GetConfigObject<R>(
            HttpClient client,
            string endpoint,
            Guid? configurationId,
            CancellationToken cancellationToken = default)
            where R : class
        {
            var httpMessage = new HttpRequestMessage(
                HttpMethod.Get,
                endpoint);
            var configId = configurationId ?? SessionService.UserConfigurationId;

            httpMessage
                .Headers
                .Add(ConfigVersionIdHeaderName, configId.ToString());


            var token = await _authService.GetTokenAsync(_authSettings.ClientId_M2M, _authSettings.ClientSecret_M2M, _authSettings.Audience_SB);

            httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            httpMessage.Headers.Add(environmentHeaderName, environment);

            var responseTask = client.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var result = default(R);

            using (var response = await responseTask.ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content != null ? await ReadContentFromJsonAsync<R>(response.Content) : result;
                }
                else
                {
                    throw new ApiFailureException(httpMessage.RequestUri.AbsoluteUri, response.StatusCode.ToString(), await response.Content?.ReadAsStringAsync());
                }
            }

            return result;
        }

        protected virtual async Task<R> ReadContentFromJsonAsync<R>(HttpContent content)
            where R : class
        {
            var json = await content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<R>(json);

            return result;
        }

        protected string GetCacheKey(string id, Guid? configurationId, Guid? languageId)
        {
            var configId = configurationId ?? SessionService.UserConfigurationId;

            return $"{_endpoint}-{id}-{configId}-{languageId}";
        }

        protected T GetFromCache(string cacheKey)
        {
            var result = GetEntityFromCache<T>(cacheKey);
            return result;
        }

        protected ET GetEntityFromCache<ET>(string cacheKey)
            where ET : class
        {
            ET result = null;

            if (_cache.TryGetValue(cacheKey, out ET data))
            {
                result = data;
            }

            return result;
        }
    }
}
