using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.StudySettingServiceTests
{
    [TestClass]
    public class StudySettingServiceGetGuidValueTests : StudySettingServiceTestBase
    {
        const string StudyIdKey = "StudyID";

        [TestMethod]
        public async Task GetGuidValueTest()
        {
            var singleSetting = AllStudyCustoms
                .First(m => m.Key == StudyIdKey);

            var expectedValue = Guid.Parse(singleSetting.Value);

            SetupHttpFactory(HttpStatusCode.OK, GetAllStudyCustomsResponseContent);

            var service = GetService();

            var result = await service.GetGuidValue(StudyIdKey);

            Assert.AreEqual(expectedValue, result);

            Assert.AreEqual(ExpectedStudyCustomAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
        }
    }
}
