using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class DashboardPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "At a Glance";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Dashboard";

        public override int WaitInterval => 2000;
        public DashboardPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

        public override List<FieldMap> FieldMaps()
        {
            return new List<FieldMap>
            {
                new FieldMap() { FieldType = "Button", Label = "Manage DCF's", Id = "btnManageDcfs", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Clear", Id = "btnClear", UiControl = "button" },
                new FieldMap() { FieldType = "Text", Label = "DCFs Requiring Your Attention", Id = "DCFWidget", UiControl = "Text" },
                new FieldMap() { FieldType = "Text", Label = "Device Inventory", Id = "DeviceInventory", UiControl = "Text" },
                new FieldMap() { FieldType = "Text", Label = "Recently Viewed", Id = "RecentlyViewedPatientsWidget", UiControl = "Text" },
                new FieldMap() { FieldType = "Text", Label = "Enrollment", Id = "EnrollmentWidget", UiControl = "Text" },
                new FieldMap() { FieldType = "Button", Label = "Hamburger", Id = "view-device-icon", UiControl = "Text" },
                new FieldMap() { FieldType = "Button", Label = "DCF", Id = "dCFTable", UiControl = "button" },
                new FieldMap() { FieldType = "Text", Label = "S-10000-001", Id = "S-10000-001", UiControl = "button" },
                new FieldMap() { FieldType = "Text", Label = "Change subject Information", Id = "Change_subject_Information", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Manage Study", Id = "manageStudyBtn", UiControl = "button" },
                new FieldMap() { FieldType = "Text", Label = "Software Release Management", Id = "softwareRelease", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Site Tablet Web Backup", Id = "webBackUpButtonSites", UiControl = "button" },
            };
        }

        public override void NavigateToPage()
        {
            base.NavigateToPage();
            ScenarioService.ChromeDriver.Navigate().Refresh();
        }

    }
}
