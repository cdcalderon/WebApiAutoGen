using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.UnitTests.Repositories.AnswerRepositoryTests
{
    [TestClass]
    public class AnswerRepositoryGetAnswersTests : AnswerRepositoryTestBase
    {
        public static Guid DiaryEntryId = Guid.NewGuid();
        public static Guid QuestionnaireId = Guid.NewGuid();

        public static Guid AnswerOneId = Guid.NewGuid();
        public static Guid AnswerTwoId = Guid.NewGuid();
        public static Guid AnswerThreeId = Guid.NewGuid();

        public static Guid ChoiceOneId = Guid.NewGuid();
        public static Guid ChoiceTwoId = Guid.NewGuid();
        public static Guid ChoiceThreeId = Guid.NewGuid();

        public static Guid QuestionOneId = Guid.NewGuid();
        public static Guid QuestionTwoId = Guid.NewGuid();
        public static Guid QuestionThreeId = Guid.NewGuid();

        public static Guid DiaryPageOneId = Guid.NewGuid();

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForDiaryEntryTest()
        {
            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();
            var choiceList = new List<QuestionChoiceModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.Checkbox.Id
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                PageId = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
                QuestionType = InputFieldType.TextArea.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var choice1 = new QuestionChoiceModel
            {
                Id = ChoiceOneId,
                Sequence = 1,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };

            var choice2 = new QuestionChoiceModel
            {
                Id = ChoiceTwoId,
                Sequence = 2,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };
            var choice3 = new QuestionChoiceModel
            {
                Id = ChoiceThreeId,
                Sequence = 3,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };

            choiceList.Add(choice1);
            choiceList.Add(choice2);
            choiceList.Add(choice3);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid()
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                ChoiceId = ChoiceOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                ChoiceId = null,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 2"
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                ChoiceId = null,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                choiceList,
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }


        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForTemperature_FahrenheitParsingTest()
        {
            var answerOneFahrenheitValue = "105.0F";
            var answerOneFahrenheitNumericValue = "105.0";
            var answerTwoFahrenheitValue = "97.8F";
            var answerTwoFahrenheitNumericValue = "97.8";

            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TemperatureSpinner.Id,
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                PageId = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
                QuestionType = InputFieldType.TemperatureSpinner.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid(),
                Patient = new Patient() { SiteId = Guid.Empty, Site = new Site() { CountryId = Guid.Empty } }
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerOneFahrenheitValue
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerTwoFahrenheitValue
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                new List<QuestionChoiceModel>(),
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(2, result.Count(i => i.Suffix == Temperature.DegreesFahrenheit));
            Assert.AreEqual(answerOneFahrenheitNumericValue, result[0].DisplayAnswer);
            Assert.AreEqual(answerTwoFahrenheitNumericValue, result[1].DisplayAnswer);
            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForTemperature_FahrenheitConversionTest()
        {
            var answerOneFahrenheitValue = "105.0F";
            var answerOneCelsiusNumericValue = "40.6";
            var answerTwoFahrenheitValue = "97.8F";
            var answerTwoCelsiusNumericValue = "36.6";

            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TemperatureSpinner.Id,
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                PageId = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
                QuestionType = InputFieldType.TemperatureSpinner.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid(),
                Patient = new Patient() { SiteId = Guid.Empty, Site = new Site() { CountryId = Guid.Empty } }
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerOneFahrenheitValue
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerTwoFahrenheitValue
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                new List<QuestionChoiceModel>(),
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = true
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(2, result.Count(i => i.Suffix == Temperature.DegreesCelsius));
            Assert.AreEqual(answerOneCelsiusNumericValue, result[0].DisplayAnswer);
            Assert.AreEqual(answerTwoCelsiusNumericValue, result[1].DisplayAnswer);
            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }
        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForTemperature_CelsiusParsingTest()
        {
            var answerOneValue = "36.5C";
            var answerOneNumericValue = "36.5";
            var answerTwoValue = "45.0C";
            var answerTwoNumericValue = "45.0";

            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TemperatureSpinner.Id,
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                PageId = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
                QuestionType = InputFieldType.TemperatureSpinner.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var testCountryGuid = Guid.NewGuid();
            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid(),
                Patient = new Patient() { SiteId = Guid.Empty, Site = new Site() { CountryId = testCountryGuid } }
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerOneValue.ToString()
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerTwoValue.ToString()
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                new List<QuestionChoiceModel>(),
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = true
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(2, result.Count(i => i.Suffix == Temperature.DegreesCelsius));
            Assert.AreEqual(answerOneNumericValue, result[0].DisplayAnswer);
            Assert.AreEqual(answerTwoNumericValue, result[1].DisplayAnswer);
            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }
        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForTemperature_CelsiusConversionTest()
        {
            var answerOneValue = "36.5C";
            var answerOneNumericValue = "97.7";
            var answerTwoValue = "45.0C";
            var answerTwoNumericValue = "113.0";

            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TemperatureSpinner.Id,
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                PageId = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
                QuestionType = InputFieldType.TemperatureSpinner.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var testCountryGuid = Guid.NewGuid();
            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid(),
                Patient = new Patient() { SiteId = Guid.Empty, Site = new Site() { CountryId = testCountryGuid } }
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerOneValue.ToString()
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = answerTwoValue.ToString()
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                new List<QuestionChoiceModel>(),
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(2, result.Count(i => i.Suffix == Temperature.DegreesFahrenheit));
            Assert.AreEqual(answerOneNumericValue, result[0].DisplayAnswer);
            Assert.AreEqual(answerTwoNumericValue, result[1].DisplayAnswer);
            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersForTemperature_NotRequiredTest()
        {
            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TemperatureSpinner.Id,
            };

            questionList.Add(question1);

            var testCountryGuid = Guid.NewGuid();
            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = Guid.NewGuid(),
                Patient = new Patient() { SiteId = Guid.Empty, Site = new Site() { CountryId = testCountryGuid } }
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = TemperatureControlConstants.NotRequiredAnswer
            };

            answerList.Add(answer1);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                new List<QuestionChoiceModel>(),
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var result = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(1, result.Count(i => i.Suffix == null));
            Assert.AreEqual(TemperatureControlConstants.NotRequiredAnswer, result[0].DisplayAnswer);
            Assert.IsNotNull(result);
            Assert.AreEqual(answerList.Count, result.Count);

            MockQuestionnaireService
                .Verify(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid?>(cid => cid == diaryEntry.ConfigurationId)), Times.Once);
        }

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersOrderedByQuestionSequenceTests()
        {
            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();
            var choiceList = new List<QuestionChoiceModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                Sequence = 1,
                QuestionType = InputFieldType.Checkbox.Id
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                Sequence = 2,
                QuestionType = InputFieldType.TextArea.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                Sequence = 3,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var choice1 = new QuestionChoiceModel
            {
                Id = ChoiceOneId,
                Sequence = 1,
                QuestionId = QuestionOneId,
                DisplayText = "choiceTranslationKey",
                ChoiceFloatValue = 1.1f
            };

            var choice2 = new QuestionChoiceModel
            {
                Id = ChoiceTwoId,
                Sequence = 2,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };
            var choice3 = new QuestionChoiceModel
            {
                Id = ChoiceThreeId,
                Sequence = 3,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };

            choiceList.Add(choice1);
            choiceList.Add(choice2);
            choiceList.Add(choice3);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                ChoiceId = ChoiceOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                ChoiceId = null,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 2"
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                ChoiceId = null,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                choiceList,
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var results = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(results[0].QuestionId, QuestionOneId);
            Assert.AreEqual(results[1].QuestionId, QuestionTwoId);
            Assert.AreEqual(results[2].QuestionId, QuestionThreeId);

        }

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersDisplayAnswerTests()
        {
            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();
            var choiceList = new List<QuestionChoiceModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.Checkbox.Id
            };

            var question2 = new QuestionModel
            {
                Id = QuestionTwoId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            var question3 = new QuestionModel
            {
                Id = QuestionThreeId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);
            questionList.Add(question2);
            questionList.Add(question3);

            var choice1 = new QuestionChoiceModel
            {
                Id = ChoiceOneId,
                Sequence = 1,
                QuestionId = QuestionOneId,
                DisplayText = "choiceTranslationKey",
                ChoiceFloatValue = 1.1f
            };

            var choice2 = new QuestionChoiceModel
            {
                Id = ChoiceTwoId,
                Sequence = 2,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };
            var choice3 = new QuestionChoiceModel
            {
                Id = ChoiceThreeId,
                Sequence = 3,
                QuestionId = QuestionOneId,
                ChoiceFloatValue = 1.1f
            };

            choiceList.Add(choice1);
            choiceList.Add(choice2);
            choiceList.Add(choice3);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
            };

            var answer1 = new Answer
            {
                Id = AnswerOneId,
                ChoiceId = ChoiceOneId,
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
            };

            var answer2 = new Answer
            {
                Id = AnswerTwoId,
                ChoiceId = null,
                QuestionId = QuestionTwoId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 2"
            };

            var answer3 = new Answer
            {
                Id = AnswerThreeId,
                ChoiceId = null,
                QuestionId = QuestionThreeId,
                DiaryEntryId = DiaryEntryId,
                FreeTextAnswer = "test answer 3"
            };

            answerList.Add(answer1);
            answerList.Add(answer2);
            answerList.Add(answer3);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                choiceList,
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var results = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(results[0].DisplayAnswer, choice1.DisplayText);
            Assert.AreEqual(results[1].DisplayAnswer, answer2.FreeTextAnswer);
            Assert.AreEqual(results[2].DisplayAnswer, answer3.FreeTextAnswer);
        }

        [TestMethod]
        public async Task AnswerRepositoryGetAnswersArchivedTests()
        {
            var archivedAnswerId = Guid.NewGuid();
            var currentAnswerId = Guid.NewGuid();

            var answerList = new List<Answer>();
            var questionList = new List<QuestionModel>();
            var choiceList = new List<QuestionChoiceModel>();

            var diaryPage = new DiaryPageModel
            {
                Id = DiaryPageOneId,
                QuestionnaireId = QuestionnaireId,
            };

            var question1 = new QuestionModel
            {
                Id = QuestionOneId,
                QuestionnaireId = QuestionnaireId,
                PageId = DiaryPageOneId,
                Sequence = 1,
                QuestionType = InputFieldType.TextArea.Id
            };

            questionList.Add(question1);

            var diaryEntry = new DiaryEntry
            {
                Id = DiaryEntryId,
                QuestionnaireId = QuestionnaireId,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
            };

            var answer1 = new Answer
            {
                Id = archivedAnswerId,
                FreeTextAnswer = "Prior answer",
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                IsArchived = true
            };

            var answer2 = new Answer
            {
                Id = currentAnswerId,
                FreeTextAnswer = "Current answer",
                QuestionId = QuestionOneId,
                DiaryEntryId = DiaryEntryId,
                IsArchived = false
            };

            answerList.Add(answer1);
            answerList.Add(answer2);

            SetupData(
                answerList,
                new List<DiaryEntry> { diaryEntry },
                choiceList,
                questionList,
                new List<DiaryPageModel> { diaryPage });

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = DiaryEntryId,
                UseMetricForAnswers = false
            };

            var results = await Repository.GetAnswers(answerProperties);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(answer2.Id, results[0].Id);
        }
    }
}
