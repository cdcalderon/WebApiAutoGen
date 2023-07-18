using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryGetDeviceIdForPatientTests : DeviceRepositoryTestBase
    {
        private readonly Guid deviceId = Guid.NewGuid();
        private readonly Guid patientId = Guid.NewGuid();

        [TestInitialize]
        public void TestInitialize()
        {
            Context.Reset();

            var devices = new[]
            {
                new Device
                {
                    Id = deviceId,
                    DeviceTypeId = DeviceType.Phone.Id,
                    PatientId = patientId
                }
            };

            SetupContext(devices);
        }

        [TestMethod]
        public void WhenCalled_WillReturnCorrespondingDeviceId()
        {
            var result = Repository.GetDeviceIdForPatient(patientId);

            Assert.AreEqual(deviceId, result);
        }

        [TestMethod]
        public void WhenNoDeviceHasThatPatientId_WillReturnNull()
        {
            var result = Repository.GetDeviceIdForPatient(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void WhenMultipleDevicesHasThatPatientId_WillReturnFirstDeviceId()
        {
            var expected = Guid.NewGuid();
            var devices = new[]
            {
                new Device
                {
                    Id = expected,
                    DeviceTypeId = DeviceType.Phone.Id,
                    PatientId = patientId
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceTypeId = DeviceType.Phone.Id,
                    PatientId = patientId
                }
            };
            SetupContext(devices);

            var result = Repository.GetDeviceIdForPatient(patientId);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void WhenPatientsDeviceIsNotHandheld_WillReturnNull()
        {
            var devices = new[]
            {
                new Device
                {
                    Id = deviceId,
                    PatientId = patientId,
                    DeviceTypeId = DeviceType.Tablet.Id,
                }
            };
            SetupContext(devices);

            var result = Repository.GetDeviceIdForPatient(patientId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void WhenPatientsDeviceIsBYOD_WillReturnDevice()
        {
            var devices = new[]
            {
                new Device
                {
                    Id = deviceId,
                    PatientId = patientId,
                    DeviceTypeId = DeviceType.BYOD.Id,
                }
            };
            SetupContext(devices);

            var result = Repository.GetDeviceIdForPatient(patientId);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenNoDevicesExist_WillReturnNull()
        {
            SetupContext(Enumerable.Empty<Device>());

            var result = Repository.GetDeviceIdForPatient(patientId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void WhenContextDevicesThrowsException_WillPassSameException()
        {
            var exception = new Exception();
            Context.Setup(ctx => ctx.Devices)
                .Throws(exception);

            YAssert.ThrowsSameException(exception, () => Repository.GetDeviceIdForPatient(patientId));
        }
    }
}