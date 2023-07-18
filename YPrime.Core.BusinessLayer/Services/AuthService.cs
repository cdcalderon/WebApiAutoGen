using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationApiClient _authenticationApiClient;
        private readonly IAuthSettings _authSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceSettings _serviceSettings;
        private readonly IKeyVaultService _keyVaultService;

        private const string _tokenCacheKey = "Auth0Token";
        private static readonly string _subjects = "/v1/subjects";
        private static readonly string _updatePassword = _subjects + "/{0}/change-password";
        private static readonly string _updateECOALink = "api/e-consent/v1/participants/update-ecoa-link/";
        private static readonly string _sendEmail = "/v1/email/send";

        public AuthService(IHttpClientFactory httpClientFactory, IAuthenticationApiClient authenticationApiClient, IAuthSettings authSettings, IMemoryCache memoryCache, IServiceSettings serviceSettings, IKeyVaultService keyVaultService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _authenticationApiClient = authenticationApiClient;
            _authSettings = authSettings;
            _memoryCache = memoryCache;
            _serviceSettings = serviceSettings;
            _keyVaultService = keyVaultService;
        }

        public async Task<bool> SendEmail(SendingEmailModel email)
        {
            var token = await GetTokenAsync(_authSettings.ClientId_M2M, _authSettings.ClientSecret_M2M, _authSettings.Audience_AAM);

            var uri = new Uri($"{_authSettings.Audience_AAM}{_sendEmail}");
            var body = JsonConvert.SerializeObject(new SendEmailRequest
            {
                From = email.From,
                To = email.To,
                Cc = email.Cc,
                Bcc = email.Bcc,
                Subject = email.Subject,
                Body = email.Body,
                Attachments = email.Attachments,
                StudyId = email.StudyId,
                SponsorId = email.SponsorId,
                Environment = email.Environment,
            });

            var policy = HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(3, (retryCount) => TimeSpan.FromSeconds(retryCount));
            var policyResult = await policy.ExecuteAndCaptureAsync(async () =>
            {
                var httpRequest = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await _httpClient.SendAsync(httpRequest);
            });

            if(policyResult.Outcome != OutcomeType.Successful)
            {
                throw new Exception($"Send email failed : {uri.AbsoluteUri} - {policyResult.FinalHandledResult?.ReasonPhrase}");
            }
            return true;
        }

        public async Task<AuthUserSignupResponse> CreateSubjectAsync(Guid patientId, string pin)
        {
            var token = await GetTokenAsync(_authSettings.ClientId_M2M, _authSettings.ClientSecret_M2M, _authSettings.Audience_AAM);

            var uri = new Uri($"{_authSettings.Audience_AAM}{_subjects}");
            var body = JsonConvert.SerializeObject(new AuthUserSignupRequest
            {
                Email = $"{patientId}@yprimesubject.com",
                Password = pin
            });
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Subject signup failed : {uri.AbsoluteUri} - {ex.Message}");
            }

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var userSignupResponse = JsonConvert.DeserializeObject<AuthUserSignupResponse>(responseJson);
                return userSignupResponse;
            }

            var textReponse = await response.Content.ReadAsStringAsync();

            throw new Exception($"Subject signup failed : {uri.AbsoluteUri} - {textReponse}");
        }

        public async Task ChangePasswordAsync(string authUserId, string pin)
        {
            var token = await GetTokenAsync(_authSettings.ClientId_M2M, _authSettings.ClientSecret_M2M, _authSettings.Audience_AAM);

            var uri = new Uri(string.Format($"{_authSettings.Audience_AAM}{_updatePassword}", authUserId));
            var body = JsonConvert.SerializeObject(new { password = pin });
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Subject update pin failed : {uri.AbsoluteUri} - {ex.Message}");
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var textReponse = await response.Content.ReadAsStringAsync();

                throw new Exception($"Subject update pin failed : {uri.AbsoluteUri} - {textReponse}");
            }
        }

        public async Task<bool> UpdateECOALink(Guid participantId, Guid ecoaRecordId, string BYODAssetTag = null)
        {
            await GeteConsentClientIdAndSecret();
            var token = await GetTokenAsync(_authSettings.ClientId_eConsent, _authSettings.ClientSecret_eConsent, _authSettings.Audience_eConsent);

            string byodAssetTag = string.Empty;
            if(!string.IsNullOrEmpty(BYODAssetTag))
            {
                byodAssetTag = $"&ecoaAssetTag={BYODAssetTag}";
            }
            var uri = new Uri($"{_serviceSettings.eConsentUrl}{_updateECOALink}{participantId}?ecoaRecordId={ecoaRecordId}{byodAssetTag}");
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Put
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Submit subject to eConsent failed : {uri.AbsoluteUri} - {ex.Message}");
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var textReponse = await response.Content.ReadAsStringAsync();

            throw new Exception($"Submit subject to eConsent failed: {uri.AbsoluteUri} - {textReponse}");
        }

        public async Task<string> GetTokenAsync(string clientId, string clientSecret, string audience)
        {
            // get token from cache
            var hasCachedToken = _memoryCache.TryGetValue($"{_tokenCacheKey}:{audience}", out string token);

            if (!hasCachedToken)
            {
                var newTokenInfo = await GetAuth0AccessToken(clientId, clientSecret, audience);
                token = newTokenInfo.AccessToken;

                _memoryCache.Set($"{_tokenCacheKey}:{audience}", token,
                    new TimeSpan(0, 0, newTokenInfo.ExpiresIn));
            }

            return token;
        }

        private async Task<AccessTokenResponse> GetAuth0AccessToken(string clientId, string clientSecret, string audience)
        {
            var tokenInfo = await _authenticationApiClient.GetTokenAsync(new ClientCredentialsTokenRequest()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Audience = audience
            });

            return tokenInfo;
        }

        private async Task<bool> GeteConsentClientIdAndSecret()
        {
            if (string.IsNullOrWhiteSpace(_authSettings.ClientId_eConsent) || string.IsNullOrWhiteSpace(_authSettings.ClientSecret_eConsent))
            {
                var json = await _keyVaultService.GetSecretValueFromKey(AuthConstants.eConsentAPICredentialsSecretKey);
                var keyvaultClientIdAndSecret = JsonConvert.DeserializeObject<KeyvaultClientIdAndSecret>(json);
                _authSettings.ClientId_eConsent = keyvaultClientIdAndSecret.ClientId;
                _authSettings.ClientSecret_eConsent = keyvaultClientIdAndSecret.ClientSecret;
            }

            return true;
        }
    }
}