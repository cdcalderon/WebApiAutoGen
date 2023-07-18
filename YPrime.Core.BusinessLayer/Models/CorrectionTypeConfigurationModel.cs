using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class CorrectionTypeConfigurationModel
    {
        public Guid ApproverGroupId { get; set; }

        public int WorkflowOrder { get; set; }
    }
}
