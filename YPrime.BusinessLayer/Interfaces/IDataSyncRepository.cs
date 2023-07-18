using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models.Models.DataSync;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IDataSyncRepository
    {
        Task<DataSyncResponse> SyncClientData(
            Guid deviceId,
            Guid deviceTypeId,
            string softwareVersion, 
            string configVersion, 
            List<dynamic> clientEntries,
            List<dynamic> autitEntries, 
            Guid? devicePatientId);

        Task<DataSyncResponse> SyncInitialData(
            Guid deviceId, 
            Guid deviceTypeId,
            Guid? siteId,
            Guid? patientId,
            string softwareVersion, 
            string configVersion, 
            List<dynamic> clientEntries);

        CheckForUpdateResponse CheckForUpdates(Guid deviceId, string softwareVersion, string configVersion);

        void LogDeviceSyncData(string ConfigVersion, 
            Guid DeviceId, 
            string SoftwareVersion, 
            string SyncAction, 
            bool syncSuccess, 
            string syncLogMessage,
            dynamic ClientEntries = null);

        void AddDeviceData(Guid deviceId, string fob);

        void CreateDeviceIfNotExists(
            Guid deviceId, 
            Guid deviceTypeId,
            Guid siteId, 
            string softwareVersion, 
            string assetTag);

        Task<List<DataSyncResponseTable>> GetTables(
            ISyncSQLBuilder sqlBuilder,
            List<dynamic> clientEntries, 
            Guid deviceConfigId);
    }
}