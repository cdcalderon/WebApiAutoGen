using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services
{
    public abstract class ConfigServiceTestBase<TMODEL>
    {
        protected readonly string GetAllResponseJsonContent;

        protected List<TMODEL> DefaultResponseModels { get; private set; } = new List<TMODEL>();
        protected readonly IServiceSettings TestServiceSettings;

        protected const string BaseTestHttpAddress = "http://localhost";
        protected const string ConfigVersionIdHeaderName = "VersionId";

        protected HttpRequestMessage PassedInRequestMessage = null;
        protected Mock<DelegatingHandler> MockClientHandler;
        protected Mock<IHttpClientFactory> MockHttpFactory;
        protected Mock<ISessionService> MockSessionService;
        protected Mock<IAuthService> MockAuthService;
        protected Mock<IServiceSettings> MockServiceSettings;
        protected IAuthSettings _authSettings;

        protected MemoryCache MemoryCache { get; set; }
        protected ConfigServiceTestBase(string getAllResponseJsonContent)
        {
            GetAllResponseJsonContent = getAllResponseJsonContent;

            TestServiceSettings = new ServiceSettings
            {
                StudyId = Guid.NewGuid(),
                AuthUrl = "https://www.authgoeshere.com",
                HMACAuthSharedKey = "+IhKNhGUJwFFExAioWY71Dv5gjMTMl7Lef8I0EQUtfw=",
            };
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
            SetupHttpFactory(HttpStatusCode.OK, GetAllResponseJsonContent);
            DefaultResponseModels = JsonConvert.DeserializeObject<List<TMODEL>>(GetAllResponseJsonContent);
            MockSessionService = new Mock<ISessionService>();
            MockAuthService = new Mock<IAuthService>();
            MockServiceSettings = new Mock<IServiceSettings>();

            MockServiceSettings.SetupProperty(ss => ss.SlidingCacheExpirationSeconds);
            MockServiceSettings.Object.SlidingCacheExpirationSeconds = 1296000;
            _authSettings = new AuthSettings
            {
                BaseUrl = "https://www.authgoeshere.com",
                Audience_AAM = "https://www.audience.com",
                ClientId_M2M = "TestClientId",
                ClientSecret_M2M = "TestClientSecret"
            };
        }

        protected void SetupHttpFactory(
            HttpStatusCode httpStatusCode,
            string responseContent)
        {
            var httpResponseMessage = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(responseContent, Encoding.UTF32, "application/json"),
            };

            MockClientHandler = new Mock<DelegatingHandler>();

            MockClientHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    nameof(HttpClient.SendAsync),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage)
                .Callback((HttpRequestMessage requestMessage, CancellationToken token) =>
                {
                    PassedInRequestMessage = requestMessage;
                })
                .Verifiable();

            MockClientHandler
                .As<IDisposable>()
                .Setup(ch => ch.Dispose());

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
