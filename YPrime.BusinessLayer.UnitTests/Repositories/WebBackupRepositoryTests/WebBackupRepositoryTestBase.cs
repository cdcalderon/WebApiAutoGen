using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Helpers;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.UnitTests.TestObjects;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public abstract class WebBackupRepositoryTestBase : RepositoryTestBase
    {
        protected const int ExpectedTabletHeight = 650;
        protected const int ExpectedTabletWidth = 433;
        protected const int ExpectedHandheldHeight = 636;
        protected const int ExpectedHandheldWidth = 320;

        protected const string WebBackupTabletPublicKeyName = "WebBackupTabletPublicKey";
        protected const string WebBackupHandheldPublicKeyName = "WebBackupHandheldPublicKey";
        protected const string WebBackupHandheldTitleValue = "Title {SiteName}";
        protected const string WebBackupHandheldInstructionValue = "Web backup instructions";
        protected const string WebBackupHandheldErrorValue = "Error instructions";

        protected const string TestHostAddress = "127.0.0.1";
        protected const string TestTimeZone = "Eastern Standard Time";
        protected const string TestUtcTimeZone = "UTC";
        protected const string TestKeyValue = "XYZ-Test-Key";
        protected const string TestAssetTag = "YP-Z12345";
        protected const string TestSiteName = "Test Site";
        protected const string TestStudyName = "Study XYZ";
        protected const string TestUrl = "https://www.fakesite.com/webbackup/";
        protected Mock<IConfirmationRepository> MockConfirmationRepository;
        protected Mock<IStudyDbContext> MockContext;
        protected Mock<IDeviceRepository> MockDeviceRepository;
        protected Mock<IJwtRepository> MockJwtRepository;
        protected Mock<IPatientRepository> MockPatientRepository;
        protected Mock<ISiteRepository> MockSiteRepository;

        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<ITimeZoneRepository> MockTimeZoneRepository;
        protected Mock<ITranslationService> MockTranslationService;
        protected Mock<ILanguageService> MockLanguageService;
        protected Mock<IWebBackupRepository> MockWebBackUpRepository;
        protected Mock<ISoftwareReleaseRepository> MockSoftwareReleaseRepository;
        protected Mock<ISystemSettingRepository> MockSystemSettingRepository;

        protected Mock<IConfigurationSettings> ConfigSettings;

        protected Patient TestPatient;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockSiteRepository = new Mock<ISiteRepository>();
            MockTranslationService = new Mock<ITranslationService>();
            MockTimeZoneRepository = new Mock<ITimeZoneRepository>();
            MockDeviceRepository = new Mock<IDeviceRepository>();
            MockJwtRepository = new Mock<IJwtRepository>();
            MockConfirmationRepository = new Mock<IConfirmationRepository>();
            MockPatientRepository = new Mock<IPatientRepository>();
            MockContext = new Mock<IStudyDbContext>();
            MockLanguageService = new Mock<ILanguageService>();
            MockStudySettingService = new Mock<IStudySettingService>();
            ConfigSettings = new Mock<IConfigurationSettings>();
            MockWebBackUpRepository = new Mock<IWebBackupRepository>();
            MockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            MockSystemSettingRepository = new Mock<ISystemSettingRepository>();

            SetupTranslations();

            var studySettingValues = new Dictionary<string, string>
            {
                { "StudyName", TestStudyName }
            };

            YPrimeSession.SinglePatientAlias = "Subject";
            YPrimeSession.StudySettingValues = studySettingValues;

            SetupSession();
        }

        protected WebBackupRepository GetRepository()
        {
            var repository = new WebBackupRepository(
                MockContext.Object,
                MockSiteRepository.Object,
                MockSystemSettingRepository.Object,
                MockTimeZoneRepository.Object,
                MockDeviceRepository.Object,
                MockJwtRepository.Object,
                MockLanguageService.Object,
                MockTranslationService.Object,
                ConfigSettings.Object,
                MockStudySettingService.Object,
                MockSoftwareReleaseRepository.Object);

            return repository;
        }

        private void SetupTranslations()
        {
            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(tk => tk == "WebBackupHandheldTitle"), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(WebBackupHandheldTitleValue);

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(tk => tk == "WebBackupHandheldInstruction"), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(WebBackupHandheldInstructionValue);

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(tk => tk == "WebBackupHandheldError"), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(WebBackupHandheldErrorValue);
        }
    }
}