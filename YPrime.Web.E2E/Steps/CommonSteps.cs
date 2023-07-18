using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models.Models;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Pages;
using YPrime.Web.E2E.Utilities;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class CommonSteps
    {       
        public static readonly List<string> DefaultDropdownValues = new List<string> { "Please Select" };

        private readonly TestData testData;

        private readonly ScenarioService scenarioService;
        private readonly SSOLoginPage ssoLoginPage;
        private readonly DashboardPage dashboardPage;
        private readonly DataCorrectionPage dataCorrectionPage;
        private readonly DataCorrectionConfirmationPage dataCorrectionConfirmationPage;
        private readonly SoftwareReleasePage softwareReleasePage;
        private readonly AddDataCorrectionPage addDataCorrectionPage;
        private readonly SubmitDataCorrectionPage submitDataCorrectionPage;
        private readonly SubjectPage subjectPage;
        private readonly SoftwareVersionPage softwareVersionPage;
        private readonly EditSubjectPage editSubjectPage;
        private readonly AddSubjectPage addSubjectPage;
        private readonly E2ERepository e2eRepository;
        private readonly DeviceManagementPage deviceManagementPage;
        private readonly DeviceDetailsPage deviceDetailsPage;
        private readonly DiaryEntryDetailsPage diaryEntryDetailsPage;
        private readonly RoleManagementPage roleManagementPage;
        private readonly ReportsPage reportsPage;
        private readonly SetPermissionsPage setPermissionsPage;
        private readonly SitePage sitePage;
        private readonly SiteImportPage siteImportPage;
        private readonly AddSitePage addSitePage;
        private readonly ConfirmationOfSiteManagementPage confirmationOfSiteManagementPage;
        private readonly HangfireDashboardPage hangfireDashboardPage;
        private readonly ScheduleAllJobsPage scheduleAllJobsPage;
        private readonly UserInformationPage userInformationPage;
        private readonly ChangePasswordPage changePasswordPage;
        private readonly SubjectInformationPage subjectInformationPage;
        private readonly EnrollmentInformationPage enrollmentInformationPage;
        private readonly UploadReferenceMaterialPage uploadReferenceMaterialPage;
        private readonly ReferenceMaterialPage referenceMaterialPage;
        private readonly BulkEditSitePage bulkEditSitePage;
        private readonly ManageRoleAnalyticsPage manageRoleAnalyticsPage;
        private readonly ManageRoleSubscriptionsPage manageRoleSubscriptionsPage;
        private readonly SavedEmails savedEmails;
        private readonly ScenarioContext scenarioContext;
        private readonly List<BasePage> pages;
        private readonly DataExportPage dataExportPage;
        private readonly WebBackupPage webBackupPage;
        private readonly AnalyticsPage analyticsPage;

        public CommonSteps(
            ScenarioService scenarioService,
            ScenarioContext scenarioContext,
            SSOLoginPage ssoLoginPage,
            DataCorrectionPage dataCorrectionPage,
            SoftwareReleasePage softwareReleasePage,
            DashboardPage dashboardPage,
            AddDataCorrectionPage addDataCorrectionPage,
            SubjectPage subjectPage,
            EditSubjectPage editSubjectPage,
            E2ERepository e2eRepository,
            SubmitDataCorrectionPage submitDataCorrectionPage,
            DataCorrectionConfirmationPage dataCorrectionConfirmationPage,
            DeviceManagementPage deviceManagementPage,
            DeviceDetailsPage deviceDetailsPage,
            DiaryEntryDetailsPage diaryEntryDetailsPage,
            AddSubjectPage addSubjectPage,
            SoftwareVersionPage softwareVersionPage,
            RoleManagementPage roleManagementPage,
            TestData testData,
            ReportsPage reportsPage,
            SetPermissionsPage setPermissionsPage,
            SitePage sitePage,
            SiteImportPage siteImportPage,
            AddSitePage addSitePage,
            ConfirmationOfSiteManagementPage confirmationOfSiteManagementPage,
            HangfireDashboardPage hangfireDashboardPage,
            ScheduleAllJobsPage scheduleAllJobsPage,
            UserInformationPage userInformationPage,
            ChangePasswordPage changePasswordPage,
            SubjectInformationPage subjectInformationPage,
            EnrollmentInformationPage enrollmentInformationPage,
            BulkEditSitePage bulkEditSitePage,
            UploadReferenceMaterialPage uploadReferenceMaterialPage,
            ManageRoleAnalyticsPage manageRoleAnalyticsPage,
            ManageRoleSubscriptionsPage manageRoleSubscriptionsPage,
            SavedEmails savedEmails,
            ReferenceMaterialPage referenceMaterialPage,
            DataExportPage dataExportPage,
            WebBackupPage webBackupPage,
            AnalyticsPage analyticsPage
            )
        {
            this.scenarioService = scenarioService;
            this.ssoLoginPage = ssoLoginPage;
            this.dashboardPage = dashboardPage;
            this.dataCorrectionPage = dataCorrectionPage;
            this.softwareReleasePage = softwareReleasePage;
            this.addDataCorrectionPage = addDataCorrectionPage;
            this.softwareVersionPage = softwareVersionPage;
            this.subjectPage = subjectPage;
            this.editSubjectPage = editSubjectPage;
            this.e2eRepository = e2eRepository;
            this.submitDataCorrectionPage = submitDataCorrectionPage;
            this.deviceManagementPage = deviceManagementPage;
            this.deviceDetailsPage = deviceDetailsPage;
            this.dataCorrectionConfirmationPage = dataCorrectionConfirmationPage;
            this.addSubjectPage = addSubjectPage;
            this.testData = testData;
            this.roleManagementPage = roleManagementPage;
            this.diaryEntryDetailsPage = diaryEntryDetailsPage;
            this.reportsPage = reportsPage;
            this.setPermissionsPage = setPermissionsPage;
            this.sitePage = sitePage;
            this.siteImportPage = siteImportPage;
            this.addSitePage = addSitePage;
            this.confirmationOfSiteManagementPage = confirmationOfSiteManagementPage;
            this.hangfireDashboardPage = hangfireDashboardPage;
            this.scheduleAllJobsPage = scheduleAllJobsPage;
            this.userInformationPage = userInformationPage;
            this.changePasswordPage = changePasswordPage;
            this.subjectInformationPage = subjectInformationPage;
            this.enrollmentInformationPage = enrollmentInformationPage;
            this.bulkEditSitePage = bulkEditSitePage;
            this.scenarioContext = scenarioContext;
            this.uploadReferenceMaterialPage = uploadReferenceMaterialPage;
            this.manageRoleAnalyticsPage = manageRoleAnalyticsPage;
            this.manageRoleSubscriptionsPage = manageRoleSubscriptionsPage;
            this.savedEmails = savedEmails;
            this.referenceMaterialPage = referenceMaterialPage;
            this.dataExportPage = dataExportPage;
            this.webBackupPage = webBackupPage;
            this.analyticsPage = analyticsPage;

            pages = new List<BasePage>
            {
                this.ssoLoginPage,
                this.dashboardPage,
                this.dataCorrectionPage,
                this.softwareReleasePage,
                this.addDataCorrectionPage,
                this.subjectPage,
                this.editSubjectPage,
                this.submitDataCorrectionPage,
                this.dataCorrectionConfirmationPage,
                this.deviceManagementPage,
                this.deviceDetailsPage,
                this.diaryEntryDetailsPage,
                this.roleManagementPage,
                this.addSubjectPage,
                this.reportsPage,
                this.setPermissionsPage,
                this.sitePage,
                this.siteImportPage,
                this.addSitePage,
                this.softwareVersionPage,
                this.confirmationOfSiteManagementPage,
                this.hangfireDashboardPage,
                this.scheduleAllJobsPage,
                this.userInformationPage,
                this.changePasswordPage,
                this.subjectInformationPage,
                this.enrollmentInformationPage,
                this.bulkEditSitePage,
                this.uploadReferenceMaterialPage,
                this.manageRoleAnalyticsPage,
                this.manageRoleSubscriptionsPage,
                this.savedEmails,
                this.referenceMaterialPage,
                this.dataExportPage,
                this.webBackupPage,
                this.analyticsPage
            };
        }





        [Given(@"I have logged in with user ""(.*)"", password ""(.*)""")]
        [When(@"I have logged in with user ""(.*)"", password ""(.*)""")]
        [Then(@"I have logged in with user ""(.*)"", password ""(.*)""")]
        public void IHaveLoggedInWithUserAndPassword(string user, string password)
        {
            LogInWithUser(user, password);
        }

        [Given(@"I have logged in with user ""(.*)""")]
        [When(@"I have logged in with user ""(.*)""")]
        [Then(@"I have logged in with user ""(.*)""")]
        [Given(@"I am logged in as ""(.*)""")]
        [Then(@"I am logged in as ""(.*)""")]
        [When(@"I am logged in as ""(.*)""")]
        public void IAmLoggedInAsUser(string userMappingName)
        {
            var matchingUser = CommonData
                .UserMappings
                .FirstOrDefault(u => u.MappingName == userMappingName);

            if (matchingUser == null)
            {
                throw new NotImplementedException($"{userMappingName} has not been implemented");
            }

            LogInWithUser(
                matchingUser.Username, 
                matchingUser.Password);
        }

        [Given(@"I am on ""(.*)"" page")]
        [Then(@"I am on ""(.*)"" page")]
        [When(@"I am on ""(.*)"" page")]
        public void GivenIAmOnPage(string pageName)
        {
            var page = FindPageByName(pageName);

            if (!(page is SoftwareReleasePage))
            {
                e2eRepository.AddBaseSoftwareRelease();
            }

            page.NavigateToPage();
            page.ThreadSleep();
        }

        [Given(@"I am back on ""(.*)"" page")]
        [Then(@"I am back on ""(.*)"" page")]
        public void GivenIAmBackOnPage(string pageName)
        {
            var page = FindPageByName(pageName);

            page.NavigateToPage();
            page.ThreadSleep();
        }

        class setColumnName
        {
            public string ColumnName { get; set; }
            public string ButtonName { get; set; }
        }


        [Given(@"I click on ""([^""]*)"" button to generate ""([^""]*)"" in ""([^""]*)"" format file to save in Export Evidence folder")]
        [When(@"I click on ""([^""]*)"" button to generate ""([^""]*)"" in ""([^""]*)"" format file to save in Export Evidence folder")]
        [Then(@"I click on ""([^""]*)"" button to generate ""([^""]*)"" in ""([^""]*)"" format file to save in Export Evidence folder")]
        public void ThenIClickOnButtonToGenerateFormatFile(string control, string fileName, string format)
        {
            var page = FindCurrentPage();
            var fieldMaps = page.FieldMaps();
            var fieldMap = fieldMaps.Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            element.Click();

            FileInfo file = page.GetDownloadFileInfo(fileName, format);

            if (file == null)
                Assert.Fail();
            else
                Assert.AreEqual(file.Name, fileName + format);
        }

        [Given(@"""(.*)"" report visibility status is ""(.*)""")]
        [When(@"""(.*)"" report visibility status is ""(.*)""")]
        [Then(@"""(.*)"" report visibility status is ""(.*)""")]
        public void GivenReportVisibilityStatusIs(string reportName, string status)
        {
            var page = FindCurrentPage();
            var hdrManageStudy = page.GetWebElementById("manageStudyBtn");
            hdrManageStudy.Click();
            var roleManagementLink = page.GetWebElementById("roleManagement");
            roleManagementLink.Click();
            var lblRoleName = page.GetWebElementByXPath("//td[text()='YPrime']");
            Assert.IsTrue(lblRoleName.Displayed);
            var btnsetReport = page.GetWebElementByXPath("//td[text()='YPrime']//parent::tr//td//a[text()='Set Reports']");
            btnsetReport.Click();
            var elementStatus = page.GetWebElementByXPath("//label[text()='" + reportName + "']//parent::h4//input ");
            switch (status)
            {
                case "Enabled":

                    if (!elementStatus.Selected)
                    {

                        var xpathToggleButton = scenarioService.ChromeDriver.FindElement(By.XPath("//label[text()='" + reportName + "']//parent::h4//label[text()='ToggleSubscription']"));
                        xpathToggleButton.Click();
                    }
                    else
                    {
                        break;
                    }

                    break;

                case "Disabled":

                    if (elementStatus.Selected)
                    {
                        var xpathToggleButton = scenarioService.ChromeDriver.FindElement(By.XPath("//label[text()='" + reportName + "']//parent::h4//label[text()='ToggleSubscription']"));
                        xpathToggleButton.Click();

                    }
                    else
                    {
                        break;
                    }
                    break;
            }


        }
        [Given(@"""([^""]*)"" is not displayed for ""([^""]*)"" dropdown")]
        [When(@"""([^""]*)"" is not displayed for ""([^""]*)"" dropdown")]
        [Then(@"""([^""]*)"" is not displayed for ""([^""]*)"" dropdown")]
        public void ElementNotDisplayedInDropdown(string value, string control)
        {
            ReadOnlyCollection<IWebElement> optionsofDropDown;
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            if (fieldMap.UiControl == "metro_dropdown")
            {
                var parentElement = element.GetParent().GetParent();
                var optionList = parentElement.FindElement(By.TagName("ul"));
                optionsofDropDown = optionList.FindElements(By.TagName("li"));

            }

            else
            {
                optionsofDropDown = element.FindElements(By.TagName("option"));
            }

            foreach (IWebElement optionVal in optionsofDropDown)
            {

                Assert.IsFalse(optionVal.Text.Equals(value));
            }

        }


        [Then(@"""([^""]*)"" is displayed for ""([^""]*)"" dropdown")]
        [Given(@"""([^""]*)"" is displayed for ""([^""]*)"" dropdown")]
        [When(@"""([^""]*)"" is displayed for ""([^""]*)"" dropdown")]
        public void ElementDisplayedInDropdown(string value, string control)
        {
            ReadOnlyCollection<IWebElement> optionsofDropDown;
            bool found = false;
            var page = FindCurrentPage();

            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            if (fieldMap.UiControl == "metro_dropdown")
            {
                var parentElement = element.GetParent().GetParent();
                var optionList = parentElement.FindElement(By.TagName("ul"));
                optionsofDropDown = optionList.FindElements(By.TagName("li"));
            }          
            else
            {
                optionsofDropDown = element.FindElements(By.TagName("option"));
            }

            foreach (IWebElement optionVal in optionsofDropDown)
            {
                if (optionVal.Text == value)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
               var controlValue = page.GetDropdownSelectedValue(control);
                bool areEqual = value == controlValue;
                Assert.AreEqual(value, controlValue);
                
                found = areEqual;
            }

            Assert.IsTrue(found);
        }



        [Given(@"I click on ""(.*)"" button")]
        [When(@"I click on ""(.*)"" button")]
        [Then(@"I click on ""(.*)"" button")]
        [Given(@"I click on ""(.*)"" togglebutton")]
        [When(@"I click on ""(.*)"" togglebutton")]
        [Then(@"I click on ""(.*)"" togglebutton")]
        [Given(@"I click on ""(.*)"" dropdown")]
        [When(@"I click on ""(.*)"" dropdown")]
        [Then(@"I click on ""(.*)"" dropdown")]
        [Given(@"I click on ""(.*)"" multi-select dropdown")]
        [When(@"I click on ""(.*)"" multi-select dropdown")]
        [Then(@"I click on ""(.*)"" multi-select dropdown")]
        [Given(@"I click on ""(.*)"" tab")]
        [When(@"I click on ""(.*)"" tab")]
        [Then(@"I click on ""(.*)"" tab")]
        [Given(@"I click on ""(.*)""")]
        [When(@"I click on ""(.*)""")]
        [Then(@"I click on ""(.*)""")]
        public async Task GivenITapButton(string control)
        {
            var page = FindCurrentPage();

            var fieldMaps = page.FieldMaps();
            var fieldMap = fieldMaps.Find(f => f.Label == control);

            if (fieldMap.WaitForElement)
            {
                page.WaitForAjax();
                page.ThreadSleep();
            }

            var element = page.GetWebElementById(fieldMap.Id);

            if (fieldMap.UiControl == "multiselect")
            {
                var elem = softwareReleasePage.GetWebElementsByClass("select2-selection")[fieldMap.Position];
                softwareReleasePage.ExecuteDriverScript("arguments[0].click();", elem);
                softwareReleasePage.ThreadSleep();

                return;
            }
            else if (fieldMap.UiControl == "select_basic")
            {
                var select = element.FindElement(By.TagName("select"));
                if (select.Displayed)
                {
                    select.Click();
                }
                else
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)scenarioService.ChromeDriver;
                    js.ExecuteScript("window.scrollBy(0,500)");
                    element.Click();
                }
                return;
            }
            else if (fieldMap.UiControl == "metro_dropdown")
            {
                // if element is a select, the metro dropdown element should be two levels up
                if (element.TagName == "select")
                {
                    var parentElement = element.GetParent().GetParent();

                    element = parentElement;
                }
            }

            element.Click();
        }

        [Given(@"""([^""]*)"" SiteNumber has isActive set to ""([^""]*)""")]
        public void GivenSiteNumberHasIsActiveSetTo(string control, string value)
        {
            e2eRepository.SetIsActivetoValue(control, value);
        }


        [Then(@"Site Number ""([^""]*)"" is updated in Sites table")]
        public void ThenIsUpdatedInTable(string value)
        {
            var siteNumber = e2eRepository.GetSiteNumberFromSitesTable(value);
            Assert.AreEqual(siteNumber, value);
        }

        [When(@"""([^""]*)"" is updated for isActive in ""([^""]*)"" in Site table")]
        [Then(@"""([^""]*)"" is updated for isActive in ""([^""]*)"" in Site table")]
        public void GivenIsUpdatedForIsActiveInInSiteTable(string value, string siteName)
        {
            var activeValue = e2eRepository.GetActiveValuefromSiteTable(siteName);
            Assert.AreEqual(activeValue, value);
        }
        
        [Given(@"Language ""([^""]*)"" is assigned to ""([^""]*)""")]
        public void GivenLanguageIsAssignedTo(string language, string siteName)
        {
            e2eRepository.SetLanguageForSite(language, siteName);
        }


        [Then(@"I clear text field ""([^""]*)""")]
        public void ThenIClearTextField(string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            element.Clear();
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" checkbox for questionnaire ""(.*)""")]
        [When(@"I select ""(.*)"" from ""(.*)"" checkbox for questionnaire ""(.*)""")]
        [Then(@"I select ""(.*)"" from ""(.*)"" checkbox for questionnaire ""(.*)""")]
        public async Task SelectOptionFromCheckbox(string choice, string question, string questionnaire)
        {
            var value = TransformTableValue(choice);

            var page = FindCurrentPage();

            var questionId = await e2eRepository.GetQuestionIdFromName(questionnaire, question);

            var checkboxAnswers = dataCorrectionPage.GetWebElementsByXPath($"//*[@questionid='{questionId}'][@type='checkbox']");

            page.WaitForAjax();          
           
            foreach (var checkbox in checkboxAnswers)
            {
                var id = checkbox.GetAttribute("id");
                var checkboxLabel = page.GetWebElementByXPath($"//label[@for='" + id + "']");
                page.WaitForAjax();
                if (checkboxLabel.Text == value)
                {
                    checkbox.Click();
                }
            }                     
        }


        [When(@"toggle for ""(.*)"" is ""(.*)"" in DCF")]
        [Given(@"toggle for ""(.*)"" is ""(.*)"" in DCF")]
        public void IActiveTheToggleControlsForDCF(string value, string action)
        {
            var rows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");
            var page = FindCurrentPage();

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));

                if (rowCells.Count > 3)
                {
                    var inputField = rowCells[3].FindElements(By.TagName("label"));

                    var id = inputField.FirstOrDefault().GetAttribute("for");
                    if (id == value)
                    {
                        inputField.FirstOrDefault().Click();
                    }
                }
            }
        }

        [When(@"Current Value for ""(.*)"" in index ""(.*)"" has a strikethrough")]
        [Given(@"Current Value for ""(.*)"" in index ""(.*)"" has a strikethrough")]
        [Then(@"Current Value for ""(.*)"" in index ""(.*)"" has a strikethrough")]
        public void ICurrentValueStrikethroughDCF(string value, string index)
        {
            var rows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");
            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[int.Parse(index) - 1]?.Text == value)
                {
                    var inputtrikethroughField = rowCells[int.Parse(index)].FindElements(By.TagName("label"));
                    var classAttribute = inputtrikethroughField.FirstOrDefault().GetAttribute("class");
                    Assert.IsTrue(classAttribute.Contains("strike-through"));
                }
            }
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" dropdown")]
        [When(@"I select ""(.*)"" from ""(.*)"" dropdown")]
        [Then(@"I select ""(.*)"" from ""(.*)"" dropdown")]
        public async Task GivenISelectOptionFromDropdown(string value, string control)
        {
            value = TransformTableValue(value);

            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            ReadOnlyCollection<IWebElement> options;

            page.WaitForAjax();

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
                var optionText = option.GetAttribute("innerText").Trim();
                if (optionText.Replace(" ", "") == value.Replace(" ", ""))
                {
                    option.Click();
                    break;
                }
            }

            var selectedValue = page.GetDropdownSelectedValue(control);
            Assert.AreEqual(selectedValue.Replace(" ", ""), value.Replace(" ", ""));
        }

        [Given(@"I enter ""([^""]*)"" in ""([^""]*)"" textbox field")]
        [When(@"I enter ""([^""]*)"" in ""([^""]*)"" textbox field")]
        [Then(@"I enter ""([^""]*)"" in ""([^""]*)"" textbox field")]
        public void ThenIEnterInTextboxField(string value, string control)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            page.ClickOnElement(element);
            value = TransformMappedValues(value);
            element.EnterText(value);
        }

        [Then(@"the following choices are displayed in ""([^""]*)"" dropdown")]
        [Given(@"the following choices are displayed in ""([^""]*)"" dropdown")]
        public void GivenTheFollowingChoicesAreDisplayedForDropdown(string control, Table table)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            ReadOnlyCollection<IWebElement> options = null;

            page.WaitForAjax();

            if (fieldMap.UiControl == "select_basic")
            {
                var select = element.FindElement(By.TagName("select"));
                options = select.FindElements(By.TagName("option"));
            }
            else if (fieldMap.UiControl == "select")
            {
                options = element.FindElements(By.TagName("option"));
            }
            else
            {
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
            var fields = table.CreateSet<LabelAndValueSet>();

            for (int i = 0; i < options.Count; i++)
            {
                var actualElement = options[i].GetAttribute("innerText").Trim();
                var expectedValue = fields.ElementAt(i).Value;
                Assert.AreEqual(expectedValue, actualElement);
            }
        }

        [Given(@"""(.*)"" report title is displayed on screen")]
        [When(@"""(.*)"" report title is displayed on screen")]
        [Then(@"""(.*)"" report title is displayed on screen")]
        [Given(@"""(.*)"" text is displayed")]
        [Then(@"""(.*)"" text is displayed")]
        public async Task GivenTextIsDisplayed(string text)
        {
            var page = FindCurrentPage();
            Assert.IsTrue(page.IsTextDisplayed(text));
        }

       
        [Given(@"""(.*)"" text is displayed in the page")]
        [When(@"""(.*)"" text is displayed in the page")]
        [Then(@"""(.*)"" text is displayed in the page")]
        [Then(@"""(.*)"" is displayed in the page")]
        [Then(@"""([^""]*)"" button is displayed in the page")]
        [Then(@"""([^""]*)"" cross is displayed in right corner of page")]
        public void GivenTextIsDisplayedInThePage(string text)
        {
            Assert.IsTrue(IsTextDisplayed(text));
        }

        [Given(@"current date for site number ""(.*)"" is displayed")]
        public void CurrentDateIsDisplayed(string siteNumber)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();

            var siteLocalTime = e2eRepository.GetSiteLocalTime(siteNumber).ToString("dd-MMM-yyyy");
            var isTextDisplayed = IsTextDisplayed(siteLocalTime);
            Assert.IsTrue(isTextDisplayed);
        }

        [Given(@"current date for site number ""(.*)"" is not displayed")]
        public void CurrentDateIsNotDisplayed(string siteNumber)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();

            var siteLocalTime = e2eRepository.GetSiteLocalTime(siteNumber).ToString("dd-MMM-yyyy");
            var isTextDisplayed = IsTextDisplayed(siteLocalTime);
            Assert.IsFalse(isTextDisplayed);
        }

        [Then(@"""([^""]*)"" value is not empty")]
        [Given(@"""([^""]*)"" value is not empty")]
        public string ThenValueIsNotEmpty(string key)
        {
            var page = FindCurrentPage();
            var element = page.GetWebElementByXPath("//span[contains(text(),'" + key + "')]//following::span[1]");
            var value = element.GetAttribute("innerText");
            Assert.IsNotNull(value);
            return value;
        }


        [Then(@"I validate that I am in the Login page")]
        public void ThenIValidateThatIAmInTheLoginPage()
        {
            IsTextDisplayed("Email");
            IsTextDisplayed("Password");
            IsTextDisplayed("Log In");
        }

        [Given(@"I validate I am on the ""(.*)"" page")]
        [Then(@"I validate I am on the ""(.*)"" page")]
        public void ThenIValidateIAmOnThePage(string pageName)
        {
            var page = FindPageByName(pageName);
            bool isUrlMatching = IsCurrentUrlMatchingText(page.PageUrl);
            Assert.IsTrue(isUrlMatching, "Urls are not matching");
        }

        [Then(@"current URL is ""(.*)""")]
        [Given(@"current URL is ""(.*)""")]
        [Then(@"I validate that the current Url contains ""(.*)"" text")]
        public void ThenIValidateThatTheCurrentUrlContainsText(string text)
        {
            Assert.IsTrue(IsCurrentUrlMatchingText(text), text + "Is not contained in the URL");
        }

        /// <summary> Searches for the text in the body of the page without using a PageObject </summary>
        /// <remarks>This method does not use PageObjects. There is a method that does the same in BasePage.cs </remarks>
        public bool IsTextDisplayed(string text)
        {
            var bodyTag = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var bodyString = bodyTag.Text.Replace(" ", "").Trim();
            var value = text.Replace(" ", "").Trim();
            if (bodyString.Contains(value))
            {
                return true;
            }
            return false;
        }

        /// <summary> Validates if the Current URL contains the string/text passed. </summary>
        /// <remarks> This method does not use PageObjects</remarks>
        public bool IsCurrentUrlMatchingText(string Text)
        {
            var currentUrl = scenarioService.ChromeDriver.Url;
            Console.WriteLine("URL from the browser " + currentUrl);
            Console.WriteLine("Text contained in Url " + Text);

            if (currentUrl.Contains(Text))
            {
                return true;
            }
            return false;
        }

        /// <summary> Switches to the window or tab number desired. E.g., 1,2,3.  Starts from 1 (not 0 ) </summary>
        /// <remarks> Usually used after the SwitchToNewWindow Method</remarks>
        public void SwitchToTheTabOrWindowNumber(int index)
        {
            index--;
            scenarioService.ChromeDriver.SwitchTo().Window(scenarioService.ChromeDriver.WindowHandles[index]);
        }

        [StepDefinition(@"I switch to the tab or window number ""(.*)""")]
        public void GivenISwitchToTheTabOrWindowNumber(int index)
        {
            SwitchToTheTabOrWindowNumber(index);
        }

        [When(@"date and time displayed matches ""(.*)"" format")]
        [Then(@"date and time displayed matches ""(.*)"" format")]
        public void GivenDateAndTimeFromElementIdMatchesFormat(string dateFormat)
        {
            var textFromElement = userInformationPage.GetDateAndTime();
            bool isDateValid = userInformationPage.IsDateWithValidFormat(textFromElement, dateFormat);
            Assert.IsTrue(isDateValid, "Date format is not valid. Check the format is written correctly");
        }

        [Then(@"date matches todays UTC date")]
        public void GivenDateMatchesTodaysUTCDate()
        {
            string dateFormat = "dd-MMMM-yyy";
            string textFromElement = userInformationPage.GetDateAndTime();
            string dateFromPage = DateTime.ParseExact(textFromElement, "d-MMMM-yyyy h:mm:ss tt (UTC)", System.Globalization.CultureInfo.InvariantCulture).ToString(dateFormat);
            string todaysDate = DateTime.UtcNow.ToString(dateFormat);
            Assert.AreEqual(dateFromPage, todaysDate, "Dates are not matching");
        }

        [Given(@"version is displayed with a valid format")]
        public void GivenVersionIsDisplayedWithAValidFormat()
        {
            IWebElement element = userInformationPage.StudyVersionText;
            string studyVersionNumber = element.Text.Replace("Version ", "");
            Console.WriteLine(studyVersionNumber);
            bool isSoftwareVersionMatchingFormat = IsSoftwareVersionMatchingFormat(studyVersionNumber);
            Assert.IsTrue(isSoftwareVersionMatchingFormat, "Software Version doesnt match the format XX.XX.XX.XX");
        }

        [Given(@"following values are associated as")]
        [Then(@"following values are associated as")]
        public void ThenFollowingValuesAreAssociatedAs(Table table)
        {
            string expectedValue;
            var tableData = table.CreateSet<ControlValueMap>().ToList();
            var page = FindCurrentPage();
            foreach (var row in tableData)
            {
                var fieldMaps = page.FieldMaps();
                var fieldMap = fieldMaps.Find(f => f.Label == row.Label);
                if (fieldMap.WaitForElement)
                {
                    page.WaitForAjax();
                    page.ThreadSleep();
                }
                var element = page.GetWebElementById(fieldMap.Id);
                if (element == null)
                {
                    continue;
                }

                if (fieldMap.UiControl == "metro_dropdown")
                {
                    expectedValue = page.GetDropdownSelectedValue(row.Label);
                }
                else
                {
                    expectedValue = element.GetAttribute("value");
                }
                if (expectedValue.Length == 0)
                    Assert.IsTrue(true);
                else
                    Assert.AreEqual(expectedValue, row.Value);

            }
        }


        [Then(@"the following fields are enabled")]
        [Given(@"the following columns have the field type within Bulk Edit Site grid as")]
        public void ThenTheFollowingFieldsAreEnabled(Table table)
        {
            var tableData = table.CreateSet<ControlValueMap>().ToList();
            bool blnResult = false;
            var page = FindCurrentPage();
            foreach (var row in tableData)
            {
                var fieldMaps = page.FieldMaps();
                var fieldMap = fieldMaps.Find(f => f.Label == row.Label);
                if (fieldMap.WaitForElement)
                {
                    page.WaitForAjax();
                    page.ThreadSleep();
                }
                var element = page.GetWebElementById(fieldMap.Id);

                if (element == null)
                {
                    continue;
                }

                switch (row.Fieldtype)
                {
                    case "Numberinput":
                    case "Inputtextbox":
                        blnResult = element.Enabled;
                        Assert.IsTrue(blnResult);
                        break;
                    case "Dropdown":
                        if (fieldMap.UiControl == "metro_dropdown")
                        {
                            if (element.TagName == "select")
                            {
                                var parentElement = element.GetParent().GetParent();

                                element = parentElement;
                            }
                        }
                        blnResult = element.Enabled;
                        Assert.IsTrue(blnResult);
                        break;
                    case "Toggle":
                        blnResult = element.Enabled;
                        Assert.IsTrue(blnResult);
                        break;
                    default:
                        break;
                }

            }

        }

        /// <summary> Gets the Software Version (E.g., "5.3.34.2") and returns true if it matches the format "XX.XX.XX.XX" </summary>
        public bool IsSoftwareVersionMatchingFormat(string softwareVersion)
        {
            Regex rgx = new Regex(@"^(\d+\.){3}(\d+)$");
            return rgx.IsMatch(softwareVersion);
        }

        [Given(@"""(.*)"" togglebutton is enabled")]
        [When(@"""(.*)"" togglebutton is enabled")]
        [Then(@"""(.*)"" togglebutton is enabled")]
        public async Task GivenToggleIsEnabled(string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            Assert.IsTrue(element.Selected);
        }

        [Given(@"""(.*)"" togglebutton is disabled")]
        [When(@"""(.*)"" togglebutton is disabled")]
        [Then(@"""(.*)"" togglebutton is disabled")]
        [Given(@"""([^""]*)"" field is disabled")]
        [Then(@"""([^""]*)"" button is not selected")]
        [Then(@"""([^""]*)"" button in the background is disabled")]
        public void GivenToggleIsDisabled(string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            Assert.IsFalse(element.Selected);
        }

        [Then(@"""(.*)"" placeholder is displayed in ""(.*)"" dropdown")]
        [Given(@"""(.*)"" placeholder is displayed in ""(.*)"" dropdown")]
        [When(@"""(.*)"" placeholder is displayed in ""(.*)"" dropdown")]
        public async Task ThenDropdownDisplaysValue(string value, string control)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            var selectedValue = page.GetDropdownSelectedValue(control);

            Assert.AreEqual(selectedValue, value);
        }

        [Given(@"Active history is updated for ""(.*)""")]
        [When(@"Active history is updated for ""(.*)""")]
        [Then(@"Active history is updated for ""(.*)""")]
        public void GivenActiveHistoryIsUpdatedFor(string siteNumber)
        {
            e2eRepository.UpdateSiteActiveHistory(siteNumber);
        }

        [Given(@"I update active history of Site ""(.*)"" for ""(.*)"" report")]
        [When(@"I update active history of Site ""(.*)"" for ""(.*)"" report")]
        [Then(@"I update active history of Site ""(.*)"" for ""(.*)"" report")]
        public void GivenActiveHistoryIsUpdatedFor(string siteNumber,string report)
        {
            e2eRepository.UpdateSiteActiveHistory(siteNumber,report);
        }


        [Then(@"""(.*)"" is displayed under ""(.*)"" header")]
        public async Task ThenToggleIsDisplayedUnderHeader(string control, string header)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            var headerElem = element.FindElement(By.XPath($".//preceding::h2[1]"));

            Assert.AreEqual(headerElem.Text, header);
        }

        [Given(@"Subject ""(.*)"" is assigned to ""(.*)"" Device")]
        public async Task GivenSubjectIsAssignedToDevice(string subjectNumber, string assetTag)
        {
            subjectNumber = TransformTableValue(subjectNumber);

            e2eRepository.AddPatient(subjectNumber);
            e2eRepository.AssignPatientToDevice(subjectNumber, assetTag);
        }

        [Given(@"Subject ""(.*)"" is created")]
        public async Task GivenSubjectIsCreated(string subjectNumber)
        {
            e2eRepository.AddPatient(subjectNumber);
        }

        [Given(@"Subject ""(.*)"" exists in the ""(.*)"" table")]
        public async Task GivenSubjectIsCreatedInTable(string subjectNumber, string table)
        {
            e2eRepository.AddPatient(subjectNumber);
        }

        [Given(@"The following data is saved for the Subject ""(.*)""")]
        public async Task GivenTheFollowingDataIsSavedForTheSubject(string subjectNumber, Table table)
        {
            var tblData = table.CreateSet<LabelAndValueSet>().ToList();
            var patientId = await e2eRepository.GetPatientId(subjectNumber);
            var patient = await e2eRepository.GetPatientById(patientId);
            
            if (patient != null)
            {
                foreach(var data in tblData)
                {
                    switch (data.Label)
                    {
                        case "Language":
                            patient.LanguageId = e2eRepository.GetSiteLanguageId(patient.SiteId, data.Value);
                            break;
                        case "EnrolledDate":
                            if (DateTimeOffset.TryParse(data.Value, out var enrolledDate))
                            {
                                patient.EnrolledDate = enrolledDate;
                            }
                            break;
                        case "CurrentStatus":
                            var patientStatusTypes = await e2eRepository.GetPatientStatuses();
                            patient.PatientStatusTypeId = patientStatusTypes.FirstOrDefault(p => p.Name == data.Value).Id;
                            break;
                        default:
                            break;
                    }
                };

                e2eRepository.UpdatePatient(patient);
            }
        }

        [Given(@"I retrieve unused subject ID")]
        public async Task GivenIRetrieveUnusedSubjectId()
        {
            var nextSubjectNumber = await e2eRepository.GetNextSubjectNumber();
            testData.NextSubjectNumber = nextSubjectNumber;
        }

        [Given(@"""(.*)"" Device ""(.*)"" is assigned to Software Release ""(.*)""")]
        public async Task GivenDeviceIsAssignedToReleae(string deviceType, string assetTag, string releaseName)
        {
            await e2eRepository.SetDeviceSoftwareRelease(deviceType, assetTag, releaseName);
        }

        [Given(@"Site ""(.*)"" is assigned to Software Release ""(.*)""")]
        public async Task GivenSiteIsAssignedToSoftwareRelease(string siteName, string releaseName)
        {
            await e2eRepository.SetSoftwareReleaseSite(releaseName, siteName);
        }

        [Given(@"Country ""(.*)"" is assigned to Software Release ""(.*)""")]
        public async Task GivenCountryIsAssignedToSoftwareRelease(string countryName, string releaseName)
        {
            await e2eRepository.SetSoftwareReleaseCountry(releaseName, countryName);
        }

        [Given(@"""(.*)"" is enabled for Software Release ""(.*)""")]
        public async Task GivenPropertyIsEnabledForSoftwareRelease(string propertyName, string releaseName)
        {
            await e2eRepository.EnableSoftwareReleaseProperty(releaseName, propertyName);
        }

        [Given(@"""(.*)"" button is enabled")]
        [When(@"""(.*)"" button is enabled")]
        [Then(@"""(.*)"" button is enabled")]
        public async Task GivenButtonIsEnabled(string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            Assert.IsTrue(element.Enabled);
        }

        [Given(@"I enter ""(.*)"" in ""(.*)"" inputtextbox field")]
        [When(@"I enter ""(.*)"" in ""(.*)"" inputtextbox field")]
        [Then(@"I enter ""(.*)"" in ""(.*)"" inputtextbox field")]
        public async Task GivenIEnterTextInField(string text, string control)
        {
            var page = FindCurrentPage();

            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            element.EnterText(text);
            var inputValue = element.GetAttribute("value");

            Assert.IsTrue(inputValue.Trim().Equals(text.Trim()));
        }

        [Given(@"""([^""]*)"" displayed in ""([^""]*)"" placeholder")]
        [When(@"""([^""]*)"" displayed in ""([^""]*)"" placeholder")]
        [Then(@"""([^""]*)"" displayed in ""([^""]*)"" placeholder")]
        public void GivenDisplayedInPlaceholder(string text, string control)
        {
            var page = FindCurrentPage();

            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            var inputValue = element.GetAttribute("placeholder");

            Assert.IsTrue(inputValue == text);
        }

        [Given(@"""(.*)"" is displayed in ""(.*)"" inputtextbox field")]
        [Then(@"""(.*)"" is displayed in ""(.*)"" inputtextbox field")]
        public async Task ThenTextIsDisplayedInField(string text, string control)
        {
            var page = FindCurrentPage();

            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            var inputValue = element.GetAttribute("value");

            Assert.IsTrue(inputValue == text);
        }

        [Then(@"I click on ""([^""]*)"" Button")]
        public void ThenIClickOnButton(string resend)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.XPath("//button[contains(text(),'" + resend + "')]"));
            element.Click();
        }

      
        [Then(@"I enter ""([^""]*)"" in ""([^""]*)"" inputBox field")]
        public void ThenIEnterInInputBoxField(string value, string control)
        {
            ReadOnlyCollection<IWebElement> element = scenarioService.ChromeDriver.FindElements(By.XPath("//label[text()='" + control + "']//following::input"));
            value = TransformMappedValues(value);
            element.FirstOrDefault().EnterText(value);
        }

        [Then(@"""(.*)"" is displayed in ""(.*)"" details field")]
        public async Task ThenTextIsDisplayedInDetailsField(string text, string label)
        {
            var page = FindCurrentPage();

            var element = page.GetWebElementsByClass("dl-horizontal");

            var labels = element[0].FindElements(By.TagName("dt"));
            var fieldValues = element[0].FindElements(By.TagName("dd"));

            var labelMatch = false;
            var valueMatch = false;

            for (var i = 0; i < labels.Count; i++)
            {
                var labelText = labels[i].Text;
                var valueText = fieldValues[i].Text;

                labelMatch = labelText == label;
                valueMatch = valueText == text;

                if (labelMatch && valueMatch)
                {
                    break;
                }
            }

            Assert.IsTrue(labelMatch);
            Assert.IsTrue(valueMatch);
        }

        [Given(@"""(.*)"" button is disabled")]
        [When(@"""(.*)"" button is disabled")]
        [Then(@"""(.*)"" button is disabled")]
        public void GivenButtonIsDisabled(string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            page.WaitForElementIsVisible(By.Id(elemId));
            var element = page.GetWebElementById(elemId);
            var blnResult = element.Enabled;
            Assert.IsFalse(blnResult);
        }
        
        [Given(@"I click on ""(.*)"" link on the top navigation bar")]
        [When(@"I click on ""(.*)"" link on the top navigation bar")]
        [Then(@"I click on ""(.*)"" link on the top navigation bar")]
        public void ClickOnTopNavigationBar(string linkName)
        {
            var page = FindCurrentPage();
            var matchingLink = page.MainMenuLinks[linkName];
            matchingLink.Click();
        }

        [Then(@"""(.*)"" report link is ""(.*)""")]
        [Given(@"""(.*)"" link is ""(.*)""")]
        [When(@"""(.*)"" link is ""(.*)""")]
        [Then(@"""(.*)"" link is ""(.*)""")]
        public void ThenLinkIs(string linkText, string visibility)
        {
            var links = scenarioService.ChromeDriver.FindElements(By.LinkText(linkText));

            if (visibility.ToUpper() == "VISIBLE")
            {
                Assert.IsTrue(links.Any());
            }
            else if (visibility.ToUpper() == "NOT VISIBLE")
            {
                Assert.IsTrue(links.Count == 0);
            }
            else
            {
                Assert.IsFalse(links.Any());
            }
        }

        [Given(@"hamburger menu is displayed with the following functionality for export")]
        [When(@"hamburger menu is displayed with the following functionality for export")]
        [Then(@"hamburger menu is displayed with the following functionality for export")]
        public void ThenHamburgerMenuIsDisplayedWithTheFollowingFunctionalityForExport(Table table)
        {
            var page = FindCurrentPage();
            var fields = table.CreateSet<ControlValueMap>();
            foreach (var field in fields)
            {
                string actualValue;
                var expectedValue = field.ButtonName;
                var elemId = page.FieldMaps().Find(f => f.Label == field.ButtonName).Id;
                var actualElement = page.GetWebElementById(elemId);
                actualValue = actualElement.GetAttribute("Title");
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [Given(@"hamburger menu is displayed with the following functionality for Visibility")]
        [When(@"hamburger menu is displayed with the following functionality for Visibility")]
        [Then(@"hamburger menu is displayed with the following functionality for Visibility")]
        public void ThenHamburgerMenuIsDisplayedWithTheFollowingFunctionalityForVisibility(Table table)
        {
            var fields = table.CreateSet<setColumnName>();
            foreach (var field in fields)
            {
                string actualValue;
                var expectedValue = field.ButtonName;
                var actualElement = scenarioService.ChromeDriver.FindElement(By.LinkText(expectedValue));
                actualValue = actualElement.Text;
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [Given(@"""([^""]*)"" grid column is not visible")]
        [When(@"""([^""]*)"" grid column is not visible")]
        [Then(@"""([^""]*)"" grid column is not visible")]
        public void ThenColumnOfGridIsNotVisible(string text)
        {
            var columns = scenarioService.ChromeDriver.FindElements(By.CssSelector("div.dataTables_scrollHeadInner th"));

            foreach (var column in columns)
            {
                var columnText = column.Text;
                Assert.AreNotEqual(text, columnText.ToUpper(), $"{text} visible on the page");

            }
        }
        [Given(@"Key ""([^""]*)"" is set with value ""([^""]*)"" in ""([^""]*)"" configuration")]
        public void GivenIsSetWithValueInMockFile(string jsonkey, string jsonvalue, string studySettingStudyCustomEndpoint)
        {
            var page = FindCurrentPage();
            var actualValue = page.GetJSONFileValueFromKey(jsonkey, jsonvalue, studySettingStudyCustomEndpoint);
            if (actualValue == null)
                Assert.Fail();
            else
                Assert.AreEqual(actualValue, jsonvalue);

        }

        [Given(@"Id ""([^""]*)"" and languageId ""([^""]*)"" is set with localText ""([^""]*)"" in ""([^""]*)"" configuration")]
        public void GivenIdAndLanguageIdIsSetWithLocalTextInConfiguration(string jsonkey,string languageKey, string jsonvalue, string transalationEndpoint)
        {
            var page = FindCurrentPage();
            var actualValue = page.GetJSONFileValueFromKeytransalation(jsonkey,languageKey, jsonvalue, transalationEndpoint);
            if (jsonvalue.Contains("EncryptedURL"))
            {
                jsonvalue = jsonvalue.Replace("\"", "\'");
                actualValue = actualValue.Replace("\"", "\'");

            }
            if (actualValue == null)
                Assert.Fail();
            else
                Assert.AreEqual(actualValue, jsonvalue);
        }

        [Then(@"""(.*)"" popup is displayed for email")]
        public void GivenPopupIsDisplayed(string popupName)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            page.ThreadSleep();

            var bodyElement = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var modalIsOpen = bodyElement.GetAttribute("class").Contains("modal-open");

            Assert.IsTrue(modalIsOpen);

        }

        [Given(@"""(.*)"" popup is displayed")]
        public void GivenPopupIsDisplayedWithName(string popupName)
        {
            var bootstrapDialog = GetCurrentBootstrapDialogModal();

            ValidateBootstrapModalTitle(bootstrapDialog, popupName);
        }

        [Given(@"I click on ""(.*)"" icon on the ""(.*)"" popup")]
        [Given(@"I click on ""([^""]*)"" icon on the ""([^""]*)"" page")]
        [When(@"I click on ""([^""]*)"" icon on the ""([^""]*)"" page")]
        [Then(@"I click on ""([^""]*)"" icon on the ""([^""]*)"" page")]
        [Then(@"I click on ""(.*)"" icon on the ""(.*)"" popup")]
        public void GivenIClickOnIconOnThePopup(string iconName, string popupName)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == iconName).Id;
            var element = page.GetWebElementById(elemId);
            element.Click();
        }


        [Given(@"""(.*)"" popup is displayed with message ""(.*)""")]
        public async Task GivenPopupIsDisplayedWithNameAndMessage(string popupName, string message)
        {
            var bootstrapDialog = GetCurrentBootstrapDialogModal();

            ValidateBootstrapModalTitle(bootstrapDialog, popupName);

            var messageElement = bootstrapDialog.FindElement(By.ClassName("bootstrap-dialog-message"));

            Assert.AreEqual(message, messageElement.Text);
        }

        [Given(@"I click ""(.*)"" button in the popup")]
        [When(@"I click ""(.*)"" button in the popup")]
        [Then(@"I click ""(.*)"" button in the popup")]

        public async Task GivenIClickButtonInThePopup(string buttonName)
        {
            var bootstrapDialog = GetCurrentBootstrapDialogModal();

            var footer = bootstrapDialog.FindElement(By.ClassName("bootstrap-dialog-footer"));
            var buttons = footer.FindElements(By.ClassName("btn"));

            for (var i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].Text == buttonName)
                {
                    buttons[i].Click();
                    break;
                }
            }
        }

        private void ValidateBootstrapModalTitle(IWebElement modal, string titleText)
        {
            var titleElement = modal.FindElement(By.ClassName("bootstrap-dialog-title"));

            Assert.AreEqual(titleText, titleElement.Text);
        }

        [Given(@"""(.*)"" has value ""(.*)""")]
        [Then(@"""(.*)"" has value ""(.*)""")]
        public async Task InputTextBoxHasValue(string control, string value)
        {
            var page = FindCurrentPage();

            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            var inputValue = element.GetAttribute("value");

            // transform user name if applicable
            value = TransformMappedValues(value);

            Assert.AreEqual(inputValue, value);
        }

        [Given(@"I enter ""(.*)"" for ""(.*)""")]
        [When(@"I enter ""(.*)"" for ""(.*)""")]
        [Then(@"I enter ""(.*)"" for ""(.*)""")]
        public async Task GivenIEnterValueIntoTextInput(string value, string control)
        {
            var page = FindCurrentPage();

            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);

            await Task.Run(() => element.SendKeys(value));
            var inputValue = element.GetAttribute("value");

            Assert.IsTrue(inputValue == value);
        }

        [Given(@"""(.*)"" Configuration Version has the following ""(.*)"" attributes:")]
        public async Task GivenConfigurationHasTheFollowingAttributes(
            string configurationVersion,
            string configurationAttributeType,
            Table table)
        {
            var configs = await e2eRepository.GetSubjectInformationModelsForVersion(configurationVersion);

            var tableData = table.CreateSet<ControlValueMap>().ToList();

            foreach (var row in tableData)
            {
                // skip subject number creation as this will not be included in config data
                if (row.Label.ToUpper() == "SUBJECT")
                {
                    continue;
                }

                var matchingConfig = configs.First(c => c.Name.ToUpper() == row.Label.ToUpper());

                var expectedChoiceType = MapFieldTypeToAttributeChoiceType(row.Fieldtype);

                Assert.AreEqual(expectedChoiceType, matchingConfig.ChoiceType);
            }
        }

        [Given(@"I enter the following data")]
        [Then(@"I enter the following data")]
        [When(@"I enter the following data")]
        public async Task GivenIEnterTheFollowingData(Table table)
        {
            var tableData = table.CreateSet<ControlValueMap>().ToList();

            var page = FindCurrentPage();

            foreach (var row in tableData)
            {
                var element = page.GetOnPageElementByLabel(row.Label);

                if (element == null)
                {
                    continue;
                }

                var value = TransformTableValue(row.Value);

                if (element.TagName == "div")
                {
                    element = GetElementFromDiv(element, row.Fieldtype, row.Value);
                }

                switch (row.Fieldtype)
                {
                    case "Numberinput":
                    case "Inputtextbox":
                        element.EnterText(value);
                        break;
                    case "datepicker":
                        element.Click();
                        var datepicker = scenarioService.ChromeDriver.FindElement(By.ClassName("bootstrap-datetimepicker-widget"));
                        var cells = datepicker.FindElements(By.TagName("td"));

                        // current implementation only works for (Current Date) values
                        var currentDay = DateTime.Now.Day;
                        var dayOne = 1;
                        var currentMonth = false;

                        for (int i = 0; i < cells.Count; i++)
                        {
                            if (cells[i].Text == dayOne.ToString())
                            {
                                currentMonth = true;
                            }

                            if (cells[i].Text == currentDay.ToString() && currentMonth)
                            {
                                cells[i].Click();
                                break;
                            }
                        }
                        break;
                    case "Radio Button":
                        element.Click();
                        break;
                    default:
                        break;

                }
            }
        }

        [Given(@"""(.*)"" permission is ""(.*)""")]
        [When(@"""(.*)"" permission is ""(.*)""")]
        [Then(@"""(.*)"" permission is ""(.*)""")]
        public async Task PermissionIsEnabledOrDisabled(string permission, string status)
        {
            if (status.ToUpper() == "ENABLED")
            {
                await e2eRepository.EnablePermissionForRole(permission);
            }
            else
            {
                await e2eRepository.DisablePermissionForRole(permission);
            }
        }

        [Given(@"the ""(.*)"" permission is ""(.*)"" for ""(.*)""")]
        [Then(@"the ""(.*)"" permission is ""(.*)"" for ""(.*)""")]
        public async Task PermissionIsEnabledOrDisabledForRole(string permission, string status, string role)
        {
            if (status.ToUpper() == "ENABLED")
            {
                await e2eRepository.EnablePermissionForRole(permission, role);
            }
            else
            {
                await e2eRepository.DisablePermissionForRole(permission, role);
            }
        }

        [Then(@"following data is not displayed in ""([^""]*)"" Grid")]
        public void ThenFollowingDataIsNotDisplayedInGrid(string grid, Table table)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == grid);
            var element = page.GetWebElementById(fieldMap.Id);
            bool result = page.VerifyGridData(grid, table, element, out string message);
            Assert.IsFalse(result, message);
        }
        
        [Then(@"User access for ""([^""]*)"" with role ""([^""]*)"" and site ""([^""]*)"" has been removed from study")]
        public async Task GivenUserAccessForWithRoleHasBeenRemovedFromStudy(
            string userName,
            string roleName,
            string siteNumber)
        {
            await e2eRepository.DeleteUserFromStudy(userName, roleName, siteNumber);
        }

        [Given(@"User ""([^""]*)"" with new role ""([^""]*)"" has access to site ""([^""]*)""")]
        public async Task GivenUserWithNewRoleHasAccessToSite(
          String userName,
          string roleName,
          string siteNumber)
        {
            await e2eRepository.AddUserWithNewRoleToSite(userName, roleName, siteNumber);
        }

        [Given(@"User ""(.*)"" with role ""(.*)"" has access to site ""(.*)""")]
        [When(@"User ""(.*)"" with role ""(.*)"" has access to site ""(.*)""")]
        [Then(@"User ""(.*)"" with role ""(.*)"" has access to site ""(.*)""")]
        public async Task UserWithRoleHasAccessToSite(
            string userName,
            string roleName,
            string siteNumber)
        {
            await e2eRepository.AddUserToSite(userName, roleName, siteNumber);
        }

        [Given(@"Version number ""(.*)"" is added with Package path ""(.*)""")]
        public async Task GivenVersionNumberIsAddedWithPackagePath(string VersionNumber, string PackagePath)
        {
            await e2eRepository.AddDataInSoftwareVersion(VersionNumber, PackagePath);
        }

        [Given(@"""(.*)"" link is displayed")]
        [When(@"""(.*)"" link is displayed")]
        [Then(@"""(.*)"" link is displayed")]
        public void ThenLinkIsDisplayed(string linkName)
        {
            var page = FindCurrentPage();
            var element = page.GetWebElementByCss("div.popover-content");
            Assert.IsTrue(element.Displayed);
            Assert.IsTrue(element.Text.Contains(linkName));
        }

        [Given(@"I click on ""(.*)"" link")]
        [When(@"I click on ""(.*)"" link")]
        [Then(@"I click on ""(.*)"" link")]
        [When(@"I click on the ""([^""]*)"" button")]
        public void ThenClickOnLinkText(string linkText)
        {
            var link = scenarioService.ChromeDriver.FindElement(By.LinkText(linkText));
            link.Click();
        }

        [Given(@"I click on ""(.*)"" partial text link")]
        public void ThenClickOnLinkPartialText(string linkText)
        {
            var link = scenarioService.ChromeDriver.FindElement(By.PartialLinkText(linkText));
            link.Click();
        }

        [Given(@"""(.*)"" Button is ""(.*)""")]
        public void VerifyTabVisibilityForButton(string tabName, string status)
        {
            var page = FindCurrentPage();

            var tabField = page
                .FieldMaps()
                .First(fm => fm.FieldType == "Button" && fm.Label == tabName);

            var elements = page.GetWebElementsById(tabField.Id);

            if (status.ToUpper() == "VISIBLE")
            {
                Assert.IsTrue(elements.Count > 0);
            }
            else
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        [Then(@"""(.*)"" tab is ""(.*)""")]
        public void VerifyTabVisibility(string tabName, string status)
        {
            var page = FindCurrentPage();

            var tabField = page
                .FieldMaps()
                .First(fm => fm.FieldType == "Tab" && fm.Label == tabName);

            var elements = page.GetWebElementsById(tabField.Id);

            if (status.ToUpper() == "VISIBLE")
            {
                Assert.IsTrue(elements.Count > 0);
            }
            else
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        [Then(@"""(.*)"" dropdown is displayed in alphabetical order")]
        public void DropdownIsInAlphabeticalOrder(string dropdownName)
        {
            var page = FindCurrentPage();

            var dropdownField = page
                .FieldMaps()
                .First(fm => fm.Label == dropdownName);

            if (dropdownField.UiControl.ToLower() == "select")
            {
                var element = page.GetWebElementById(dropdownField.Id);

                var options = element.FindElements(By.TagName("option"));

                var currentList = options
                    .Select(o => o.Text)
                    .ToList();

                currentList.RemoveAll(item => DefaultDropdownValues.Contains(item));

                var alphabetizedList = currentList
                    .OrderBy(o => o);

                Assert.IsTrue(currentList.SequenceEqual(alphabetizedList), $"{dropdownName} is not alphabetized");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Given(@"""(.*)"" dropdown has placeholder ""(.*)""")]
        [When(@"""(.*)"" dropdown has placeholder ""(.*)""")]
        [Then(@"""(.*)"" dropdown has placeholder ""(.*)""")]
        public void DropDownHasPlaceHolderValue(string control, string value)
        {
            var page = FindCurrentPage();

            var selectedValue = page.GetDropdownSelectedValue(control);

            Assert.AreEqual(selectedValue, value);
        }

        [Then(@"""([^""]*)"" entries under the table grid displayed in ""([^""]*)""")]
        public void ThenEntriesUnderTheTableGridDisplayedIn(string value, string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            var totalrows = element.Text;
            Assert.AreEqual(totalrows.Contains(value + " entries"), true);
        }

        [StepDefinition(@"""(.*)"" Toggle is displayed")]
        [Given(@"""(.*)"" button is displayed")]
        [When(@"""(.*)"" button is displayed")]
        [Then(@"""(.*)"" button is displayed")]
        [Given(@"""(.*)"" dropdown is displayed")]
        [When(@"""(.*)"" dropdown is displayed")]
        [Then(@"""(.*)"" dropdown is displayed")]
        [Then(@"""(.*)"" is displayed")]
        [Given(@"""(.*)"" header is displayed")]
        [When(@"""(.*)"" header is displayed")]
        [Then(@"""(.*)"" header is displayed")]

        public void ElementIsDisplayed(string elementName)
        {
            VerifyElementVisibility(elementName, true);
        }

        [StepDefinition(@"""(.*)"" Toggle is ""(.*)""")]
        [Given(@"""(.*)"" dropdown is ""(.*)""")]
        [When(@"""(.*)"" dropdown is ""(.*)""")]
        [Then(@"""(.*)"" dropdown is ""(.*)""")]
        [Given(@"""(.*)"" text is ""(.*)""")]
        [When(@"""(.*)"" text is ""(.*)""")]
        [Then(@"""(.*)"" text is ""(.*)""")]
        [Given(@"""(.*)"" button is ""(.*)""")]
        [When(@"""(.*)"" button is ""(.*)""")]
        [Then(@"""(.*)"" button is ""(.*)""")]
        [Then(@"""(.*)"" is ""(.*)""")]
        public void ElementVisibility(string elementName, string expectedVisibility)
        {
            var shouldBeVisible = GetExepectedVisibility(expectedVisibility);

            VerifyElementVisibility(elementName, shouldBeVisible);
        }

        [Given(@"""(.*)"" is not ""(.*)""")]
        [When(@"""(.*)"" is not ""(.*)""")]
        [Then(@"""(.*)"" is not ""(.*)""")]
        [Given(@"""(.*)"" dropdown is not ""(.*)""")]
        [When(@"""(.*)"" dropdown is not ""(.*)""")]
        [Then(@"""(.*)"" dropdown is not ""(.*)""")]
        [Then(@"""([^""]*)"" column of subject index grid is not ""(.*)""")]
        [Given(@"""(.*)"" Input box is not ""(.*)""")]
        [Then(@"""(.*)"" header is not ""(.*)""")]
        [Given(@"""(.*)"" button is not ""(.*)""")]
        [Then(@"""(.*)"" button is not ""(.*)""")]
        [When(@"""(.*)"" button is not ""(.*)""")]
        [Then(@"""([^""]*)"" button in the background is ""([^""]*)"" and greyed out")]
       
        public void ElementNegativeVisibility(string elementName, string expectedVisibility)
        {
            var shouldBeVisible = GetExepectedVisibility(expectedVisibility);

            VerifyElementVisibility(elementName, !shouldBeVisible);
        }


        private bool GetExepectedVisibility(string expectedVisibility)
        {
            bool shouldBeVisible;

            switch (expectedVisibility.ToUpper())
            {
                case "VISIBLE":
                    shouldBeVisible = true;
                    break;
                case "NOT VISIBLE":
                    shouldBeVisible = false;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return shouldBeVisible;
        }

        private void VerifyElementVisibility(string elementName, bool shouldBeVisible)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
;           var expectedElementField = page
                .FieldMaps()
                .First(fm => fm.Label == elementName);
            ReadOnlyCollection<IWebElement> expectedElements;
            if (!expectedElementField.LocatorType.Equals("id"))
            {
                expectedElements = page.GetWebElements(expectedElementField.LocatorType, expectedElementField.LocatorValue);
            }
            else
            {
                expectedElements= page.GetWebElementsById(expectedElementField.Id);

            }
             
            if (shouldBeVisible)
            {
                Assert.IsTrue(expectedElements.Count > 0, $"{elementName} does not exist");

                var expectedElement = expectedElements.First();

                Assert.IsTrue(expectedElement.Displayed, $"{elementName} is not displayed");
            }
            else
            {
                Assert.AreEqual(0, expectedElements.Count, $"{elementName} exists on the page");
            }
        }

        private void LogInWithUser(string user, string password)
        {
            scenarioService.ChromeDriver.Navigate().GoToUrl(ssoLoginPage.PageUrl);
            ssoLoginPage.Login(user, password);
            e2eRepository.SetupSessionService();
        }

        private IWebElement GetCurrentBootstrapDialogModal()
        {
            var currentPage = FindCurrentPage();

            currentPage.WaitForDialog();
            var popups = currentPage.GetWebElementsByClass("bootstrap-dialog");

            return popups[0];
        }
      
        private BasePage FindPageByName(string pageName)
        {
            BasePage matchingPage = null;

            foreach (var page in pages)
            {
                if (page.PageName == pageName)
                {
                    matchingPage = page;
                    break;
                }
            }

            return matchingPage;
        }

        private BasePage FindCurrentPage()
        {
            BasePage matchingPage = null;
            var currentUrl = scenarioService.ChromeDriver.Url;

            if (currentUrl.Contains('?'))
            {
                currentUrl = currentUrl.Substring(0, currentUrl.IndexOf('?'));
            }

            var urlPaths = currentUrl.Split('/');

            foreach (var path in urlPaths)
            {
                var isGuid = Guid.TryParse(path, out Guid result);

                if (isGuid)
                {
                    currentUrl = currentUrl.Substring(0, currentUrl.IndexOf(path));
                }
            }

            foreach (var page in pages)
            {
                if (currentUrl.Equals(page.PageUrl))
                {
                    matchingPage = page;
                    break;
                }
            }

            return matchingPage;
        }

        private string MapFieldTypeToAttributeChoiceType(string fieldTypeName)
        {
            string result;

            switch (fieldTypeName.ToUpper())
            {
                case "NUMBERINPUT":
                    result = DataType.NumberAttribute.DisplayName;
                    break;
                case "RADIO BUTTON":
                    result = DataType.ChoicesAttribute.DisplayName;
                    break;
                case "DATEPICKER":
                    result = DataType.DateAttribute.DisplayName;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        private IWebElement GetElementFromDiv(IWebElement divElement, string fieldType, string value = null)
        {
            IWebElement result = divElement;

            var inputs = divElement.FindElements(By.TagName("input"));

            if (fieldType == "Numberinput")
            {
                var numberInput = inputs.First(i =>
                    i.GetAttribute("class").Contains("numeric-control-value") ||
                   i.GetAttribute("class").Contains("numeric-value-control") || i.GetAttribute("class").Contains("text-box single-line"));

                result = numberInput;
            }
            else if (fieldType == "datepicker")
            {
                var dateInput = inputs.First(i => i.GetAttribute("class").Contains("datepicker"));

                result = dateInput;
            }
            else if (fieldType == "Radio Button" && !string.IsNullOrWhiteSpace(value))
            {
                var radioGroup = divElement.FindElement(By.ClassName("radio-group"));
                var radioButtons = radioGroup.FindElements(By.ClassName("btn-radio"));

                foreach (var radioButton in radioButtons)
                {
                    var radioButtonText = radioButton.Text.ToUpper();
                    var valueText = value.ToUpper();

                    if (radioButtonText == valueText)
                    {
                        result = radioButton;
                        break;
                    }
                }
            }

            return result;
        }

        private string TransformTableValue(string tableValue)
        {
            string result;

            switch (tableValue)
            {
                case "<SubjectID>":
                    result = testData.NextSubjectNumber;
                    break;
                case "(Current Date)":
                    result = DateTime.Now.ToString("dd/MMMM/yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    result = tableValue;
                    break;
            }

            return result;
        }

        private string TransformMappedValues(string value)
        {
            var result = value;

            // transform user
            var matchingUser = CommonData
                .UserMappings
                .FirstOrDefault(um => um.MappingName.Equals(value, StringComparison.InvariantCultureIgnoreCase));

            if (matchingUser != null)
            {
                result = matchingUser.Username;
            }

            return result;
        }

        [Given(@"I click on ""(.*)"" link on top navigation bar")]
        [When(@"I click on ""(.*)"" link on top navigation bar")]
        [Then(@"I click on ""(.*)"" link on top navigation bar")]
        [Given(@"I click on ""([^""]*)"" on the top navigation bar")]
        public void GivenIClickOnLinkOnTopNavigationBar(string menuItem)
        {
            ClickOnMenuItem(menuItem);
        }

        public void ClickOnMenuItem(string menuItem)
        {
            var menuGroup = scenarioService.ChromeDriver.FindElements(By.CssSelector("a[menu-group"));
            var menuElement = menuGroup.SingleOrDefault(e => e.Text == menuItem);
            menuElement.Click();
        }

        [Then(@"I click on ""(.*)"" page")]
        public void ThenIClickOnPage(string pageText) => reportsPage.ClickOnPagination("Next");

        [Given(@"I select the given ""(.*)"" from ""(.*)"" dropdown")]
        [When(@"I select the given ""(.*)"" from ""(.*)"" dropdown")]
        [Then(@"I select the given ""(.*)"" from ""(.*)"" dropdown")]
        public async Task GivenISelectTheGivenOptionFromDropdown(string value, string control)
        {
            value = TransformTableValue(value);

            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);

            page.WaitForAjax();

            if (value.Contains("(Current Date)"))
            {
                value = value.Replace("(Current Date)", DateTime.Now.ToString("dd-MMM-yyyy"));
            }

            ReadOnlyCollection<IWebElement> options;
            if (fieldMap.UiControl == "select_basic")
            {
                var select = element.FindElement(By.TagName("select"));
                options = select.FindElements(By.TagName("option"));
            }
            else if (fieldMap.UiControl == "metro_dropdown")
            {
                var parentElement = element.GetParent().GetParent();
                var optionList = parentElement.FindElement(By.TagName("ul"));
                options = optionList.FindElements(By.TagName("li"));
            }
            else
            {
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

            var mergedValue = value.Replace(" ", string.Empty);

            ReadOnlyCollection<IWebElement> altOptions = default;
            foreach (var option in options)
            {
                var optionText = option.GetAttribute("innerText");
                if (optionText.Replace(" ", string.Empty).Contains(mergedValue))
                {
                    if (option.Displayed)
                    {
                        option.Click();
                    }
                    else
                    {
                        var optionList = element.FindElement(By.TagName("ul"));
                        altOptions = optionList.FindElements(By.TagName("li"));
                    }
                    break;
                }
            }

            if (altOptions != null)
            {
                foreach (var option in altOptions)
                {
                    var optionText = option.GetAttribute("innerText");
                    if (optionText.Replace(" ", string.Empty).Contains(mergedValue))
                    {
                        option.Click();
                        break;
                    }
                }
            }

            var selectedValue = page.GetDropdownSelectedValue(control);
            Assert.IsTrue(selectedValue.Replace(" ", string.Empty).Contains(mergedValue));
        }

        [Then(@"the given ""(.*)"" is displayed for ""(.*)"" dropdown")]
        public async Task ThenDropdownDisplaysGivenValue(string value, string control)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            var selectedValue = page.GetDropdownSelectedValue(control);
            Assert.IsTrue(selectedValue.Replace(" ", string.Empty).Contains(value.Replace(" ", string.Empty)));
        }

        [Given(@"""(.*)"" records are displayed in ""(.*)"" data grid")]
        [When(@"""(.*)"" records are displayed in ""(.*)"" data grid")]
        [Then(@"""(.*)"" records are displayed in ""(.*)"" data grid")]
        public void GivenRecordsAreDisplayedDataGrid(string value, string control)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            int rowsize = page.TotalRows(control);
            Assert.AreEqual(Convert.ToInt32(value), rowsize);
        }

        [Then(@"""(.*)"" popup is displayed with message ""(.*)""")]
        public void ThenPopupIsDisplayedWithMessage(string title, string content)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            page.ThreadSleep();

            var bodyElement = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var modalIsOpen = bodyElement.GetAttribute("class").Contains("modal-open");

            Assert.IsTrue(modalIsOpen);

            var modalTitleElement = scenarioService.ChromeDriver.FindElement(By.ClassName("bootstrap-dialog-title"));

            Assert.AreEqual(
                title.Trim(),
                modalTitleElement.Text.Trim(),
                $"Pop up message does not match the expected title");

            var modalMessageElement = scenarioService.ChromeDriver.FindElement(By.ClassName("bootstrap-dialog-message"));

            Assert.AreEqual(
                content.Trim(),
                modalMessageElement.Text.Trim(),
                $"Pop up message does not match the expected Message");
        }

        [StepDefinition(@"I click on ""(.*)"" button in the popup")]
        public void WhenIClickButtonInThePopup(string text)
        {
            var page = FindCurrentPage();

            if (text == "Ok")
            {
                var buttons = page.GetWebElementsByClass("btn");

                foreach (var button in buttons)
                {
                    if (button.Text == text)
                    {
                        button.Click();
                        break;
                    }
                }
            }

            page.ThreadSleep();
        }

        [Given(@"Subject ""(.*)"" is in ""(.*)"" status")]
        [Then(@"Subject ""(.*)"" is in ""(.*)"" status")]
        public async Task ThenSubjectIsInStatus(string subject, string status)
        {
            var patientId = await e2eRepository.GetPatientId(subject);
            var patient = await e2eRepository.GetPatientById(patientId);

            if (patient != null)
            {
                var patientStatusTypes = await e2eRepository.GetPatientStatuses();
                var patientStatusType = patientStatusTypes.FirstOrDefault(p => p.Id == patient.PatientStatusTypeId);

                if (patientStatusType != null)
                {
                    var inactiveStatus = !patientStatusType.IsActive || patientStatusType.IsRemoved;

                    if (status.ToLowerInvariant() == "inactive")
                        Assert.IsTrue(inactiveStatus);
                }
            }
        }

        [Given(@"Notification Schedule endpoint is down")]
        public async Task GivenNotificationScheduleEndpointIsDown()
        {
            await Hooks.mockServer.UpdateNotificationStatusCodeValue((int)System.Net.HttpStatusCode.InternalServerError);         
        }

        [Given(@"Notification Schedule endpoint is up")]
        public async Task GivenNotificationScheduleEndpointIsUp()
        {
            await Hooks.mockServer.UpdateNotificationStatusCodeValue((int)System.Net.HttpStatusCode.OK);
        }
      

        [Then(@"Cancel request is made")]
        public async Task ThenCancelRequestIsMade()
        {
            var result = await Hooks.mockServer.GetLatestCancelNotificationsRequest();

            Assert.IsNotNull(result);
        }

        [Then(@"Update status request is made")]
        public async Task ThenupdateStatusRequestIsMade()
        {
            var result = await Hooks.mockServer.GetLatestUpdateStatusNotificationsRequest();

            Assert.IsNotNull(result);
        }

        [Then(@"Update status request fails")]
        public async Task ThenUpdateRequestFails()
        {
            var lastRequest = await GetLastNotificationRequest();
            Assert.IsTrue(lastRequest != null && lastRequest.ReponseCode != 200);
        }

        [Then(@"Cancel request fails")]
        public async Task ThenCancelRequestFails()
        {
            var lastRequest = await GetLastNotificationRequest();
            Assert.IsTrue(lastRequest != null && lastRequest.ReponseCode != 200);
        }

        public async Task<NotificationRequest> GetLastNotificationRequest()
        {
            var requestLogs = await e2eRepository.GetNotificationRequests();
            return requestLogs.LastOrDefault();
        }

        
        [Given(@"I navigate to ""(.*)"" page")]
        [When(@"I navigate to ""(.*)"" page")]
        [Then(@"I navigate to ""(.*)"" page")]
        public void WhenINavigateToPage(string pageName)
        {
            var page = FindPageByName(pageName);
            page.NavigateToPage();
        }

        [Given(@"I hover on ""(.*)"" button ""(.*)"" message is displayed")]
        [When(@"I hover on ""(.*)"" button ""(.*)"" message is displayed")]
        [Then(@"I hover on ""(.*)"" button ""(.*)"" message is displayed")]
        public void WhenIHoverOnButtonTextIsDisplayed(string element, string message)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == element).Id;
            page.WaitForElementIsVisible(By.Id(elemId));
            var actualElement = page.GetWebElementById(elemId);
            page.HoverOnElement(actualElement);
            IWebElement element1 = page.GetWebElementByXPath("//div[@class='tooltip-inner']");
            string tooltipText = page.GetTextFromElement(element1);
            Assert.AreEqual(message, tooltipText);
        }

        [Given(@"the following button is displayed on the page")]
        [When(@"the following button is displayed on the page")]
        [Then(@"the following button is displayed on the page")]
        [Given(@"the following text is displayed on the page")]
        [When(@"the following text is displayed on the page")]
        [Then(@"the following text is displayed on the page")]
        [Then(@"hamburger button is displayed with the following functionality")]
        public void GivenTheFollowingTextIsDisplayedOnThePage(Table table)
        {
            var fields = table.CreateSet<LabelAndValueSet>();
            var page = FindCurrentPage();
            foreach (var field in fields)
            {
                var expectedValue = field.Value;

                var expectedElementField = page.FieldMaps().Find(f => f.Label == field.Value);
                IWebElement actualElement;
                if (!expectedElementField.LocatorType.Equals("id"))
                {
                    actualElement = page.GetWebElement(expectedElementField.LocatorType, expectedElementField.LocatorValue);
                }
                else
                {
                    actualElement = page.GetWebElementById(expectedElementField.Id);
                }
                Assert.AreEqual(expectedValue, actualElement.Text);
            }
        }

        class LabelAndValueSet
        {
            public string Value { get; set; }
            public string Label { get; set; }
        }

        [Given(@"the generated file name is displayed as ""(.*)"" with ""(.*)"" format")]
        [When(@"the generated file name is displayed as ""(.*)"" with ""(.*)"" format")]
        [Then(@"the generated file name is displayed as ""(.*)"" with ""(.*)"" format")]
        public void GivenTheGeneratedFileNameIsDisplayedAs(string actualfilename, string format)
        {
            var page = FindCurrentPage();
            if (format == ".xlsx")
            {
                var element = page.GetWebElementByXPath("//li[@title ='Excel']");
                element.Click();
            }
            else if (format == ".pdf")
            {
                var element = page.GetWebElementByXPath("//li[@title ='PDF']");
                element.Click();
            }
            else if (format == ".csv")
            {
                var element = page.GetWebElementByXPath("//li[@title ='CSV']");
                element.Click();
            }
            else
            {
                throw new NotImplementedException();
            }
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string folderName = "Downloads";
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + actualfilename + format;
            if (File.Exists(expectedFilePath))
            {
                return;
            }
            else
            {
                Assert.Fail();
            }
        }
        [Then(@"I get a 403 - Forbidden error")]
        public void ThenIGetAHttpError()
        {
            var errors = scenarioService.ChromeDriver.FindElements(By.XPath("//*[text()='403 - Forbidden: Access is denied.']"));
            Assert.IsTrue(errors.Any());
        }

        [Given(@"I close the current tab")]
        public void GivenICloseTheCurrentTab()
        {
            //After closing the method has to switch to another window, otherwise the capture screenshot (AfterStep) method will fail
            scenarioService.ChromeDriver.Close();
            SwitchToTheTabOrWindowNumber(1);
        }

        [When(@"popup is displayed on entering following data in ""(.*)"" field")]
        [Given(@"popup is displayed on entering following data in ""(.*)"" field")]
        public async Task WhenPopupIsDisplayedOnEnteringFollowingDateInFieldAsync(string field, Table table)
        {

            var tableData = table.CreateSet<ControlValueMap>().ToList();
            foreach (var row in tableData)
            {
                await GivenIEnterTextInField(row.Input, field);
                await GivenITapButton("Create");
                ThenPopupIsDisplayedWithMessage(row.PopUpTitle, row.PopUpMessage);
                await GivenIClickButtonInThePopup("Ok");
            }


        }

        [Then(@"Enrolled Date for Patient ""(.*)"" is saved with local Time Zone in DB")]
        [When(@"Enrolled Date for Patient ""(.*)"" is saved with local Time Zone in DB")]
        public void VerifyPatientEnrolledDateIsSavedAsLocalTimeZone(string PatientNumber)
        {
            var enrolleddateFromDB = e2eRepository.GetPatientEnrolledDate(PatientNumber);
            Assert.AreEqual(enrolleddateFromDB.Split(" ")[0], DateTimeOffset.Now.ToString().Split(" ")[0], "Dates are not equal");
            Assert.AreEqual(enrolleddateFromDB.Split(" ")[3], DateTimeOffset.Now.ToString().Split(" ")[3], "TimeZones are not equal");
        }

        [Given(@"""(.*)"" value is ""(.*)""")]
        [When(@"""(.*)"" value is ""(.*)""")]
        [Then(@"""(.*)"" value is ""(.*)""")]
        public void GivenValueIsVisible(string value, string visibility)
        {
            string actualValue = string.Empty;
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == value);
            var element = page.GetWebElementById(fieldMap.Id);
            page.WaitForAjax();
            if (fieldMap.UiControl == "metro_dropdown")
            {
                var parent = element.GetParent().GetParent();
                var selectedElement = parent.FindElement(By.ClassName("selected"));
                actualValue = selectedElement.Text;
            }
            if (visibility.ToUpper() == "VISIBLE")
            {
                Assert.AreEqual(value, actualValue);
            }
        }

        [Given(@"I update subject number ""(.*)"" to ""(.*)""")]
        [When(@"I update subject number ""(.*)"" to ""(.*)""")]
        [Then(@"I update subject number ""(.*)"" to ""(.*)""")]
        public void GivenIUpdateSubjectNumberTo(string OldSubjectNumber, string NewSubjectNumber)
        {
            e2eRepository.UpdateSubjectNumber(OldSubjectNumber, NewSubjectNumber);
        }

        [Given(@"I marked ""(.*)"" as removed")]
        [When(@"I marked ""(.*)"" as removed")]
        [Then(@"I marked ""(.*)"" as removed")]
        public void GivenIMarkedAsRemoved(string SubjectNumber)
        {
            e2eRepository.RemoveSubjectNumber(SubjectNumber);
        }

        [Given(@"I hover on ""(.*)"" ""(.*)"" message is displayed")]
        [Then(@"I hover on ""(.*)"" ""(.*)"" message is displayed")]
        [When(@"I hover on ""(.*)"" ""(.*)"" message is displayed")]
        public void WhenIHoverOnTextIsDisplayed(string element, string inputText)
        {
            var page = FindCurrentPage();
            var elemId = scenarioService.ChromeDriver.FindElement(By.XPath("//*[contains(@id,'" + element + "')]"));
            page.HoverOnElement(elemId);
            Thread.Sleep(3000);
            var elem = scenarioService.ChromeDriver.FindElement(By.XPath("//*[contains(@id,'centreLabel')]"));
            var text = elem.Text;
            Assert.IsTrue(text.Equals(inputText));
        }

        [When(@"The Subject status ""(.*)"" is not ""(.*)"" on the graph")]
        [Given(@"The Subject status ""(.*)"" is not ""(.*)"" on the graph")]
        [Then(@"The Subject status ""(.*)"" is not ""(.*)"" on the graph")]
        public void WhenTheSubjectStatusIsNotOnTheGraph(string elementName, string expectedVisibility)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.XPath("//*[contains(@id,'" + elementName + "')]"));
            var shouldBeVisible = GetExepectedVisibility(expectedVisibility);
            if (shouldBeVisible)
            {
                Assert.IsTrue(element.Displayed);
            }
        }

        [Then(@"all the languages configured are displayed for ""(.*)"" dropdown")]
        public async Task CheckAllLanguagesArePresent(string control)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            var element = page.GetWebElementById(fieldMap.Id);
            var allLangauges = await e2eRepository.GetAllLanguages();

            var parentElement = element.GetParent().GetParent();
            var optionList = parentElement.FindElement(By.TagName("ul"));
            var options = optionList.FindElements(By.TagName("li")).Where(x => x.Text != string.Empty);
            Assert.IsTrue(allLangauges.Count == options.Count());
        }

        class FieldAndValueSet
        {
            public string Value { get; set; }
            public string Field { get; set; }

        }

        [Given(@"following labels are displayed")]
        [When(@"following labels are displayed")]
        [Then(@"following labels are displayed")]
        public void ValidateAllLabels(Table table)
        {
            foreach (var row in table.Rows)
            {
                var dataFound = false;

                var label = row["Label"];
                var labelElems = scenarioService.ChromeDriver.FindElements(By.ClassName("control-label"));

                foreach (var labelElem in labelElems)
                {
                    if (label.Trim().ToUpper() == labelElem.Text.Trim().ToUpper())
                    {
                        dataFound = true;
                        Assert.AreEqual(label, labelElem.Text);
                        break;
                    }
                }
                Assert.IsTrue(dataFound);

            }
        }

        [Given(@"I click on the background screen")]
        [When(@"I click on the background screen")]
        [Then(@"I click on the background screen")]
        public void ThenIClickOnTheBackgroundScreen()
        {
            var page = FindCurrentPage();
            IJavaScriptExecutor js = (IJavaScriptExecutor)scenarioService.ChromeDriver;
            js.ExecuteScript("document.getElementById('loadingSpinner').click()");
        }

        [Given(@"following data is displayed in ""(.*)"" Grid")]
        [Then(@"following data is displayed in ""(.*)"" Grid")]
        [When(@"following data is displayed in ""(.*)"" Grid")]
        public void ThenFollowingDataIsDisplayedInTable(string grid, Table table)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == grid);
            IWebElement element;
            if (grid.Contains("Response"))
            {
                if (!scenarioContext.ContainsKey("TableIndex"))
                {
                    scenarioContext.Add("TableIndex", 0);
                }
                string diaryEntryId = scenarioContext.Get<string>("diaryEntryID");
                fieldMap.Id = fieldMap.Id.Replace("{}", diaryEntryId);
                element = page.GetWebElementById(fieldMap.Id).FindElements(By.TagName("table"))[(int)scenarioContext.Get<int>("TableIndex")];
                scenarioContext["TableIndex"] = (int)scenarioContext.Get<int>("TableIndex") + 1;
                //scenarioContext.Add("TableIndex", (int)scenarioContext.Get<int>("TableIndex") + 1);
            }
            else
            {
                if (!scenarioContext.ContainsKey("TableIndex"))
                {
                    scenarioContext.Add("TableIndex", 0);
                }
                element = page.GetWebElementById(fieldMap.Id);
            }
            bool result = page.VerifyGridData(grid, table, element, out string message);
            Assert.IsTrue(result, message);
        }

        [Then(@"following data is displayed in ""([^""]*)"" section")]
        public void ThenFollowingDataIsDisplayedInSection(string tableName, Table table)
        {
            var tableData = table.CreateSet<LabelAndValueSet>().ToList();
            ReadOnlyCollection<IWebElement> tableElement;
            if (tableName.Equals("emailHeader"))
                tableElement = scenarioService.ChromeDriver.FindElements(By.XPath("(//div[@id='" + tableName + "']//following::table[2])[1]"));
            else if (tableName.Equals("emailBody"))
                tableElement = scenarioService.ChromeDriver.FindElements(By.XPath("(//div[@id='emailHeader']//following::table[2])[2]"));
            else
                tableElement = scenarioService.ChromeDriver.FindElements(By.XPath("//span[contains(text(),'" + tableName + "')]//following::table//tbody"));

            var rows = tableElement[0].FindElements(By.TagName("tr"));

            int i = 0;

            foreach (var row in rows)
            {
                int j = 0;
                bool blnLabel = false, blnValue = false;
                var colData = row.FindElements(By.TagName("td"));

                var label = tableData.ElementAt(i).Label;
                if (label.Contains("Enrollment ID:"))
                {
                    j++; i++; continue;
                }
                if (colData[j].Text.Equals(label))
                    blnLabel = true;

                var value = TransformMappedValues(tableData.ElementAt(i).Value);

                j++;

                if (label.Contains("Date"))
                {
                    string date = DateTime.Now.ToString("dd-MMM-yyyy");
                    var actualValue = CommonUtilities.getDateFromDatetimeUI(colData[j].Text);
                    if (actualValue.Contains(date))
                        blnValue = true;

                }
                else if (colData[j].Text.Trim().Equals(value))
                    blnValue = true;
                i++;
                Assert.AreEqual(blnLabel, blnValue);
            }
        }

        [Then(@"the following text is displayed on the ""(.*)"" page")]
        [When(@"the following text is displayed on the ""(.*)"" page")]
        [Given(@"the following text is displayed on the ""(.*)"" page")]
        public void ThenTheFollowingTextIsDisplayedOnThePage(string pageName, Table table)
        {
            ReadOnlyCollection<IWebElement> element, Labels, FieldValues;
             
            if(pageName =="Site Import")
            {
                element = scenarioService.ChromeDriver.FindElements(By.ClassName("panel-body"));
                Labels = element[0].FindElements(By.TagName("label"));
                FieldValues = element[0].FindElements(By.TagName("span"));
            }
            else
            {
                element = scenarioService.ChromeDriver.FindElements(By.ClassName("dl-horizontal"));
                Labels = element[0].FindElements(By.TagName("dt"));
                FieldValues = element[0].FindElements(By.TagName("dd"));
            }
           
            
            bool result = true;
            Dictionary<string, string> expectedData = TableUtilities.ToDictionary(table);
            Dictionary<string, string> actualData = new Dictionary<string, string>();
            #region Getting all the labels and values from the UI and saving into dictioanry
            for (int i = 0; i < Labels.Count; i++)
            {
                var actualLabel = Labels[i].Text;
                var actualField = FieldValues[i].Text;
                actualData.Add(actualLabel, actualField);
            }
            #endregion
            var expectedLabel = "";
            string message = "";
            #region if expected label is not present fail the case and if label is present but value is not matched fail the case.
            for (int index = 0; index < expectedData.Count; index++)
            {
                expectedLabel = expectedData.ElementAt(index).Key;
                if (actualData.ContainsKey(expectedLabel))
                {
                    var expectedValue = expectedData[expectedLabel];
                    var actualValue = actualData[expectedLabel];
                    #region replacing "current date" string with  actual date and also picking only date from the UI.
                    if (expectedValue.Contains("Current Date"))
                    {
                        string date = DateTime.Now.ToString("dd-MMM-yyyy");
                        expectedValue = date;
                        actualValue = CommonUtilities.getDateFromDatetimeUI(actualValue);
                    }
                    #endregion
                    if (expectedValue.Equals(actualValue))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        message = $"{expectedValue} value is not matched on the page:: actual value is {actualValue}";
                        break;
                    }
                }
                else
                {
                    result = false;
                    message = $"{expectedLabel} Label is not present on the page";
                    break;
                }
            }
            #endregion
            message = result ? $"Text is successfully verfied on {pageName}" : message;
            Assert.IsTrue(result, message);
        }

        [Given(@"I update Diary Entry ""(.*)"" for ""(.*)"" with following details")]
        public async Task GivenIUpdateDiaryEntryForWithFollowingDetails(string questionnaire, string patient, Table table)
        {
            Dictionary<string,string> values=TableUtilities.ToDictionary(table);
            await e2eRepository.UpdateDiaryEntry( patient,  questionnaire,values);
        }
        [Given(@"I remain on ""([^""]*)"" tab")]
        [When(@"I remain on ""([^""]*)"" tab")]
        [Then(@"I remain on ""([^""]*)"" tab")]
        public void GivenTabIsSelected(string control)
        {
            var page = FindCurrentPage();

            var fieldMaps = page.FieldMaps();
            var fieldMap = fieldMaps.Find(f => f.Label == control);

            if (fieldMap.WaitForElement)
            {
                page.WaitForAjax();
                page.ThreadSleep();
            }

            var element = page.GetWebElementById(fieldMap.Id);
            Assert.IsTrue(element.GetAttribute("class").Contains("active"));

        }
        
        [Given(@"Role ""(.*)"" is configured for the study")]
        public async Task VerifyStudyRoleId(string rolename)
        {
            var studyRoleID= e2eRepository.GetStudyRoleID(rolename);
            Assert.IsTrue(studyRoleID.ToString().Length>0);
        }

        [Then(@"I ""(.*)"" ""(.*)"" permission")]
        [When(@"I ""(.*)"" ""(.*)"" permission")]
        [Given(@"I ""(.*)"" ""(.*)"" permission")]

        public void updatePermission(string action, string permission)
        {
            roleManagementPage.updatePermission(action, permission);
        }

        [Then(@"following columns are displayed for ""(.*)"" Grid Menu")]
        public void FollowingColumnsAreDisplayedForGridMenu(string grid, Table table)
        {
            var page = FindCurrentPage();
            var fieldMap = page.FieldMaps().Find(f => f.Label == grid).Id;
            var tableData = table.CreateSet<ControlValueMap>().ToList();
            List<string> expectedColumn = new List<string>();
            foreach (var column in tableData)
            {
                expectedColumn.Add(column.Label);
            }

            ReadOnlyCollection<IWebElement> actualColumn;
                actualColumn = scenarioService.ChromeDriver.FindElements(By.XPath("//*[@id='"+ fieldMap+"']//ancestor::*[@class='table-responsive']//li[contains(@class,'buttons-columnVisibility')]"));
            if(actualColumn.Count ==0)
                actualColumn = scenarioService.ChromeDriver.FindElements(By.XPath("//*[@id='" + fieldMap + "']//ancestor::*[@class='table-responsive']//tr[not(@style)]//th"));
            List<string> actualColumns = new List<string>();
            foreach (var column in actualColumn)
            {
                actualColumns.Add(column.Text);
            }

            Assert.IsTrue(expectedColumn.SequenceEqual(actualColumns));
        }

        [Then(@"""(.*)"" file is opened in new tab")]
        [Given(@"""(.*)"" file is opened in new tab")]
        [When(@"""(.*)"" file is opened in new tab")]
        public void ThenFileIsOpenedInNewTab(string name)
        {
            var referenceMaterialId = e2eRepository.GetReferenceMaterial(name).Id.ToString();
            var page = FindCurrentPage();
            var newURL = $"?referenceMaterialId={referenceMaterialId}";
            bool result = page.VerifyNewTabIsOpened(newURL, true);
            Assert.IsTrue(result, result ? $"New Tab is opened for {name}" : $"New tab is not opened for {name}");
        }

        [When(@"I click on Hamburger icon for Grid")]
        [Then(@"I click on Hamburger icon for Grid")]
        [Given(@"I click on Hamburger icon for Grid")]
        public void WhenIClickOnHamburgerIconForGrid()
        {
            scenarioService.ChromeDriver.FindElement(By.Id("gridMenuButton")).Click();

        }
        [Then(@"""(.*)"" column is not visible in ""(.*)"" grid")]
        public void ThenColoumnIsNotVisibleInGrid(string text, string grid)
        {
            var element = scenarioService.ChromeDriver.FindElements(By.XPath("//thead/tr/th"));
            var count = element.Count;
            for (var i = 0; i < count; i++)
            {
                if (element[i].Text == text)
                {
                    Assert.Fail("'" + text + "' is present in '" + grid + "' grid");
                }

            }
        }

        [Then(@"the following DCF number will ""([^""]*)"" as hyperlink for ""([^""]*)""")]

        public void ThenTheFollowingNumberWillDisplayAsHyperlinkFor(string visibility, string type, Table table)
        {
            var rowCount = table.RowCount;

            var numberList = scenarioService.ChromeDriver.FindElements(By.XPath("//table//tbody//tr//td//a[@href]"));

            if (visibility.Equals("Display"))
            {
                Assert.IsTrue(numberList.Count == rowCount, $" link is not displayed ");

            }
            else if (visibility.Equals("Not Display"))
            {

                Assert.IsTrue(numberList.Count == 0, $" link is displayed");
            }
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" datepicker")]
        [Then(@"I select ""(.*)"" from ""(.*)"" datepicker")]
        public void GivenISelectFromDatepicker(string date, string control)
        {
            var page = FindCurrentPage();
            var fieldMaps = page.FieldMaps();
            var fieldMap = page.FieldMaps().Find(f => f.Label == control);
            IWebElement element;
            element = page.GetWebElementById(fieldMap.Id);
            element.Click();
            ((IJavaScriptExecutor)scenarioService.ChromeDriver).ExecuteScript("arguments[0].value='" + date + "'", element);


        }
        [Then(@"""(.*)"" file is downloaded")]
        public void ThenFileIsDownloaded(string fileName)
        {
            var page = FindCurrentPage();
            FileInfo file = page.GetDownloadFileInfoExclusive(fileName);
            string actualFileName = file.Name;
            actualFileName = actualFileName.Substring(0, actualFileName.IndexOf('-'));
            string expectedFileName = fileName.Substring(0, fileName.IndexOf('*'));
            if (file == null)
                Assert.Fail();
            else
                Assert.AreEqual(actualFileName, expectedFileName);
        }

        [Given(@"""(.*)"" email addresses is not stored in Data base")]
        [When(@"""(.*)"" email addresses is not stored in Data base")]
        [Then(@"""(.*)"" email addresses is not stored in Data base")]
        public void EmailAddressesIsNotStoredInDataBase(string EmailAddress)
        {
            var AcutalEmailRecepient = e2eRepository.SubjectEmailNotStored(EmailAddress);
            if (AcutalEmailRecepient != null)
            {
                Assert.Fail("Email address is present in Email Recepient table");
            }
        }

        [Given(@"""(.*)"" is saved in DB for ""(.*)"" DCF type")]
        [When(@"""(.*)"" is saved in DB for ""(.*)"" DCF type")]
        [Then(@"""(.*)"" is saved in DB for ""(.*)"" DCF type")]
        public void AnswerIsStoredInDataBase(string answerValue, string dcfType)
        {
            var correctionApprovalData = e2eRepository.GetDataCorrectionData(dcfType);
            if (correctionApprovalData != null)
            {
                Assert.AreEqual(answerValue, correctionApprovalData.NewDisplayValue);
            }
        }

        [Given(@"Questionnaires will be completed at time by subjects as follows")]
        public async Task GivenQuestionnairesWillBeCompletedAtTimeBySubjectsAsFollows(Table table)
        {
            await e2eRepository.UpdateQuestionnaireTime(table);
        }

        [Given(@"""(.*)"" page is displayed")]
        [When(@"""(.*)"" page is displayed")]
        [Then(@"""(.*)"" page is displayed")]
        public void ThenPageIsDisplayed(string title)
        {
            Assert.AreEqual(scenarioService.ChromeDriver.Title, title + ": is not correct");
        }

        
        [Given(@"I click on Active toggle button in Grid for ""(.*)""")]
        [When(@"I click on Active toggle button in Grid for ""(.*)""")]
        [Then(@"I click on Active toggle button in Grid for ""(.*)""")]
        public void ClickOnToggleButtonInGrid(string name)
        {
            softwareReleasePage.GetWebElement("xpath", "//*[text()='" + name + "']//.//following-sibling::td//label").Click();
        }

        
       
        [Given(@"Top navigation bar is ""(.*)""")]
        [When(@"Top navigation bar is ""(.*)""")]
        [Then(@"Top navigation bar is ""(.*)""")]
        public void VerifyTopNavigationBarVisibilty(string state)
        {

            var headerCount = softwareReleasePage.GetWebElements("xpath", "//*[@role='navigation']//ul[contains(@class,'navbar-nav')]//li").Count;
            if (state.Equals("Visible"))
            {
                Assert.IsTrue(headerCount > 0);
            }
            else
            {
                Assert.IsTrue(headerCount == 0);
            }

        }

       
       
        [Given(@"Page title is ""(.*)""")]
        [When(@"Page title is ""(.*)""")]
        [Then(@"Page title is ""(.*)""")]
        public void verifyPageTitle(string title)
        {
            Assert.AreEqual(title, softwareReleasePage.GetWebElement("xpath", "//h2").Text);

        }

        
        [Given(@"Active toggle for ""(.*)"" is ""(.*)""")]
        [When(@"Active toggle for ""(.*)"" is ""(.*)""")]
        [Then(@"Active toggle for ""(.*)"" is ""(.*)""")]
        public void VerifyToggleState(string name, string state)
        {
            var checkedValue = softwareReleasePage.GetWebElement("xpath", "//*[text()='" + name + "']//following-sibling::td//input");
            if (state.Equals("Disabled"))
            {
                Assert.IsFalse(checkedValue.Selected);
            }
            else
            {
                Assert.IsTrue(checkedValue.Selected);
            }

        }

        
        [Given(@"""(.*)"" message is displayed in ""(.*)"" PopUp")]
        [When(@"""(.*)"" message is displayed in ""(.*)"" PopUp")]
        [Then(@"""(.*)"" message is displayed in ""(.*)"" PopUp")]
        public void VerifyMessageIsDisplayedInPopup(string message, string popUpName)
        {
            var page = FindCurrentPage();
            var fieldMaps = page.FieldMaps();
            var fieldMap = fieldMaps.Find(f => f.Label == popUpName);
            var element = page.GetWebElementById(fieldMap.Id);
            Assert.AreEqual(message, element.FindElement(By.CssSelector(".modal-content .modal-body")).Text);
        }

        [Given(@"No configuration is assigned to the study")]
        [When(@"No configuration is assigned to the study")]
        [Then(@"No configuration is assigned to the study")]
        public void GivenNoConfigurationVerIsAssignedToTheStudy()
        {
            e2eRepository.DeleteAllConfigurationVersion();
        }


        [Then(@"""(.*)"" error message is displayed")]
        [Given(@"""(.*)"" error message is displayed")]
        [When(@"""(.*)"" error message is displayed")]
        public void VerifyErrorMessageIsDisplayed(string errorMessage)
        {

            Assert.AreEqual(errorMessage, softwareReleasePage.GetWebElement("xpath", "//h2[@class='text-danger']").Text);
        }

        [Given(@"I click at ""(.*)"" from ""(.*)"" dropdown")]
        [When(@"I click at ""(.*)"" from ""(.*)"" dropdown")]
        [Then(@"I click at ""(.*)"" from ""(.*)"" dropdown")]
        public void GivenIClickFromDropdown(string text, string control)
        {
            var page = FindCurrentPage();
            var elemId = page.FieldMaps().Find(f => f.Label == control).Id;
            var element = page.GetWebElementById(elemId);
            page.SelectOptions(element, text, false);
        }

        [Then(@"I change Subject status from ""(.*)"" to ""(.*)"" for ""(.*)"" patient number")]
        public void ThenIChangeSubjectStatusFromToForPatientNumber(string OldSubjectStatus, string NewSubjectStatus, string SubjectNumber)
        {
            e2eRepository.UpdateSubjectStatus(OldSubjectStatus, NewSubjectStatus, SubjectNumber);
        }

        [Then(@"I merge two subjects as one for ""(.*)"" patient number")]
        public void ThenIMergeTwoSubjectsAsOneForPatientNumber(string subjectNumber)
        {
            e2eRepository.DeleteSubjectNumber(subjectNumber);

        }
        [StepDefinition(@"I update ""(.*)"" key with value ""(.*)"" days")]
        public async void IUpdateKeyWithValue(string key, string value)
        {
            await Hooks.mockServer.UpdateStudyCustomValue(key, value);
        }

        [StepDefinition(@"""(.*)"" Toggle is greyed out")]
        public void VerifyElementIsGreyedOut(string elementName)
        {
            var page = FindCurrentPage();
            page.WaitForAjax();
            var expectedElementField = page
                  .FieldMaps()
                  .First(fm => fm.Label == elementName);

            var expectedElement = page.GetWebElementById(expectedElementField.Id);

            Assert.IsNotNull(expectedElement.GetAttribute("disabled"), $"{elementName} is not greyed out");
        }
        [Then(@"I wait for ""(.*)"" seconds")]
        public void ThenIWaitForSeconds(double WaitTimeInSeconds)
        {
            double WaitTimeInMilliseconds = TimeSpan.FromSeconds(WaitTimeInSeconds).TotalMilliseconds;
            Thread.Sleep(((int)WaitTimeInMilliseconds));
        }

    }
}
