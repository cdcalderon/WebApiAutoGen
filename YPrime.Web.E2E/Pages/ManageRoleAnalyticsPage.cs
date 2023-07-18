using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class ManageRoleAnalyticsPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        public readonly TestData testData;
        public override string PageName => "Manage Role Analytics";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Role/SetAnalyticsStudyRoles/{testData.SelectedRole}";

        public ManageRoleAnalyticsPage(E2ESettings e2eSettings, ScenarioService scenarioService, TestData testData) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            this.testData = testData;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
        };

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }
    }
}
