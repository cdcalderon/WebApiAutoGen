using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetSoftwareReleaseDeviceConfirmationTests : SoftwareReleaseRepositoryTestBase
    {
        private FakeDbSet<SoftwareRelease> _dataSet;
        private SoftwareReleaseDto _dto;
        private SoftwareRelease _model;
        private Device _deviceModel;
        private Guid _deviceId;
        private List<Device> _deviceList;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _softwareVersionId;
        private FakeDbSet<SoftwareVersion> _svDataSet;
        private SoftwareVersion _svModel;
        private ConfigurationVersion _configurationVersion;

        private const string ExistingSoftwareVersionNumber = "1.2.3.4";
        private const string ExistingConfigVersionNumber = "4.0";

        private const string HigherSoftwareVersionNumber = "2.0.0.0";
        private const string HigherConfigVersionNumber = "5.0";

        private const string LowerSoftwareVersionNumber = "1.0.0.1";
        private const string LowerConfigVersionNumber = "2.0";

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersionId = Guid.NewGuid();
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();
            _deviceId = Guid.NewGuid();

            _configurationVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                SrdVersion = "02.53",
                ConfigurationVersionNumber = ExistingConfigVersionNumber
            };

            _dto = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = ExistingSoftwareVersionNumber,
                IsActive = true,
                Required = true,
                ConfigVersionNumber = _configurationVersion.DisplayVersion,
                ConfigurationId = _configurationVersion.Id,
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2"
            };
            
            _svModel = new SoftwareVersion
            {
                Id = _softwareVersionId,
                VersionNumber = ExistingSoftwareVersionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            var softwareVersionExisting = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = ExistingSoftwareVersionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _model = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                Name = "test",
                SoftwareVersionId = _softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                SoftwareVersion = softwareVersionExisting,
                ConfigurationVersion = ExistingConfigVersionNumber
            };

            _deviceModel = new Device
            {
                Id = _deviceId,
                SoftwareReleaseId = _model.Id,
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = null,
                DeviceTypeId = new Guid(),
                LastSyncDate = null,
                SoftwareRelease = _model
            };

            _deviceList = new List<Device>();
            _deviceList.Add(_deviceModel);

            SetupDeviceContext(new[] { _deviceModel });

            ConfigurationVersionService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ConfigurationVersion> { _configurationVersion });

            var message = "{{eligibleSoftwareVersions}} device(s) will be assigned to this Software Release. <br><br> {{ineligibleSoftwareVersions}} device(s) have a higher Software Release. <br><br><br><br> {{eligibleConfigVersions}} device(s) will be assigned to this Configuration Release. <br><br> {{ineligibleConfigVersions}} device(s) have a higher Configuration Release.";

            TranslationService
                .Setup(s => s.GetByKey(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(message);

            SetupSoftwareVersionContext(new[] { _svModel });
            SetupContext(new[] { _model });
        }

        [TestMethod]
        public async Task WhenCalledWillReturnConfirmationMessage()
        {
            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public async Task HigherSoftwareHigherConfigVersionTest()
        {
            _dto.VersionNumber = HigherSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = HigherConfigVersionNumber;
            
            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("1 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("0 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("1 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("0 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task HigherSoftwareSameConfigVersionTest()
        {
            _dto.VersionNumber = HigherSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = ExistingConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("1 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("0 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task HigherSoftwareLowerConfigVersionTest()
        {
            _dto.VersionNumber = HigherSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = LowerConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task SameSoftwareSameConfigVersionTest()
        {
            _dto.VersionNumber = ExistingSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = ExistingConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task LowerSoftwareHigherConfigVersionTest()
        {
            _dto.VersionNumber = LowerSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = HigherConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("1 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("0 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task SameSoftwareHigherConfigVersionTest()
        {
            _dto.VersionNumber = ExistingSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = HigherConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("1 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("0 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task LowerSoftwareLowerConfigVersionTest()
        {
            _dto.VersionNumber = LowerSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = LowerSoftwareVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task LowerSoftwareSameConfigVersionTest()
        {
            _dto.VersionNumber = LowerSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = ExistingConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        [TestMethod]
        public async Task SameSoftwareLowerConfigVersionTest()
        {
            _dto.VersionNumber = ExistingSoftwareVersionNumber;
            _configurationVersion.ConfigurationVersionNumber = LowerConfigVersionNumber;

            var result = await Repository.GetSoftwareReleaseDeviceConfirmation(_dto);

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Software Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Software Release."));

            Assert.IsTrue(result.Contains("0 device(s) will be assigned to this Configuration Release."));
            Assert.IsTrue(result.Contains("1 device(s) have a higher Configuration Release."));
        }

        private void SetupContext(IEnumerable<SoftwareRelease> items)
        {
            _dataSet = new FakeDbSet<SoftwareRelease>(items);

            _dataSet
                .Setup(ds => ds.Add(It.IsAny<SoftwareRelease>()));

            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_dataSet.Object);
        }

        private void SetupSoftwareVersionContext(IEnumerable<SoftwareVersion> items)
        {
            _svDataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_svDataSet.Object);
        }

        private void SetupDeviceContext(IEnumerable<Device> items)
        {
            var dbSet = new FakeDbSet<Device>(items);
            Context.Setup(ctx => ctx.Devices)
                .Returns(dbSet.Object);
        }
    }
}