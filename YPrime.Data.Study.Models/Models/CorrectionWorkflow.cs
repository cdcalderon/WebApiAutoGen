using System;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionWorkflow : ModelBase
    {
        public Guid? ApproverGroupId { get; set; }
        public Guid CorrectionId { get; set; }
        public virtual Correction Correction { get; set; }
        public int WorkflowOrder { get; set; }

        public Guid? CorrectionActionId { get; set; }
        public virtual CorrectionAction CorrectionAction { get; set; }

        public Guid? StudyUserId { get; set; }
        public virtual StudyUser StudyUser { get; set; }

        public DateTimeOffset WorkflowChangedDate { get; set; }

        [NotMapped]
        public string ApproverGroupName { get; set; }

        [NotMapped]
        public string PatientNumber { get; set; }
    }
}