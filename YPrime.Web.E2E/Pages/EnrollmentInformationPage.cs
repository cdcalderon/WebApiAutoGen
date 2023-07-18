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
    public class EnrollmentInformationPage : BasePage
    {
     
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Enrollment Information";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Patient";

        public override string GetDropdownSelectedValue(string control)
        {
            var result = string.Empty;

            return result;

        }

        public EnrollmentInformationPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Button", Label = "Email icon", Id = "webConfirmationEmailBtn", UiControl = "button", WaitForElement = false },
            
        };

    }
}
