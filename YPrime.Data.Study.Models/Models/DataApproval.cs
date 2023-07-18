using System.Collections.Generic;

namespace YPrime.Data.Study.Models
{
    public class DataApproval : ModelBase
    {
        public string Name { get; set; }

        public virtual IEnumerable<CorrectionApprovalData> ApprovalData { get; set; }

        public string Comments { get; set; }
    }
}