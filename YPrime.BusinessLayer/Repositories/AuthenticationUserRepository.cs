using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.Repositories
{
    public class AuthenticationUserRepository : IAuthenticationUserRepository
    {
        private readonly IApiRepository _apiRepository;
        private readonly IServiceSettings _serviceSettings;

        public AuthenticationUserRepository(
            IApiRepository apiRepository,
            IServiceSettings serviceSettings)
        {
            _apiRepository = apiRepository;
            _serviceSettings = serviceSettings;
        }

        public async Task<IEnumerable<dynamic>> GetUsersAsync(IEnumerable<Guid> userIds)
        {
            var address = "api/GetUsers";
            var result = await _apiRepository.Post<IEnumerable<dynamic>>(_serviceSettings.AuthUrl, address, userIds);
            return result;
        }
    }
}