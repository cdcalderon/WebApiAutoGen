using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryGetWebBackupHandheldDeviceTests : DeviceRepositoryTestBase
    {
        private FakeDbSet<Device> _dataSet;
        private Guid _deviceId;
        private Device _deviceModel;
        private Guid _patientId;

        [TestInitialize]
        public void TestInitialize()
        {
            Context.Reset();

            _deviceId = Guid.NewGuid();
            _patientId = Guid.NewGuid();
            _deviceModel = new Device
            {
                Id = _deviceId,
                SoftwareReleaseId = Guid.NewGuid(),
                AssetTag = "YP-Test",
                IMEI1 = null,
                IMEI2 = null,
                MACAddress = null,
                SerialNumber = null,
                SiteId = null,
                DeviceTypeId = DeviceType.Phone.Id,
                LastSyncDate = null,
                PatientId = _patientId
            };
            SetupContext(new[] {_deviceModel});
        }

        [TestMethod]
        public void WhenCalledWithPatientId_WithExistingDevice()
        {
            var device = Repository.GetWebBackupHandheldDevice(_patientId);
            Assert.IsNotNull(device);
        }

        [TestMethod]
        public void WhenCalledWithPatientId_WithExistingBYODDevice()
        {
            var patientId = Guid.NewGuid();
            var devices = new[]
            {
                new Device
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DeviceTypeId = DeviceType.BYOD.Id,
                }
            };
            SetupContext(devices);

            var result = Repository.GetWebBackupHandheldDevice(patientId);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenCalledWithPatientId_WithoutExistingDevice()
        {
            var patientId = Guid.NewGuid();
            var device = Repository.GetWebBackupHandheldDevice(patientId);
            Assert.IsNull(device);
        }

        [TestMethod]
        public void WhenCalledOnAnEmptyDatabase()
        {
            SetupContext(Enumerable.Empty<Device>());
            Assert.IsNull(Repository.GetWebBackupHandheldDevice(_patientId));
        }

        [TestMethod]
        public void WhenDatabaseSendsAnException()
        {
            var exception = new Exception();
            Context.Setup(ctx => ctx.Devices)
                .Throws(exception);

            YAssert.ThrowsSameException(exception, () => Repository.GetWebBackupHandheldDevice(_patientId));
        }
    }
}