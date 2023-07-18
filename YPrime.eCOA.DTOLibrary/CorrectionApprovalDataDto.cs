using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class CorrectionApprovalDataDto : DtoBase
    {
        public Guid Id { get; set; }
        public Guid CorrectionId { get; set; }
        public int DataCorrectionNumber { get; set; }
        public Guid RowId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string NewDataPoint { get; set; }
        public string NewDisplayValue { get; set; }

        public string OldDataPoint { get; set; }
        public string OldDisplayValue { get; set; }
    }
}