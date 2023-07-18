using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Auth.Data.Models.JSON;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Endpoints;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Models.Api;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class SSOSteps
    {
        private const string HttpClientName = "e2eHttpClient";
        private readonly ApiTestData apiTestData;
        private readonly E2ERepository e2eRepository;
        protected readonly IHttpClientFactory httpClientFactory;

        private const string ypStudyHeaderValue = "System";
        private UserData userData;

        public SSOSteps(CheckForUpdatesEndpoint checkForUpdatesEndpoint,
            ApiTestData apiTestData,
            E2ERepository e2eRepository,
            IHttpClientFactory httpClientFactory)
        {
            this.apiTestData = apiTestData;
            this.e2eRepository = e2eRepository;
            this.httpClientFactory = httpClientFactory;

            e2eRepository.SetupSessionService();
            userData = new UserData();
        }

        [Given(@"API request contains study user data")]
        public void GivenAPIRequestContainsStudyUserData(Table table)
        {
            var idGuid = new Guid();
            var syncRequest = table.CreateInstance<SSORequestStudy>();

            var mappedUser = GetMappedUser(syncRequest.StudyUserName);

            idGuid = (userData.StudyUser != null) ? userData.StudyUser.Id : idGuid;
            e2eRepository.SetupSessionService(mappedUser.Username);
            userData = new UserData();
            userData.FirstName = syncRequest.StudyUserName;
            userData.LastName = syncRequest.StudyUserName;
            userData.StudyUser = new StudyUser();
            userData.StudyUser.Id = idGuid;
            userData.StudyUser.Email = mappedUser.Username;
            userData.StudyUser.UserName = mappedUser.Username;
        }

        [Given(@"API request contains ids for study user role data")]
        public async Task GivenAPIRequestContainsIdsForStudyUserRoleData(Table table)
        {
            var syncRequest = table.CreateInstance<SSORequestIds>();

            var site = e2eRepository.GetSite(syncRequest.Site);
            var role = await e2eRepository.GetRole(syncRequest.StudyRole);

            bool existsInfo = false;
            if (site != null && role != null && apiTestData.SSORequestStudy != null)
            {
                userData.StudyUserRoles = new List<StudyUserRole>();
                StudyUserRole studyuserRole = new StudyUserRole();
                studyuserRole.Id = new Guid();
                studyuserRole.SiteId = site.Id;
                studyuserRole.SiteName = site.Name;
                studyuserRole.SiteNumber = site.SiteNumber;
                studyuserRole.StudyRoleId = role.Id;
                studyuserRole.StudyName = role.ShortName;

                userData.StudyUserRoles.Add(studyuserRole);

                existsInfo = true;
            }

            Assert.AreEqual(true, existsInfo);
        }

        [Given(@"API request contains header")]
        public void GivenAPIRequestContainsHeader(Table table)
        {
            var syncRequest = table.CreateInstance<SSORequestHeaders>();

            apiTestData.SSORequestHeaders.Header = (!string.IsNullOrEmpty(syncRequest.Header)) ? syncRequest.Header : null;
            apiTestData.SSORequestHeaders.Value = (!string.IsNullOrEmpty(syncRequest.Value)) ? new Guid().ToString() : null;
        }

        [When(@"the POST request is made and the response is successful")]
        [Given(@"the POST request is made and the response is successful")]
        public async Task WhenThePOSTRequestIsMadeAndTheResponseIsSuccessful()
        {
            var path = $"SSO/PostUserData/";
            var client = httpClientFactory.CreateClient(HttpClientName);

            object requestObject = userData;

            var data = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

            var httpMessage = new HttpRequestMessage(
                HttpMethod.Post,
                path);
            httpMessage.Content = data;

            if ((!string.IsNullOrEmpty(apiTestData.SSORequestHeaders.Header)) && (!string.IsNullOrEmpty(apiTestData.SSORequestHeaders.Value)))
            {
                httpMessage.Headers.Add(apiTestData.SSORequestHeaders.Header, apiTestData.SSORequestHeaders.Value);
            }

            var response = await client.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead, new CancellationToken());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        [Then(@"I modified the study user")]
        public async Task ThenIModifiedTheStudyUser(Table table)
        {
            var syncRequest = table.CreateInstance<SSORequestIds>();

            var site = e2eRepository.GetSite(syncRequest.Site);
            var role = await e2eRepository.GetRole(syncRequest.StudyRole);

            userData.StudyUserRoles.Clear();
            StudyUserRole studyuserRole = new StudyUserRole();
            studyuserRole.Id = new Guid();
            studyuserRole.SiteId = site.Id;
            studyuserRole.SiteName = site.Name;
            studyuserRole.SiteNumber = site.SiteNumber;
            studyuserRole.StudyRoleId = role.Id;
            studyuserRole.StudyName = role.ShortName;
            userData.StudyUserRoles.Add(studyuserRole);
        }

        [Then(@"Study User Role audit table has new record for following data")]
        [Given(@"Study User Role audit table has new record for following data")]
        public async Task ThenStudyUserRoleAuditTableHasNewRecordForFollowingData(Table table)
        {
            int totRows = 0;

            var syncRequest = table.CreateSet<SSOUserRoleAuditRequest>();

            foreach (var tr in syncRequest)
            {
                totRows = 0;

                var site = e2eRepository.GetSite(tr.SiteId);
                var role = await e2eRepository.GetRole(tr.StudyRoleId);

                if ((site != null) && (role != null))
                {
                    totRows = e2eRepository.GetAuditRecord(
                                                             string.IsNullOrEmpty(apiTestData.SSORequestHeaders.Value) ? ypStudyHeaderValue : apiTestData.SSORequestHeaders.Value  //apiTestData.SSORequestHeaders.Value
                                                            , apiTestData.SSORequestStudy.StudyUserId
                                                            , role.Id
                                                            , site.Id
                                                            , tr.AuditAction);
                }
                Assert.AreEqual(true, totRows > 0);
            }
        }

        private E2EUser GetMappedUser(string mappedUsername)
        {
            // lookup user mapping to get details
            var mappedUser = CommonData
                .UserMappings
                .FirstOrDefault(um => um.MappingName.Equals(
                    mappedUsername,
                    StringComparison.InvariantCultureIgnoreCase));

            if (mappedUser == null)
            {
                throw new NotImplementedException($"{mappedUsername} is not an implemented user");
            }

            return mappedUser;
        }
    }
}
