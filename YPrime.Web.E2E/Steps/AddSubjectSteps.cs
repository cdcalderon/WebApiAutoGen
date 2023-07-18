using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class AddSubjectSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly AddSubjectPage addSubjectPage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;

        public AddSubjectSteps(
           ScenarioService scenarioService,
           AddSubjectPage addSubjectPage,
           E2ERepository e2eRepository,
           ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.addSubjectPage = addSubjectPage;
            this.e2eRepository = e2eRepository;
            this.scenarioContext = scenarioContext;
        }

        public static string _subjectNumber;
        private readonly Random _random = new Random();        

        [Given(@"I enter ""(.*)"" in Subject field")]
        public void GivenIEnterInSubjectField(string subjectNumber)
        {
            if (subjectNumber == "<SubjectId>")
            {
                subjectNumber = RandomNumber(100, 999).ToString();                
            }
            this.addSubjectPage.EnterSubjectNumber(subjectNumber);
            _subjectNumber = "S-10001-" + subjectNumber;
            this.scenarioContext["<SubjectId>"] = _subjectNumber;
        }

        [Given(@"I close the subject welcome page")]
        public void GivenICloseTheSubjectWelcomePage()
        {
            this.addSubjectPage.GetWebElementById("mainclose").Click();
        }

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max) => _random.Next(min, max);
    }
}