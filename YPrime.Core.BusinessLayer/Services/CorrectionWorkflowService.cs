using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class CorrectionWorkflowService : ConfigServiceBase<CorrectionWorkflowSettingsModel, Guid>, ICorrectionWorkflowService
    {
        private const string ConfigEndpoint = "CorrectionType/Workflow";

        public CorrectionWorkflowService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public override Task<List<CorrectionWorkflowSettingsModel>> GetAll(Guid? configurationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
