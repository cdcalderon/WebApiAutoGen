using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using YPrime.Web.E2E.Models;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;
using System.IO;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Interactions;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Utilities;
using TechTalk.SpecFlow.Assist;
using YPrime.Core.BusinessLayer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YPrime.Web.E2E.Data;

namespace YPrime.Web.E2E.Pages
{
    public abstract class BasePage
    {
        protected ScenarioService ScenarioService { get; private set; }
        protected BasePage(ScenarioService scenarioService)
        {
            ScenarioService = scenarioService;
        }

        protected E2ERepository e2eRepository { get; private set; }
        protected BasePage(ScenarioService scenarioService, E2ERepository e2eRepository)
        {
            this.e2eRepository = e2eRepository;
            ScenarioService = scenarioService;
        }

        public abstract string PageName { get; }
        public abstract string PageUrl { get; }
        public abstract string GetDropdownSelectedValue(string control);
        public virtual int WaitInterval => 500;
        public abstract List<FieldMap> FieldMaps();
        public Dictionary<string, IWebElement> MainMenuLinks
        {
            get => new Dictionary<string, IWebElement>
            {
                { "Subject", BtnSubjects },
                { "Manage Study", BtnManageStudy },
                { "Sites", BtnSites },
                { "Analytics & Reports", BtnAnalytics },
                { "At a Glance", BtnAtaGlance},
                { "User Information", BtnUserInformation}
            };
        }
        public IWebElement BtnSubjects => ScenarioService.ChromeDriver.FindElement(By.XPath("//a[@menu-group='Patients']"));
        public IWebElement BtnManageStudy => ScenarioService.ChromeDriver.FindElement(By.XPath("//a[@menu-group='ManageStudy']"));
        public IWebElement BtnSites => ScenarioService.ChromeDriver.FindElement(By.XPath("//a[@menu-group='Sites']"));

        public IWebElement BtnAnalytics => ScenarioService.ChromeDriver.FindElement(By.XPath("//a[@menu-group='AnalyticsReports']"));

        public IWebElement BtnAtaGlance => ScenarioService.ChromeDriver.FindElement(By.Id("atAGlance"));
        public IWebElement BtnUserInformation => ScenarioService.ChromeDriver.FindElement(By.Id("application-information-popover"));

        public virtual void NavigateToPage()
        {
            WaitForAjax();
            var currentUrl = ScenarioService.ChromeDriver.Url;
            if (currentUrl != PageUrl)
            {
                if (currentUrl.Contains(PageUrl)) return;
                ScenarioService.ChromeDriver.Navigate().GoToUrl(PageUrl);
            }
        }

        public void NavigateTo(string url)
        {
            ScenarioService.ChromeDriver.Navigate().GoToUrl(url);
        }

        public IWebElement GetWebElementById(string elementId)
        {
            return ScenarioService.ChromeDriver.FindElement(By.Id(elementId));
        }

        public ReadOnlyCollection<IWebElement> GetWebElementsByClass(string elementId)
        {
            return ScenarioService.ChromeDriver.FindElements(By.ClassName(elementId));
        }

        public ReadOnlyCollection<IWebElement> GetWebElementsById(string elementId)
        {
            return ScenarioService.ChromeDriver.FindElements(By.Id(elementId));
        }

        public IWebElement GetWebElementByXPath(string xPath)
        {
            return ScenarioService.ChromeDriver.FindElement(By.XPath(xPath));
        }

        public ReadOnlyCollection<IWebElement> GetWebElementsByXPath(string xPath)
        {
            return ScenarioService.ChromeDriver.FindElements(By.XPath(xPath));
        }

