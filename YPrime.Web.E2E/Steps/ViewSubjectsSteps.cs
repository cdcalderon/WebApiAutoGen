using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class ViewSubjectsSteps
    {
        private readonly ScenarioService scenarioService;
        private readonly DashboardPage dashboardPage;
        private readonly SubjectPage subjectPage;
        private readonly E2ERepository e2eRepository;
        private readonly ScenarioContext scenarioContext;

        public ViewSubjectsSteps(
            ScenarioService scenarioService,
            DashboardPage dashboardPage,
            SubjectPage subjectPage,
            E2ERepository e2eRepository,
            ScenarioContext scenarioContext)
        {
            this.scenarioService = scenarioService;
            this.scenarioContext = scenarioContext;
            this.dashboardPage = dashboardPage;
            this.subjectPage = subjectPage;
            this.e2eRepository = e2eRepository;
        }

        [Given(@"I am on Dashboard page")]
        public void GivenIAmOnStudyPortalPage()
        {
            e2eRepository.AddBaseSoftwareRelease();
            dashboardPage.NavigateToPage();
            dashboardPage.ThreadSleep();
        }

        [Given(@"I click on the Subject menu item")]
        public void GivenIClickOnTheMenuItem()
        {
            dashboardPage.BtnSubjects.Click();
        }

        [Given(@"I select Subject ""(.*)""")]
        [When(@"I select Subject ""(.*)""")]
        [Then(@"I select Subject ""(.*)""")]
        public void GivenISelectSubject(string subjectNumber)
        {
            if (subjectNumber == "<SubjectId>")
            {
                subjectNumber = (string)this.scenarioContext["<SubjectId>"];
            }
            var element = subjectPage.GetSubjectNumberLink(subjectNumber);

            element.Click();
        }

        [Then(@"hamburger menu is displayed with the following functionality for Visibilty")]
        public void ThenHamburgerButtonIsDisplayedWithTheFollowingFunctionalityForVisibilty(Table table)
        {
            subjectPage.VerifyElementForVisibilty(table);
        }

        [Then(@"I click on background screen")]
        public void ThenIClickOnBackgroundScreen()
        {
            subjectPage.ClickSubjectPageBackground();
        }

        [Then(@"I click on Headers of ""([^""]*)"" to verify the sorting order")]
        public void ThenIClickOnHeadersOfToVerifyTheSortingOrder(string control, Table table)
        {
            subjectPage.VerifySortingOfElementsInGrid(control,table);
        }
        [Given(@"the following columns is displayed within subject index grid")]
        public void GivenTheFollowingColumnsIsDisplayedWithinSubjectIndexGrid(Table table)
        {
            subjectPage.verifyColumnsNameDisplayed(table);
        }

        [Given(@"the value with respect to given column name is displayed as")]
        public void GivenTheValueWithRespectToGivenColumnNameIsDisplayedAs(Table table)
        {
            subjectPage.verifyColumnValues(table);
        }


    }
}