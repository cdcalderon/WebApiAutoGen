using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryInsertDeviceDataTests : DataSyncRepositoryTestBase
    {
        private const int InitialSyncVersion = 1;

        private List<Device> _insertedDevices;
        private List<Device> _existingDevices;
        private List<DeviceData> _deviceDatas;

        private Device _existingDevice;
        private Device _createdDevice;
        private Guid _siteId;
        private Guid _softwareReleaseId;
        private Guid _softwareVersionId;
        private Guid _configVersionId;
        private string _assetTag;
        private string _softwareVersion;
        private string _fob;
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
            _fob = "key";

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

            _deviceDatas = new List<DeviceData>();

            var deviceDataDbSet = new FakeDbSet<DeviceData>(_deviceDatas);
            Context
                .Setup(c => c.DeviceDatas)
                .Returns(deviceDataDbSet.Object);

            deviceDataDbSet
                .Setup(ds => ds.Add(It.IsAny<DeviceData>()))
                .Callback((DeviceData deviceData) =>
                {
                    _deviceDatas.Add(deviceData);
                });

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
                .Setup(r => r.AddDevice(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.Is<Guid>(id => id == _siteId), It.IsAny<Guid>(), It.IsAny<string>()))
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
        public void AddDeviceDataTest()
        {
            Repository.AddDeviceData(_existingDevice.Id, _fob);

            var expectedData = new DeviceData()
            {
                Id = Guid.NewGuid(),
                DeviceId = _existingDevice.Id,
                Fob = _fob
            };

            var addedData = _deviceDatas.SingleOrDefault(d => d.DeviceId == _existingDevice.Id);
            Assert.AreEqual(expectedData.DeviceId, addedData.DeviceId);
            Assert.AreEqual(expectedData.Fob, addedData.Fob);
            Context.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void AddDeviceDataTestMultiplePhysicalDevices()
        {
            AddDeviceDataTest();

            var expectedData1 = new DeviceData()
            {
                Id = Guid.NewGuid(),
                DeviceId = _existingDevice.Id,
                Fob = _fob
            };

            var expectedData2 = new DeviceData()
            {
                Id = Guid.NewGuid(),
                DeviceId = _existingDevice.Id,
                Fob = "newKey"
            };

            Repository.AddDeviceData(expectedData2.DeviceId, expectedData2.Fob);

            Assert.AreEqual(2, _deviceDatas.Count(d => d.DeviceId == _existingDevice.Id));
            _deviceDatas.SingleOrDefault(d => d.DeviceId == _existingDevice.Id && d.Fob == expectedData1.Fob );
            _deviceDatas.SingleOrDefault(d => d.DeviceId == _existingDevice.Id && d.Fob == expectedData2.Fob);
            Context.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));
        }


        [TestMethod]
        public void AddDeviceDataNoDeviceTest()
        {
            Assert.ThrowsException<DeviceNotFoundException>(() => Repository.AddDeviceData(Guid.NewGuid(), _fob));
        }
    }
}
