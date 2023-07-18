using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.ConfigurationVersionServiceTests
{
    [TestClass]
    public class ConfigurationVersionServiceGetTests : ConfigurationVersionServiceTestBase
    {
        private const string ExpectedConfigurationVersion = "1.0";
        [TestMethod]
        public async Task GetTest()
        {
            var expectedResult = JsonConvert.DeserializeObject<List<ConfigurationVersion>>(GetAllResponse).First();

            var service = GetService();

            var result = await service.Get(Guid.Parse("b20b304c-14f1-4df4-9195-20de7653c047"), null);

            result.Should().BeEquivalentTo(expectedResult);

            Assert.AreEqual(ExpectedConfigurationVersion, result.ConfigurationVersionNumber);

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
