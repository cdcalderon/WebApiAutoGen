using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Defaults;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

[assembly: PreApplicationStartMethod(typeof(MvcApplication), nameof(MvcApplication.InitModule))]

namespace YPrime.StudyPortal
{
    //**********************************************************
    //README: NO DEVELOPERS SHOULD EVER HAVE TO MODIFY THIS FILE
    //**********************************************************

    public class MvcApplication : HttpApplication
    {
        private const string DefaultGlobalDateFormat = "dd-MMM-yyyy";
        private const string SessionPingPathSuffix = "/Session/Ping";
        private readonly Regex FileTypesToExcludeFromSessionRegex = new Regex(@".*\.(css|js|gif|jpg|png|ico|eot|svg|ttf|woff|woff2|otf|map)$");

        private readonly Dictionary<string, string> unauthenticatedEndpoints = new Dictionary<string, string>
        {
            {"WebBackup", "WebBackupHandheld"}
        };

        public static void InitModule()
        {
            RegisterModule(typeof(ServiceScopeModule));
        }

        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var serviceCollectionConfig = new ServiceCollectionConfig();
            var serviceCollection = serviceCollectionConfig.BuildServiceCollection();
            var serviceProvider = serviceCollectionConfig.BuildServiceProvider(serviceCollection);
            ServiceScopeModule.SetServiceProvider(serviceProvider);
            var resolver = serviceCollectionConfig.BuildResolver(serviceProvider);

            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier; // Fix AntiForgeryToken

            var appInitializations = DependencyResolver.Current.GetServices<IApplicationInitialization>();
            foreach (var appInit in appInitializations)
            {
                appInit.Execute(HttpContext.Current);
            }

            ServiceCollectionConfig.ActivateIronPdf();
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            // had to remove SwitchedStudies() to run locally
            if (IsValidSessionPath(sender))
            {
                LoadSession();
            }
        }

        private bool IsValidSessionPath(object sender)
        {
            var app = sender as HttpApplication;

            if (
                app == null ||
                app.Request == null ||
                FileTypesToExcludeFromSessionRegex.Match(app.Request.Path).Success ||
                app.Request.Path.EndsWith(SessionPingPathSuffix)
            )
            {
                return false;
            }

            return true;
        }


        private void LoadSession()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = claimsIdentity?.FindFirst(c => c.Type == "https://auth.eclinicalcloud.net/ypAuthUserId")?.Value;

