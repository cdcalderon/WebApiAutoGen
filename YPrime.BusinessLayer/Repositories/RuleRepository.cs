using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessRule.Interfaces;
using YPrime.BusinessRule.Services;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.Repositories
{
    public class RuleRepository : RuleServiceBase, IRuleService
    {
        private readonly IStudyDbContext _db;
        private readonly IBusinessRuleService businessRuleService;
        private readonly IQuestionnaireService questionnaireService;

        public RuleRepository(
            IStudyDbContext db,
            IBusinessRuleOperationFactory factory,
            IBusinessRuleService businessRuleService,
            IQuestionnaireService questionnaireService)
            : base(factory)
        {
            _db = db;
            this.businessRuleService = businessRuleService;
            this.questionnaireService = questionnaireService;
        }

        protected override IEnumerable<IAlarmSchedule> GetAlarmSchedules(Guid alarmId)
        {
            return new List<IAlarmSchedule>();
        }

        protected override IEnumerable<IAnswer> GetAnswersFromDiaryEntries(List<Guid> diaryEntryIds, Guid questionId)
        {
            var answers = Enumerable.Empty<IAnswer>();

            if (diaryEntryIds.Any())
            {
                answers = _db.Answers
                    .Where(a =>
                        a.QuestionId == questionId &&
                        !a.IsArchived &&
                        diaryEntryIds.Contains(a.DiaryEntryId));
            }

            return answers;
        }

        protected override IBusinessRule GetBusinessRule(Guid businessRuleId)
        {
            BusinessRuleModel businessRuleModel = new BusinessRuleModel();
            Task.Run(async () =>  businessRuleModel = await businessRuleService.Get(businessRuleId)).Wait();
            return businessRuleModel;
        }

        protected override IEnumerable<IDiaryEntry> GetDiaryEntries(Guid? patientId, Guid questionnaireId)
        {
            return _db.DiaryEntries
                .Where(de =>
                    de.PatientId == patientId &&
                    de.QuestionnaireId == questionnaireId);
        }

        protected override IAnswer GetDiaryEntryAnswer(Guid diaryEntryId, Guid questionId)
        {
            return _db.Answers
                .FirstOrDefault(a =>
                    a.DiaryEntryId == diaryEntryId &&
                    !a.IsArchived &&
                    a.QuestionId == questionId);
        }

        protected override IPatient GetPatient(Guid? patientId)
        {
            return _db.Patients
                .FirstOrDefault(p => p.Id == patientId);
        }

        protected override IPatientAttribute GetPatientAttribute(Guid? patientId,
            Guid patientAttributeConfigurationDetailId)
        {
            return _db.PatientAttributes
                .FirstOrDefault(pa =>
                    pa.PatientId == patientId &&
                    pa.PatientAttributeConfigurationDetailId == patientAttributeConfigurationDetailId);
        }

        protected override IPatientVisit GetPatientVisit(Guid? patientId, Guid visitId)
        {
            return _db.PatientVisits
                .FirstOrDefault(pv => pv.PatientId == patientId && pv.VisitId == visitId);
        }

        protected override IPatientVisit GetLatestPatientVisit(Guid? patientId)
        {
            return _db.PatientVisits
                .Where(pv => pv.PatientId == patientId)
                .OrderByDescending(pv => pv.VisitDate)
                .FirstOrDefault();
        }

        protected override ISite GetSite(Guid siteId)
        {
            var siteFilter = new SiteFilter();

            var sites = siteFilter.Execute(_db.Sites)
                .FirstOrDefault(s => s.Id == siteId);
            return sites;
                
        }

        protected override IQuestion GetQuestion(Guid questionnaireId, Guid questionId)
        {
            List<QuestionModel> questionModels = new List<QuestionModel>();
            Task.Run(async () => questionModels = await questionnaireService.GetQuestions(questionnaireId)).Wait();
            return questionModels?.FirstOrDefault(q => q.Id == questionId);
        }

        protected override IChoice GetChoice(Guid questionnaireId, Guid questionId, Guid choiceId)
        {
            var question = GetQuestion(questionnaireId, questionId) as QuestionModel;
            return question?.Choices?.FirstOrDefault(c => c.Id == choiceId);
        }
    }
}
