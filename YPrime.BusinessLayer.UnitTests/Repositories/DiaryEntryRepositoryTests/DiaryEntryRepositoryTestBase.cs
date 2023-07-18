using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DiaryEntryRepositoryTests
{
    [TestClass]
    public abstract class DiaryEntryRepositoryTestBase
    {
        protected Mock<IStudyDbContext> MockContext;

        protected Mock<IAnswerRepository> MockAnswerRepository;
        protected Mock<IAnswerRepository> MockTemperatureAnswerRepository;
        protected Mock<IDiaryPageRepository> MockDiaryPageRepository;
        protected Mock<ISiteRepository> MockSiteRepository;

        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<IConfigurationVersionService> MockConfigurationVersionService;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IVisitService> MockVisitService;
        protected Mock<ICountryService> MockCountryService;

        protected Patient TestPatient;
        protected VisitModel TestVisit;
        protected QuestionnaireModel TestQuestionnaire;
        protected AnswerDto TestAnswer;
        protected AnswerDto TestTempAnswer;
        protected DiaryEntry TestDiaryEntry;
        protected SiteDto TestSite;
        protected ConfigurationVersion TestConfigVersion;
        protected DiaryPageModel TestDiaryPage;
        protected QuestionModel TestQuestionModel;
        protected CountryModel TestCountry;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            SetupTestData();

            MockContext = new Mock<IStudyDbContext>();

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { TestPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            var diaryEntryDataset = new FakeDbSet<DiaryEntry>(new List<DiaryEntry> { TestDiaryEntry });
            MockContext.Setup(c => c.DiaryEntries).Returns(diaryEntryDataset.Object);

            MockDiaryPageRepository = new Mock<IDiaryPageRepository>();

            MockDiaryPageRepository
                .Setup(r => r.GetQuestionnaireDiaryPages(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<DiaryPageModel>
                {
                    TestDiaryPage
                });
                
            MockSiteRepository = new Mock<ISiteRepository>();
            MockCountryService = new Mock<ICountryService>();

            MockCountryService
                .Setup(r => r.Get(It.Is<Guid>(s => s == TestCountry.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(TestCountry);         

            MockCountryService
               .Setup(s => s.GetAll(It.IsAny<Guid?>()))
               .ReturnsAsync(new List<CountryModel>()
               {
                  TestCountry
               });

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == TestSite.Id)))
                .ReturnsAsync(TestSite);

            MockTranslationService = new Mock<ITranslationService>();

            MockAnswerRepository = new Mock<IAnswerRepository>();
            MockTemperatureAnswerRepository = new Mock<IAnswerRepository>();

            AnswerPropertiesDto answerProperties = new AnswerPropertiesDto()
            {
                DiaryEntryId = TestDiaryEntry.Id,
                UseMetricForAnswers = TestCountry.UseMetric
            };

            MockAnswerRepository
                .Setup(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)))
                .ReturnsAsync(new List<AnswerDto>
                {
                    TestAnswer
                });

            MockTemperatureAnswerRepository
                .Setup(r => r.GetAnswers(It.Is<AnswerPropertiesDto>(id => id.DiaryEntryId == TestDiaryEntry.Id)))
                .ReturnsAsync(new List<AnswerDto>
                {
                    TestTempAnswer
                });

            MockQuestionnaireService = new Mock<IQuestionnaireService>();

            MockQuestionnaireService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionnaireModel>
                {
                    TestQuestionnaire
                });

            MockQuestionnaireService
                .Setup(s => s.GetAllWithPages(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionnaireModel>
                {
                    TestQuestionnaire
                });

            MockQuestionnaireService
                .Setup(s => s.Get(It.Is<Guid>(id => id == TestQuestionnaire.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(TestQuestionnaire);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(
                    It.Is<Guid>(qid => qid == TestDiaryEntry.QuestionnaireId),
                    It.IsAny<Guid?>(),
                    It.Is<Guid>(cid => cid == TestConfigVersion.Id)))
                .ReturnsAsync(TestQuestionnaire);

            MockVisitService = new Mock<IVisitService>();

            MockVisitService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel>
                {
                    TestVisit
                });

            MockVisitService
                .Setup(s => s.Get(It.Is<Guid>(id => id == TestVisit.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(TestVisit);

            MockConfigurationVersionService = new Mock<IConfigurationVersionService>();

            MockConfigurationVersionService
                .Setup(s => s.Get(It.Is<Guid>(id => id == TestConfigVersion.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(TestConfigVersion);
        }

        protected virtual void SetupTestData()
        {
            TestConfigVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                ConfigurationVersionNumber = "18.0",
                SrdVersion = "1.2"
            };

            TestCountry = new CountryModel()
            {
                Id = Guid.NewGuid(),
                UseMetric = false
            };

            TestSite = new SiteDto
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                CountryId = TestCountry.Id
            };

            TestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                SiteId = TestSite.Id
            };

            TestVisit = new VisitModel
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            TestQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireTypeId = QuestionnaireType.PatientHandheld.Id,
                CanBlindedSeeAnswers = false
            };

            TestDiaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = TestQuestionnaire.Id,
                PatientId = TestPatient.Id,
                Patient = TestPatient,
                VisitId = TestVisit.Id,
                DiaryStatusId = DiaryStatus.Source.Id,
                DataSourceId = DataSource.eCOAApp.Id,
                ConfigurationId = TestConfigVersion.Id
            };

            TestQuestionModel = new QuestionModel
            {
                Id = Guid.NewGuid()
            };

            TestAnswer = new AnswerDto
            {
                Id = Guid.NewGuid(),
                DiaryEntryId = TestDiaryEntry.Id,
                QuestionId = TestQuestionModel.Id,
            };

            TestTempAnswer = new AnswerDto
            {
                Id = Guid.NewGuid(),
                DiaryEntryId = TestDiaryEntry.Id,
                QuestionId = TestQuestionModel.Id,
                Question = new QuestionModel()
                {
                    QuestionType = InputFieldType.TemperatureSpinner.Id
                },
                DisplayAnswer = "20",
                Suffix = Temperature.DegreesFahrenheit
            };

            TestDiaryPage = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 2,
                Questions = new List<QuestionModel>
                {
                    TestQuestionModel
                }
            };

         
        }

        protected IDiaryEntryRepository GetRepository(bool includeTemperatureAnswers = false)
        {
            var repository = new DiaryEntryRepository(
                MockContext.Object,
                MockTranslationService.Object,
                includeTemperatureAnswers ? MockTemperatureAnswerRepository.Object :MockAnswerRepository.Object,
                MockDiaryPageRepository.Object,
                MockSiteRepository.Object,
                MockVisitService.Object,
                MockQuestionnaireService.Object,
                new Mock<IFileService>().Object,
                MockConfigurationVersionService.Object,
                MockCountryService.Object);

            return repository;
        }
    }
}
