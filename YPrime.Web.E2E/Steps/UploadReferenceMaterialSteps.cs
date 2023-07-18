using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;
using YPrime.Web.E2E.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class UploadReferenceMaterialSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly UploadReferenceMaterialPage uploadReferenceMaterialPage;

        public UploadReferenceMaterialSteps(
            ScenarioService scenarioService,
            E2ERepository e2eRepository,
            ScenarioContext scenarioContext,
            UploadReferenceMaterialPage uploadReferenceMaterialPage)
        {
            this.scenarioService = scenarioService;
            this.scenarioContext = scenarioContext;
            this.e2eRepository = e2eRepository;
            this.uploadReferenceMaterialPage = uploadReferenceMaterialPage;
        }

        [Given(@"I upload a file with file format ""([^""]*)""")]
        [When(@"I upload a file with file format ""([^""]*)""")]
        [Then(@"I upload a file with file format ""([^""]*)""")]
        [Then(@"I upload existing file with file format ""([^""]*)""")]
        public void ThenIUploadAFileWithFileFormat(string fileFormat)

        {
            IWebElement element = scenarioService.ChromeDriver.FindElement(By.Id("fileUpload"));
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            switch (fileFormat)
            {
                case "PNG":
                    string pngExpectedFilePath = $"{projectDirectory}\\UploadFiles\\PNG_File.png";
                    element.SendKeys(pngExpectedFilePath);
                    break;
                case "PDF":
                    string pdfExpectedFilePath = $"{projectDirectory}\\UploadFiles\\PDF_File.pdf";
                    element.SendKeys(pdfExpectedFilePath);
                    break;
                case "MP4":
                    string mp4ExpectedFilePath = $"{projectDirectory}\\UploadFiles\\MP4_File.mp4"; ;
                    element.SendKeys(mp4ExpectedFilePath);
                    break;
                default:
                    string path = $"{projectDirectory}\\UploadFiles\\PDF_File1.pdf";
                    element.SendKeys(path);
                    break;
                    
            }
        }

        [Then(@"grid menu is displayed with the following functionality for export")]
        public void ThenGridMenuIsDisplayedWithTheFollowingFunctionalityForExport(Table table)
        {
            uploadReferenceMaterialPage.VerifyElementForExport(table);
        }

        [Then(@"grid menu is displayed with the following functionality for Visibilty")]
        public void ThenGridMenuIsDisplayedWithTheFollowingFunctionalityForVisibilty(Table table)
        {
            uploadReferenceMaterialPage.VerifyElementForVisibilty(table);
        }

        [Given(@"""([^""]*)"" is displayed in ""([^""]*)"" textbox field")]
        [When(@"""([^""]*)"" is displayed in ""([^""]*)"" textbox field")]
        [Then(@"""([^""]*)"" is displayed in ""([^""]*)"" textbox field")]
        public void GivenIsDisplayedInUserInputtextboxField(string name, string control)
        {
            uploadReferenceMaterialPage.TextDisplayedInField(name, control);
        }

        [Then(@"""([^""]*)"" is not visible in the grid list")]
        private void VerifyElementVisibilityInGrid(string elementName)
        {
            var elemId = scenarioService.ChromeDriver.FindElement(By.Id("referenceMaterials"));
            var expectedElement = elemId.FindElement(By.TagName("td"));
            Assert.IsFalse(expectedElement.Text.Contains(elementName), $"{elementName} exists on the page");

        }

        [Then(@"""([^""]*)"" is displayed for invalid file")]
        public void ThenIsDisplayedForInvalidFile(string text)
        {
            uploadReferenceMaterialPage.WaitForAjax();
            uploadReferenceMaterialPage.WaitForElementIsVisible(By.Id("formErrors"));
            var elemId = uploadReferenceMaterialPage.GetWebElementById("formErrors");
            var expectedElement = elemId.Text.Replace("\r\n", "");
            Assert.AreEqual(text, expectedElement);
        }

        [Then(@"""([^""]*)"" column of reference material grid is not visible")]
        public void ThenColumnOfReferenceMaterialGridIsNotVisible(string text)
        {
            var elem = scenarioService.ChromeDriver.FindElements(By.CssSelector("div.dataTables_scrollHeadInner th"));

            foreach (var columnElem in elem)
            {
                var colText = columnElem.Text;
                Assert.AreNotEqual(text, colText.ToUpper(), $"{text} visible on the page");

            }

        }

        [Given(@"I click on ""(.*)"" Dropdown")]
        public void GivenIClickOnDropdown(string control)
        {
            var locator = uploadReferenceMaterialPage.FieldMaps().Find(f => f.Label == control);
            uploadReferenceMaterialPage.GetWebElement(locator.LocatorType,locator.LocatorValue).Click();
        }
    }
}

