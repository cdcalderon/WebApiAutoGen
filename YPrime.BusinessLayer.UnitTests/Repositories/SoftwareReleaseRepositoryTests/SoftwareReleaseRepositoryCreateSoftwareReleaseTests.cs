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
    public class SoftwareReleaseRepositoryCreateSoftwareReleaseTests : SoftwareReleaseRepositoryTestBase
    {
        private FakeDbSet<SoftwareRelease> _dataSet;
        private SoftwareReleaseDto _Dto;
        private SoftwareRelease _softwareRelease;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _priorSoftwareVersionId;
        private SoftwareVersion _priorSoftwareVersion;
        private Guid _newSoftwareVersionId;
        private SoftwareVersion _newSoftwareVersion;
        private FakeDbSet<SoftwareVersion> _softwareVersionDataSet;
        private List<SoftwareReleaseDeviceType> _softwareDeviceTypes;
        private List<SoftwareVersion> _softwareVersions;
        
        private string _priorVersionNumber;
        private string _versionNumber;
        private ConfigurationVersion _configurationVersion;
        private ConfigurationVersion _configurationVersion2;
        private SoftwareRelease _addedSoftwareRelease = null;
        private Device _device;

        [TestInitialize]
        public void TestInitialize()
        {
            _priorSoftwareVersionId = Guid.NewGuid();
            _newSoftwareVersionId = Guid.NewGuid();
            _priorVersionNumber = "1.2.3.3";
            _versionNumber = "1.2.3.4";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();
            _softwareDeviceTypes = new List<SoftwareReleaseDeviceType>();
            _softwareVersions = new List<SoftwareVersion>();

            _configurationVersion = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                SrdVersion = "02.53",
                ConfigurationVersionNumber = "4.0"
            };

            _configurationVersion2 = new ConfigurationVersion
            {
                Id = Guid.NewGuid(),
                SrdVersion = "02.53",
                ConfigurationVersionNumber = "5.0"
            };

            _Dto = new SoftwareReleaseDto
            {
                ReleaseDate = "dd-MM-yy",
                Name = "test",
                VersionNumber = _versionNumber,
                IsActive = true,
                Required = true,
                ConfigVersionNumber = _configurationVersion.Id.ToString(),
                ConfigurationId = Guid.NewGuid(),
                StudyWide = true,
                CountryNameList = "USA, Canada",
                SiteNameList = "10001, 20001",
                AssetTagList = "YP-12345, YP-67890",
                AssignedReportedVersionCount = "2",
            };

            _priorSoftwareVersion = new SoftwareVersion
            {
                Id = _priorSoftwareVersionId,
                VersionNumber = _priorVersionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _newSoftwareVersion = new SoftwareVersion
            {
                Id = _newSoftwareVersionId,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _softwareRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                Name = "test",
                SoftwareVersionId = _priorSoftwareVersionId,
                SoftwareVersion = _priorSoftwareVersion,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = Guid.NewGuid(),
                ConfigurationVersion = "1.0",
                StudyWide = true
            };

            _device = new Device
            {
                Id = Guid.NewGuid(),
                SoftwareReleaseId = _softwareRelease.Id,
                SoftwareRelease = _softwareRelease
            };

            ConfigurationVersionService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ConfigurationVersion> { _configurationVersion, _configurationVersion2 });

            _softwareVersions.Add(_priorSoftwareVersion);
            _softwareVersions.Add(_newSoftwareVersion);

            SetupSoftwareVersionContext(_softwareVersions);
            SetupSoftwareReleaseContext(new List<SoftwareRelease> {_softwareRelease});
            SetupDeviceContext(new[] { _device });

            var softwareDeviceTypeDataSet = new FakeDbSet<SoftwareReleaseDeviceType>(_softwareDeviceTypes);

            softwareDeviceTypeDataSet
                .Setup(ds => ds.Add(It.IsAny<SoftwareReleaseDeviceType>()))
                .Callback((SoftwareReleaseDeviceType addedDeviceType) =>
                {
                    _softwareDeviceTypes.Add(addedDeviceType);
                });

            Context
                .Setup(c => c.SoftwareReleaseDeviceTypes)
                .Returns(softwareDeviceTypeDataSet.Object);
        }

        [TestMethod]
        public async Task CreateSoftwareReleaseTest()
        {
            await Repository.CreateSoftwareRelease(_Dto);

            Assert.IsNotNull(_Dto.Id);
            
            Assert.IsNotNull(_addedSoftwareRelease);
            Assert.AreEqual(_addedSoftwareRelease.Id, _Dto.Id);
            Assert.AreEqual(_configurationVersion.Id, _addedSoftwareRelease.ConfigurationId);
            Assert.AreEqual(_configurationVersion.SrdVersion, _addedSoftwareRelease.SRDVersion);


            Assert.AreEqual(_Dto.Id, _device.SoftwareReleaseId);

            Context
                .Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.AreEqual(0, _softwareDeviceTypes.Count);
        }

        [TestMethod]
        public async Task CreateSoftwareReleaseDeviceIdsTest()
        {
            _Dto.StudyWide = false;
            _Dto.ConfigVersionNumber = _configurationVersion.Id.ToString();
            _Dto.DeviceTypeIds = new List<Guid>
            {
                DeviceType.Phone.Id
            };

            await Repository.CreateSoftwareRelease(_Dto);

            Assert.IsNotNull(_Dto.Id);

            Assert.IsNotNull(_addedSoftwareRelease);
            Assert.AreEqual(_addedSoftwareRelease.Id, _Dto.Id);
            Assert.AreEqual(_configurationVersion.Id, _addedSoftwareRelease.ConfigurationId);

            Context
                .Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.AreEqual(1, _softwareDeviceTypes.Count);
            Assert.AreEqual(DeviceType.Phone.Id, _softwareDeviceTypes[0].DeviceTypeId);
        }

        [TestMethod]
        public async Task CreateSoftwareReleaseUpdateSessionConfigIdTest()
        {
            _Dto.StudyWide = true;
            _Dto.ConfigVersionNumber = _configurationVersion2.Id.ToString();

            await Repository.CreateSoftwareRelease(_Dto);

            Assert.IsNotNull(_Dto.Id);

            Assert.IsNotNull(_addedSoftwareRelease);
            Assert.AreEqual(_addedSoftwareRelease.Id, _Dto.Id);
            Assert.AreEqual(_configurationVersion2.Id, _addedSoftwareRelease.ConfigurationId);

            Context
                .Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Exactly(2));

            Assert.AreEqual(YPrimeSession.ConfigurationId, _configurationVersion2.Id);
        }


        private void SetupSoftwareReleaseContext(List<SoftwareRelease> items)
        {
            _dataSet = new FakeDbSet<SoftwareRelease>(items);

            _dataSet
                .Setup(ds => ds.Add(It.IsAny<SoftwareRelease>()))
                .Callback((SoftwareRelease passedInEntity) =>
                {
                    var matchingVersion = _softwareVersions.FirstOrDefault(v => v.Id == passedInEntity.SoftwareVersionId);
                    passedInEntity.SoftwareVersion = matchingVersion;
                    items.Add(passedInEntity);
                    _addedSoftwareRelease = passedInEntity;
                });

            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_dataSet.Object);
        }

        private void SetupSoftwareVersionContext(List<SoftwareVersion> items)
        {
            _softwareVersionDataSet = new FakeDbSet<SoftwareVersion>(items);

            _softwareVersionDataSet
                .Setup(ds => ds.Add(It.IsAny<SoftwareVersion>()))
                .Callback((SoftwareVersion passedInEntity) =>
                {
                    items.Add(passedInEntity);
                });

            Context
                .Setup(ctx => ctx.SoftwareVersions)
                .Returns(_softwareVersionDataSet.Object);
        }

        private void SetupDeviceContext(IEnumerable<Device> items)
        {
            var dataSet = new FakeDbSet<Device>(items);

            Context.Setup(ctx => ctx.Devices)
                .Returns(dataSet.Object);
        }
    }
}