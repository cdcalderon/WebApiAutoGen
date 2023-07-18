using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Reports.Reports
{
    public class TotalEnrollmentReport : IReport
    {
        private readonly IStudyDbContext _db;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;
        public TotalEnrollmentReport(IStudyDbContext db, ITranslationService translationService, IPatientStatusService patientStatusService)
        {
            _translationService = translationService;
            _patientStatusService = patientStatusService;
            _db = db;
        }

        public async Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            var statusCounts = await GetReportData(parameters, userId);

            var chartObject = new ChartDataObject
            {
                ChartType = SeriesChartType.Doughnut,
                ChartSeries = new List<ChartSeriesObject>(),
                IncludeLegend = false
            };

            foreach (var site in statusCounts.Keys)
            {
                chartObject.ChartSeries.Add(new ChartSeriesObject
                {
                    SeriesName = site,
                    SeriesDataPoints = new List<ChartDataPoint>()
                });
            }

            chartObject.XLabels = new Dictionary<double, string>();
            var XLabelKey = 1;
            var XValue = 1;

            foreach (var site in statusCounts.Keys)
            {
                foreach (var status in statusCounts[site].Keys)
                {
                    var series = chartObject.ChartSeries.Single(c => c.SeriesName == site);
                    series.SeriesStyle = new ChartSeriesStyle();

                    series.SeriesDataPoints.Add(
                        new ChartDataPoint
                        {
                            X = XValue,
                            XLabel = status,
                            Y = statusCounts[site][status]
                        });
                    XValue++;
                }
            }

            foreach (var site in statusCounts.Keys)
            {
                chartObject.XLabels.Add(XLabelKey++, site);
            }

            return chartObject;
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var data = new List<ReportDataDto>();
            var enrollmentTotals = await GetReportData(parameters, userId);

            enrollmentTotals.Keys.ToList().ForEach(k =>
            {
                var result = new ReportDataDto();
                int siteTotal = 0;

                enrollmentTotals[k].Keys.ToList().ForEach(status => { siteTotal += enrollmentTotals[k][status]; });

                result.Row.Add("Site_Name", k);
                result.Row.Add("TotalPatientCount", siteTotal);
                enrollmentTotals[k].Keys.ToList().ForEach(status =>
                {
                    result.Row.Add(status, enrollmentTotals[k][status]);
                });
                data.Add(result);
            });

            return data;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return ColumnHeaderMappings();
        }

        public Dictionary<string, string> ColumnHeaderMappings()
        {
            var patientCountLabel = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblTotalPatientCount).ConfigureAwait(false)).Result
                 ?? "Total Patient Count";

            var Rtn = new Dictionary<string, string>();

            Rtn.Add("Site_Name", "Site Name");
            Rtn.Add("TotalPatientCount", patientCountLabel);

            var statuses = Task.Run(async () => await PatientStatusesToDictionary().ConfigureAwait(false)).Result;
            statuses.Keys.ToList().ForEach(k => { Rtn.Add(k, k); });

            return Rtn;
        }

        private async Task<Dictionary<string, Dictionary<string, int>>> GetReportData(Dictionary<string, object> parameters, Guid userId)
        {
            Guid? selectedSiteId = parameters != null && parameters.Any(k => k.Key == "SelectedSiteId")
                ? (Guid?)parameters["SelectedSiteId"]
                : null;
            List<SiteDto> allowedSites =
                parameters != null && parameters.Any(k => k.Key == "Sites") ? (List<SiteDto>)parameters["Sites"] : null;
            List<Guid> allowedSitesList = allowedSites?.Select(s => s.Id).ToList() ?? GetUserAllowedSites(userId);
            var siteName = selectedSiteId == null ? "All Sites" : allowedSites.Single(s => s.Id == selectedSiteId).Name;

            var patientStatuses = await _patientStatusService.GetAll();
            Dictionary<string, int> patientStatusDict = PatientStatusesToDictionary(patientStatuses);

            var removedStatusIds = patientStatuses
                .Where(pst => pst.IsRemoved)
                .Select(pst => pst.Id);

            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patients = await patientFilter.Execute(_db.Patients)
                .Where(p =>
                    (p.SiteId == selectedSiteId || selectedSiteId == null && allowedSitesList.Contains(p.SiteId))
                    && !removedStatusIds.Contains(p.PatientStatusTypeId))
                .ToListAsync();

            var statuses = patients
                .GroupBy(p => p.GetPatientStatusType(patientStatuses)?.Name)
                .Select(group => new {Name = group.Key, Count = group.Count()})
                .Where(k => k.Name != null)
                .ToDictionary(k => k.Name, v => v.Count);

            statuses.Keys.ToList().ForEach(k => { patientStatusDict[k] = statuses[k]; });

            var result = new Dictionary<string, Dictionary<string, int>>();
            result.Add(siteName, patientStatusDict);

            return result;
        }

        private List<Guid> GetUserAllowedSites(Guid userId)
        {
            var result = _db.StudyUserRoles
                .Where(sur => sur.StudyUserId == userId)
                .GroupBy(group => group.Site.Id)
                .Select(k => k.Key)
                .ToList();

            return result;
        }

        private Dictionary<string, int> PatientStatusesToDictionary(List<PatientStatusModel> patientStatusTypes)
        {
            return patientStatusTypes
                .Where(x => !x.IsRemoved)
                .OrderBy(p => p.Name)
                .ToDictionary(k => k.Name, v => 0);
        }

        private async Task<Dictionary<string, int>> PatientStatusesToDictionary()
        {
            var patientStatusTypes = await _patientStatusService.GetAll();
            return PatientStatusesToDictionary(patientStatusTypes);
        }
    }
}