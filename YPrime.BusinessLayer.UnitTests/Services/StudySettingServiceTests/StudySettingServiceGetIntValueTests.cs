using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.StudySettingServiceTests
{
    [TestClass]
    public class StudySettingServiceGetIntValueTests : StudySettingServiceTestBase
    {
        const string PatientPinLengthKey = "PatientPINLength";

        [TestMethod]
        public async Task GetIntValueTest()
        {
            var singleSetting = AllStudyCustoms
                .First(m => m.Key == PatientPinLengthKey);

            var expectedValue = int.Parse(singleSetting.Value);

            SetupHttpFactory(HttpStatusCode.OK, GetAllStudyCustomsResponseContent);

            var service = GetService();

            var result = await service.GetIntValue(PatientPinLengthKey);

            Assert.AreEqual(expectedValue, result);

            Assert.AreEqual(ExpectedStudyCustomAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
        }
    }
}
