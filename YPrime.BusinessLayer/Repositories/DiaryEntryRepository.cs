using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class DiaryEntryRepository : BaseRepository, IDiaryEntryRepository
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IDiaryPageRepository _diaryPageRepository;
        private readonly ISiteRepository _siteRepository;

        private readonly IQuestionnaireService _questionnaireService;
        private readonly ITranslationService _translationService;
        private readonly IVisitService _visitService;
        private readonly IConfigurationVersionService _configVersionService;
        private readonly IFileService _fileService;
        private readonly ICountryService _countryService;

        public DiaryEntryRepository(IStudyDbContext db,
            ITranslationService translationService,
            IAnswerRepository answerRepository, 
            IDiaryPageRepository diaryPageRepository, 
            ISiteRepository siteRepository, 
            IVisitService visitService,
            IQuestionnaireService questionnaireService,
            IFileService fileService,
            IConfigurationVersionService configVersionService ,
            ICountryService countryService
        ) : base(db)
        {
            _translationService = translationService;
            _answerRepository = answerRepository;
            _diaryPageRepository = diaryPageRepository;
            _siteRepository = siteRepository;
            _visitService = visitService;
            _questionnaireService = questionnaireService;
            _fileService = fileService;
            _configVersionService = configVersionService;
            _countryService = countryService;
        }

        public Task<List<DiaryEntryDto>> GetDiaryEntriesInflated(
            Guid? questionnaireId, 
            bool includeAnswers, 
            bool? isUserBlinded,
            Guid? patientId = null)
        {
            var result = PopulateDiaryEntryDtos(
                questionnaireId,
                includeAnswers,
                isUserBlinded,
                null,
                patientId);

            return result;
        }

        public Task<List<DiaryEntryDto>> GetAllPatientDiaryEntriesByVisit(
            Guid? patientId, 
            Guid? visitId,
            bool includeAnswers,
            bool? isBlinded, 
            string cultureCode)
        {
            var result = PopulateDiaryEntryDtos(
                null,
                includeAnswers,
                isBlinded,
                visitId,
                patientId);

            return result;
        }

        public IQueryable<DiaryEntryDto> GetDiaryEntries(Guid patientId, List<QuestionnaireModel> questionnaires)
        {
            var diaryEntries = _db.DiaryEntries.Where(de => de.PatientId == patientId).OrderByDescending(d => d.CompletedTime)
                .Select(diaryEntry => new DiaryEntryDto
                {
                    Id = diaryEntry.Id,
                    QuestionnaireId = diaryEntry.QuestionnaireId,
                    VisitId = diaryEntry.VisitId,
                    ConfigurationId = diaryEntry.ConfigurationId,
                    DiaryStatusId = diaryEntry.DiaryStatusId,
                    DiaryDate = diaryEntry.DiaryDate,
                    DataSourceId = diaryEntry.DataSourceId,
                    PatientId = diaryEntry.PatientId,
                    DeviceId = diaryEntry.DeviceId,
                    StartedTime = diaryEntry.StartedTime,
                    CompletedTime = diaryEntry.CompletedTime,
                    TransmittedTime = diaryEntry.TransmittedTime,
                    ReviewedByUserid = diaryEntry.ReviewedByUserid,
                    ReviewedDate = diaryEntry.ReviewedDate
                }); 

            return diaryEntries;
        }

        public async Task<DiaryEntryDto> GetDiaryEntry(Guid Id, bool includeDCFData, string CultureCode)
        {
            var diaryEntry = _db.DiaryEntries
                .Include(e => e.Patient)
                .First(e => e.Id == Id);

            var matchingVisit = diaryEntry.VisitId.HasValue && diaryEntry.VisitId != Guid.Empty
                ? await _visitService.Get(diaryEntry.VisitId.Value, diaryEntry.ConfigurationId)
                : null;

            var matchingQuestionnaire = await _questionnaireService.GetInflatedQuestionnaire(
                diaryEntry.QuestionnaireId,
                configurationId: diaryEntry.ConfigurationId);

            var diaryEntryDto = diaryEntry.ToDto(
                matchingQuestionnaire,
                matchingVisit);

            diaryEntryDto.Site = await _siteRepository.GetSite(diaryEntryDto.SiteId);

            var country = await _countryService.Get(diaryEntryDto.Site.CountryId);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = diaryEntryDto.Id,
                UseMetricForAnswers = country.UseMetric
            };

            diaryEntryDto.Answers = await _answerRepository
                .GetAnswers(answerProperties);

            diaryEntryDto.DiaryPages = await _diaryPageRepository.GetQuestionnaireDiaryPages(
                diaryEntryDto.QuestionnaireId, diaryEntryDto.ConfigurationId);
         
            var config = await _configVersionService.Get(diaryEntry.ConfigurationId);
            
            if (diaryEntryDto.DataSourceId == DataSource.Paper.Id)
            {
                diaryEntryDto.Version = config.ConfigurationVersionNumber + "-" + config.SrdVersion;
            }
            else
            {
                diaryEntryDto.Version = $"{diaryEntry.SoftwareVersionNumber}-{config.ConfigurationVersionNumber}-{config.SrdVersion}";
            }
            
            foreach (var answer in diaryEntryDto.Answers)
            {
                var matchingPage = diaryEntryDto.DiaryPages
                    .FirstOrDefault(p => p.Questions.Any(q => q.Id == answer.QuestionId));

                answer.PageNumber = matchingPage?.Number ?? 0;
            }

            if (includeDCFData)
            {
                diaryEntryDto.QuestionAnswers = await CreateDcfAnswersFromQuestionnaire(
                    matchingQuestionnaire, 
                    diaryEntryDto.Answers, 
                    true,
                    diaryEntry.ConfigurationId);

                _db.CorrectionApprovalDatas.Where(cad => cad.RowId == Id && cad.Correction.CorrectionStatusId == CorrectionStatusEnum.Completed).ToList().ForEach(cad =>
                {
                    diaryEntryDto.CorrectionApprovalDataDtos.Add(cad.EntityToDto());
                });
            }

            return diaryEntryDto;
        }

        public DiaryEntry GetDiaryEntry(Guid id)
        {
            return _db.DiaryEntries.SingleOrDefault(x => x.Id == id);
        }

        public async Task<DiaryEntryDto> GetLastDiaryEntryByDevice(Guid DeviceId, string CultureCode)
        {
            DiaryEntryDto result = null;
            var lastDiaryEntryId = _db.DiaryEntries.OrderByDescending(e => e.TransmittedTime)
                .FirstOrDefault(e => e.DeviceId == DeviceId)?.Id;

            if (lastDiaryEntryId != null)
            {
                result = await GetDiaryEntry((Guid) lastDiaryEntryId, false, CultureCode);
            }

            return result;
        }

        public async Task SaveDiaryEntryImageToDisk(Guid DiaryEntryId, string Base64, string FileName)
        {
            //get the entry and work out the path
            var entry = await GetDiaryEntry(DiaryEntryId, false, TranslationConstants.DefaultCultureCode);
            // Patient, then Visit, then questionnaire: '-' seperated.
            await _fileService.SaveDiaryImage(Base64, entry.PatientId, entry.VisitId, entry.QuestionnaireId, FileName);
        }
        private async Task<List<DiaryEntryDto>> PopulateDiaryEntryDtos(
            Guid? questionnaireId,
            bool includeAnswers,
            bool? isUserBlinded,
            Guid? visitId,
            Guid? patientId = null)
        {
            var visits = await _visitService.GetAll();
            var countries = await _countryService.GetAll();         

            var diaryEntries = await _db.DiaryEntries
                .Where(e =>
                    (patientId == null || e.PatientId == patientId) &&
                    (questionnaireId == null || e.QuestionnaireId == questionnaireId) &&
                    (visitId == null || e.VisitId == visitId))
                .ToListAsync();

            var questionnaireDictionary = new Dictionary<Guid?, List<QuestionnaireModel>>();

            var diaryEntryConfigIds = diaryEntries
                .Select(de => de.ConfigurationId)
                .Distinct()
                .ToList();

            foreach (var diaryEntryConfigId in diaryEntryConfigIds)
            {
                var questionnaires = await _questionnaireService
                    .GetAllWithPages(diaryEntryConfigId);

                questionnaireDictionary.Add(diaryEntryConfigId, questionnaires);
            }

            var diaryEntryDtos = new List<DiaryEntryDto>();

            foreach (var diaryEntry in diaryEntries)
            {
                var matchingQuestionnaire = questionnaireDictionary[diaryEntry.ConfigurationId]
                    .FirstOrDefault(q => q.Id == diaryEntry.QuestionnaireId);

                if (isUserBlinded.HasValue &&
                    isUserBlinded.Value &&
                    matchingQuestionnaire.CanBlindedSeeAnswers.HasValue &&
                    !matchingQuestionnaire.CanBlindedSeeAnswers.Value)
                {
                    continue;
                }

                var matchingVisit = visits
                    .FirstOrDefault(v => v.Id == diaryEntry.VisitId);

                var dto = diaryEntry.ToDto(
                    matchingQuestionnaire,
                    matchingVisit);

                diaryEntryDtos.Add(dto);
            }

            if (includeAnswers)
            {
                foreach (var diaryEntryDto in diaryEntryDtos)
                {
                    diaryEntryDto.Site = await _siteRepository
                        .GetSite(diaryEntryDto.SiteId);

                   var isMetric = countries.FirstOrDefault(x => x.Id == diaryEntryDto.Site.CountryId).UseMetric;

                    AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
                    {
                        DiaryEntryId = diaryEntryDto.Id,
                        UseMetricForAnswers = isMetric
                    };

                    diaryEntryDto.Answers = await _answerRepository
                        .GetAnswers(answerProperties);
          
                    diaryEntryDto.DiaryPages = await _diaryPageRepository
                        .GetQuestionnaireDiaryPages(diaryEntryDto.QuestionnaireId, diaryEntryDto.ConfigurationId);
                }
            }

            return diaryEntryDtos
                .OrderBy(e => e.Id)
                .ToList();
        }

        private async Task<List<QuestionAnswerDto>> CreateDcfAnswersFromQuestionnaire(
            QuestionnaireModel questionnaire, 
            List<AnswerDto> answers, 
            bool translate,
            Guid? configurationId)
        {
            var answerResult = new List<QuestionAnswerDto>();
            var answerPosition = 0;

            questionnaire.RemoveNonDcfQuestions();
            var questions = questionnaire.GetSortedquestions();

            foreach (var question in questions)
            { 
                if (!answerResult.Any(a => a.Question.Id == question.Id))
                {
                    await InitQuestionAnswerQuestion(
                        translate, 
                        question, 
                        answerResult, 
                        configurationId);
                }

                var questionAnswers = answerResult.Single(a => a.Question.Id == question.Id).Answers;

                if (!answers.Any(a => a.QuestionId == question.Id))
                {
                    InitQuestionAnswerAnswers(question, ref answerPosition, questionAnswers);
                }

                //load pre-existing answers
                if (question.GetInputFieldType().MultipleChoice)
                {
                    question.Choices.OrderBy(c => c.Sequence).ToList().ForEach(c =>
                    {
                        questionAnswers.Add(new AnswerDto()
                        {
                            IsRequired = question.IsRequired,
                            Checked = answers.Any(ans => ans.ChoiceId == c.Id && ans.QuestionId == question.Id),
                            MultipleChoice = question.GetInputFieldType().MultipleChoice,
                            Position = answerPosition,
                            QuestionId = question.Id,
                            ChoiceId = c.Id,
                            Choice = c,
                            Id = answers.SingleOrDefault(ans => ans.ChoiceId == c.Id && ans.QuestionId == question.Id)?.Id ?? Guid.Empty
                        });

                        answerPosition++;
                    });
                }
                else
                {
                    answers.Where(a => a.QuestionId == question.Id).ToList().ForEach(a =>
                    {

                        if (a.ChoiceId != null && a.Choice == null)
                        {
                            a.Choice = question.Choices.Single(c => c.Id == a.ChoiceId);
                        }
                        questionAnswers.Add(a);

                        if (question.QuestionType != InputFieldType.None.Id)
                        {
                            a.Position = answerPosition;
                            answerPosition++;
                        }
                    });
                }
            }

            return answerResult;
        }

        private void InitQuestionAnswerAnswers(QuestionModel q, ref int answerPosition, List<AnswerDto> questionAnswers)
        {
            var isRequired = q.QuestionType != 1 
                ? q.IsRequired 
                : false;

            if (!q.GetInputFieldType().MultipleChoice)
            {
                questionAnswers.Add(new AnswerDto
                {
                    IsRequired = isRequired,
                    Checked = true,
                    MultipleChoice = q.GetInputFieldType().MultipleChoice,
                    Position = answerPosition
                });

                answerPosition++;
            }
        }

        private async Task InitQuestionAnswerQuestion(
            bool translate, 
            QuestionModel q,
            List<QuestionAnswerDto> answerResult,
            Guid? configurationId)
        {
            if (translate && q.QuestionSettings != null)
            {
                q.QuestionSettings.MinValueText = (await _translationService.GetByKey(q.QuestionSettings.MinValueTextKey, configurationId));
                q.QuestionSettings.MaxValueText = (await _translationService.GetByKey(q.QuestionSettings.MaxValueTextKey, configurationId));
            }

            answerResult.Add(new QuestionAnswerDto
            {
                Question = q,
                Answers = new List<AnswerDto>()
            });
        }
    }
}