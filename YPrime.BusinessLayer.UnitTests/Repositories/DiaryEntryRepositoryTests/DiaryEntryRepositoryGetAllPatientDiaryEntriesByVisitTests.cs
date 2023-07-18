using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DiaryEntryRepositoryTests
{
    [TestClass]
    public class DiaryEntryRepositoryGetAllPatientDiaryEntriesByVisitTests : DiaryEntryRepositoryTestBase
    {
        [TestMethod]
        public async Task GetAllPatientDiaryEntriesByVisitTest()
        {
            var repository = GetRepository();

            var results = await repository.GetAllPatientDiaryEntriesByVisit(
                TestPatient.Id,
                TestVisit.Id,
                false,
                null,
                "en-US");

            Assert.AreEqual(1, results.Count);

            var result = results[0];

            Assert.AreEqual(TestDiaryEntry.Id, result.Id);
            Assert.AreEqual(TestQuestionnaire.QuestionnaireTypeId, result.QuestionnaireTypeId);
            Assert.AreEqual(TestVisit.Name, result.VisitName);

            result.Answers.Should().BeEmpty();
            result.DiaryPages.Should().BeEmpty();

            Assert.IsNull(result.Site);

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockVisitService
                .Verify(s => s.GetAll(It.IsAny<Guid?>()), Times.Once);

            MockQuestionnaireService
                .Verify(s => s.GetAllWithPages(It.IsAny<Guid?>()), Times.Once);

            MockSiteRepository
                .Verify(r => r.GetSite(It.Is<Guid>(id => id == TestPatient.SiteId)), Times.Never);

            MockAnswerRepository
                .Verify(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)), Times.Never);

            MockDiaryPageRepository
                .Verify(r => r.GetQuestionnaireDiaryPages(It.Is<Guid>(id => id == TestDiaryEntry.QuestionnaireId), It.IsAny<Guid>()), Times.Never);
        }
    }
}
