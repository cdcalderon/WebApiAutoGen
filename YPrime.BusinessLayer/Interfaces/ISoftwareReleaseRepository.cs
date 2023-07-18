using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISoftwareReleaseRepository
    {
        Task CreateSoftwareRelease(SoftwareReleaseDto softwareRelease);
        Task<Dictionary<string, string>> GetCountryDictionaryByDeviceType(List<Guid> deviceTypeIds);
        Task<List<DeviceType>> GetProvisionalDeviceTypesForStudy();
        Task<Dictionary<string, string>> GetSiteDictionaryByCountry(List<Guid> deviceTypeIds, List<Guid> countryIds);
        Task<string> GetSoftwareReleaseDeviceConfirmation(SoftwareReleaseDto softwareRelease);
        bool DeactivateSoftwareRelease(Guid id);
        IEnumerable<Device> GetEligibleDevices(List<Device> devices, Guid softwareReleaseId);
        List<Device> GetDevicesForSoftwareRelease(SoftwareReleaseDto softwareRelease);
        SoftwareRelease GetLatestSoftwareRelease(Guid? SiteId, Guid DeviceTypeId);
        Task<List<SoftwareReleaseDto>> GetSoftwareReleaseGridData();
        Task UpdateDeviceSoftwareRelease(IEnumerable<Device> devices, SoftwareReleaseDto softwareRelease);
        Task<Guid> FindLatestConfigurationVersion(List<Guid> siteIds, List<Guid> countryIds);
        Guid GetLatestGlobalConfigurationVersionId();
    }
}