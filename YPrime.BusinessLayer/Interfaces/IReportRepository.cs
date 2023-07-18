using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IReportRepository
    {
        Task<ChartDataObject> GetReportChartData(Guid reportId, Guid userId, Dictionary<string, object> parameters);

        Task<ReportDto> GetReportDataObject(Guid id, Guid userId, Dictionary<string, object> parameters);

        List<string> GetAllReports();

        Task<List<ReportDisplayDto>> GetAllReportsForDisplay(Guid UserId);

        Task<List<ReportDisplayDto>> GetAllReportsForDisplay(StudyRoleModel studyRole);
    }
}