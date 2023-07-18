using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SoftwareReleasePage : BasePage
    {
        public readonly E2ESettings e2eSettings;

        public override string PageName => "Software Release Management";
        public override string PageUrl => $"{e2eSettings.PortalUrl}/SoftwareRelease";

        public override int WaitInterval => 1000;
        public SoftwareReleasePage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService) 
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps()
        {
            return new List<FieldMap>()
            {
                new FieldMap() { FieldType = "Inputtextbox", Label = "Release Name inputtextbox", Id = "releaseName" , ValidationId ="invalid-name", UiControl = "input"},
                new FieldMap() { FieldType = "Dropdown", Label = "Software Version", Id = "softwareVersion", ValidationId ="invalid-version", UiControl = "select" },
                new FieldMap() { FieldType = "Dropdown", Label = "Configuration Version", Id = "configurationVersion", ValidationId ="invalid-config-version", UiControl = "select" },
                new FieldMap() { FieldType = "MultiDropdown", Label = "Device Type", Id = "deviceTypeIds", UiControl = "multiselect", Position = 0, FieldContainerElement = "deviceTypeMultiSelect", WaitForElement = true },
                new FieldMap() { FieldType = "MultiDropdown", Label = "Country(s)", Id = "countryIds", UiControl = "multiselect", Position = 1, FieldContainerElement = "countryMultiSelect" },
                new FieldMap() { FieldType = "MultiDropdown", Label = "Site(s)", Id = "siteIds", UiControl = "multiselect", Position = 2, FieldContainerElement = "siteMultiSelect" },
                new FieldMap() { FieldType = "MultiDropdown", Label = "Device(s)", Id = "deviceIds", UiControl = "multiselect", Position = 3, FieldContainerElement = "deviceMultiSelect" },
                new FieldMap() { FieldType = "Toggle", Label = "Required", Id = "Required", UiControl = "toggle" },
                new FieldMap() { FieldType = "Toggle", Label = "Study Wide", Id = "StudyWide", UiControl = "toggle" },
                new FieldMap() { FieldType = "Button", Label = "Create Release", Id = "confirm", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Confirm", Id = "saveBtn", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Cancel", Id = "confirmModalCancelBtn", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "X", Id = "confirmModalCloseBtn", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Release", Id = "releaseGrid", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Deactivate Release", Id = "deactivateRelease", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Yes", Id = "deactivate", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "No", Id = "btnNo", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Review", Id = "confirmationModal", UiControl = "button" }

            };
        }

        public IWebElement BtnSubjects => ScenarioService.ChromeDriver.FindElement(By.XPath("//a[@menu-group='Patients']"));

        public IWebElement GetControlValidationElement(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).ValidationId;
            var element = GetWebElementById(elemId);

            return element;
        }
        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.ClassName("selected"));

            return selected.Text;
        }

        public ReadOnlyCollection<IWebElement> GetMultiDropdownSelectedValues(IWebElement control)
        {
            var elements = control.FindElements(By.ClassName("select2-selection__choice"));

            return elements;
        }
    }
}
