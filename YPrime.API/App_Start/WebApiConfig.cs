using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Web.Mvc;
using Elmah.Contrib.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Owin;
using YPrime.API.Controllers;
using YPrime.BusinessLayer.DataSync.Factories;
using YPrime.BusinessLayer.Helpers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Query.Handlers;
using YPrime.BusinessLayer.Query.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.Services;
using YPrime.BusinessRule.Factories;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;
using YPrime.Data.Study;
using YPrime.StorageService.Services.Interfaces;
using YPrime.StorageService.Services.Services;
using System.Configuration;
using Auth0.AuthenticationApi;

namespace YPrime.API
{
    public static class WebApiConfig
    {
        private const int DefaultSlidingCacheExpirationSeconds = 1296000; // 15 days in seconds

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //Enable ELMAH
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                "NotFound",
                "{*path}",
                new { controller = "Error", action = "NotFound" }
            );

            config.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings(); //remove backing fields from json
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            config.Formatters.JsonFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

        public static void Configure(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Set WebAPI Resolver and register            
            config.DependencyResolver = resolver;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            var serviceSettings = BuildServiceSettings();
            services.AddSingleton<IServiceSettings>(serviceSettings);

            var authSettings = BuildAuthSettings();
            services.AddSingleton<IAuthSettings>(authSettings);

            var keyVault = BuildKeyVaultClient();
            services.AddSingleton<IKeyVault>(keyVault);

            services.AddSingleton<IFileService, FileService>();

            services.AddTransient<IStudyDbContext, StudyDbContext>();
            services.AddTransient<IConfigurationSettings, BusinessLayer.Helpers.ConfigurationSettings>();
            services.AddTransient<ISqlBuilderFactory, SqlBuilderFactory>();

            //Register Repositories
            services.AddTransient<IAlarmRepository, AlarmRepository>();
            services.AddTransient<IAnalyticsRepository, AnalyticsRepository>();
            services.AddTransient<IAnswerRepository, AnswerRepository>();
            services.AddTransient<IApiRepository, ApiRepository>();
            services.AddTransient<IAuthenticationUserRepository, AuthenticationUserRepository>();
            services.AddTransient<IConfirmationRepository, ConfirmationRepository>();
            services.AddTransient<ICorrectionRepository, CorrectionRepository>();
            services.AddTransient<IDataCopyRepository, DataCopyRepository>();
            services.AddTransient<IDataSyncRepository, DataSyncRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<IDiaryEntryRepository, DiaryEntryRepository>();
            services.AddTransient<IDiaryPageRepository, DiaryPageRepository>();
            services.AddTransient<IExportRepository, ExportRepository>();
            services.AddTransient<IJwtRepository, JwtRepository>();
            services.AddTransient<INotificationRequestRepository, NotificationRequestRepository>();
            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<IPatientVisitRepository, PatientVisitRepository>();
            services.AddTransient<IPatientAttributeRepository, PatientAttributeRepository>();
            services.AddTransient<IPrimeInventoryAPIRepository, PrimeInventoryAPIRepository>();
            services.AddTransient<IBusinessRuleOperationFactory, BusinessRuleOperationFactory>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRuleService, RuleRepository>();
            services.AddTransient<ISiteRepository, SiteRepository>();
            services.AddTransient<ISoftwareReleaseRepository, SoftwareReleaseRepository>();
            services.AddTransient<ISoftwareVersionRepository, SoftwareVersionRepository>();
            services.AddTransient<ISyncLogRepository, SyncLogRepository>();
            services.AddTransient<ISystemSettingRepository, SystemSettingRepository>();
            services.AddTransient<ITimeZoneRepository, TimeZoneRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IWebBackupRepository, WebBackupRepository>();

            //Config Services
            services.AddTransient<IApproverGroupService, ApproverGroupService>();
            services.AddTransient<IBusinessRuleService, BusinessRuleService>();
            services.AddTransient<ICareGiverTypeService, CareGiverTypeService>();
            services.AddTransient<IConfigurationVersionService, ConfigurationVersionService>();
            services.AddTransient<ICorrectionTypeService, CorrectionTypeService>();
            services.AddTransient<ICorrectionWorkflowService, CorrectionWorkflowService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IPatientStatusService, PatientStatusService>();
            services.AddTransient<IQuestionnaireService, QuestionnaireService>();
            services.AddTransient<IStudyRoleService, StudyRoleService>();
            services.AddTransient<IStudySettingService, StudySettingService>();
            services.AddTransient<ISubjectInformationService, SubjectInformationService>();
            services.AddTransient<ITranslationService, TranslationService>();
            services.AddTransient<IVisitService, VisitService>();
            services.AddTransient<INotificationScheduleService, NotificationScheduleService>();
            services.AddTransient<IAlarmService, AlarmService>();
            services.AddTransient<IKeyVaultService, KeyVaultService>();
            services.AddTransient<IBlobStorageService, BlobStorageService>();
            services.AddTransient<IStorageAccountService, StorageAccountService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAuthenticationApiClient>(s => new AuthenticationApiClient(new Uri(authSettings.BaseUrl)));

            //Scoped services
            services.AddScoped<ISessionService, SessionService>();

            //Register controllers
            services.AddTransient(typeof(AnalyticsController));
            services.AddTransient(typeof(DataSyncController));
            services.AddTransient(typeof(DeviceManagementController));
            services.AddTransient(typeof(DiaryEntryController));
            services.AddTransient(typeof(ErrorController));
            services.AddTransient(typeof(PatientController));
            services.AddTransient(typeof(PatientVisitController));
            services.AddTransient(typeof(SiteController));
            services.AddTransient(typeof(SSOController));
            services.AddTransient(typeof(StudyController));
            services.AddTransient(typeof(UserController));
            services.AddTransient(typeof(BusinessRuleController));
            services.AddTransient(typeof(StudyDbContext));

            services.AddTransient<IPatientVisitSummaryQueryHandler, PatientVisitSummaryQueryHandler>();

            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            resolver = ConfigureHttpClient(resolver, services);

            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        public static ServiceSettings BuildServiceSettings()
        {
            var settings = new ServiceSettings
            {
                StudyId = Guid.Parse(ConfigurationManager.AppSettings["StudyID"]),
                AuthUrl = ConfigurationManager.AppSettings["YPrimeAuthURL"],
                StudyBuilderAppEnvironment = ConfigurationManager.AppSettings["AppEnvironmentShortName"],
                StudyPortalAppEnvironment = ConfigurationManager.AppSettings["AppEnvironment"]
            };

            int slidingExpirationSeconds;

            if (!int.TryParse(ConfigurationManager.AppSettings["SlidingCacheExpirationSeconds"], out slidingExpirationSeconds))
            {
                slidingExpirationSeconds = DefaultSlidingCacheExpirationSeconds;
            }

            settings.SlidingCacheExpirationSeconds = slidingExpirationSeconds;

            return settings;
        }

        public static AuthSettings BuildAuthSettings()
        {
            return new AuthSettings
            {
                BaseUrl = $"https://{ConfigurationManager.AppSettings["Auth.Domain"]}",
                Audience_AAM = ConfigurationManager.AppSettings["Auth.Audience.AAM"],
                Audience_SB = ConfigurationManager.AppSettings["Auth.Audience.SB"],
                ClientId_M2M = ConfigurationManager.AppSettings["Auth.ClientId.M2M"],
                ClientSecret_M2M = ConfigurationManager.AppSettings["Auth.ClientSecret.M2M"]
            };
        }

        private static KeyVaultClient BuildKeyVaultClient()
        {
            string keyVaultURL = $"https://{ConfigurationManager.AppSettings["KeyVaultName"]}.vault.azure.net";

            TokenCredential credential = new DefaultAzureCredential();

            // If debugging locally you will need a different set of credientals. Reach out to team for them

            var client = new KeyVaultClient()
            {
                Client = new SecretClient(new Uri(keyVaultURL), credential)
            };

            return client;
        }

        public static DefaultDependencyResolver ConfigureHttpClient(DefaultDependencyResolver resolver, IServiceCollection services)
        {
            var studyId = ConfigurationManager.AppSettings["StudyID"];
            var studyBuilderUrl = ConfigurationManager.AppSettings["StudyBuilderApiBaseURL"];

            services.AddHttpClient("configHttpClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("StudyId", studyId);
                client.BaseAddress = new Uri(studyBuilderUrl);
            });

            services.AddMemoryCache();

            var updatedResolver = new DefaultDependencyResolver(services.BuildServiceProvider());

            return updatedResolver;
        }

    }
}