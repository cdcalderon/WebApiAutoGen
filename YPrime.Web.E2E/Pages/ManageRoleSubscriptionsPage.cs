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
    public class ManageRoleSubscriptionsPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        public readonly TestData testData;
        public override string PageName => "Manage Role Subscriptions";


        public override string PageUrl => $"{e2eSettings.PortalUrl}/Role/SetSubscriptions/{testData.SelectedRole}";



        public ManageRoleSubscriptionsPage(E2ESettings e2eSettings, ScenarioService scenarioService, TestData testData) : base(scenarioService)
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
