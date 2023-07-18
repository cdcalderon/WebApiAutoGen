using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.UnitTests.YPrime.Api.StudyControllerTests
{
    public class StudyControllerUpdateStudyPortalConfigDataTests : StudyControllerTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            BaseInitialize();
        }

        [TestMethod]
        public async Task UpdateStudyPortalConfigDataTest()
        {
            var testData = new StudyPortalConfigDataDto();

            MockDataCopyRepository
                .Setup(r => r.UpdateStudyPortalConfigData(It.Is<StudyPortalConfigDataDto>(d => d == testData)));

            var controller = GetController();

            var responseResult = await controller.UpdateStudyPortalConfigData(testData);

            Assert.IsNotNull(responseResult);
            Assert.IsInstanceOfType(responseResult, typeof(OkResult));

            MockDataCopyRepository
                .Verify(r => r.UpdateStudyPortalConfigData(It.Is<StudyPortalConfigDataDto>(d => d == testData)), Times.Once);
        }
    }
}