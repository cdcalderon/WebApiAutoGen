using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.MockServer;

namespace YPrime.Web.E2E
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioService _scenarioService;
        private readonly FeatureContext _featureContext;
        private readonly E2ERepository e2ERepository;
        private const string ApiTestTag = "ApiTest";
        private const string NotificationServiceDownTag = "NotificationServiceDown";
        public static MockServer.MockServer mockServer;
        private readonly ScenarioContext _scenarioContext;
        private readonly E2ESettings e2eSettings;

        public Hooks(
            ScenarioService scenarioService,
            FeatureContext featureContext,
            E2ERepository e2eRepository,
            ScenarioContext scenarioContext)
        {
            _scenarioService = scenarioService;
            _featureContext = featureContext;
            this.e2ERepository = e2eRepository;
            _scenarioContext = scenarioContext;
        }

        [BeforeFeature]
        public static async Task StartMockServerAsync()
        {
            if (mockServer == null)
            {
                mockServer = new MockServer.MockServer();
                await mockServer.ClearMockResources();
                await mockServer.SetupInitialMappings();
            }
        }

        [BeforeScenario(Order = 2)]
        public async Task InitializeChromeDriver()
        {
            e2ERepository.ResetSystemActionStudyRoles();
            //e2ERepository.CacheSystemActionStudyRoles();
            e2ERepository.CleanupE2EData();
            e2ERepository.GiveTestUsersAllSystemActions();


            if (_featureContext.FeatureInfo.Tags.Contains(ApiTestTag))
            {
                Console.WriteLine("Api Test");
            }
            else
            {
                _scenarioService.InitializeChromeDriver();
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            e2ERepository.CleanupE2EData();
            //e2ERepository.ResetSystemActionStudyRoles();
            await mockServer.CleanupNotificationMapping();
            if (_featureContext.FeatureInfo.Tags.Contains(ApiTestTag))
            {
                Console.WriteLine("Api Test");
            }
            else
            {
                _scenarioService.LogoutAndQuit();
            }
        }

        [AfterStep]
        public void AfterStep()
        {
            if (!_scenarioContext.ScenarioInfo.ScenarioAndFeatureTags.Any(t => t.ToUpper() == "APITEST"))
            {
                try
                {
                    CaptureScreenshot();
                }
                catch (StaleElementReferenceException ex)
                {
                    //adding second attempt to grab screenshot in case the element became stale
                    CaptureScreenshot();
                }
            }
        }

        private void CaptureScreenshot()
        {
            string title = String.Concat(_scenarioContext.StepContext.StepInfo.Text.Where(c => !Char.IsPunctuation(c)));
            title = title.Replace("<", "").Replace(">", "").Replace("|", "").Replace("?", "");
            if (title.Length > 150) title = title.Substring(0, 150);
            string screenshotfilename = $"{title}.png";

            var vcs = new VerticalCombineDecorator(new ScreenshotMaker());
            ((IJavaScriptExecutor)_scenarioService.ChromeDriver).ExecuteScript("document.body.style.overflow = 'hidden'");
            _scenarioService.ChromeDriver.TakeScreenshot(vcs).ToMagickImage().Write(screenshotfilename, ImageMagick.MagickFormat.Png);
            ((IJavaScriptExecutor)_scenarioService.ChromeDriver).ExecuteScript("document.body.style.overflow = 'visible'");
            var testContext = _scenarioContext.ScenarioContainer.Resolve<TestContext>();
            testContext.AddResultFile(screenshotfilename);

            //Scroll to the top of the page
            ((IJavaScriptExecutor)_scenarioService.ChromeDriver).ExecuteScript("window.scrollTo(document.body.scrollHeight,0)");
        }
    }
}