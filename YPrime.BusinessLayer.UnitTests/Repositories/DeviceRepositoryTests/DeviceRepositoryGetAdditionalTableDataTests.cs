using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    [TestClass]
    public class DeviceRepositoryGetAdditionalTableDataTests : LegacyTestBase
    {
        private readonly Guid _deviceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private Mock<MockableDbSetWithExtensions<AdditionalTablesToSync>> _additionalTablesToSync;
        private Mock<IStudyDbContext> _dbContext;
        private Mock<ISoftwareReleaseRepository> _softwareReleaseRepository;
        private Mock<IConfigurationVersionService> _configurationVersionService;
        private Mock<MockableDbSetWithExtensions<Device>> _devices;

        [TestInitialize]
        public void Initialize()
        {
            _devices = CreateDbSetMock(new List<Device>
            {
                new Device
                {
                    Id = _deviceId
                }
            });

            _additionalTablesToSync = CreateDbSetMock(new List<AdditionalTablesToSync>
            {
                new AdditionalTablesToSync
                {
                    TableName = "DoTable1Sync",
                    DoSync = true
                },
                new AdditionalTablesToSync
                {
                    TableName = "DoNotDoTableSync",
                    DoSync = false
                }
            });

            _softwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            _configurationVersionService = new Mock<IConfigurationVersionService>();
            _dbContext = new Mock<IStudyDbContext>();
            _dbContext.Setup(x => x.AdditionalTablesToSync).Returns(_additionalTablesToSync.Object);
            _dbContext.Setup(x => x.Devices).Returns(_devices.Object);
            //_container.RegisterInstance<IStudyDbContext>(_dbContext.Object);
        }

        [TestMethod]
        public void GetAdditionalTableDataTest_NoUpdatesNeeded()
        {
            var clientEntry = JsonConvert.DeserializeObject<dynamic>("{ 'TableName': 'StudyCustom', 'Rows':[] }");
            var clientEntries = new List<dynamic> {clientEntry};

            _devices.Object.Single(x => x.Id == _deviceId).DoAdditionalTableSync = false;
            var deviceRepository = new DeviceRepository(_dbContext.Object, null, _softwareReleaseRepository.Object, _configurationVersionService.Object);

            var result = deviceRepository.GetAdditionalTableData(_deviceId, clientEntries);

            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public void GetAdditionalTableDataTest_UpdatesNeeded()
        {
            var clientEntry = JsonConvert.DeserializeObject<dynamic>("{ 'TableName': 'StudyCustom', 'Rows':[] }");
            var clientEntries = new List<dynamic> {clientEntry};

            _devices.Object.Single(x => x.Id == _deviceId).DoAdditionalTableSync = true;
            var deviceRepository = new DeviceRepository(_dbContext.Object, null, _softwareReleaseRepository.Object, _configurationVersionService.Object);

            var result = deviceRepository.GetAdditionalTableData(_deviceId, clientEntries);

            Assert.IsTrue(result.Count == 2);
        }

        [TestMethod]
        public void GetAdditionalTableDataTest_UpdatesNeededDuplicate()
        {
            var clientEntry = JsonConvert.DeserializeObject<dynamic>("{ 'TableName': 'DoTable1Sync', 'Rows':[] }");
            var clientEntries = new List<dynamic> {clientEntry};

            _devices.Object.Single(x => x.Id == _deviceId).DoAdditionalTableSync = true;
            var deviceRepository = new DeviceRepository(_dbContext.Object, null, _softwareReleaseRepository.Object, _configurationVersionService.Object);

            var result = deviceRepository.GetAdditionalTableData(_deviceId, clientEntries);

            Assert.IsTrue(result.Count == 1);
        }
    }
}