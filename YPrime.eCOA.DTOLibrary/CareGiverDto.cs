using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class CareGiverDto : DtoBase
    {
        public CareGiverDto()
        {
            DiaryEntries = new HashSet<DiaryEntryDto>();
        }

        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid? CareGiverTypeId { get; set; }

        public string Pin { get; set; }

        public bool IsTempPin { get; set; }

        public bool IsHandheldTrainingComplete { get; set; }

        public bool IsTabletTrainingComplete { get; set; }

        public short? LoginAttempts { get; set; }

        public Guid? SecurityQuestionId { get; set; }

        public string SecurityAnswer { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        [SkipPropertyCopy] public virtual PatientDto Patient { get; set; }

        public CareGiverTypeModel CareGiverType { get; set; }

        public virtual ICollection<DiaryEntryDto> DiaryEntries { get; set; }
    }
}