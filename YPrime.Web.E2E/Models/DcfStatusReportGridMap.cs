using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models
{
    class DcfStatusReportGridMap
    {
        public string DcfNumber { get; set; }
        public string Site { get; set; }
        public string Subject { get; set; }
        public string DcfType { get; set; }
        public string DcfStatus { get; set; }
        public string DcfOpenedDate { get; set; }
        public string DcfClosedDate { get; set; }
        public string PendingApprover { get; set; }
        public string CompletedApprovals { get; set; }
        public string DaysDcfOpen { get; set; }
    }
}
