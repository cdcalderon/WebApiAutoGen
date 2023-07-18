using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.Repositories
{
    public class AnswerRepository : BaseRepository, IAnswerRepository
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly ITranslationService _translationService;
        private readonly IFileService _fileService;

        public AnswerRepository(
            IStudyDbContext db,
            IQuestionnaireService questionnaireService,
            ITranslationService translationService,
            IFileService fileService) 
            : base(db)
        {
            _questionnaireService = questionnaireService;
            _translationService = translationService;
            _fileService = fileService;
        }        

        public async Task<List<AnswerDto>> GetAnswers(AnswerPropertiesDto answerProperties)
        {
            var results = new List<AnswerDto>();

            var diaryEntry = await _db.DiaryEntries
                .FirstAsync(de => de.Id == answerProperties.DiaryEntryId);

            var answers = await _db.Answers
                .Where(a => a.DiaryEntryId == answerProperties.DiaryEntryId && !a.IsArchived)
                .OrderBy(a => a.AnswerOrder)
                .ToListAsync();

            var answeredQuestionIds = answers
                .Select(a => a.QuestionId)
                .ToList();

            var questionnaire = await _questionnaireService
                .GetInflatedQuestionnaire(
                    diaryEntry.QuestionnaireId,
                    configurationId: diaryEntry.ConfigurationId);

            var questions = questionnaire
                .Pages
                .SelectMany(p => p.Questions)
                .ToList();

            var unansweredQuestions = questions
                .Where(q => !answeredQuestionIds.Contains(q.Id))
                .ToList();

            foreach (var question in unansweredQuestions)
            {
                answers.Insert(0, new Answer
                {
                    Id = default,
                    QuestionId = question.Id
                });
            }

            var spirobankAnswers = new List<Answer>();

            foreach (var answer in answers)
            {
                var matchingQuestion = questions
                    .First(q => q.Id == answer.QuestionId);

                if (matchingQuestion.GetInputFieldType() != InputFieldType.Spirobank)
                {                            
                    var dto = answer.ToDto(
                        matchingQuestion,
                        diaryEntry, 
                        useMetricForAnswers : answerProperties.UseMetricForAnswers);

                    results.Add(dto);
                }
                else
                {
                    spirobankAnswers.Add(answer);
                }
            }

            results.AddRange(await BuildSpirobankAnswers(spirobankAnswers, questions));     
            return results;
        }       

        public async Task<Image> GetImage(Guid Id)
        {
            var answer = _db.Answers.Single(a => a.Id == Id);
            Image result = await _fileService.GetDiaryAnswerImage(answer.FreeTextAnswer, answer.DiaryEntry.PatientId,
                answer.DiaryEntry.VisitId, answer.DiaryEntry.QuestionnaireId);
            return result;
        }

        private async Task<List<AnswerDto>> BuildSpirobankAnswers(
            List<Answer> answers,
            List<QuestionModel> questions)
        {
            var result = new List<AnswerDto>();

            if (!answers.Any())
            {
                return result;
            }

            var testTimeTranslation = await _translationService
                .GetByKey("TestTime") ?? string.Empty;

            var answerGroups = answers
                .GroupBy(a => a.AnswerOrder)
                .OrderBy(ag => ag.Key);

            foreach (var answerGroup in answerGroups)
            {
                var headerAnswer = answerGroup.First();

                var headerQuestion = questions
                    .First(q => q.Id == headerAnswer.QuestionId);

                var headerRow = headerAnswer.ToDto(headerQuestion);
                result.Add(headerRow);
                
                foreach (var answer in answerGroup)
                {
                    var matchingQuestion = questions
                        .First(q => q.Id == answer.QuestionId);

                    result.Add(await BuildSpirobankResultRow(
                        answer, 
                        matchingQuestion,
                        testTimeTranslation));
                }
            }

            return result;
        }

        private async Task<AnswerDto> BuildSpirobankResultRow(
            Answer answer,
            QuestionModel question,
            string testTimeTranslation)
        {
            string questionText;
            string answerText;

            if (answer.ChoiceId.HasValue)
            {
                var questionInputFieldTypeResult = await _db.QuestionInputFieldTypeResults
                    .Include(q => q.InputFieldTypeResult)
                    .FirstOrDefaultAsync(q => q.ChoiceId == answer.ChoiceId);

                var inputFieldTypeResult = questionInputFieldTypeResult?.InputFieldTypeResult 
                    ?? new InputFieldTypeResult();

                questionText = $"{inputFieldTypeResult.ResultCode}";
                answerText = $"{answer.FreeTextAnswer} {inputFieldTypeResult.UnitOfMeasure}";
            }
            else
            {
                questionText = testTimeTranslation;
                answerText = answer.FreeTextAnswer;
            }

            var dto = answer.ToDto(
                question, 
                customQuestionText: questionText, 
                customAnswerText: answerText);

            return dto;
        }
    }
}