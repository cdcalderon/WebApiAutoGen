using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryGetPaperDiaryEntryQuestionnaireTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task GetPaperDiaryEntryQuestionnaireTest()
        {
            var patientId = Guid.NewGuid();
            var configId = Guid.NewGuid();

            MockPatientRepository
                .Setup(r => r.GetAssignedConfiguration(It.Is<Guid>(pid => pid == patientId)))
                .ReturnsAsync(configId);

            var testQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                PageId = Guid.NewGuid(),
            };

            var testPage = new DiaryPageModel
            {
                Id = testQuestion.PageId,
                Questions = new List<QuestionModel>
                {
                    testQuestion
                }
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Questionnaire",
                Pages = new List<DiaryPageModel>
                {
                    testPage
                }
            };

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == testQuestionnaire.Id),
                    It.Is<Guid?>(lid => lid == null),
                    It.Is<Guid?>(cid => cid == configId)))
                .ReturnsAsync(testQuestionnaire);

            var result = await Repository.GetPaperDiaryEntryQuestionnaire(
                testQuestionnaire.Id,
                patientId);

            Assert.AreEqual(result.Id, testQuestionnaire.Id);
            Assert.AreEqual(result.DisplayName, testQuestionnaire.DisplayName);
            Assert.AreEqual(result.Pages.Count, testQuestionnaire.Pages.Count);
            Assert.AreEqual(result.Questions.Count, testPage.Questions.Count);

            var resultQuestion = result.Questions.First();

            Assert.AreEqual(testQuestion.Id, resultQuestion.Id);
            Assert.AreEqual(testQuestion.PageId, resultQuestion.PageId);
        }

        [TestMethod]
        public async Task GetPaperDiaryEntryQuestionnaireNullResultTest()
        {
            var patientId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var testQuestionnaireId = Guid.NewGuid();

            MockPatientRepository
                .Setup(r => r.GetAssignedConfiguration(It.Is<Guid>(pid => pid == patientId)))
                .ReturnsAsync(configId);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == testQuestionnaireId),
                    It.Is<Guid?>(lid => lid == null),
                    It.Is<Guid?>(cid => cid == configId)))
                .ReturnsAsync((QuestionnaireModel)null);

            var result = await Repository.GetPaperDiaryEntryQuestionnaire(
                testQuestionnaireId,
                patientId);

            Assert.IsNull(result);
        }
    }
}
