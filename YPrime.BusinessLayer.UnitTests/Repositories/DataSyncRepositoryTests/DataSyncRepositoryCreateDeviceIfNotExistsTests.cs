using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.UnitTests.TestExtensions;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryCreateDeviceIfNotExistsTests : DataSyncRepositoryTestBase
    {
        private const int InitialSyncVersion = 1;

        private List<Device> _insertedDevices;
        private List<Device> _existingDevices;

        private Device _existingDevice;
        private Device _createdDevice;
        private Guid _siteId;
        private Guid _softwareReleaseId;
        private Guid _softwareVersionId;
        private Guid _configVersionId;
        private string _assetTag;
        private string _softwareVersion;
        private Guid _deviceType;
        private Guid _deviceId;

        [TestInitialize]
        public void TestInitialize()
        {
            _siteId = Guid.NewGuid();
            _softwareReleaseId = Guid.NewGuid();
            _softwareVersionId = Guid.NewGuid();
            _configVersionId = Guid.NewGuid();
            _deviceId = Guid.NewGuid();
            _softwareVersion = Guid.NewGuid().ToString();
            _assetTag = Guid.NewGuid().ToString();
            _deviceType = DeviceType.Phone.Id;

            _insertedDevices = new List<Device>();

            _createdDevice = new Device
            {
                Id = _deviceId,
                AssetTag = _assetTag,
                DeviceTypeId = _deviceType,
                SiteId = _siteId,
                SoftwareReleaseId = _softwareReleaseId,
                LastReportedSoftwareVersionId = _softwareVersionId,
                LastReportedConfigurationId = _configVersionId,
                LastSyncDate = DateTime.Now
            };

            _existingDevice = new Device
            {
                Id = Guid.NewGuid(),
                SyncVersion = InitialSyncVersion
            };

            _existingDevices = new List<Device>
            {
                _existingDevice
            };

            var deviceDbSet = new FakeDbSet<Device>(_existingDevices);

            deviceDbSet
                .Setup(ds => ds.Add(It.IsAny<Device>()))
                .Callback((Device addedDevice) =>
                {
                    _insertedDevices.Add(addedDevice);
                });

            deviceDbSet
                .Setup(d => d.Find(It.IsAny<object[]>()))
                .Returns((object[] id) =>
                {
                    return _existingDevices.SingleOrDefault(ed => ed.Id == (Guid)id[0]);
                });

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDbSet.Object);

            DeviceRepository
                .Setup(r => r.AddDevice(It.IsAny<Guid>(), It.IsAny<Guid?>(),It.Is<Guid>(id => id == _siteId), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(_createdDevice);

            SoftwareReleaseRepository
                .Setup(r => r.GetLatestSoftwareRelease(It.Is<Guid>(id => id == _siteId), It.IsAny<Guid>()))
                .Returns(new SoftwareRelease
                {
                    Id = _softwareReleaseId,
                    SoftwareVersionId = _softwareVersionId,
                    ConfigurationId = _configVersionId
                });

            SoftwareReleaseRepository
                .Setup(r => r.GetLatestSoftwareRelease(It.IsAny<Guid?>(), It.IsAny<Guid>()))
                .Returns(new SoftwareRelease
                {
                    Id = _softwareReleaseId,
                    SoftwareVersionId = _softwareVersionId
                });
        }

        [TestMethod]
        public void CreateDeviceIfNotExistsTest()
        {
            var deviceId = _deviceId;
            var softwareVersion = _softwareVersion;
            var assetTag = _assetTag;
            var deviceTypeId = _deviceType;

            Repository.CreateDeviceIfNotExists(
                deviceId,
                deviceTypeId,
                _siteId,
                softwareVersion,
                assetTag);

            Assert.AreEqual(deviceId, _createdDevice.Id);
            Assert.AreEqual(_siteId, _createdDevice.SiteId);
            Assert.AreEqual(_softwareVersionId, _createdDevice.LastReportedSoftwareVersionId);
            Assert.AreEqual(_configVersionId, _createdDevice.LastReportedConfigurationId);
            Assert.AreEqual(_softwareReleaseId, _createdDevice.SoftwareReleaseId);
            Assert.AreEqual(deviceTypeId, _createdDevice.DeviceTypeId);
            Assert.AreEqual(assetTag, _createdDevice.AssetTag);

            Assert.AreEqual(InitialSyncVersion, _createdDevice.SyncVersion);
            Assert.That.AreCloseInSeconds(_createdDevice.LastSyncDate, DateTime.Now, 5);

            Context
                .Verify(c => c.DetatchEntity(It.Is<Device>(d => d == _createdDevice)), Times.Once);
        }

        [TestMethod]
        public void CreateDeviceIfNotExistsNotNullTest()
        {
            var softwareVersion = Guid.NewGuid().ToString();
            var assetTag = Guid.NewGuid().ToString();
            var deviceType = DeviceType.Phone;

            Repository.CreateDeviceIfNotExists(
                _existingDevice.Id,
                deviceType.Id,
                _siteId,
                softwareVersion,
                assetTag);

            Assert.AreEqual(0, _insertedDevices.Count);

            Assert.AreEqual(InitialSyncVersion, _existingDevice.SyncVersion);

            Context
                .Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Never);

            Context
                .Verify(c => c.DetatchEntity(It.Is<Device>(d => d == _existingDevice)), Times.Never);
        }

        [TestMethod]
        public void CreateDeviceIfNotExistsInvalidDeviceTypeTest()
        {
            var softwareVersion = Guid.NewGuid().ToString();
            var assetTag = Guid.NewGuid().ToString();

            Assert.ThrowsException<DeviceTypeNotFoundException>(() => Repository.CreateDeviceIfNotExists(
                _existingDevice.Id,
                Guid.Empty,
                _siteId,
                softwareVersion,
                assetTag));
        }
    }
}
