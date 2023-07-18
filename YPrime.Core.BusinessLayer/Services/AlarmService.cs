using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class AlarmService : ConfigServiceBase<AlarmModel, Guid>, IAlarmService
    {
        private const string ConfigEndpoint = "Alarms";
        private Guid _defaultLanguage = Config.Defaults.Languages.English.Id;

        public AlarmService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public async Task<AlarmModel> GetTranslatedAlarmModel(
            Guid alarmId,
            Guid? languageId = null,
            Guid? configurationId = null)
        {
            languageId = languageId ?? _defaultLanguage;

            var endpoint = $"{ConfigEndpoint}/{alarmId}/{languageId}";

            var result = await ProcessGet<AlarmModel>(endpoint, configurationId);

            return result;
        }
    }
}
