using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Forms.DataVisualization.Charting;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Reports.Factory;

namespace YPrime.Reports.Reports
{
    public class AverageDiaryDurationReport : IReport
    {
        private const string QuestionnaireNameColumnName = "Questionnaire_Name";
        private const string AverageDurationColumnName = "Average_Duration";

        private readonly Dictionary<string, string> _columnHeadings = new Dictionary<string, string>
        {
            { QuestionnaireNameColumnName, "Questionnaire Name" },
            { AverageDurationColumnName, "Average Duration (Minutes)" }
        };

    private readonly IDiaryEntryRepository _diaryEntryRepository;

        public AverageDiaryDurationReport(IDiaryEntryRepository diaryEntryRepository)
        {
            _diaryEntryRepository = diaryEntryRepository;
        }

        public Dictionary<string, string> GetColumnHeadings() => _columnHeadings;

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var reportData = await GetReportData();

            var gridData = new List<ReportDataDto>();

            foreach (var s in reportData)
            {
                var result = new ReportDataDto();

                result.Row.Add(QuestionnaireNameColumnName, s.Key);
                result.Row.Add(AverageDurationColumnName, s.Value);
                gridData.Add(result);
            }

            return gridData;
        }

        public async Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            var chartObject = new ChartDataObject
            {
                ChartType = SeriesChartType.Bar,
                ChartSeries = new List<ChartSeriesObject>(),
                IncludeLegend = false
            };

            var x = 1;

            var data = await GetReportData();

            // One Series

            var series = new ChartSeriesObject
            {
                SeriesName = "Questionnaires",
                SeriesDataPoints = new List<ChartDataPoint>(),
                SeriesStyle = new ChartSeriesStyle()
            };

            foreach (var dataPoint in data)
            {
                series.SeriesDataPoints.Add(new ChartDataPoint
                {
                    X = x,
                    Y = dataPoint.Value
                });

                x++;
            }

            chartObject.ChartSeries.Add(series);

            x = 1;

            foreach (var dataPoint in data)
            {
                chartObject.XLabels.Add(x, dataPoint.Key);
                x++;
            }

            return chartObject;
        }

        private async Task<Dictionary<string, float>> GetReportData()
        {
            var diaryEntries = await _diaryEntryRepository.GetDiaryEntriesInflated(null, false, false, null);

            var result = diaryEntries
            .GroupBy(de => de.QuestionnaireDisplayName, d => d)
            .Select(g => new { Name = g.Key, Duration = g.Average(v => (v.CompletedTime- v.StartedTime).TotalSeconds / 60) })
            .ToDictionary(k => k.Name, v => v.Duration > 1 ? (float)Math.Round((decimal)v.Duration) : (float)Math.Round((decimal)v.Duration, 2));

            return result;
        }
    }
}