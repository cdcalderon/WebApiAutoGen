using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class UserRoleEditSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly RoleManagementPage roleManagementPage;
        private readonly E2ERepository e2eRepository;
        private readonly TestData testData;

        public UserRoleEditSteps(
            ScenarioService scenarioService,
            RoleManagementPage roleManagementPage,
            E2ERepository e2eRepository,
            TestData testData)
        {
            this.scenarioService = scenarioService;
            this.roleManagementPage = roleManagementPage;
            this.e2eRepository = e2eRepository;
            this.testData = testData;
        }

        [Given(@"I am on Role Management page")]
        public void GivenIAmOnRoleManagementPage()
        {
            e2eRepository.AddBaseSoftwareRelease();

            scenarioService.ChromeDriver.Navigate().GoToUrl(roleManagementPage.e2eSettings.PortalUrl);

            var manageStudyBtn = roleManagementPage.GetWebElementById("manageStudyBtn");
            manageStudyBtn.Click();            
            var btn = roleManagementPage.GetWebElementById("roleManagement");            
            btn.Click();
        }

        [Then(@"the following data is displayed in the grid")]
        public void ThenTextIsDisplayedInTheField(Table table)
        {
            //wait for the grid to render
            var gridElem = scenarioService.ChromeDriver.FindElement(By.CssSelector("a.btn.btn-primary.btn-block"));

            foreach (var row in table.Rows)
            {
                var name = row["Name"];
                IWebElement elem = null;
                foreach (var element in scenarioService.ChromeDriver.FindElements(By.CssSelector("tr td:first-child")))
                {
                    var text = element.Text;
                    if (text == name)
                    {
                        elem = element;
                        break;
                    }
                }

                Assert.IsNotNull(elem);

                var rowElem = elem.FindElement(By.XPath(".."));
                
                var lastUpdate = row["Last Update"];
                var setPermissions = row["Set Permissions"].Replace(" button", "");
                var setSubscriptions = row["Set Subscriptions"].Replace(" button", "");
                var setReports = row["Set Reports"].Replace(" button", "");

                var index = 0;
                foreach (var columnElem in rowElem.FindElements(By.CssSelector("td")))
                {
                    var colText = columnElem.Text;
                    var elemFull = columnElem.TagName;
                    switch (index)
                    {
                        case 0:
                            Assert.IsTrue(colText.ToUpper() == name.ToUpper());
                            break;
                        case 1:
                            if (lastUpdate.ToUpper() == "NOT NULL")
                            {
                                Assert.IsFalse(string.IsNullOrWhiteSpace(colText));
                            }
                            else
                            {
                                Assert.IsTrue(string.IsNullOrWhiteSpace(colText));
                            }                            
                            break;
                        case 2:
                            var buttonElem1 = columnElem.FindElement(By.CssSelector("a.btn.btn-primary.btn-block"));
                            Assert.IsTrue(buttonElem1.Displayed);
                            Assert.IsTrue(colText.ToUpper().Contains(setPermissions.ToUpper()));
                            break;
                        case 3:
                            var buttonElem2 = columnElem.FindElement(By.CssSelector("a.btn.btn-primary.btn-block"));
                            Assert.IsTrue(buttonElem2.Displayed);
                            Assert.IsTrue(colText.ToUpper().Contains(setSubscriptions.ToUpper()));
                            break;
                        case 4:
                            var buttonElem3 = columnElem.FindElement(By.CssSelector("a.btn.btn-primary.btn-block"));
                            Assert.IsTrue(buttonElem3.Displayed);
                            Assert.IsTrue(colText.ToUpper().Contains(setReports.ToUpper()));
                            break;
                        default:
                            break;
                    }
                    index++;
                }
            }
        }

        [Given(@"I am on At a Glance page")]
        [When(@"I am on At a Glance page")]
        [Then(@"I am on At a Glance page")]
        public void GivenIAmOnAtAGlancePage()
        {
            scenarioService.ChromeDriver.Navigate().GoToUrl(roleManagementPage.e2eSettings.PortalUrl);
            
            var btn = roleManagementPage.GetWebElementById("atAGlance");
            btn.Click();
        }

        [When(@"I click on the user icon")]
        [Given(@"I click on the user icon")]
        [Then(@"I click on the user icon")]
        public void WhenIClickOnTheUserIcon()
        {
            var iconId = "application-information-popover";
            var elem = scenarioService.ChromeDriver.FindElement(By.Id(iconId));
            elem.Click();
        }

        [Then(@"""(.*)"" text is displayed in user info menu")]
        public void ThenTextIsDisplayed(string inputText)
        {
            var elem = scenarioService.ChromeDriver.FindElement(By.CssSelector("h3.popover-title"));
            var text = elem.Text;
            Assert.IsTrue(text.Contains(inputText));
        }


        [Given(@"I click ""(.*)"" button for row ""(.*)""")]
        [When(@"I click ""(.*)"" button for row ""(.*)""")]
        [Then(@"I click ""(.*)"" button for row ""(.*)""")]
        public void WhenIClickButtonForRow(string buttonText, string rowName)
        {
            roleManagementPage.ThreadSleep();

            var roleTable = scenarioService.ChromeDriver.FindElement(By.Id("roles"));
            var roleTableBody = roleTable.FindElement(By.TagName("tbody"));
            var gridRows = roleTableBody.FindElements(By.TagName("tr"));

            var foundElement = false;

            foreach (var row in gridRows)
            {
                var nameCols = row.FindElements(By.CssSelector("td.sorting_1"));
                var nameCol = nameCols[0];

                if (nameCol.Text.Trim().ToUpper() != rowName.Trim().ToUpper())
                {
                    continue;
                }

                var buttons = row.FindElements(By.CssSelector("a.btn.btn-primary.btn-block"));

                foreach (var button in buttons)
                {
                    if (button.Text.Trim().ToUpper() == buttonText.Trim().ToUpper())
                    {
                        foundElement = true;

                        //role short name matches the last part of the href url
                        var href = button.GetAttribute("href");
                        var shortRoleName = href.Split("/").Last();

                        testData.SelectedRole = shortRoleName;
                        button.Click();
                        break;
                    }
                }

                break;
            }

            Assert.IsTrue(foundElement);
        }

        [Given(@"I am now on ""(.*)"" page")]
        [When(@"I am now on ""(.*)"" page")]
        [Then(@"I am now on ""(.*)"" page")]

        public void WhenIAmOnPage(string inputText)
        {
            var elem = scenarioService.ChromeDriver.FindElement(By.CssSelector("ul.breadcrumb"));
            var text = elem.Text;
            Assert.IsTrue(text.Contains(inputText));
        }



        [Then(@"""(.*)"" text is displayed on manage role page")]
        public void ThenTextIsDisplayedOnManageRolePage(string inputText)
        {
            var elem = scenarioService.ChromeDriver.FindElement(By.CssSelector("div.grid-wrapper div h1"));
            var text = elem.Text;
            Assert.IsTrue(text.ToUpper().Equals(inputText.ToUpper()));
        }
        [Then(@"""(.*)"" graph is displayed")]
        public void ThenGraphIsDisplayed(string p0)
        {
            var elem = scenarioService.ChromeDriver.FindElement(By.XPath("//div[contains(@id,'chartContainer')]"));
            Assert.IsTrue(elem.Displayed);
        }

        [Then(@"""(.*)"" is ""(.*)"" below the graph")]
        public void ThenIsBelowTheGraph(string inputText, string p1)
        {
            var elem = scenarioService.ChromeDriver.FindElement(By.XPath("//*[contains(@id,'centreLabel')]"));
            var text = elem.Text;
            Assert.IsTrue(text.ToUpper().Equals(inputText.ToUpper()));
        }



    }
}