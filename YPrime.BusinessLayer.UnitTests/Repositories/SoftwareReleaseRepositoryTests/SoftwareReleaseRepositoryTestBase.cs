using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessLayer.UnitTests.TestObjects;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    public abstract class SoftwareReleaseRepositoryTestBase : RepositoryTestBase
    {
        protected SoftwareReleaseRepository Repository;
        protected Mock<IStudyDbContext> Context;
        protected readonly Mock<ICountryService> CountryService;
        protected readonly Mock<ITranslationService> TranslationService;
        protected readonly Mock<IConfigurationVersionService> ConfigurationVersionService;

        protected Mock<IPatientVisitRepository> MockPatientVisitRepository;
        protected Mock<ISiteRepository> MockSiteRepository;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IPrimeInventoryAPIRepository> MockPrimeInventoryAPIRepository;
        protected Mock<ICountryService> MockCountryService;
        protected Mock<ISubjectInformationService> MockSubjectInformationService;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IVisitService> MockVisitService;
        protected Mock<ICareGiverTypeService> MockCareGiverTypeService;
        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<ISoftwareReleaseRepository> MockSoftwareReleaseRepository;
        protected Mock<IDeviceRepository> MockDeviceRepository;

        protected Site BaseSite;
        protected List<Site> BaseSites;
        protected List<CorrectionApprovalData> BaseCorrectionApprovalDatas;
        protected List<SubjectInformationModel> BaseSubjectInformationModels;
        protected List<Patient> BasePatients;
        protected List<VisitModel> BaseVisits;
        protected List<DiaryEntry> BaseDiaryEntires;
        protected List<PatientVisit> BasePatientVisits;
        protected List<SoftwareRelease> BaseSoftwareReleases;
        protected List<SoftwareReleaseCountry> BaseSoftwareReleaseCountries;
        protected List<SoftwareReleaseDeviceType> BaseSoftwareReleaseDeviceTypes;
        protected List<Device> BaseDevices;

        protected Guid BasePatientId = Guid.NewGuid();
        protected CountryModel UnitedStatesCountry;

        protected FakeDbSet<Patient> PatientDataset;
        protected FakeDbSet<PatientVisit> PatientVisitDataset;
        protected FakeDbSet<DiaryEntry> DiaryEntryDataset;
        protected FakeDbSet<SoftwareRelease> SoftwareReleaseDataset;
        protected FakeDbSet<SoftwareReleaseCountry> SoftwareReleaseCountryDataset;
        protected FakeDbSet<SoftwareReleaseDeviceType> SoftwareReleaseDeviceTypeDataset;
        protected FakeDbSet<Device> DeviceDataset;

        protected SoftwareReleaseRepositoryTestBase()
        {

            CountryService = new Mock<ICountryService>();
            TranslationService = new Mock<ITranslationService>();
            ConfigurationVersionService = new Mock<IConfigurationVersionService>();

            Context = new Mock<IStudyDbContext>();

            Repository = new SoftwareReleaseRepository(
                Context.Object, 
                CountryService.Object,
                TranslationService.Object,
                ConfigurationVersionService.Object);

            MockTranslationService = new Mock<ITranslationService>();
            MockPatientVisitRepository = new Mock<IPatientVisitRepository>();
            MockPrimeInventoryAPIRepository = new Mock<IPrimeInventoryAPIRepository>();
            MockSiteRepository = new Mock<ISiteRepository>();
            MockCountryService = new Mock<ICountryService>();
            MockSubjectInformationService = new Mock<ISubjectInformationService>();
            MockVisitService = new Mock<IVisitService>();
            MockCareGiverTypeService = new Mock<ICareGiverTypeService>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            MockDeviceRepository = new Mock<IDeviceRepository>();

            SetupBaseData();
        }

        protected virtual void SetupBaseData()
        {
            UnitedStatesCountry = new CountryModel
            {
                Id = Guid.NewGuid()
            };

            var allCountries = new List<CountryModel>
            {
                UnitedStatesCountry
            };

            BaseSite = new Site
            {
                Id = Guid.NewGuid(),
                SiteNumber = "100",
                TimeZone = "Eastern Standard Time",
                CountryId = UnitedStatesCountry.Id,
                IsActive = true
            };

            var baseSiteDto = new SiteDto
            {
                Id = BaseSite.Id,
                SiteNumber = BaseSite.SiteNumber,
                TimeZone = BaseSite.TimeZone,
                CountryId = UnitedStatesCountry.Id,
                IsActive = true
            };

            BaseSites = new List<Site>
            {
                BaseSite
            };

            MockSiteRepository

                .Setup(r => r.GetSite(It.Is<Guid>(id => id == BaseSite.Id)))
                .ReturnsAsync(baseSiteDto);


            MockSiteRepository
                .Setup(r => r.GetSiteLocalTime(It.Is<Guid>(id => id == BaseSite.Id)))
                .Returns(DateTime.UtcNow);

            var siteDataset = new FakeDbSet<Site>(BaseSites);
            siteDataset.Setup(d => d.Find(It.Is<Guid>(id => id == BaseSite.Id))).Returns(BaseSite);
            siteDataset.Setup(d => d.FindAsync(It.Is<Guid>(id => id == BaseSite.Id))).ReturnsAsync(BaseSite);
            Context.Setup(c => c.Sites).Returns(siteDataset.Object);

            BasePatients = new List<Patient>();
            PatientDataset = new FakeDbSet<Patient>(BasePatients);
            Context.Setup(c => c.Patients).Returns(PatientDataset.Object);

            BaseSoftwareReleases = new List<SoftwareRelease>();
            SoftwareReleaseDataset = new FakeDbSet<SoftwareRelease>(BaseSoftwareReleases);
            Context.Setup(c => c.SoftwareReleases).Returns(SoftwareReleaseDataset.Object);

            BaseSoftwareReleaseCountries = new List<SoftwareReleaseCountry>();
            SoftwareReleaseCountryDataset = new FakeDbSet<SoftwareReleaseCountry>(BaseSoftwareReleaseCountries);
            Context.Setup(c => c.SoftwareReleaseCountry).Returns(SoftwareReleaseCountryDataset.Object);

            BaseSoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>();
            SoftwareReleaseDeviceTypeDataset = new FakeDbSet<SoftwareReleaseDeviceType>(BaseSoftwareReleaseDeviceTypes);
            Context.Setup(c => c.SoftwareReleaseDeviceTypes).Returns(SoftwareReleaseDeviceTypeDataset.Object);

            BaseDevices = new List<Device>();
            DeviceDataset = new FakeDbSet<Device>(BaseDevices);
            Context.Setup(c => c.Devices).Returns(DeviceDataset.Object);

            BaseCorrectionApprovalDatas = new List<CorrectionApprovalData>();
            var correctionApprovalDataset = new FakeDbSet<CorrectionApprovalData>(BaseCorrectionApprovalDatas);
            Context.Setup(c => c.CorrectionApprovalDatas).Returns(correctionApprovalDataset.Object);

            MockTranslationService
                .Setup(r => r.GetByKey(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(string.Empty);

            BaseVisits = new List<VisitModel>();
            var baseVisit1 = new VisitModel()
            {
                Id = Guid.NewGuid(),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                Name = "Visit 1"
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.NewGuid(),
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 2,
                Name = "Visit 2"
            };

            BaseVisits.Add(baseVisit1);
            BaseVisits.Add(baseVisit2);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel>
                {
                    baseVisit1,
                    baseVisit2
                });

            BasePatientVisits = new List<PatientVisit>();
            PatientVisitDataset = new FakeDbSet<PatientVisit>(BasePatientVisits);
            Context.Setup(c => c.PatientVisits).Returns(PatientVisitDataset.Object);

            BaseDiaryEntires = new List<DiaryEntry>();
            DiaryEntryDataset = new FakeDbSet<DiaryEntry>(BaseDiaryEntires);
            Context.Setup(c => c.DiaryEntries).Returns(DiaryEntryDataset.Object);

            MockCountryService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(allCountries);

            YPrimeSession.CurrentUser = new StudyUserDto
            {
                Sites = new List<SiteDto>
                {
                    baseSiteDto
                },
                Roles = new List<Core.BusinessLayer.Models.StudyRoleModel>
                {
                    new Core.BusinessLayer.Models.StudyRoleModel
                    {
                        ShortName = "YP"
                    }
                }
            };

            SetupSession();
        }
    }
}