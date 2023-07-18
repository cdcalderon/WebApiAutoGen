using Auth0.AuthenticationApi;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Security.Provider;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using YPrime.BusinessLayer.Factories;
using YPrime.BusinessLayer.Helpers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Query.Handlers;
using YPrime.BusinessLayer.Query.Interfaces;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.Services;
using YPrime.BusinessLayer.SyncSQLBuilders;
using YPrime.BusinessRule.Factories;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;
using YPrime.Data.Study;
using YPrime.StorageService.Services.Interfaces;
using YPrime.StorageService.Services.Services;
using YPrime.StudyPortal.Controllers;
using YPrime.StudyPortal.Models;

namespace YPrime.StudyPortal
{
    public class ServiceCollectionConfig
    {
        private const int DefaultCacheExpirationSeconds = 86400; // one day in seconds
        private const int DefaultSlidingCacheExpirationSeconds = 1296000; // 15 days in seconds

        protected static IServiceCollection services { get; set; }
        public ServiceCollection BuildServiceCollection()
        {
            var services = new ServiceCollection();

            var serviceSettings = BuildServiceSettings();
            services.AddSingleton<IServiceSettings>(serviceSettings);

            var authSettings = BuildAuthSettings();
            services.AddSingleton<IAuthSettings>(authSettings);

            var keyVault = BuildKeyVaultClient();
            services.AddSingleton<IKeyVault>(keyVault);

            //Singleton services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IKeyVaultBasedContextFactory, KeyVaultBasedContextFactory>();
            services.AddSingleton<IKeyVaultService, KeyVaultService>();
            services.AddSingleton<IBlobStorageService, BlobStorageService>();
            services.AddSingleton<IStorageAccountService, StorageAccountService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAuthenticationApiClient>(s => new AuthenticationApiClient(new Uri(authSettings.BaseUrl)));

            //Scoped services
            services.AddScoped<IStudyDbContext, StudyDbContext>();
            services.AddScoped<IConfigurationSettings, BusinessLayer.Helpers.ConfigurationSettings>();
            services.AddScoped<IApplicationCache, ApplicationCache>();
            services.AddScoped<IApplicationInitialization, CacheInitializer>();
            services.AddScoped<ISyncSQLBuilder, SQLBuilderPatient>();
            services.AddScoped<ISyncSQLBuilder, SQLBuilderSite>();
            services.AddScoped<ISessionService, SessionService>();

            //Register Repositories
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IApiRepository, ApiRepository>();
            services.AddScoped<IAuthenticationUserRepository, AuthenticationUserRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<IConfirmationRepository, ConfirmationRepository>();
            services.AddScoped<ICorrectionRepository, CorrectionRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IDiaryEntryRepository, DiaryEntryRepository>();
            services.AddScoped<IDiaryPageRepository, DiaryPageRepository>();
            services.AddScoped<IExportRepository, ExportRepository>();
            services.AddScoped<IJwtRepository, JwtRepository>();
            services.AddScoped<INotificationRequestRepository, NotificationRequestRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientForEditAdapter, PatientForEditAdapter>();
            services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
            services.AddScoped<IPatientAttributeRepository, PatientAttributeRepository>();
            services.AddScoped<IPrimeInventoryAPIRepository, PrimeInventoryAPIRepository>();
            services.AddScoped<IReferenceMaterialRepository, ReferenceMaterialRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IBusinessRuleOperationFactory, BusinessRuleOperationFactory>();
            services.AddScoped<IRuleService, RuleRepository>();
            services.AddScoped<IScheduledJobRepository, ScheduledJobRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<ISoftwareReleaseRepository, SoftwareReleaseRepository>();
            services.AddScoped<ISoftwareVersionRepository, SoftwareVersionRepository>();
            services.AddScoped<ISyncLogRepository, SyncLogRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
            services.AddScoped<ISystemActionRepository, SystemActionRepository>();
            services.AddScoped<ITimeZoneRepository, TimeZoneRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWidgetRepository, WidgetRepository>();
            services.AddScoped<IWebBackupRepository, WebBackupRepository>();

            //Config Services
            services.AddScoped<IApproverGroupService, ApproverGroupService>();
            services.AddScoped<IBusinessRuleService, BusinessRuleService>();
            services.AddScoped<ICareGiverTypeService, CareGiverTypeService>();
            services.AddScoped<IConfigurationVersionService, ConfigurationVersionService>();
            services.AddScoped<ICorrectionTypeService, CorrectionTypeService>();
            services.AddScoped<ICorrectionWorkflowService, CorrectionWorkflowService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IPatientStatusService, PatientStatusService>();
            services.AddScoped<IQuestionnaireService, QuestionnaireService>();
            services.AddScoped<IStudyRoleService, StudyRoleService>();
            services.AddScoped<IStudySettingService, StudySettingService>();
            services.AddScoped<ISubjectInformationService, SubjectInformationService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IVisitService, VisitService>();
            services.AddScoped<INotificationScheduleService, NotificationScheduleService>();
            services.AddScoped<IKeyVaultService, KeyVaultService>();
            services.AddScoped<IBIEmbedService, BIEmbedService>();

            //Register controllers
            services.AddTransient(typeof(AnalyticsController));
            services.AddTransient(typeof(CareGiverController));
            services.AddTransient(typeof(ConfirmationController));
            services.AddTransient(typeof(CorrectionController));
            services.AddTransient(typeof(CorrectionWorkflowController));
            services.AddTransient(typeof(DCFRequestController));
            services.AddTransient(typeof(DashboardController));
            services.AddTransient(typeof(DevicesController));
            services.AddTransient(typeof(DiaryEntriesController));
            services.AddTransient(typeof(ExportController));
            services.AddTransient(typeof(FunctionManagementController));
            services.AddTransient(typeof(HomeController));
            services.AddTransient(typeof(PatientController));
            services.AddTransient(typeof(PatientVisitController));
            services.AddTransient(typeof(ReferenceMaterialController));
            services.AddTransient(typeof(ReportController));
            services.AddTransient(typeof(RoleController));
            services.AddTransient(typeof(SiteController));
            services.AddTransient(typeof(SoftwareReleaseController));
            services.AddTransient(typeof(SoftwareVersionController));
            services.AddTransient(typeof(SupportController));
            services.AddTransient(typeof(SyncLogController));
            services.AddTransient(typeof(UIController));
            services.AddTransient(typeof(WebBackupController));
            services.AddTransient(typeof(HangfireJobsController));

            // Other
            services.AddScoped<IReportFactory, ReportFactory>();
            services.AddScoped<IPatientVisitSummaryQueryHandler, PatientVisitSummaryQueryHandler>();

            services.AddScoped<Hangfire.IRecurringJobManager, Hangfire.RecurringJobManager>();
            services.AddScoped<Hangfire.IBackgroundJobClient, Hangfire.BackgroundJobClient>();

            return services;
        }

