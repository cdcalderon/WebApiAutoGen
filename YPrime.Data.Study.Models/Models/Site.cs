using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.BusinessRule.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("Site")]
    public class Site : ModelBase, ISite
    {
        public Site()
        {
            Exports = new HashSet<Export>();
            Patient = new HashSet<Patient>();
            StudyUserRoles = new HashSet<StudyUserRole>();
            SoftwareReleases = new HashSet<SoftwareRelease>();
        }

        [Required]
        [StringLength(450)]
        [Index("IX_Name", IsUnique = true)]
        public string Name { get; set; }

        [Required]
        [StringLength(450)]
        [Index("IX_SiteNumber", IsUnique = true)]
        public string SiteNumber { get; set; }

        [Required] public bool IsActive { get; set; }

        [Required] public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        [Required] public string City { get; set; }

        [Required] public string State { get; set; }

        [Required] public string Zip { get; set; }

        [Required] public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        [Required] public string PrimaryContact { get; set; }

        [Required] public string TimeZone { get; set; }

        public DateTime? LastUpdate { get; set; }

        [StringLength(255)] public string Notes { get; set; }

        public int? PatientDOBFormatId { get; set; }

        [StringLength(50)] public string Investigator { get; set; }

        public virtual ICollection<Export> Exports { get; set; }

        public virtual ICollection<Patient> Patient { get; set; }

        public virtual ICollection<StudyUserRole> StudyUserRoles { get; set; }

        public virtual ICollection<SiteActiveHistory> SiteActiveHistory { get; set; } =
            new HashSet<SiteActiveHistory>();

        public virtual ICollection<SiteLanguage> SiteLanguages { get; set; }

        public virtual ICollection<SoftwareRelease> SoftwareReleases { get; set; }
        public DateTime? WebBackupExpireDate { get; set; }

        public Guid CountryId { get; set; }
        
        public Guid ConfigurationId { get; set; }

        public Guid SiteDisplayLanguageId { get; set; }
    }
}