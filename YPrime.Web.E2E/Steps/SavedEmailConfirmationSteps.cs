using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;
namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class SavedEmailConfirmationSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;
        private const string BCCId = "026ef731-aac2-4133-a134-ebda342b3339";

        public SavedEmailConfirmationSteps(
           ScenarioService scenarioService,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
        }

        [Then(@"Emails ""([^""]*)"" will be sent to recipients utilizing the BCC method")]
        public void ThenEmailsWillBeSentToRecipientsUtilizingTheBCCMethod(String subject)
        {
            string expectedbccID = e2eRepository.GetBCCidEmailRecipient(subject);

            Assert.IsTrue(expectedbccID.Equals(BCCId));
        }

        [Then(@"Recipients label has value set from ""([^""]*)"" table")]
        public void ThenRecipientsLabelHasValueSetFromTable(string emailRecipients)
        {
            var element = scenarioService.ChromeDriver.FindElement(By.XPath("//dd[@id='recipientsValue']"));
            string allRecipients = element.GetAttribute("innerText");
            string[] expectedRecipients = allRecipients.Replace(" ", "").Split(",");
            List<string> actualListRecipients = e2eRepository.GetEmailIdFromEmailRecipients();
            string[] actualRecipients = actualListRecipients.ToArray();
            bool isEqual = Enumerable.SequenceEqual(expectedRecipients.OrderBy(t => t), actualRecipients.OrderBy(t => t));
            Assert.IsTrue(isEqual);
        }

        [Then(@"""([^""]*)"" has ""([^""]*)"" count in ""([^""]*)"" table")]
        public void ThenHasCountInTable(string subject, string value, string emailSent)
        {
            var subjectCount = e2eRepository.GetSubjectCountInEmailSent(subject);
            Assert.AreEqual(int.Parse(value), subjectCount);
        }

    }
}