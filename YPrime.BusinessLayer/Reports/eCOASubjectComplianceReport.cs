using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Reports.Reports
{
    public class eCOASubjectComplianceReport : IReport
    {
        private const string ComplianceIdKey = "QuestionnaireComplianceId";
        private const string SyncDateFormat = "dd-MMM-yyyy hh:mm";

        private readonly IStudyDbContext _db;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;

        public eCOASubjectComplianceReport(
            IStudyDbContext db,
            ITranslationService translationService,
            IPatientStatusService patientStatusService
            )
        {
            _db = db;
            _translationService = translationService;
            _patientStatusService = patientStatusService;
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var siteIds = await _db.StudyUserRoles
                .Where(sur => sur.StudyUserId == userId)
                .Select(sur => sur.SiteId)
                .ToListAsync();

            var patientStatuses = await _patientStatusService.GetAll();
            var inActiveStatusTypeIds = patientStatuses
                .Where(pst => !pst.IsActive)
                .Select(pst => pst.Id);

            var dailyDiaryQuestionnaireId = Guid.Parse(ConfigurationManager.AppSettings[ComplianceIdKey]);

            var rawResults = await _db
                .Patients
                .Include(p => p.Site)
                .Include(p => p.DiaryEntries)
                .Include(p => p.Devices)
                .Where(p => !inActiveStatusTypeIds.Contains(p.PatientStatusTypeId) && siteIds.Contains(p.SiteId))
                .Select(p => new
                {
                    PatientId = p.Id,
                    p.PatientStatusTypeId,
                    p.PatientNumber,
                    p.EnrolledDate,
                    p.SiteId,
                    p.Site.SiteNumber,
                    LastSyncDate = p.Devices
                        .OrderByDescending(d => d.LastSyncDate)
                        .Select(d => d.LastSyncDate)
                        .FirstOrDefault(),
                    DailyDiaryDates = p.DiaryEntries
                        .Where(de => de.QuestionnaireId == dailyDiaryQuestionnaireId)
                        .Select(de => de.DiaryDate)
                })
                .OrderBy(p => p.SiteNumber)
                .ThenBy(p => p.PatientNumber)
                .ToListAsync();

            var reportResults = new List<ReportDataDto>();

            foreach (var rawResult in rawResults)
            {
                // don't count diaries completed or missed on the date of enrollment
                var dailyDiaryCount = rawResult
                    .DailyDiaryDates
                    .Count(dd => dd.Date <= DateTime.Now.AddDays(-1).Date);

                var daysEligibleForDailyDiary = Convert.ToInt32(Math.Floor((DateTime.Now.Date - rawResult.EnrolledDate.Date).TotalDays)) - 1;
                var missedCount = daysEligibleForDailyDiary - dailyDiaryCount;

                missedCount = missedCount < 0
                    ? 0
                    : missedCount;

                var expectedCount = daysEligibleForDailyDiary < 0
                    ? 0
                    : daysEligibleForDailyDiary;

                var compliancePercentage = expectedCount > 0m
                    ? dailyDiaryCount / (expectedCount * 1.0m) * 100m
                    : 0m;

                compliancePercentage = compliancePercentage < 0
                    ? 0m
                    : compliancePercentage;
                
                compliancePercentage = compliancePercentage > 100
                    ? 100
                    : compliancePercentage;

                var reportDto = new ReportDataDto
                {
                    Row = new Dictionary<string, object>
                    {
                        { "SiteNumber", rawResult.SiteNumber },
                        { "PatientNumber", rawResult.PatientNumber },
                        { "LastSyncDate", rawResult.LastSyncDate?.ToString(SyncDateFormat) },
                        { "DiariesCompleted", dailyDiaryCount },
                        { "DiariesMissed", missedCount },
                        { "CompliancePercentage", compliancePercentage.ToString("N2") },
                    }
                };

                reportResults.Add(reportDto);
            }

            return reportResults;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            var patientLabel = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatientNumber).ConfigureAwait(false)).Result
                 ?? "Subject Number";

            var columns = new Dictionary<string, string>
            {
                {"SiteNumber", "Site"},
                {"PatientNumber", patientLabel},
                {"LastSyncDate", "Date of last data sync"},
                {"DiariesCompleted", "Diaries Completed"},
                {"DiariesMissed", "Diaries Missed"},
                {"CompliancePercentage", "Compliance %"}
            };

            return columns;
        }
    }
}