using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.eCOA.DTOLibrary.ApiDtos;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class AnalyticsSteps
    {
        private const string HttpClientName = "e2eHttpClient";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly E2ERepository _e2eRepository;
        private readonly E2ESettings _e2eSettings;

        private AnalyticsReferenceInputDto _analyticsInputDto;
        private int _responseStatusCode;

        public AnalyticsSteps(
            E2ERepository e2eRepository,
            IHttpClientFactory httpClientFactory,
            E2ESettings e2eSettings)
        {
            e2eRepository.SetupSessionService();
            _e2eRepository = e2eRepository;
            _httpClientFactory = httpClientFactory;
            _analyticsInputDto = new AnalyticsReferenceInputDto();
            _e2eSettings = e2eSettings;
        }

        [Given(@"API request contains analytics")]
        [Then(@"API request contains analytics")]
        public async Task GivenAPIRequestContainsAnalytics(Table table)
        {
            _analyticsInputDto = table.CreateInstance<AnalyticsReferenceInputDto>();
        }

        [When(@"the request is made to ""(.*)"" Analytics Report endpoint ""(.*)"" ""(.*)""")]
        public async Task RequestIsMadeToActionOnAnalyticsReportByInternalName(string type, string InternalValue, string displayValue)
        {
            string path = string.Empty;
            if (type?.ToLower() =="delete")
            {
                path = $"Analytics/DeleteReportByInternalName?internalName={_analyticsInputDto.InternalName}";
            }
            else
            {
                path = $"Analytics/UpdateReportName?internalName={_analyticsInputDto.InternalName}&updatedInternalName={InternalValue}&updatedDisplayName={displayValue}";
            }

            var client = _httpClientFactory.CreateClient(HttpClientName);

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, path);
            var response = await client.SendAsync(httpMessage);
            _responseStatusCode = (int)response.StatusCode;
        }

        [Given(@"the request is made to Add Analytics Report endpoint")]
        [When(@"the request is made to Add Analytics Report endpoint")]
        public async Task WhenTheRequestIsMadeToAddAnalyticsReportEndpoint()
        {
            var path = $"Analytics/AnalyticsReference/";
            var client = await _httpClientFactory.CreateAuthClient(HttpClientName, _e2eSettings);
            var data = new StringContent(JsonConvert.SerializeObject(_analyticsInputDto), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(HttpMethod.Post,path)
                {
                    Content = data
                };

            var response = await client.SendAsync(httpMessage);
            _responseStatusCode = (int)response.StatusCode;
        }

        [Given(@"Analytics report has the following data")]
        public async Task InsertAnalyticsReportByInternalAndDisplayName(Table table)
        {
            var insertItem = table.CreateInstance<AnalyticsReferenceInputDto>();
            var path = $"Analytics/AnalyticsReference/";
            var client = _httpClientFactory.CreateClient(HttpClientName);
            var data = new StringContent(JsonConvert.SerializeObject(insertItem), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = data
            };

            var response = await client.SendAsync(httpMessage);
            _responseStatusCode = (int)response.StatusCode;
        }

        [Given(@"the Add Analytics Report API response status code is ""(.*)""")]
        [Then(@"the Add Analytics Report API response status code is ""(.*)""")]
        [Then(@"the Delete Analytics Report API response status code is ""(.*)""")]
        [Then(@"the Update Analytics Report API reponse status code is ""(.*)""")]
        public void ThenAPIResponseSatusCodeIs(int statusCode)
        {
            Assert.AreEqual(_responseStatusCode, statusCode);
        }

        [Then(@"the data is in the analytics table")]
        public void ThenStudyUserRoleAuditTableHasNewRecordForFollowingData()
        {
            var result = _e2eRepository.GetAnalytics(_analyticsInputDto.InternalName);

            Assert.IsNotNull(result);
            Assert.AreEqual(_analyticsInputDto.DisplayName , result.DisplayName);
        }

        [Then(@"the Updated data is in the analytics table ""(.*)""")]
        public void UpdatedDataIsReadyIntheResults(string internalNameUpdated)
        {
            var result = _e2eRepository.GetAnalytics(internalNameUpdated);

            Assert.IsNotNull(result);
            Assert.AreEqual(internalNameUpdated, result.InternalName);
        }

        [Then(@"the data is not in the analytics table")]
        public void TheDataIsNotInThAnalyticsTable()
        {
            var result = _e2eRepository.GetAnalytics(_analyticsInputDto.InternalName);
            Assert.IsNull(result);
        }

        [Given(@"API request contains authentication header and has the following data")]
        [Given(@"API request contains no authentication header and has the following data")]
        public void GivenAPIRequestContainsAuthenticationHeaderAndHasTheFollowingData(Table table)
        {
            _analyticsInputDto = table.CreateInstance<AnalyticsReferenceInputDto>();
        }

        [When(@"the request is made to Add Analytics Report endpoint with authentication header")]
        public async Task WhenTheRequestIsMadeToAddAnalyticsReportEndpointWithAuthenticationHeader()
        {
            var path = $"Analytics/AnalyticsReference/";
            var client = await _httpClientFactory.CreateAuthClient(HttpClientName, _e2eSettings);
            var data = new StringContent(JsonConvert.SerializeObject(_analyticsInputDto), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = data
            };

            var response = await client.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead, new CancellationToken());
            _responseStatusCode = (int)response.StatusCode;
        }
    }
}
