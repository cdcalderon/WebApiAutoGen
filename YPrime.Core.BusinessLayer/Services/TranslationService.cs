using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YPrime.Config.Defaults;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class TranslationService : ConfigServiceBase<TranslationModel, string>, ITranslationService
    {
        private const string ConfigEndpoint = "Translation";
        private const string ResourceKeyParamName = "resourceKey";
        private const string LanguageIdParamName = nameof(TranslationModel.LanguageId);
        private const string SourceParamName = nameof(TranslationModel.Source);

        private readonly Regex _translationRegex = new Regex("{{Translation:(.*?)}}");

        public TranslationService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public override Task<List<TranslationModel>> GetAll(Guid? configurationId = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<TranslationModel> Get(string id, Guid? configurationId = null)
        {
            var models = await GetModels(id, configurationId: configurationId)
                .ConfigureAwait(false);

            var result = models.FirstOrDefault();

            return result;
        }

        public async Task<string> GetByKey(string id, Guid? configurationId = null, Guid? languageId = null)
        {
            var configId = configurationId ?? SessionService.UserConfigurationId;
            var langId = languageId ?? Languages.English.Id;
            var cacheKey = GetCacheKey(id, configId, langId);
            var translationEntity = GetFromCache(cacheKey);

            if (translationEntity == null)
            {
                var models = await GetModels(id, configurationId: configId, languageId: langId).ConfigureAwait(false);
                translationEntity = models.FirstOrDefault();
                
                _cache.Set(cacheKey, translationEntity, BuildCacheOptions());
            }

            var translatedValue = translationEntity?.LocalText ?? $"Translation Missing: {id}";
            translatedValue = await ReplaceNestedTranslation(translatedValue).ConfigureAwait(false);

            return translatedValue;
        }

        public async Task LoadIntoCache(string source, Guid configurationId, Guid languageId)
        {
            var sourceLoadedCacheKey = $"{nameof(LoadIntoCache)}-{GetCacheKey(source, configurationId, languageId)}";

            if (GetEntityFromCache<string>(sourceLoadedCacheKey) == null)
            {
                var results = await GetModels(source: source, configurationId: configurationId, languageId: languageId);
                foreach (var result in results)
                {
                    _cache.Set(GetCacheKey(result.Id, configurationId, languageId), result, BuildCacheOptions());
                }
                _cache.Set(sourceLoadedCacheKey, DateTime.Now.ToString(), BuildCacheOptions());
            }
        }

        public Task<List<TranslationModel>> GetByLanguage(Guid languageId, Guid? configurationId = null)
        {
            var modelTask = GetModels(languageId: languageId, configurationId: configurationId);

            return modelTask;
        }

        private async Task<string> ReplaceNestedTranslation(string result)
        {
            if (result != null)
            {
                var matches = _translationRegex.Matches(result);
                foreach (var match in matches)
                {
                    var translationVariable = match.ToString().Replace("{", "").Replace("}", "");
                    var values = translationVariable.Split(':');
                    var translation = await GetByKey(values[1]).ConfigureAwait(false);
                    result = result.Replace(match.ToString(), translation);
                }
            }

            return result;
        }

        private async Task<List<TranslationModel>> GetModels(
            string resourceKey = null,
            string source = null,
            Guid? languageId = null,
            Guid? configurationId = null)
        {
            var url = BuildUrl(
                resourceKey,
                source,
                languageId);

            var results = await ProcessGet<List<TranslationModel>>(url, configurationId).ConfigureAwait(false);

            return results;
        }

        private string BuildUrl(
            string resourceKey = null,
            string source = null,
            Guid? languageId = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(resourceKey))
            {
                queryParams.Add(ResourceKeyParamName, resourceKey);
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                queryParams.Add(SourceParamName, source);
            }

            if (languageId.HasValue)
            {
                queryParams.Add(LanguageIdParamName, languageId.Value.ToString());
            }

            var url = AddQueryString(_endpoint, queryParams);

            return url;
        }

        private string AddQueryString(
            string uri,
            Dictionary<string, string> queryString)
        {
            var queryIndex = uri.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uri);
            foreach (var parameter in queryString)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            return sb.ToString();
        }
    }
}