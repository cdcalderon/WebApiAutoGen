using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Exceptions;

namespace YPrime.BusinessLayer.UnitTests.Services.ConfigurationVersionServiceTests
{
    [TestClass]
    public class ConfigurationVersionServiceGetLatestTests : ConfigurationVersionServiceTestBase
    {
        private const string ConfigServiceResponse = "[{\"id\":\"b20b304c-14f1-4df4-9195-20de7653c047\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"01.00\",\"configurationVersionNumber\":\"1.0\",\"dateCreated\":\"2020-09-15T12:48:49.325Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":true,\"description\":\"Initial file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}},{\"id\":\"975ba44f-8980-4829-bfc4-f7d31cc6c1f0\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"02.00\",\"configurationVersionNumber\":\"2.0\",\"dateCreated\":\"2020-09-15T12:48:58.394Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":false,\"description\":\"Second file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}}]";
        private const string ConfigServiceNoProdConfigResponse = "[{\"id\":\"b20b304c-14f1-4df4-9195-20de7653c047\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"01.00\",\"configurationVersionNumber\":\"1.0\",\"dateCreated\":\"2020-09-15T12:48:49.325Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":false,\"description\":\"Initial file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}},{\"id\":\"975ba44f-8980-4829-bfc4-f7d31cc6c1f0\",\"studyId\":\"265326f4-e285-4117-a8ba-915d5373bd1f\",\"srdVersion\":\"02.00\",\"configurationVersionNumber\":\"2.0\",\"dateCreated\":\"2020-09-15T12:48:58.394Z\",\"createdBy\":\"ypadmin@yprime.com\",\"approvedForProd\":false,\"description\":\"Second file version\",\"actionPanel\":{\"canDelete\":true,\"canUpdate\":true,\"acknowledge\":false}}]";

        [TestMethod]
        public async Task GetLatestShouldReturnLatestApprovedForProd()
        {
            MockServiceSettings
                .Setup(ss => ss.StudyPortalAppEnvironment)
                .Returns("PRODUCTION");

            SetupHttpFactory(HttpStatusCode.OK, ConfigServiceResponse);

            var expectedResult = Guid.Parse("b20b304c-14f1-4df4-9195-20de7653c047");

            var service = GetService();

            var result = await service.GetLatest();

            Assert.AreEqual(expectedResult, result);

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

            Assert.AreEqual(ExpectedBaseEndpointAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }

        [TestMethod]
        public async Task GetLatestShouldReturnLatestCreated()
        {
            MockServiceSettings
                .Setup(ss => ss.StudyPortalAppEnvironment)
                .Returns("DEV");

            SetupHttpFactory(HttpStatusCode.OK, ConfigServiceResponse);

            var expectedResult = Guid.Parse("975ba44f-8980-4829-bfc4-f7d31cc6c1f0");

            var service = GetService();

            var result = await service.GetLatest();

            Assert.AreEqual(expectedResult, result);

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

            Assert.AreEqual(ExpectedBaseEndpointAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }

        [TestMethod]
        public async Task GetLatestShouldReturnWorkingCopyIfNoConfiguration()
        {
            MockServiceSettings
                .Setup(ss => ss.StudyPortalAppEnvironment)
                .Returns("DEV");

            const string response = "[]";
            SetupHttpFactory(HttpStatusCode.OK, response);

            var service = GetService();

            var result = await service.GetLatest();

            Assert.AreEqual(Config.Defaults.ConfigurationVersions.InitialVersion.Id, result);

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

            Assert.AreEqual(ExpectedBaseEndpointAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }

        [TestMethod]
        public void GetLatestShouldThrowExceptionInProd()
        {
            MockServiceSettings
                .Setup(ss => ss.StudyPortalAppEnvironment)
                .Returns("PRODUCTION");

            SetupHttpFactory(HttpStatusCode.OK, ConfigServiceNoProdConfigResponse);

            var service = GetService();

            service
                .Invoking(async s => await s.GetLatest())
                .Should()
                .Throw<NoProductionConfigurationException>();

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

            Assert.AreEqual(ExpectedBaseEndpointAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }
    }
}
