using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("SyncLog")]
    public class SyncLog : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required] public Guid DeviceId { get; set; }

        public Guid? SoftwareVersionId { get; set; }

        public string ConfigurationVersionNumber { get; set; }

        public virtual SoftwareVersion SoftwareVersion { get; set; }

        [Required] public string SyncAction { get; set; }

        [Required] public DateTimeOffset SyncDate { get; set; }

        public string SyncData { get; set; }

        public bool SyncSuccess { get; set; }
        public string SyncLogMessage { get; set; }

        public virtual Device Device { get; set; }
    }
}