using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class EnrollmentInformationSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly ScenarioContext scenarioContext;
        private readonly EditSubjectPage editSubjectPage;
        private readonly CommonSteps commonSteps;

        public EnrollmentInformationSteps
        (
          ScenarioService scenarioService,
          ScenarioContext scenarioContext,
           EditSubjectPage editSubjectPage,
           CommonSteps commonSteps

        )
        {
            this.scenarioService = scenarioService;
            this.scenarioContext = scenarioContext;
            this.editSubjectPage = editSubjectPage;
            this.commonSteps= commonSteps;

        }

        [Given(@"""([^""]*)"" value is same to the id present at the time of creation")]
        [When(@"""([^""]*)"" value is same to the id present at the time of creation")]
        [Then(@"""([^""]*)"" value is same to the id present at the time of creation")]
        public void ThenValueIsSameToTheIdPresentAtTheTimeOfCreation(string key)
        {
            string expectedEnrollmentId = commonSteps.ThenValueIsNotEmpty(key);
            var element = scenarioService.ChromeDriver.FindElement(By.XPath("//span[contains(text(),'" + key + "')]//following::span[1]"));
            string actualEnrollmentId = element.GetAttribute("innerText");
            Assert.AreEqual(expectedEnrollmentId, actualEnrollmentId);
        }

        [Given(@"""([^""]*)"" button is not enabled")]
        [When(@"""([^""]*)"" button is not enabled")]
        [Then(@"""([^""]*)"" button is not enabled")]
        public void ThenValueIsNotEnabled(string key)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.LinkText(key));
            var value = bool.Parse(element.GetAttribute("disabled"));
            Assert.IsTrue(value);

        }

    }
}

