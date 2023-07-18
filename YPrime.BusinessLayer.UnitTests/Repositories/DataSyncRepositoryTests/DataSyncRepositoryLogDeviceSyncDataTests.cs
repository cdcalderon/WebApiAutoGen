using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryLogDeviceSyncDataTests : DataSyncRepositoryTestBase
    {
        private SoftwareVersion _softwareVersion;
        private Device _device;
        private List<SyncLog> _syncLogs;

        [TestInitialize]
        public void TestInitialize()
        {
            _syncLogs = new List<SyncLog>();

            _softwareVersion = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = "1.2.3.4"
            };

            _device = new Device
            {
                Id = Guid.NewGuid(),
            };

            var syncLogDataSet = new FakeDbSet<SyncLog>(_syncLogs);

            syncLogDataSet
                .Setup(ds => ds.Add(It.IsAny<SyncLog>()))
                .Callback((SyncLog addedLog) =>
                {
                    _syncLogs.Add(addedLog);
                });

            Context
                .Setup(c => c.SyncLogs)
                .Returns(syncLogDataSet.Object);

            var versionDataset = new FakeDbSet<SoftwareVersion>(new List<SoftwareVersion> { _softwareVersion });

            Context
                .Setup(c => c.SoftwareVersions)
                .Returns(versionDataset.Object);

            var deviceDataset = new FakeDbSet<Device>(new List<Device> { _device });

            Context
                .Setup(c => c.Devices)
                .Returns(deviceDataset.Object);
        }

        [TestMethod]
        public void LogDeviceSyncDataTest()
        {
            const string syncAction = "Syncing";
            dynamic clientEntries = new { id = Guid.NewGuid() };

            var expectedClientEntries = JsonConvert.SerializeObject(clientEntries);

            Repository.LogDeviceSyncData(
                "1.0",
                _device.Id,
                _softwareVersion.VersionNumber,
                syncAction,
                true,
                "OK",
                clientEntries);

            var addedLog = _syncLogs.First(l => l.DeviceId == _device.Id);

            Assert.AreEqual(_softwareVersion.Id, addedLog.SoftwareVersionId);
            Assert.IsTrue(addedLog.SyncSuccess);
            Assert.AreEqual(expectedClientEntries, addedLog.SyncData);

            Context
                .Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);
        }
    }
}
