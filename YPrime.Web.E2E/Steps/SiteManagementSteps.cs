using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class SiteManagementSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SitePage _sitePage;
        private readonly ScenarioService _scenarioService;
        private readonly E2ERepository _e2eRepository;

        public SiteManagementSteps(ScenarioContext scenarioContext, 
            SitePage siteManagementPage,
            E2ERepository e2eRepository,
            ScenarioService scenarioService)
        {
            _scenarioContext = scenarioContext;
            _sitePage = siteManagementPage;
            _scenarioService = scenarioService;
            _e2eRepository = e2eRepository;
        }

        [Given(@"I click on ""(.*)"" from the sites table")]
        [Then(@"I click on ""(.*)"" from the sites table")]
        public void ClickOnItemInTable(string columnValue)
        {         
            var element = _sitePage.GetSiteNumberLink(columnValue);
            if (element != null)
            {
                element.Click();
            }
        }


        [Given(@"I am on ""(.*)"" detail page")]
        [Then(@"I am on ""(.*)"" detail page")]
        public void GivenIAmOnSitePage(string siteName)
        {
            if (siteName == "<SiteName>")
            {
                siteName = (string)_scenarioContext["<SiteName>"];
            }

            var siteId = _e2eRepository.GetSite(siteName).Id;

            var currentUrl = _scenarioService.ChromeDriver.Url;

            var expectedUrl = $"{_sitePage.PageUrl}/AddEdit/{siteId.ToString().ToLower()}";

            Assert.AreEqual(currentUrl, expectedUrl);
        }

        [Given(@"I have multiple languages set up in the study")]
        [Then(@"I have multiple languages set up in the study")]
        public async Task LoadMultipleLangaugesForStudy()
        {            
            await Hooks.mockServer.LanguageEndpoint("LanguagesMultipleEndpoint.json");       
        }
    }
}
