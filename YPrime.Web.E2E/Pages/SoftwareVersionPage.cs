using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SoftwareVersionPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Software Version";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/SoftwareVersion";

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

        public SoftwareVersionPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }


        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {

            new FieldMap() { FieldType = "Button", Label = "Hamburger Grid", Id = "gridMenuButton", UiControl = "button" },
            new FieldMap() { FieldType = "table", Label = "Software Version", Id = "softwareVersions", UiControl = "table", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Excel", Id = "gridExcelButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "CSV", Id = "gridCsvButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Print", Id = "gridPrintButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridPdfButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Version Number", Id = "hdr-VersionNumber", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "tableheader", Label = "Package Path", Id = "hdr-PackagePath", UiControl = "hdr", WaitForElement = false },
            new FieldMap() { FieldType = "Link", Label = "Package Path", Id = "PackagePath", UiControl = "link", WaitForElement = false },
        };
    }
}