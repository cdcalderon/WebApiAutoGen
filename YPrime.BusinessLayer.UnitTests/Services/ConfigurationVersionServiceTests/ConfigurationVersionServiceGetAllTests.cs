using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.ConfigurationVersionServiceTests
{
    [TestClass]
    public class ConfigurationVersionServiceGetAllTests : ConfigurationVersionServiceTestBase
    {
        private const string ExpectedVersionOneDisplayVersion = "1.0-01.00";

        [TestMethod]
        public async Task GetAllTest()
        {
            var expectedResult = JsonConvert.DeserializeObject<List<ConfigurationVersion>>(GetAllResponse);

            var service = GetService();

            var result = await service.GetAll();

            result.Should().BeEquivalentTo(expectedResult);

            var firstResult = result.First(r => r.Id == VersionOneId);

            Assert.AreEqual(ExpectedVersionOneDisplayVersion, firstResult.DisplayVersion);

            MockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            MockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(ExpectedBaseEndpointAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }
    }
}
