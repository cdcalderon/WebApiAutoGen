using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.Reports
{
    public class VisitComplianceReport : ReportBase, IReport
    {
        private readonly IVisitService _visitService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly List<string> _internalNameColumns;

        public VisitComplianceReport(IStudyDbContext db, 
            IVisitService visitService,
            ITranslationService translationService,
            IQuestionnaireService questionnaireService,
            IPatientStatusService patientStatusService) : base(db)
        {
            _internalNameColumns = new List<string>();
            _visitService = visitService;
            _translationService = translationService;
            _questionnaireService = questionnaireService;
            _patientStatusService = patientStatusService;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return ColumnHeaderMappings();
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {

            var questionnaires = await _questionnaireService.GetAllInflatedQuestionnaires();
            var questionnaireStubs = questionnaires.Select(q => new QuestionnaireStub
            {
                Id = q.Id,
                Name = q.DisplayName,
                InternalName = q.InternalName,
                QuestionnaireTypeId = q.QuestionnaireTypeId
            });

            var visits = await _visitService.GetAll();
            var visitStubs = visits.Select(v => new VisitStub
            {
                Id = v.Id, 
                Name = v.Name,
                IsScheduled = v.IsScheduled,
                VisitOrder = v.VisitOrder
            });

            var visitQuestionnaires = visits.SelectMany(v => v.Questionnaires.Select(vq => new VisitQuestionnaireStub
            {
                VisitId = v.Id, 
                QuestionnaireId = vq.QuestionnaireId
            }));

            var visitComplianceReportQuestionnaireTypes = visits.SelectMany(v => v.Questionnaires.Select(vq => new VisitComplianceReportQuestionnaireTypeStub
            {
                VisitId = v.Id,
                QuestionnaireTypeId = questionnaires.FirstOrDefault(q => q.Id == vq.QuestionnaireId).QuestionnaireTypeId
            }));


            var patientVisitStatusTypes = PatientVisitStatusType.GetAll<PatientVisitStatusType>();
            var patientStatusTypes = await _patientStatusService.GetAll();

            var questionnaireJson = SerializeObject(questionnaireStubs);
            var visitJson = SerializeObject(visitStubs);
            var patientVisitStatusTypeJson = SerializeObject(patientVisitStatusTypes);
            var patientStatusTypeJson = SerializeObject(patientStatusTypes);
            var visitQuestionnaireJson = SerializeObject(visitQuestionnaires);
            var visitComplianceReportQuestionnaireTypeJson = SerializeObject(visitComplianceReportQuestionnaireTypes);

            var reportData = new List<ReportDataDto>();
            const string spName = "[dbo].[VisitComplianceReport]";

            var paramDictionary = new Dictionary<string, object>
            {
                { "QuestionnaireJson", questionnaireJson },
                { "VisitJson", visitJson },
                { "VisitQuestionnaireJson", visitQuestionnaireJson },
                { "PatientVisitStatusTypeJson", patientVisitStatusTypeJson },
                { "PatientStatusTypeJson", patientStatusTypeJson },
                { "VisitComplianceReportQuestionnaireTypeJson", visitComplianceReportQuestionnaireTypeJson },
            };

            var rawData = _db.CollectionFromSqlStoredProcedure(spName, paramDictionary).ToList();
            _internalNameColumns.Clear();

            var sites = _db.StudyUserRoles.Where(sur => sur.StudyUserId == userId).Select(s => s.SiteId).Distinct().ToList();

            var siteData = rawData.Where(x => sites.Contains(x.SiteId))
            .Select(data =>
            {
                IDictionary<string, object> item = data;
                item.Remove("SiteId");
                item.Remove("PatientId");

                return item;
            })
            .ToList();


            var nonQuestionnaireColumns = new List<string>
            {
                "SiteNumber", "PatientNumber", "VisitName", "VisitDate", "VisitCompliance", "SiteComplianceRate",
                "DateOfDeactivation", "DateOfReactivation"
            };

            bool FirstPass = true;

            foreach (var data in siteData)
            {
                double cnt = 0;
                double total = 0;

                var result = new ReportDataDto
                {
                    Row = new Dictionary<string, object>(data)
                };

                foreach (var row in result.Row.Where(r => !nonQuestionnaireColumns.Contains(r.Key)))
                {
                    CheckCompleted(result, row.Value.ToString(), row.Key, ref cnt, ref total);
                    if (FirstPass)
                    {
                        _internalNameColumns.Add(row.Key);
                    }
                }

                FirstPass = false;

                reportData.Add(result);
            }
   
            return reportData;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        public Dictionary<string, string> ColumnHeaderMappings()
        {
            var patientLabel = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatient).ConfigureAwait(false)).Result
                 ?? "Subject";

            var rtn = new Dictionary<string, string>
            {
                {"SiteNumber", "Site"},
                {"PatientNumber", patientLabel},
                {"VisitName", "Visit"},
                {"VisitDate", "Visit Date"}
            };
            rtn.Add("VisitCompliance", "Visit Compliance");
            rtn.Add("SiteComplianceRate", "Average Compliance for Visits");
            rtn.Add("DateOfDeactivation", "Date of Deactivation");
            rtn.Add("DateOfReactivation", "Date of Reactivation");

            foreach (var value in _internalNameColumns)
            {
                rtn.Add(value, value);
            }

            return rtn;
        }

        private void CheckCompleted(ReportDataDto result, string val, string key, ref double cnt, ref double total)
        {
            if (!string.IsNullOrEmpty(val))
            {
                cnt++;
                if (val == "Completed")
                {
                    total++;
                }
                else
                {
                    result.CustomCellTextColors.Add(key, "#da3434");
                }
            }
        }
    }
}