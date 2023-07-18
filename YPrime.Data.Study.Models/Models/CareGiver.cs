using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CareGiver : DataSyncBase, IDataSyncObject
    {
        public CareGiver()
        {
            DiaryEntries = new HashSet<DiaryEntry>();
        }

        public Guid PatientId { get; set; }

        public Guid? CareGiverTypeId { get; set; }

        public string Initials { get; set; }

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

        public virtual Patient Patient { get; set; }

        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
    }
}