using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SSOLoginPage : BasePage
    {
        private readonly E2ESettings e2eSettings;

        public override string PageName => string.Empty;

        public override string PageUrl => $"{e2eSettings.SSOUrl}";

        public override List<FieldMap> FieldMaps() => new List<FieldMap>();

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }
        public SSOLoginPage(E2ESettings e2eSettings, ScenarioService scenarioService) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
        }

        public IWebElement TxtEmail => ScenarioService.ChromeDriver.FindElement(By.Id("Email"));

        public IWebElement TxtPassword => ScenarioService.ChromeDriver.FindElement(By.Id("Password"));

        public IWebElement BtnLogin => ScenarioService.ChromeDriver.FindElement(By.Id("SubmitLogin"));

        public void Login(string email, string password)
        {
            TxtEmail.EnterText(email);
            TxtPassword.EnterText(password);
            BtnLogin.Submit();
        }
    }
}
