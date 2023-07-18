using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Reports
{
    public class eCOAComplianceReport : IReport
    {
        private readonly IVisitService _visitService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ITranslationService _translationService;
        private readonly IStudyDbContext _db;
        private List<VisitModel> _visits;

        public eCOAComplianceReport(IStudyDbContext db, IVisitService visitService, IPatientStatusService patientStatusService, ITranslationService translationService)
        {
            _db = db;
            _translationService = translationService;
            _visitService = visitService;
            _patientStatusService = patientStatusService;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId) => Task.FromResult<ChartDataObject>(null);


        public Dictionary<string, string> GetColumnHeadings()
        {
            return ColumnHeaderMappings();
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var reportData = new List<ReportDataDto>();
            var visits = await _visitService.GetAll();

            _visits = visits
                .OrderBy(v => v.VisitOrder)
                .ToList();

            var complianceData = await GetReportData(userId);

            foreach (var data in complianceData)
            {
                var result = new ReportDataDto();

                CheckCompletionRate(result, data.CompletionRates);

                result.Row.Add("SiteId", data.SiteId);
                result.Row.Add("SubjectNumber", data.SubjectNumber);

                foreach (var name in _visits.Select(v => v.Name))
                {
                    result.Row.Add(name,
                        data.CompletionRates[name].HasValue && data.CompletionRates[name].Value > 0
                            ? data.CompletionRates[name].Value.ToString("N2")
                            : "-");
                }

                result.Row.Add("TotalCompliance", $"{data.TotalCompleted}/{data.TotalExpected}");
                reportData.Add(result);
            }

            return reportData;
        }

        public Dictionary<string, string> ColumnHeaderMappings()
        {
            var patientNumber = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatientNumber).ConfigureAwait(false)).Result
                 ?? "Subject Number";

            var cols = new Dictionary<string, string>
            {
                {"SiteId", "Site Id"},
                {"SubjectNumber", patientNumber}
            };

            foreach (var visit in _visits)
            {
                cols.Add(visit.Name, visit.Name);
            }

            cols.Add("TotalCompliance", "Total Compliance");

            return cols;
        }

        private void CheckCompletionRate(ReportDataDto result, Dictionary<string, decimal?> completionRates)
        {
            foreach (var key in completionRates.Keys)
            {
                if (completionRates[key] < 100 && completionRates[key] != 0)
                {
                    result.CustomCellTextColors.Add(key, "#da3434");
                }
            }
        }

        private async Task<IEnumerable<ComplianceReportDto>> GetReportData(Guid? UserId = null)
        {
            var siteIdsToSearch = await _db
                .StudyUserRoles
                .Where(x => x.StudyUserId == UserId)
                .Select(x => x.SiteId)
                .ToListAsync();

            var patientStatuses = await _patientStatusService.GetAll();
            var nonActiveStatusTypeIds = patientStatuses
                .Where(pst => !pst.IsActive)
                .Select(pst => pst.Id);

            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var query = patientFilter.Execute(_db.Patients)
                .Include(p => p.Site)
                .Include(p => p.DiaryEntries)
                .Where(p => !nonActiveStatusTypeIds.Contains(p.PatientStatusTypeId));

            if (siteIdsToSearch.Any())
            {
                query = query
                    .Where(p => siteIdsToSearch.Contains(p.SiteId));
            }

            var rawResults = await query
                .Select(p => new
                {
                    p.PatientNumber,
                    p.SiteId,
                    SiteName = p.Site.Name,
                    Diaries = p.DiaryEntries.Select(de => new
                    {
                        de.CompletedTime,
                        de.VisitId,
                        de.QuestionnaireId
                    })
                })
                .OrderBy(r => r.SiteName)
                .ThenBy(r => r.PatientNumber)
                .ToListAsync();

            var visitAndQuestionnaireIds = _visits
                .SelectMany(
                    v => v.Questionnaires,
                    (v, q) => new
                    {
                        VisitId = v.Id,
                        Questionnaire = q
                    })
                .Select(vq => new
                {
                    vq.VisitId,
                    vq.Questionnaire.QuestionnaireId
                });

            var finalResults = new List<ComplianceReportDto>();

            foreach (var rawResult in rawResults)
            {
                var reportDto = new ComplianceReportDto
                {
                    SiteId = rawResult.SiteName,
                    SubjectNumber = rawResult.PatientNumber,
                    TotalCompleted = rawResult.Diaries
                        .Count(d => visitAndQuestionnaireIds.Any(vq => vq.QuestionnaireId == d.QuestionnaireId && vq.VisitId == d.VisitId)),
                    TotalExpected = visitAndQuestionnaireIds.Count()
                };

                foreach (var visit in _visits)
                {
                    var visitCompletionCount = decimal.Zero;

                    if (rawResult.Diaries.Any(d => d.VisitId == visit.Id))
                    {
                        visitCompletionCount = rawResult.Diaries.Count(d => d.VisitId == visit.Id) / (visit.Questionnaires.Count * 1.0m) * 100m;
                    }
                    
                    reportDto.CompletionRates.Add(visit.Name, visitCompletionCount);
                }

                finalResults.Add(reportDto);
            }

            return finalResults;
        }
    }
}