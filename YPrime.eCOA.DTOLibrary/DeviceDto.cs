using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class DeviceDto : DtoBase
    {
        public Guid Id { get; set; }
        public Guid? SiteId { get; set; }
        public string SiteNumber { get; set; }
        public string SiteName { get; set; }
        public string SoftwareReleaseName { get; set; }
        public Guid SoftwareReleaseId { get; set; }
        public Guid? LastReportedSoftwareVersionId { get; set; }
        public Guid? AssignedSoftwareVersionId { get; set; }
        public Guid DeviceTypeId { get; set; }
        public string MACAddress { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public string DeviceTypeName { get; set; }
        public string AssetTag { get; set; }
        public string AssignedSoftwareVersionNumber { get; set; }
        public string AssignedConfigurationVersionNumber { get; set; }
        public string LastReportedSoftwareVersionNumber { get; set; }
        public string LastReportedConfigurationVersionNumber { get; set; }
        public DateTimeOffset? LastDataSyncDate { get; set; }

        public DiaryEntryDto LastDiaryEntry { get; set; }
        public List<SyncLogDto> SyncLogs { get; set; }
        public string DeviceDrilldownButtonHTML { get; set; }
        public Guid LastReportedConfigurationId { get; set; }
    }
}