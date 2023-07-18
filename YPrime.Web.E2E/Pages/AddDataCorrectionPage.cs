using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class AddDataCorrectionPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        private readonly E2ERepository e2eRepository;
        public override string PageName => "Create Data Correction";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Correction/Create";

        public override int WaitInterval => 1000;

        public AddDataCorrectionPage(
            E2ESettings e2eSettings,
            ScenarioService scenarioService,
            E2ERepository e2eRepository)
            : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            this.e2eRepository = e2eRepository;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Dropdown", Label = "Site Name", Id = "siteDropdown", UiControl = "select" },
            new FieldMap() { FieldType = "Dropdown", Label = "Subject", Id = "patientDropdown", UiControl = "select_", WaitForElement = true },
            new FieldMap() { FieldType = "Dropdown", Label = "Type Of Correction", Id = "correctionTypeDropdown", UiControl = "select", WaitForElement = true },
            new FieldMap() { FieldType = "Dropdown", Label = "Select a Questionnaire", Id = "questionnaireDropdownContainer", UiControl = "select_basic", WaitForElement = true },
            new FieldMap() { FieldType = "Dropdown", Label = "Please select a subject visit", Id = "CorrectionDiv", UiControl = "select_basic", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "Next", Id = "SubmitCorrection", UiControl = "button" },
            new FieldMap() { FieldType = "Inputtextbox", Label = "User Name", Id = "ElectronicSignatureUserName", UiControl = "input" },
            new FieldMap() { FieldType = "Password", Label = "Password", Id = "ElectronicSignaturePassword", UiControl = "input" },
            new FieldMap() { FieldType = "TextArea", Label = "Reason For Correction", Id = "ReasonForCorrection", UiControl = "textarea" },
            new FieldMap() { FieldType = "Button", Label = "Back", Id = "backButton", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = "Submit", Id = "SubmitCorrection", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = "Approve", Id = "SubmitCorrection", UiControl = "button" },
            new FieldMap() { FieldType = "DatePicker", Label = "Date of Questionnaire Completion", Id = "DCFPaperQuestionnaireDate", UiControl = "text", WaitForElement = false }
        };

        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.ClassName("selected"));

            return selected.Text;
        }
    }
}
