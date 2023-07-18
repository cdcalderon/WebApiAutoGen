using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models.Models
{
    [Table(nameof(SoftwareReleaseDeviceType))]
    public class SoftwareReleaseDeviceType : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("SoftwareRelease")]
        public Guid SoftwareReleaseId { get; set; }

        public Guid DeviceTypeId { get; set; }

        public virtual SoftwareRelease SoftwareRelease { get; set; }
    }
}
