using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionApprovalDataAdditional : ModelBase
    {
        [ForeignKey("CorrectionApprovalData")] public Guid CorrectionApprovalDataId { get; set; }

        public virtual CorrectionApprovalData CorrectionApprovalData { get; set; }

        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }

        public bool IgnorePropertyUpdate { get; set; }
    }
}