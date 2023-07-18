using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SyncLogRepositoryTests
{
    [TestClass]
    public class SyncLogRepositoryTest
    {
        private readonly string _assetTag1 = "TestAssetTag";
        private readonly string _assetTag2 = "TestSyncAssetTag";
        private Mock<IStudyDbContext> _context;
        private Mock<IConfigurationVersionService> _configurationVersionService;
        private FakeDbSet<SyncLog> _dataSet;
        private readonly DateTimeOffset _dateTimeOffset1 = DateTimeOffset.Now;
        private readonly DateTimeOffset _dateTimeOffset2 = DateTimeOffset.UtcNow;
        private SyncLog _syncLog;
        private SyncLogRepository _syncLogRepository;
        private readonly Guid firstDevice = Guid.NewGuid();
        private readonly Guid secondDevice = Guid.NewGuid();

        [TestInitialize]
        public void TestInit()
        {
            _context = new Mock<IStudyDbContext>();
            _configurationVersionService = new Mock<IConfigurationVersionService>();
            _syncLogRepository = new SyncLogRepository(_context.Object, _configurationVersionService.Object);
            SetupContext(new[]
            {
                new SyncLog
                {
                    DeviceId = firstDevice,
                    SyncAction = "CheckForUpdates",
                    SyncDate = _dateTimeOffset1,
                    Device = new Device
                    {
                        AssetTag = _assetTag1
                    },
                    SyncSuccess = true
                },
                new SyncLog
                {
                    DeviceId = secondDevice,
                    SyncAction = "SyncClientData",
                    SyncDate = _dateTimeOffset2,
                    Device = new Device
                    {
                        AssetTag = _assetTag2
                    },
                    SyncSuccess = true
                }
            });
        }

        private void SetupContext(IEnumerable<SyncLog> items)
        {
            _dataSet = new FakeDbSet<SyncLog>(items);
            _context.Setup(ctx => ctx.SyncLogs)
                .Returns(_dataSet.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void GetSyncDate()
        {
            var result = _syncLogRepository.GetLastSyncDateFromLogsByDevice(_assetTag2);

            Assert.AreEqual(result, _dateTimeOffset2);
        }

        [TestMethod]
        public void GetSyncDateNull()
        {
            var result = _syncLogRepository.GetLastSyncDateFromLogsByDevice(_assetTag1);

            Assert.IsTrue(result == null);
        }
    }
}