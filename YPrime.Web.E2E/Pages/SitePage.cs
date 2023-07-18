using OpenQA.Selenium;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SitePage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        private const string SiteNumberLinkClassName = "site-number-link";

        public override string PageName => "Site Management";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Site";

        public override string GetDropdownSelectedValue(string control)
        {
            var result = string.Empty;

            var matchingFieldMap = FieldMaps().First(fm => fm.Label == control);

            if (matchingFieldMap.UiControl == "metro_dropdown")
            {
                var parent = GetWebElementById(matchingFieldMap.Id).GetParent().GetParent();

                var selectedElement = parent.FindElement(By.ClassName("selected"));

                result = selectedElement.Text;
            }

            return result;

        }
        public SitePage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Button", Label = "Add New Site", Id = "btnAddSite", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Bulk Edit Sites", Id = "btnBulkEditSite", UiControl = "button", WaitForElement = false },

        };
        public void VerifyColumnValues(Table table)
        {
            var fields = table.CreateSet<setValues>();
            foreach (var field in fields)
            {

                string expectedValueOfSiteNumber = field.SiteNumber;
                string actualValueOfSiteNumber = ScenarioService.ChromeDriver.FindElement(By.Id("SiteNumber-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfSiteNumber, actualValueOfSiteNumber);

                string expectedValueOfName = field.Name;
                string actualValueOfName = ScenarioService.ChromeDriver.FindElement(By.Id("Name-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfName, actualValueOfName);

                string expectedValueOfAddress = field.Address;
                string actualValueOfAddress = ScenarioService.ChromeDriver.FindElement(By.Id("Address-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfAddress, actualValueOfAddress);

                string expectedValueOfCity = field.City;
                string actualValueOfCity = ScenarioService.ChromeDriver.FindElement(By.Id("City-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfCity, actualValueOfCity);

                string expectedValueOfCountry = field.Country;
                string actualValueOfCountry = ScenarioService.ChromeDriver.FindElement(By.Id("Country-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfCountry, actualValueOfCountry);

                string expectedValueOfPhoneNumber = field.PhoneNumber;
                string actualValueOfPhoneNumber = ScenarioService.ChromeDriver.FindElement(By.Id("PhoneNumber-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfPhoneNumber, actualValueOfPhoneNumber);

                string expectedValueOfPrimaryContact = field.PrimaryContact;
                string actualValueOfPrimaryContact = ScenarioService.ChromeDriver.FindElement(By.Id("PrimaryContact-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfPrimaryContact, actualValueOfPrimaryContact);

                string expectedValueOfActive = field.Active;
                string actualValueOfActive = ScenarioService.ChromeDriver.FindElement(By.Id("Active-" + field.SiteNumber)).Text;
                Assert.AreEqual(expectedValueOfActive, actualValueOfActive);
            }
        }

        class setValues
        {
            public string SiteNumber { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string PhoneNumber { get; set; }
            public string PrimaryContact { get; set; }
            public string Active { get; set; }
        }
        public IWebElement GetSiteNumberLink(string siteNumber)
        {
            var elements = ScenarioService.ChromeDriver.FindElements(By.ClassName(SiteNumberLinkClassName));

            var matchingNumberLink = elements
                .First(sn => sn.Text.Trim().ToUpper() == siteNumber.ToUpper());

            return matchingNumberLink;
        }
    }
}

