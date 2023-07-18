using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SubmitDataCorrectionPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        private readonly E2ERepository e2eRepository;
        public override string PageName => "Submit Data Correction";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Correction/Create";

        public override int WaitInterval => 1000;

        public SubmitDataCorrectionPage(
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
            new FieldMap() { FieldType = "Button", Label = "Back", Id = "backButton", UiControl = "button" },
            new FieldMap() { FieldType = "Button", Label = "Submit", Id = "SubmitCorrection", UiControl = "button" }
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
