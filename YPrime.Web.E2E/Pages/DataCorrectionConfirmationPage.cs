using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class DataCorrectionConfirmationPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        private readonly E2ERepository e2eRepository;
        public override string PageName => "Data Correction Confirmation";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Confirmation/Index/";

        public override int WaitInterval => 1000;

        public DataCorrectionConfirmationPage(
            E2ESettings e2eSettings,
            ScenarioService scenarioService,
            E2ERepository e2eRepository)
            : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap> { };

        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.ClassName("selected"));

            return selected.Text;
        }
    }
}
