using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Reports.Reports
{
    public class AnswerAuditReport : ReportBase, IReport
    {
        private readonly Dictionary<string, string> _columns;
        private readonly ITranslationService _translationService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IStudySettingService _studySettingService;

        public AnswerAuditReport(
            IStudyDbContext db,
            ITranslationService translationService,
            IQuestionnaireService questionnaireService,
            IStudySettingService studySettingService) : base(db)
        {
            _translationService = translationService;
            _questionnaireService = questionnaireService;
            _studySettingService = studySettingService;

            var nomenclature = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatient).ConfigureAwait(false)).Result
                 ?? "Subject";

            // First column is field name in query or stored procedure, second column is header column on report
            _columns = new Dictionary<string, string>
            {
                {"Protocol", "Protocol"},
                {"SiteNumber", "Site Number"},
                {"SubjectNumber", nomenclature + " Number"},
                {"DiaryDate", "Diary Date"},
                {"Questionnaire", "Questionnaire"},
                {"Question", "Question"},
                {"OldValue", "Old Value"},
                {"NewValue", "New Value"},
                {"ChangeReasonType", "Change Reason Type"},
                {"ChangedBy", "Changed By"},
                {"ChangedDate", "Changed Date"},
                {"CorrectionReason", "Correction Reason"},
                {"DCFNumber", "DCF Number"},
                {"AssetTag", "Asset Tag"}
            };
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var pSites = "ALL";
            var pSubj = "ALL";

            if (parameters.ContainsKey("SITES"))
            {
                pSites = parameters["SITES"].ToString();
            }

            if (parameters.ContainsKey("SUBJ"))
            {
                pSubj = parameters["SUBJ"].ToString();
            }

            string sites = "";
            var patientId = Guid.Empty;

            if (string.IsNullOrEmpty(pSites) || pSites == "ALL")
            {
                var UserId = YPrimeSession.Instance.CurrentUser.Id;
                var SiteList = await _db.StudyUserRoles.Where(x => x.StudyUserId == UserId).Select(x => x.SiteId).ToListAsync();
                sites = string.Join(",", SiteList);
            }
            else
            {
                sites = pSites;
            }

            if (!string.IsNullOrEmpty(pSubj) && pSubj != "ALL")
            {
                patientId = new Guid(pSubj);
            }

            var subject = await GetPatientNumberById(patientId);
            var protocol = await _studySettingService.GetStringValue("Protocol");
            var questionnaires = await _questionnaireService.GetAllInflatedQuestionnaires();
            var questionnaireStubs = questionnaires.Select(q => new QuestionnaireStub
            {
                Id = q.Id,
                Name = q.DisplayName,
                InternalName = q.InternalName
            });
            var questionnaireJson = SerializeObject(questionnaireStubs);

            var questions = questionnaires.SelectMany(q => q.Pages.SelectMany(p => p.Questions)).ToList();
            var questionStubs = questions.Select(q => new QuestionStub
            {
                Id = q.Id,
                Name = q.QuestionText,
                QuestionnaireId = q.QuestionnaireId,
                Sequence = q.Sequence,
                InputFieldTypeId = q.InputFieldTypeId
            });
            var questionJson = SerializeObject(questionStubs);

            var choiceStubs = questions.SelectMany(q => q.Choices.Select(c => new ChoiceStub
            {
                Id = c.Id,
                Name = c.DisplayText,
                QuestionId = q.Id
            })).ToList();

            var choiceJson = SerializeObject(choiceStubs);

            var paperQuestionnaireDcfTypeId = CorrectionType.PaperDiaryEntry.Id.ToString();

            var spName = "[dbo].[AnswerAuditRecReport_Filtered] @p1,@p2,@p3,@p4,@p5,@p6,@p7";

            var rawData =
                _db.ExecuteSqlToList<AnswerAuditDto>(spName,
                    new SqlParameter("p1", sites),
                    new SqlParameter("p2", subject),
                    new SqlParameter("p3", protocol),
                    new SqlParameter("p4", questionnaireJson),
                    new SqlParameter("p5", questionJson),
                    new SqlParameter("p6", choiceJson),
                    new SqlParameter("p7", paperQuestionnaireDcfTypeId))
                    .ToList();

            return rawData.ToReportData(GetColumnHeadings().Keys.ToList());
        }

        private async Task<string> GetPatientNumberById(Guid patientId)
        {
            var patient = await _db
               .Patients
               .FirstOrDefaultAsync(p => p.Id == patientId);

            return patient?.PatientNumber;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return _columns;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        public class AnswerAuditDto
        {
            public string Protocol { get; set; }

            public string SiteNumber { get; set; }

            public string SubjectNumber { get; set; }

            public string DiaryDate { get; set; }

            public string Questionnaire { get; set; }

            public string Question { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }

            public string ChangeReasonType { get; set; }

            public string ChangedBy { get; set; }

            public string ChangedDate { get; set; }

            public string CorrectionReason { get; set; }

            public string DCFNumber { get; set; }

            public string AuditSource { get; set; }

            public string AssetTag { get; set; }
        }
    }
}