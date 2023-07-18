using OpenQA.Selenium;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SiteImportPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        public override string PageName => "Site Import";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Site/Import";

        public SiteImportPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Button", Label = "Validate Import", Id = "validateImport", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Import", Id = "import", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Delimiter", Id = "importFileDelimiter", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Dropdown", Label = "Extension", Id = "importFileExtension", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "download", Id = "downloadImportTemplate", UiControl = "button", WaitForElement = false },
            
        };

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
    }
}

