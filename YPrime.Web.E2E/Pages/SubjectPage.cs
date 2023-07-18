using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SubjectPage : BasePage
    {
        private const string SubjectNumberLinkClassName = "patient-number-link";

        private readonly E2ESettings e2eSettings;
        public override string PageName => "Subject Management";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Patient";

        public override string GetDropdownSelectedValue(string control)
        {
            var result = string.Empty;

            var matchingFieldMap = FieldMaps().First(fm => fm.Label == control);

            if (matchingFieldMap.UiControl == "metro_dropdown")
            {
                var parent = GetWebElementById(matchingFieldMap.Id).GetParent().GetParent();

                var selectedElement = parent.FindElement(By.ClassName("selected"));

                result = selectedElement.Text;
            }

            return result;

        }
        public SubjectPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Dropdown", Label = "All Sites", Id = "MainSiteId", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "Add New Subject", Id = "btnAddSubject", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Table", Label = "Active Grid Table", Id = "ActivePatientsGrid", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Active Patient dropdown", Id = "ActivePatientsGrid_length", UiControl = "select_basic", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridPdfButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Hamburger Menu", Id = "gridMenuButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "TextArea", Label = "Patients Grid info", Id = "ActivePatientsGrid_info", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Excel", Id = "gridExcelButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "CSV", Id = "gridCsvButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Print", Id = "gridPrintButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Spinner", Label = "Spinner", Id = "loadingSpinner", UiControl = "spinner", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Subject Number Input", Id = "Search_SubjectNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Initial Site", Id = "MainSiteId", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Link", Label = "All Subjects", Id = "PatientsGrid-results-badge", UiControl = "link", WaitForElement = false },
            new FieldMap() { FieldType = "Link", Label = "Inactive Subjects", Id = "CompletedPatientsGrid-results-badge", UiControl = "link", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Site Name", Id = "hdrSiteName-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Subject Number", Id = "hdrSubjectNumber-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Subject Status", Id = "hdrSubjectStatus-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Enrollment Date", Id = "hdrEnrollmentDate-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Compliance", Id = "hdrCompliance-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Handheld Training Complete", Id = "hdrHandheldTrainingComplete-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Tablet Training Complete", Id = "hdrTabletTrainingComplete-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Last Diary Date", Id = "hdrLastDiaryDate-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Last Sync Date", Id = "hdrLastSyncDate-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "S-10000-004", Id = "SubjectNumber-S-10000-004", UiControl = "button", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "S-20000-001", Id = "SubjectNumber-S-20000-001", UiControl = "button", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "X", Id = "mainclose", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Email icon", Id = "webConfirmationEmailBtn", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Cancel", Id = "cancelBtn_00000000-0000-0000-0000-000000000000", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Print icon", Id = "webConfirmationPrintBtn", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Email Confirmation", Id = "webConfirmationEmailBtn", UiControl = "button", WaitForElement = false  },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Email textbox", Id = "patientEmailCe_00000000-0000-0000-0000-000000000000", UiControl = "input", WaitForElement = false  },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Subject textbox", Id = "emailSubjectCe_00000000-0000-0000-0000-000000000000", UiControl = "input", WaitForElement = false  },
            new FieldMap() { FieldType = "Inputtextbox", Label = "To access your BYOD enrollment information select the attachment", Id = "emailBodyWrapperCe_00000000-0000-0000-0000-000000000000", UiControl = "input", WaitForElement = false  },
            new FieldMap() { FieldType = "Button", Label = "PDF Attachment", Id = "bConfirmationEmailBtn", UiControl = "button", WaitForElement = false  },
            new FieldMap() { FieldType = "Button", Label = "Send", Id = "sendBtn_00000000-0000-0000-0000-000000000000", UiControl = "button", WaitForElement = false  },
        };

        public void ClickSubjectPageBackground()
        {
            var field = FieldMaps().Find(f => f.Label == "Spinner").Id;
            IJavaScriptExecutor js = (IJavaScriptExecutor)ScenarioService.ChromeDriver;
            js.ExecuteScript("document.getElementById('" + field + "').click()");
        }

        public IWebElement GetSubjectNumberLink(string subjectNumber)
        {
            var elements = ScenarioService.ChromeDriver.FindElements(By.ClassName(SubjectNumberLinkClassName));

            var matchingNumberLink = elements
                .First(sn => sn.Text.Trim().ToUpper() == subjectNumber.ToUpper());

            return matchingNumberLink;
        }

        public override void NavigateToPage()
        {
            base.NavigateToPage();
            ScenarioService.ChromeDriver.Navigate().Refresh();
        }

        public void VerifyElementForVisibilty(Table table)
        {
            var fields = table.CreateSet<ControlValueMap>();
            foreach (var field in fields)
            {
                string actualValue;
                var expectedValue = field.ButtonName;
                var actualElement = ScenarioService.ChromeDriver.FindElement(By.LinkText(expectedValue));
                actualValue = actualElement.Text;
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        public void VerifySortingOfElementsInGrid(string control, Table table)
        {
            ((IJavaScriptExecutor)ScenarioService.ChromeDriver).ExecuteScript("document.body.style.zoom='90%'");
            var fields = table.CreateSet<setColumnName>();
            foreach (var field in fields)
            {
                var headerId = FieldMaps().Find(f => f.Label == field.ColumnName).Id;
                var element = ScenarioService.ChromeDriver.FindElements(By.Id(headerId));
                IJavaScriptExecutor js = (IJavaScriptExecutor)ScenarioService.ChromeDriver;
                js.ExecuteScript("arguments[0].scrollIntoView(true);", element.FirstOrDefault());
                var header = element.FirstOrDefault().Text.Replace(" ", "");
                ClickByJavascript(headerId);
                var valuesBeforeSorted = GetColumnsData(control, header);
                string[] beforeSortData = new string[valuesBeforeSorted.Count()];

                for (int i = 0, j = beforeSortData.Length - 1; j >= 0; i++, j--)
                {
                    beforeSortData[i] = valuesBeforeSorted[j].Trim().ToString();
                }
                ClickByJavascript(headerId);

                var valuesAfterSorted = GetColumnsData(control, header);

                string[] afterSortData = new string[valuesAfterSorted.Count()];

                for (int i = 0; i < valuesAfterSorted.Count(); i++)
                {
                    afterSortData[i] = valuesAfterSorted[i].Trim().ToString();
                }
                bool isEqual = Enumerable.SequenceEqual(beforeSortData, afterSortData);

                Assert.IsTrue(isEqual);
            }
        }

        public List<string> GetColumnsData(string control, string header)
        {
            WaitForAjax();
            List<string> columnsData = new List<string>();
            var tableId = FieldMaps().Find(f => f.Label == control).Id;
            var subjectNumberList = ScenarioService.ChromeDriver
                .FindElements(By.XPath("//table[@id='" + tableId + "']//td[contains(@id,'SubjectNumber')]//a"));
            string headerValue;
            if (header.Contains("HandheldTraining"))
            {
                headerValue = "HandheldTraining";
            }
            else if (header.Contains("TabletTraining"))
            {
                headerValue = "TabletTraining";
            }
            else if (header.Contains("LastDiaryDate"))
            {
                headerValue = "DiaryEntryDate";
            }
            else if (header.Contains("LastSyncDate"))
            {
                headerValue = "LastDeviceSyncDate";
            }
            else
            {
                headerValue = header;
            }
            for (int i = 0; i < subjectNumberList.Count; i++)
            {
                var id = headerValue + "-" + subjectNumberList[i].Text;
                var data = ScenarioService.ChromeDriver.FindElement(By.Id(id)).Text;
                columnsData.Add(data.ToString());
            }
            return columnsData;
        }

        class setColumnName
        {
            public string ColumnName { get; set; }
        }

        public void verifyColumnsNameDisplayed(Table table)
        {
            var tableData = table.CreateSet<ControlValueMap>().ToList();
            List<string> expectedColumn = new List<string>();
            foreach (var column in tableData)
            {
                expectedColumn.Add(column.Label);
            }



            var fieldMap = FieldMaps().Find(f => f.Label == "Active Grid Table");
            var element = GetWebElementById(fieldMap.Id);



            ReadOnlyCollection<IWebElement> actualColumn;
            actualColumn = element.FindElement(By.TagName("thead")).FindElement(By.TagName("tr")).FindElements(By.TagName("th"));
            List<string> actualColumns = new List<string>();



            for (int index = 0; index < actualColumn.Count; index++)
            {
                var headerName = actualColumn.ElementAt(index).GetAttribute("innerText") == "" ? actualColumn.ElementAt(index).GetAttribute("value") == "" ? actualColumn.ElementAt(index).Text : actualColumn.ElementAt(index).GetAttribute("value") : actualColumn.ElementAt(index).GetAttribute("innerText");
                actualColumns.Add(headerName);



            }
            Assert.IsTrue(expectedColumn.SequenceEqual(actualColumns));
        }



            public void verifyColumnValues(Table table)
        {
            var fields = table.CreateSet<setValues>();
            foreach (var field in fields)
            {

                string expectedValueOfSiteName = field.SiteName;
                string actualValueOfSiteName = ScenarioService.ChromeDriver.FindElement(By.Id("SiteName-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfSiteName, actualValueOfSiteName);

                string expectedValueOfSubjectNumber = field.SubjectNumber;
                string actualValueOfSubjectNumber = ScenarioService.ChromeDriver.FindElement(By.Id("SubjectNumber-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfSubjectNumber, actualValueOfSubjectNumber);

                string expectedValueOfSubjectStatus = field.SubjectStatus;
                string actualValueOfSubjectStatus = ScenarioService.ChromeDriver.FindElement(By.Id("SubjectStatus-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfSubjectStatus, actualValueOfSubjectStatus);

                string expectedValueOfEnrollmentDate = field.EnrollmentDate;
                if (expectedValueOfEnrollmentDate.Equals("Current Date"))
                {
                    var expectedEnrollmentDate = DateTime.UtcNow.Date;
                    var date = expectedEnrollmentDate.ToString("dd-MMM-yyyy");
                    string actualValueOfEnrollmentDate = ScenarioService.ChromeDriver.FindElement(By.Id("EnrollmentDate-" + field.SubjectNumber)).Text;
                    Assert.AreEqual(date, actualValueOfEnrollmentDate);
                }

                string expectedValueOfCompliance = field.Compliance;
                string actualValueOfCompliance = ScenarioService.ChromeDriver.FindElement(By.Id("Compliance-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfCompliance, actualValueOfCompliance);

                string expectedValueOfHTC = field.HandheldTrainingComplete;
                string actualValueOfHTC = ScenarioService.ChromeDriver.FindElement(By.Id("HandheldTraining-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfHTC, actualValueOfHTC);

                string expectedValueOfTabletTraining = field.HandheldTrainingComplete;
                string actualValueOfTablettraining = ScenarioService.ChromeDriver.FindElement(By.Id("TabletTraining-" + field.SubjectNumber)).Text;
                Assert.AreEqual(expectedValueOfTabletTraining, actualValueOfTablettraining);

                string expectedValueOfDiaryDate = field.LastDiaryDate;
                if (expectedValueOfDiaryDate.Equals("Current Date"))
                {
                    var expectedLastDiaryDate = DateTime.UtcNow.Date;
                    var Diarydate = expectedLastDiaryDate.ToString("dd-MMM-yyyy");
                    string actualValueOfLastDiaryDate = ScenarioService.ChromeDriver.FindElement(By.Id("DiaryEntryDate-" + field.SubjectNumber)).Text;
                    Assert.AreEqual(Diarydate, actualValueOfLastDiaryDate);
                }
                else
                {
                    var expectedLastDiaryDate = "";
                    string actualValueOfLastDiaryDate = ScenarioService.ChromeDriver.FindElement(By.Id("DiaryEntryDate-" + field.SubjectNumber)).Text;
                    Assert.AreEqual(expectedLastDiaryDate, actualValueOfLastDiaryDate);
                }

                string expectedValueOfLSD = field.LastSyncDate;
                if (expectedValueOfLSD.Equals("Current Date"))
                {
                    var expectedLastSyncDate = DateTime.UtcNow.Date;
                    var Syncdate = expectedLastSyncDate.ToString("dd-MMM-yyyy");
                    string actualLastSyncDate = ScenarioService.ChromeDriver.FindElement(By.Id("LastDeviceSyncDate-" + field.SubjectNumber)).Text;
                    Assert.AreEqual(Syncdate, actualLastSyncDate);
                }
                else
                {
                    var expectedLastSyncDate = "";
                    string actualLastSyncDate = ScenarioService.ChromeDriver.FindElement(By.Id("LastDeviceSyncDate-" + field.SubjectNumber)).Text;
                    Assert.AreEqual(expectedLastSyncDate, actualLastSyncDate);
                }
            }
        }

        [Given(@"Add New Subject button is disabled")]
        public void VerifyAddNewSubjectButtonIsDisabled()
        {

            var actualElement = ScenarioService.ChromeDriver.FindElement(By.Id("btnAddSubject")).GetAttribute("class");
            Assert.IsTrue(actualElement.Contains("disabled-panel"));
        }

        class setValues
        {
            public string SiteName { get; set; }
            public string SubjectNumber { get; set; }
            public string SubjectStatus { get; set; }
            public string EnrollmentDate { get; set; }
            public string Compliance { get; set; }
            public string HandheldTrainingComplete { get; set; }
            public string TabletTrainingComplete { get; set; }
            public string LastDiaryDate { get; set; }
            public string LastSyncDate { get; set; }

        }
    }
}