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
    public class SiteSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly SitePage sitePage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly AddSitePage addSitePage;

        public SiteSteps(
           ScenarioService scenarioService,
           SitePage sitePage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext, AddSitePage addSitePage)
        {
            this.scenarioService = scenarioService;
            this.sitePage = sitePage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
            this.addSitePage = addSitePage;
        }

        [Given(@"the data grid values given under column name is displayed as")]
        public void GivenTheDataGridValuesGivenUnderColumnNameIsDisplayedAs(Table table)
        {
            sitePage.VerifyColumnValues(table);
        }

        [StepDefinition(@"I verify Enabled until date is ""(.*)""")]
        public void IVerifyEnabledUntilDate(string date)
        {
            addSitePage.VerifyEnabledUntilDate(date);
        }

        [StepDefinition(@"I enable Web Backup until ""([^""]*)"" for the site ""([^""]*)""")]
        public void IEnableWebBackupForTheSite(string date, string siteNumber)
        {
            string resolvedDate = addSitePage.ResolveDate(date, "yyyy-MM-dd");
            e2eRepository.UpdateWebBackupExpireDate(resolvedDate, siteNumber);
        }
    }
}
