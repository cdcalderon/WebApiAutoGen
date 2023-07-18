using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmah;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Models;
using YPrime.BusinessLayer.PowerBi;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Services
{
    public class BIEmbedService : IBIEmbedService
    {
        private const string AuthorityUrlTenantIdPlaceholder = "organizations";

        private readonly IRoleRepository _roleRepository;
        private readonly IPowerBiContext _powerBiContext;
        private readonly IServiceSettings _serviceSettings;

        public BIEmbedService(
            IServiceSettings serviceSettings,
            IRoleRepository roleRepository,
            IKeyVaultBasedContextFactory powerBiContextService)
        {
            _roleRepository = roleRepository;
            _serviceSettings = serviceSettings;

            try
            {
                _powerBiContext = Task.Run(async () => await powerBiContextService.GetCurrentContext<PowerBiContext>().ConfigureAwait(false)).Result;
            }
            catch (Exception ex)
            {
                // not catching the error here results in an exposure of stack trace
                // letting it fail later down the line will result in a more graceful 
                // error handling
                ErrorSignal.FromCurrentContext().Raise(ex);
                _powerBiContext = null;
            }
        }

        public async Task<IEnumerable<ExternalReport>> FilterReportsFromRole(IEnumerable<Report> analytics)
        {
            var currentUserRoles = await _roleRepository.GetUserRoles(YPrimeSession.Instance.CurrentUser.Id);
            var assignedRoleId = currentUserRoles.FirstOrDefault().Id;

            var referencesForRole = await _roleRepository.GetAllAnalyticsReferencesByRole(assignedRoleId);

            var result = new List<ExternalReport>();

            foreach (var analytic in analytics ?? new List<Report>())
            {
                var matchingReference = referencesForRole.FirstOrDefault(r => r.DisplayName.ToLower() == analytic.Name.ToLower());

                if (matchingReference != null)
                {
                    result.Add(new ExternalReport
                    {
                        Report = analytic,
                        IsSponsorReport = matchingReference.SponsorReport.HasValue && matchingReference.SponsorReport.Value
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<ExternalReport>> GetAnalyticsInWorkspace()
        {
            using (var pbiClient = await GetPowerBiClient(await GetAppAccessToken()))
            {
                var analyticsForWorkGroup = pbiClient.Reports.GetReportsInGroup(_powerBiContext.WorkspaceId.Value);
                var analyticsForCurrentUser = await FilterReportsFromRole(analyticsForWorkGroup.Value);

                return analyticsForCurrentUser
                    .FilterByExcludedPrefix(_powerBiContext.ExcludedPrefix)
                    .FilterByExcludedTerms(_powerBiContext.ExcludedTermSet);
            }
        }

        public async Task<EmbedConfig> GetEmbedConfig(Guid reportId, string[] userSiteIds)
        {
            EmbedConfig embedConfig;

            try
            {
                var appAccessToken = await GetAppAccessToken();
                embedConfig = await GenerateEmbedConfig(reportId, appAccessToken, userSiteIds);
            }
            catch (Exception e)
            {
                embedConfig = new EmbedConfig
                {
                    ErrorMessage = e.ToString(),
                };
            }

            return embedConfig;
        }

        private async Task<EmbedConfig> GenerateEmbedConfig(Guid reportId, string accessToken, string[] userSiteIds)
        {
            using (var pbiClient = await GetPowerBiClient(accessToken))
            {
                var report = pbiClient.Reports.GetReportInGroup(_powerBiContext.WorkspaceId.Value, reportId);

                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");

                var tokenResponse = pbiClient.Reports
                    .GenerateToken(_powerBiContext.WorkspaceId.Value, reportId, generateTokenRequestParameters);

                var studyID = _serviceSettings.StudyId;
                var sponsorID = _serviceSettings.SponsorId;

                return new EmbedConfig
                {
                    EmbedToken = tokenResponse,
                    AccessToken = accessToken,
                    EmbedUrl = report.EmbedUrl,
                    ReportId = report.Id,
                    ErrorMessage = null,
                    StudyID = studyID.ToString(),
                    SponsorID = sponsorID.ToString(),
                    UserSites = userSiteIds
                };
            }
        }

        private async Task<string> GetAppAccessToken()
        {
            var tenantSpecificURL = _powerBiContext.AuthorityUrl.Replace(AuthorityUrlTenantIdPlaceholder, _powerBiContext.TenantId);

            var clientApp = ConfidentialClientApplicationBuilder
                .Create(_powerBiContext.ApplicationId)
                .WithClientSecret(_powerBiContext.ApplicationSecret)
                .WithAuthority(tenantSpecificURL)
                .Build();

            var authenticationResult = await clientApp
                .AcquireTokenForClient(new string[] { _powerBiContext.ResourceUrl })
                .ExecuteAsync();

            return authenticationResult.AccessToken;
        }

        private async Task<PowerBIClient> GetPowerBiClient(string accessToken)
        {
            var tokenCredentials = new TokenCredentials(accessToken, "Bearer");
            return new PowerBIClient(new Uri(_powerBiContext.ApiUrl), tokenCredentials);
        }
    }
}