using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionHistory : ModelBase
    {
        [ForeignKey("Correction")] public Guid CorrectionId { get; set; }

        public virtual Correction Correction { get; set; }

        public string Discussion { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public Guid? StudyUserId { get; set; }
        public virtual StudyUser StudyUser { get; set; }

        public virtual CorrectionAction CorrectionAction { get; set; }
        public Guid CorrectionActionId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateTimeStamp { get; set; }
    }
}