        public IWebElement GetWebElementByCss(string cssSelector)
        {
            return ScenarioService.ChromeDriver.FindElement(By.CssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> GetWebElementsByCss(string cssSelector)
        {
            return ScenarioService.ChromeDriver.FindElements(By.CssSelector(cssSelector));
        }

        public void EnterText(IWebElement element, string value)
        {
            element.Click();
            element.Clear();
            element.SendKeys(value);
        }

        public string GetTextFromElement(IWebElement element)
        {
            return element.Text;
        }
        /// <summary>
        /// Validates the string with the date against a format date. E.g. The date "16-November-2021 7:43:57 PM (UTC)" , should match the format "d-MMMM-yyyy h:mm:ss tt (UTC)"
        /// </summary>
        public bool IsDateWithValidFormat(String date, String dateFormat)
        {
            DateTime d;
            bool chValidity = DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
            Console.WriteLine(chValidity);
            return chValidity;
        }


        public void ExecuteDriverScript(string arguments, IWebElement element)
        {
            ScenarioService.ChromeDriver.ExecuteScript(arguments, element);
        }

        public void SelectOptions(IWebElement element, string optionText, bool selectFirstIndex)
        {
            element.Click();
            var selectList = new SelectElement(element);
            if (!selectFirstIndex)
            {
                selectList.SelectByText(optionText);
            }
            else
            {
                selectList.SelectByIndex(1);
            }
        }
        public void RefreshPage()
        {
            ScenarioService.ChromeDriver.Navigate().Refresh();
        }
        public void ThreadSleep()
        {
            Thread.Sleep(WaitInterval);
        }

        public void WaitForAjax()
        {
            while (true) // Handle timeout somewhere
            {
                var ajaxIsComplete = (bool)((IJavaScriptExecutor)ScenarioService.ChromeDriver).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    break;
                Thread.Sleep(100);
            }
        }
        public void WaitForSpinner()
        {
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.Id("loadingSpinner")));
        }
        public void WaitForPopUp()
        {
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("confirmationModal")));
        }
        public void WaitForPopUp(string popupName)
        {
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(popupName)));
        }
        public virtual IWebElement GetOnPageElementByLabel(string label)
        {
            var matchingFieldMap = FieldMaps().FirstOrDefault(fm => fm.Label == label);
            var element = matchingFieldMap != null
                ? GetWebElementById(matchingFieldMap.Id)
                : null;
            return element;
        }
        public void WaitForDialog()
        {
            WaitForAjax();
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, new TimeSpan(0, 0, 60));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("bootstrap-dialog")));
        }
        public void SelectFromDropdown(string control, string item)
        {
            WaitForAjax();
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var options = element.FindElements(By.TagName("option"));
            var select = options.SingleOrDefault(i => i.Text == item);
            select.Click();
        }
        public int TotalRows(string control)
        {
            WaitForAjax();
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            Thread.Sleep(3000);
            var element = GetWebElementById(elemId);
            var tableElement = element.FindElement(By.TagName("tbody"));
            var rowSize = tableElement.FindElements(By.TagName("tr")).Count;
            return rowSize;
        }
        public void SearchFromTable(string searchText, string searchHeader)
        {
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector($".dataTable input[placeholder = '{searchHeader}']")));
            var element = GetWebElementByCss($".dataTable input[placeholder='{searchHeader}']");
            IJavaScriptExecutor js = (IJavaScriptExecutor)ScenarioService.ChromeDriver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector($".dataTable input[placeholder = '{searchHeader}']")));
            EnterText(element, searchText);
        }

        public FileInfo CheckFileExists(String filePath)
        {
            bool fileExists = false;
            FileInfo fileInfo = null;
            try
            {
                var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(60));
                Wait.Until<bool>(x => fileExists = File.Exists(filePath));
                fileInfo = new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fileInfo;
        }


        public FileInfo GetDownloadFileInfo(string downloadFileName, string format)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string folderName = "Downloads";
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + downloadFileName + format;
            FileInfo fileInfo = null;
            try
            {
                fileInfo = CheckFileExists(expectedFilePath);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if ((fileInfo != null) & File.Exists(expectedFilePath))
                {
                    var destinationFolder = "ExportEvidence";
                    string destinationFilePath = projectDirectory + "\\" + destinationFolder + "\\" + downloadFileName + format;
                    File.Move(expectedFilePath, destinationFilePath, true);
                    File.Delete(expectedFilePath);
                }
            }
            return fileInfo;
        }

        public FileInfo GetDownloadFileInfoExclusive(string downloadFileName)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string folderName = "Downloads";
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\";
            string[] Filematches = Directory.GetFiles(@expectedFilePath, downloadFileName);
            bool fileExists = false;
            FileInfo fileInfo = null;
            try
            {
                foreach (var Filematch in Filematches)
                {
                    var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(60));
                    Wait.Until<bool>(x => fileExists = File.Exists(Filematch));
                    fileInfo = new FileInfo(Filematch.ToString());
                }
            }

            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                foreach (var Filematch in Filematches)
                {
                    var destinationFolder = "ExportEvidence";
                    string destinationFilePath = projectDirectory + "\\" + destinationFolder;
                    File.Delete(Filematch);
                }

            }
            return fileInfo;
        }
        public IWebElement GetWebElementByLinkText(string text) => ScenarioService.ChromeDriver.FindElement(By.LinkText(text));
        public void ClickButtonByLabel(string label)
        {
            WaitForCssElementExists("a.btn.btn-primary");
            var buttons = GetWebElementsByCss("a.btn.btn-primary");
            var button = buttons.SingleOrDefault(e => e.Text == label);
            button.Click();
        }

        public void ClickByJavascript(string id)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)ScenarioService.ChromeDriver;
            js.ExecuteScript("document.getElementById('" + id + "').click()");
        }

        private void WaitForCssElementExists(string cssSelector)
        {
            WaitForAjax();
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector(cssSelector)));
        }

        public bool IsTextDisplayed(string text)
        {
            WaitForAjax();
            var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(5));
            Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.TagName("body")));
            var bodyTag = ScenarioService.ChromeDriver.FindElement(By.TagName("body"));
            var bodyString = bodyTag.Text.Trim().Replace(" ", "").Trim();
            var value = text.Replace(" ", "").Trim();
            if (bodyString.Contains(value))
            {
                return true;
            }
            return false;
        }
        public void WaitForElementIsVisible(By locator)
        {
            try
            {
                var Wait = new WebDriverWait(ScenarioService.ChromeDriver, TimeSpan.FromSeconds(10));
                Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(locator));
            }
            catch (Exception ex)
            {
                //specifically catching NoSuchElementException
            }
        }
        public void HoverOnElement(IWebElement element)
        {
            Actions action = new Actions(ScenarioService.ChromeDriver);
            action.MoveToElement(element).Build().Perform();
        }
        public void ClickOnElement(IWebElement element)
        {
            Actions action = new Actions(ScenarioService.ChromeDriver);
            action.MoveToElement(element).Click().Build().Perform();
            
        }
        public void OpenLinkInNewTab(string element)
        {
            ScenarioService.ChromeDriver.SwitchTo().NewWindow(WindowType.Tab);
            ScenarioService.ChromeDriver.Navigate().GoToUrl(element);
        }
        public bool VerifyGridData(string grid, Table table, IWebElement tableWebElement, out string message)
        {
            ICollection<string> expectedTableHeaders = table.Header;
            List<int> headersIndex = GetHeaderIndex(expectedTableHeaders, tableWebElement, out bool isHeaderMatched);
            if (!isHeaderMatched)
            {
                message = $"{grid} grid header  is not matched";
                return false;
            }
            List<List<string>> tableData = GetTableData(headersIndex, tableWebElement);
            bool result = CompareTableData(table, tableData);
            message = (result ? $"{grid} grid data is matched" : $"{ grid} grid data is not matched");
            return result;
        }

        public bool CompareTableData(Table Expectedtable, List<List<string>> ActualtableData)
        {
            bool result = true;
            List<List<string>> ExpectedTableRows = new List<List<string>>();
            foreach (var row in Expectedtable.Rows.ToList())
            {
                ExpectedTableRows.Add(row.Values.ToList());
            }


            foreach (var expectedrowOriginalRow in ExpectedTableRows)
            {
                if (result == false)
                {
                    return false;
                }
                int count = 0, index = 0;
                string date = string.Empty, formatedDateTimeUTC = string.Empty;
                List<string> expectedrow = new List<string>(expectedrowOriginalRow);
                List<string> expectedrowOriginalRowCopy = new List<string>(expectedrowOriginalRow);

                foreach (var actualrow in ActualtableData)
                {

                    if (result == false && count > 0)
                    {
                        expectedrow = new List<string>(expectedrowOriginalRowCopy);
                    }
                    #region replace "current date" string with  actual date.

                    if (expectedrow.Contains("Current Date"))
                    {
                        ++count;
                        date = DateTime.Now.ToString("dd-MMM-yyyy");
                        while (expectedrow.IndexOf("Current Date") != -1)
                        {
                            index = expectedrow.IndexOf("Current Date");
                            expectedrow[index] = date;
                            actualrow[index] = CommonUtilities.getDateFromDatetimeUI(actualrow[index]);
                        }
                    }

                    else if (expectedrow.Contains("(dd-MMM-yy)Current Date"))
                    {
                        ++count;
                        date = DateTime.Now.ToString("dd-MMM-yy");
                        while (expectedrow.IndexOf("(dd-MMM-yy)Current Date") != -1)
                        {
                            index = expectedrow.IndexOf("(dd-MMM-yy)Current Date");
                            expectedrow[index] = date;
                            actualrow[index] = CommonUtilities.getDateFromDatetimeUI(actualrow[index]);
                        }
                    }
                    if (expectedrow.Contains("(dd-MMMM-yyyy)Current Date"))
                    {
                        ++count;
                        date = DateTime.Now.ToString("dd-MMMM-yyyy");
                        while (expectedrow.IndexOf("(dd-MMMM-yyyy)Current Date") != -1)
                        {
                            index = expectedrow.IndexOf("(dd-MMMM-yyyy)Current Date");
                            expectedrow[index] = date;
                            actualrow[index] = CommonUtilities.getDateFromDatetimeUI(actualrow[index]);
                        }
                    }
                    if (expectedrow[0].Contains("DCF Report"))
                    {
                        index = expectedrow.IndexOf(expectedrow[0]);
                        expectedrow[index] = e2eRepository.GetDataCorrectionNumber(expectedrow[3]);
                        actualrow[index] = (actualrow[index]).TrimStart('0').ToString();
                    }
                    if (expectedrow.Contains(CommonData.createdDateTime))
                    {
                        formatedDateTimeUTC = (DateTimeOffset.Parse(e2eRepository.GetExportGridDate(CommonData.createdDateTime))).ToUniversalTime().UtcDateTime.ToString("dd-MMM-yyyy HH:mm");
                        index = expectedrow.IndexOf(CommonData.createdDateTime);
                        expectedrow[index] = formatedDateTimeUTC;
                        actualrow[index] = (actualrow[index]).ToString();
                    }
                    if (expectedrow.Contains(CommonData.StartedDateTime))
                    {
                        formatedDateTimeUTC = (DateTimeOffset.Parse(e2eRepository.GetExportGridDate(CommonData.StartedDateTime))).ToUniversalTime().UtcDateTime.ToString("dd-MMM-yyyy HH:mm");
                        index = expectedrow.IndexOf(CommonData.StartedDateTime);
                        expectedrow[index] = formatedDateTimeUTC;
                        actualrow[index] = (actualrow[index]).ToString();
                    }
                    if (expectedrow.Contains(CommonData.CompletedDateTime))
                    {
                        formatedDateTimeUTC = (DateTimeOffset.Parse(e2eRepository.GetExportGridDate(CommonData.CompletedDateTime))).ToUniversalTime().UtcDateTime.ToString("dd-MMM-yyyy HH:mm");
                        index = expectedrow.IndexOf(CommonData.CompletedDateTime);
                        expectedrow[index] = formatedDateTimeUTC;
                        actualrow[index] = (actualrow[index]).ToString();
                    }
                    if (expectedrow.Count > 2)
                    {
                        if (expectedrow[2].Contains("currentday"))
                        {
                            expectedrow[2] = ResolveDate(expectedrow[2], "dd-MMM-yyyy");
                        }
                    }
                    #endregion
                    #region Replace mapped user name with actual user name
                    for (var i = 0; i < expectedrow.Count; i++)
                    {
                        var matchingMapping = CommonData
                            .UserMappings
                            .FirstOrDefault(um => um.MappingName.Equals(expectedrow[i], StringComparison.InvariantCultureIgnoreCase));

                        if (matchingMapping != null)
                        {
                            expectedrow[i] = matchingMapping.Username;
                        }
                    }
                    #endregion
                }
            }
            return result;
        }
        public List<int> GetHeaderIndex(ICollection<string> headers, IWebElement element, out bool isHeadersMatched)
        {
            ReadOnlyCollection<IWebElement> headerElements;
            try
            {
                headerElements = element.FindElement(By.TagName("thead")).FindElement(By.TagName("tr")).FindElements(By.TagName("th"));
            }
            catch
            {
                headerElements = element.FindElement(By.TagName("tbody")).FindElement(By.TagName("tr")).FindElements(By.TagName("th"));
            }
            List<int> headersIndex = new List<int>();
            int count = 0;
            for (int index = 0; index < headerElements.Count; index++)
            {
                var headerName = headerElements.ElementAt(index).GetAttribute("innerText") == "" ? headerElements.ElementAt(index).GetAttribute("value") == "" ? headerElements.ElementAt(index).Text : headerElements.ElementAt(index).GetAttribute("value") : headerElements.ElementAt(index).GetAttribute("innerText");
                if (headers.Contains(headerName))
                {
                    headersIndex.Add(index);
                    count++;
                }
            }
            isHeadersMatched = (count == headers.Count) ? true : false;
            return headersIndex;
        }

        public List<List<string>> GetTableData(List<int> headerIndex, IWebElement element)
        {
            ReadOnlyCollection<IWebElement> rows = element.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
            List<List<string>> tableData = new List<List<string>>();
            foreach (var row in rows)
            {
                List<String> data = new List<String>();
                var rowData = row.FindElements(By.TagName("td"));
                if (rowData.Count > 0)
                {
                    foreach (var index in headerIndex)
                    {
                        data.Add(rowData.ElementAt(index).Text);
                    }
                    tableData.Add(data);
                }
            }
            return tableData;
        }

        public string GetJSONFileValueOfVisitFromKey(string visitName)
        {
            string binaryPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            List<VisitModel> AllVisitModel;
            string folderName = "Mocks", format = ".json", projectName = "YPrime.Web.E2E.MockServer";
            string projectDirectory = binaryPath + "\\" + projectName;
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + "VisitEndpoint" + format;
            string actualValue = null;
            try
            {
                FileInfo fileInfo = CheckFileExists(expectedFilePath);
                if ((fileInfo != null) & File.Exists(expectedFilePath))
                {
                    dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(expectedFilePath));
                    JToken key = jsonFile.SelectToken("Response.BodyAsJson");
                    AllVisitModel = JsonConvert.DeserializeObject<List<VisitModel>>(key.ToString());
                    foreach (var val in AllVisitModel)
                    {
                        if (val.Name == visitName)
                        {
                            actualValue = val.VisitOrder.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // catch any exception
            }
            return actualValue;
        }
        public int GetVisitQuestionaireCount(string visitName)
        {
            string binaryPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            string folderName = "Mocks", format = ".json", projectName = "YPrime.Web.E2E.MockServer";
            string projectDirectory = binaryPath + "\\" + projectName;
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + "VisitEndpoint" + format;
            int _visitQuestionnaire = 0;
            List<VisitModel> _visitModel;
            try
            {
                FileInfo fileInfo = CheckFileExists(expectedFilePath);
                if ((fileInfo != null) & File.Exists(expectedFilePath))
                {
                    dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(expectedFilePath));
                    JToken key = jsonFile.SelectToken("Response.BodyAsJson");
                    _visitModel = JsonConvert.DeserializeObject<List<VisitModel>>(key.ToString());
                    foreach (var v in _visitModel)
                    {
                        if(v.Name == visitName)
                        {
                            _visitQuestionnaire= v.Questionnaires.Count;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // catch any exception
            }
            return _visitQuestionnaire;
        }
        public string GetJSONFileValueFromKey(string jsonkey, string jsonvalue, string studySettingStudyCustomEndpoint)
        {
            string binaryPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            List<StudyCustomModel> AllStudyCustoms;
            string folderName = "Mocks", format = ".json", projectName = "YPrime.Web.E2E.MockServer";
            string projectDirectory = binaryPath + "\\" + projectName;
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + studySettingStudyCustomEndpoint + format;
            string actualValue = null;
            try
            {
                FileInfo fileInfo = CheckFileExists(expectedFilePath);
                if ((fileInfo != null) & File.Exists(expectedFilePath))
                {
                    dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(expectedFilePath));
                    JToken key = jsonFile.SelectToken("Response.BodyAsJson");
                    AllStudyCustoms = JsonConvert.DeserializeObject<List<StudyCustomModel>>(key.ToString());
                    foreach (var val in AllStudyCustoms)
                    {
                        if (val.Key == jsonkey)
                        {
                            actualValue = val.Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // catch any exception
            }
            return actualValue;
        }

        public string GetJSONFileValueFromKeytransalation(string jsonkey, string languageKey, string jsonvalue, string transalationEndpoint)
        {
            string binaryPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            List<TranslationModel> AllStudyCustoms;
            string folderName = "Mocks", format = ".json", projectName = "YPrime.Web.E2E.MockServer";
            string projectDirectory = binaryPath + "\\" + projectName;
            string expectedFilePath = projectDirectory + "\\" + folderName + "\\" + transalationEndpoint + format;
            string actualValue = null;
            try
            {
                FileInfo fileInfo = CheckFileExists(expectedFilePath);
                if ((fileInfo != null) & File.Exists(expectedFilePath))
                {
                    dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(expectedFilePath));
                    JToken key = jsonFile.SelectToken("Response.BodyAsJson");
                    AllStudyCustoms = JsonConvert.DeserializeObject<List<TranslationModel>>(key.ToString());
                    foreach (var val in AllStudyCustoms)
                    {
                        if (val.Id == jsonkey && val.LanguageId == languageKey)
                        {
                            actualValue = val.LocalText;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return actualValue;

        }
        public bool VerifyNewTabIsOpened(string value, bool switchToOriginalWindow = false)
        {
            bool result = false;
            var windowHandles = ScenarioService.ChromeDriver.WindowHandles;
            var parentWindowHandle = ScenarioService.ChromeDriver.CurrentWindowHandle;
            bool isOpenedInCurrentTab = ScenarioService.ChromeDriver.Url.Contains(value);
            foreach (var window in windowHandles)
            {
                ScenarioService.ChromeDriver.SwitchTo().Window(window);
                if (ScenarioService.ChromeDriver.Url.Contains(value))
                {
                    result = true;
                    break;
                }
            }
            if (switchToOriginalWindow)
            {
                ScenarioService.ChromeDriver.SwitchTo().Window(parentWindowHandle);
            }
            return result && !isOpenedInCurrentTab;
        }
        public IWebElement GetWebElement(string locatorType, string value)
        {
            switch (locatorType.ToLower())
            {
                case "xpath":
                    return ScenarioService.ChromeDriver.FindElement(By.XPath(value));
                case "css":
                    return ScenarioService.ChromeDriver.FindElement(By.CssSelector(value));
                case "class":
                    return ScenarioService.ChromeDriver.FindElement(By.ClassName(value));
                default:
                    return ScenarioService.ChromeDriver.FindElement(By.Id(value));
            }
        }
        public ReadOnlyCollection<IWebElement> GetWebElements(string locatorType, string value)
        {
            switch (locatorType.ToLower())
            {
                case "xpath":
                    return ScenarioService.ChromeDriver.FindElements(By.XPath(value));
                case "css":
                    return ScenarioService.ChromeDriver.FindElements(By.CssSelector(value));
                case "class":
                    return ScenarioService.ChromeDriver.FindElements(By.ClassName(value));
                default:
                    return ScenarioService.ChromeDriver.FindElements(By.Id(value));
            }
        }
        public string ResolveDate(string date, string format)
        {
            DateTime day = default;
            if (date.ToLower().Contains("currentday"))
                day = DateTime.UtcNow;

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(day, easternZone);

            if (date.Contains("+"))
                day = easternTime.AddDays(Convert.ToDouble(date.Split('+')[1]));
            else if (date.Contains("-"))
                day = easternTime.AddDays(-(Convert.ToDouble(date.Split('-')[1])));

            return day.ToString(format);
        }
    }
}