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

namespace YPrime.BusinessLayer.UnitTests.Services.TranslationServiceTests
{
    [TestClass]
    public class TranslationSerivceGetByLanguageTests : TranslationServiceTestBase
    {
        [TestMethod]
        public async Task GetByLanguageTest()
        {
            var testLanguageId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<List<TranslationModel>>(EnglishLanguageResponse);
            var expectedAddress = $"{ExpectedHttpAddress}?{nameof(TranslationModel.LanguageId)}={testLanguageId}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, EnglishLanguageResponse);

            var service = GetService();

            var result = await service.GetByLanguage(testLanguageId);

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
