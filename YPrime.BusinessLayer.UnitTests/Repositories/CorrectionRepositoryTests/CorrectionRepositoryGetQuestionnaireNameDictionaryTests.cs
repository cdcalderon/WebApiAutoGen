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
    public class CorrectionRepositoryGetQuestionnaireNameDictionaryTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task GetQuestionnaireNameDictionaryTest()
        {
            var patientId = Guid.NewGuid();
            var configId = Guid.NewGuid();

            MockPatientRepository
                .Setup(r => r.GetAssignedConfiguration(It.Is<Guid>(pid => pid == patientId)))
                .ReturnsAsync(configId);

            var firstQuestionnaireName = "Questionnaire 1";
            var secondQuestionnaireName = "Questionnaire 2";

            var firstQuestionnaireId = Guid.NewGuid();
            var secondQuestionnaireId = Guid.NewGuid();

            var questionnaires = new List<QuestionnaireModel>
            {
                new QuestionnaireModel
                {
                    Id = firstQuestionnaireId,
                    DisplayName = firstQuestionnaireName
                },
                new QuestionnaireModel
                {
                    Id = secondQuestionnaireId,
                    DisplayName = secondQuestionnaireName
                }
            };

            MockQuestionnaireService
                .Setup(s => s.GetAllWithPages(It.Is<Guid?>(cid => cid == configId)))
                .ReturnsAsync(questionnaires);

            var result = await Repository.GetQuestionnaireNameDictionary(patientId);

            Assert.AreEqual(questionnaires.Count, result.Count);

            var firstResult = result.First(r => r.Key == firstQuestionnaireId.ToString());
            var secondResult = result.First(r => r.Key == secondQuestionnaireId.ToString());

            Assert.AreEqual(firstQuestionnaireName, firstResult.Value);
            Assert.AreEqual(secondQuestionnaireName, secondResult.Value);
        }
    }
}
