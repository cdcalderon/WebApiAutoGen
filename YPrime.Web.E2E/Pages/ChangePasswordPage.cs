using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class ChangePasswordPage : BasePage
    {
        public readonly E2ESettings e2eSettings;

        public override string PageName =>"Change Password";
        public override string PageUrl => $"{e2eSettings.SSOUrl}/Manage/ChangePassword";

        public ChangePasswordPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps()
        {
            throw new NotImplementedException();
        }

        public override string GetDropdownSelectedValue(string control)
        {
            throw new NotImplementedException();
        }
    }
}
