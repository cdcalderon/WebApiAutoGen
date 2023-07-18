using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryGetEligibleDevicesTests : SoftwareReleaseRepositoryTestBase
    {
        // --- Scenarios to test ---
        // -------------------------------------------------------
        //   New Release Has              |  Eligible for Release |
        // -------------------------------------------------------
        // Higher Software Version        |  Yes                  |
        // Higher Config Version          |                       |
        // -------------------------------------------------------
        // Higher Software Version        |  Yes                  |
        // Same Config Version            |                       |
        // -------------------------------------------------------
        // Higher Software Version        |  No                   |
        // Lower Config Version           |                       |
        // -------------------------------------------------------
        // Same Software Version          |  No                   |
        // Same Config Version            |                       |
        // -------------------------------------------------------
        // Lower Software Version         |  Yes                  |
        // Higher Config Version          |                       |
        // -------------------------------------------------------
        // Same Software Version          |  Yes                  |
        // Higher Config Version          |                       |
        // -------------------------------------------------------
        // Lower Software Version         |  No                   |
        // Lower Config Version           |                       |
        // -------------------------------------------------------
        // Lower Software Version         |  No                   |
        // Same Config Version            |                       |
        // -------------------------------------------------------
        // Same Software Version          |  No                   |
        // Lower Config Version           |                       |
        // -------------------------------------------------------

        private SoftwareRelease _softwareRelease;
        private SoftwareVersion _softwareVersion;

        private Device _device;

        private const string ExistingSoftwareVersionNumber = "1.32.2.6";
        private const string ExistingConfigVersionNumber = "2.0";

        private const string HigherSoftwareVersionNumber = "1.32.2.7";
        private const string HigherConfigVersionNumber = "3.0";

        private const string LowerSoftwareVersionNumber = "1.32.2.5";
        private const string LowerConfigVersionNumber = "1.0";

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = ExistingSoftwareVersionNumber
            };

            _softwareRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersionId = _softwareVersion.Id,
                SoftwareVersion = _softwareVersion,
                ConfigurationVersion = ExistingConfigVersionNumber
            };

            _device = new Device
            {
                Id = Guid.NewGuid(),
                SoftwareReleaseId = _softwareRelease.Id,
                SoftwareRelease = _softwareRelease
            };

            SetupDeviceContext(new[] { _device });
        }

        // Higher Software Version, Higher Config Version - Should Get Release
        [TestMethod]
        public void GetEligibleDevicesHigherSoftwareHigherConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = HigherSoftwareVersionNumber,
                },
                ConfigurationVersion = HigherConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(_device.Id, results.First().Id);
        }

        // Higher Software Version, Same Config Version - Should Get Release
        [TestMethod]
        public void GetEligibleDevicesHigherSoftwareSameConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = HigherSoftwareVersionNumber,
                },
                ConfigurationVersion = ExistingConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(_device.Id, results.First().Id);
        }

        // Higher Software Version, Lower Config Version - Should Not Get Release
        [TestMethod]
        public void GetEligibleDevicesHigherSoftwareLowerConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = HigherSoftwareVersionNumber,
                },
                ConfigurationVersion = LowerConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        // Same Software Version, Same Config Version - Should Not Get Release
        [TestMethod]
        public void GetEligibleDevicesSameSoftwareSameConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = ExistingSoftwareVersionNumber,
                },
                ConfigurationVersion = ExistingConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        // Lower Software Version, Higher Config Version - Should Get Release
        [TestMethod]
        public void GetEligibleDevicesLowerSoftwareHigherConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = LowerSoftwareVersionNumber,
                },
                ConfigurationVersion = HigherConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(_device.Id, results.First().Id);
        }

        // Same Software Version, Higher Config Version - Should Get Release
        [TestMethod]
        public void GetEligibleDevicesSameSoftwareHigherConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = ExistingSoftwareVersionNumber,
                },
                ConfigurationVersion = HigherConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(_device.Id, results.First().Id);
        }

        // Lower Software Version, Lower Config Version - Should Not Get Release
        [TestMethod]
        public void GetEligibleDevicesLowerSoftwareLowerConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = LowerSoftwareVersionNumber,
                },
                ConfigurationVersion = LowerConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        // Lower Software Version, Same Config Version - Should Not Get Release
        [TestMethod]
        public void GetEligibleDevicesLowerSoftwareSameConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = LowerSoftwareVersionNumber,
                },
                ConfigurationVersion = ExistingConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        // Same Software Version, Lower Config Version - Should Not Get Release
        [TestMethod]
        public void GetEligibleDevicesSameSoftwareLowerConfigTest()
        {
            var devices = new List<Device>
            {
                _device
            };

            var newRelease = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersion = new SoftwareVersion
                {
                    VersionNumber = ExistingSoftwareVersionNumber,
                },
                ConfigurationVersion = LowerConfigVersionNumber
            };

            SetupReleaseContext(new[] { newRelease });

            var results = Repository.GetEligibleDevices(
                devices,
                newRelease.Id);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        private void SetupDeviceContext(IEnumerable<Device> devices)
        {
            var deviceDataSet = new FakeDbSet<Device>(devices);

            Context.Setup(ctx => ctx.Devices)
                .Returns(deviceDataSet.Object);
        }

        private void SetupReleaseContext(IEnumerable<SoftwareRelease> releases)
        {
            var releaseDataSet = new FakeDbSet<SoftwareRelease>(releases);

            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(releaseDataSet.Object);
        }
    }
}
