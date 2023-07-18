using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class ConfigurationVersionService : ConfigServiceBase<ConfigurationVersion, Guid>, IConfigurationVersionService
    {
        private const string ConfigEndpoint = "Versions/true";
        
        private readonly IServiceSettings _serviceSettings;

        public ConfigurationVersionService(
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

        public async override Task<ConfigurationVersion> Get(Guid id, Guid? configurationId = null)
        {
            var configs = await GetAll();
            var config = configs.FirstOrDefault(x => x.Id == id);
            return config;
        }

        public async Task<Guid> GetLatest()
        {
            var configs = await GetAll();
            configs.Sort((x ,y) => y.DateCreated.CompareTo(x.DateCreated));

            ConfigurationVersion latestVersion;

            if (_serviceSettings.IsProductionEnvironment())
            {
                latestVersion = configs.FirstOrDefault(x => x.ApprovedForProd);
                
                if (latestVersion == null)
                {
                    throw new NoProductionConfigurationException();
                }
            }
            else
            {
                latestVersion = configs.FirstOrDefault();
            }

            var result = latestVersion?.Id ?? Config.Defaults.ConfigurationVersions.InitialVersion.Id;

            return result;
        }
    }
}
