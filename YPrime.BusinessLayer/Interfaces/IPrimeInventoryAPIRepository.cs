using System;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary.PrimeInventoryDtos;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPrimeInventoryAPIRepository
    {
        Task<PrimeInventoryDeviceDto> AddBringYourOwnDeviceAssetTag(Guid StudyId, string Environment, int NumberOfUses);

        void SetBaseUrl(string baseUrl);

    }
}