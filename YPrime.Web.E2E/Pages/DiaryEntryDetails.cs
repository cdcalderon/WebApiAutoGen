using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class DiaryEntryDetailsPage : BasePage
    {

        private readonly E2ESettings e2eSettings;

        public override string PageName => "Diary Entry Details";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/DiaryEntries/Details/";

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }
        public DiaryEntryDetailsPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>()
        {
            new FieldMap() { FieldType = "Table", Label = "Response", Id = "diary-responses-{}", UiControl = "table", WaitForElement = false }
        };
    }
}
