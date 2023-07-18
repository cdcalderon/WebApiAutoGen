using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Models;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class SoftwareReleaseSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly SoftwareReleasePage softwareReleasePage;
        private readonly E2ERepository e2eRepository;

        public SoftwareReleaseSteps(
            ScenarioService scenarioService,
            SoftwareReleasePage softwareReleasePage,
            EditSubjectPage editSubjectPage,
            E2ERepository e2eRepository)
        {
            this.scenarioService = scenarioService;
            this.softwareReleasePage = softwareReleasePage;
            this.e2eRepository = e2eRepository;
        }

        [Given(@"I am on Software Release Management page")]
        public void GivenIAmOnSoftwareReleaseManagementPage()
        {
            scenarioService.ChromeDriver.Navigate().GoToUrl(softwareReleasePage.e2eSettings.PortalUrl);

            //navigating directly to the srm page because the releases have been cleaned up and 
            // the security settings will not allow navigation outside of it
            scenarioService.ChromeDriver.Navigate().GoToUrl(softwareReleasePage.PageUrl);

            //var manageStudyBtn = softwareReleasePage.GetWebElementById("manageStudyBtn");
            //manageStudyBtn.Click();

            //var softwareReleaseBtn = softwareReleasePage.GetWebElementById("softwareRelease");
            //softwareReleaseBtn.Click();
        }

        [Given(@"I refresh the Software Release Management page")]
        public void GivenIRefreshTheSoftwareReleaseManagementPage()
        {
            scenarioService.ChromeDriver.Navigate().GoToUrl(softwareReleasePage.PageUrl);
        }

        [Given(@"""(.*)"" Device ""(.*)"" is assigned to Site ""(.*)""")]
        public void AndDeviceIsAssignedSite(string deviceType, string assetTag, string site)
        {
            e2eRepository.AddDevice(deviceType, assetTag, site);
        }

        [Given(@"Software Release ""(.*)"" has been created with Software Version ""(.*)"" and Configuration Version ""(.*)""")]
        public async Task GivenReleaseIsCreatedWithVersions(string releaseName, string softwareVersion, string configurationVersion)
        {
            e2eRepository.AddSoftwareVersion(softwareVersion);
            await e2eRepository.CreateReleaseWithVersions(releaseName, softwareVersion, configurationVersion);            
        }

        [Given(@"Software Release ""(.*)"" has been created with Software Version ""(.*)"" and Configuration Version ""(.*)"" and assigned to Study Wide")]
        public async Task GivenReleaseIsCreatedWithVersionsAndStudyWide(string releaseName, string softwareVersion, string configurationVersion)
        {
            e2eRepository.AddSoftwareVersion(softwareVersion);
            await e2eRepository.CreateReleaseWithVersions(releaseName, softwareVersion, configurationVersion, true);
        }

        [Given(@"Device ""(.*)"" is assigned to Software Version ""(.*)"" and Configuration Version ""(.*)""")]
        public async Task GivenDeviceIsAssignedVersions(string assetTag, string softwareVersion, string configurationVersion)
        {
            await e2eRepository.SetDeviceSoftwareAndConfigVersions(assetTag, softwareVersion, configurationVersion);
        }

        [Given(@"The following Software Versions are assigned to study Yprime-Sandbox-e2e")]
        [Given(@"The following Software Versions are assigned to study")]
        [When(@"The following Software Versions are assigned to study")]
        [Then(@"The following Software Versions are assigned to study")]
        public void AndSoftwareVesionIsAssignedToStudy(Table table)
        {
            var versions = table.Rows;
            
            foreach (var version in versions)
            {
                e2eRepository.AddSoftwareVersion(version.Values.First());
            }
        }

        [Given(@"Site ""(.*)"" is assigned to Country ""(.*)"" and has site number ""(.*)""")]
        [When(@"Site ""(.*)"" is assigned to Country ""(.*)"" and has site number ""(.*)""")]
        [Then(@"Site ""(.*)"" is assigned to Country ""(.*)"" and has site number ""(.*)""")]
        public async Task AndSiteIsAssignedCountry(string site, string country, string siteNumber)
        {
            await e2eRepository.AddSite(site, country, siteNumber);
        }

        [Given(@"Site ""(.*)"" is assigned to Country ""(.*)"" and has site number ""(.*)"" with ""(.*)"" status")]
        public async Task AndSiteIsAssignedToCountryWithInactiveStatus(string site, string country, string siteNumber, string status)
        {
            await e2eRepository.AddSiteWithInactiveStatus(site, country, siteNumber , status);
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" multi-select dropdown")]
        [Then(@"I select ""(.*)"" from ""(.*)"" multi-select dropdown")]
        [When(@"I select ""(.*)"" from ""(.*)"" multi-select dropdown")]
        public async Task GivenISelectOptionFromMultiDropdown(string value, string control)
        {
            var options = value.Split(",");

            foreach (var option in options)
            {
                var selectedValues = SearchAndGetDropdownSelectedValues(option, control);

                var hasMatchingValue = false;

                foreach (var selected in selectedValues)
                {
                    if (selected.GetAttribute("title") == value)
                    {
                        hasMatchingValue = true;
                        break;
                    }
                }

                Assert.IsTrue(hasMatchingValue);
            } 
        }

        [Given(@"""(.*)"" multi-select dropdown is disabled")]
        [When(@"""(.*)"" multi-select dropdown is disabled")]
        [Given(@"""(.*)"" dropdown is disabled")]
        [When(@"""(.*)"" dropdown is disabled")]
        [Then(@"""(.*)"" dropdown is disabled")]
        [Then(@"""(.*)"" multi-select dropdown is disabled")]
        public async Task GivenDropdownIsDisabled(string control)
        {
            var elemId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).Id;
            var element = softwareReleasePage.GetWebElementById(elemId);

            Assert.IsFalse(element.Enabled);
        }

        [Given(@"""(.*)"" dropdown is empty")]
        public async Task GivenDropdownIsEmpty(string control)
        {
            var elemId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).Id;
            var element = softwareReleasePage.GetWebElementById(elemId);

            string subContainerClass = "#select2-drop:not([style*='display: none'])";
            var isEmpty = !element.FindElements(By.CssSelector(subContainerClass + " .select2-results li.select2-result-selectable")).Any();

            Assert.IsTrue(isEmpty);
        }

        [When(@"""(.*)"" is selected value in ""(.*)"" dropdown")]
        public async Task ThenDropdownDisplaysSelectedValue(string value, string control)
        {
            var selectedValue = softwareReleasePage.GetDropdownSelectedValue(control);

            Assert.AreEqual(selectedValue, value);
        }

        [Then(@"""(.*)"" value is displayed in ""(.*)"" dropdown in order ""(.*)""")]
        public async Task ThenValueIsDisplayedInDropdown(string value, string control, string order)
        {            
            var elemId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).Id;
            var element = softwareReleasePage.GetWebElementById(elemId);
            var optionList = element.FindElement(By.TagName("ul"));
            var options = optionList.FindElements(By.TagName("li"));
            var index = int.Parse(order);
            var option = options[index];

            Assert.AreEqual(option.Text, value);
        }

        [Then(@"""(.*)"" validation message is displayed for ""(.*)""")]
        [Then(@"""(.*)"" validation message is displayed for ""(.*)"" dropdown")]
        public async Task ThenValidationMessageIsDisplayed(string message, string control)
        {
            var elem = softwareReleasePage.GetControlValidationElement(control);

            Assert.IsTrue(elem.Displayed);
            Assert.AreEqual(message, elem.Text);
        }

        [Given(@"a popup is displayed")]
        [Then(@"a popup is displayed")]
        [When(@"a popup is displayed")]
        public async Task ThenPopupIsDisplayed(Table table)
        {
            softwareReleasePage.WaitForPopUp();
            var tblData = table.CreateSet<PopUpMap>();
            foreach (var data in tblData)
            {
                var popup = softwareReleasePage.GetWebElementById("confirmationModal");

                var title = popup.FindElement(By.ClassName("modal-title"));
                var message = popup.FindElement(By.ClassName("modal-body"));
                var footer = popup.FindElement(By.ClassName("modal-footer"));

                var messageText = message.Text.Replace("\n", "").Replace("\r", "");
                var actionButtons = data.ActionButtons.Split(',');
                
                Assert.AreEqual(data.Message, messageText);

                foreach (var btn in actionButtons)
                {
                    if (btn == "Confirm")
                    {
                        var confirmBtn = footer.FindElement(By.ClassName("btn-primary"));
                        Assert.IsTrue(confirmBtn.Displayed);
                        Assert.AreEqual(confirmBtn.Text, btn);
                    }
                    if (btn == "Cancel")
                    {
                        var cancelBtn = footer.FindElement(By.ClassName("btn-secondary"));
                        Assert.IsTrue(cancelBtn.Displayed);
                        Assert.AreEqual(cancelBtn.Text, btn);
                    }
                }

                Assert.AreEqual(title.Text, data.PopUpType);
            }
        }

        [Given(@"Initial Release is displayed in grid")]
        [Given(@"the following data is added to the grid")]
        [When(@"the following data is added to the grid")]
        public async Task ThenDataIsAddedToGrid(Table table)
        {
            softwareReleasePage.WaitForAjax();
            softwareReleasePage.WaitForSpinner();

            var tblData = table.CreateSet<SoftwareReleaseGridMap>().ToList();
            var grid = softwareReleasePage.GetWebElementsByCss("#releaseGrid tbody tr");

            foreach (var row in grid)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                var index = grid.IndexOf(row);
                var dataRow = tblData[index];

                if (dataRow.ReleaseDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[0].Text));
                    var validDate = DateTime.TryParse(gridCells[0].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.ReleaseDate, gridCells[0].Text);
                }

                Assert.AreEqual(dataRow.ReleaseName, gridCells[1].Text);
                Assert.AreEqual(dataRow.SoftwareVersion, gridCells[2].Text);
                Assert.AreEqual(dataRow.ConfigurationVersion, gridCells[3].Text);
                Assert.AreEqual(dataRow.Active, gridCells[4].FindElement(By.TagName("input")).GetAttribute("checked") == "true" ? "ON" : "OFF");
                Assert.AreEqual(dataRow.Required, gridCells[5].Text);
                Assert.AreEqual(dataRow.StudyWide, gridCells[6].Text);
                Assert.AreEqual(dataRow.DeviceType, gridCells[7].Text);
                Assert.AreEqual(dataRow.Countrys, gridCells[8].Text);
                Assert.AreEqual(dataRow.Sites, gridCells[9].Text);
                Assert.AreEqual(dataRow.AssignedReportedConfig, gridCells[10].Text);
                Assert.AreEqual(dataRow.AssignedReportedSoftware, gridCells[11].Text);
            }
        }

        [When(@"I select ""(.*)"" from ""(.*)"" from dropdown")]
        public async Task ThenISelectOptionFromDropdown(string value, string control)
        {
            var elemId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).Id;
            var element = softwareReleasePage.GetWebElementById(elemId);
            var optionList = element.FindElement(By.TagName("ul"));
            var options = optionList.FindElements(By.TagName("li"));
            
            foreach (var option in options)
            {
                if (option.Text == value)
                {
                    option.Click();
                    return;
                }
            }
        }

        [Then(@"""(.*)"" value is displayed in ""(.*)"" multi-select dropdown")]
        public async Task ThenValueIsDisplayedMultiDropdown(string value, string control)
        {
            var selectedValues = SearchAndGetDropdownSelectedValues(value, control);

            var hasMatchingValue = false;

            foreach (var selected in selectedValues)
            {
                if (selected.GetAttribute("title") == value)
                {
                    hasMatchingValue = true;
                    break;
                }
            }

            Assert.IsTrue(hasMatchingValue);
        }

        [Given(@"""(.*)"" multi-select dropdown is empty")]
        [Then(@"""(.*)"" multi-select dropdown is empty")]
        public async Task ThenDropdownIsEmpty(string control)
        {
            var elemContainerId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).FieldContainerElement;
            var elementContainer = softwareReleasePage.GetWebElementById(elemContainerId);

            var selectedValues = softwareReleasePage.GetMultiDropdownSelectedValues(elementContainer);

            Assert.AreEqual(0, selectedValues.Count);
        }

        [Then(@"""(.*)"" value is not displayed in ""(.*)"" multi-select dropdown")]
        public async Task ThenValueIsNotDisplayedInMultiDropdown(string value, string control)
        {
            // sleep is needed to wait for api calls to update dropdowns
            softwareReleasePage.ThreadSleep();

            var selectedValues = SearchAndGetDropdownSelectedValues(value, control);
           
            var hasMatchingValue = false;

            foreach (var selected in selectedValues)
            {
                if (selected.GetAttribute("title") == value)
                {
                    hasMatchingValue = true;
                    break;
                }
            }

            Assert.IsFalse(hasMatchingValue);
        }

        [Then(@"""(.*)"" is displayed for ""(.*)""")]
        public async Task ThenValueIsInTextInput(string value, string control)
        {
            var elemId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).Id;
            var element = softwareReleasePage.GetWebElementById(elemId);

            var inputValue = element.GetAttribute("value");

            Assert.IsTrue(inputValue == value);
        }

        [When(@"I click View Devices link")]
        public async Task WhenIClickViewDevices()
        {
            var element = softwareReleasePage.GetWebElementById("viewDevices");
            element.Click();
        }

        private ReadOnlyCollection<IWebElement> SearchAndGetDropdownSelectedValues(string value, string control)
        {
            var elemContainerId = softwareReleasePage.FieldMaps().Find(f => f.Label == control).FieldContainerElement;
            var elementContainer = softwareReleasePage.GetWebElementById(elemContainerId);

            softwareReleasePage.WaitForAjax();

            var searchBox = elementContainer.FindElement(By.ClassName("select2-search__field"));
            searchBox.SendKeys(value);
            searchBox.SendKeys(Keys.Return);

            softwareReleasePage.ThreadSleep();

            var selectedValues = softwareReleasePage.GetMultiDropdownSelectedValues(elementContainer);

            return selectedValues;
        }

       [Then(@"""(.*)"" is sorted from the ""(.*)"" to the ""(.*)""")]
       [Given(@"""(.*)"" is sorted from the ""(.*)"" to the ""(.*)""")]
       [When(@"""(.*)"" is sorted from the ""(.*)"" to the ""(.*)""")]
        public void ThenTheControlIsSortedBy(string control, string from, string to)
        {

            bool isLatestToOldest = from.ToLower() == "latest" && to.ToLower() == "oldest";
            var page = softwareReleasePage;

            var fieldMaps = page.FieldMaps();
            var fieldMap = fieldMaps.Find(f => f.Label == control);

            if (fieldMap.WaitForElement)
            {
                page.WaitForAjax();
                page.ThreadSleep();
            }

            var element = page.GetWebElementById(fieldMap.Id);
            var options = element.FindElements(By.TagName("li")).Skip(1);

            var currentList = options
                .Select(o => o.Text).ToList();

            var sortedlist = currentList;
            sortedlist.OrderByDescending(c => c);

            Assert.IsTrue(currentList.SequenceEqual(sortedlist), $"{control} is not sorted correctly");
        }


        [Given(@"I click on cross Icon for ""(.*)""")]
        [When(@"I click on cross Icon for ""(.*)""")]
        [Then(@"I click on cross Icon for ""(.*)""")]
        public void WhenIClickOnIconFor(string multiselectDropdownValue)
        {

            IJavaScriptExecutor js = (IJavaScriptExecutor)scenarioService.ChromeDriver;
            var crossIcon = softwareReleasePage.GetWebElement("xpath", "//*[text()='" + multiselectDropdownValue + "']//*[@class='select2-selection__choice__remove']");
            softwareReleasePage.WaitForAjax();
            js.ExecuteScript("arguments[0].click();", crossIcon);
        }

        [Then(@"""(.*)"" value is ""(.*)"" in ""(.*)"" textBox")]
        [Given(@"""(.*)"" value is ""(.*)"" in ""(.*)"" textBox")]
        [When(@"""(.*)"" value is ""(.*)"" in ""(.*)"" textBox")]
        public void ValueIsDisplayedInTextBox(string Value, string visibilty, string multiSelectDropDown)
        {
            int count = softwareReleasePage.GetWebElements("xpath", "//*[text()='" + multiSelectDropDown + "']//..//li[text()='" + Value + "']").Count;

            if (visibilty.Equals("displayed"))
            {
                Assert.IsTrue(count > 0);
            }
            else
            {
                

                Assert.IsTrue(count == 0);
            }
        }

        [Then(@"""(.*)"" is set as ""(.*)"" in db")]
        [When(@"""(.*)"" is set as ""(.*)"" in db")]
        [Given(@"""(.*)"" is set as ""(.*)"" in db")]
        public void VerifyRequiredValueInDB(string releaseName, string requiredValue)
        {
            bool RequiredValue = e2eRepository.GetRequiredValueFromSoftwareRelease(releaseName);

            if (requiredValue.Equals("Required"))
            {
                Assert.IsTrue(RequiredValue);
            }
            else
            {
                Assert.IsFalse(RequiredValue);
            }

        }




    }


}