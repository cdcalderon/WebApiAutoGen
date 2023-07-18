using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Azure.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        private static readonly Guid _id = Guid.Parse("3aadd299-5b3d-4e28-962b-a9ec5530b3e7");
        private static readonly Guid _participantId = Guid.Parse("5fade599-543d-4e28-962b-a9ec5530b3e7");
        private static readonly Guid _ecoaRecordId = Guid.Parse("6dadf299-5b6d-4e28-962b-a9ec5530b3e7");
        private static readonly Guid _studyId = Guid.Parse("539eadf2-2c4b-45d7-a6fd-ff9a31948312");
        private static readonly Guid _sponsorId = Guid.Parse("4772fd9e-0030-4bdb-9926-2132b3ada98f");
        private static readonly string _userId = "auth0|63890e9742952c4831e38fc6";
        private static readonly string _pin = "1234";
        private static readonly string _updatedPin = "4321";

        private static readonly string _sendEmail = "/v1/email/send";
        private static readonly string _subjects = "/v1/subjects";
        private static readonly string _updateECOALink = "api/e-consent/v1/participants/update-ecoa-link/";
        private static readonly string _emailRequest = "{\"from\":\"test@test.com\",\"to\":[],\"cc\":[],\"bcc\":[],\"subject\":\"test\",\"body\":\"test\",\"attachments\":{},\"studyId\":\"539eadf2-2c4b-45d7-a6fd-ff9a31948312\",\"sponsorId\":\"4772fd9e-0030-4bdb-9926-2132b3ada98f\",\"environment\":\"DEV\"}";
        private static readonly string _authSignupRequest = "{\"email\":\"3aadd299-5b3d-4e28-962b-a9ec5530b3e7@yprimesubject.com\",\"password\":\"1234\"}";
        private static readonly string _authSignupResponse = "{\"email\":\"3aadd299-5b3d-4e28-962b-a9ec5530b3e7@yprimesubject.com\",\"userId\":\"auth0|636ac0f70a642613aabf0599\"}";

        private static readonly string _changePassword = _subjects + "/{0}/change-password";
        private static readonly string _authChangePasswordRequest = "{\"password\":\"4321\"}";
        private static readonly string keyVaultKey = AuthConstants.eConsentAPICredentialsSecretKey;
        private const string keyVaultResponse = "{\"ClientId\":\"testId\",\"ClientSecret\":\"testSecret\"}";
        private static readonly SendingEmailModel emailModel = new SendingEmailModel()
        {
            Id = Guid.NewGuid(),
            To = new List<string>(),
            Cc = new List<string>(),
            Bcc = new List<string>(),
            ToUsers = new List<Guid>(),
            CcUsers = new List<Guid>(),
            BccUsers = new List<Guid>(),
            From = "test@test.com",
            Subject = "test",
            Body = "test",
            CreatedDate = DateTime.Now,
            Attachments = new Dictionary<string, byte[]>(),
            StudyId = _studyId,
            SponsorId = _sponsorId,
            Environment = "DEV"
        };
        private HttpRequestMessage _passedInRequestMessage = null;
        private string _passedInRequestJson = null;

        private IAuthSettings _authSettings;

        private Mock<DelegatingHandler> _mockClientHandler;
        private Mock<IHttpClientFactory> _mockHttpFactory;
        private Mock<IAuthenticationApiClient> _mockAuthenticationApiClient;
        private Mock<IKeyVaultService> _mockKeyVaultService;
        private IServiceSettings _serviceSettings;
        private IMemoryCache _memoryCache;

        private AuthService _authService;

        [TestInitialize]
        public void Initialize()
        {
            _authSettings = new AuthSettings
            {
                BaseUrl = "https://www.authgoeshere.com",
                Audience_AAM = "https://www.audience.com",
                ClientId_M2M = "TestClientId",
                ClientSecret_M2M = "TestClientSecret",
            };

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            _memoryCache = serviceProvider.GetService<IMemoryCache>();
            _mockAuthenticationApiClient = new Mock<IAuthenticationApiClient>();

            var tokenResponse = new AccessTokenResponse();
            tokenResponse.AccessToken = "test_token";
            tokenResponse.ExpiresIn = 100;
            _mockAuthenticationApiClient.Setup(x => x.GetTokenAsync(It.IsAny<ClientCredentialsTokenRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(tokenResponse));

            _serviceSettings = new ServiceSettings
            {
                eConsentUrl = "https://www.econsent.com/"
            };

            _mockKeyVaultService = new Mock<IKeyVaultService>();
            _mockKeyVaultService
                .Setup(q => q.GetSecretValueFromKey(keyVaultKey))
                .ReturnsAsync(keyVaultResponse);
        }

        [TestMethod]
        public async Task AuthService_SendEmail_MakesCorrectApiCall()
        {
            // Arrange
            var expectedAddress = new Uri($"{_authSettings.Audience_AAM}{_sendEmail}").AbsoluteUri;
            SetupHttpFactory(HttpStatusCode.OK, string.Empty);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            await _authService.SendEmail(emailModel);

            // Assert
            _mockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            _mockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(expectedAddress, _passedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Post, _passedInRequestMessage.Method);
            Assert.AreEqual(_emailRequest, _passedInRequestJson);
        }

        [TestMethod]
        public async Task AuthService_SendEmail_Success_ReturnsResponse()
        {
            // Arrange
            SetupHttpFactory(HttpStatusCode.OK, string.Empty);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            var response = await _authService.SendEmail(emailModel);

            // Assert
            Assert.AreEqual(true, response);
        }

        [TestMethod]
        public async Task AuthService_SendEmail_Failure_ThrowsException()
        {
            // Arrange
            var failMessage = "Internal Server Error";
            var exceptionMessage = $"Send email failed : {_authSettings.Audience_AAM}{_sendEmail} - {failMessage}";
            SetupHttpFactory(HttpStatusCode.InternalServerError, failMessage);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            try
            {
                await _authService.SendEmail(emailModel);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(exceptionMessage, e.Message);
            }
        }

        [TestMethod]
        public async Task AuthService_CreateSubjectAsync_MakesCorrectApiCall()
        {
            // Arrange
            var expectedAddress = new Uri($"{_authSettings.Audience_AAM}{_subjects}").AbsoluteUri;
            SetupHttpFactory(HttpStatusCode.OK, _authSignupResponse);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            await _authService.CreateSubjectAsync(_id, _pin);

            // Assert
            _mockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            _mockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(expectedAddress, _passedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Post, _passedInRequestMessage.Method);
            Assert.AreEqual(_authSignupRequest, _passedInRequestJson);
        }

        [TestMethod]
        public async Task AuthService_CreateSubjectAsync_Success_ReturnsResponse()
        {
            // Arrange
            SetupHttpFactory(HttpStatusCode.OK, _authSignupResponse);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            var response = await _authService.CreateSubjectAsync(_id, _pin);

            // Assert
            Assert.AreEqual(_authSignupResponse, JsonConvert.SerializeObject(response));
        }

        [TestMethod]
        public async Task AuthService_CreateSubjectAsync_Failure_ThrowsException()
        {
            // Arrange
            var failMessage = "Failed!";
            var expcetionMessage = $"Subject signup failed : {_authSettings.Audience_AAM}{_subjects} - {failMessage}";
            SetupHttpFactory(HttpStatusCode.InternalServerError, failMessage);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            try
            {
                await _authService.CreateSubjectAsync(_id, _pin);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(expcetionMessage, e.Message);
            }
        }

        [TestMethod]
        public async Task AuthService_ChangePasswordAsync_MakesCorrectApiCall()
        {
            // Arrange
            var expectedAddress = new Uri(string.Format($"{_authSettings.Audience_AAM}{_changePassword}", _userId)).AbsoluteUri;
            SetupHttpFactory(HttpStatusCode.OK, string.Empty);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            await _authService.ChangePasswordAsync(_userId, _updatedPin);

            // Assert
            _mockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            _mockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(expectedAddress, _passedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Post, _passedInRequestMessage.Method);
            Assert.AreEqual(_authChangePasswordRequest, _passedInRequestJson);
        }

        [TestMethod]
        public async Task AuthService_ChangePasswordAsync_Success_DoesNotThrowException()
        {
            // Arrange
            SetupHttpFactory(HttpStatusCode.OK, string.Empty);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            try
            {
                // Act
                await _authService.ChangePasswordAsync(_userId, _pin);
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception thrown: {e.Message}");
            }
        }

        [TestMethod]
        public async Task AuthService_ChangePasswordAsync_Failure_ThrowsException()
        {
            // Arrange
            var failMessage = "Failed!";
            var expectedAddress = new Uri(string.Format($"{_authSettings.Audience_AAM}{_changePassword}", _userId)).AbsoluteUri;
            var expcetionMessage = $"Subject update pin failed : {expectedAddress} - {failMessage}";
            SetupHttpFactory(HttpStatusCode.InternalServerError, failMessage);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            try
            {
                await _authService.ChangePasswordAsync(_userId, _pin);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(expcetionMessage, e.Message);
            }
        }

        [TestMethod]
        public async Task AuthService_UpdateECOALink_Failure_ThrowsException()
        {
            // Arrange
            var failMessage = "Failed!";
            var exceptionMessage = $"Submit subject to eConsent failed: {_serviceSettings.eConsentUrl}{_updateECOALink}{_participantId}?ecoaRecordId={_ecoaRecordId} - {failMessage}";
            SetupHttpFactory(HttpStatusCode.InternalServerError, failMessage);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            try
            {
                await _authService.UpdateECOALink(_participantId, _ecoaRecordId);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(exceptionMessage, e.Message);
            }
        }

        [TestMethod]
        public async Task AuthService_UpdateECOALink_MakesCorrectApiCall()
        {
            // Arrange
            var expectedAddress = new Uri(string.Format($"{_serviceSettings.eConsentUrl}{_updateECOALink}{_participantId}?ecoaRecordId={_ecoaRecordId}")).AbsoluteUri;
            SetupHttpFactory(HttpStatusCode.OK, string.Empty);
            _authService = new AuthService(_mockHttpFactory.Object, _mockAuthenticationApiClient.Object, _authSettings, _memoryCache, _serviceSettings, _mockKeyVaultService.Object);

            // Act
            await _authService.UpdateECOALink(_participantId, _ecoaRecordId);

            // Assert
            _mockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            _mockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(expectedAddress, _passedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Put, _passedInRequestMessage.Method);
        }

        private void SetupHttpFactory(
            HttpStatusCode httpStatusCode,
            string responseContent)
        {
            var httpResponseMessage = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(responseContent, Encoding.UTF32, "application/json"),
            };

            _mockClientHandler = new Mock<DelegatingHandler>();

            _mockClientHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    nameof(HttpClient.SendAsync),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage)
                .Callback(async (HttpRequestMessage requestMessage, CancellationToken token) =>
                {
                    _passedInRequestMessage = requestMessage;
                    _passedInRequestJson = await requestMessage.Content.ReadAsStringAsync();
                })
                .Verifiable();

            _mockClientHandler
                .As<IDisposable>()
                .Setup(ch => ch.Dispose());

            var httpClient = new HttpClient(_mockClientHandler.Object);

            _mockHttpFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

            _mockHttpFactory
                .Setup(r => r.CreateClient(It.IsAny<string>()))
                .Returns(httpClient)
                .Verifiable();
        }
    }
}
