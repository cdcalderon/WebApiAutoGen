using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryRemoveDeviceTests : DeviceRepositoryTestBase
    {
        private const int InitialSyncVersion = 0;

        private List<Device> _insertedDevices;
        private List<Device> _existingDevices;
        private SoftwareVersion _softwareVersion;
        private SoftwareRelease _softwareRelease;
        private Device _existingDevice;
        private Guid _softwareReleaseId;
        private Guid _softwareVersionId;
        private Guid _configVersionId;

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersion = new SoftwareVersion
            {
                Id = _softwareVersionId
            };

            _softwareRelease = new SoftwareRelease
            {
                Id = _softwareReleaseId,
                SoftwareVersionId = _softwareVersionId,
                SoftwareVersion = _softwareVersion,
                ConfigurationId = _configVersionId
            };

            _existingDevice = new Device
            {
                Id = Guid.NewGuid(),
                SyncVersion = InitialSyncVersion,
                DeviceTypeId = DeviceType.BYOD.Id,
                Site = new Site { Id = Guid.NewGuid(),Name="TestSite"},
                SoftwareRelease = _softwareRelease
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

            deviceDbSet
                .Setup(q => q.Remove(It.IsAny<Device>()))
                .Returns((Device removedDevice) =>
                {
                    _existingDevices.Remove(removedDevice);

                    return removedDevice;
                });

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDbSet.Object);

            _softwareReleaseRepository
                .Setup(r => r.GetLatestSoftwareRelease(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(_softwareRelease);

            _configurationVersionService
                .Setup(q => q.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<Core.BusinessLayer.Models.ConfigurationVersion> { new Core.BusinessLayer.Models.ConfigurationVersion { Id = _configVersionId, StudyId = Guid.NewGuid() } });
        }

        [TestMethod]
        public async Task RemoveDevice_NoDeviceDoesNotThrow()
        {
            var device = await Repository.GetDevice(_existingDevice.Id, new List<Guid>());

            Assert.IsNotNull(device);
            Assert.AreEqual(device.Id, _existingDevice.Id);

            await Repository.RemoveDevice(_existingDevice);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async() => await Repository.GetDevice(_existingDevice.Id, new List<Guid>()));

            Context
                .Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveDevice_DeviceIsRemoved()
        {
            await Repository.RemoveDevice(null);

            Context.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Never);
        }
    }
}