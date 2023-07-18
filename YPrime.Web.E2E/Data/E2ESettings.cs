using System;
using System.Security.Principal;

namespace YPrime.Web.E2E.Data
{
    public class E2ESettings
    {
        public Guid StudyId { get; set; }
        public string StudyDbContext { get; set; }
        public string PortalUrl { get; set; }
        public string ApiUrl { get; set; }
        public string SSOUrl { get; set; }
        public string StudyBuilderApiUrl { get; set; }
        public string NotificationScheduleUrl { get; set; }
        public string Auth0ClientId { get; set; }
        public string Auth0ClientSecret { get; set; }
        public string Auth0Audience { get; set; }
        public string Auth0Url { get; set; }
        public IPrincipal CurrentUser { get; set; }
    }
}
