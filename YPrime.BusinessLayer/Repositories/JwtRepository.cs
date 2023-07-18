using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Shared.Helpers.Data.Interfaces;

namespace YPrime.BusinessLayer.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private readonly IApiRepository _apiRepository;
        private string JwtRepositoryBaseUrl = ConfigurationManager.AppSettings["YPrimeJwtURL"];

        public JwtRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public T Decrypt<T>(string encryptedToken) where T : class
        {
            var address = "api/jwt/decrypt";
            var result = _apiRepository.Post(JwtRepositoryBaseUrl, $"{address}?token={encryptedToken}", null);
            var decryptedObject = JsonConvert.DeserializeObject<T>(result);
            return decryptedObject;
        }

        public string Encrypt(object modelObject)
        {
            var address = "api/jwt/encrypt";
            var result = _apiRepository.Post(JwtRepositoryBaseUrl, address, modelObject);
            return result.Replace(@"""", "");
        }
    }
}