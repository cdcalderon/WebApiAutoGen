using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.BusinessRule.Interfaces;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("Patient")]
    public class Patient : DataSyncBase, IDataSyncObject, IPatient
    {
        public Patient()
        {
            Exports = new HashSet<Export>();
            DiaryEntries = new HashSet<DiaryEntry>();
            PatientVisits = new HashSet<PatientVisit>();
            Devices = new HashSet<Device>();
            PatientAttributes = new HashSet<PatientAttribute>();
            Corrections = new List<Correction>();
        }

        [StringLength(255)] [Required] public string PatientNumber { get; set; }

        public Guid LanguageId { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime? LastUpdate { get; set; }

        [StringLength(150)] public string Notes { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RowId { get; set; }

        [StringLength(100)] public string Pin { get; set; }

        public bool IsTempPin { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public Guid? SecurityQuestionId { get; set; }

        [StringLength(100)] public string SecurityAnswer { get; set; }

        public bool IsHandheldTrainingComplete { get; set; }

        public bool IsTabletTrainingComplete { get; set; }

        public virtual Site Site { get; set; }

        public virtual SecurityQuestion SecurityQuestion { get; set; }

        public virtual ICollection<PatientVisit> PatientVisits { get; set; }

        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; }

        public virtual ICollection<Export> Exports { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<PatientAttribute> PatientAttributes { get; set; }

        public virtual ICollection<CareGiver> CareGivers { get; set; }

        public virtual List<Correction> Corrections { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required] [ForeignKey("Site")] public Guid SiteId { get; set; }

        public int PatientStatusTypeId { get; set; } = -1;

        public DateTimeOffset EnrolledDate { get; set; }

        public Guid ConfigurationId { get; set; }

        [StringLength(30)]
        public string AuthUserId { get; set; }

        public Guid? ConsentParticipantId { get; set; }
    }
}