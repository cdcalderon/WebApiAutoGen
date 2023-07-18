using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Pages;
using TechTalk.SpecFlow.Assist;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DeviceDetailsSteps
    {
        private readonly ScenarioService _scenarioService;
        private readonly ScenarioContext _scenarioContext;
        private readonly DeviceDetailsPage _deviceDetailsPage;
        private readonly E2ERepository e2eRepository;
        public DeviceDetailsSteps(ScenarioContext scenarioContext, DeviceDetailsPage deviceDetailsPage, ScenarioService scenarioService, E2ERepository e2eRepository)
        {
            _scenarioContext = scenarioContext;
            _deviceDetailsPage = deviceDetailsPage;
            _scenarioService = scenarioService;
            this.e2eRepository = e2eRepository;
        }

        [Given(@"Subject ""(.*)"" has completed ""(.*)"" questionnaire for question ""(.*)"" and choice ""(.*)""")]
        [Given(@"Subject ""(.*)"" has completed ""(.*)"" questionnaire for question ""(.*)"" and value ""(.*)""")]
        public async Task GivenSubjectHasCompletedQuestionnaireForQuestionAndChoice(string p0, string p1, string p2, string p3)
        {            
            await e2eRepository.AddAnswer(p0,p1, p2, p3);
        }

        [Given(@"Subject ""(.*)"" has completed ""(.*)"" questionnaire for question ""(.*)"" and choice ""(.*)"" adding to existing diary entry")]
        [Given(@"Subject ""(.*)"" has completed ""(.*)"" questionnaire for question ""(.*)"" and value ""(.*)"" adding to existing diary entry")]
        public async Task GivenSubjectHasCompletedQuestionnaireForQuestionAndChoiceMultipleAnswers(string p0, string p1, string p2, string p3)
        {
            await e2eRepository.AddAnswer(p0, p1, p2, p3, true);
        }

        [Then(@"following data is displayed in device details grid")]
        public void ThenFollowingDataIsDisplayedInTheGrid(Table table)
        {
            foreach (var row in table.Rows)
            {
                var syncDate = row["Sync Date"];
                var version = row["Last Reported Software Version"];
                var configuration = row["Last Reported Configuration Version"];
                var action = row["Sync Action"];

                //wait for the grid to load
                IWebElement waitElem = _scenarioService.ChromeDriver.FindElement(By.CssSelector(@"tbody tr[role=""row""]"));

                IWebElement rowElem = _scenarioService.ChromeDriver.FindElement(By.CssSelector(@"tbody tr[role=""row""]:nth-child(1)"));
                
                Assert.IsNotNull(rowElem);                

                var index = 0;
                foreach (var columnElem in rowElem.FindElements(By.CssSelector("td")))
                {
                    var colText = columnElem.Text;
                    var elemFull = columnElem.TagName;
                    switch (index)
                    {
                        case 0:
                            if (syncDate == "(Current Date)")
                            {
                                DateTime dataDate;
                                DateTime.TryParse(colText, out dataDate);
                                Assert.IsTrue(dataDate.Date.Equals(DateTime.Today), "Last Data Sync Date");
                            }
                            else
                            {
                                Assert.IsTrue(string.IsNullOrWhiteSpace(colText), colText);
                            }
                            break;
                        case 1:
                            Assert.AreEqual(version.ToUpper(), colText.ToUpper(), "Last Reported Software Version");
                            break;
                        case 2:
                            Assert.AreEqual(configuration.ToUpper(), colText.ToUpper(), "Last Reported Configuration Version");
                            break;
                        case 3:
                            Assert.AreEqual(action.ToUpper(), colText.ToUpper(), "Sync Action");
                            break;
                        default:
                            break;
                    }
                    index++;
                }
            }
        }

    


        [Then(@"""(.*)"" is displayed on device details")]
        public void ThenIsDisplayed(string p0)
        {
            var button = _deviceDetailsPage.GetWebElementById("detailTitle");
            Assert.IsTrue(button.Text == p0);
        }

        [Then(@"following data is displayed on Device Details page")]
        public void ThenValueIsDisplayedForField(Table table)
        {
            var fields = table.CreateSet<FieldAndValueSet>();
            var dees = _scenarioService.ChromeDriver.FindElements(By.TagName("dd"));
            var dtees = _scenarioService.ChromeDriver.FindElements(By.TagName("dt"));
            Assert.AreEqual(dees.Count, dtees.Count);
            var found = false;
            foreach (var field in fields)
            {
                for (int index = 0; index < dees.Count; index++)
                {
                    var fieldTitle = dtees[index].Text;
                    var fieldValue = dees[index].Text;
                    if (fieldTitle.ToUpper() == field.Field.ToUpper())
                    {
                        if (field.Value == "(Current Date)")
                        {
                            DateTime dataDate;
                            DateTime.TryParse(fieldValue, out dataDate);
                            Assert.IsTrue(dataDate.Date.Equals(DateTime.Today), "Last Data Sync Date");
                        }
                        else
                        {
                            Assert.IsTrue(field.Value.ToUpper() == fieldValue.ToUpper(), field.Field);
                        }
                        found = true;
                    }
                }
                Assert.IsTrue(found, field.Field);
            }
        }

        class FieldAndValueSet
        {
            public string Value { get; set; }
            public string Field { get; set; }

        }


        
        [Then(@"following data is displayed in the data field table")]
        public void ThenFollowingDataIsDisplayedInTheDataFieldTable(Table table)
        {
            foreach (var row in table.Rows)
            {
                var dataFound = false;

                var label = row["Label"];
                var value = row["Value"];
                var labelElems = _scenarioService.ChromeDriver.FindElements(By.ClassName("control-label"));

                foreach (var labelElem in labelElems)
                {
                    if (label.Trim().ToUpper() == labelElem.Text.Trim().ToUpper())
                    {
                        var parentElem = labelElem.GetParent();
                        var valueElem = parentElem.FindElement(By.ClassName("detail-display-answer"));

                        var valText = valueElem.Text;
                        dataFound = true;
                        Assert.AreEqual(label, labelElem.Text);
                        Assert.AreEqual(value, valueElem.Text);
                        break;
                    }
                }

                Assert.IsTrue(dataFound);
                
            }
        }

    }
}
