using TechTalk.SpecFlow;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class WebBackupSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly WebBackupPage webBackupPage;

        public WebBackupSteps(
           ScenarioService scenarioService,
           WebBackupPage webBackupPage)
        {
            this.scenarioService = scenarioService;
            this.webBackupPage = webBackupPage;
        }

        [StepDefinition(@"I verify Launch Web Backup button is ""([^""]*)"" for site ""([^""]*)""")]
        public void IVerifyLaunchWebBackupButtonForSite(string isEnabled, string site)
        {
            webBackupPage.VerifyLaunchWebBackupButton(isEnabled == "enabled" ? true : false, site);
        }

        [StepDefinition(@"I click Launch Web Backup button for site ""([^""]*)""")]
        public void IClickButtonForSite(string site)
        {
            webBackupPage.ClickLaunchWebBackupForSite(site);
        }
    }
}
