using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.StudySettingServiceTests
{
    [TestClass]
    public class StudySettingServiceGetAllStudyCustomsTests : StudySettingServiceTestBase
    {
        [TestMethod]
        public async Task GetAllStudyCustoms()
        {
            SetupHttpFactory(HttpStatusCode.OK, GetAllStudyCustomsResponseContent);

            var service = GetService();

            var result = await service.GetAllStudyCustoms();

            result.Should().BeEquivalentTo(AllStudyCustoms);

            Assert.AreEqual(ExpectedStudyCustomAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
        }
    }
}
