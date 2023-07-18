using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientAttributeRepositoryTests
{
    [TestClass]
    public class PatientAttributeRepositoryGetPatientAttributesTests : LegacyTestBase
    {
        private Mock<IStudyDbContext> _context;
        private Mock<IPatientRepository> _patientRepository;
        private Mock<ISubjectInformationService> _subjectInformationService;
        private Mock<IQuestionnaireService> _questionnaireService;
        private Mock<IStudySettingService> _studySettingService;
        private Mock<ITranslationService> _translationService;
        private Mock<IPatientStatusService> _patientStatusService;
        private Mock<ISiteRepository> _siteRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new Mock<IStudyDbContext>();
            _patientRepository = new Mock<IPatientRepository>();
            _subjectInformationService = new Mock<ISubjectInformationService>();
            _questionnaireService = new Mock<IQuestionnaireService>();
            _studySettingService = new Mock<IStudySettingService>();
            _translationService = new Mock<ITranslationService>();
            _patientStatusService = new Mock<IPatientStatusService>();
            _siteRepository = new Mock<ISiteRepository>();
        }

        private IPatientAttributeRepository GetRepository()
        {
            var repo = new PatientAttributeRepository(
                _context.Object,
                _patientRepository.Object,
                _subjectInformationService.Object,
                _questionnaireService.Object,
                _patientStatusService.Object,
                _studySettingService.Object,
                _translationService.Object,
                _siteRepository.Object);

            return repo;
        }
        
        [TestMethod]
        public async Task GetPatientAttributes()
        {
            var patient1Id = Guid.NewGuid();
            var patient2Id = Guid.NewGuid();

            var site = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            var patient = new Patient
            {
                Id = patient1Id,
                Site = site,
                SiteId = site.Id
            };

            var expectedAttribute = new PatientAttribute
            {
                PatientId = patient1Id,
                AttributeValue = RandomString(),
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var secondAttribute = new PatientAttribute
            {
                PatientId = patient2Id,
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var patientAttributePool = new List<PatientAttribute>
            {
                expectedAttribute,
                secondAttribute
            };

            var expectedSubjectInfoModel = new SubjectInformationModel
            {
                Id = expectedAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            var secondSubjectInfoModel = new SubjectInformationModel
            {
                Id = secondAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            _context.Setup(x => x.PatientAttributes).Returns(CreateDbSetMock(patientAttributePool).Object);
            _context.Setup(x => x.Patients).Returns(CreateDbSetMock(new List<Patient> { patient }).Object);
            _context.Setup(x => x.Sites).Returns(CreateDbSetMock(new List<Site> { site }).Object);

            _subjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    expectedSubjectInfoModel,
                    secondSubjectInfoModel
                });

            var sut = GetRepository();
            var results = await sut.GetPatientAttributes(patient1Id, "en-US");

            Assert.IsTrue(results.All(x => x is PatientAttributeDto));
            Assert.IsTrue(results.Count() == 2);
            var result = results.First();
            Assert.AreEqual(expectedAttribute.Id, result.Id);
            Assert.AreEqual(expectedAttribute.AttributeValue, result.AttributeValue);
            Assert.AreEqual(expectedAttribute.PatientId, result.PatientId);
        }

        [TestMethod]
        public async Task GetPatientAttributesSkipCountryTest()
        {
            var patient1Id = Guid.NewGuid();
            var patient2Id = Guid.NewGuid();

            var site = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            var patient = new Patient
            {
                Id = patient1Id,
                Site = site,
                SiteId = site.Id
            };

            var expectedAttribute = new PatientAttribute
            {
                PatientId = patient1Id,
                AttributeValue = RandomString(),
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var secondAttribute = new PatientAttribute
            {
                PatientId = patient2Id,
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var patientAttributePool = new List<PatientAttribute>
            {
                expectedAttribute,
                secondAttribute
            };

            var expectedSubjectInfoModel = new SubjectInformationModel
            {
                Id = expectedAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            var secondSubjectInfoModel = new SubjectInformationModel
            {
                Id = secondAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            var differentCountrySubjectInfoModel = new SubjectInformationModel
            {
                Id = secondAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            _context.Setup(x => x.PatientAttributes).Returns(CreateDbSetMock(patientAttributePool).Object);
            _context.Setup(x => x.Patients).Returns(CreateDbSetMock(new List<Patient> { patient }).Object);
            _context.Setup(x => x.Sites).Returns(CreateDbSetMock(new List<Site> { site }).Object);

            _subjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    expectedSubjectInfoModel,
                    secondSubjectInfoModel,
                    differentCountrySubjectInfoModel
                });

            var sut = GetRepository();

            var results = await sut.GetPatientAttributes(patient1Id, "en-US");

            Assert.IsTrue(results.All(x => x is PatientAttributeDto));
            Assert.IsTrue(results.Count() == 2);
            var result = results.First();
            Assert.AreEqual(expectedAttribute.Id, result.Id);
            Assert.AreEqual(expectedAttribute.AttributeValue, result.AttributeValue);
            Assert.AreEqual(expectedAttribute.PatientId, result.PatientId);
        }

        [TestMethod]
        public async Task GetPatientAttributeWithNewConfigData()
        {
            var patient1Id = Guid.NewGuid();
            var patient2Id = Guid.NewGuid();

            var site = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            var patient = new Patient
            {
                Id = patient1Id,
                Site = site,
                SiteId = site.Id
            };

            var expectedAttribute = new PatientAttribute
            {
                PatientId = patient1Id,
                AttributeValue = RandomString(),
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var secondAttribute = new PatientAttribute
            {
                PatientId = patient2Id,
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var patientAttributePool = new List<PatientAttribute>
            {
                expectedAttribute,
                secondAttribute
            };

            var expectedSubjectInfoModel = new SubjectInformationModel
            {
                Id = expectedAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            var secondSubjectInfoModel = new SubjectInformationModel
            {
                Id = secondAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            var newConfigSubjectInfoModel = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Countries = new List<CountryBaseModel>
                {
                    new CountryBaseModel
                    {
                        Id = site.CountryId
                    }
                }
            };

            _context.Setup(x => x.PatientAttributes).Returns(CreateDbSetMock(patientAttributePool).Object);
            _context.Setup(x => x.Patients).Returns(CreateDbSetMock(new List<Patient> { patient }).Object);
            _context.Setup(x => x.Sites).Returns(CreateDbSetMock(new List<Site> { site }).Object);

            _subjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    expectedSubjectInfoModel,
                    secondSubjectInfoModel, 
                    newConfigSubjectInfoModel
                });

            var sut = GetRepository();

            var results = await sut.GetPatientAttributes(patient1Id, "en-US");
            var resultList = results.ToList();

            Assert.IsTrue(results.All(x => x is PatientAttributeDto));
            Assert.IsTrue(results.Count() == 3);
            Assert.IsFalse(resultList[0].NewAttributeData);
            Assert.IsTrue(resultList[1].NewAttributeData);
            Assert.IsTrue(resultList[2].NewAttributeData);
            Assert.IsFalse(string.IsNullOrEmpty(resultList[0].AttributeValue));
            Assert.IsTrue(string.IsNullOrEmpty(resultList[1].AttributeValue));
            Assert.IsTrue(string.IsNullOrEmpty(resultList[2].AttributeValue));
        }

        [TestMethod]
        public async Task GetPatientAttributeDisplayValueWithChoiceData()
        {
            var patient1Id = Guid.NewGuid();

            var site = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };

            var patient = new Patient
            {
                Id = patient1Id,
                Site = site,
                SiteId = site.Id
            };

            var genderAttribute = new PatientAttribute
            {
                PatientId = patient1Id,
                AttributeValue = "Male",
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = Guid.NewGuid(),
            };

            var selectedColorValueId = Guid.NewGuid();

            var colorAttribute = new PatientAttribute
            {
                PatientId = patient1Id,
                Id = Guid.NewGuid(),
                AttributeValue = selectedColorValueId.ToString(),
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            var patientAttributePool = new List<PatientAttribute>
            {
                genderAttribute,
                colorAttribute
            };

            var countryModel = new CountryBaseModel
            {
                Id = site.CountryId
            };

            var genderInfoModel = new SubjectInformationModel
            {
                Id = genderAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    countryModel
                },
                Choices = new List<ChoiceModel>
                {
                    new ChoiceModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Male"
                    },
                    new ChoiceModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Female"
                    }
                }
            };

            var favoriteColorInfoModel = new SubjectInformationModel
            {
                Id = colorAttribute.PatientAttributeConfigurationDetailId,
                Countries = new List<CountryBaseModel>
                {
                    countryModel
                },
                Choices = new List<ChoiceModel>
                {
                    new ChoiceModel
                    {
                        Id = selectedColorValueId,
                        Name = "Blue"
                    },
                    new ChoiceModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Red"
                    }
                }
            };

            _context.Setup(x => x.PatientAttributes).Returns(CreateDbSetMock(patientAttributePool).Object);
            _context.Setup(x => x.Patients).Returns(CreateDbSetMock(new List<Patient> { patient }).Object);
            _context.Setup(x => x.Sites).Returns(CreateDbSetMock(new List<Site> { site }).Object);

            _subjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    genderInfoModel,
                    favoriteColorInfoModel
                });

            var sut = GetRepository();

            var results = await sut.GetPatientAttributes(patient1Id, "en-US");

            Assert.IsTrue(results.Count() == 2);

            var returnedGenderAttribute = results.First(r => r.Id == genderAttribute.Id);
            var returnedColorAttribute = results.First(r => r.Id == colorAttribute.Id);

            Assert.AreEqual("Male", returnedGenderAttribute.DisplayValue);
            Assert.AreEqual("Blue", returnedColorAttribute.DisplayValue);
        }
    }
}