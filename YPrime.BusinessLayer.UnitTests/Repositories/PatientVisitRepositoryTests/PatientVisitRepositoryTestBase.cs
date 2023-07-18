using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Query.Interfaces;
using YPrime.BusinessLayer.Query.Parameters;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.UnitTests.TestObjects;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientVisitRepositoryTests
{
    public abstract class PatientVisitRepositoryTestBase : RepositoryTestBase
    {
        protected PatientVisitRepository Repository;
        protected Mock<IStudyDbContext> MockContext;

        protected Mock<IConfirmationRepository> MockConfirmationRepository;
        protected Mock<IDiaryEntryRepository> MockDiaryEntryRepository;
        protected Mock<ISiteRepository> MockSiteRepository;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IWebBackupRepository> MockWebBackupRepository;
        protected Mock<IRuleService> MockRuleRepository;
        protected Mock<IVisitService> MockVisitService;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<IPatientStatusService> MockPatientStatusService;
        protected Mock<ISystemSettingRepository> MockSystemSettingRepository;
        protected Mock<IPatientVisitSummaryQueryHandler> MockPatientVisitSummaryQueryHandler;

        protected List<Site> BaseSites;
        protected List<StudySettingModel> BaseStudySetting;
        protected List<Patient> BasePatients;
        protected List<CareGiver> BaseCaregivers;
        protected List<VisitModel> BaseVisits;
        protected List<PatientVisit> BasePatientVisits;
        protected Guid BasePatientId = Guid.NewGuid();
        protected Guid BaseSiteId = Guid.NewGuid();
        protected FakeDbSet<Patient> PatientDataset;
        protected FakeDbSet<CareGiver> CaregiverDataset;
        protected FakeDbSet<PatientVisit> PatientVisitDataset;

        protected readonly DateTime DefaultAttributeMinimumDate = new DateTime(1980, 01, 01);
        protected readonly DateTime DefaultAttributeMaximumDate = new DateTime(2030, 12, 31);
        protected string TestQuestionnaireDisplayName = "Test Questionnaire ABC";

        protected PatientVisitRepositoryTestBase()
        {
            MockContext = new Mock<IStudyDbContext>();

            MockContext
                .Setup(c => c.ExecuteSqlToList<ProjectedVisitDto>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(new List<ProjectedVisitDto>());

            MockConfirmationRepository = new Mock<IConfirmationRepository>();
            MockTranslationService = new Mock<ITranslationService>();
            MockSiteRepository = new Mock<ISiteRepository>();
            MockRuleRepository = new Mock<IRuleService>();
            MockVisitService = new Mock<IVisitService>();
            MockDiaryEntryRepository = new Mock<IDiaryEntryRepository>();
            MockStudySettingService = new Mock<IStudySettingService>();
            MockWebBackupRepository = new Mock<IWebBackupRepository>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockPatientStatusService = new Mock<IPatientStatusService>();
            MockSystemSettingRepository = new Mock<ISystemSettingRepository>();
            MockPatientVisitSummaryQueryHandler = new Mock<IPatientVisitSummaryQueryHandler>();
            SetupBaseData();

            Repository = new PatientVisitRepository(
                MockContext.Object,
                MockWebBackupRepository.Object,
                MockTranslationService.Object,
                MockDiaryEntryRepository.Object,
                MockRuleRepository.Object,
                MockVisitService.Object,
                MockPatientStatusService.Object,
                MockQuestionnaireService.Object,
                MockStudySettingService.Object, 
                MockSiteRepository.Object, 
                MockSystemSettingRepository.Object,
                MockPatientVisitSummaryQueryHandler.Object);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MockContext
                .Setup(c => c.ExecuteSqlToList<ProjectedVisitDto>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(new List<ProjectedVisitDto>());
        }

        protected virtual void SetupBaseData()
        {
            var baseSite = new Site
            {
                Id = BaseSiteId,
                SiteNumber = "100",
                TimeZone = "Eastern Standard Time",
                WebBackupExpireDate = DateTime.Now.AddDays(3)
            };

            var baseSiteDto = new SiteDto
            {
                Id = baseSite.Id,
                SiteNumber = baseSite.SiteNumber,
                TimeZone = baseSite.TimeZone                
            };


            BaseSites = new List<Site>
            {
                baseSite
            };

            MockSiteRepository
                .Setup(r => r.GetSite(It.Is<Guid>(id => id == baseSite.Id)))
                .ReturnsAsync(baseSiteDto);

            MockSiteRepository
                .Setup(r => r.GetPatientSiteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(baseSite);

            MockQuestionnaireService
                .Setup(r => r.GetAll(null))
                .ReturnsAsync(new List<QuestionnaireModel>());

            var siteDataset = new FakeDbSet<Site>(BaseSites);
            siteDataset.Setup(d => d.Find(It.Is<Guid>(id => id == baseSite.Id))).Returns(baseSite);
            MockContext.Setup(c => c.Sites).Returns(siteDataset.Object);


            BasePatients = new List<Patient>();
            var basePatient = new Patient()
            {
                Id = BasePatientId,
                EnrolledDate = DateTime.Now.AddDays(-5),
            };

            BasePatients.Add(basePatient);
            PatientDataset = new FakeDbSet<Patient>(BasePatients);
            MockContext.Setup(c => c.Patients).Returns(PatientDataset.Object);


            BaseCaregivers = new List<CareGiver>();
            var baseCaregiver = new CareGiver()
            {
                Id = Guid.NewGuid(),
            };

            BaseCaregivers.Add(baseCaregiver);
            CaregiverDataset = new FakeDbSet<CareGiver>(BaseCaregivers);
            MockContext.Setup(c => c.CareGivers).Returns(CaregiverDataset.Object);

            BaseVisits = new List<VisitModel>();
            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-2222-123456745624"),
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 2
            };

            BaseVisits.Add(baseVisit1);
            BaseVisits.Add(baseVisit2);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(BaseVisits);

            BasePatientVisits = new List<PatientVisit>();
            PatientVisitDataset = new FakeDbSet<PatientVisit>(BasePatientVisits);
            MockContext.Setup(c => c.PatientVisits).Returns(PatientVisitDataset.Object);

            YPrimeSession.ConfigurationId = Guid.NewGuid();
            SetupSession();
        }

        protected void SetupPatients(List<Patient> patients)
        {
            BasePatients = new List<Patient>();
            BasePatients.AddRange(patients);
            PatientDataset = new FakeDbSet<Patient>(BasePatients);
            MockContext.Setup(c => c.Patients).Returns(PatientDataset.Object);
        }

        protected void SetupPatientVisits(List<PatientVisit> patientVisits)
        {
            BasePatientVisits = new List<PatientVisit>();
            BasePatientVisits.AddRange(patientVisits);
            PatientVisitDataset = new FakeDbSet<PatientVisit>(BasePatientVisits);
            MockContext.Setup(c => c.PatientVisits).Returns(PatientVisitDataset.Object);
        }

        protected void SetupQuestionnaires(List<QuestionnaireModel> questionnaireModels)
        {
            MockQuestionnaireService
                .Setup(r => r.GetAll(null))
                .ReturnsAsync(questionnaireModels);
        }

        protected void SetupStudyCustoms(int webbackuptabletEnabled = 1)
        {

            var studyCustoms = new List<StudyCustomModel>
            {
                new StudyCustomModel
                {
                    Key = "ActivateSubjectForms",
                    Value = "1"
                },
                new StudyCustomModel
                {
                    Key = "CaregiverPatientFormsEnabled",
                    Value = "1"
                },
                new StudyCustomModel
                {
                    Key = "IgnoreVisitOrder",
                    Value = "0"
                },
                new StudyCustomModel
                {
                    Key = "WebBackupTabletEnabled",
                    Value = webbackuptabletEnabled.ToString()
                }
            };

            MockStudySettingService.Setup(c => c.GetAllStudyCustoms(It.IsAny<Guid?>())).ReturnsAsync(studyCustoms);
        }

        protected void SetupDiaryEntries(Patient patient)
        {
            /* Diary setup */

            var DiaryEntry1 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 1, 1, 9, 30, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 1, 1, 9, 40, 0)),
            };

            var DiaryEntry2 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 2, 17, 13, 20, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 2, 17, 13, 40, 0)),
                PatientId = patient.Id
            };

            var DiaryEntry3 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 3, 4, 17, 06, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 3, 4, 17, 36, 0)),
            };

            MockDiaryEntryRepository.Setup(d => d.GetDiaryEntriesInflated(null, true, null, It.IsAny<Guid>()))
              .ReturnsAsync(new List<DiaryEntryDto>
                {
                    DiaryEntry1,
                    DiaryEntry2,
                    DiaryEntry3
                });

        }

        protected void SetupPatientVisitSummaryQueryHandlers(
            List<PatientVisitDto> patientVisitsData,            
            List<DiaryEntryDto> diaryEntryData,
            SiteDto siteData,
            List<CareGiver> careGiversData = null
        )
        {
            MockPatientVisitSummaryQueryHandler.Setup(m => m.ReadPatientVisitsForPatientVisitSummary(It.IsAny<PatientVisitSummaryQueryParameters>()))
               .ReturnsAsync((PatientVisitSummaryQueryParameters parameters) => patientVisitsData);

            MockPatientVisitSummaryQueryHandler.Setup(m => m.ReadDiaryEntriesForPatientVisitSummary(It.IsAny<PatientVisitSummaryQueryParameters>()))
                .ReturnsAsync((PatientVisitSummaryQueryParameters parameters) => diaryEntryData);
            MockPatientVisitSummaryQueryHandler.Setup(m => m.ReadSiteForPatientVisitSummary(It.IsAny<PatientVisitSummaryQueryParameters>()))
                .ReturnsAsync((PatientVisitSummaryQueryParameters parameters) => siteData);
            if ( careGiversData != null )
            {
                MockPatientVisitSummaryQueryHandler.Setup(m => m.ReadCareGiversForPatientVisitSummary(It.IsAny<PatientVisitSummaryQueryParameters>()))
                    .ReturnsAsync((PatientVisitSummaryQueryParameters parameters) => careGiversData);
            }
            
        }
    }
}
