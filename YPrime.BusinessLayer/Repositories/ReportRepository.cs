using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
namespace YPrime.BusinessLayer.Repositories
{
    public class ReportRepository : BaseRepository, IReportRepository
    {
        private readonly ITranslationService translationService;
        private readonly IStudySettingService studySettingService;
        private readonly IReportFactory reportFactory;

        public ReportRepository(
            IStudyDbContext db,
            ITranslationService translationService,
            IStudySettingService studySettingService,
            IReportFactory reportFactory)
            : base(db)
        {
            this.translationService = translationService;
            this.studySettingService = studySettingService;
            this.reportFactory = reportFactory;
        }

        public List<string> GetAllReports()
        {
            var reportNames = ReportType.GetAll<ReportType>()
                .Select(ar => ar.Name)
                .ToList();

            return reportNames;
        }

        public async Task<List<ReportDisplayDto>> GetAllReportsForDisplay(Guid userId)
        {
            var result = new List<ReportDisplayDto>();
            var userRoleIds = _db.StudyUserRoles
                .Where(u => u.StudyUserId == userId)
                .Select(u => u.StudyRoleId)
                .ToList();

            var userReportIds = _db.ReportStudyRoles
                .Where(r => userRoleIds.Contains(r.StudyRoleId))
                .Select(r => r.ReportId)
                .ToList();

            foreach (var reportMetadata in ReportType.Where<ReportType>(r => userReportIds.Contains(r.Id)))
            {
                result.Add(await ConstructReportDisplayDto(reportMetadata));
            }

            return result.OrderBy(r => r.ReportTitle)
                .ToList();
        }

        public async Task<List<ReportDisplayDto>> GetAllReportsForDisplay(StudyRoleModel studyRole)
        {
            var result = new List<ReportDisplayDto>();

            foreach (var reportMetadata in ReportType.GetAll<ReportType>())
            {
                result.Add(await ConstructReportDisplayDto(reportMetadata));
            }

            return result.OrderBy(r => r.ReportTitle).ToList();
        }

        public async Task<ChartDataObject> GetReportChartData(
            Guid reportId,
            Guid userId,
            Dictionary<string, object> parameters)
        {
            IReport report = null;

            var reportType = ReportType.FirstOrDefault<ReportType>(r => r.Id == reportId);

            if (reportType == null)
            {
                return null;
            }

            report = reportFactory.CreateReport(reportType);

            return await report.GetReportChartData(parameters, userId);
        }

        public async Task<ReportDto> GetReportDataObject(
            Guid id,
            Guid userId,
            Dictionary<string, object> parameters)
        {
            IReport report = null;

            var reportType = ReportType.FirstOrDefault<ReportType>(r => r.Id == id);
            var reportName = await translationService.GetByKey(reportType.Name);
            report = reportFactory.CreateReport(reportType);

            var sponsor = await studySettingService.GetStringValue("StudySponsor");
            var protocol = await studySettingService.GetStringValue("Protocol");

            var result = new ReportDto
            {
                ReportData = await report.GetGridData(parameters, userId)
            };

            result.Id = id;
            result.ReportName = reportName;
            result.ExportFileName = $"{sponsor}_{protocol}_{reportName}";
            result.GridName = "ReportGrid";
            result.ColumnNameDisplayMappings = report.GetColumnHeadings();
            result.RestrictToPdfExport = reportType.RestrictToPdfExport;

            return result;
        }

        private async Task<ReportDisplayDto> ConstructReportDisplayDto(ReportType reportType)
        {
            var reportName = await translationService.GetByKey(reportType.Name);
            var reportDisplayDto = new ReportDisplayDto
            {
                DisplayId = reportType.Id,
                ReportTitle = reportName
            };

            return reportDisplayDto;
        }
    }
}