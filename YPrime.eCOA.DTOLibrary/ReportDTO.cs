using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ReportDto
    {
        public ReportDto()
        {
            ColumnNameDisplayMappings = new Dictionary<string, string>();
            ReportData = new List<ReportDataDto>();
            Charts = new List<ChartDataObject>();
        }

        public Guid Id { get; set; }
        public string GridName { get; set; }
        public string ReportName { get; set; }
        public bool AssociatedToUser { get; set; }
        public Dictionary<string, string> ColumnNameDisplayMappings { get; set; }
        public List<ReportDataDto> ReportData { get; set; }
        public List<ChartDataObject> Charts { get; set; }
        public string ReportStudyType { get; set; }
        public string ExportFileName { get; set; }
        public int TotalRecords { get; set; }
        public int TotalRecordsFiltered { get; set; }
        public bool IsServerSide { get; set; }
        public bool RestrictToPdfExport { get; set; }
    }
}