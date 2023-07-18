namespace YPrime.BusinessLayer.Reports.Models
{
    public class SubjectInformationAuditDto
    {
        public string Protocol { get; set; }

        public string SiteNumber { get; set; }

        public string SubjectNumber { get; set; }

        public string AuditSeq { get; set; }

        public string SubjectAttribute { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string ChangeReasonType { get; set; }

        public string ChangedBy { get; set; }

        public string ChangedDate { get; set; }

        public string CorrectionReason { get; set; }

        public string DCFNumber { get; set; }

        public string AuditSource { get; set; }

        public string AssetTag { get; set; }
    }
}
