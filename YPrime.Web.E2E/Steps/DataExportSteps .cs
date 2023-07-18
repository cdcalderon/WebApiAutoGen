using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DataExportSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly DataExportPage dataExportPage;

        public DataExportSteps(
            ScenarioService scenarioService,
            E2ERepository e2eRepository,
            DataExportPage dataExportPage,
            ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.scenarioContext = scenarioContext;
            this.e2eRepository = e2eRepository;
            this.dataExportPage = dataExportPage;
        }

        [Then(@"""(.*)"" error is displayed")]
        public void ThenErrorIsDisplayed(string text)
        {

            dataExportPage.WaitForAjax();
            dataExportPage.WaitForElementIsVisible(By.XPath("//*[@id='createExportForm']/div[1]/ul/li[1]"));
            var elemId = dataExportPage.GetWebElementByXPath("//*[@id='createExportForm']/div[1]/ul/li[1]");
            var expectedElement = elemId.Text.Replace("\r\n", "");
            Assert.AreEqual(text, expectedElement);
        }

    }
}