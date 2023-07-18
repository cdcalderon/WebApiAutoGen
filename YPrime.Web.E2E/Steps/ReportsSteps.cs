using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YPrime.Web.E2E.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;
namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class ReportsSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly ReportsPage reportsPage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private readonly E2ESettings e2eSettings;
      

        public ReportsSteps
           (
           ScenarioService scenarioService,
           ReportsPage reportsPage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext,
           E2ESettings e2eSettings
           )
        {
            this.scenarioService = scenarioService;
            this.reportsPage = reportsPage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
            this.e2eSettings = e2eSettings;
        }

        [Given(@"I click on ""(.*)"" from the Report List")]
        [When(@"I click on ""(.*)"" from the Report List")]
        [Then(@"I click on ""(.*)"" from the Report List")]
        public void GivenIClickOnFromTheReportList(string reportName) => reportsPage.ClickOnReportList(reportName);

        [Given(@"the following columns is displayed within Report index grid")]
        [When(@"the following columns is displayed within Report index grid")]
        [Then(@"the following columns is displayed within Report index grid")]
        public void AndColumnsIsDisplayedWithinReportIndexGrid(Table table)
        {
            reportsPage.verifyColumnsNameDisplayed(table);
        }

        [Given(@"sites to which user has access will be displayed")]
        [When(@"sites to which user has access will be displayed")]
        [Then(@"sites to which user has access will be displayed")]
        public void AndSitesToWhichUserHasAccessWillBeDisplayed(Table table)
        {
            reportsPage.verifySiteVisibility(table);
        }

        [Then(@"Newly completed DCF is displayed in DCF Status Report")]
        public void ThenNewlyCompletedDCFIsDisplayedInDCFStatusReport(Table table)
        {
            reportsPage.WaitForAjax();

            var tblData = table.CreateSet<DcfStatusReportGridMap>().ToList();
            var grid = reportsPage.GetWebElementsByCss(".dataTable tbody tr");

            foreach (var row in grid)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                var index = grid.IndexOf(row);
                var dataRow = tblData[index];

                if (dataRow.Subject == "<SubjectId>")
                {
                    dataRow.Subject = (string)this.scenarioContext["<SubjectId>"];
                }

                if (dataRow.DcfOpenedDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[5].Text));
                    var validDate = DateTime.TryParse(gridCells[5].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.DcfOpenedDate, gridCells[5].Text);
                }

                if (dataRow.DcfClosedDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[6].Text));
                    var validDate = DateTime.TryParse(gridCells[6].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.DcfClosedDate, gridCells[6].Text);
                }

                Assert.AreEqual(dataRow.Site, gridCells[1].Text);
                Assert.AreEqual(dataRow.Subject, gridCells[2].Text);
                Assert.AreEqual(dataRow.DcfType, gridCells[3].Text);
                Assert.AreEqual(dataRow.DcfStatus, gridCells[4].Text);
                Assert.AreEqual(dataRow.PendingApprover, gridCells[7].Text);
                Assert.AreEqual(dataRow.CompletedApprovals, gridCells[8].Text);
                Assert.AreEqual(dataRow.DaysDcfOpen, gridCells[9].Text);
            }
        }

        [Given(@"Key ""([^""]*)"" is set with value ""([^""]*)"" in configuration for ""([^""]*)""")]
        public void GivenKeyIsSetWithValueForInConfiguration(string visitOrder, string visitValue, string visitName)
        {
            
            var actualValue = reportsPage.GetJSONFileValueOfVisitFromKey(visitName);
            if (actualValue == null)
                Assert.Fail();
            else
                Assert.AreEqual(actualValue, visitValue);
        }

        [Given(@"""([^""]*)"" questionnaire has been configured in ""([^""]*)""")]
        public void GivenQuestionnaireHasBeenConfiguredIn(string sum, string visitName)
        {   
            int count = reportsPage.GetVisitQuestionaireCount(visitName);
            Assert.AreEqual(count, int.Parse(sum));
        }

        [Then(@"Answers displayed in Answer Audit Report")]
        public void ThenAnswersDisplayedInAnswerAuditReport(Table table)
        {
            reportsPage.WaitForAjax();

            var tblData = table.CreateSet<AnswerAuditReportGridMap>().ToList();
            var grid = reportsPage.GetWebElementsByCss(".dataTable tbody tr");

            foreach (var row in grid)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                var index = grid.IndexOf(row);
                var dataRow = tblData[index];

                if (dataRow.SubjectNumber == "<SubjectId>")
                {
                    dataRow.SubjectNumber = (string)this.scenarioContext["<SubjectId>"];
                }

                if (dataRow.DiaryDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[3].Text));
                    var validDate = DateTime.TryParse(gridCells[3].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.DiaryDate, gridCells[3].Text);
                }

                if (dataRow.ChangeDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[10].Text));
                    var validDate = DateTime.TryParse(gridCells[10].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.ChangeDate, gridCells[9].Text);
                }

                Assert.AreEqual(dataRow.Protocol, gridCells[0].Text);
                Assert.AreEqual(dataRow.SiteNumber, gridCells[1].Text);
                Assert.AreEqual(dataRow.SubjectNumber, this.scenarioContext["<SubjectId>"]);
                Assert.AreEqual(dataRow.Questionnaire, gridCells[4].Text);
                Assert.AreEqual(dataRow.Question, gridCells[5].Text);
                Assert.AreEqual(dataRow.NewValue, gridCells[7].Text);
                Assert.AreEqual(dataRow.ChangeReason, gridCells[8].Text);
                Assert.AreEqual(dataRow.ChangeBy, gridCells[9].Text);
                Assert.AreEqual(dataRow.CorrectionReason, gridCells[11].Text);
            }
        }

        [Then(@"I select ""(.*)"" from ""(.*)"" report dropdown")]
        public void ThenISelectFromReportDropdown(string item, string dropdown)
        {
            if (dropdown == "Subject Number")
            {
                item = (string)this.scenarioContext["<SubjectId>"];
            }
            reportsPage.SelectFromDropdown(dropdown, item);
        }

        [Then(@"Patient status update listed in Subject Information Audit Report")]
        public void ThenPatientStatusUpdateListedInSubjectInformationAuditReport(Table table)
        {
            reportsPage.WaitForAjax();
            reportsPage.ThreadSleep();

            var tblData = table.CreateSet<SubjectInformationAuditReportGridMap>().ToList();
            var grid = reportsPage.GetWebElementsByCss(".dataTable tbody tr");

            foreach (var row in grid)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                var index = grid.IndexOf(row);
                var dataRow = tblData[index];

                if (dataRow.SubjectNumber == "<SubjectId>")
                {
                    dataRow.SubjectNumber = (string)this.scenarioContext["<SubjectId>"];
                }                

                if (dataRow.ChangeDate == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[8].Text));
                    var validDate = DateTime.TryParse(gridCells[8].Text, out var date);

                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.ChangeDate, gridCells[8].Text);
                }

                if (dataRow.NewValue == "<SubjectId>")
                {
                    dataRow.NewValue = (string)this.scenarioContext["<SubjectId>"];
                }

                if (dataRow.NewValue == "(Current Date)")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(gridCells[5].Text));
                    var validDate = DateTime.TryParse(gridCells[5].Text, out var date);
                    Assert.IsTrue(validDate);
                }
                else
                {
                    Assert.AreEqual(dataRow.NewValue, gridCells[5].Text);
                }

                Assert.AreEqual(dataRow.SiteNumber, gridCells[1].Text);
                Assert.AreEqual(dataRow.SubjectNumber, this.scenarioContext["<SubjectId>"]);
                Assert.AreEqual(dataRow.SubjectAttribute, gridCells[3].Text);
                Assert.AreEqual(dataRow.OldValue, gridCells[4].Text);
                Assert.AreEqual(dataRow.ChangeReason, gridCells[6].Text);
                Assert.AreEqual(dataRow.ChangeBy, gridCells[7].Text);
            }

        }

        [Then(@"I enter ""(.*)"" on ""(.*)"" search textbox on the report table")]
        public void ThenIEnterOnSearchTextboxFromTheTheReportTable(string searchText, string searchHeader)
        {            
            reportsPage.SearchFromTable(searchText, searchHeader);
        }

        [Then(@"I click on Show Entries per grid dropdown")]
        public void IClickOnShowEntriesDropdown()
        {
            reportsPage.ClickOnGridPageItemByClass("dataTables_length");
        }

        [Then(@"I select ""(.*)"" from Show Entries per grid dropdown")]
        public void ISelectValueFromShowEntriesDropdown(string value)
        {
            reportsPage.SetShowEntriesSelectedValue(value);
        }

        [Given(@"Subject number ""(.*)"" is not visible")]
        [Then(@"Subject number ""(.*)"" is not visible")]
        public void GivenSubjectNumberIsNotVisible(string SubjectNumber)
        {
            var SubjectNumberList = reportsPage.GetWebElementsByXPath("//tbody/tr/td[2]");
            foreach (var Subjectoption in SubjectNumberList)
            {
                string DisplayedOption = Subjectoption.Text;
                if (SubjectNumber == DisplayedOption)
                {
                    Assert.Fail();
                }
            }
        }

        

        [Then(@"I click on the ""([^""]*)"" for DCF Type ""([^""]*)""")]
        public void ThenIClickOnForDCFType(string dcfReport, string dcfType)
        {
            string expectedDCFNumber = e2eRepository.GetDataCorrectionNumber(dcfType);
            string dcfTypeValue = expectedDCFNumber;
            var dcfNumberList = scenarioService.ChromeDriver.FindElements(By.XPath("//table//tbody//tr//td"));
            var element = dcfNumberList.SingleOrDefault(d => d.Text.TrimStart('0') == dcfTypeValue);
            reportsPage.ClickOnElement(element);
            if (dcfReport.Contains("link"))
            {
                var elementLink = scenarioService.ChromeDriver.FindElement(By.LinkText(element.Text));
                elementLink.Click();

            }
            else 
            {
                element.Click();
            }

        }

        [Then(@"I click on ""([^""]*)"" button for DCF Report")]
        public void ThenIClickOnButtonForDCFReport(string p0)
        {
            var element = reportsPage.GetWebElement("xpath", "//div[@id='9087c7cb-771c-41ee-9b8c-2213d8b9a02b_wrapper']//button");
            element.Click();
        }

        [Then(@"grid menu for DCF Report is displayed with the following functionality for export")]
        public void VerifyElementForExportforDCFReport(Table table)
        {
            var fields = table.CreateSet<SetColumnName>();
            foreach (var field in fields)
            {
                string actualValue = null;
                var expectedValue = field.ButtonName;
                switch (expectedValue)
                {
                    case "Excel":
                        actualValue = scenarioService.ChromeDriver.FindElement(By.CssSelector("li.dt-button.buttons-excel.buttons-html5")).GetAttribute("Title");
                        break;
                    case "CSV":
                        actualValue = scenarioService.ChromeDriver.FindElement(By.CssSelector("li.dt-button.buttons-csv.buttons-html5")).GetAttribute("Title");
                        break;
                    case "PDF":
                        actualValue = scenarioService.ChromeDriver.FindElement(By.CssSelector("li.dt-button.buttons-pdf.buttons-html5")).GetAttribute("Title");
                        break;
                    case "Print":
                        actualValue = scenarioService.ChromeDriver.FindElement(By.CssSelector("li.dt-button.buttons-print")).GetAttribute("Title");
                        break;
                }
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [Then(@"I click on ""([^""]*)"" button to generate ""([^""]*)"" in ""([^""]*)"" format file to save in Export Evidence folder for DCF Report")]
        public void ThenIClickOnButtonToGenerateInFormatFileToSaveInExportEvidenceFolderForDCFReport(string control, string fileName, string format)
        {
            var element = reportsPage.GetWebElement("css", "li.dt-button.buttons-pdf.buttons-html5");
            element.Click();

            FileInfo file = reportsPage.GetDownloadFileInfo(fileName, format);

            if (file == null)
                Assert.Fail();
            else
                Assert.AreEqual(file.Name, fileName + format);
        }


        [Then(@"I navigate to ""([^""]*)"" actual details for DCF Type ""([^""]*)""")]
        public void ThenINavigateToActualDetailsForDCFType(string dcfReport, string dcfType)
        {
            string expectedDCFNumberDetails = e2eRepository.GetCorrectionWorkflowId(dcfType);
            var expectedUrl = $"{e2eSettings.PortalUrl}/CorrectionWorkflow/Workflow/" + expectedDCFNumberDetails;
            var currentUrl = scenarioService.ChromeDriver.Url;
            Assert.AreEqual(currentUrl, expectedUrl);

        }

        [Given(@"bar graph displays row containing AverageTime for each completed Questionnaire")]
        [Then(@"bar graph displays row containing AverageTime for each completed Questionnaire")]
        public void GivenBarGraphDisplaysRowContainingAverageTimeForEachCompletedQuestionnaire(Table table)
        {
            List<SetChartData> expectedChartData = table.CreateSet<SetChartData>().ToList();
            var barElement = reportsPage.GetWebElement("xpath", "//div[@class='chartTooltip']//following-sibling::input");
            string elementValues = barElement.GetAttribute("value");
            var deserializedData = JsonConvert.DeserializeObject<ChartDataObjectModel>(elementValues);
            Dictionary<double, string> xData = deserializedData.XLabels;
            Dictionary<float, float> yData = deserializedData.ChartSeries[0].SeriesDataPoints.Select(x => x).ToDictionary(x => x.X, y => y.Y);
            List<SetChartData> actualChartData = new List<SetChartData>();

            foreach (var xRow in xData)
            {
                foreach (var yRow in yData)
                {
                    if (xRow.Key == yRow.Key)
                    {
                        actualChartData.Add(new SetChartData { Xaxis = xRow.Value, Yaxis = yRow.Value.ToString("F1") });
                        break;
                    }
                }
            }
            int a = 5;
            var b = Convert.ToDecimal(a);
            expectedChartData = expectedChartData.OrderBy(o => o.Yaxis).ToList();
            actualChartData = actualChartData.OrderBy(o => o.Yaxis).ToList();
            if (expectedChartData.Count == actualChartData.Count)
            {
                for (int i = 0; i < expectedChartData.Count; i++)
                {
                    var rowEqual = (expectedChartData[i].Xaxis.Trim() == actualChartData[i].Xaxis.Trim() && expectedChartData[i].Yaxis.Trim() == actualChartData[i].Yaxis.Trim());
                    Assert.IsTrue(rowEqual);
                }
            }
            else
                Assert.Fail();
        }


        class SetColumnName
        {
            public string ButtonName { get; set; }
        }
        class SetChartData
        {
            public string Xaxis { get; set; }
            public string Yaxis { get; set; }
        }
    }
}