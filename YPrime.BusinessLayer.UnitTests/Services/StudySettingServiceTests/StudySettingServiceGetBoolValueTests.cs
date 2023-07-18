using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.StudySettingServiceTests
{
    [TestClass]
    public class StudySettingServiceGetBoolValueTests : StudySettingServiceTestBase
    {
        const string SiteActivationKey = "InitialOnSiteActivation";

        [TestMethod]
        public async Task GetGuidValueTest()
        {
            var singleSetting = AllStudyCustoms
                .First(m => m.Key == SiteActivationKey);

            var expectedValue = bool.Parse(singleSetting.Value);

            SetupHttpFactory(HttpStatusCode.OK, GetAllStudyCustomsResponseContent);

            var service = GetService();

            var result = await service.GetBoolValue(SiteActivationKey);

            Assert.AreEqual(expectedValue, result);

            Assert.AreEqual(ExpectedStudyCustomAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
        }
    }
}
