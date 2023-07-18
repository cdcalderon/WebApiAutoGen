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
    public class TranslationServiceLoadIntoCacheTests : TranslationServiceTestBase
    {
        [TestMethod]
        public async Task LoadIntoCacheTest()
        {
            const string portalSource = "Portal";

            var configId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var expectedResult = JsonConvert.DeserializeObject<List<TranslationModel>>(PortalSourceResponse);
            var expectedAddress = $"{ExpectedHttpAddress}?{nameof(TranslationModel.Source)}={portalSource}&{nameof(TranslationModel.LanguageId)}={languageId}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, PortalSourceResponse);

            var service = GetService();

            await service.LoadIntoCache(portalSource, configId, languageId);

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
