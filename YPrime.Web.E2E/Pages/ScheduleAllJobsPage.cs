using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class ScheduleAllJobsPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public ScheduleAllJobsPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override string PageName => "Schedule All Jobs";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/HangfireJobs/ScheduleAllJobs";

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
