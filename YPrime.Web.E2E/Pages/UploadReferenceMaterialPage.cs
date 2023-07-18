using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class UploadReferenceMaterialPage : BasePage
    {

        public readonly E2ESettings e2eSettings;
        public readonly E2ERepository e2eRepository;

        public override string PageUrl => $"{e2eSettings.PortalUrl}/ReferenceMaterial/Upload";

        public override string PageName => "Upload Reference Material";

        public UploadReferenceMaterialPage(E2ESettings e2eSettings, ScenarioService scenarioService, E2ERepository e2eRepository) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            this.e2eRepository = e2eRepository;
        }

        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.XPath("./parent::span//following-sibling::span[@class='selected']"));

            return selected.Text;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "TextArea", Label = "Name", Id = "referenceMaterialName", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Select a file", Id = "fileUploadText", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Change file", Id = "fileUploadText", UiControl = "button", WaitForElement = false },
            new FieldMap { FieldType = "Dropdown", Label = "ReferenceMaterialTypeId", Id = "referenceMaterialSelectList", UiControl = "metro_dropdown" },
            new FieldMap { FieldType = "Dropdown", Label = "Frequently Asked Questions", Id = "referenceMaterialSelectList", UiControl = "metro_dropdown" },
            new FieldMap() { FieldType = "TextArea", Label = "User", Id = "UserId", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "File", Label = "FileName", Id = "fileName", UiControl = "FileName", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Add New", Id = "btnSubmit", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "GridMenuicon", Id = "gridMenuButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Excel", Id = "gridExcelButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "CSV", Id = "gridCsvButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "PDF", Id = "gridPdfButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Print", Id = "gridPrintButton", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Link", Label = "Reference Materials tab", Id = "referenceMaterials_info", UiControl = "TextArea", WaitForElement = false },
            new FieldMap() { FieldType = "Table",Label = "Reference Material", Id = "referenceMaterials", UiControl = "Table", WaitForElement = true },
            new FieldMap { FieldType = "Dropdown", Label = "Reference Material Type", Id = "createReferenceMaterial", UiControl = "metro_dropdown" },
            new FieldMap { FieldType = "Dropdown", Label = "Reference Materials", Id = "//span[@class='selected']", UiControl = "",LocatorType="xpath", LocatorValue="//span[@class='selected']"}


        };

        class setColumnName
        {
            public string ColumnName { get; set; }
            public string ButtonName { get; set; }
        }

        public void VerifyElementForExport(Table table)
        {
            var fields = table.CreateSet<setColumnName>();
            foreach (var field in fields)
            {
                string actualValue;
                var expectedValue = field.ButtonName;
                var elemId = FieldMaps().Find(f => f.Label == field.ButtonName).Id;
                var actualElement = ScenarioService.ChromeDriver.FindElement(By.Id(elemId));
                actualValue = actualElement.GetAttribute("Title");
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        public void VerifyElementForVisibilty(Table table)
        {
            var fields = table.CreateSet<setColumnName>();
            foreach (var field in fields)
            {
                string actualValue;
                var expectedValue = field.ButtonName;
                var actualElement = ScenarioService.ChromeDriver.FindElement(By.LinkText(expectedValue));
                actualValue = actualElement.Text;
                Assert.AreEqual(expectedValue, actualValue);     
            }
        }


        public void TextDisplayedInField(string text, string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = ScenarioService.ChromeDriver.FindElement(By.XPath("//div[@class='col-lg-6 form-group']//input[@id='" + elemId + "']"));
            var inputValue = element.GetAttribute("value");
            Assert.IsTrue(inputValue == text);
        }

    }
}


