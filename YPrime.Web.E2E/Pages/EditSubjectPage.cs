using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using System;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class EditSubjectPage : BasePage
    {
        private const string CompletedDiaryEntryClassName = "questionnaire-link";
        private const string PatientManagementTabId = "patientDetailsMenuItem";
        private const string CaregiverManagementTabId = "caregiverMenuItem";
        private const string VisitsTabId = "patientVisitMenuItem";
        private const string QuestionnairesTabId = "questionnaireMenuItem";
        private const string DataCorrectionButtonId = "btnDataCorrection";
        private const string ChangeSubjectStatusButtonId = "btnChangeSubjectStatus";
        private const string ResetPINID = "btnResetPin";

        public readonly E2ESettings e2eSettings;
        private readonly ScenarioService _scenarioService;
       
        public override int WaitInterval => 1000;
        public override string PageName => "Subject Management";
        public override string PageUrl => $"{e2eSettings.PortalUrl}/Patient/Edit";

        public EditSubjectPage(
            E2ESettings e2eSettings,
            ScenarioService scenarioService)
            : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            _scenarioService = scenarioService;
            
        }

        public override List<FieldMap> FieldMaps()
        {
            var maps = new List<FieldMap>
            {
                new FieldMap { FieldType = "Tab", Label = "Patient Management", Id = PatientManagementTabId, UiControl = "button" },
                new FieldMap { FieldType = "Tab", Label = "Caregiver Management", Id = CaregiverManagementTabId, UiControl = "button" },
                new FieldMap { FieldType = "Tab", Label = "Visits", Id = VisitsTabId, UiControl = "button" },
                new FieldMap { FieldType = "Tab", Label = "Questionnaires", Id = QuestionnairesTabId, UiControl = "button" },
                new FieldMap { FieldType = "Button", Label = "Data Correction", Id = DataCorrectionButtonId, UiControl = "button" },
                new FieldMap { FieldType = "Button", Label = "Change Subject Status", Id = ChangeSubjectStatusButtonId, UiControl = "button" },
                new FieldMap { FieldType = "Button", Label = "Reset PIN", Id = ResetPINID, UiControl = "button" },
                new FieldMap { FieldType = "Button", Label = "Email Web Backup URL (Subject Handheld)", Id = "webBackupEmailBtn", UiControl = "button" },
                new FieldMap { FieldType = "Button", Label = "BYOD Enrollment Information", Id = "btnBYODEnrollmentInformation", UiControl = "button" },
                new FieldMap { FieldType = "header", Label = "Status", Id = "subjectStatus", UiControl = "hdr" },
                new FieldMap { FieldType = "list", Label = "Compliance status", Id = "subjectComplianceStatus", UiControl = "list" },
                new FieldMap() { FieldType = "Table", Label = "Subject Questionnaire", Id = "allquestionnaires", UiControl = "Table", WaitForElement = true },
                new FieldMap() { FieldType = "Dropdown", Label = "Patient status", Id = "PatientStatusTypeId", UiControl = "select", WaitForElement = false },
            };

            IWebElement activeTabElement = null;
            var bodyElement = _scenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var modalIsOpen = bodyElement.GetAttribute("class").Contains("modal-open");

            foreach (var map in maps)
            {
                var foundElements = _scenarioService.ChromeDriver.FindElements(By.Id(map.Id));

                if (foundElements.Any())
                {
                    var foundElement = foundElements.First();

                    if (foundElement.GetAttribute("class").Contains("active"))
                    {
                        activeTabElement = foundElement;
                        break;
                    }
                }
            }

            switch (activeTabElement?.GetAttribute("id"))
            {
                case CaregiverManagementTabId:

                    maps.AddRange(new List<FieldMap>
                    {
                        new FieldMap { FieldType = "Dropdown", Label = "Select A Caregiver Type", Id = "caregiverDropdown", UiControl = "select" },
                        new FieldMap { FieldType = "Button", Label = "Assign Caregiver", Id = "saveBtn", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "Save", Id = "submitCaregiver", UiControl = "button" },
                        new FieldMap { FieldType = "tableheader", Label = "Caregiver", Id = "hdrCaregiver", UiControl = "hdr", WaitForElement = false },
                        new FieldMap { FieldType = "tableheader", Label = "Handheld Training Complete", Id = "hdrCaregiverHandheldTraining", UiControl = "hdr", WaitForElement = false },
                        new FieldMap { FieldType = "tableheader", Label = "Tablet Training Complete", Id = "hdrCaregiverTabletTraining", UiControl = "hdr", WaitForElement = false },
                        new FieldMap { FieldType = "tableheader", Label = "Account Locked", Id = "hdrCaregiverAccountLocked", UiControl = "hdr", WaitForElement = false },
                        new FieldMap { FieldType = "tableheader", Label = "Reset PIN", Id = "hdrCaregiverResetPIN", UiControl = "hdr", WaitForElement = false },
                        new FieldMap { FieldType = "Button", Label = "Update PIN", Id = "btnUpdatePinForCareGiver", UiControl = "button" },
                  });
                    var resetPinButtons = _scenarioService.ChromeDriver.FindElements(By.CssSelector("div.pull-right"));

                    if (modalIsOpen)
                    {
                        maps.Add(new FieldMap { FieldType = "Button", Label = "Cancel", Id = "confirmModalCancelBtn", UiControl = "button" });
                    }
                    else if (resetPinButtons.Count>0) 
                    {
                        maps.Add(new FieldMap { FieldType = "Button", Label = "Cancel", Id = "btnCancelResetPinCareGiver", UiControl = "button" });
                    }
                    else
                    {
                        maps.Add(new FieldMap { FieldType = "Button", Label = "Cancel", Id = "cancelBtn", UiControl = "button" });
                    }

                    break;
                case PatientManagementTabId:

                    maps.AddRange(new List<FieldMap>
                    {
                        new FieldMap { FieldType = "Dropdown", Label = "Patient Status", Id = "PatientStatusTypeId", UiControl = "select" },
                        new FieldMap { FieldType = "Button", Label = "Save Patient Status", Id = "patientStatusSaveButton", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "Cancel Patient Status", Id = "patientStatusCancelButton", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "Update Pin", Id = "btnUpdatePinForPatient", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "Cancel", Id = "btnCancelResetPin", UiControl = "button" },
                        new FieldMap { FieldType = "Label", Label = "Tablet training label", Id = "lblIsTabletTrainingCompleteForPatient", UiControl = "lbl" },
                        new FieldMap { FieldType = "Label", Label = "Tablet training status", Id = "TabletTrainingStatusForPatient", UiControl = "lbl" },
                        new FieldMap { FieldType = "Label", Label = "Handheld training label", Id = "lblIsTabletTrainingCompleteForPatient", UiControl = "lbl" },
                        new FieldMap { FieldType = "Label", Label = "Handheld training status", Id = "TabletTrainingStatusForPatient", UiControl = "lbl" },
                        new FieldMap { FieldType = "TableData", Label = "Gender", Id = "lbl-Gender-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Female", Id = "value-Gender-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Male", Id = "value-Gender-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Date of Birth", Id = "lbl-Date of Birth-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "CurrentDate", Id = "value-Date of Birth-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Weight", Id = "lbl-Weight-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Height", Id = "lbl-Height-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "100.00", Id = "value-Weight-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "100.00", Id = "value-Height-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Language", Id = "lblLanguageForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "English (United States)", Id = "languageOfPatient", UiControl = "td" },
                        new FieldMap { FieldType = "TableData", Label = "Subject Attributes", Id = "lbl-Gender-ForPatient", UiControl = "td" },
                        new FieldMap { FieldType = "Label", Label = "Corrections", Id = "lblCorrections", UiControl = "lbl" },
                        new FieldMap { FieldType = "Button", Label = "Oval", Id = "dcf-information-popover", UiControl = "button" },
            });
                    if (modalIsOpen)
                    {
                        maps.Add(new FieldMap { FieldType = "Button", Label = "Cancel Patient Status", Id = "patientStatusCancelButton", UiControl = "button" });
                    }
                    else
                    {
                        maps.Add(new FieldMap { FieldType = "Button", Label = "Cancel", Id = "cancelBtn", UiControl = "button" });
                    }

                    break;
                case VisitsTabId:
                    maps.AddRange(new List<FieldMap>
                    {
                        new FieldMap { FieldType = "Button", Label = "Yes", Id = "VMyesBtn", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "VMok", Id = "VMokBtn", UiControl = "button" }
                    });
                    break;
                case QuestionnairesTabId:
                default:
                    maps.AddRange(new List<FieldMap>
                    {
                        new FieldMap { FieldType = "Dropdown", Label = "Patient Status", Id = "PatientStatusTypeId", UiControl = "select_direct" },
                        new FieldMap { FieldType = "Button", Label = "Save", Id = "patientStatusSaveButton", UiControl = "button" },
                        new FieldMap { FieldType = "Button", Label = "Cancel", Id = "patientStatusCancelButton", UiControl = "button" }
                    });
                    break;
            }

            return maps;
        }

        public override string GetDropdownSelectedValue(string control)
        {
            var fieldMap = FieldMaps().Find(f => f.Label == control);

            var elemId = fieldMap.Id;
            var element = GetWebElementById(elemId);

            string result;

            if (fieldMap.UiControl == "select" || fieldMap.UiControl == "select_direct")
            {
                var options = element.FindElements(By.TagName("option"));
                var selectedOption = options.First(o => o.Selected);
                result = selectedOption.Text;
            }
            else
            {
                var selected = element.FindElement(By.ClassName("selected"));
                result = selected.Text;
            }

            return result;
        }

        public IWebElement GetCompletedDiaryEntryLinkByName(string diaryEntryName)
        {
            var elements = GetWebElementsByClass(CompletedDiaryEntryClassName);

            var diaryEntryLinkElement = elements.First(e => e.Text.ToUpper() == diaryEntryName.ToUpper());

            return diaryEntryLinkElement;
        }
        public void verifyDataIsDisplayedInTheSubjectGrid(Table table)
        {
            var fields = table.CreateSet<LabelAndValueSet>();
            foreach (var field in fields)
            {
                var expectedLabelValue = field.Label.Trim();
                var expectedLabelelemId = FieldMaps().Find(f => f.Label == field.Label).Id;
                var actualLabelElement = GetWebElementById(expectedLabelelemId);
                Assert.AreEqual(expectedLabelValue, actualLabelElement.Text);

                var expectedValue = field.Value;
                if(expectedValue == "CurrentDate")
                {
                    var DOB = DateTime.UtcNow.Date;
                    var DateOfBirth = DOB.ToString("dd/MMMMMMM/yyyy");
                    var elemId = FieldMaps().Find(f => f.Label == field.Value).Id;
                    var actualElement = GetWebElementById(elemId);
                    Assert.AreEqual(DateOfBirth, actualElement.Text);
                }
                else
                {
                    var elemId = FieldMaps().Find(f => f.Label == field.Value).Id;
                    var actualElement = GetWebElementById(elemId);
                    Assert.AreEqual(expectedValue, actualElement.Text);
                }
               
            }
        }
        public void verifyDataIsNotDisplayedInTheSubjectGrid(Table table)
        {
            var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("td#handheldTrainingStatusForPatient span"));
           // lbl - Gender - ForPatient;
            var subjectattribute = element.GetAttribute("id").Contains("Gender");
            Assert.IsFalse(subjectattribute);
        }

        public void verifyTrainingStatus(string trainingName , string status)
        {
            switch (trainingName)
            {
                case "Handheld training status":
                    if (status == "Red")
                    {
                        var trainingstatus = "false";
                        var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("td#handheldTrainingStatusForPatient span"));
                        var classname = element.GetAttribute("class");
                        Assert.IsTrue(classname.Contains(trainingstatus));
                    }
                    else if (status == "Green")
                    {
                        var trainingstatus = "true";
                        var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("td#handheldTrainingStatusForPatient span"));
                        var classname = element.GetAttribute("class");
                        Assert.IsTrue(classname.Contains(trainingstatus));
                    }
                    break;
                case "Tablet training status":
                    if (status == "Red")
                    {
                        var trainingstatus = "false";
                        var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("td#TabletTrainingStatusForPatient span"));
                        var classname = element.GetAttribute("class");
                        Assert.IsTrue(classname.Contains(trainingstatus));
                    }
                    else if (status == "Green")
                    {
                        var trainingstatus = "true";
                        var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("td#TabletTrainingStatusForPatient span"));
                        var classname = element.GetAttribute("class");
                        Assert.IsTrue(classname.Contains(trainingstatus));
                    }
                    break;
            }
            
        }

        public void verifyAssociatedValueWithSubjectAttribute(string attributeName , string value)
        {
            var element = _scenarioService.ChromeDriver.FindElement(By.CssSelector("a#dcf-information-popover span"));
            Assert.AreEqual(value, element.Text);
        }

        public void verifySubjectStatus(string statusName,string subjectNumber , string statusValue)
        {
            var fieldMap = FieldMaps().Find(f => f.Label == statusName);
            var element = GetWebElementById(fieldMap.Id);
            Assert.AreEqual(statusValue, element.Text);
        }

        class LabelAndValueSet
        {
            public string Label { get; set; }
            public string Value { get; set; }
        } 

        public async void verifyCaregiverGridData(string careGiverforSubject ,Table table)
        {
            var fields = table.CreateSet<setValues>();
            foreach (var field in fields)
            {  

                string expectedValueOfCaregiver = field.Caregiver;
                string actualValueOfCaregiver = ScenarioService.ChromeDriver.FindElement(By.Id("name-" + careGiverforSubject)).Text; 
                Assert.AreEqual(expectedValueOfCaregiver, actualValueOfCaregiver);

                string expectedValueOfHandheldTraining = field.HandheldTrainingComplete;
                string actualValueOfHandheldTraining = ScenarioService.ChromeDriver.FindElement(By.Id("handheldTraining-" +careGiverforSubject)).Text;
                Assert.AreEqual(expectedValueOfHandheldTraining, actualValueOfHandheldTraining);

                string expectedValueOfTabletTraining = field.TabletTrainingComplete;
                string actualValueOfTabletTraining = ScenarioService.ChromeDriver.FindElement(By.Id("tabletTraining-" +careGiverforSubject)).Text;
                Assert.AreEqual(expectedValueOfTabletTraining, actualValueOfTabletTraining);

                string expectedValueOfAccount = field.AccountLocked;
                string actualValueOfAccount = ScenarioService.ChromeDriver.FindElement(By.Id("accountLocked-" + careGiverforSubject)).Text;
                Assert.AreEqual(expectedValueOfAccount, actualValueOfAccount);

                string expectedValueOfResetPin = field.ResetPin;
                string actualValueOfResetPin = ScenarioService.ChromeDriver.FindElement(By.Id("caregiverResetPin-" + careGiverforSubject)).Text;
                Assert.AreEqual(expectedValueOfResetPin,actualValueOfResetPin);
            }
        }


        public void WhenIHoverOverButtonTextIsDisplayed(string element, string message)
        {
            var actualElement = ScenarioService.ChromeDriver.FindElement(By.LinkText(element));
            HoverOnElement(actualElement);
            var title = actualElement.GetAttribute("title");
            Assert.AreEqual(message, title);
        }

        class setValues

        {
            public string Caregiver { get; set; }
            public string HandheldTrainingComplete { get; set; }
            public string TabletTrainingComplete { get; set; }
            public string AccountLocked { get; set; }
            public string ResetPin { get; set; }

        }
    }
}

