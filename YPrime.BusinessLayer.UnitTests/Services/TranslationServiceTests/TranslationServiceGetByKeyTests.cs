using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Config.Defaults;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.TranslationServiceTests
{
    [TestClass]
    public class TranslationServiceGetByKeyTests : TranslationServiceTestBase
    {
        [TestMethod]
        public async Task GetByKeyTest()
        {
            const string loginResourceKey = "Login";

            var expectedResult = JsonConvert.DeserializeObject<List<TranslationModel>>(SingleEnglishTranslationResponse).First();
            var expectedAddress = $"{ExpectedHttpAddress}?resourceKey={loginResourceKey}&LanguageId={Languages.English.Id}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, SingleEnglishTranslationResponse);

            var service = GetService();

            var result = await service.GetByKey(loginResourceKey);

            result.Should().BeEquivalentTo(expectedResult.LocalText);

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

        /*
        [TestMethod]
        public async Task GetBySourceCacheTest()
        {
            const string loginResourceKey = "Login";

            var expectedResult = JsonConvert.DeserializeObject<List<TranslationModel>>(SingleEnglishTranslationResponse).First();
            var expectedAddress = $"{ExpectedHttpAddress}?resourceKey={loginResourceKey}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, SingleEnglishTranslationResponse);

            Assert.IsTrue(MemoryCache.Count == 0);

            var service = GetService();

            var result = await service.GetByKey(loginResourceKey);

            result.Should().BeEquivalentTo(expectedResult.LocalText);

            Assert.IsTrue(MemoryCache.Count == 1);

            var secondResult = await service.GetByKey(loginResourceKey);

            secondResult.Should().BeEquivalentTo(expectedResult.LocalText);

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
        */

        [TestMethod]
        public async Task GetByKeyNestedTranslationTest()
        {
            const string subjectResourceKey = "lblSubject";
            const string dependentResourceKey = "lblDependent";
            const string expectedResult = "Subject goes here";

            var subjectResource = new TranslationModel
            {
                Id = subjectResourceKey,
                LocalText = "Subject"
            };

            var dependentResource = new TranslationModel
            {
                Id = dependentResourceKey,
                LocalText = "{{Translation:lblSubject}} goes here"
            };

            var subjectJson = JsonConvert.SerializeObject(new List<TranslationModel> { subjectResource } );
            var dependentJson = JsonConvert.SerializeObject(new List<TranslationModel> { dependentResource });

            MockClientHandler = new Mock<DelegatingHandler>();

            MockClientHandler
                .As<IDisposable>()
                .Setup(ch => ch.Dispose());

            SetupClientHandler(
                dependentResourceKey,
                dependentJson);

            SetupClientHandler(
                subjectResourceKey,
                subjectJson);

            SetupSingleUseFactory();

            var service = GetService();

            var result = await service.GetByKey(dependentResourceKey);

            Assert.AreEqual(expectedResult, result);

            MockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Exactly(2));

            MockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(2),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        private void SetupClientHandler(string pathQualifier, string responseContent)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF32, "application/json"),
            };

            MockClientHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    nameof(HttpClient.SendAsync),
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.PathAndQuery.Contains(pathQualifier)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
        }

        private void SetupSingleUseFactory()
        {
            var httpClient = new HttpClient(MockClientHandler.Object);
            httpClient.BaseAddress = new Uri(BaseTestHttpAddress);

            MockHttpFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

            MockHttpFactory
                .Setup(r => r.CreateClient(It.IsAny<string>()))
                .Returns(httpClient)
                .Verifiable();
        }
    }
}
