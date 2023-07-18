using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("SoftwareReleaseCountry")]
    public class SoftwareReleaseCountry : AuditModel
    {
        [Key]
        [ForeignKey("SoftwareRelease")]
        public Guid SoftwareReleaseId { get; set; }

        public Guid CountryId { get; set; }

        public virtual SoftwareRelease SoftwareRelease { get; set; }
    }
}
