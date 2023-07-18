using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class UserInformationPage : BasePage
    {
        private readonly E2ESettings e2eSettings;
        public UserInformationPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }
        public override string PageName => "User Information";

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Dashboard#";
        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        {
            new FieldMap() { FieldType = "Button", Label = "Privacy Policy", Id = "btnPrivacyPolicy", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Change Password", Id = "btnChangePassword", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Button", Label = "Support", Id = "btnSupport", UiControl = "button", WaitForElement = true },
            new FieldMap() { FieldType = "Button", Label = " Logout", Id = "btnLogout", UiControl = "button", WaitForElement = false },
            new FieldMap() { FieldType = "Text", Label = "servertime", Id = "servertime", UiControl = "text", WaitForElement = false }
        };
    
        //Element not added in the FieldMap, because we dont want to search it by the Label, but we want to know if whatever text the ID is retrieving is in valid format
        public IWebElement ServerTimeText => ScenarioService.ChromeDriver.FindElement(By.Id("servertime"));
        public IWebElement StudyVersionText => ScenarioService.ChromeDriver.FindElement(By.Id("applicationVersion"));

        public string GetDateAndTime()
        {
            return GetTextFromElement(ServerTimeText);   
        }

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }
    }
}
