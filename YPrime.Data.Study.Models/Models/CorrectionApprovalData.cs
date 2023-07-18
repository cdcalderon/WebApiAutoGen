using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionApprovalData : ModelBase
    {
        public CorrectionApprovalData()
        {
            AllowDelete = false;
            RemoveItem = false;
            CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>();
        }

        [ForeignKey("Correction")] public Guid CorrectionId { get; set; }

        public virtual Correction Correction { get; set; }

        public Guid RowId { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string TranslationKey { get; set; }

        [NotMapped] public string Description { get; set; }

        [NotMapped] public bool AllowDelete { get; set; }

        [NotMapped]
        public Guid? QuestionId { get; set; }

        public string OldDataPoint { get; set; }

        public string NewDataPoint { get; set; }

        public string OldDisplayValue { get; set; }

        public string NewDisplayValue { get; set; }

        [NotMapped] public bool RemoveItem { get; set; }

        public virtual List<CorrectionApprovalDataAdditional> CorrectionApprovalDataAdditionals { get; set; }
    }
}