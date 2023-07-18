using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;
using System.Collections.Generic;
using System.Linq;

namespace YPrime.BusinessLayer.UnitTests.Services.QuestionnaireServiceTests
{
    [TestClass]
    public class QuestionnaireServiceGetAllWithPagesTests : QuestionnaireServiceTestBase
    {
        [TestMethod]
        public async Task GetAllWithPagesTest()
        {
            var testQuestionnaireId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<List<QuestionnaireModel>>(AllQuestionnairesResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.PagesEndpoint}/{Config.Defaults.Languages.English.Id}";

            var service = GetService();

            var result = await service.GetAllWithPages();

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
        public async Task GetAllWithPagesEmptyResultTest()
        {
            SetupHttpFactory(System.Net.HttpStatusCode.OK, string.Empty);
       
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.PagesEndpoint}/{Config.Defaults.Languages.English.Id}";

            var service = GetService();

            var result = await service.GetAllWithPages();

            result.Should().BeEmpty();

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
        public async Task GetAllWithPagesWithConfigIdTest()
        {
            var configId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<List<QuestionnaireModel>>(AllQuestionnairesResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{QuestionnaireService.PagesEndpoint}/{Config.Defaults.Languages.English.Id}";

            var service = GetService();

            var result = await service.GetAllWithPages(configId);

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

            var versionIdHeaders = PassedInRequestMessage.Headers.GetValues("VersionId");

            Assert.AreEqual(1, versionIdHeaders.Count());
            Assert.AreEqual(configId.ToString(), versionIdHeaders.First());
        }
    }
}
