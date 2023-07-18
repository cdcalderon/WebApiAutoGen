using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.QuestionnaireServiceTests
{
    [TestClass]
    public class QuestionnaireServiceTestsGetInflatedQuestionnaireTests : QuestionnaireServiceTestBase
    {
        [TestMethod]
        public async Task GetInflatedQuestionnaireTest()
        {
            var testQuestionnaireId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<QuestionnaireModel>(QuestionnaireWithDetailsResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.DeepLoadedEndpoint}/{testQuestionnaireId}/{Config.Defaults.Languages.English.Id}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, QuestionnaireWithDetailsResponse);

            var service = GetService();

            var result = await service.GetInflatedQuestionnaire(testQuestionnaireId);

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

        [TestMethod]
        public async Task GetInflatedQuestionnaireSpecificLanguageIdTest()
        {
            var testQuestionnaireId = Guid.NewGuid();
            var testLanguageId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<QuestionnaireModel>(QuestionnaireWithDetailsResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.DeepLoadedEndpoint}/{testQuestionnaireId}/{testLanguageId}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, QuestionnaireWithDetailsResponse);

            var service = GetService();

            var result = await service.GetInflatedQuestionnaire(
                testQuestionnaireId,
                testLanguageId);

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

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
        }

        [TestMethod]
        public async Task GetInflatedQuestionnaireSpecificConfigurationIdTest()
        {
            var testQuestionnaireId = Guid.NewGuid();
            var testConfigurationId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<QuestionnaireModel>(QuestionnaireWithDetailsResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.DeepLoadedEndpoint}/{testQuestionnaireId}/{Config.Defaults.Languages.English.Id}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, QuestionnaireWithDetailsResponse);

            var service = GetService();

            var result = await service.GetInflatedQuestionnaire(testQuestionnaireId, null, configurationId: testConfigurationId);

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

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
            Assert.IsTrue(headerValues.Contains(testConfigurationId.ToString()));
        }
    }
}
