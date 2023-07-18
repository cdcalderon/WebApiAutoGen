using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.BusinessLayer.Constants;
using System.Data.Entity;
using System.Linq;
using YPrime.BusinessRule.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public abstract class CorrectionRepositoryTestBase
    {
        protected const string DefaultCultureCode = "en-US";
        protected CorrectionRepository Repository;
        protected PatientAttributeDto FirstAttribute;
        protected Mock<IStudyDbContext> MockContext;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IPatientAttributeRepository> MockPatientAttributeRepository;
        protected Mock<IPatientRepository> MockPatientRepository;
        protected Mock<ISiteRepository> MockSiteRepository;
        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<ICorrectionTypeService> MockCorrectionTypeService;
        protected Mock<ICorrectionWorkflowService> MockCorrectionWorkflowService;
        protected Mock<IApproverGroupService> MockApproverGroupService;
        protected Mock<ISessionService> MockSessionService;
        protected Mock<IPatientStatusService> MockPatientStatusService;
        protected Mock<ICountryService> MockCountryService;

        protected Mock<INotificationRequestRepository> MockNotificationRequestRepository;

        protected PatientAttributeDto SecondAttribute;
        protected PatientAttributeDto ThirdAttribute;
        protected List<PatientAttributeDto> BaseAttributes;

        protected CountryModel TestCountry;
        protected Site TestSite;
        protected Patient TestPatient;

        protected CorrectionRepositoryTestBase()
        {
            MockContext = new Mock<IStudyDbContext>();
            MockTranslationService = new Mock<ITranslationService>();
            MockPatientAttributeRepository = new Mock<IPatientAttributeRepository>();
            MockPatientRepository = new Mock<IPatientRepository>();
            MockSiteRepository = new Mock<ISiteRepository>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockCorrectionTypeService = new Mock<ICorrectionTypeService>();
            MockCorrectionWorkflowService = new Mock<ICorrectionWorkflowService>();
            MockApproverGroupService = new Mock<IApproverGroupService>();
            MockSessionService = new Mock<ISessionService>();
            MockPatientStatusService = new Mock<IPatientStatusService>();
            MockCountryService = new Mock<ICountryService>();

            TestCountry = new CountryModel()
            {
                Id = Guid.NewGuid(),
                UseMetric = false
            };

            TestSite = new Site
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                CountryId = TestCountry.Id
            };

            TestPatient = new Patient
            {
                Id = Guid.NewGuid()
            };          

            MockCountryService
                .Setup(r => r.Get(It.Is<Guid>(s => s == TestCountry.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(TestCountry);

            MockSiteRepository
                .Setup(r => r.GetSiteEntity(It.Is<Guid>(id => id == TestSite.Id)))
                .Returns(TestSite);

            Repository = new CorrectionRepository(
                MockContext.Object,
                MockTranslationService.Object,
                MockPatientRepository.Object,
                MockPatientAttributeRepository.Object,
                MockSiteRepository.Object,
                MockQuestionnaireService.Object,
                MockPatientStatusService.Object,
                MockCorrectionTypeService.Object,
                MockCorrectionWorkflowService.Object,
                MockApproverGroupService.Object,
                MockSessionService.Object,
                MockCountryService.Object);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MockContext.Reset();
            MockQuestionnaireService.Reset();

            SetupBaseData();
            SetupTranslations();
            SetupPatientAttributeRepository();
            SetupPatientStatusService();
            SetUpBaseContext();

            MockPatientRepository.Reset();
        }

        private void SetUpBaseContext()
        {
            var patients = new List<Patient>
            {
                TestPatient
            };

            var dbPatienSet = new FakeDbSet<Patient>(patients);

            MockContext.Setup(x => x.Patients).Returns(dbPatienSet.Object);
        }

        private void SetupPatientStatusService()
        {
            var patientStatusList = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    IsActive = true,
                    IsRemoved = false,
                    Name = "Test Status1"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    IsActive = false,
                    IsRemoved = false,
                    Name = "Test Status2"
                },
                new PatientStatusModel
                {
                    Id = 3,
                    IsActive = false,
                    IsRemoved = true,
                    Name = "Test Status3"
                }
            };

            MockPatientStatusService.Setup(p => p.GetAll(It.IsAny<Guid?>())).ReturnsAsync(patientStatusList);
        }

        protected void SetPatientNumberValidationResult(bool result)
        {
            MockPatientRepository
                .Setup(r => r.ValidatePatientNumber(It.IsAny<string>(), It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(result);
        }

        protected void SetupPatientAttributeRepository()
        {
            MockPatientAttributeRepository.Reset();

            MockPatientAttributeRepository
                .Setup(r => r.GetPatientAttributes(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(BaseAttributes);
        }

        protected void SetupTranslations()
        {
            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(t => t == TranslationConstants.InvalidNumericValueErrorSuffix), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync("Contains an invalid numeric value");

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(t => t == TranslationConstants.InvalidNumericLengthErrorSuffix), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync("field needs to be {{numberLength}}-digits long. If needed, please use preceeding zeros to make your selection");
        }

        protected virtual void SetupBaseData()
        {
            FirstAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid()
            };

            SecondAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid()
            };

            ThirdAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid()
            };

            BaseAttributes = new List<PatientAttributeDto>
            {
                FirstAttribute,
                SecondAttribute,
                ThirdAttribute
            };
        }
    }
}