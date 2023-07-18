using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryGetPatientDeviceTests : DeviceRepositoryTestBase
    {
        private Device device;
        private readonly Guid patientId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            Context.Reset();

            device = new Device
            {
                Id = Guid.NewGuid(),
                DeviceTypeId = DeviceType.BYOD.Id,
                PatientId = patientId,
                AssetTag = "AssetTag"
            };

            var devices = new[]
            {
                device
            };

            SetupContext(devices);
        }

        [TestMethod]
        public void WhenCalled_WillReturnCorrespondingDevice()
        {
            var result = Repository.GetPatientBYODDevice(patientId);

            Assert.AreEqual(device, result);
        }

        [TestMethod]
        public void WhenNoDeviceHasPatientId_WillReturnNull()
        {
            var result = Repository.GetPatientBYODDevice(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void WhenNoDeviceExist_WillReturnNull()
        {
            SetupContext(Enumerable.Empty<Device>());

            var result = Repository.GetPatientBYODDevice(patientId);

            Assert.IsNull(result);
        }
    }
}