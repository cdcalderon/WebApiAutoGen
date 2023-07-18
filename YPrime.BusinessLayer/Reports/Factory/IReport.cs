using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Reports.Factory
{
    public interface IReport
    {
        Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId);

        Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId);

        Dictionary<string, string> GetColumnHeadings();
    }
}