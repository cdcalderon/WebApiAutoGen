namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Moq;

    using YPrime.BusinessLayer.Interfaces;
    using YPrime.BusinessLayer.Repositories;
    using YPrime.BusinessLayer.UnitTests.TestObjects;
    using YPrime.Config.Enums;
    using YPrime.Core.BusinessLayer.Interfaces;
    using YPrime.Core.BusinessLayer.Models;
    using YPrime.Data.Study;
    using YPrime.Data.Study.Models;
    using YPrime.Data.Study.Models.Models;
    using YPrime.Data.Study.Proxies;
    using YPrime.eCOA.DTOLibrary;

    public abstract class PatientRepositoryTestBase : RepositoryTestBase
    {
        protected const string DefaultCultureCode = "en-US";

        protected PatientRepository Repository;
        protected Mock<IStudyDbContext> MockContext;
        protected Mock<IDbContextTransactionProxy> MockTransaction;

        protected Mock<IPatientVisitRepository> MockPatientVisitRepository;
        protected Mock<ISiteRepository> MockSiteRepository;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<IPrimeInventoryAPIRepository> MockPrimeInventoryAPIRepository;
        protected Mock<ICountryService> MockCountryService;
        protected Mock<ISubjectInformationService> MockSubjectInformationService;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IVisitService> MockVisitService;
        protected Mock<IPatientStatusService> MockPatientStatusService;
        protected Mock<ICareGiverTypeService> MockCareGiverTypeService;
        protected Mock<IQuestionnaireService> MockQuestionnaireService;
        protected Mock<ISoftwareReleaseRepository> MockSoftwareReleaseRepository;
        protected Mock<IDeviceRepository> MockDeviceRepository;
        protected Mock<INotificationRequestRepository> MockNotificationRequestRepositoy;
        protected Mock<IAuthService> MockAuthService;

        protected Site BaseSite;
        protected List<Site> BaseSites;
        protected List<CorrectionApprovalData> BaseCorrectionApprovalDatas;
        protected List<SubjectInformationModel> BaseSubjectInformationModels;
        protected List<Patient> BasePatients;
        protected List<PatientStatusModel> BasePatientStatuses;
        protected List<VisitModel> BaseVisits;
        protected List<DiaryEntry> BaseDiaryEntries;
        protected List<PatientVisit> BasePatientVisits;
        protected List<SoftwareRelease> BaseSoftwareReleases;
        protected List<SoftwareReleaseCountry> BaseSoftwareReleaseCountries;
        protected List<SoftwareReleaseDeviceType> BaseSoftwareReleaseDeviceTypes;
        protected List<Device> BaseDevices;
        protected List<CareGiver> BaseCareGivers;

        protected Guid BasePatientId = Guid.NewGuid();
        protected Guid BaseCareGiverTypeId = Guid.NewGuid();
        protected CountryModel UnitedStatesCountry;

        protected FakeDbSet<Patient> PatientDataset;
        protected FakeDbSet<PatientVisit> PatientVisitDataset;
        protected FakeDbSet<DiaryEntry> DiaryEntryDataset;
        protected FakeDbSet<SoftwareRelease> SoftwareReleaseDataset;
        protected FakeDbSet<SoftwareReleaseCountry> SoftwareReleaseCountryDataset;
        protected FakeDbSet<SoftwareReleaseDeviceType> SoftwareReleaseDeviceTypeDataset;
        protected FakeDbSet<Device> DeviceDataset;
        protected FakeDbSet<CareGiver> CareGiverDataset;

        protected SubjectInformationModel LettersOnlyConfigDetail;
        protected SubjectInformationModel DateFormatConfigDetail;
        protected SubjectInformationModel NumberAttributeConfigDetail;

        protected readonly DateTime DefaultAttributeMinimumDate = new DateTime(1980, 01, 01);
        protected readonly DateTime DefaultAttributeMaximumDate = new DateTime(2030, 12, 31);

        protected PatientRepositoryTestBase()
        {
            MockContext = new Mock<IStudyDbContext>();

            MockContext
                .Setup(c => c.ExecuteSqlToList<ProjectedVisitDto>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(new List<ProjectedVisitDto>());

            MockTransaction = new Mock<IDbContextTransactionProxy>();

            MockContext
                .Setup(db => db.BeginTransaction())
                .Returns(MockTransaction.Object);

            MockTranslationService = new Mock<ITranslationService>();
            MockPatientVisitRepository = new Mock<IPatientVisitRepository>();
            MockPrimeInventoryAPIRepository = new Mock<IPrimeInventoryAPIRepository>();
            MockSiteRepository = new Mock<ISiteRepository>();
            MockCountryService = new Mock<ICountryService>();
            MockSubjectInformationService = new Mock<ISubjectInformationService>();
            MockVisitService = new Mock<IVisitService>();
            MockCareGiverTypeService = new Mock<ICareGiverTypeService>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockStudySettingService = BuildBaseMockStudySettingService();
            MockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            MockDeviceRepository = new Mock<IDeviceRepository>();
            MockPatientStatusService = new Mock<IPatientStatusService>();
            MockNotificationRequestRepositoy = new Mock<INotificationRequestRepository>();
            MockAuthService = new Mock<IAuthService>();
            SetupBaseData();

            YPrimeSession.SinglePatientAlias = "Subject";

            SetupSession();

            Repository = new PatientRepository(
                MockContext.Object,
                MockTranslationService.Object,
                MockPatientVisitRepository.Object,
                MockSiteRepository.Object,
                MockPrimeInventoryAPIRepository.Object,
                MockCountryService.Object,
                MockSubjectInformationService.Object,
                MockVisitService.Object,
                MockPatientStatusService.Object,
                MockStudySettingService.Object,
                MockCareGiverTypeService.Object,
                MockQuestionnaireService.Object,
                MockSoftwareReleaseRepository.Object,
                MockDeviceRepository.Object,
                MockNotificationRequestRepositoy.Object,
                MockAuthService.Object);
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

            BasePatientStatuses = new List<PatientStatusModel>
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

            var baseCareGiverType = new CareGiverTypeModel
            {
                Id = BaseCareGiverTypeId,
                Name = "Other",
                TranslationKey = "Other"
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
            MockContext.Setup(c => c.Sites).Returns(siteDataset.Object);

            BasePatients = new List<Patient>();
            PatientDataset = new FakeDbSet<Patient>(BasePatients);
            MockContext.Setup(c => c.Patients).Returns(PatientDataset.Object);

            BaseSoftwareReleases = new List<SoftwareRelease>();
            SoftwareReleaseDataset = new FakeDbSet<SoftwareRelease>(BaseSoftwareReleases);
            MockContext.Setup(c => c.SoftwareReleases).Returns(SoftwareReleaseDataset.Object);

            BaseSoftwareReleaseCountries = new List<SoftwareReleaseCountry>();
            SoftwareReleaseCountryDataset = new FakeDbSet<SoftwareReleaseCountry>(BaseSoftwareReleaseCountries);
            MockContext.Setup(c => c.SoftwareReleaseCountry).Returns(SoftwareReleaseCountryDataset.Object);

            BaseSoftwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>();
            SoftwareReleaseDeviceTypeDataset = new FakeDbSet<SoftwareReleaseDeviceType>(BaseSoftwareReleaseDeviceTypes);
            MockContext.Setup(c => c.SoftwareReleaseDeviceTypes).Returns(SoftwareReleaseDeviceTypeDataset.Object);

            BaseDevices = new List<Device>();
            DeviceDataset = new FakeDbSet<Device>(BaseDevices);
            MockContext.Setup(c => c.Devices).Returns(DeviceDataset.Object);

            BaseCorrectionApprovalDatas = new List<CorrectionApprovalData>();
            var correctionApprovalDataset = new FakeDbSet<CorrectionApprovalData>(BaseCorrectionApprovalDatas);
            MockContext.Setup(c => c.CorrectionApprovalDatas).Returns(correctionApprovalDataset.Object);

            LettersOnlyConfigDetail = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Name = nameof(LettersOnlyConfigDetail),
                DateFormat = string.Empty,
                ChoiceType = DataType.LettersOnlyAttribute.DisplayName,
                Min = "3",
                Max = "10",
                DisableNumeric = true
            };

            DateFormatConfigDetail = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Name = nameof(DateFormatConfigDetail),
                DateFormat = $"dd/MM/yyyy",
                ChoiceType = DataType.DateAttribute.DisplayName,
                Min = "{{" + DefaultAttributeMinimumDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) + "+0d:0m:0y}}",
                Max = "{{" + DefaultAttributeMaximumDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) + "+0d:0m:0y}}",
            };

            NumberAttributeConfigDetail = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Name = nameof(NumberAttributeConfigDetail),
                ChoiceType = DataType.NumberAttribute.DisplayName,
                Max = 99.ToString(),
                Choices = new List<ChoiceModel>(),
            };

            MockSubjectInformationService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    LettersOnlyConfigDetail,
                    DateFormatConfigDetail,
                    NumberAttributeConfigDetail
                });

            MockSubjectInformationService
                .Setup(s => s.GetForCountry(It.Is<Guid>(id => id == UnitedStatesCountry.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    LettersOnlyConfigDetail,
                    DateFormatConfigDetail,
                    NumberAttributeConfigDetail
                });

            MockTranslationService
                .Setup(r => r.GetByKey(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(string.Empty);

            MockPatientStatusService
                .Setup(s => s.GetAll(null))
                .ReturnsAsync(BasePatientStatuses);

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
            MockContext.Setup(c => c.PatientVisits).Returns(PatientVisitDataset.Object);

            BaseDiaryEntries = new List<DiaryEntry>();
            DiaryEntryDataset = new FakeDbSet<DiaryEntry>(BaseDiaryEntries);
            MockContext.Setup(c => c.DiaryEntries).Returns(DiaryEntryDataset.Object);

            MockCountryService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(allCountries);

            MockCareGiverTypeService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CareGiverTypeModel> { baseCareGiverType });

            BaseCareGivers = new List<CareGiver>();
            CareGiverDataset = new FakeDbSet<CareGiver>(BaseCareGivers);
            MockContext.Setup(c => c.CareGivers).Returns(CareGiverDataset.Object);

            this.MockAuthService.Setup(x => x.CreateSubjectAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(new AuthUserSignupResponse { UserId = "AuthUserId" }));
        }
    }
}
