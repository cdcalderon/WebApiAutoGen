using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;
namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class BulkEditSiteSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly BulkEditSitePage bulkEditSitePage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;

        public BulkEditSiteSteps(
           ScenarioService scenarioService,
           BulkEditSitePage bulkEditSitePage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.bulkEditSitePage = bulkEditSitePage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
        }

        [Given(@"the following columns is displayed within Bulk Edit Site grid")]
        public void GivenTheFollowingColumnsIsDisplayed(Table table)
        {

            bulkEditSitePage.verifyColumnsNameDisplayed(table);
        }

    }
}
