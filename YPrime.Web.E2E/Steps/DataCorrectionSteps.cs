using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Models.Api;
using YPrime.Web.E2E.Pages;
using YPrime.Config.Enums;
using YPrime.Data.Study.Constants;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DataCorrectionSteps
    {
        private const string CurrentDateChoice = "(current date)";
        private readonly ScenarioService scenarioService;
        private readonly DataCorrectionPage dataCorrectionPage;
        private readonly E2ERepository e2eRepository;
        private readonly TestData testData;
        private readonly ApiTestData apiTestData;
        private DateTime? _currentDateTime;

        private List<string> placeholderTextOptions = new List<string>() { "pleaseselect", "pleaseprovidearesponse" };

        public DataCorrectionSteps(
            ScenarioService scenarioService,
            DataCorrectionPage dataCorrectionPage,
            E2ERepository e2eRepository,
            TestData testData,
            ApiTestData apiTestData)
        {
            this.scenarioService = scenarioService;
            this.dataCorrectionPage = dataCorrectionPage;
            this.e2eRepository = e2eRepository;
            this.testData = testData;
            this.apiTestData = apiTestData;
        }

        private DateTime CurrentDateTime => _currentDateTime.HasValue ? _currentDateTime.Value : (_currentDateTime = DateTime.Now).Value;

        [When(@"I click on ""(.*)"" in data correction field for ""(.*)"" question")]
        [Given(@"I click on ""(.*)"" in data correction field for ""(.*)"" question")]
        [Then(@"I click on ""(.*)"" in data correction field for ""(.*)"" question")]
        public async Task GivenIClickControlInCorrectionField(string controlType, string questionTitle)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.ThreadSleep();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {
                        case "datepicker":
                        case "inputtextbox":
                        case "numberspinner":
                            var input = gridCells[1].FindElement(By.TagName("input"));
                            input.Click();
                            break;
                        case "dropdown":
                            var dropdown = gridCells[1].FindElement(By.ClassName("metro"));
                            dropdown.Click();
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }
                }
            }
        }

        [When(@"I click on ""(.*)"" in data correction field for ""(.*)"" attribute")]
        [Given(@"I click on ""(.*)"" in data correction field for ""(.*)"" attribute")]
        [Then(@"I click on ""(.*)"" in data correction field for ""(.*)"" attribute")]
        public async Task GivenIClickControlInCorrectionFieldattribute(string controlType, string questionTitle)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.ThreadSleep();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));
                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {
                        case "datepicker":
                        case "inputtextbox":
                        case "numberspinner":
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));
                            dropdown.Click();
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }
                }
            }
        }


        [Given(@"""(.*)"" is not displayed for ""(.*)"" ""(.*)""")]
        [When(@"""(.*)"" is not displayed for ""(.*)"" ""(.*)""")]
        [Then(@"""(.*)""is not displayed for ""(.*)"" ""(.*)""")]
        public void IsNotDisplayedForControlType(string value, string questionTitle, string controlType)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

              if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {                       
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));                           
                            var selectedValue = dropdown.FindElement(By.ClassName("selected")).Text;
                            Assert.AreNotEqual(selectedValue.Replace(" ", ""), value.Replace(" ", ""));                          
                            break;
                        default:
                            break;
                    }
                }
            }           
        }

        [Given(@"""(.*)"" is displayed for ""(.*)"" ""(.*)""")]
        [When(@"""(.*)"" is displayed for ""(.*)"" ""(.*)""")]
        [Then(@"""(.*)"" is displayed for ""(.*)"" ""(.*)""")]
        public void IsDisplayedForControlType(string value, string questionTitle, string controlType)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));
                            var selectedValue = dropdown.FindElement(By.ClassName("selected")).Text;
                            Assert.AreEqual(selectedValue.Replace(" ", ""), value.Replace(" ", ""));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [Given(@"X icon is displayed for DCF datepicker ""(.*)""")]
        [Then(@"X icon is displayed for DCF datepicker ""(.*)""")]
        [When(@"X icon is displayed for DCF datepicker ""(.*)""")]
        public void XIconIsDisplayedForDCFDatepicker(string questionTitle)
        {
            Assert.IsNotNull(FindXIconForDatepicker(questionTitle));
        }

        [Given(@"I click on X icon for DCF datepicker ""(.*)""")]
        [Then(@"I click on X icon for DCF datepicker ""(.*)""")]
        [When(@"I click on X icon for DCF datepicker ""(.*)""")]
        public void IClickOnXIconForDCFDatepicker(string questionTitle)
        {
            FindXIconForDatepicker(questionTitle).Click();
        }

        [Given(@"X icon is not displayed for DCF datepicker ""(.*)""")]
        [Then(@"X icon is not displayed for DCF datepicker ""(.*)""")]
        [When(@"X icon is not displayed for DCF datepicker ""(.*)""")]
        public void XIconIsNotDisplayedForDCFDatepicker(string questionTitle)
        {
            Assert.IsNull(FindXIconForDatepicker(questionTitle));
        }

        private IWebElement FindXIconForDatepicker(string questionTitle)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");
            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells.Any()
                    && gridCells[0].Text == questionTitle)
                {
                    ReadOnlyCollection<IWebElement> webElements1 = gridCells[2].FindElements(By.ClassName("btn"));
                    ReadOnlyCollection<IWebElement> webElements = gridCells[2].FindElements(By.CssSelector("-webkit-search-cancel-button"));
                    return webElements.FirstOrDefault();
                }
            }

            return null;
        }

        [Given(@"I select ""(.*)"" from existing ""(.*)"" ""(.*)""")]
        [Then(@"I select ""(.*)"" from existing ""(.*)"" ""(.*)""")]
        [When(@"I select ""(.*)"" from existing ""(.*)"" ""(.*)""")]
        public void GivenIMakeSelectionInCorrectionFieldForExisitingQuestion(string choice, string questionTitle, string controlType)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");
            var succeeded = false;

            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    controlType = controlType.ToLower();
                    switch (controlType)
                    {
                        case "datepicker":
                        case "datetimepicker":
                        case "timepicker":
                            succeeded = SelectDateTime(controlType, gridCells, choice, dateTimeCellIndex: 2);
                            break;
                        case "textbox":
                            var input = gridCells[2].FindElement(By.TagName("input"));
                            input.Click();
                            input.Clear();
                            input.SendKeys(choice);
                            succeeded = true;
                            break;
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));                   
                            var options = dropdown.FindElements(By.TagName("li"));                       
                       
                            for (int i = 0; i < options.Count; i++)
                            {
                                var optionText = options[i].Text.Trim();
                                if (optionText.Replace(" ", "") == choice.Replace(" ", ""))
                                {
                                    options[i].Click();
                                    var selectedValue = dropdown.FindElement(By.ClassName("selected")).Text;
                                    Assert.AreEqual(selectedValue.Replace(" ", ""), choice.Replace(" ", ""));
                                    succeeded = true;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!succeeded)
            {
                Assert.Fail();
            }
        }

        [Given(@"I select ""(.*)"" from ""(.*)"" ""(.*)""")]
        [Then(@"I select ""(.*)"" from ""(.*)"" ""(.*)""")]
        [When(@"I select ""(.*)"" from ""(.*)"" ""(.*)""")]
        public void GivenIMakeSelectionInCorrectionField(string choice, string questionTitle, string controlType)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");
            var succeeded = false;
            
            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                ReadOnlyCollection<IWebElement> gridCells = row.FindElements(By.TagName("td"));

                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    controlType = controlType.ToLower();
                    switch (controlType)
                    {
                        case "datepicker":
                        case "datetimepicker":
                        case "timepicker":
                            succeeded = SelectDateTime(controlType, gridCells, choice, dateTimeCellIndex: 1);
                            break;
                        case "inputtextbox":
                            break;
                        case "dropdown":
                            var dropdown = gridCells[1].FindElement(By.ClassName("metro"));
                            var options = dropdown.FindElements(By.TagName("li"));

                            for (int i = 0; i < options.Count; i++)
                            {
                                var optionText = options[i].Text.Trim();
                                if (optionText.Replace(" ", "") == choice.Replace(" ", ""))
                                {
                                    options[i].Click();
                                    var selectedValue = dropdown.FindElement(By.ClassName("selected")).Text;
                                    Assert.AreEqual(selectedValue.Replace(" ", ""), choice.Replace(" ", ""));
                                    succeeded = true;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!succeeded)
            {
                Assert.Fail();
            }
        }

        [Given(@"the following choices are displayed for ""(.*)"" question")]
        [Then(@"the following choices are displayed for ""(.*)"" question")]
        public void ThenChoicesAreDisplayedForQuestion(string questionTitle, Table table)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr.unchanged");
            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells[0].Text.Contains(questionTitle))
                {
                    var actualChoices = gridCells[1].Text.Split(Environment.NewLine).ToList();
                    var choices = table.Rows.Select(r => r[0]).ToList();

                    for (int i = 0; i < actualChoices.Count; i++)
                    {
                        var optionText = actualChoices[i].Trim();
                        var isPresent = choices.Contains(optionText);
                        Assert.IsTrue(isPresent);
                    }
                }
            }
        }

        [Given(@"the following choices are displayed in dropdown for ""(.*)"" question")]
        [Then(@"the following choices are displayed in dropdown for ""(.*)"" question")]
        public async Task ThenChoicesAreDisplayedInDropdown(string questionTitle, Table table)
        {
        
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr.unchanged");
            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells[0].Text == questionTitle)
                {
                    var dropdownElem = gridCells[1].FindElement(By.TagName("ul"));
                    var options = dropdownElem.FindElements(By.TagName("li")).ToList();
                    var choices = table.Rows.Select(r => r[0]).ToList();
                    var firstOption = options[0].Text.Replace(" ", string.Empty).ToLower();
                    if (placeholderTextOptions.Contains(firstOption))
                        options.RemoveAt(0);
                    Assert.AreEqual(choices.Count, options.Count);
                    for (int i = 0; i < options.Count; i++)
                    {
                        var optionText = options[i].Text.Trim();
                        var isPresent = choices.Contains(optionText);
                        Assert.IsTrue(isPresent);
                    }
                }
            }
        }

        [Given(@"the following data is displayed in the data correction field for ""(.*)"" type of correction")]
        public void GivenTheFollowingDataIsDisplayedInTheDataCorrectionFieldFor(string typeOfCorrection, Table table)
        {
            var tblData = table.CreateSet<ControlValueMap>().ToList();

            switch (typeOfCorrection)
            {
                case "Merge subjects":
                    {
                        dataCorrectionPage.WaitForAjax();

                        var element = dataCorrectionPage.GetWebElementById("MergePatientDiv");

                        if (element != null)
                        {
                            var gridCols = element.FindElements(By.CssSelector("dt"));
                            var gridCells = element.FindElements(By.CssSelector("dd"));

                            dataCorrectionPage.WaitForSpinner();

                            for (int idx = 0; idx < gridCols.Count; idx++)
                            {
                                if (idx < tblData.Count)
                                {
                                    var dataRow = tblData[idx];

                                    Assert.AreEqual(dataRow.Label, gridCols[idx].Text);

                                    var value = dataRow.Value;

                                    switch (dataRow.Fieldtype.ToLower())
                                    {
                                        case "text":
                                            Assert.AreEqual(value, gridCells[idx].Text);
                                            break;

                                        case "date":
                                            value = TransformValue(value);
                                            Assert.AreEqual(value, gridCells[idx].Text);
                                            break;

                                        default:
                                            Assert.Fail();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "Change subject Information":
                    {
                        var element = dataCorrectionPage.GetWebElementById("subjectInfoFields");
                        var gridRows = element.FindElements(By.CssSelector("tbody tr"));
                        dataCorrectionPage.WaitForAjax();

                        for (var i = 1; i < gridRows.Count; i++)
                        {
                            var row = gridRows[i];

                            var gridCells = row.FindElements(By.TagName("td"));
                            var index = gridRows.IndexOf(row);

                            if (index - 1 >= tblData.Count) continue;

                            var dataRow = tblData[index - 1];

                            Assert.AreEqual(dataRow.Label, gridCells[0].Text);

                            var value = TransformTableValue(dataRow.Value);

                            switch (dataRow.Fieldtype.ToLower())
                            {
                                case "text":
                                    Assert.AreEqual(value, gridCells[1].Text);
                                    break;
                                case "radiobuttons1":
                                    var container = dataCorrectionPage.GetWebElementById("Question_Container_1");
                                    var selected = container.FindElement(By.ClassName("active"));
                                    var selectedValue = selected.Text;
                                    Assert.AreEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                                    break;

                                default:
                                    Assert.Fail();
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    Assert.Fail();
                    break;
            }
        }

        [Given(@"the following data is displayed in the attribute data visit field")]
        [Then(@"the following data is displayed in the attribute data visit field")]
        [Given(@"the following data is displayed in the existing question visit fields")]
        [Then(@"the following data is displayed in the existing question visit fields")]
        public async Task ThenTheFollowingDataIsDisplayedInTheExistingVisitFields(Table table)
        {
            var tblData = table.CreateSet<ControlValueMap>().ToList();
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#ChangePatientVisitFieldset table tbody tr");

            dataCorrectionPage.WaitForAjax();

            for (var i = 1; i < gridRows.Count; i++)
            {
                var row = gridRows[i];

                var gridCells = row.FindElements(By.TagName("td"));
                var index = gridRows.IndexOf(row);

                if (index - 1 >= tblData.Count) continue;

                var dataRow = tblData[index - 1];
                Assert.AreEqual(dataRow.Label, gridCells[0].Text);
                string value = TransformTableValue(dataRow.Value);
                var fieldType = dataRow.Fieldtype.ToLower();
                switch (fieldType)
                {
                    case "datepicker":
                        var datetimePicker = gridCells[2].FindElement(By.ClassName("datepicker"));
                        if (value == null)
                        {
                            Assert.AreEqual(DCFConstants.PlaceholderText, datetimePicker.GetAttribute("placeholder"));
                        }
                        else
                        {
                            var expectedDatetime = !string.IsNullOrWhiteSpace(value) ? GetExpectedDateValue(value, dataRow.DateTimeFormat) : value;
                            Assert.AreEqual(expectedDatetime, datetimePicker.GetAttribute("value"));
                        }
                        break;
                    case "dropdown":
                        var selected = gridCells[2].FindElement(By.ClassName("selected"));
                        var selectedValue = selected.Text;

                        if (value == null)
                        {
                            Assert.AreEqual(DCFConstants.PlaceholderText, selectedValue ?? string.Empty);
                        }
                        else
                        {
                            Assert.AreEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                        }

                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Given(@"the following data is displayed in the data correction field")]
        [Then(@"the following data is displayed in the data correction field")]
        public async Task ThenDataIsDisplayedInCorrectionField(Table table)
        {
            var tblData = table.CreateSet<ControlValueMap>().ToList();
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.WaitForAjax();

            for (var i = 1; i < gridRows.Count; i++)
            {
                var row = gridRows[i];

                var gridCells = row.FindElements(By.TagName("td"));
                var index = gridRows.IndexOf(row);

                if (index - 1 >= tblData.Count) continue;

                var dataRow = tblData[index - 1];

                Assert.AreEqual(dataRow.Label, gridCells[0].Text);

                var value = TransformTableValue(dataRow.Value);
                var fieldType = dataRow.Fieldtype.ToLower();
                switch (fieldType)
                {
                    case "text":
                        Assert.AreEqual(value, gridCells[1].Text);
                        break;
                    case "datepicker":
                    case "datetimepicker":
                    case "timepicker":
                        var datetimePicker = gridCells[1].FindElement(By.ClassName("datepicker"));
                        if(value == DCFConstants.PlaceholderText)
                        {
                            Assert.AreEqual(value, datetimePicker.GetAttribute("placeholder"));
                        }
                        else
                        {
                            var expectedDatetime = !string.IsNullOrWhiteSpace(value) ? GetExpectedDateValue(value, dataRow.DateTimeFormat) : value;
                            Assert.AreEqual(expectedDatetime, datetimePicker.GetAttribute("value"));
                        }
                        break;
                    case "inputtextbox":
                    case "numberspinner":
                        var control = gridCells[1].FindElement(By.TagName("input"));
                        if (value == DCFConstants.PlaceholderText)
                        {
                            Assert.AreEqual(value, control.GetAttribute("placeholder"));
                        }
                        else
                        {
                            Assert.AreEqual(value, control.GetAttribute("value"));
                        }
                        break;
                    case "dropdown":
                        var selected = gridCells[1].FindElement(By.ClassName("selected"));
                        var selectedValue = selected.Text;
                        Assert.AreEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                        break;
                    case "multiselect":
                        var selectedCheckBoxes = gridCells[1]
                            .FindElements(By.TagName("input"))
                            .Where(e => e.GetAttribute("type") == "checkbox" && e.Selected)
                            .Select(e => e.GetAttribute("name")).ToList();

                        var multiSelectValues = gridCells[1].FindElements(By.TagName("label"))
                                        .Where(e => selectedCheckBoxes.Contains(e.GetAttribute("for")))
                                        .Select(e => e.Text).ToList();
                        Assert.AreEqual(value ?? string.Empty, string.Join(',', multiSelectValues) ?? string.Empty);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        private string GetExpectedDateValue(string value, string dateTimeFormat)
        {
            int.TryParse(value.ToLower().Replace(CurrentDateChoice, ""), out var dayOffset);
            return CurrentDateTime.AddDays(dayOffset).ToString(string.IsNullOrWhiteSpace(dateTimeFormat) ? "dd/MMMM/yyyy" : dateTimeFormat, CultureInfo.InvariantCulture);
        }

        [Given(@"the following data is displayed in the attribute data correction field")]
        [Then(@"the following data is displayed in the attribute data correction field")]
        [Given(@"the following data is displayed in the existing question correction fields")]
        [Then(@"the following data is displayed in the existing question correction fields")]
        public async Task ThenTheFollowingDataIsDisplayedInTheExistingQuestionCorrectionFields(Table table)
        {
            var tblData = table.CreateSet<ControlExistingValueMap>().ToList();
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.WaitForAjax();

            for (var i = 1; i < gridRows.Count; i++)
            {
                if (i <= tblData.Count)
                {
                    var row = gridRows[i];

                    var gridCells = row.FindElements(By.TagName("td"));
                    var index = gridRows.IndexOf(row);
                    var dataRow = tblData[index - 1];

                    var label = gridCells[0].Text;
                    Assert.AreEqual(dataRow.Label, label);

                var text = gridCells[1].Text;
                var transval = TransformTableValue(dataRow.CurrentValue);
                Assert.AreEqual(transval, text);

                var value = TransformTableValue(dataRow.RequestedValue);
                var fieldType = dataRow.Fieldtype.ToLower();

                if (dataRow.IsRemoveEnabled.HasValue)
                {
                    var correctionRemove = gridCells[3].FindElement(By.ClassName("correction-remove"));
                    Assert.AreEqual(dataRow.IsRemoveEnabled, correctionRemove.Enabled);
                }

                switch (dataRow.Fieldtype.ToLower())
                {
                    case "text":
                        Assert.AreEqual(value, gridCells[2].Text);
                        break;
                    case "datepicker":
                    case "datetimepicker":
                    case "timepicker":
                        var datetimePicker = gridCells[2].FindElement(By.ClassName("datepicker"));

                        if (dataRow.IsEnabled.HasValue)
                        {
                            Assert.AreEqual(dataRow.IsEnabled, !datetimePicker.GetAttribute("class").Contains("disabled"));
                        }

                        if (value == DCFConstants.PlaceholderText)
                        {
                            Assert.AreEqual(value, datetimePicker.GetAttribute("placeholder"));
                        }
                        else
                        {
                            var expectedDatetime = !string.IsNullOrWhiteSpace(value) ? GetExpectedDateValue(value, dataRow.DateTimeFormat) : value;
                            Assert.AreEqual(expectedDatetime, datetimePicker.GetAttribute("value"));
                        }

                        break;
                    case "inputtextbox":
                    case "numberspinner":
                        var control = gridCells[2].FindElement(By.TagName("input"));
                        if (value == DCFConstants.PlaceholderText)
                        {
                            Assert.AreEqual(value, control.GetAttribute("placeholder"));
                        }
                        else
                        {
                            Assert.AreEqual(value, control.GetAttribute("value"));
                        }
                        break;
                    case "dropdown":
                        var selected = gridCells[2].FindElement(By.ClassName("selected"));
                        var selectedValue = selected.Text;
                        Assert.AreEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                        break;
                    case "multiselect":
                        // use this one for change questionnaire responses as our index is shifted over one
                        var selectedCheckBoxes = gridCells[2]
                            .FindElements(By.TagName("input"))
                            .Where(e => e.GetAttribute("type") == "checkbox" && e.Selected)
                            .Select(e => e.GetAttribute("name")).ToList();

                        var multiSelectValues = gridCells[2].FindElements(By.TagName("label"))
                                        .Where(e => selectedCheckBoxes.Contains(e.GetAttribute("for")))
                                        .Select(e => e.Text).ToList();
                        Assert.AreEqual(value ?? string.Empty, string.Join(',', multiSelectValues) ?? string.Empty);
                        break;
                    case "radiobutton":
                        var activeLabel = gridCells[2].FindElements(By.TagName("label"))
                            .FirstOrDefault(q => q.GetAttribute("class").Contains("active"))?
                            .Text;

                        Assert.AreEqual(value ?? String.Empty, activeLabel ?? String.Empty);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
                }
            }
        }

        [When(@"I click on ""(.*)"" in data correction field for ""(.*)"" existing question")]
        [Given(@"I click on ""(.*)"" in data correction field for ""(.*)"" existing question")]
        [Then(@"I click on ""(.*)"" in data correction field for ""(.*)"" existing question")]
        public async Task GivenIClickOnControlInDataCorrectionFieldForExistingQuesiton(string controlType, string questionTitle)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            dataCorrectionPage.ThreadSleep();

            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells.Any() && gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {
                        case "datepicker":
                        case "inputtextbox":
                            var input = gridCells[2].FindElement(By.TagName("input"));
                            input.Click();
                            break;
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));
                            dropdown.Click();
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }
                }
            }
        }


        [Given(@"the following choices are displayed in dropdown for ""(.*)"" existing question")]
        [Then(@"the following choices are displayed in dropdown for ""(.*)"" existing question")]
        public async Task ThenTheFollowingChoicesAreDisplayedInDropdownForExistingQuestion(string questionTitle, Table table)
        {           
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr.unchanged");
            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells[0].Text == questionTitle)
                {
                    var dropdownElem = gridCells[2].FindElement(By.TagName("ul"));
                    var options = dropdownElem.FindElements(By.TagName("li")).ToList();
                    var choices = table.Rows.Select(r => r[0]).ToList();
                    var firstOption = options[0].Text.Replace(" ", string.Empty).ToLower();
                    if (placeholderTextOptions.Contains(firstOption))
                        options.RemoveAt(0);
                    Assert.AreEqual(choices.Count, options.Count);
                    for (int i = 0; i < options.Count; i++)
                    {
                        var optionText = options[i].Text.Trim();
                        var isPresent = choices.Contains(optionText);
                        Assert.IsTrue(isPresent);
                    }
                }
            }
        }

        [Given(@"I select ""(.*)"" as new value from ""(.*)"" ""(.*)""")]
        public async Task GivenISelectTheNewChoiceFromQuestionControl(string choice, string questionTitle, string controlType)
        {
            var gridRows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr.unchanged");

            var succeeded = false;
            foreach (var row in gridRows)
            {
                var gridCells = row.FindElements(By.TagName("td"));

                if (gridCells[0].Text == questionTitle)
                {
                    switch (controlType.ToLower())
                    {
                        case "datepicker":
                        case "datetimepicker":
                        case "timepicker":
                            succeeded = SelectDateTime(controlType, gridCells, choice, dateTimeCellIndex: 2);
                            break;
                        case "textbox":
                        case "dropdown":
                            var dropdown = gridCells[2].FindElement(By.ClassName("metro"));
                            var options = dropdown.FindElements(By.TagName("li"));

                            for (int i = 0; i < options.Count; i++)
                            {
                                var optionText = options[i].Text.Trim();
                                if (optionText.Replace(" ", "") == choice.Replace(" ", ""))
                                {
                                    options[i].Click();
                                    var selectedValue = dropdown.FindElement(By.ClassName("selected")).Text;
                                    Assert.AreEqual(selectedValue.Replace(" ", ""), choice.Replace(" ", ""));
                                    succeeded = true;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!succeeded)
            {
                Assert.Fail();
            }
        }

        private bool SelectDateTime(
            string controlType,
            ReadOnlyCollection<IWebElement> gridCells,
            string choice,
            int dateTimeCellIndex)
        {
            var succeeded = false;
            string datetimeFormat = null;
            
            var inputElement = gridCells[dateTimeCellIndex].FindElement(By.ClassName("datepicker"));
            if (inputElement.GetParent()?.GetAttribute("class")?.Contains("dcf-nonconfigurable-date") ?? false)
            {
                datetimeFormat = "dd-MMM-yyyy";
            }
            else
            {
                ReadOnlyCollection<IWebElement> datetimeFormatField = gridCells[dateTimeCellIndex].FindElements(By.ClassName("date-format"));
                if (datetimeFormatField.Any())
                {
                    datetimeFormat = datetimeFormatField.First().GetAttribute("value")?.Replace("D", "d").Replace("Y", "y").Replace("A", "tt");
                }
            }

            inputElement.Click();

            var datetimePicker = gridCells[dateTimeCellIndex].FindElement(By.ClassName("bootstrap-datetimepicker-widget"));

            //Set time
            if (controlType != "datepicker")
            {
                var buttons = datetimePicker.FindElements(By.ClassName("btn"));
                var decrementHoursButton = buttons.First(b => b.GetAttribute("data-action") == "decrementHours");
                var decrementMinutesButton = buttons.First(b => b.GetAttribute("data-action") == "decrementMinutes");
                var togglePeriodButton = buttons.FirstOrDefault(b => b.GetAttribute("data-action") == "togglePeriod");

                var hourSpan = datetimePicker.FindElement(By.ClassName("timepicker-hour"));
                var minuteSpan = datetimePicker.FindElement(By.ClassName("timepicker-minute"));

                var targetHour = CurrentDateTime.Hour;
                var targetMinute = CurrentDateTime.Minute;
                string targetPeriod = null;

                if (togglePeriodButton != null)
                {
                    (targetHour, targetPeriod) = targetHour > 12 ? (targetHour - 12, "PM") : (targetHour, "AM");
                }

                var clickCount = 0;
                var maxClicks = 60; //60 minutes

                while (int.Parse(minuteSpan.Text) != targetMinute && clickCount++ <= maxClicks)
                {
                    decrementMinutesButton.Click();
                }

                clickCount = 0;
                maxClicks = 24; //24 hours

                do
                {
                    decrementHoursButton.Click();
                } while ((int.Parse(hourSpan.Text) != targetHour 
                        || (targetPeriod != null && togglePeriodButton.Text != targetPeriod))
                        && ++clickCount <= maxClicks);

                Assert.AreEqual(targetHour, int.Parse(hourSpan.Text));
                Assert.AreEqual(targetMinute,int.Parse(minuteSpan.Text));

                if (togglePeriodButton != null)
                {
                    Assert.AreEqual(togglePeriodButton.Text, targetPeriod);
                }
                
                succeeded = controlType == "timepicker";
            }

            // Set date
            if (!succeeded)
            {
                var cells = datetimePicker.FindElements(By.TagName("td"));
                int.TryParse(choice.ToLower().Replace(CurrentDateChoice, ""), out var dayOffset);
                var targetDate = CurrentDateTime.AddDays(dayOffset);
                var targetDay = targetDate.Day.ToString();
                var dayOne = 1;
                var currentMonth = false;

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i].Text == dayOne.ToString())
                    {
                        currentMonth = true;
                    }

                    if (cells[i].Text == targetDay && currentMonth)
                    {
                        var isEnabled = !cells[i].GetAttribute("class").Contains("disabled");
                        cells[i].Click();

                        if (isEnabled)
                        {
                            var currentDate = targetDate.ToString(string.IsNullOrEmpty(datetimeFormat) ? "dd/MMMM/yyyy" : datetimeFormat, CultureInfo.InvariantCulture);
                            Assert.AreEqual(currentDate, inputElement.GetAttribute("value"));
                        }

                        succeeded = true;
                        break;
                    }
                }
            }

            //Click on the label to dismiss datetime picker
            gridCells[0].Click();

            return succeeded;
        }

        [Given(@"the following data is displayed in the correction data field table")]
        [Then(@"the following data is displayed in the correction data field table")]
        public async Task ThenTheFollowingDataIsDisplayedInTheCorrectionDataFieldTable(Table table)
        {
            var tblData = table.CreateSet<ControlExistingValueMap>().ToList();
            var gridRows = dataCorrectionPage.GetWebElementsByCss("table tbody tr");

            dataCorrectionPage.WaitForAjax();

            for (var i = 1; i < gridRows.Count; i++)
            {
                if (i <= tblData.Count)
                {
                    var row = gridRows[i];
                    var gridCells = row.FindElements(By.TagName("td"));
                    var dataRow = tblData[i - 1];
                    var requestedValue = !string.IsNullOrWhiteSpace(dataRow.RequestedValue) &&
                                (!string.IsNullOrWhiteSpace(dataRow.DateTimeFormat) || dataRow.RequestedValue.ToLower() == CurrentDateChoice)
                                ?
                                GetExpectedDateValue(dataRow.RequestedValue, dataRow.DateTimeFormat)
                                :
                                dataRow.RequestedValue;
                    var cellIndex = gridCells.Count - 1;
                    Assert.AreEqual(dataRow.Label, gridCells[0].Text);
                    var transval = TransformTableValue(dataRow.CurrentValue);
                    Assert.AreEqual(transval, gridCells[cellIndex - 1].Text);
                    Assert.AreEqual(requestedValue, gridCells[cellIndex].Text);
                }
            }
        }

        [Given(@"the following data is displayed in the data field table")]
        [Then(@"the following data is displayed in the data field table")]
        public async Task ThenDataIsDisplayedInDataField(Table table)
        {
            var tblData = table.CreateSet<ControlValueMap>().ToList();
            var gridRows = dataCorrectionPage.GetWebElementsByCss("table tbody tr");

            dataCorrectionPage.WaitForAjax();

            foreach (var row in gridRows)
            {
                var index = gridRows.IndexOf(row);

                if (index != 0)
                {
                    var gridCells = row.FindElements(By.TagName("td"));

                    if (index - 1 < tblData.Count)
                    {
                        var dataRow = tblData[index - 1];
                        var value = !string.IsNullOrWhiteSpace(dataRow.Value) && 
                            (!string.IsNullOrWhiteSpace(dataRow.DateTimeFormat) || dataRow.Value.ToLower() == CurrentDateChoice) 
                            ? 
                            GetExpectedDateValue(dataRow.Value, dataRow.DateTimeFormat) 
                            : 
                            dataRow.Value;
                        var cellIndex = gridCells.Count - 1;
                        Assert.AreEqual(dataRow.Label, gridCells[0].Text);
                        Assert.AreEqual(value, gridCells[cellIndex].Text);
                    }
                }
            }
        }

        [Given(@"dropdown for ""(.*)"" question has placeholder ""(.*)""")]
        [Then(@"dropdown for ""(.*)"" question has placeholder ""(.*)""")]
        [Given(@"dropdown for ""(.*)"" question has selected value ""(.*)""")]
        [Then(@"dropdown for ""(.*)"" question has selected value ""(.*)""")]
        public void ThenDropdownForQuestionHasSelectedValue(string questionTitle, string value)
        {
            var questionnaireResponseDiv = dataCorrectionPage.GetWebElementById("QuestionnaireResponseDiv");
            var rows = questionnaireResponseDiv.FindElements(By.TagName("tr"));

            dataCorrectionPage.WaitForAjax();

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                    var dropdown = rowCells[1].FindElements(By.ClassName("dcf-multiple-choice-response"))?.FirstOrDefault()
                        ?? rowCells[1].FindElement(By.ClassName("dcf-single-select-response"));

                    var selected = dropdown.FindElement(By.ClassName("selected"));
                    var selectedValue = selected.Text;
                    Assert.AreEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                }
            }
        }

        [Given(@"dropdown for ""(.*)"" question does not have selected value ""(.*)""")]
        [Then(@"dropdown for ""(.*)"" question does not have selected value ""(.*)""")]
        public void ThenDropdownForQuestionDoesNotHaveSelectedValue(string questionTitle, string value)
        {
            var questionnaireResponseDiv = dataCorrectionPage.GetWebElementById("QuestionnaireResponseDiv");
            var rows = questionnaireResponseDiv.FindElements(By.TagName("tr"));
            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                    var dropdown = rowCells[1].FindElements(By.ClassName("dcf-multiple-choice-response"))?.FirstOrDefault()
                        ?? rowCells[1].FindElement(By.ClassName("dcf-single-select-response"));

                    var selected = dropdown.FindElement(By.ClassName("selected"));
                    var selectedValue = selected.Text;
                    Assert.AreNotEqual(value ?? string.Empty, selectedValue ?? string.Empty);
                }
            }
        }

        [Given(@"dropdown for ""(.*)"" existing question has placeholder ""(.*)""")]
        [Then(@"dropdown for ""(.*)"" existing question has placeholder ""(.*)""")]
        [Then(@"dropdown for ""(.*)"" existing attribute has placeholder ""(.*)""")]
        public async Task ThenDropdownForExistingQuestionHasPlaceholder(string questionTitle, string placeholder)
        {
            var questionnaireResponseDiv = dataCorrectionPage.GetWebElementById("QuestionnaireResponseDiv");
            var rows = questionnaireResponseDiv.FindElements(By.TagName("tr"));
            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                   
                    var dropdown = rowCells[2].FindElements(By.ClassName("dcf-multiple-choice-response"))?.FirstOrDefault()
                            ?? rowCells[2].FindElement(By.ClassName("dcf-single-select-response"));
                    var selected = dropdown.FindElement(By.ClassName("selected"));
                    var selectedValue = selected.Text;
                    Assert.AreEqual(placeholder ?? string.Empty, selectedValue ?? string.Empty);
                }
            }
        }

        private string TransformTableValue(string tableValue)
        {
            string result;

            switch (tableValue)
            {
                case "<SubjectID>":
                    result = testData.NextSubjectNumber;
                    break;
                case "(Current Date)":
                    result = DateTime.Now.ToString("dd/MMMM/yyyy", CultureInfo.InvariantCulture);
                    break;
                case "(Current Date score)":
                    result = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    break;
                case "(Current Date score sumarized)":
                    result = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    result = tableValue;
                    break;
            }

            return result;
        }

        [Then(@"the validation message is displayed for ""(.*)"" question")]
        public void ThenTheValidationMessageIsDisplayedForQuestion(string questionTitle)
        {
            Assert.IsTrue(GetValidationMessages().Any(m => m.Contains(questionTitle)));
        }

        [Then(@"the validation message is not displayed")]
        public void ThenTheValidationMessageIsNotDisplayed()
        {
            var validationDiv = dataCorrectionPage.GetWebElementsByClass("validation-summary-errors")?.FirstOrDefault();
            Assert.IsNull(validationDiv);
        }

        [Given(@"the validation message ""(.*)"" is displayed for question ""(.*)""")]
        [Then(@"the validation message ""(.*)"" is displayed for question ""(.*)""")]
        public void TheValidationMessageIsDisplayedForQuestion(string validationMessage, string questionText)
        {
            Assert.IsTrue(GetValidationMessages().Any(m => m.Contains(questionText) && m.Contains(validationMessage)));
        }

        [Given(@"the validation message ""(.*)"" is displayed")]
        [Then(@"the validation message ""(.*)"" is displayed")]
        public void ThenTheValidationMessageIsDisplayed(string validationMessage)
        {
            Assert.IsTrue(GetValidationMessages().Any(m => m == validationMessage));
        }

        [Then(@"the DCF Visit Status message ""(.*)"" is displayed")]
        public void TheDCFVisitStatusMessageIsDisplayed(string validationMessage)
        {
            var messageLabels = dataCorrectionPage.GetWebElementsByCss("#CorrectionDiv label");
            Assert.IsTrue(messageLabels.Any(ml => ml.Displayed && ml.Text == validationMessage));
        }

        private IEnumerable<string> GetValidationMessages()
        {
            var validationDiv = dataCorrectionPage.GetWebElementsByClass("validation-summary-errors")?.FirstOrDefault();
            Assert.IsNotNull(validationDiv);
            var validationDivTable = validationDiv.FindElement(By.TagName("ul"));
            Assert.IsNotNull(validationDivTable);
            var validationDivTableElements = validationDivTable.FindElements(By.TagName("li"));
            Assert.IsNotNull(validationDivTableElements);

            return validationDivTableElements.Select(e => e.Text);
        }

        [Given(@"I click ""(.*)"" on the dialog")]
        [When(@"I click ""(.*)"" on the dialog")]
        [Then(@"I click ""(.*)"" on the dialog")]
        public async Task GivenIClickOnTheDialog(string dialogButton)
        {
            var dialogButtonsDiv = dataCorrectionPage.GetWebElementsByClass("bootstrap-dialog-footer-buttons")?.FirstOrDefault();
            Assert.IsNotNull(dialogButtonsDiv);
            var validationDivButtons = dialogButtonsDiv.FindElements(By.TagName("button"));
            Assert.IsNotNull(validationDivButtons);
            var okButton = validationDivButtons.FirstOrDefault(b => b.Text == dialogButton);
            Assert.IsNotNull(okButton);
            okButton.Click();
            dataCorrectionPage.WaitForAjax();
        }

        [Given(@"DCF workflow approval ""(.*)"" is ""(.*)"" for DCF type ""(.*)"" for ""(.*)"" Configuration Version")]
        public async Task DcfWorkflowApprovalStatus(string workflowValueName, string status, string dcfType, string configurationVersion)
        {
            var workflow = await e2eRepository.GetCorrectionWorkflowForVersion(configurationVersion, dcfType);

            Assert.IsNotNull(workflow);

            var expectedToBeEnabled = false;

            switch (status.ToLower())
            {
                case "enabled":
                    expectedToBeEnabled = true;
                    break;
            }

            switch (workflowValueName.ToLower())
            {
                case "no approval needed":
                    Assert.AreEqual(workflow.NoApprovalNeeded, expectedToBeEnabled);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [Given(@"""(.*)"" status is selected for data correction field")]
        public void StatusIsSelectedForDataCorrection(string status)
        {
            var statusButton = GetDcfActiveStatusRadioButton();
            var selectedStatus = statusButton?.Text ?? string.Empty;

            Assert.AreEqual(status.Trim().ToLower(), selectedStatus.Trim().ToLower());
        }

        [Given(@"I select ""(.*)"" for Current Status")]
        public void SelectValueForCurrentStatus(string status)
        {
            var statusButtons = GetDcfStatusRadioButtons();

            foreach (var statusButton in statusButtons)
            {
                if (statusButton.Text.Trim().ToLower() == status.Trim().ToLower())
                {
                    statusButton.Click();
                    break;
                }
            }
        }

        [Given(@"Patient ""(.*)"" has the following subject attributes:")]
        public async Task SubjectHasTheFollowingAttributes(string subjectMapping, Table table)
        {
            var patient = apiTestData.PatientMappings
                .First(p => p.MappingName.Equals(subjectMapping, StringComparison.InvariantCultureIgnoreCase));

            var attributes = table.CreateSet<SubjectAttributeMap>().ToList();

            foreach (var attribute in attributes)
            {
                attribute.Value = TransformValue(attribute.Value);

                await e2eRepository.AddPatientAttribute(
                    patient.Patient.Id,
                    attribute.Label,
                    attribute.Value);
            }
        }

        [Given(@"I select ""(.*)"" in data correction field")]
        public void GivenISelectInDataCorrectionField(string value)
        {
            var targetValue = string.Empty;

            if (value == "Subject 1")
            {
                targetValue = "0";
            }

            var element = dataCorrectionPage.GetWebElementById("MergePatientDiv");

            if (element != null)
            {
                var elements = element.FindElements(By.TagName("div"));

                foreach (var e in elements)
                {
                    var approvalPosition = e.GetAttribute("approvalposition");
                    if (approvalPosition == targetValue)
                    {
                        e.Click();
                        break;
                    }
                }
            }
        }

        [Then("Correction table has new record for following data")]
        public void CorrectionTableHasNewRecordForFollowingData(Table table)
        {
            const string defaultUser = "System";
            const string ypStudyUserId = "YP-Study-User-ID";
            const string currentDate = "Current date";
            const string lastModifiedByDatabaseUserNull = "Null";
            const string lastModifiedByDatabaseUserNotNull = "Not null";

            var auditModel = table.CreateSet<AuditModelMap>().FirstOrDefault();

            List<string> possibleAuditUserIds;
            if (auditModel.AuditUserID == ypStudyUserId)
            {
                possibleAuditUserIds = e2eRepository.GetMatchingSessionStudyUserIds();
                if (!possibleAuditUserIds.Any())
                    possibleAuditUserIds.Add(defaultUser);
            }
            else
            {
                possibleAuditUserIds = new List<string> { auditModel.AuditUserID };
            }

            DateTime lastModified;
            if (auditModel.LastModified == currentDate)
            {
                lastModified = DateTime.UtcNow;
            }
            else
            {
                lastModified = DateTime.Parse(auditModel.LastModified);
            }

            var lastModifiedByDatabaseUser = auditModel.LastModifiedByDatabaseUser?.ToLower();

            var correctionWorkflow = e2eRepository.GetLatestCorrectionWorkflow();

            Assert.IsNotNull(correctionWorkflow);
            // We need to check with multiple possible IDs since there is a chance that more than one user has the same email.
            Assert.IsTrue(possibleAuditUserIds.Any(id => id == correctionWorkflow.AuditUserID));
            Assert.AreEqual(correctionWorkflow.LastModified.Date, lastModified.Date);
            if (lastModifiedByDatabaseUser == lastModifiedByDatabaseUserNull.ToLower())
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(correctionWorkflow.LastModifiedByDatabaseUser));
            }
            else if (lastModifiedByDatabaseUser == lastModifiedByDatabaseUserNotNull.ToLower())
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(correctionWorkflow.LastModifiedByDatabaseUser));
            }
            else
            {
                Assert.Fail("LastModifiedByDatabaseUser parameter unknown value.");
            }
        }

        private string TransformValue(string currentValue)
        {
            const string dateFormat = "dd-MMM-y";
            const string currentDateIndicator = "(Current Date)";

            var result = currentValue;

            if (currentValue.Trim().Equals(currentDateIndicator, StringComparison.InvariantCultureIgnoreCase))
            {
                result = DateTime.UtcNow.ToString(dateFormat);
            }

            return result;
        }

        private List<IWebElement> GetDcfStatusRadioButtons()
        {
            var statusContainer = dataCorrectionPage.GetWebElementsByClass("SubjectStatusUpdate")?.FirstOrDefault();
            var inputs = statusContainer?.FindElements(By.TagName("label"));
            return inputs?.ToList() ?? new List<IWebElement>();
        }

        private IWebElement GetDcfActiveStatusRadioButton()
        {
            var statusContainer = dataCorrectionPage.GetWebElementsByClass("SubjectStatusUpdate")?.FirstOrDefault();
            var activeButton = statusContainer?.FindElement(By.ClassName("active"));
            return activeButton;
        }

        [Given(@"I create a Data Correction for ""(.*)"" associated with Patient ""(.*)""")]
        [When(@"I create a Data Correction for ""(.*)"" associated with Patient ""(.*)""")]
        [Then(@"I create a Data Correction for ""(.*)"" associated with Patient ""(.*)""")]
        public void GivenICreateADataCorrectionForAssociatedWithPatient(string parameter, string patientNumber)
        {
            e2eRepository.AddDataCorrectionForPatient(parameter, patientNumber);
        }

       
        [Given(@"I create a Data Correction with details as follows")]
        public void GivenICreateADataCorrectionWithDetailsAsFollows(Table table)
        { 
            e2eRepository.AddMultipleDataCorrectionsForPatient(table);
        }

        [Given(@"""(.*)"" suffix is displayed for question ""(.*)"" in PaperDCF")]
        [When(@"""(.*)"" suffix is displayed for question ""(.*)"" in PaperDCF")]
        [Then(@"""(.*)"" suffix is displayed for question ""(.*)"" in PaperDCF")]
        public void SuffixIsDisplayedForQuestionPaperDCF(string suffix, string questionTitle)
        {
            ValidateSuffix(suffix, questionTitle, CorrectionType.PaperDiaryEntry);
        }

        [Given(@"""(.*)"" suffix is displayed for question ""(.*)""")]
        [When(@"""(.*)"" suffix is displayed for question ""(.*)""")]
        [Then(@"""(.*)"" suffix is displayed for question ""(.*)""")]
        public void SuffixIsDisplayedForQuestion(string suffix, string questionTitle)
        {
            ValidateSuffix(suffix, questionTitle, CorrectionType.ChangeQuestionnaireResponses);
        }

        [Given(@"""(.*)"" suffix is displayed for attribute ""(.*)""")]
        [When(@"""(.*)"" suffix is displayed for attribute ""(.*)""")]
        [Then(@"""(.*)"" suffix is displayed for attribute ""(.*)""")]
        public void SuffixIsDisplayedForAttribute(string suffix, string attributeName)
        {
            ValidateSuffix(suffix, attributeName, CorrectionType.ChangeSubjectInfo);
        }

        private void ValidateSuffix(string suffix, string itemName, CorrectionType correctionType)
        {

            var dataCorrectionTable = dataCorrectionPage.GetWebElementById("DataCorrectionTable");
            var rows = dataCorrectionTable.FindElements(By.TagName("tr"));

            var suffixDisplayed = false;

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(itemName))
                {                    
                    IWebElement answerSuffix = GetSuffixForType(rowCells, correctionType);
                    
                    Assert.AreEqual(
                        answerSuffix.Text.Trim(),
                        suffix.Trim(),
                        $"Suffix does not match {suffix}");

                    suffixDisplayed = true;
                }
            }

            Assert.IsTrue(suffixDisplayed, $"Suffix not found for {itemName}");
        }

        private static IWebElement GetSuffixForType(ReadOnlyCollection<IWebElement> rowCells, CorrectionType correctionType)
        {
            var cellIndex = correctionType == CorrectionType.PaperDiaryEntry
                ? 1
                : 2;

            return rowCells[cellIndex].FindElement(By.ClassName("answer-suffix"));
        }

        [Given(@"I enter ""(.*)"" for ""(.*)"" question")]
        public void IEnterValueForQuestion(string value, string questionTitle)
        {
            EnterValueForQuestion(value, questionTitle, false);
        }
        [When(@"I enter ""(.*)"" for ""(.*)"" question in PaperDCF")]
        [Given(@"I enter ""(.*)"" for ""(.*)"" question in PaperDCF")]
        public void IEnterValueForQuestionForPaperDCF(string value, string questionTitle)
        {
            EnterValueForQuestion(value, questionTitle, true);
        }

        private void EnterValueForQuestion(string value, string questionTitle, bool isPaperDCF)
        {
            var dataCorrectionTable = dataCorrectionPage.GetWebElementById("DataCorrectionTable");
            var rows = dataCorrectionTable.FindElements(By.TagName("tr"));

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));

                if (rowCells[0].Text.Contains(questionTitle))
                {
                    int index = isPaperDCF ? 1 : 2;
                    var inputField = rowCells[index].FindElements(By.TagName("input")).First(e => e.GetAttribute("type") != "hidden");
                    inputField.EnterText(value);
                    //click on label to dismiss any popup widget such as datetime picker ,etc
                    rowCells[0].Click();
                }
            }
        }

        [When(@"I enter current day for ""(.*)"" ""(.*)"" DCF")]
        [Given(@"I enter current day for ""(.*)"" ""(.*)"" DCF")]
        public void IEnterValueForQuestionForDCF(string questionTitle, string fieldName)
        {
            EnterValueForDCF(questionTitle, fieldName);
        }

        private void EnterValueForDCF(string questionTitle, string fieldName)
        {
            var rows = dataCorrectionPage.GetWebElementsByCss("#DataCorrectionTable tbody tr");

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));

                if (rowCells[0].Text.Contains(questionTitle))
                {
                    var inputField = rowCells[2].FindElements(By.TagName("input")).First(e => e.GetAttribute("name") == fieldName);
                    inputField.Click();
                    var getCalendar = rowCells[2].FindElements(By.TagName("div")).FirstOrDefault(e => e.GetAttribute("class").Contains("bootstrap-datetimepicker"));
                    var days = getCalendar.FindElements(By.TagName("td"));
                    var day = days.FirstOrDefault(x => x.Text == DateTime.Now.Day.ToString());
                    day.Click();
                    //click on label to dismiss any popup widget such as datetime picker ,etc
                    rowCells[0].Click();
                }
            }
        }

        [Given(@"""(.*)"" is displayed for ""(.*)"" question in PaperDCF")]
        [When(@"""(.*)"" is displayed for ""(.*)"" question in PaperDCF")]
        [Then(@"""(.*)"" is displayed for ""(.*)"" question in PaperDCF")]
        public async Task ValueIsDisplayedForQuestionInPaperDCF(string value, string questionTitle)
        {
            ValueDisplayedForQuestion(value, questionTitle, true);
        }

        private void ValueDisplayedForQuestion(string value, string questionTitle, bool isPaperDCF)
        {
            var dataCorrectionTable = dataCorrectionPage.GetWebElementById("DataCorrectionTable");
            var rows = dataCorrectionTable.FindElements(By.TagName("tr"));

            var answerDisplayed = false;

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                    int index = isPaperDCF ? 1 : 2;
                    var inputField = rowCells[index].FindElement(By.TagName("input"));
                    var valueAttribute = inputField.GetAttribute("value");

                    Assert.AreEqual(
                        value.Trim(),
                        valueAttribute.Trim(),
                        $"Response does not match {value}");

                    answerDisplayed = true;
                }
            }

            Assert.IsTrue(answerDisplayed, $"Answer for {questionTitle} not found");
        }

        [Given(@"""(.*)"" is displayed for ""(.*)"" question")]
        [When(@"""(.*)"" is displayed for ""(.*)"" question")]
        [Then(@"""(.*)"" is displayed for ""(.*)"" question")]
        public async Task ValueIsDisplayedForQuestion(string value, string questionTitle)
        {
            ValueDisplayedForQuestion(value, questionTitle, false);
        }

        [Given(@"I scroll to minimum allowed value for question ""(.*)"" in PaperDCF")]
        [When(@"I scroll to minimum allowed value for question ""(.*)"" in PaperDCF")]
        public void ScrollToMinimumValueInPaperDCF(string questionTitle)
        {
            ScrollToMin(questionTitle, true);
        }

        public void ScrollToMin(string questionTitle, bool isPaperDCF)
        {
            var dataCorrectionTable = dataCorrectionPage.GetWebElementById("DataCorrectionTable");
            var rows = dataCorrectionTable.FindElements(By.TagName("tr"));


            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                    int index = isPaperDCF ? 1 : 2;
                    var inputField = rowCells[index].FindElement(By.TagName("input"));
                    var minValueAttribute = inputField.GetAttribute("min");
                    inputField.EnterText(minValueAttribute);
                }
            }
        }

        [Given(@"I scroll to minimum allowed value for question ""(.*)""")]
        [When(@"I scroll to minimum allowed value for question ""(.*)""")]
        public void ScrollToMinimumValue(string questionTitle)
        {
            ScrollToMin(questionTitle, false);
        }

        [Given(@"I scroll to maximum allowed value for question ""(.*)""")]
        [When(@"I scroll to maximum allowed value for question ""(.*)""")]
        public void ScrollToMaximumValue(string questionTitle)
        {
            ScrollToMax(questionTitle, false);
        }

        [Given(@"I scroll to maximum allowed value for question ""(.*)"" in PaperDCF")]
        [When(@"I scroll to maximum allowed value for question ""(.*)"" in PaperDCF")]
        public void ScrollToMaximumValueForPaperDCF(string questionTitle)
        {
            ScrollToMax(questionTitle, true);
        }

        [Then(@"""(.*)"" is not selected for ""(.*)"" question for questionnaire ""(.*)""")]
        [When(@"""(.*)"" is not selected for ""(.*)"" question for questionnaire ""(.*)""")]
        [Given(@"""(.*)"" is not selected for ""(.*)"" question for questionnaire ""(.*)""")]
        public void ChoiceIsNotSelectedForCheckBoxQuestionnaire(string choice, string question, string questionnaire)
        {
            CheckForSelectionOfCheckbox(choice, questionnaire, question, false);
        }

        [Then(@"""(.*)"" is selected for ""(.*)"" question for questionnaire ""(.*)""")]
        [When(@"""(.*)"" is selected for ""(.*)"" question for questionnaire ""(.*)""")]
        [Given(@"""(.*)"" is selected for ""(.*)"" question for questionnaire ""(.*)""")]
        public void ChoiceIsSelectedForCheckBoxQuestionnaire(string choice, string question, string questionnaire)
        {
            CheckForSelectionOfCheckbox(choice, questionnaire, question, true);
        }

        private async void CheckForSelectionOfCheckbox(string choice, string questionnaire, string question, bool boolValue)
        {
            var value = TransformTableValue(choice);

            var questionId = await e2eRepository.GetQuestionIdFromName(questionnaire, question);

            var checkboxAnswers = dataCorrectionPage.GetWebElementsByXPath($"//*[@questionid='{questionId}'][@type='checkbox']");
            bool found = false;
            dataCorrectionPage.WaitForAjax();

            int tries = 2;
            int counter = 0;

            while (counter != tries || !found)
            {
                foreach (var checkbox in checkboxAnswers)
                {
                    var id = checkbox.GetAttribute("id");
                    var checkboxLabel = dataCorrectionPage.GetWebElementByXPath($"//label[@for='" + id + "']");
                    dataCorrectionPage.WaitForAjax();
                    if (checkboxLabel.Text == value)
                    {
                        found = true;
                        Assert.AreEqual(boolValue, checkbox.Selected);
                    }
                }
                counter++;
            }

            if (!found)
            {
                Assert.Fail();
            }
        }

        [Then(@"clear other responses is ""(.*)"" for ""(.*)"" choice for ""(.*)"" question for questionnaire ""(.*)""")]
        [When(@"clear other responses is ""(.*)"" for ""(.*)"" choice for ""(.*)"" question for questionnaire ""(.*)""")]
        [Given(@"clear other responses is ""(.*)"" for ""(.*)"" choice for ""(.*)"" question for questionnaire ""(.*)""")]
        public async Task ClearOtherReponsesForChoice(string enabled, string choice, string question, string questionnaire)
        {
            var questionnaireId = await e2eRepository.GetQuestionnaireIDFromName(questionnaire);
            var questionId = await e2eRepository.GetQuestionIdFromName(questionnaire, question);                         
            await Hooks.mockServer.UpdateQuestionnaireMockedValues(enabled == "enabled" ? "true" : "false", choice, questionId, "clear other responses", "choice", questionnaireId.ToString());
        }

        [Then(@"max value is set with value ""(.*)"" for ""(.*)"" question for questionnaire ""(.*)""")]
        [When(@"max value is set with value ""(.*)"" for ""(.*)"" question for questionnaire ""(.*)""")]
        [Given(@"max value is set with value ""(.*)"" for ""(.*)"" question for questionnaire ""(.*)""")]
        public async Task SetMaxTextLengthForTextArea(string value, string question, string questionnaire)
        {
            var questionnaireId = await e2eRepository.GetQuestionnaireIDFromName(questionnaire);
            var questionId = await e2eRepository.GetQuestionIdFromName(questionnaire, question);
            await Hooks.mockServer.UpdateQuestionnaireMockedValues(value, "", questionId, "", "maxText", questionnaireId.ToString());
        }

        [Given(@"""(.*)"" for ""(.*)"" is in ""(.*)"" status")]
        public void VisitForPatientIsInStatus(string patientVisitMappingName, string patientMappingName, string status)
        {
            var patientId = apiTestData.PatientMappings.FirstOrDefault(p => p.MappingName == patientMappingName).Patient.Id;
            var patientVisit = e2eRepository.AddPatientVisit(patientId, patientVisitMappingName, status);

            var patientVisitMapping = new PatientVisitMapping
            {
                MappingName = patientVisitMappingName,
                PatientVisit = patientVisit,
            };

            apiTestData.PatientVisitMappings.Add(patientVisitMapping);
        }

        private void ScrollToMax(string questionTitle, bool isPaperDCF)
        {
            var dataCorrectionTable = dataCorrectionPage.GetWebElementById("DataCorrectionTable");
            var rows = dataCorrectionTable.FindElements(By.TagName("tr"));

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.FindElements(By.TagName("td"));
                if (rowCells[0].Text.Contains(questionTitle))
                {
                    int index = isPaperDCF ? 1 : 2;
                    var inputField = rowCells[index].FindElement(By.TagName("input"));
                    var maxValueAttribute = inputField.GetAttribute("max");
                    inputField.EnterText(maxValueAttribute);
                }
            }
        }
    }
}
