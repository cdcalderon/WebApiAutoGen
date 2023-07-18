using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Reports.Reports
{
    public class DuplicateSubjectReport : IReport
    {
        private readonly IStudyDbContext _db;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;

        public DuplicateSubjectReport(IStudyDbContext db, ITranslationService translationService, IPatientStatusService patientStatusService)
        {
            _db = db;
            _translationService = translationService;
            _patientStatusService = patientStatusService;
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var data = new List<ReportDataDto>();

            var patientStatuses = await _patientStatusService.GetAll();
            var removedStatusIds = patientStatuses
                .Where(pst => pst.IsRemoved)
                .Select(pst => pst.Id);

            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var duplicatePatients = patientFilter.Execute(_db.Patients)
                .Where(p => 
                    !removedStatusIds.Contains(p.PatientStatusTypeId))
                .GroupBy(p => p.PatientNumber)
                .Where(g => g.Count() > 1)
                .SelectMany(p => p)
                .OrderBy(p => p.Site.SiteNumber)
                .ThenBy(p => p.PatientNumber)
                .ThenBy(p => p.EnrolledDate)
                .ToList();

            foreach (var p in duplicatePatients)
            {
                var result = new ReportDataDto();
                result.Row.Add("Site", p.Site.SiteNumber);
                result.Row.Add("SubjectNumber", p.PatientNumber);
                result.Row.Add("EnrolledDate", p.EnrolledDate.ToString(DateTimeFormatConstants.DefaultDateTime));
                data.Add(result);
            }

            return data;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return ColumnHeaderMappings();
        }

        public async Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return await Task.FromResult<ChartDataObject>(null);
        }

        public Dictionary<string, string> ColumnHeaderMappings()
        {
            var patientNumber = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatientNumber).ConfigureAwait(false)).Result
                 ?? "Subject Number";

            var columns = new Dictionary<string, string>
            {
                {"Site", "Site"},
                {"SubjectNumber", patientNumber},
                {"EnrolledDate", "Enrolled Date"}
            };

            return columns;
        }
    }
}