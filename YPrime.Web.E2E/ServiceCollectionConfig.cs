using BoDi;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SolidToken.SpecFlow.DependencyInjection;
using System;
using System.Linq;
using System.Security.Principal;
using TechTalk.SpecFlow;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;
using YPrime.Data.Study;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Models.Api;

namespace YPrime.Web.E2E
{
    public class ServiceCollectionConfig
    {
        private const string EnvironmentSettingName = "AppEnvironment";
        private const string StudyIdSettingName = "StudyId";
        private const string StudyDbContextSettingName = "StudyDbContext";
        private const string StudyBuilderApiUrlSettingName = "StudyBuilderApiUrl";
        private const string PortalUrlSettingName = "PortalUrl";
        private const string ApiUrlSettingName = "ApiUrl";
        private const string SSOUrlSettingName = "SSOUrl";
        private const string NotificationScheduleUrlName = "NotificationScheduleUrl";
        private const string Auth0ClientIdName = "Auth0ClientId";
        private const string Auth0ClientSecretName = "Auth0ClientSecret";
        private const string Auth0AudienceName = "Auth0Audience";
        private const string Auth0UrlName = "Auth0Url";


        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            var settings = BuildSettings();
            services.AddSingleton(settings);

            var serviceSettings = BuildServiceSettings();
            services.AddSingleton<IServiceSettings>(serviceSettings);

            //factory for E2E Settings
            services.AddScoped((_) =>
            {
                var jsonSettings = GetScenarioContextSettings(services);
                return jsonSettings;
            });

            var testData = new TestData();
            var apiTestData = new ApiTestData();

            services.AddSingleton(testData);
            services.AddSingleton(apiTestData);

            services.AddTransient<E2ERepository>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IConfigurationVersionService, ConfigurationVersionService>();
            services.AddTransient<IQuestionnaireService, QuestionnaireService>();
            services.AddTransient<ISubjectInformationService, SubjectInformationService>();
            services.AddTransient<IStudyRoleService, StudyRoleService>();
            services.AddTransient<IStudySettingService, StudySettingService>();
            services.AddTransient<ICorrectionTypeService, CorrectionTypeService>();
            services.AddTransient<ICorrectionWorkflowService, CorrectionWorkflowService>();
            services.AddTransient<IPatientStatusService, PatientStatusService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IVisitService, VisitService>();
            services.AddTransient<ITranslationService, TranslationService>();

            //factory for db context
            services.AddScoped<IStudyDbContext, StudyDbContext>((_) =>
            {
                var jsonSettings = GetScenarioContextSettings(services);

                var context = new StudyDbContext(jsonSettings.StudyDbContext, jsonSettings.CurrentUser);
                return context;
            });

            //Scoped services
            services.AddScoped<ISessionService, SessionService>();

            services.AddHttpClient("e2eHttpClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(settings.ApiUrl);
            });

            services.AddHttpClient("configHttpClient", client =>
            {
                var jsonSettings = GetScenarioContextSettings(services);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("StudyId", settings.StudyId.ToString());
                client.BaseAddress = new Uri(jsonSettings.StudyBuilderApiUrl);
            });

            services.AddHttpClient("notificationHttpClient", client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(settings.NotificationScheduleUrl);
            });

            Action<MemoryCacheOptions> setupOptions = (MemoryCacheOptions memoryCacheOptions) =>
            {
                memoryCacheOptions = new MemoryCacheOptions
                {
                    CompactionPercentage = .25,
                    ExpirationScanFrequency = TimeSpan.FromSeconds(1),
                };

                Options.Create(memoryCacheOptions);
            };
            services.AddMemoryCache(setupOptions);

            services.AddSingleton<IServiceCollection>(services);

            return services;
        }

        public static E2ESettings BuildSettings(
            string jsonFileNane = "Appsettings.json",
            string identityUsername = CommonData.DefaultPortalUserName)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(jsonFileNane)
                .Build();

            var identity = new GenericIdentity(identityUsername);
            String[] userRoles = { "YP" };
            var currentUser = new GenericPrincipal(identity, userRoles);

            return new E2ESettings()
            {
                StudyId = Guid.Parse(config[StudyIdSettingName]),
                StudyDbContext = config[StudyDbContextSettingName],
                StudyBuilderApiUrl = config[StudyBuilderApiUrlSettingName],
                PortalUrl = config[PortalUrlSettingName],
                ApiUrl = config[ApiUrlSettingName],
                SSOUrl = config[SSOUrlSettingName],
                NotificationScheduleUrl = config[NotificationScheduleUrlName],
                CurrentUser = currentUser
            };
        }

        public static ServiceSettings BuildServiceSettings(string jsonFileNane = "Appsettings.json")
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(jsonFileNane)
                .Build();

            var settings = new ServiceSettings
            {
                StudyId = Guid.Parse(config[StudyIdSettingName]),
                AuthUrl = config[SSOUrlSettingName],
                StudyBuilderAppEnvironment = config[EnvironmentSettingName],
                SlidingCacheExpirationSeconds = 1,
                HMACAuthSharedKey = config["HMACAuthSharedKey"]
            };

            return settings;
        }

        private static E2ESettings GetScenarioContextSettings(IServiceCollection services)
        {
            var configuredServices = services.BuildServiceProvider();
            var objectContainer = configuredServices.GetService<IObjectContainer>();
            var scenarioContext = objectContainer.Resolve<ScenarioContext>();

            var jsonFileName = scenarioContext.ScenarioInfo.Tags.Any(t => t.ToUpper() == "MOCKSTUDYBUILDER")
                    ? "MockBuilderAppSettings.json"
                    : "Appsettings.json";

            var jsonSettings = BuildSettings(jsonFileName);

            return jsonSettings;
        }
    }
}
