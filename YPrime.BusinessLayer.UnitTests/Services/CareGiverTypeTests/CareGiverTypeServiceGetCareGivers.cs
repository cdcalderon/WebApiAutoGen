using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.CareGiverTypeTests
{
    [TestClass]
    public class CareGiverTypeServiceGetCareGivers : CareGiverTypeServiceTestBase
    {
        [TestMethod]
        public async Task CareGiverTypeService_GetAll_Test()
        {
            var service = GetService();

            var results = await service.GetAll();

            foreach (var result in results)
            {
                var matchingModel = DefaultResponseModels
                    .FirstOrDefault(dm => dm.Id == result.Id);

                result.Should().BeEquivalentTo(matchingModel);
            };

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

        [TestMethod]
        public async Task CareGiverTypeService_GetAll_500Test()
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
        }

        [TestMethod]
        public async Task CareGiverTypeService_GetAllAlphabetical()
        {
            var results = await GetService()
                .GetAllAlphabetical();

            for (var i = 0; i < results.Count; i++)
            {
                CareGiverTypeModel matchingModel;

                var currentCareGiverType = results[i];
                AlphabeticalCareGiverTypeModels.TryGetValue(i, out matchingModel);

                currentCareGiverType.Should().BeEquivalentTo(matchingModel);
            };
        }

        /*
        [TestMethod]
        public async Task CareGiverTypeService_GetAllFromCache_Test()
        {
            var service = GetService();

            Assert.IsTrue(MemoryCache.Count == 0);

            var results = await service.GetAll();

            MemoryCache.TryGetValue("CareGiverType", out object data);

            var careGiverTypes = data as List<CareGiverTypeModel>;
            Assert.IsTrue(careGiverTypes.Count == results.Count);
        }
        */
    }
}