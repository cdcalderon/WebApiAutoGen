using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DiaryEntryRepositoryTests
{
    [TestClass]
    public class DiaryEntryRepositoryGetAllPatientDiaryEntriesTests : DiaryEntryRepositoryTestBase
    {
        [TestMethod]
        public async Task GetAllPatientDiaryEntriesTest()
        {
            var repository = GetRepository();

            var results = await repository.GetDiaryEntriesInflated(
                TestQuestionnaire.Id,
                false,
                null,
                TestPatient.Id);

            Assert.AreEqual(1, results.Count);

            var result = results[0];

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(TestQuestionnaire.QuestionnaireTypeId, result.QuestionnaireTypeId);
            Assert.AreEqual(TestVisit.Name, result.VisitName);

            result.Answers.Should().BeEmpty();
            result.DiaryPages.Should().BeEmpty();

            Assert.IsNull(result.Site);

            MockVisitService
                .Verify(s => s.GetAll(It.IsAny<Guid?>()), Times.Once);

            MockQuestionnaireService
                .Verify(s => s.GetAllWithPages(It.IsAny<Guid?>()), Times.Once);

            MockSiteRepository
                .Verify(r => r.GetSite(It.Is<Guid>(id => id == TestPatient.SiteId)), Times.Never);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockAnswerRepository
                .Verify(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)), Times.Never);

            MockDiaryPageRepository
                .Verify(r => r.GetQuestionnaireDiaryPages(It.Is<Guid>(id => id == TestDiaryEntry.QuestionnaireId), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task GetAllPatientDiaryEntriesNullVisitTest()
        {
            MockVisitService.Reset();

            MockVisitService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel>());

            var repository = GetRepository();

            var results = await repository.GetDiaryEntriesInflated(
                TestQuestionnaire.Id,
                false,
                null,
                TestPatient.Id);

            Assert.AreEqual(1, results.Count);

            var result = results[0];

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(TestQuestionnaire.QuestionnaireTypeId, result.QuestionnaireTypeId);
            Assert.IsNull(result.VisitName);

            result.Answers.Should().BeEmpty();
            result.DiaryPages.Should().BeEmpty();

            Assert.IsNull(result.Site);

            MockVisitService
                .Verify(s => s.GetAll(It.IsAny<Guid?>()), Times.Once);

            MockQuestionnaireService
                .Verify(s => s.GetAllWithPages(It.IsAny<Guid?>()), Times.Once);

            MockSiteRepository
                .Verify(r => r.GetSite(It.Is<Guid>(id => id == TestPatient.SiteId)), Times.Never);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockAnswerRepository
                .Verify(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)), Times.Never);

            MockDiaryPageRepository
                .Verify(r => r.GetQuestionnaireDiaryPages(It.Is<Guid>(id => id == TestDiaryEntry.QuestionnaireId), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task GetAllPatientDiaryEntriesWithAnswersTest()
        {
            var repository = GetRepository();

            var results = await repository.GetDiaryEntriesInflated(
                TestQuestionnaire.Id,
                true,
                null,
                TestPatient.Id);

            Assert.AreEqual(1, results.Count);

            var result = results[0];

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(TestQuestionnaire.QuestionnaireTypeId, result.QuestionnaireTypeId);
            Assert.AreEqual(TestVisit.Name, result.VisitName);
            Assert.AreEqual(1, result.Answers.Count);

            var answer = result.Answers[0];

            Assert.AreEqual(TestAnswer.Id, answer.Id);
            Assert.AreEqual(TestSite, result.Site);

            MockVisitService
                .Verify(s => s.GetAll(It.IsAny<Guid?>()), Times.Once);

            MockQuestionnaireService
                .Verify(s => s.GetAllWithPages(It.IsAny<Guid?>()), Times.Once);

            MockSiteRepository
                .Verify(r => r.GetSite(It.Is<Guid>(id => id == TestPatient.SiteId)), Times.Once);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockAnswerRepository
                .Verify(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)), Times.Once);

            MockDiaryPageRepository
                .Verify(r => r.GetQuestionnaireDiaryPages(It.Is<Guid>(id => id == TestDiaryEntry.QuestionnaireId), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAllPatientDiaryEntriesBlindedTest()
        {
            var repository = GetRepository();

            var results = await repository.GetDiaryEntriesInflated(
                TestQuestionnaire.Id,
                false,
                true,
                TestPatient.Id);

            Assert.AreEqual(0, results.Count);

            MockVisitService
                .Verify(s => s.GetAll(It.IsAny<Guid?>()), Times.Once);

            MockQuestionnaireService
                .Verify(s => s.GetAllWithPages(It.IsAny<Guid?>()), Times.Once);

            MockSiteRepository
                .Verify(r => r.GetSite(It.Is<Guid>(id => id == TestPatient.SiteId)), Times.Never);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockAnswerRepository
                .Verify(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)), Times.Never);

            MockDiaryPageRepository
                .Verify(r => r.GetQuestionnaireDiaryPages(It.Is<Guid>(id => id == TestDiaryEntry.QuestionnaireId), It.IsAny<Guid>()), Times.Never);
        }
    }
}
