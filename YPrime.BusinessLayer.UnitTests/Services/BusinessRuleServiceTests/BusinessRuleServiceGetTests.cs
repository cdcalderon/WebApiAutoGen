using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests.Services.BusinessRuleServiceTests
{
    [TestClass]
    public class BusinessRuleServiceGetTests : BusinessRuleServiceTestBase
    {
        [TestMethod]
        public async Task GetTest()
        {
            var service = GetService();

            var results = await service.GetAll();

            foreach (var result in results)
            {
                var matchingModel = DefaultResponseModels
                    .FirstOrDefault(dm => dm.Id == result.Id);

                result.Should().BeEquivalentTo(matchingModel);
            }

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

            Assert.AreEqual(ExpectedHttpAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }
    }
}

