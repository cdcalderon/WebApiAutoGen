using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class AddSubjectPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Add New Subject";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Patient/Create";

        public override string GetDropdownSelectedValue(string control)
        {
            var result = string.Empty;
            var matchingFieldMap = FieldMaps().First(fm => fm.Label == control);
            if (matchingFieldMap.UiControl == "metro_dropdown" || matchingFieldMap.UiControl == "select")
            {
                var parent = GetWebElementById(matchingFieldMap.Id).GetParent().GetParent();
                var selectedElement = parent.FindElement(By.ClassName("selected"));
                result = selectedElement.Text;
            }
            return result;
        }

        public AddSubjectPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }


        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Inputtextbox", Label = "Subject Number", Id = "subjectNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Language", Id = "LanguageId", UiControl = "metro_dropdown", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = "Yes, subject will use their personal device", Id = "btnSubjectUserPersonalDeviceTrue", UiControl = "radio", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "No, subject will use a provisioned device", Id = "btnSubjectUserPersonalDeviceFalse", UiControl = "radio", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Create", Id = "btnSubmitCreate", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Cancel", Id = "btnCancelCreate", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "10001", Id = "siteNumberContainer", UiControl = "textarea", WaitForElement = true },
             new FieldMap() { FieldType = "Inputtextbox", Label = "Weight", Id = "PatientAttributes_2__AttributeValue", UiControl = "input", WaitForElement = false },
        };
        public override IWebElement GetOnPageElementByLabel(string label)
        {
            var matchingFieldMap = base.GetOnPageElementByLabel(label);

            if (matchingFieldMap != null)
            {
                return matchingFieldMap;
            }

            var formContainer = GetWebElementById("addSubjectFormContainer");

            var attributeContainers = formContainer.FindElements(By.ClassName("dynamic-attribute"));

            foreach (var container in attributeContainers)
            {
                var controlLabels = container.FindElements(By.ClassName("control-label"));

                foreach (var controlLabel in controlLabels)
                {
                    if (controlLabel.Text.ToUpper() == label.ToUpper())
                    {
                        var matchingInputContainer = container.FindElement(By.ClassName("attribute-input"));

                        return matchingInputContainer;
                    }
                }
            }

            return null;
        }

        public void EnterSubjectNumber(string subjectNumber)
        {
            var elemId = FieldMaps().Find(f => f.Label == "Subject").Id;
            var element = GetWebElementById(elemId);

            element.SendKeys(subjectNumber);
        }        
    }
}
