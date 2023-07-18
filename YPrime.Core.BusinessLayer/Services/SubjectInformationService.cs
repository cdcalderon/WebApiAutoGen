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
    public class SubjectInformationService : ConfigServiceBase<SubjectInformationModel, Guid>, ISubjectInformationService
    {
        private const string ConfigEndpoint = "SubjectInformation";

        public SubjectInformationService(
            IHttpClientFactory httpClientFactory, 
            IMemoryCache cache,
            ISessionService sessionService,
            IServiceSettings serviceSettings,
            IAuthSettings authSettings,
            IAuthService authService)
            : base(ConfigEndpoint, httpClientFactory, cache, sessionService, serviceSettings, authSettings, authService)
        { }

        public async Task<List<SubjectInformationModel>> GetForCountry(Guid countryId, Guid? configurationId = null)
        {
            var allModels = await GetAll(configurationId);

            var filteredModels = allModels
                .Where(m => m.Countries.Any(c => c.Id == countryId))
                .ToList();

            return filteredModels;
        }
    }
}
