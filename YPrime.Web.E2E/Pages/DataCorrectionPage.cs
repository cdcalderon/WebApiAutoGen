using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class DataCorrectionPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Data Correction";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Correction/#";

        public DataCorrectionPage(
            E2ESettings e2eSettings, 
            ScenarioService scenarioService) 
            : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps()
        {
            return new List<FieldMap>()
            {
                new FieldMap() { FieldType = "Button", Label = "Add New DCF", Id = "btnAddDcf", UiControl = "button" },
                new FieldMap() { FieldType = "Button", Label = "Correction", Id = "queueCorrectionsGrid", UiControl = "button" },
               new FieldMap() { FieldType = "Button", Label = " Logout", Id = "btnLogout", UiControl = "button", WaitForElement = false },
            };
        }

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

    }
}
