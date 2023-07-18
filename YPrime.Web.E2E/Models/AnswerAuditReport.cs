using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models
{
    class AnswerAuditReportGridMap
    {
        public string Protocol { get; set; }
        public string SiteNumber { get; set; }
        public string SubjectNumber { get; set; }
        public string DiaryDate { get; set; }
        public string Questionnaire { get; set; }
        public string Question { get; set; }
        public string NewValue { get; set; }
        public string ChangeReason { get; set; }
        public string ChangeBy { get; set; }
        public string ChangeDate { get; set; }
        public string CorrectionReason { get; set; }
    }
}
