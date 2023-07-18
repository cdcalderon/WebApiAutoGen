using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Extensions;
using YPrime.Web.E2E.Pages;
using TechTalk.SpecFlow.Assist;
namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DeviceEmulationSteps
    {
        private readonly ScenarioService _scenarioService;
        private readonly ScenarioContext _scenarioContext;
        private readonly DeviceEmulationPage _deviceEmulationPage;
        private readonly E2ERepository e2eRepository;
        private readonly EditSubjectSteps _editSubjectSteps;
        public DeviceEmulationSteps(ScenarioContext scenarioContext, DeviceEmulationPage deviceEmulationPage, ScenarioService scenarioService, E2ERepository e2eRepository, EditSubjectSteps editSubjectSteps)
        {
            _scenarioContext = scenarioContext;
            _deviceEmulationPage = deviceEmulationPage;
            _scenarioService = scenarioService;
            this.e2eRepository = e2eRepository;
            _editSubjectSteps = editSubjectSteps;
        }

        [StepDefinition(@"I navigate to Device emulation page")]
        public void NavigateToDeviceEmulationPage()
        {
            var currentUrl = _scenarioService.ChromeDriver.Url;
            var elementUrlPartialText = currentUrl.ToString().Split('=')[1];
            string tokenValue = elementUrlPartialText.ToString();
            var expectedUrl = $"{_deviceEmulationPage.PageUrl}?token={tokenValue}";
            Assert.AreEqual(currentUrl, expectedUrl);
        }

    }
}
