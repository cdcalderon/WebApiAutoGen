using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class PatientDto : DtoBase
    {
        public PatientDto()
        {
            PatientAttributes = new List<PatientAttributeDto>();
            DiaryEntries = new List<DiaryEntryDto>();
            PatientVisits = new List<PatientVisitDto>();
            CareGivers = new List<CareGiverDto>();
        }

        public Guid Id { get; set; }

        public int RowId { get; set; }

        [DisplayName("Site")] public Guid SiteId { get; set; }

        public string SiteNumber { get; set; }
        public bool IsSiteActive { get; set; }

        [Required]
        [DisplayName("Subject Number")]
        public string PatientNumber { get; set; }

        public int PatientStatusTypeId { get; set; }

        [DisplayName("Subject Status")] public string PatientStatus { get; set; }

        public DateTimeOffset? EnrolledDate { get; set; }
        public bool IsActive { get; set; }
        public bool Compliance { get; set; }
        public DateTime? PatientDOB { get; set; }
        public string PatientGender { get; set; }
        public bool IsHandheldTrainingComplete { get; set; }
        public bool IsTabletTrainingComplete { get; set; }

        public string Pin { get; set; }

        public bool IsTempPin { get; set; }

        public DateTime? LastDiaryEntryDate { get; set; }

        public DateTimeOffset? LastDeviceSyncDate { get; set; }

        public string LastVisit { get; set; }

        public DateTimeOffset? LastVisitDate { get; set; }

        public SiteDto Site { get; set; }

        public int SyncVersion { get; set; }

        public List<CareGiverDto> CareGivers { get; set; }

        public List<PatientVisitDto> PatientVisits { get; set; }

        public List<QuestionnaireModel> QuestionnairesTaken { get; set; }

        public List<SubjectInformationModel> SubjectInformations { get; set; } = new List<SubjectInformationModel>();

        public List<PatientAttributeDto> PatientAttributes { get; set; }

        [SkipPropertyCopy] public List<Correction> Corrections { get; set; }

        public PatientStatusModel PatientStatusType { get; set; }

        public ICollection<DiaryEntryDto> DiaryEntries { get; set; }

        public virtual SecurityQuestionDto SecurityQuestion { get; set; }
        public Guid? SecurityQuestionId { get; set; }

        public string SecurityAnswer { get; set; }

        public Guid? LanguageId { get; set; }

        [JsonIgnore]
        [SkipPropertyCopy] public bool? SubjectUsePersonalDevice { get; set; }

        [SkipPropertyCopy]
        [JsonIgnore]
        public Guid? ConfigurationId { get; set; }

        public string AuthUserId { get; set; }

        public Guid? ConsentParticipantId { get; set; }
    }
}