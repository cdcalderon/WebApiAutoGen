using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class BulkEditSitePage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Bulk Site Management";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Site/SiteBulkUpdate";

        public BulkEditSitePage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "tableheader", Label = "Site Number", Id = "hdrSiteNumber", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Name", Id = "hdrName", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Active", Id = "hdrActive", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Site Number input", Id = "Search_SiteNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Name input", Id = "Search_Name", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "toggle", Label = "Master Active toggle", Id = "page-toggle", UiControl = "toggle", WaitForElement = false },
            new FieldMap() { FieldType = "toggle", Label = "InitalSite toggle", Id = "IsActive-0", UiControl = "toggle", WaitForElement = false },
            new FieldMap() { FieldType = "toggle", Label = "Site1 toggle", Id = "IsActive-1", UiControl = "toggle", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Save", Id = "btnSave", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Toggle", Label = "master toggle", Id = "site-active-toggle", UiControl = "toggle", WaitForElement = false},
        };

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

        public void verifyColumnsNameDisplayed(Table table)
        {
            var fields = table.CreateSet<setColumnName>();
            foreach (var field in fields)
            {

                string actualValue;
                var expectedValue = field.ColumnName;
                var elemId = FieldMaps().Find(f => f.Label == field.ColumnName).Id;
                var actualElement = ScenarioService.ChromeDriver.FindElements(By.Id(elemId));
                actualValue = actualElement.FirstOrDefault().Text;
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        class setColumnName
        {
            public string ColumnName { get; set; }
            public string FieldType { get; set; }
        }
    }

}