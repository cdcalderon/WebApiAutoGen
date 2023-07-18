using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("SoftwareVersion")]
    public class SoftwareVersion : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string VersionNumber { get; set; }

        public string PackagePath { get; set; }

        public Guid PlatformTypeId { get; set; }
    }
}