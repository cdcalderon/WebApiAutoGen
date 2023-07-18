using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.Extensions
{
    public static class AnswerExtensions
    {
        private const string SupportedImageTypesRegexPattern = @"^.*(\.png|\.jpg)$";

        public static AnswerDto ToDto(
            this Answer entity,
            QuestionModel question,
            DiaryEntry diaryEntry = null,
            string customQuestionText = null,
            string customAnswerText = null,
            bool useMetricForAnswers = true)
        {
            var choice = question
                ?.Choices
                ?.FirstOrDefault(c => c.Id == entity.ChoiceId);

            var questionText = !string.IsNullOrWhiteSpace(customQuestionText)
                ? customQuestionText
                : question?.QuestionText ?? string.Empty;

            var answerText = !string.IsNullOrWhiteSpace(customAnswerText)
                ? customAnswerText
                : choice?.DisplayText ?? entity.FreeTextAnswer;

            var freeText = entity.FreeTextAnswer;
            if (question.IsTemperatureQuestionType())
            {
                // both need to be set for dcf to display properly 
                answerText = FormatTemperatureAnswers(answerText, useMetricForAnswers);
                freeText = answerText;
            }

            var dto = new AnswerDto
            {
                Id = entity.Id,
                DiaryEntry = diaryEntry?.ToDto(),
                Question = question,
                ChoiceId = entity.ChoiceId,
                Choice = choice,
                ChoiceSequence = choice?.Sequence ?? 0,
                FreeTextAnswer = question.IsTemperatureQuestionType() ? freeText : entity.FreeTextAnswer,
                DiaryEntryId = entity.DiaryEntryId,
                QuestionId = entity.QuestionId,
                DisplayQuestion = questionText,
                DisplayAnswer = answerText ?? string.Empty,
                MultipleChoice = question.GetInputFieldType().MultipleChoice,
                IsCountryMetric = useMetricForAnswers
                // fill in once we get diary pages - PageNumber = question.DiaryPage.Number
            };

            if (dto.Question.IsTemperatureQuestionType() && !string.IsNullOrEmpty(freeText) && freeText != TemperatureControlConstants.NotRequiredAnswer)
            {
                dto.Suffix = useMetricForAnswers ? Temperature.DegreesCelsius : Temperature.DegreesFahrenheit;
            }

            return dto;
        }
        private static string FormatTemperatureAnswers(string answerValue, bool useMetricForAnswers)
        {
            if (!string.IsNullOrWhiteSpace(answerValue))
            {
                return answerValue != TemperatureControlConstants.NotRequiredAnswer
                    ? new Temperature(answerValue, useMetricForAnswers).Value.ToString(TemperatureControlConstants.DisplayFormat)
                    : answerValue;
            }
            
            return answerValue;
        }

        public static bool HasValidImageExtension(
            this AnswerDto entity)
        {
            if (string.IsNullOrWhiteSpace(entity?.DisplayAnswer))
            {
                return false;
            }

            var regex = new Regex(SupportedImageTypesRegexPattern, RegexOptions.IgnoreCase);
            var result = regex.Matches(entity.DisplayAnswer.Trim())?.Count > 0;

            return result;
        }

        /// <summary>
        /// Format final answer text depending on the question type
        /// </summary>
        /// <param name="entity">AnswerDto</param>
        /// <returns>The text to display</returns>
        public static string FormatDisplayAnswer(this AnswerDto entity)
        {
            string result = entity.DisplayAnswer;
            if (entity.Question.IsTemperatureQuestionType() && !string.IsNullOrEmpty(entity.Suffix))
            {
                result = $"{entity.DisplayAnswer}{entity.Suffix}";                
            }

            return result;
        }
    }
}
