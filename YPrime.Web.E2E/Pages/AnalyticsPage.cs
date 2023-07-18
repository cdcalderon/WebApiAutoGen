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
    public class AnalyticsPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => "Analytics";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Analytics/Index";

        public override string GetDropdownSelectedValue(string control)
        {
            var result = string.Empty;

            var matchingFieldMap = FieldMaps().First(fm => fm.Label == control);

            if (matchingFieldMap.UiControl == "metro_dropdown")
            {
                var parent = GetWebElementById(matchingFieldMap.Id).GetParent().GetParent();

                var selectedElement = parent.FindElement(By.ClassName("selected"));

                result = selectedElement.Text;
            }

            return result;

        }

        public AnalyticsPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps()
        {
            return new List<FieldMap> { };
        }
    }
}
