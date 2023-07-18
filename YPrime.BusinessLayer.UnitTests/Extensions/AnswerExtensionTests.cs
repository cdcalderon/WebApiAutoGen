using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Extensions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class AnswerExtensionTests
    {
        [TestMethod]
        public void ToDtoWithFreeTextTest()
        {
            var testQuestionId = Guid.NewGuid();
            var questionType = InputFieldType.NumberSpinner;

            var testQuestion = new QuestionModel
            {
                Id = testQuestionId,
                QuestionText = Guid.NewGuid().ToString(),
                QuestionType = questionType.Id,
                Choices = new List<QuestionChoiceModel>()
            };

            var testAnswer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = testQuestionId,
                ChoiceId = null,
                FreeTextAnswer = Guid.NewGuid().ToString(),
                DiaryEntryId = Guid.NewGuid(),
            };

            var result = testAnswer.ToDto(
                testQuestion);

            Assert.AreEqual(testAnswer.Id, result.Id);
            
            Assert.AreEqual(testAnswer.ChoiceId, result.ChoiceId);
            Assert.AreEqual(0, result.ChoiceSequence);
            Assert.AreEqual(testAnswer.FreeTextAnswer, result.FreeTextAnswer);
            Assert.AreEqual(testAnswer.DiaryEntryId, result.DiaryEntryId);
            Assert.AreEqual(testAnswer.QuestionId, result.QuestionId);
            Assert.AreEqual(testQuestion.QuestionText, result.DisplayQuestion);
            Assert.AreEqual(testAnswer.FreeTextAnswer, result.DisplayAnswer);
            Assert.AreEqual(questionType.MultipleChoice, result.MultipleChoice);
            Assert.IsNull(result.Choice);
            Assert.IsNull(result.DiaryEntry);

            result.Question.Should().BeEquivalentTo(testQuestion);
        }

        [TestMethod]
        public void ToDtoWithChoiceTest()
        {
            var testQuestionId = Guid.NewGuid();
            var questionType = InputFieldType.RadioButton;
            var testChoiceValueA = 1.1f;
            var testChoiceValueB = 2;

            var testChoiceA = new QuestionChoiceModel
            {
                Id = Guid.NewGuid(),
                QuestionId = testQuestionId,
                Sequence = 1,
                DisplayText = Guid.NewGuid().ToString(), 
                ChoiceFloatValue = testChoiceValueA
            };

            var testChoiceB = new QuestionChoiceModel
            {
                Id = Guid.NewGuid(),
                QuestionId = testQuestionId,
                Sequence = 2,
                DisplayText = Guid.NewGuid().ToString(),
                ChoiceFloatValue = testChoiceValueB
            };

            var testQuestion = new QuestionModel
            {
                Id = testQuestionId,
                QuestionText = Guid.NewGuid().ToString(),
                QuestionType = questionType.Id,
                Choices = new List<QuestionChoiceModel>
                {
                    testChoiceA,
                    testChoiceB
                }
            };

            var testAnswer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = testQuestionId,
                ChoiceId = testChoiceB.Id,
                FreeTextAnswer = string.Empty,
                DiaryEntryId = Guid.NewGuid(),
            };

            var result = testAnswer.ToDto(
                testQuestion);

            Assert.AreEqual(testAnswer.Id, result.Id);

            Assert.AreEqual(testAnswer.ChoiceId, result.ChoiceId);
            Assert.AreEqual(testChoiceB.Sequence, result.ChoiceSequence);
            Assert.AreEqual(testAnswer.FreeTextAnswer, result.FreeTextAnswer);
            Assert.AreEqual(testAnswer.DiaryEntryId, result.DiaryEntryId);
            Assert.AreEqual(testAnswer.QuestionId, result.QuestionId);
            Assert.AreEqual(testQuestion.QuestionText, result.DisplayQuestion);
            Assert.AreEqual(testChoiceB.DisplayText, result.DisplayAnswer);
            Assert.AreEqual(questionType.MultipleChoice, result.MultipleChoice);
            Assert.IsNull(result.DiaryEntry);

            result.Choice.Should().BeEquivalentTo(testChoiceB);
            result.Question.Should().BeEquivalentTo(testQuestion);
        }

        [TestMethod]
        public void ToDtoWithCustomTextTest()
        {
            var testQuestionId = Guid.NewGuid();
            var questionType = InputFieldType.NumberSpinner;

            var customQuestionText = Guid.NewGuid().ToString();
            var customAnswerText = Guid.NewGuid().ToString();

            var testQuestion = new QuestionModel
            {
                Id = testQuestionId,
                QuestionText = Guid.NewGuid().ToString(),
                QuestionType = questionType.Id,
                Choices = new List<QuestionChoiceModel>()
            };

            var testAnswer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = testQuestionId,
                ChoiceId = null,
                FreeTextAnswer = Guid.NewGuid().ToString(),
                DiaryEntryId = Guid.NewGuid(),
            };

            var result = testAnswer.ToDto(
                testQuestion,
                null,
                customQuestionText,
                customAnswerText);

            Assert.AreEqual(testAnswer.Id, result.Id);

            Assert.AreEqual(testAnswer.ChoiceId, result.ChoiceId);
            Assert.AreEqual(0, result.ChoiceSequence);
            Assert.AreEqual(testAnswer.FreeTextAnswer, result.FreeTextAnswer);
            Assert.AreEqual(testAnswer.DiaryEntryId, result.DiaryEntryId);
            Assert.AreEqual(testAnswer.QuestionId, result.QuestionId);
            Assert.AreEqual(customQuestionText, result.DisplayQuestion);
            Assert.AreEqual(customAnswerText, result.DisplayAnswer);
            Assert.AreEqual(questionType.MultipleChoice, result.MultipleChoice);
            Assert.IsNull(result.Choice);
            Assert.IsNull(result.DiaryEntry);

            result.Question.Should().BeEquivalentTo(testQuestion);
        }

        [TestMethod]
        public void HasValidImageExtension_JpgTrueTest()
        {
            var testAnswer = new eCOA.DTOLibrary.AnswerDto
            {
                DisplayAnswer = $"{Guid.NewGuid()}.jpg"
            };

            var result = testAnswer.HasValidImageExtension();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasValidImageExtension_PngTrueTest()
        {
            var testAnswer = new eCOA.DTOLibrary.AnswerDto
            {
                DisplayAnswer = $"{Guid.NewGuid()}.png"
            };

            var result = testAnswer.HasValidImageExtension();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasValidImageExtension_CaseSensitiveTrueTest()
        {
            var testAnswer = new eCOA.DTOLibrary.AnswerDto
            {
                DisplayAnswer = $"{Guid.NewGuid()}.PNG"
            };

            var result = testAnswer.HasValidImageExtension();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasValidImageExtension_FalseTest()
        {
            var testAnswer = new eCOA.DTOLibrary.AnswerDto
            {
                DisplayAnswer = $"{Guid.NewGuid()}.bmp"
            };

            var result = testAnswer.HasValidImageExtension();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasValidImageExtension_NullTest()
        {
            var testAnswer = new eCOA.DTOLibrary.AnswerDto
            {
                DisplayAnswer = null
            };

            var result = testAnswer.HasValidImageExtension();

            Assert.IsFalse(result);
        }
    }
}
