using System.Collections.Generic;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class ComplianceReportDto
    {
        public string SiteId { get; set; }
        public string SubjectNumber { get; set; }
        public string Name { get; set; }
        public int? TotalCompleted { get; set; }
        public int? TotalExpected { get; set; }
        public Dictionary<string, decimal?> CompletionRates { get; set; } = new Dictionary<string, decimal?>();
        public int? VisitOrder { get; set; }
        public string SiteName { get; set; }
    }
}
