using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Extensions;
using TechTalk.SpecFlow.Assist;
using System.Collections.ObjectModel;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class ReportsPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Reports";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Report#";

        public ReportsPage(
            E2ESettings e2eSettings,
            ScenarioService scenarioService ,E2ERepository e2eRepository)
            :base(scenarioService,e2eRepository)
        {
            this.e2eSettings = e2eSettings;
        }


        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Dropdown", Label = "site Number", Id = "SiteID", UiControl = "select" },
            new FieldMap() { FieldType = "Dropdown", Label = "Subject Number", Id = "SubjectID", UiControl = "select", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "Display the report", Id = "FILTER_BTN", UiControl = "button" },
            new FieldMap() { FieldType = "header", Label = "Site Details (Blinded)", Id = "RptHeaderName", UiControl = "hdr" },
            new FieldMap() { FieldType = "header", Label = "Site Details (Blinded)", Id = "RptHeaderName", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Country", Id = "hdr-Country", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Site Number", Id = "hdr-SiteNumber", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Investigator Name", Id = "hdr-InvestigatorName", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Site Status", Id = "hdr-SiteStatus", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Date of First Screening", Id = "hdr-DateofFirstScreening", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Date of Activation", Id = "hdr-DateofActivation", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Date of Deactivation", Id = "hdr-DateofDeactivation", UiControl = "hdr" },
            new FieldMap() { FieldType = "tableheader", Label = "Date of Reactivation", Id = "hdr-DateofReactivation", UiControl = "hdr" },
            new FieldMap() { FieldType = "Button", Label = "Hamburger Menu", Id = "btnGridCollection", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Hamburger", Id = "btnGridCollection", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridReportPDFButton", UiControl = "button" },
            new FieldMap() { FieldType = "table", Label = "Study User (Unblinded)", Id = "6bf147b0-c292-4c74-af27-f180a2095a34", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Duplicate Subject Report", Id = "86aa839e-d43c-4e1d-9ad9-b7ff75810b4e", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Total Enrollment (Unblinded)", Id = "58bedfae-1deb-42d2-b10e-78516c36fe6f", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Display the report", Id = "FILTER_BTN", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = "Excel", Id = "gridReportExcelButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "CSV", Id = "gridReportCSVButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Print", Id = "gridReportPrintButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridReportPDFButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Site Report", Id = "15903efc-b8b8-41cf-a20e-a335565ac0f5", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "DCF Status Report", Id = "9087c7cb-771c-41ee-9b8c-2213d8b9a02b", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "eCOA Compliance (Unblinded)", Id = "ebdd4231-632a-4526-bd30-dc8f716f5448", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Average Diary Duration (Unblinded)", Id = "805df81a-8ce2-4e7a-887d-399dcd481cc5", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Site", Id = "hdrSiteName-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Subject Number", Id = "hdrSubjectNumber-ActivePatientsGrid", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "Link", Label = "Enrolled Date", Id = "EnrolledDate", UiControl = "link", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridReportPDFButton", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = " Logout", Id = "btnLogout", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Site", Id = "Search_Site", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Subject Number", Id = "Search_SubjectNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Enrolled Date", Id = "Search_EnrolledDate", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Site Number", Id = "Search_SiteNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Username", Id = "Search_Username", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Site Name", Id = "Search_SiteName", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_DCFStatus", Id = "Search_DCFStatus", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Subject Information Audit Report", Id = "4338f297-e844-40f9-bcce-450fbd11523a", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_Subject Attribute", Id = "Search_SubjectAttribute", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_QuestionnaireName", Id = "Search_QuestionnaireName", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search_AverageDuration (Minutes)", Id = "Search_AverageDuration (Minutes)", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "table", Label = "Visit Compliance", Id = "43fa6902-311e-442c-a759-3b431916a7df", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Search Visit", Id = "Search_Visit", UiControl = "input", WaitForElement = false },

        };

        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.ClassName("selected"));

            return selected.Text;
        }
        class setColumnName
        {
            public string ColumnName { get; set; }
            public string ButtonName { get; set; }
        }

        public void ClickOnGridPageItemByClass(string value)
        {
            var element = GetWebElementsByClass(value).FirstOrDefault();
            element.Click();
        }

        public void SetShowEntriesSelectedValue(string value)
        {
            var element = GetWebElementsByClass("dataTables_length"); 
            var optionsofDropDown = element.SelectMany(x => x.FindElements(By.TagName("option")));

            var dropdownValue = optionsofDropDown.First(x => x.Text == value);
            dropdownValue.Click();
        }

        public void ClickOnReportList(string reportName)
        {
            WaitForAjax();
            ThreadSleep();

            var reportList = GetWebElementsByCss("a.list-group-item");
            var reportItem = reportList.SingleOrDefault(e => e.Text == reportName);
            reportItem.Click();
        }
        public void verifyColumnsNameDisplayed(Table table)
        {
            var tableData = table.CreateSet<ControlValueMap>().ToList();
            List<string> expectedColumn = new List<string>();
            foreach (var column in tableData)
            {
                expectedColumn.Add(column.Label);
            }
            var fieldMap = FieldMaps().Find(f => f.Label == "Site Report");
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










        public void verifySiteVisibility(Table table)
        {
            var fields = table.CreateSet<SetValues>();
            foreach (var field in fields)
            {
                var expectedValue = field.SiteNumber;
                var actualElement = GetWebElementByXPath("//div[@class='dataTables_scrollBody']//tbody/tr//td[text()='" + expectedValue + "']");
                Assert.AreEqual(expectedValue, actualElement.Text);
            }
        }
        class SetValues
        {
            public string SiteNumber { get; set; }
        }

       
        public void ClickOnPagination(string buttonText)
        {           
            var button = GetWebElementByLinkText(buttonText);
            button.Click();
        }
    }
}
