using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ApiDtos;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IDeviceRepository
    {
        int GetTotalDevicesAtSite(Guid? SiteId, List<SiteDto> AllowedSites);

        Task<IOrderedQueryable<DeviceDto>> GetAllDevices(List<Guid> AllowedSites);

        Device AddDevice(Guid deviceId, Guid? patientId, Guid siteId, Guid deviceTypeId, string assetTag);
        Task<DeviceDto> GetDevice(Guid Id, List<Guid> allowedSites);
        Device GetPatientBYODDevice(Guid patientId);

        IList<DeviceType> GetAllDeviceTypes();
        void AddUpdateDevice(AddUpdateDeviceDto AddUpdateDevice, ApiRequestResultViewModel result);
        int GetDevicesAtSiteCountByDeviceType(Guid? SiteId, List<SiteDto> AllowedSites, string DeviceTypeName);

        bool AuthenticateDevice(string Id, string Token);
        string GenerateToken(string Id);
        Device GetWebBackupTabletDevice(Guid siteId);
        Guid? GetDeviceIdForPatient(Guid patientId);
        Device GetWebBackupHandheldDevice(Guid patientId);
        List<dynamic> GetAdditionalTableData(Guid deviceId, List<dynamic> clientEntries);
        Task<Guid?> GetLastReportedConfigurationForDevice(Guid id);
        Task RemoveDevice(Device device);
    }
}