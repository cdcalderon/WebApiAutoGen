using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.CountryServiceTests
{
    [TestClass]
    public class CountryServiceGetCountriesTests : CountryServiceTestBase
    {
        [TestMethod]
        public async Task CountryService_GetAll_Test()
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

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
        }

        [TestMethod]
        public async Task CountryService_GetAllWithConfigId_Test()
        {
            var testConfigurationId = Guid.NewGuid();

            var service = GetService();

            var results = await service.GetAll(testConfigurationId);

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

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
            Assert.IsTrue(headerValues.Contains(testConfigurationId.ToString()));
        }

        [TestMethod]
        public async Task CountryService_GetSingle_Test()
        {
            var expectedResult = JsonConvert.DeserializeObject<CountryModel>(DefaultSingleResponseContent);
            var expectedSingleAddress = $"{ExpectedHttpAddress}/{ChinaId}";

            SetupHttpFactory(HttpStatusCode.OK, DefaultSingleResponseContent);

            var service = GetService();

            var result = await service.Get(Guid.Parse(ChinaId));

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

            Assert.AreEqual(expectedSingleAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
        }

        [TestMethod]
        public async Task CountryService_GetSingleWithConfigId_Test()
        {
            var testConfigurationId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<CountryModel>(DefaultSingleResponseContent);
            var expectedSingleAddress = $"{ExpectedHttpAddress}/{ChinaId}";

            SetupHttpFactory(HttpStatusCode.OK, DefaultSingleResponseContent);

            var service = GetService();

            var result = await service.Get(Guid.Parse(ChinaId), testConfigurationId);

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

            Assert.AreEqual(expectedSingleAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
            Assert.IsTrue(headerValues.Contains(testConfigurationId.ToString()));
        }

        [TestMethod]
        public async Task CountryService_GetSingleWithWorkingCopyConfigId_Test()
        {
            var testConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id;

            var expectedResult = JsonConvert.DeserializeObject<CountryModel>(DefaultSingleResponseContent);
            var expectedSingleAddress = $"{ExpectedHttpAddress}/{ChinaId}";

            SetupHttpFactory(HttpStatusCode.OK, DefaultSingleResponseContent);

            var service = GetService();

            var result = await service.Get(Guid.Parse(ChinaId), testConfigurationId);

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

            Assert.AreEqual(expectedSingleAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
        }

        [TestMethod]
        public async Task CountryService_GetAll_500Test()
        {
            SetupHttpFactory(
                HttpStatusCode.InternalServerError, 
                DefaultGetAllResponseContent);

            var service = GetService();

            var thrownException = await Assert.ThrowsExceptionAsync<ApiFailureException>(async () => await service.GetAll());
            var expectedException = new ApiFailureException(PassedInRequestMessage.RequestUri.AbsoluteUri, HttpStatusCode.InternalServerError.ToString(), DefaultGetAllResponseContent);

            Assert.AreEqual(thrownException.Message, expectedException.Message);

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

            var containsConfigIdHeader = PassedInRequestMessage.Headers.TryGetValues(ConfigVersionIdHeaderName, out var headerValues);

            Assert.IsTrue(containsConfigIdHeader);
        }

        /*
        [TestMethod]
        public async Task CountryService_GetSingleFromCache_Test()
        {
            var expectedResult = JsonConvert.DeserializeObject<CountryModel>(DefaultSingleResponseContent);
            var expectedSingleAddress = $"{ExpectedHttpAddress}/{ChinaId}";

            SetupHttpFactory(HttpStatusCode.OK, DefaultSingleResponseContent);

            Assert.IsTrue(MemoryCache.Count == 0);
            var service = GetService();

            var result = await service.Get(c => c.Id == Guid.Parse(ChinaId), Guid.Parse(ChinaId));

            result.Should().BeEquivalentTo(expectedResult);

            MemoryCache.TryGetValue("Country", out object data);

            var countries = data as List<CountryModel>;
            Assert.IsTrue(countries.Count == 1);
        }

   
        [TestMethod]
        public async Task CountryService_GetAllFromCache_Test()
        {
            var service = GetService();

            Assert.IsTrue(MemoryCache.Count == 0);

            var results = await service.GetAll();

            MemoryCache.TryGetValue("Country", out object data);

            var countries = data as List<CountryModel>;
            Assert.IsTrue(countries.Count == results.Count);
        }
        */
    }
}
