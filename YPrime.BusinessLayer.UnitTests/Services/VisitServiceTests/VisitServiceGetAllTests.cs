using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.VisitServiceTests
{
    [TestClass]
    public class VisitServiceGetAllTests : VisitServiceTestBase
    {
        [TestMethod]
        public async Task GetAllTest()
        {
            var expectedResult = JsonConvert.DeserializeObject<List<VisitModel>>(GetAllResponse);

            var service = GetService();

            var result = await service.GetAll();

            result.Should().BeEquivalentTo(expectedResult);

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
            Assert.AreEqual(2, result[0].CustomExtensions.Count);
        }
    }
}
