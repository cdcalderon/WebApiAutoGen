using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.QuestionnaireServiceTests
{
    [TestClass]
    public class QuestionnaireServiceGetQuestionsTests : QuestionnaireServiceTestBase
    {
        [TestMethod]
        public async Task GetQuestionsTest()
        {
            var testQuestionnaireId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<List<QuestionModel>>(QuestionsForQuestionnaireResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{testQuestionnaireId}/{QuestionnaireService.QuestionEndpoint}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, QuestionsForQuestionnaireResponse);

            var service = GetService();

            var result = await service.GetQuestions(testQuestionnaireId);

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
