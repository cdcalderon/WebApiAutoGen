using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models
{
    class SubjectInformationAuditReportGridMap
    {
        public string SiteNumber { get; set; }
        public string SubjectNumber { get; set; }
        public string SubjectAttribute { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangeReason { get; set; }
        public string ChangeBy { get; set; }
        public string ChangeDate { get; set; }
    }
}
