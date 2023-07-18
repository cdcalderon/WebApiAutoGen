using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class ApproverGroupService : ConfigServiceBase<ApproverGroupModel, Guid>, IApproverGroupService
    {
        private const string ConfigEndpoint = "ApproverGroup";

        public ApproverGroupService(
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
