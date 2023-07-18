using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models.Api;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class EditSubjectSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly EditSubjectPage editSubjectPage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly ApiTestData apiTestData;

        public EditSubjectSteps(
           ApiTestData apiTestData,
           ScenarioService scenarioService,
           EditSubjectPage editSubjectPage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.editSubjectPage = editSubjectPage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
            this.apiTestData = apiTestData;
        }

        [Given(@"I am on Subject ""(.*)"" page")]
        [When(@"I am on Subject ""(.*)"" page")]
        [Then(@"I am on Subject ""(.*)"" page")]
        public async Task GivenIAmOnSubjectPage(string subjectNumber)
        {
            if (subjectNumber == "<SubjectId>")
            {
                subjectNumber = (string)this.scenarioContext["<SubjectId>"];
            }

            var subjectId = await e2eRepository.GetPatientId(subjectNumber);

            var currentUrl = scenarioService.ChromeDriver.Url;

            var expectedUrl = $"{editSubjectPage.PageUrl}?guid={subjectId.ToString().ToLower()}";

            Assert.AreEqual(currentUrl, expectedUrl);
        }

        [Given(@"I click on subject ""(.*)"" tab")]
        public async Task GivenIClickOnSubjectTab(string tabName)
        {
            var matchingTab = editSubjectPage
                .FieldMaps()
                .First(fm => fm.FieldType == "Tab" && fm.Label == tabName);

            var element = editSubjectPage.GetWebElementById(matchingTab.Id);

            element.Click();
            editSubjectPage.WaitForAjax();
        }

        [Given(@"Caregiver study setting ""(.*)"" is ""(.*)""")]
        public async Task GivenCaregiverSettingIsSetToValue(string studySettingKey, string value)
        {
            var settingValue = value.ToUpper();
            if (settingValue == "DISABLED")
            {
                await Hooks.mockServer.StudySettingEndpoint("StudySettingCaregiverDisabled.json");
            }
            else
            {
                await Hooks.mockServer.StudySettingEndpoint("StudySettingCaregiverEnabled.json");
            }

            var studySettingValue = await e2eRepository.GetStudySettingValue(studySettingKey.Replace(" ", ""));
            var enabled = ConvertStudySettingValue(studySettingValue);
            Assert.AreEqual(enabled, settingValue);
        }

        [When(@"I click on ""(.*)"" diary entry")]
        public async Task WhenIClickOnCompletedDiaryEntry(string diaryEntryName)
        {
            var matchingElement = editSubjectPage
                .GetCompletedDiaryEntryLinkByName(diaryEntryName);

            matchingElement.Click();
        }

        [Given(@"I click on ""(.*)"" button in Subject Management page")]
        public void GivenIClickOnButtonInSubjectManagementPage(string buttonName) =>  editSubjectPage.ClickButtonByLabel(buttonName);

        [Given(@"I have a study configured using pin length of ""(.*)""")]
        public async Task IHaveAStudyConfiguredForPinLength(string length)
        {
            int pinLength;

            if (!int.TryParse(length, out pinLength))
            {
                throw new NotImplementedException();
            }

            const string pinLengthKey = "PatientPINLength";

            await Hooks.mockServer.UpdateStudyCustomValue(pinLengthKey, pinLength.ToString());
        }

        [Given(@"I ""([^""]*)"" on Reset PIN button for subject ""([^""]*)""")]
        [When(@"I ""([^""]*)"" on Reset PIN button for subject ""([^""]*)""")]
        [Then(@"I ""([^""]*)"" on Reset PIN button for subject ""([^""]*)""")]
        [Given(@"Reset PIN button is ""([^""]*)"" for caregiver for subject ""([^""]*)""")]
        [When(@"Reset PIN button is ""([^""]*)"" for caregiver for subject ""([^""]*)""")]
        [Then(@"Reset PIN button is ""([^""]*)"" for caregiver for subject ""([^""]*)""")]
        public void clickonResetPin(string type, string subjectNumber)
        {
            string careGiverforSubject = e2eRepository.getCaregiverForSubject(subjectNumber);
            var actualValueOfResetPin = scenarioService.ChromeDriver.FindElement(By.Id("caregiverResetPin-" + careGiverforSubject));
            switch (type)
            {
                case "click":
                    actualValueOfResetPin.Click();
                    break;

                case "displayed":
                    Assert.IsTrue(actualValueOfResetPin.Text.Contains("Reset PIN"));
                    break;
            }


        }

        [Given(@"pop up ""(.*)"" is displayed with message ""(.*)"" and buttons ""(.*)""")]
        [When(@"pop up ""(.*)"" is displayed with message ""(.*)"" and buttons ""(.*)""")]
        [Then(@"pop up ""(.*)"" is displayed with message ""(.*)"" and buttons ""(.*)""")]
        [Given(@"""([^""]*)"" pop up is displayed with message ""([^""]*)"" and buttons ""([^""]*)""")]
        [When(@"""([^""]*)"" pop up is displayed with message ""([^""]*)"" and buttons ""([^""]*)""")]
        [Then(@"""([^""]*)"" pop up is displayed with message ""([^""]*)"" and buttons ""([^""]*)""")]
        public void PopupIsDisplayedWithMessageAndButtons(string popuptype, string message, string buttonNames)
        {
            editSubjectPage.ThreadSleep();
            var modalBodyElement = "";
            var bodyElement = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var modalIsOpen = bodyElement.GetAttribute("class").Contains("modal-open");
            Assert.IsTrue(modalIsOpen);
            switch (popuptype)
            {
                case "AssignCaregiver":
                    modalBodyElement = scenarioService.ChromeDriver.FindElement(By.CssSelector("div#confirmModal div.modal-body")).Text;
                    Assert.AreEqual(message.Trim(), modalBodyElement.Trim(), $"Pop up message does not match the expected message");
                    foreach (var buttonName in buttonNames.Split(","))
                    {
                        var trimmedName = buttonName.Trim();

                        var fieldMap = editSubjectPage.FieldMaps().First(fm => fm.Label == trimmedName);

                        var buttonElement = editSubjectPage.GetWebElementById(fieldMap.Id);

                        Assert.AreEqual(trimmedName, buttonElement.Text, $"Pop up button {trimmedName} is not found in the pop up.");
                    }
                    break;

                case "UpdatePin":
                    modalBodyElement = scenarioService.ChromeDriver.FindElement(By.ClassName("bootstrap-dialog-message")).Text;
                    Assert.AreEqual(message.Trim(), modalBodyElement.Trim(), $"Pop up message does not match the expected message");
                    foreach (var buttonName in buttonNames.Split(","))
                    {
                        var trimmedName = buttonName.Trim();

                        var buttonElement = scenarioService.ChromeDriver.FindElement(By.CssSelector("div.bootstrap-dialog-footer-buttons button"));

                        Assert.AreEqual(trimmedName, buttonElement.Text, $"Pop up button {trimmedName} is not found in the pop up.");
                    }
                    break;
            }

        }

        [Given(@"Caregiver ""(.*)"" is displayed in the grid")]
        [When(@"Caregiver ""(.*)"" is displayed in the grid")]
        [Then(@"Caregiver ""(.*)"" is displayed in the grid")]
        public void CaregiverIsDisplayedInGrid(string caregiverName)
        {
            editSubjectPage.WaitForAjax();

            var tableElement = scenarioService.ChromeDriver.FindElement(By.CssSelector("table#caregiver"));
            var tableBodyElement = tableElement.FindElement(By.TagName("tbody"));
            var rowElements = tableBodyElement.FindElements(By.TagName("tr"));

            Assert.IsTrue(rowElements.Count > 0);

            var anyMatching = false;

            foreach (var rowElement in rowElements)
            {
                var nameColumnElement = rowElement.FindElements(By.CssSelector("td:nth-child(1)"));

                if (nameColumnElement?.FirstOrDefault()?.Text.ToUpper() == caregiverName.ToUpper())
                {
                    anyMatching = true;
                    break;
                }
            }

            Assert.IsTrue(anyMatching, $"{caregiverName} not displayed in the grid.");
        }

        [Given(@"popup is dismissed")]
        [Then(@"popup is dismissed")]
        [Then(@"popup is not displayed")]
        public void PopupIsDismissed()
        {
            editSubjectPage.ThreadSleep();
            editSubjectPage.WaitForAjax();

            var bodyElement = scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var modalIsOpen = bodyElement.GetAttribute("class").Contains("modal-open");

            editSubjectPage.WaitForAjax();

            Assert.IsFalse(modalIsOpen);
        }

        private string ConvertStudySettingValue(string setting)
        {
            var result = string.Empty;
            switch(setting)
            {
                case "1":
                result = "ENABLED";
                break;

                case "0":
                result = "DISABLED";
                break;

                default:
                break;
            }

            return result;
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" Subject Management dropdown")]
        public void GivenISelectFromSubjectManagementDropdown(string dropdown, string item)
        {
            editSubjectPage.SelectFromDropdown(item, dropdown);
        }

        [Given(@"I have a study with BYOD as ""(.*)""")]
        public async Task GivenIHaveAStudyWithBYODAs(string status)
        {
            string value;
            if (status.ToUpper().Contains("DISABLED"))
                value = "False";
            else
                value = "True";
            await Hooks.mockServer.UpdateStudyCustomValue("BringYourOwnDeviceAvailable", value);
        }

        [Given(@"The following data is displayed in the subject grid")]
        [When(@"The following data is displayed in the subject grid")]
        [Then(@"The following data is displayed in the subject grid")]
        public void GivenTheFollowingDataIsDisplayedInTheSubjectGrid(Table table)
        {
            editSubjectPage.verifyDataIsDisplayedInTheSubjectGrid(table);
        }

        [Given(@"The following data is not displayed in the grid")]
        [When(@"The following data is not displayed in the grid")]
        [Then(@"The following data is not displayed in the grid")]
        public void GivenTheFollowingDataIsNotDisplayedInTheGrid(Table table)
        {
            editSubjectPage.verifyDataIsNotDisplayedInTheSubjectGrid(table);
        }

        [Given(@"Message ""([^""]*)"" is displayed above ""([^""]*)"" grid with ""([^""]*)""")]
        [When(@"Message ""([^""]*)"" is displayed above ""([^""]*)"" grid with ""([^""]*)""")]
        [Then(@"Message ""([^""]*)"" is displayed above ""([^""]*)"" grid with ""([^""]*)""")]
        [Given(@"Message ""([^""]*)"" is displayed below ""([^""]*)"" grid with ""([^""]*)""")]
        [When(@"Message ""([^""]*)"" is displayed below ""([^""]*)"" grid with ""([^""]*)""")]
        [Then(@"Message ""([^""]*)"" is displayed below ""([^""]*)"" grid with ""([^""]*)""")]
        public void ResetPinMessageForCareGiver(string message, string type , string buttons)

        {
            editSubjectPage.ThreadSleep();
            var resetMessage = "";
            switch (type)
            {
                case "Caregiver":

                    resetMessage = scenarioService.ChromeDriver.FindElement(By.CssSelector("div#ResetCareGiverPinInnerDiv div.col-lg-8")).Text;
                    break;
                case "Subject":

                    resetMessage = scenarioService.ChromeDriver.FindElement(By.CssSelector("div#ResetPinInnerDiv div.col-lg-8")).Text;
                    break;
            }
            Assert.AreEqual(message.Trim(), resetMessage.Trim(), $"Pop up message does not match the expected message");

            foreach (var buttonName in buttons.Split(","))
            {
                var trimmedName = buttonName.Trim();
                var fieldMap = editSubjectPage.FieldMaps().First(fm => fm.Label == trimmedName);
                var buttonElement = editSubjectPage.GetWebElementById(fieldMap.Id);
                Assert.AreEqual(trimmedName, buttonElement.Text, $"Pop up button {trimmedName} is not found in the pop up.");

            }
        }

        [Given(@"Message ""([^""]*)"" is not displayed above the ""([^""]*)"" grid")]
        [When(@"Message ""([^""]*)"" is not displayed above the ""([^""]*)"" grid")]
        [Then(@"Message ""([^""]*)"" is not displayed above the ""([^""]*)"" grid")]
        [Given(@"Message ""(.*)"" is not displayed below ""(.*)"" grid")]
        [When(@"Message ""(.*)"" is not displayed below ""(.*)"" grid")]
        [Then(@"Message ""(.*)"" is not displayed below ""(.*)"" grid")]
        public void MessageIsNotDisplayed(string message, string type)
        {
            editSubjectPage.ThreadSleep();
            var bodyElement = scenarioService.ChromeDriver.FindElement(By.TagName("div"));
            switch (type)
            {
                case "Caregiver":
                    var modalIsOpen = bodyElement.GetAttribute("id").Contains("ResetCareGiverPinDiv");
                    Assert.IsFalse(modalIsOpen);
                    break;
                case "Subject":
                    var modalIsOpen1 = bodyElement.GetAttribute("id").Contains("ResetPinInnerDiv");
                    Assert.IsFalse(modalIsOpen1);
                    break;
            }
        }


        [Given(@"status of ""(.*)"" is ""(.*)"" Tick")]
        [When(@"status of ""(.*)"" is ""(.*)"" Tick")]
        [Then(@"status of ""(.*)"" is ""(.*)"" Tick")]
        [Given(@"status of ""(.*)"" is ""(.*)"" X")]
        [When(@"status of ""(.*)"" is ""(.*)"" X")]
        [Then(@"status of ""(.*)"" is ""(.*)"" X")]
        public void GivenStatusOfTraining(string trainingName, string status)
        {
            editSubjectPage.verifyTrainingStatus(trainingName , status);
        }

        [StepDefinition(@"Status ""(.*)"" of subject ""(.*)"" is ""(.*)""")]
        [Given(@"""(.*)"" of subject ""(.*)"" is updated to ""(.*)""")]
        [When(@"""(.*)"" of subject ""(.*)"" is updated to ""(.*)""")]
        [Then(@"""(.*)"" of subject ""(.*)"" is updated to ""(.*)""")]
        public void verifyStatusOfsubject(string statusName, string subjectNumber, string statusValue)
        {
            editSubjectPage.verifySubjectStatus(statusName,subjectNumber, statusValue);
        }

        [Given(@"I completed ""([^""]*)"" for ""([^""]*)""")]
        [When(@"I completed ""([^""]*)"" for ""([^""]*)""")]
        [Then(@"I completed ""([^""]*)"" for ""([^""]*)""")]
        [Given(@"I locked ""([^""]*)"" for ""([^""]*)""")]
        [When(@"I locked ""([^""]*)"" for ""([^""]*)""")]
        [Then(@"I locked ""([^""]*)"" for ""([^""]*)""")]
        public void ThenICompletedFor(string gridColumn, string CareGivers)
        {
            e2eRepository.setGridData(gridColumn, CareGivers);
        }

        [Given(@"I sync the data")]
        [When(@"I sync the data")]
        [Then(@"I sync the data")]
        [Given(@"I refresh page")]
        [When(@"I refresh page")]
        [Then(@"I refresh page")]
        public void ThenISyncTheData()
        {
            editSubjectPage.RefreshPage();
        }

        [Given(@"value associated with ""(.*)"" is ""(.*)""")]
        [When(@"value associated with ""(.*)"" is ""(.*)""")]
        [Then(@"value associated with ""(.*)"" is ""(.*)""")]
        public void ValueAssociatedWithSubjectAttribute(string attributeName, string value)
        {
            editSubjectPage.verifyAssociatedValueWithSubjectAttribute(attributeName,value);
        }

        [StepDefinition(@"I click on link ""(.*)"" for ""(.*)""")]
        public void ThenIClickOnLinkFor(string linkName, string attributeName)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.CssSelector("div.popover-content"));
            element.Click();
        }

        [Given(@"the following data is displayed in the caregiver grid for subject ""([^""]*)""")]
        [When(@"the following data is displayed in the caregiver grid for subject ""([^""]*)""")]
        [Then(@"the following data is displayed in the caregiver grid for subject ""([^""]*)""")]
        public void FollowingDataDisplayedinCaregiverGrid(string subjectNumber, Table table)
        {

            string careGiverforSubject = e2eRepository.getCaregiverForSubject(subjectNumber);
            editSubjectPage.verifyCaregiverGridData(careGiverforSubject, table);
        }

        [StepDefinition(@"""([^""]*)"" tooltip displays message as ""([^""]*)""")]
        [StepDefinition(@"I hover on ""([^""]*)"" button and ""([^""]*)"" message is displayed")]
        public void MessageWillAppearInTooltipForButton(string buttonName, string message)
        {
            editSubjectPage.WhenIHoverOverButtonTextIsDisplayed(buttonName, message);
        }

        [StepDefinition(@"I click on ""([^""]*)"" button in Send Patient Web Backup Email")]
        public void ClickOnButtonInSendPatientWebBackupEmail(string send)
        {
            editSubjectPage.GetWebElement("xpath", "//button[contains(text(),'Send')]").Click();
        }

        [Given(@"""(.*)"" specific popup is displayed")]
        public async Task ThenSpecificPopupIsDisplayed(string popupName)
        {
            IWebElement popup = default;

            if(popupName != "SendWebBackUpEmailModal")
            {
                editSubjectPage.WaitForPopUp(popupName);
                popup = editSubjectPage.GetWebElementById(popupName);
            }
            else
            {
                popup = editSubjectPage.GetWebElement("xpath", "(//div[@id='SendWebBackUpEmailModal'])[2]");
            }
            
            var footer = popup.FindElement(By.ClassName("modal-footer"));

            switch (popupName)
            {
                case "HardStopWarningModal":
                    {
                        var confirmBtn = footer.FindElement(By.Id("VMyesBtn"));
                        Assert.IsTrue(confirmBtn.Displayed);
                        var cancelBtn = footer.FindElement(By.Id("VMcancelBtn"));
                        Assert.IsTrue(cancelBtn.Displayed);
                        break;
                    }
                case "ActivateVisitModal":
                    {
                        var confirmBtn = footer.FindElement(By.Id("VMokBtn"));
                        Assert.IsTrue(confirmBtn.Displayed);
                        var cancelBtn = footer.FindElement(By.Id("VMcancelBtn"));
                        Assert.IsTrue(cancelBtn.Displayed);
                        break;
                    }
                case "SendWebBackUpEmailModal":
                    {
                        var confirmBtns = footer.FindElements(By.CssSelector("[id^=sendBtn_]"));
                        var confirmBtn = confirmBtns.LastOrDefault();
                        Assert.IsTrue(confirmBtn.Displayed);
                        var cancelBtns = footer.FindElements(By.CssSelector("[id^=cancelBtn_]"));
                        var cancelBtn = cancelBtns.LastOrDefault();
                        Assert.IsTrue(cancelBtn.Displayed);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        [Given(@"I click on ""([^""]*)"" button in Activate Visit Modal")]
        public void ClickOnButtonInActivateVisitModal(string ok)
        {
            editSubjectPage.GetWebElement("css", "div#ActivateVisitModal button#VMokBtn").Click();
        }

        [Given(@"I click on ""([^""]*)"" button in Web Backup Email Modal")]
        public void ClickOnButtonInWebBackupEmailModal(string option)
        {
            IReadOnlyCollection<IWebElement> buttons = default;
            if(option == "Send")
            {
                buttons = editSubjectPage.GetWebElements("xpath", "//button[starts-with(@id,'sendBtn_')]");
            }
            else if(option == "Cancel")
            {
                buttons = editSubjectPage.GetWebElements("xpath", "//button[starts-with(@id,'cancelBtn_')]");
            }
            var element = buttons.LastOrDefault();
            element.Click();
        }

        [StepDefinition(@"I enter ""([^""]*)"" in  Web Backup Patient Email inputtextbox field")]
        public void EnterTextInWebBackupPatientEmailInputtextboxField(string text)
        {
            var element = editSubjectPage.GetWebElement("xpath", "//div[@id='patientEmailWrapper']//input");
            element.EnterText(text);
        }

        [Given(@"I enter ""([^""]*)"" in Web Backup Email inputtextbox field")]
        public void EnterTextInWebBackupEmailInputtextboxField(string text)
        {
            var inputtextbox = editSubjectPage.GetWebElements("xpath", "//div[@id='patientEmailWrapper']//input");
            var element = inputtextbox.LastOrDefault();
            element.EnterText(text);
        }

        [StepDefinition(@"""([^""]*)"" is displayed in Web Backup Subject inputtextbox field")]
        public void MessageIsDisplayedInWebBackupSubjectInputtextboxField(string subjectText)
        {
            var element = editSubjectPage.GetWebElement("xpath", "//div[@id='emailSubjectWrapper']//input");
            string elementText = element.GetAttribute("value");
            Assert.AreEqual(elementText, subjectText);

        }

        [StepDefinition(@"I click on ""([^""]*)"" link in new tab")]
        public void ClickOnLinkInNewTab(string element)
        {
            var elementLink = editSubjectPage.GetWebElementByLinkText(element);
            var elementUrl = elementLink.GetAttribute("href");
            editSubjectPage.OpenLinkInNewTab(elementUrl);
        }

        [StepDefinition(@"""([^""]*)"" is displayed in email body")]
        public void MessageDisplayedInEmailBody(string element)
        {
            editSubjectPage.WaitForAjax();
            var actualText = editSubjectPage.GetWebElement("xpath", "//div[@class='form-group email-content-form-group']//div[1]").Text;
            string elementText = element.Replace("currentday+3", editSubjectPage.ResolveDate("currentday+3", "dd-MMM-yyyy"));
            Assert.AreEqual(elementText, actualText);
        }

        [Given(@"""(.*)"" activation date is set for ""(.*)"" for ""(.*)""")]
        public void ActivationDateIsSetFor(string date, string patientVisitName, string patientMappingName)
        {
            var mapping = apiTestData.PatientMappings.FirstOrDefault(p => p.MappingName == patientMappingName);
            var patientId = mapping?.Patient.Id ?? Guid.Empty;

            e2eRepository.UpdatePatientVisit(date, patientVisitName, patientId);
        }
    }
}
