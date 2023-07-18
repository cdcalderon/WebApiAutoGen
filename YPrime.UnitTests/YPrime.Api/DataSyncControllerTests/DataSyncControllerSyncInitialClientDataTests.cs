using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.API.Controllers;
using YPrime.API.Models;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models.Models.DataSync;

namespace YPrime.UnitTests.YPrime.Api.DataSyncControllerTests
{
    [TestClass]
    public class DataSyncControllerSyncInitialClientDataTests : DataSyncControllerTestBase
    {
        [TestMethod]
        public async Task SyncInitialClientDataTest()
        {
            var testSyncResponse = new DataSyncResponse
            {
                Message = "Sync response"
            };

            var deviceType = DeviceType.Phone;

            var testRequest = new DataSyncRequest
            {
                SoftwareVersion = Guid.NewGuid().ToString(),
                ConfigurationVersion = Guid.NewGuid().ToString(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id,
                ClientEntries = new List<dynamic>(),
            };

            MockDataSyncRepository
                .Setup(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == null),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()))
                .ReturnsAsync(testSyncResponse);

            var controller = GetController();

            var result = await controller.SyncInitialClientData(testRequest);

            Assert.AreEqual(testSyncResponse, result);

            MockDataSyncRepository
                .Verify(r => r.LogDeviceSyncData(
                    It.Is<string>(sv => sv == testRequest.ConfigurationVersion),
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(sa => sa == nameof(DataSyncController.SyncInitialClientData)),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()), Times.Once);

            MockDataSyncRepository
                .Verify(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == null),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()), Times.Once);
        }

        [TestMethod]
        public async Task SyncInitialClientDataNullSiteIdTest()
        {
            var testSyncResponse = new DataSyncResponse
            {
                Message = "Sync response"
            };

            var deviceType = DeviceType.Phone;

            var testRequest = new DataSyncRequest
            {
                SoftwareVersion = Guid.NewGuid().ToString(),
                ConfigurationVersion = Guid.NewGuid().ToString(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = null,
                DeviceId = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id,
                ClientEntries = new List<dynamic>(),
            };

            MockDataSyncRepository
                .Setup(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == null),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()))
                .ReturnsAsync(testSyncResponse);

            var controller = GetController();

            var result = await controller.SyncInitialClientData(testRequest);

            Assert.AreEqual(testSyncResponse, result);

            MockDataSyncRepository
                .Verify(r => r.LogDeviceSyncData(
                    It.Is<string>(sv => sv == testRequest.ConfigurationVersion),
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(sa => sa == nameof(DataSyncController.SyncInitialClientData)),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()), Times.Once);

            MockDataSyncRepository
                .Verify(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == null),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()), Times.Once);
        }

        [TestMethod]
        public async Task SyncInitialClientDataWithPatientIdTest()
        {
            var testSyncResponse = new DataSyncResponse
            {
                Message = "Sync response"
            };

            var deviceType = DeviceType.Phone;

            var testRequest = new DataSyncRequest
            {
                SoftwareVersion = Guid.NewGuid().ToString(),
                ConfigurationVersion = Guid.NewGuid().ToString(),
                AssetTag = Guid.NewGuid().ToString(),
                SiteId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                DeviceTypeId = deviceType.Id,
                ClientEntries = new List<dynamic>(),
                PatientId = Guid.NewGuid()
            };

            MockDataSyncRepository
                .Setup(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == testRequest.PatientId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()))
                .ReturnsAsync(testSyncResponse);

            var controller = GetController();

            var result = await controller.SyncInitialClientData(testRequest);

            Assert.AreEqual(testSyncResponse, result);

            MockDataSyncRepository
                .Verify(r => r.LogDeviceSyncData(
                    It.Is<string>(sv => sv == testRequest.ConfigurationVersion),
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(sa => sa == nameof(DataSyncController.SyncInitialClientData)),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()), Times.Once);

            MockDataSyncRepository
                .Verify(r => r.SyncInitialData(
                    It.Is<Guid>(did => did == testRequest.DeviceId),
                    It.Is<Guid>(did => did == testRequest.DeviceTypeId),
                    It.Is<Guid?>(s => s == testRequest.SiteId),
                    It.Is<Guid?>(pid => pid == testRequest.PatientId),
                    It.Is<string>(sv => sv == testRequest.SoftwareVersion),
                    It.Is<string>(cv => cv == testRequest.ConfigurationVersion),
                    It.IsAny<List<dynamic>>()), Times.Once);
        }
    }
}
