using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryAddDeviceTests : DeviceRepositoryTestBase
    {
        private const int InitialSyncVersion = 0;

        private List<Device> _insertedDevices;
        private List<Device> _existingDevices;

        private Device _existingDevice;
        private Guid _softwareReleaseId;
        private Guid _softwareVersionId;
        private Guid _configVersionId;

        [TestInitialize]
        public void TestInitialize()
        {
            _existingDevice = new Device
            {
                Id = Guid.NewGuid(),
                SyncVersion = InitialSyncVersion
            };

            _insertedDevices = new List<Device>();

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

            _softwareReleaseRepository
                .Setup(r => r.GetLatestSoftwareRelease(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(new SoftwareRelease
                {
                    Id = _softwareReleaseId,
                    SoftwareVersionId = _softwareVersionId,
                    ConfigurationId = _configVersionId
                });
        }

        [TestMethod]
        public void WhenCalled_WillReturnAddedDevice()
        {
            var siteId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();
            var assetTag = Guid.NewGuid().ToString();
            var deviceType = DeviceType.Phone.Id;
            var patientId = Guid.NewGuid();

            var result = Repository.AddDevice(deviceId, patientId, siteId, deviceType, assetTag);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, _insertedDevices.Count);

            var addedDevice = _insertedDevices.First(d => d.Id == deviceId);

            Assert.IsNotNull(addedDevice);
            Assert.AreEqual(InitialSyncVersion, addedDevice.SyncVersion);
            Assert.AreEqual(patientId, addedDevice.PatientId);
            Assert.AreEqual(siteId, addedDevice.SiteId);
            Assert.AreEqual(assetTag, addedDevice.AssetTag);
            Assert.AreEqual(InitialSyncVersion, addedDevice.SyncVersion);
            Assert.AreEqual(_configVersionId, addedDevice.LastReportedConfigurationId);
            Assert.AreEqual(_softwareVersionId, addedDevice.LastReportedSoftwareVersionId);

            Context
                .Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);
        }
    }
}