using System;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class PatientVisitAuditDto
    {
        public string Protocol { get; set; }

        public string SiteNumber { get; set; }

        public string SubjectNumber { get; set; }

        public Guid VisitID { get; set; }

        public string VisitName { get; set; }

        public string SubjectVisitAttribute { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string ChangeReasonType { get; set; }

        public string ChangedBy { get; set; }

        public string ChangedDate { get; set; }

        public string CorrectionReason { get; set; }

        public string DCFNumber { get; set; }

        public string AuditSource { get; set; }

        public string AssetTag { get; set; }

        public int SortOrder { get; set; }
    }
}
