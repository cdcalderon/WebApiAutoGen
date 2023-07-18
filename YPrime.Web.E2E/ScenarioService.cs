using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.IO;
using TechTalk.SpecFlow;

namespace YPrime.Web.E2E
{
    [Binding]
    public class ScenarioService
    {
        public ScenarioService() { }

        public ChromeDriver ChromeDriver { get; set; }

        private ChromeDriverService chromeDriverService { get; set; }

        public void InitializeChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            options.AddUserProfilePreference("download.default_directory", projectDirectory+"\\Downloads");
            chromeDriverService = ChromeDriverService.CreateDefaultService();
            ChromeDriver = new ChromeDriver(
                chromeDriverService, 
                options,
                TimeSpan.FromMinutes(4));
            ChromeDriver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(60));
            ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            ChromeDriver.Manage().Window.Maximize();
        }

        public void LogoutAndQuit()
        {
            ChromeDriver.Quit();
            ChromeDriver.Dispose();
            Process.GetProcessById(chromeDriverService.ProcessId).CloseMainWindow();
        }
    }
}

