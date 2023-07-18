using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Attributes;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    public class Device : DataSyncBase, IDataSyncObject
    {
        public Device()
        {
            DiaryEntries = new HashSet<DiaryEntry>();
            DeviceDatas = new HashSet<DeviceData>();
            SyncLogs = new HashSet<SyncLog>();
        }

        //[MaxLength(10)]
        public Guid? SiteId { get; set; }

        [SyncDeviceColumnAlways]
        public Guid? PatientId { get; set; }

        [ForeignKey("LastReportedSoftwareVersion")]
        public Guid LastReportedSoftwareVersionId { get; set; }

        public Guid LastReportedConfigurationId { get; set; }

        [NonSyncColumn]
        [ForeignKey("SoftwareRelease")]
        public Guid SoftwareReleaseId { get; set; }

        public Guid DeviceTypeId { get; set; }

        [MaxLength(12)] public string MACAddress { get; set; }

        [MaxLength(16)] public string SerialNumber { get; set; }

        [MaxLength(16)] public string IMEI1 { get; set; }

        [MaxLength(16)] public string IMEI2 { get; set; }

        public string AssetTag { get; set; }

        public virtual SoftwareVersion LastReportedSoftwareVersion { get; set; }

        public virtual SoftwareRelease SoftwareRelease { get; set; }

        public virtual Site Site { get; set; }
        public virtual ICollection<DeviceData> DeviceDatas { get; set; }

        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; }

        public virtual ICollection<SyncLog> SyncLogs { get; set; }

        public bool SendDatabase { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public bool DoAdditionalTableSync { get; set; }

        public Guid Id { get; set; }
    }
}