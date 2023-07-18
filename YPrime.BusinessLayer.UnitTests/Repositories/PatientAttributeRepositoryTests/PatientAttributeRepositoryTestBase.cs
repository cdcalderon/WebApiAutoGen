using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using System.Linq;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientAttributeRepositoryTests
{
    [TestClass]
    public abstract class PatientAttributeRepositoryTestBase
    {
        protected const string SubjectNumberTranslationKey = "SubjectNumber";
        protected const string SubjectNumberTranslation = "Subject Number";

        private const string StatusTranslationKey = "lblPatientCurrentStatus";
        private const string StatusTranslation = "Current Status";

        protected List<PatientAttribute> BasePatientAttributes;
        protected List<SubjectInformationModel> BaseSubjectInformationModels;

        protected Mock<IStudyDbContext> MockContext;

        protected Mock<IPatientRepository> MockPatientRepository;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<ISubjectInformationService> MockSubjectInformationService;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IPatientStatusService> MockPatientStatusService;
        protected Mock<ISiteRepository> MockSiteRepository;

        protected PatientAttributeRepository Repository;

        protected Patient TestPatient;
        protected PatientDto TestPatientDto;
        protected Site TestSite;

        protected Guid UnitedStatesCountryId;
        protected LanguageModel English;
        protected LanguageModel Japanese;

        protected PatientAttributeRepositoryTestBase()
        {
            MockContext = new Mock<IStudyDbContext>();
            MockPatientRepository = new Mock<IPatientRepository>();

            MockSubjectInformationService = new Mock<ISubjectInformationService>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockStudySettingService = new Mock<IStudySettingService>();
            MockTranslationService = new Mock<ITranslationService>();
            MockPatientStatusService = new Mock<IPatientStatusService>();
            MockSiteRepository = new Mock<ISiteRepository>();

            SetupBaseData();

            Repository = new PatientAttributeRepository(
                MockContext.Object,
                MockPatientRepository.Object,
                MockSubjectInformationService.Object,
                MockQuestionnaireService.Object,
                MockPatientStatusService.Object,
                MockStudySettingService.Object,
                MockTranslationService.Object,
                MockSiteRepository.Object);
        }

        protected void SetStudySettingPatientLength(int length, bool setSeperator = true)
        {
            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(s => s == "PatientNumberLength"), It.IsAny<Guid?>()))
                .ReturnsAsync(length.ToString());

            var seperatorReturn = setSeperator
                ? "-"
                : null;

            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(s => s == "PatientNumberSiteSubjectNumberSeparator"), It.IsAny<Guid?>()))
                .ReturnsAsync(seperatorReturn);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MockStudySettingService.Reset();
        }

        protected virtual void SetupBaseData()
        {
            UnitedStatesCountryId = Guid.NewGuid();

            English = new LanguageModel
            {
                Id = Config.Defaults.Languages.English.Id,
                DisplayName = Config.Defaults.Languages.English.DisplayName,
                CultureName = Config.Defaults.Languages.English.CultureName
            };

            Japanese = new LanguageModel
            {
                Id = Guid.NewGuid(),
                DisplayName = "Japanese (Japan)",
                CultureName = "ja-JP"
            };

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    Name = "Enrolled"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Removed"
                }
            };

            TestSite = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = UnitedStatesCountryId
            };

            TestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-102-105678",
                PatientStatusTypeId = statusTypes.First().Id,
                SiteId = TestSite.Id,
                Site = TestSite,
                LanguageId = English.Id
            };

            TestPatientDto = new PatientDto
            {
                Id = TestPatient.Id,
                PatientNumber = TestPatient.PatientNumber,
                PatientStatusTypeId = TestPatient.PatientStatusTypeId,
                PatientStatusType = TestPatient.GetPatientStatusType(statusTypes),
                LanguageId = English.Id,
                SiteId = TestSite.Id
            };

            MockPatientRepository
                .Setup(r => r.GetPatient(It.Is<Guid>(g => g == TestPatient.Id), It.IsAny<string>()))
                .ReturnsAsync(TestPatientDto);

            MockPatientRepository
                .Setup(r => r.GetPatient(It.Is<Patient>(p => p == TestPatient), It.IsAny<string>()))
                .ReturnsAsync(TestPatientDto);

            var configDetailId = Guid.NewGuid();

            var basePatientAttribute = new PatientAttribute
            {
                Id = Guid.NewGuid(),
                PatientId = TestPatient.Id,
                PatientAttributeConfigurationDetailId = configDetailId,
                AttributeValue = "Test Value"
            };

            BasePatientAttributes = new List<PatientAttribute>
            {
                basePatientAttribute
            };

            var baseSubjectInformationModel = new SubjectInformationModel
            {
                Id = configDetailId,
                ChoiceType = DataType.TextAttribute.DisplayName,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = TestSite.CountryId
                    }
                }
            };

            BaseSubjectInformationModels = new List<SubjectInformationModel>
            {
                baseSubjectInformationModel
            };

            var attributeDataset = new FakeDbSet<PatientAttribute>(BasePatientAttributes);
            MockContext.Setup(c => c.PatientAttributes).Returns(attributeDataset.Object);

            var patientDataset = new FakeDbSet<Patient>(new List<Patient> { TestPatient });
            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            var siteDataset = new FakeDbSet<Site>(new List<Site> { TestSite });
            MockContext.Setup(c => c.Sites).Returns(siteDataset.Object);

            var siteLanguageDataset = new FakeDbSet<SiteLanguage>(new List<SiteLanguage>
            {
                new SiteLanguage 
                { 
                    Id = Guid.NewGuid(),
                    SiteId = TestSite.Id,
                    LanguageId = English.Id
                },
                new SiteLanguage
                {
                    Id = Guid.NewGuid(),
                    SiteId = TestSite.Id,
                    LanguageId = Japanese.Id
                },
            });
            MockContext.Setup(c => c.SiteLanguages).Returns(siteLanguageDataset.Object);

            MockSubjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(BaseSubjectInformationModels);

            MockTranslationService
                .Setup(s => s.GetByKey(It.Is<string>(key => key == SubjectNumberTranslationKey), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(SubjectNumberTranslation);

            MockTranslationService
                .Setup(s => s.GetByKey(It.Is<string>(key => key == StatusTranslationKey), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(StatusTranslation);

            var basePatientStatuses = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Removed",
                    IsRemoved = true,
                    IsActive = false
                },
                new PatientStatusModel
                {
                    Id = 4,
                    Name = "Discontinued",
                    IsRemoved = false,
                    IsActive = false
                }
            };

            MockPatientStatusService
                .Setup(s => s.GetAll(null))
                .ReturnsAsync(basePatientStatuses);

            var baseLanguages = new List<LanguageModel>
            {
                English,
                Japanese
            };

            MockSiteRepository
                .Setup(s => s.GetLanguagesForSite(
                    It.Is<Guid>(sid => sid == TestSite.Id),
                    It.IsAny<Guid?>()))
                .ReturnsAsync(baseLanguages);
        }
    }
}