        public static void ActivateIronPdf()
        {
            IronPdf.License.LicenseKey = ConfigurationManager.AppSettings["IronPdfLicenseKey"];
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

        private static ServiceSettings BuildServiceSettings()
        {
            var settings = new ServiceSettings
            {
                StudyId = Guid.Parse(ConfigurationManager.AppSettings["StudyID"]),
                AuthUrl = ConfigurationManager.AppSettings["YPrimeAuthURL"],
                StudyBuilderAppEnvironment = ConfigurationManager.AppSettings["AppEnvironmentShortName"],
                StudyPortalAppEnvironment = ConfigurationManager.AppSettings["AppEnvironment"],
                InventoryAppEnvironment = ConfigurationManager.AppSettings["AppInventoryEnvironmentName"],
                HMACAuthSharedKey = ConfigurationManager.AppSettings["HMACAuthSharedKey"],
                eConsentUrl = ConfigurationManager.AppSettings["eConsentURL"],
            };

            int slidingExpirationSeconds;

            if (!int.TryParse(ConfigurationManager.AppSettings["SlidingCacheExpirationSeconds"], out slidingExpirationSeconds))
            {
                slidingExpirationSeconds = DefaultSlidingCacheExpirationSeconds;
            }

            settings.SlidingCacheExpirationSeconds = slidingExpirationSeconds;

            // Visual studio will fail to build this if the sponsorId declaration is put inline for some reason
            // but the tryparse() is added for backwards compatability with upgraded studies that may not use the sponsor 
            // level analytics feature
            var sponsorId = Guid.Empty;
            if (Guid.TryParse(ConfigurationManager.AppSettings["SponsorID"], out sponsorId))
            {
                settings.SponsorId = sponsorId;
            }

            return settings;
        }

        public PortalDependencyResolver BuildResolver(IServiceProvider serviceProvider)
        {
            var resolver = new PortalDependencyResolver(serviceProvider);
            return resolver;
        }

        public static AuthSettings BuildAuthSettings()
        {
            return new AuthSettings
            {
                BaseUrl = $"https://{ConfigurationManager.AppSettings["Auth.Domain"]}",
                Audience_AAM = ConfigurationManager.AppSettings["Auth.Audience.AAM"],
                Audience_SB = ConfigurationManager.AppSettings["Auth.Audience.SB"],
                Audience_eConsent = ConfigurationManager.AppSettings["Auth.Audience.eConsent"],
                ClientId_M2M = ConfigurationManager.AppSettings["Auth.ClientId.M2M"],
                ClientSecret_M2M = ConfigurationManager.AppSettings["Auth.ClientSecret.M2M"]
            };
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            var studyId = ConfigurationManager.AppSettings["StudyID"];
            var studyBuilderUrl = ConfigurationManager.AppSettings["StudyBuilderApiBaseURL"];
            var notificationScheduleUrl = ConfigurationManager.AppSettings["YPrimeNotificationScheduleUrl"];
            var notificationSchedulerApiKey = ConfigurationManager.AppSettings["YPrimeNotificationServiceApiKey"];

            services.AddHttpClient("configHttpClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("StudyId", studyId);
                client.BaseAddress = new Uri(studyBuilderUrl);
            });

            services.AddHttpClient("notificationHttpClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", notificationSchedulerApiKey);
                client.BaseAddress = new Uri(notificationScheduleUrl);
            });

            var cacheExpirationSeconds = ConfigurationManager.AppSettings["CacheExpirationSeconds"];
            int expirationSeconds;

            if (!int.TryParse(cacheExpirationSeconds, out expirationSeconds))
            {
                expirationSeconds = DefaultCacheExpirationSeconds;
            }

            Action<MemoryCacheOptions> setupOptions = (MemoryCacheOptions memoryCacheOptions) =>
            {
                memoryCacheOptions = new MemoryCacheOptions
                {
                    CompactionPercentage = .25,
                    ExpirationScanFrequency = TimeSpan.FromSeconds(expirationSeconds),
                };

                Options.Create(memoryCacheOptions);
            };

            services.AddMemoryCache(setupOptions);

            var provider = services.BuildServiceProvider();

            return provider;
        }
    }
}