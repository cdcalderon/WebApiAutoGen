using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGeneratePatientNumberTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task GeneratePatientNumberTest()
        {
            const string testPatientNumber = "555";
            const string expectedPrefix = "S-100-";

            var result = await Repository.GeneratePatientNumber(BaseSite.Id, testPatientNumber);

            Assert.AreEqual($"{expectedPrefix}{testPatientNumber}", result);

            MockStudySettingService.Verify(
                s => s.GetAllStudyCustoms(It.Is<Guid?>(cid => cid == null)),
                Times.Once);
        }

        [TestMethod]
        public async Task GeneratePatientNumberWithConfigTest()
        {
            var testConfigId = Guid.NewGuid();

            const string testPatientNumber = "555";
            const string expectedPrefix = "S-100-";

            var result = await Repository.GeneratePatientNumber(BaseSite.Id, testPatientNumber, testConfigId);

            Assert.AreEqual($"{expectedPrefix}{testPatientNumber}", result);

            MockStudySettingService.Verify(
                s => s.GetAllStudyCustoms(It.Is<Guid?>(cid => cid == testConfigId)),
                Times.Once);
        }
    }
}