            if (userId != null)
            {
                string firstName = claimsIdentity?.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
                string lastName = claimsIdentity?.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;

                var studyUser = GetUser(userId, firstName, lastName);
                LoadSession(studyUser);
            }
        }

        private void LoadSession(StudyUserDto studyUser)
        {
            Response.Headers.Add("SessionLoad", DateTime.Now.ToString());
            var studyId = ConfigurationManager.AppSettings["StudyID"];
            // Inline is not supported.
            Guid studyIdAsGuid;

            if (YPrimeSession.Instance != null)
            {
                if (!string.IsNullOrWhiteSpace(studyId)
                    && Guid.TryParse(studyId, out studyIdAsGuid))
                {
                    YPrimeSession.Instance.StudyId = studyIdAsGuid;
                }
                YPrimeSession.Instance.SinglePatientAlias = "Subject";
                YPrimeSession.Instance.PluralPatientAlias = "Subjects";
                YPrimeSession.Instance.CurrentUser = studyUser;

                var claimsIdentity = User.Identity as ClaimsIdentity;
                string IdToken = claimsIdentity?.FindFirst(c => c.Type == "id_token")?.Value;
                if (!string.IsNullOrEmpty(IdToken))
                {
                    YPrimeSession.Instance.CurrentUserAuth0 = claimsIdentity;
                }

                var softwareReleaseRepository = DependencyResolver.Current.GetService<ISoftwareReleaseRepository>();
                YPrimeSession.Instance.ConfigurationId =
                    Task.Run(async () =>
                    {
                        var userSiteIds = studyUser.Sites.Select(s => s.Id).ToList();
                        var userCountryIds = studyUser.Sites.Select(s => s.CountryId).Distinct().ToList();

                        var vals = await softwareReleaseRepository.FindLatestConfigurationVersion(userSiteIds, userCountryIds);
                        return vals;
                    }).Result;

                var studySettingService = DependencyResolver.Current.GetService<IStudySettingService>();
                var configId = YPrimeSession.Instance.ConfigurationId;
                YPrimeSession.Instance.StudySettingValues =
                     Task.Run(async () =>
                     {
                         var vals = await studySettingService.GetAll(configId);
                         var result = new Dictionary<string, string>();

                         foreach (var val in vals)
                         {
                             if (!result.ContainsKey(val.Key))
                             {
                                 result.Add(val.Key, val.Value);
                             }
                         }

                         return result;
                     }).Result;

                var translationService = DependencyResolver.Current.GetService<ITranslationService>();
                var studyRoleService = DependencyResolver.Current.GetService<IStudyRoleService>();

                // load caches
                Task.Run(async () =>
                {
                    await studySettingService.LoadIntoCache(configId);
                    await translationService.LoadIntoCache(null, configId, Languages.English.Id);
                    await studyRoleService.LoadIntoCache(configId);
                });

                YPrimeSession.Instance.Protocol = YPrimeSession.Instance.StudySettingValues.ContainsKey("Protocol")
                    ? YPrimeSession.Instance.StudySettingValues["Protocol"]
                    : string.Empty;

                YPrimeSession.Instance.Sponsor = YPrimeSession.Instance.StudySettingValues.ContainsKey("StudySponsor")
                    ? YPrimeSession.Instance.StudySettingValues["StudySponsor"]
                    : string.Empty;

                YPrimeSession.Instance.GlobalDateFormat = YPrimeSession.Instance.StudySettingValues.ContainsKey("GlobalDateFormat")
                    ? YPrimeSession.Instance.StudySettingValues["GlobalDateFormat"]
                    : DefaultGlobalDateFormat;

                // Inline is not supported.
                int supportChatEnabled;
                YPrimeSession.Instance.SupportChatEnabled = YPrimeSession.Instance.StudySettingValues.ContainsKey("SupportChatEnabled")
                    && int.TryParse(YPrimeSession.Instance.StudySettingValues["SupportChatEnabled"], out supportChatEnabled)
                    && supportChatEnabled == 1;

                YPrimeSession.Instance.SupportChatURL = YPrimeSession.Instance.StudySettingValues.ContainsKey("SupportChatURL")
                    ? YPrimeSession.Instance.StudySettingValues["SupportChatURL"]
                    : string.Empty;
            }
        }

        private bool SwitchedStudies()
        {
            var applicationCache = DependencyResolver.Current.GetService<IApplicationCache>();
            if (HttpContext.Current.Cache == null || applicationCache.Get("StudyID") == null)
            {
                var appInitializations = DependencyResolver.Current.GetServices<IApplicationInitialization>();
                foreach (var appInit in appInitializations)
                {
                    appInit.Execute(HttpContext.Current);
                }
            }

            var cachedStudyId = applicationCache.Get("StudyID") as Guid?;

            return (HttpContext.Current.Session != null && YPrimeSession.Instance.StudyId != Guid.Empty &&
                    YPrimeSession.Instance.StudyId != cachedStudyId &&
                    !User.Identity.IsAuthenticated);
        }

        private StudyUserDto GetUser(string userId, string firstName, string lastName)
        {
            StudyUserDto studyUser = null;
            var userRepo = DependencyResolver.Current.GetService<IUserRepository>();
            
            Guid userIdAsGuid;
            Guid.TryParse(userId, out userIdAsGuid);

            studyUser = Task.Run(async () =>
            {
                return await userRepo.GetUser(userIdAsGuid, firstName, lastName);
            }).Result;

            return studyUser;
        }
    }
}