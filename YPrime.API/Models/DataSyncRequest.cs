using System;
using System.Collections.Generic;

namespace YPrime.API.Models
{
    public class DataSyncRequest
    {
        public string AssetTag { get; set; }

        public Guid DeviceId { get; set; }

        public Guid DeviceTypeId { get; set; }

        public string SoftwareVersion { get; set; }

        public string ConfigurationVersion { get; set; }

        public string EncryptionKey { get; set; }

        public Guid? PatientId { get; set; }

        public Guid? SiteId { get; set; }

        public List<dynamic> ClientEntries { get; set; } = new List<dynamic>();

        public List<dynamic> AuditEntries { get; set; } = new List<dynamic>();
    }
}