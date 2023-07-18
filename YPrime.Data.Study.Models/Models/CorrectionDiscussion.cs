using System;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionDiscussion : ModelBase
    {
        public string Discussion { get; set; }
        public DateTimeOffset DiscussionDate { get; set; }
        public Guid CorrectionId { get; set; }

        public Guid StudyUserId { get; set; }

        public StudyUser StudyUser { get; set; }

        public virtual Correction Correction { get; set; }

        public Guid CorrectionActionId { get; set; }
        public CorrectionAction CorrectionAction { get; set; }
    }
}