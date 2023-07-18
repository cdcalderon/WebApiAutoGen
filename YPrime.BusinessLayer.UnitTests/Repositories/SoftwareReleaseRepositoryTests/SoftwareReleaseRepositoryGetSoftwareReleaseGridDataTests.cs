using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetSoftwareReleaseGridDataTests : SoftwareReleaseRepositoryTestBase
    {
        private FakeDbSet<SoftwareRelease> _dataSet;
        private FakeDbSet<SoftwareReleaseCountry> _srcDataSet;
        private FakeDbSet<Device> _deviceDataSet;
        private Device _deviceModel;
        private SoftwareReleaseDto _Dto;
        private SoftwareRelease _model;
        private SoftwareReleaseCountry _softwareCountryModel;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _softwareVersionId;
        private Guid _softwareReleaseId;
        private Guid _countryId;
        private FakeDbSet<SoftwareVersion> _svDataSet;
        private SoftwareVersion _svModel;
        private string _versionNumber;
        private ConfigurationVersion _firstConfigVersion;
        private ConfigurationVersion _secondConfigVersion;

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersionId = Guid.NewGuid();
            _softwareReleaseId = Guid.NewGuid();
            _countryId = Guid.NewGuid();
            _versionNumber = "1.2.3.4";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();

            _firstConfigVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                ConfigurationVersionNumber = "1.0",
                SrdVersion = "01.00"
            };

            _secondConfigVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                ConfigurationVersionNumber = "2.0",
                SrdVersion = "02.00"
            };

            _Dto = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = "0.0.0.1",
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2"
            };

            _model = new SoftwareRelease
            {
                Id = _softwareReleaseId,
                Name = "test",
                SoftwareVersionId = _softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = _firstConfigVersion.Id,
                StudyWide = true
            };


            _softwareCountryModel = new SoftwareReleaseCountry
            {
                CountryId = _countryId,
                SoftwareReleaseId = _softwareReleaseId
            };

            _svModel = new SoftwareVersion
            {
                Id = _softwareVersionId,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _deviceModel = new Device();

            CountryService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<CountryModel>
            {
                new CountryModel
                {
                    Id = _countryId,
                    Name = "Test Country"
                }
            });

            ConfigurationVersionService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ConfigurationVersion>
                {
                    _firstConfigVersion,
                    _secondConfigVersion
                });

            SetupSoftwareVersionContext(new[] {_svModel});
            SetupSoftwareReleaseCountryContext(new[] { _softwareCountryModel });
            SetupDeviceContext(new[] {_deviceModel});
            SetupContext(new[] {_model});
        }

        [TestMethod]
        public async Task WithExistingRecordsInDataBase_WillReturnAllSoftwareReleaseData()
        {
            var result = await Repository.GetSoftwareReleaseGridData();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());

            var softwareRelease = result.First();

            Assert.AreEqual(softwareRelease.ConfigurationId, _firstConfigVersion.Id);
            Assert.AreEqual(softwareRelease.ConfigVersionNumber, _firstConfigVersion.DisplayVersion);
        }

        [TestMethod]
        public async Task WithNoDeviceRecordsInDataBase_WillReturnEmptyAssetTagString()
        {
            SetupDeviceContext(Enumerable.Empty<Device>());

            var result = await Repository.GetSoftwareReleaseGridData();

            Assert.IsTrue(result.FirstOrDefault().AssetTagList == string.Empty);
        }

        [TestMethod]
        public async Task WhenCalledWillReturnCollectionType_List()
        {
            var result = await Repository.GetSoftwareReleaseGridData();
            Assert.IsTrue(result.GetType() == typeof(List<SoftwareReleaseDto>));

            var firstResult = result.First();

            Assert.IsTrue(firstResult.DeviceTypeNames.Contains(DeviceType.Phone.Name));
            Assert.IsTrue(firstResult.DeviceTypeNames.Contains(","));
            Assert.IsTrue(firstResult.DeviceTypeNames.Contains(DeviceType.Tablet.Name));
        }

        [TestMethod]
        public async Task WithNoExistingRecordsInDataBase_WillReturnEmptyList()
        {
            SetupContext(Enumerable.Empty<SoftwareRelease>());
            var result = await Repository.GetSoftwareReleaseGridData();
            Assert.IsFalse(result.Any());
        }

        private void SetupContext(IEnumerable<SoftwareRelease> items)
        {
            _dataSet = new FakeDbSet<SoftwareRelease>(items);
            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_dataSet.Object);

            var deviceTypes = DeviceType.GetAll<DeviceType>();
            var softwareReleaseDeviceTypes = new List<SoftwareReleaseDeviceType>();

            foreach (var softwareRelease in items)
            {
                foreach (var deviceType in deviceTypes)
                {
                    softwareReleaseDeviceTypes.Add(new SoftwareReleaseDeviceType
                    {
                        Id = Guid.NewGuid(),
                        SoftwareReleaseId = softwareRelease.Id,
                        DeviceTypeId = deviceType.Id
                    });
                }
            }

            var releaseDeviceTypeDataSet = new FakeDbSet<SoftwareReleaseDeviceType>(softwareReleaseDeviceTypes);

            Context
                .Setup(c => c.SoftwareReleaseDeviceTypes)
                .Returns(releaseDeviceTypeDataSet.Object);
        }


        private void SetupSoftwareVersionContext(IEnumerable<SoftwareVersion> items)
        {
            _svDataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_svDataSet.Object);
        }

        private void SetupSoftwareReleaseCountryContext(IEnumerable<SoftwareReleaseCountry> items)
        {
            _srcDataSet = new FakeDbSet<SoftwareReleaseCountry>(items);
            Context.Setup(ctx => ctx.SoftwareReleaseCountry)
                .Returns(_srcDataSet.Object);
        }

        private void SetupDeviceContext(IEnumerable<Device> items)
        {
            _deviceDataSet = new FakeDbSet<Device>(items);
            Context.Setup(ctx => ctx.Devices)
                .Returns(_deviceDataSet.Object);
        }
    }
}