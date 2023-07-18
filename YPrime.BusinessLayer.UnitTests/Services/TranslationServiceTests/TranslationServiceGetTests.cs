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

namespace YPrime.BusinessLayer.UnitTests.Services.TranslationServiceTests
{
    [TestClass]
    public class TranslationServiceGetTests : TranslationServiceTestBase
    {
        [TestMethod]
        public async Task GetTest()
        {
            const string loginResourceKey = "Login";

            var expectedResult = JsonConvert.DeserializeObject<List<TranslationModel>>(SingleEnglishTranslationResponse).First();
            var expectedAddress = $"{ExpectedHttpAddress}?resourceKey={loginResourceKey}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, SingleEnglishTranslationResponse);

            var service = GetService();

            var result = await service.Get(loginResourceKey);

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

            Assert.AreEqual(expectedAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }
    }
}
