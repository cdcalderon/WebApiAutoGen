using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class QuestionnaireService : ConfigServiceBase<QuestionnaireModel, Guid>, IQuestionnaireService
    {
        public const string ConfigEndpoint = "Questionnaire";
        public const string QuestionEndpoint = "questions";
        public const string PagesEndpoint = "questionnairePages";
        public const string DeepLoadedEndpoint = "deepLoadedQuestionnaire";
        public const string AllDeepLoadedEndpoint = "allDeepLoadedQuestionnaire";

        private Guid _defaultLanguage = Config.Defaults.Languages.English.Id;
        private readonly IServiceSettings _serviceSettings;

        public QuestionnaireService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        {
            _serviceSettings = serviceSettings;
        }

        public async Task<List<QuestionModel>> GetQuestions(
            Guid questionnaireId, 
            Guid? configurationId = null)
        {
            var endpoint = $"{ConfigEndpoint}/{questionnaireId}/{QuestionEndpoint}";

            var results = await ProcessGet<List<QuestionModel>>(endpoint, configurationId);

            return results;
        }

        public async Task<List<QuestionnaireModel>> GetAllWithPages(
            Guid? configurationId = null)
        {
            var endpoint = $"{ConfigEndpoint}/{PagesEndpoint}/{_defaultLanguage}";

            var result = await ProcessGet<List<QuestionnaireModel>>(endpoint, configurationId);

            result = result ?? new List<QuestionnaireModel>();

            return result;
        }

        public async Task<QuestionnaireModel> GetInflatedQuestionnaire(
            Guid questionnaireId, 
            Guid? languageId = null,
            Guid? configurationId = null)
        {
            languageId = languageId ?? _defaultLanguage;

            var endpoint = $"{ConfigEndpoint}/{DeepLoadedEndpoint}/{questionnaireId}/{languageId}";

            var result = await ProcessGet<QuestionnaireModel>(endpoint, configurationId);

            return result;
        }

        public async Task<List<QuestionnaireModel>> GetAllInflatedQuestionnaires(
            Guid? languageId = null, 
            Guid? configurationId = null)
        {
            languageId = languageId ?? _defaultLanguage;

            var endpoint = $"{ConfigEndpoint}/{AllDeepLoadedEndpoint}/{languageId}";

            var result = await ProcessGet<List<QuestionnaireModel>>(endpoint, configurationId);

            return result;
        }
    }
}
