using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Extensions;
using System.Linq;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class DataExportPage : BasePage
    {
        public readonly E2ESettings e2eSettings;
        public override string PageUrl => $"{e2eSettings.PortalUrl}/Export#";
        public override string PageName => "Data Export";
        public DataExportPage(E2ESettings e2eSettings, ScenarioService scenarioService, E2ERepository e2eRepository) : base(scenarioService, e2eRepository)
        {
            this.e2eSettings = e2eSettings;
        }

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
        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "TextArea", Label = "Export Name", Id = "Name", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "TextArea", Label = "User", Id = "UserId", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Patient", Id = "patientSelectList", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "Completed Exports", Id = "export_info", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "All Sites", Id = "siteSelectList", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "DatePicker", Label = "From", Id = "DiaryStartDate", UiControl = "text", WaitForElement = false },
            new FieldMap() { FieldType = "DatePicker", Label = "To", Id = "DiaryEndDate", UiControl = "text", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Create", Id = "btnSubmit", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Hamburger", Id = "gridMenuButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Table",Label = "Completed Export", Id = "export", UiControl = "Table", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = " Logout", Id = "btnLogout", UiControl = "button", WaitForElement = false },

        };



    }
}


