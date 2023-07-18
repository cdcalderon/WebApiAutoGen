using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Models;

namespace YPrime.Data.Study.Models
{
    [Table("SoftwareRelease")]
    public class SoftwareRelease : AuditModel
    {
        public SoftwareRelease()
        {
            Sites = new HashSet<Site>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid SoftwareVersionId { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool StudyWide { get; set; }
        public bool Required { get; set; }
        public string ConfigurationVersion { get; set; }
        public string SRDVersion { get; set; }
        public virtual SoftwareVersion SoftwareVersion { get; set; }
        public virtual ICollection<Site> Sites { get; set; }
        public Guid ConfigurationId { get; set; }

        public virtual ICollection<SoftwareReleaseDeviceType> SoftwareReleaseDeviceTypes { get; set; }
    }
}