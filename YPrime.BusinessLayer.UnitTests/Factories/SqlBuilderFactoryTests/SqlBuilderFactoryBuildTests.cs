using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.BusinessLayer.DataSync.Factories;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.SyncSQLBuilders;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Factories.SqlBuilderFactoryTests
{
    [TestClass]
    public class SqlBuilderFactoryBuildTests : LegacyTestBase
    {
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly SqlBuilderFactory SqlBuilderFactory;
        private Guid? _patientId;
        private Guid _deviceId;

        public SqlBuilderFactoryBuildTests()
        {
            Context = new Mock<IStudyDbContext>();
            SqlBuilderFactory = new SqlBuilderFactory(Context.Object);
        }

        [TestInitialize]
        public void InitializeTests()
        {
            _patientId = Guid.NewGuid();
            _deviceId = Guid.NewGuid();
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsBYODDeviceType()
        {
            var deviceTypeId = DeviceType.BYOD.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(_patientId, _deviceId, deviceTypeId, true);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderBYOD);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsBYODDeviceTypeInitialSync()
        {
            var deviceTypeId = DeviceType.BYOD.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(null, _deviceId, deviceTypeId, true);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderBYOD);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsTabletDeviceType()
        {
            var deviceTypeId = DeviceType.Tablet.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(_patientId, _deviceId, deviceTypeId, true);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderSite);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceTypeInitialSync()
        {
            var deviceTypeId = DeviceType.Phone.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(null, _deviceId, deviceTypeId, true);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderPatientInitial);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceTypeInitialSyncWithPatient()
        {
            var deviceTypeId = DeviceType.Phone.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(_patientId, _deviceId, deviceTypeId, true);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderPatient);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceTypeIncrementalSync()
        {
            var deviceTypeId = DeviceType.Phone.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(_patientId, _deviceId, deviceTypeId, false);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderPatient);
        }

        [TestMethod]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceTypeIncrementalSyncNoPatient()
        {
            var deviceTypeId = DeviceType.Phone.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = null,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            var builder = SqlBuilderFactory.Build(null, _deviceId, deviceTypeId, false);

            Assert.IsNotNull(builder);
            Assert.IsTrue(builder is SQLBuilderPatientInitial);
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceTypeNotFoundException), "Device type not found")]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceTypeNotFound()
        {
            var deviceTypeId = Guid.NewGuid();
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            SqlBuilderFactory.Build(_patientId, _deviceId, deviceTypeId, false);
        }

        [TestMethod]
        [ExpectedException(typeof(PatientNotFoundException), "Patient Id not found")]
        public void SqlBuilderFactoryBuildTestsPhonePatientNotFound()
        {
            var deviceTypeId = DeviceType.BYOD.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            SqlBuilderFactory.Build(null, Guid.NewGuid(), deviceTypeId, true);
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceNotFoundException), "Device not found")]
        public void SqlBuilderFactoryBuildTestsTabletDeviceNotFound()
        {
            var deviceTypeId = DeviceType.Tablet.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            SqlBuilderFactory.Build(_patientId, Guid.NewGuid(), deviceTypeId, true);
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceNotFoundException), "Device not found")]
        public void SqlBuilderFactoryBuildTestsPhoneDeviceNotFound()
        {
            var deviceTypeId = DeviceType.Phone.Id;
            var deviceList = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId,
                    PatientId = _patientId,
                    DeviceTypeId = deviceTypeId
                }
            });

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceList.Object);

            SqlBuilderFactory.Build(null, Guid.NewGuid(), deviceTypeId, true);

        }
    }
}
