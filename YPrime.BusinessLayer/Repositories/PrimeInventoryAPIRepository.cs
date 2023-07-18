using System;
using YPrime.eCOA.DTOLibrary.PrimeInventoryDtos;
using YPrime.BusinessLayer.Interfaces;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Repositories
{
    public class PrimeInventoryAPIRepository : IPrimeInventoryAPIRepository
    {
        private string baseUrl { get; set; }
        private readonly IApiRepository _apiRepository;
        public PrimeInventoryAPIRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public async Task<PrimeInventoryDeviceDto> AddBringYourOwnDeviceAssetTag(Guid StudyId, string Environment, int NumberOfUses)
        {
            Uri baseUri = new Uri(this.baseUrl);
            Uri uri = new Uri(baseUri, $"api/AddBringYourOwnDeviceAssetTag/{StudyId.ToString()}/{Environment}/{NumberOfUses.ToString()}");            
            var result = await _apiRepository.Post<PrimeInventoryDeviceDto>(uri.AbsoluteUri, "", null);
            return result;
        }

        public void SetBaseUrl(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
    }
}
