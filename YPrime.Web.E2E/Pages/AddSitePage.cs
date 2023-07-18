using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class AddSitePage : BasePage
    {
        private readonly E2ESettings _e2eSettings;

        private readonly E2ERepository _e2ERepository;

        public override string PageName => "Site Details";

        public override string PageUrl => $"{_e2eSettings.PortalUrl}/Site/AddEdit/";

        public AddSitePage(E2ESettings e2eSettings, 
            ScenarioService scenarioService,
            E2ERepository e2ERepository) : base(scenarioService)
        {
            _e2ERepository = e2ERepository;
            _e2eSettings = e2eSettings;
        }

        public override List<FieldMap> FieldMaps()
        {
            var fieldMaps = new List<FieldMap>() {
            new FieldMap() { FieldType = "Inputtextbox", Label = "Name", Id = "Name", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Site Number", Id = "SiteNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Primary Contact", Id = "PrimaryContact", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Investigator", Id = "Investigator", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Address1", Id = "Address1", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Address2", Id = "Address2", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Address3", Id = "Address3", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Fax Number", Id = "FaxNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "City", Id = "City", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "State", Id = "State", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Zip", Id = "Zip", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "Country", Id = "CountryId", UiControl = "metro_dropdown" },
            new FieldMap() { FieldType = "Inputtextbox", Label = "Phone Number", Id = "PhoneNumber", UiControl = "input", WaitForElement = false },
            new FieldMap() { FieldType = "Dropdown", Label = "TimeZone", Id = "TimeZone", UiControl = "metro_dropdown" },
            new FieldMap() { FieldType = "Toggle", Label = "Active", Id = "IsActive", UiControl = "toggle"},
            new FieldMap() { FieldType = "Toggle", Label = "WebBackupEnabled", Id = "IsWebBackupEnabled", UiControl = "toggle"},
            new FieldMap() { FieldType = "Button", Label = "Site Languages", Id = "siteLanguageTab", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Button", Label = "Site Details", Id = "siteTab", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Button", Label = "Next", Id = "next-save", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Button", Label = "Next Button To Site Language", Id = "next-language", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Button", Label = "Save", Id = "submit-form", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Toggle", Label = "en-US", Id = "en-US", UiControl = "toggle" },
            new FieldMap() { FieldType = "Toggle", Label = "en-UK", Id = "en-UK", UiControl = "toggle", WaitForElement = false},
            new FieldMap() { FieldType = "Button", Label = "Next Language", Id = "next-language", UiControl = "button", WaitForElement = false},
            new FieldMap() { FieldType = "Toggle", Label = "Site-Facing Text Display Languages", Id = "SiteDisplayLanguageId", UiControl = "toggle" },
            new FieldMap() { FieldType = "Dropdown", Label = "Site-Facing Text Display Language", Id = "SiteDisplayLanguageId", UiControl = "metro_dropdown" },
            new FieldMap() { FieldType = "Button", Label = "Save Tab", Id = "saveTab", UiControl = "button" },
            new FieldMap() { FieldType = "Text", Label = "Enabled until", Id = "webbackup-expirationdate", UiControl = "text" },
            new FieldMap() { FieldType = "Text", Label = "There are no devices at the site", Id = "webbackup-disabled", UiControl = "text" },
            new FieldMap() { FieldType = "Text", Label = "Web Backup Enabled", UiControl = "toggle",LocatorType="xpath",LocatorValue="//label[text()='Web Backup Enabled']"}
            };
            var additionalFields = Task.Run(async () => await AddAdditionalFieldMaps()).Result;
            fieldMaps.AddRange(additionalFields);
            return fieldMaps;
         }

        public override string GetDropdownSelectedValue(string control)
        {
            var elemId = FieldMaps().Find(f => f.Label == control).Id;
            var element = GetWebElementById(elemId);
            var selected = element.FindElement(By.XPath("./parent::span/parent::div/span[@class='selected']"));

            return selected.Text;
        }

        public override IWebElement GetOnPageElementByLabel(string label)
        {
            var matchingFieldMap = base.GetOnPageElementByLabel(label);

            if (matchingFieldMap != null)
            {
                return matchingFieldMap;
            }

            var formContainer = GetWebElementById("addSiteFormContainer");

            var attributeContainers = formContainer.FindElements(By.ClassName("dynamic-attribute"));

            foreach (var container in attributeContainers)
            {
                var controlLabels = container.FindElements(By.ClassName("control-label"));

                foreach (var controlLabel in controlLabels)
                {
                    if (controlLabel.Text.ToUpper() == label.ToUpper())
                    {
                        var matchingInputContainer = container.FindElement(By.ClassName("attribute-input"));

                        return matchingInputContainer;
                    }
                }
            }

            return null;
        }

        public async Task<List<FieldMap>> AddAdditionalFieldMaps()
        {
            var langauges = await _e2ERepository.GetAllLanguages();
            var fieldMaps = new List<FieldMap>();

            foreach (var language in langauges)
            {
                var map = new FieldMap() { FieldType = "Toggle", Label = language.CultureName, Id = language.CultureName, UiControl = "toggle" };

                fieldMaps.Add(map);
            }
            return fieldMaps;
        }

        public void VerifyEnabledUntilDate(string date)
        {
            var matchingFieldMap = FieldMaps().First(fm => fm.Label == "Enabled until");
            string expectedDate = "Enabled until " + ResolveDate(date, "dd-MMM-yyyy");
            var actualDate = GetWebElementById(matchingFieldMap.Id).Text;
            Assert.AreEqual(expectedDate, actualDate);
        }
    }
}