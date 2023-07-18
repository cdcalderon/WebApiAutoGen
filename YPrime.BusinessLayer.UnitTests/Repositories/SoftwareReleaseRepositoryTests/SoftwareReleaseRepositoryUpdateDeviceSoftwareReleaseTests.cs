using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryUpdateDeviceSoftwareReleaseTests : SoftwareReleaseRepositoryTestBase
    {
        private Guid _deviceId;
        private List<Device> _deviceList;
        private Device _deviceModel;
        private SoftwareReleaseDto _Dto;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _softwareReleaseId;
        private Guid _softwareVersionId;
        private SoftwareRelease _srModel;
        private SoftwareVersion _svModel;
        private string _versionNumber;

        [TestInitialize]
        public void TestInitialize()
        {
            Context.Reset();

            _softwareVersionId = Guid.NewGuid();
            _versionNumber = "1.2.3.4";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();
            _deviceId = new Guid();
            _softwareReleaseId = new Guid();
            _Dto = new SoftwareReleaseDto
            {
                Id = _softwareReleaseId,
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
                AssignedReportedVersionCount = "2",
            };

            _srModel = new SoftwareRelease
            {
                Id = _softwareReleaseId,
                Name = "test",
                SoftwareVersionId = _softwareVersionId,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true
            };

            _svModel = new SoftwareVersion
            {
                Id = _softwareVersionId,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _deviceModel = new Device
            {
                Id = _deviceId,
                SoftwareReleaseId = new Guid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = null,
                DeviceTypeId = new Guid(),
                LastSyncDate = null,
                SoftwareRelease = _srModel
            };

            _deviceList = new List<Device>();
            _deviceList.Add(_deviceModel);

            SetupContext(new[] { _deviceModel });
        }

        [TestMethod]
        public async Task WhenCalledWillUpdateDeviceSoftwareReleaseId()
        {
            await Repository.UpdateDeviceSoftwareRelease(_deviceList, _Dto);

            Assert.IsTrue(_deviceModel.SoftwareReleaseId == _Dto.Id);
        }

        private void SetupContext(IEnumerable<Device> devices)
        {
            var dbSet = new FakeDbSet<Device>(devices);

            Context.Setup(ctx => ctx.Devices)
                .Returns(dbSet.Object);
        }
    }
}
