using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.StudySettingServiceTests
{
    [TestClass]
    public class StudySettingServiceGetStringValueTests : StudySettingServiceTestBase
    {
        const string PatientPrefixKey = "PatientNumberPrefix";

        [TestMethod]
        public async Task GetStringValueTest()
        {
            var singleSetting = AllStudyCustoms
                .First(m => m.Key == PatientPrefixKey);

            var expectedValue = singleSetting.Value;

            SetupHttpFactory(HttpStatusCode.OK, GetAllStudyCustomsResponseContent);

            var service = GetService();

            var result = await service.GetStringValue(PatientPrefixKey);

            Assert.AreEqual(expectedValue, result);

            Assert.AreEqual(ExpectedStudyCustomAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
        }
    }
}
