using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class WebBackupPage : BasePage
    {
        private readonly E2ESettings _e2eSettings;
        private readonly E2ERepository _e2ERepository;
        private readonly ScenarioService _scenarioService;
        private readonly AddSitePage _addSitePage;
        public WebBackupPage(E2ESettings e2eSettings,
            ScenarioService scenarioService,
            E2ERepository e2ERepository, AddSitePage addSitePage) : base(scenarioService)
        {
            _e2ERepository = e2ERepository;
            _e2eSettings = e2eSettings;
            _scenarioService = scenarioService;
            _addSitePage = addSitePage;
        }

        public override string PageName => "Web Backup";

        public override string PageUrl => $"{_e2eSettings.PortalUrl}/WebBackup";

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Text", Label = "Home Web Backup", UiControl = "text",LocatorType="class",LocatorValue="breadcrumb"},
            new FieldMap() { FieldType = "Text", Label = "If you do not see your site, please ensure that Web Backup is currently enabled.", UiControl = "text",LocatorType="xpath",LocatorValue="//ul[@class='breadcrumb']//following-sibling::p"},
            new FieldMap() { FieldType = "Grid", Label = "Web Backup Details", Id = "sites", UiControl = "table" },
        };

        public override string GetDropdownSelectedValue(string control)
        {
            throw new NotImplementedException();
        }

        public IWebElement GetLaunchWebBackupButton(string site)
        {
            var roleTable = _scenarioService.ChromeDriver.FindElement(By.Id("sites"));
            var roleTableBody = roleTable.FindElement(By.TagName("tbody"));
            var gridRows = roleTableBody.FindElements(By.TagName("tr"));

            var foundElement = false;
            IWebElement webElement = null;

            foreach (var row in gridRows)
            {
                var nameCols = row.FindElements(By.CssSelector("td.sorting_1"));
                var nameCol = nameCols[0];

                if (nameCol.Text.Trim().ToUpper() != site.Trim().ToUpper())
                {
                    continue;
                }

                var buttons = row.FindElements(By.CssSelector("button.btn.btn-primary"));

                foreach (var button in buttons)
                {
                    if (button.Text.Trim().ToUpper() == "Launch Web Backup".Trim().ToUpper())
                    {
                        foundElement = true;
                        Assert.IsTrue(foundElement);
                        webElement = button;
                        break;
                    }
                }
                break;
            }
            return webElement;
        }

        public void VerifyLaunchWebBackupButton(bool isEnabled, string site)
        {
            IWebElement button = GetLaunchWebBackupButton(site).FindElement(By.XPath("./parent::td"));

            if (isEnabled)
                Assert.IsTrue(button.Enabled);
            else
            {
                Assert.AreEqual(button.GetAttribute("style"), "opacity: 0.5;");
                Assert.AreEqual(button.GetAttribute("title"), "There are no devices at this site");
            }
        }

        public void ClickLaunchWebBackupForSite(string site)
        {
            IWebElement button = GetLaunchWebBackupButton(site);
            button.Click();
        }
    }
}

