using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using YPrime.Config.Enums;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class ReferenceMaterialSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly ReferenceMaterialPage referenceMaterialPage;

        public ReferenceMaterialSteps(
            ScenarioService scenarioService,
            E2ERepository e2eRepository,
            ScenarioContext scenarioContext,
            ReferenceMaterialPage referenceMaterialPage)
        {
            this.scenarioService = scenarioService;
            this.scenarioContext = scenarioContext;
            this.e2eRepository = e2eRepository;
            this.referenceMaterialPage = referenceMaterialPage;
        }

        [Then(@"following data is displayed in ""(.*)"" page")]
        [Given(@"following data is displayed in ""(.*)"" page")]
        [When(@"following data is displayed in ""(.*)"" page")]
        public void ThenFollowingDataIsDisplayedInPage(string page, Table table)
        {
           
            var tableData=Utilities.TableUtilities.ToDictionary(table);
            foreach(var referenceMaterialType in tableData.Keys)
            {
                #region getting reference material type id and using that to create a id to locate reference material type in the UI
                var referenceMaterialTypeID=referenceMaterialPage.GetReferenceMaterialTypeId(referenceMaterialType);
                var headerID = referenceMaterialPage.FieldMaps().Find(f => f.Label == $"{page} Header").Id.Replace("{}", referenceMaterialTypeID.ToString());
                #endregion
                //verifying if correct reference material type is visible on the page or not
                var actualHeaderValue = referenceMaterialPage.GetWebElementById(headerID).Text;
                var result = referenceMaterialType.Equals(actualHeaderValue);
                Assert.IsTrue(result, result ? $"{referenceMaterialType} is matched" : $"{referenceMaterialType} is not  matched");

                #region verifying if correct reference materials are visible under reference material type
                List<string> actualValues = referenceMaterialPage.GetReferenceMaterialNames(referenceMaterialType,page);
                List<string> expectedValues = tableData[referenceMaterialType].Split(",").ToList();
                result = expectedValues.All(actualValues.Contains) && expectedValues.Count == actualValues.Count;
                Assert.IsTrue(result, result ? $"Reference Material file name is matched" : $"Reference Material file name is not matched");
                #endregion
            }
        }
    }
}
