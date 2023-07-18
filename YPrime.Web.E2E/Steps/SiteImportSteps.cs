using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Pages;
namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class SiteImportSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly SiteImportPage siteImportPage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;

        public SiteImportSteps(
           ScenarioService scenarioService,
           SiteImportPage siteImportPage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.siteImportPage = siteImportPage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
        }

        [Given(@"I upload a file with file name ""([^""]*)""")]
        [When(@"I upload a file with file name ""([^""]*)""")]
        [Then(@"I upload a file with file name ""([^""]*)""")]
        public void WhenIUploadAFileWithFileName(string fileName)
        {
            IWebElement element = scenarioService.ChromeDriver.FindElement(By.Id("fileImport"));
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string pngExpectedFilePath = $"{projectDirectory}\\UploadFiles\\" + fileName;
            element.SendKeys(pngExpectedFilePath);
        }

        [Then(@"the following message is displayed on the page")]
        public bool ThenTheFollowingMessageIsDisplayedOnThePage(Table table)
        {
            var fields = table.CreateSet<SetMessageValue>();
            foreach (var field in fields)
            {
                var expectedValue = field.Messages;
                var Wait = new WebDriverWait(scenarioService.ChromeDriver, TimeSpan.FromSeconds(5));
                Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.TagName("body")));
                var bodyTag = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
                if (bodyTag.Text.Contains(expectedValue))
                {
                    return true;
                }
                return false;
            }
            return true;

        }

        class SetMessageValue
        {
            public string Messages { get; set; }
        }

        [Given(@"I click on ""([^""]*)"" on ""([^""]*)"" page")]
        [When(@"I click on ""([^""]*)"" on ""([^""]*)"" page")]
        [Then(@"I click on ""([^""]*)"" on ""([^""]*)"" page")]
        public void GivenIClickOnOnPage(string name, string pageName)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.XPath("//span[contains(text(),'" + name + "')]"));
            element.Click();
        }

        [Given(@"I select ""([^""]*)"" from ""([^""]*)"" dropdown on site import page")]
        [When(@"I select ""([^""]*)"" from ""([^""]*)"" dropdown on site import page")]
        [Then(@"I select ""([^""]*)"" from ""([^""]*)"" dropdown on site import page")]
        public void GivenISelectFromDropdownOnSiteImportPage(string value, string control)
        {
           
            var fieldMap = siteImportPage.FieldMaps().Find(f => f.Label == control);
            var element = siteImportPage.GetWebElementById(fieldMap.Id);
            ReadOnlyCollection<IWebElement> options;

            siteImportPage.WaitForAjax();

            if (fieldMap.UiControl == "select_basic")
            {
                var select = element.FindElement(By.TagName("select"));
                options = select.FindElements(By.TagName("option"));
            }
            else if (fieldMap.UiControl == "select_direct")
            {
                options = element.FindElements(By.TagName("option"));
            }
            else if (fieldMap.UiControl == "metro_dropdown")
            {
                var parentElement = element.GetParent().GetParent();
                var optionList = parentElement.FindElement(By.TagName("ul"));
                options = optionList.FindElements(By.TagName("li"));
            }
            else
            {
                //select ui control types can have elements in a html list (ul/li) or within options

                var optionLists = element.FindElements(By.TagName("ul"));

                if (optionLists.Count == 0)
                {
                    options = element.FindElements(By.TagName("option"));
                }
                else
                {
                    var optionList = optionLists.First();
                    options = optionList.FindElements(By.TagName("li"));
                }
            }

            foreach (var option in options)
            {
                var optionText = option.Text;
                if (optionText.Replace(" ", "") == value.Replace(" ", ""))
                {
                    option.Click();
                    break;
                }
            }
        }
    }
}
