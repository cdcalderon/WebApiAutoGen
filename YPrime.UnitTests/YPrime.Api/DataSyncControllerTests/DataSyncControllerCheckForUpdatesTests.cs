using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.API.Controllers;
using YPrime.API.Models;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models.Models.DataSync;

namespace YPrime.UnitTests.YPrime.Api.DataSyncControllerTests
{
    [TestClass]
    public class DataSyncControllerCheckForUpdatesTests : DataSyncControllerTestBase
    {
        [TestMethod]
        public void CheckForUpdatesTest()
        {
            var testSyncResponse = new CheckForUpdateResponse();

            var deviceType = DeviceType.Phone;

            var testRequest = new CheckForUpdatesRequest
            {
                SoftwareVersion = Guid.NewGuid().ToString(),
                ConfigVersion = "1.0",
                AssetTag = "YP-Test",
                SiteId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id,
            };

            MockDataSyncRepository
                .Setup(r => r.CheckForUpdates(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigVersion)))
                .Returns(testSyncResponse);

            var controller = GetController();

            var result = controller.CheckForUpdates(testRequest);

            Assert.AreEqual(testSyncResponse, result);

            MockDataSyncRepository
                .Verify(r => r.CreateDeviceIfNotExists(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(dtid => dtid == deviceType.Id),
                    It.Is<Guid>(sid => sid == testRequest.SiteId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(at => at == testRequest.AssetTag)), Times.Once);

            MockDataSyncRepository
                .Verify(r => r.LogDeviceSyncData(
                    It.Is<string>(sv => sv == testRequest.ConfigVersion),
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(sa => sa == nameof(DataSyncController.CheckForUpdates)),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void CheckForUpdatesIsByodTest()
        {
            var testSyncResponse = new CheckForUpdateResponse();

            var deviceType = DeviceType.BYOD;

            var testRequest = new CheckForUpdatesRequest
            {
                SoftwareVersion = Guid.NewGuid().ToString(),
                ConfigVersion = "1.0",
                AssetTag = "YP-Test",
                SiteId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id,
            };

            MockDataSyncRepository
                .Setup(r => r.CheckForUpdates(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigVersion)))
                .Returns(testSyncResponse);

            var controller = GetController();

            var result = controller.CheckForUpdates(testRequest);

            Assert.AreEqual(testSyncResponse, result);

            MockDataSyncRepository
                .Verify(r => r.CreateDeviceIfNotExists(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(dtid => dtid == deviceType.Id),
                    It.Is<Guid>(sid => sid == testRequest.SiteId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(at => at == testRequest.AssetTag)), Times.Never);

            MockDataSyncRepository
                .Verify(r => r.LogDeviceSyncData(
                    It.Is<string>(sv => sv == testRequest.ConfigVersion),
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(sa => sa == nameof(DataSyncController.CheckForUpdates)),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()), Times.Once);
        }
    }
}
