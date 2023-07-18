using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryGetCorrectionConfigurationIdTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task GetPaperDcfConfigurationIdTest()
        {
            var testPatientId = Guid.NewGuid();
            var testConfigurationId = Guid.NewGuid();

            MockPatientRepository
                .Setup(r => r.GetAssignedConfiguration(It.Is<Guid>(pid => pid == testPatientId)))
                .ReturnsAsync(testConfigurationId);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                PatientId = testPatientId
            };

            var result = await Repository.GetCorrectionConfigurationId(testCorrection);

            Assert.AreEqual(testConfigurationId, result);
        }

        [TestMethod]
        public async Task GetChangeResponseDcfConfigurationIdTest()
        {
            var testDiaryEntryId = Guid.NewGuid();
            var testConfigurationId = Guid.NewGuid();

            var testDiaryEntry = new DiaryEntry
            {
                Id = testDiaryEntryId,
                ConfigurationId = testConfigurationId
            };

            var diaryEntryDataset = new FakeDbSet<DiaryEntry>(new List<DiaryEntry> { testDiaryEntry });

            diaryEntryDataset
                .Setup(ds => ds.FindAsync(It.Is<object[]>(parameters => parameters.Count() == 1 && (Guid)parameters.First() == testDiaryEntryId)))
                .ReturnsAsync(testDiaryEntry);

            MockContext
                .Setup(c => c.DiaryEntries)
                .Returns(diaryEntryDataset.Object);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                DiaryEntryId = testDiaryEntryId
            };

            var result = await Repository.GetCorrectionConfigurationId(testCorrection);

            Assert.AreEqual(testConfigurationId, result);
        }

        [TestMethod]
        public async Task GetUserConfigIdConfigurationIdTest()
        {
            var userConfigId = Guid.NewGuid();
            MockSessionService.Setup(m => m.UserConfigurationId).Returns(userConfigId);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var result = await Repository.GetCorrectionConfigurationId(testCorrection);

            Assert.AreEqual(userConfigId, result);
        }
    }
}
