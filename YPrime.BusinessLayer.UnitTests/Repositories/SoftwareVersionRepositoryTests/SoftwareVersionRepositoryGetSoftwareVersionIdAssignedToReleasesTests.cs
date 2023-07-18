using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareVersionRepositoryTests
{
    [TestClass]
    public class
        SoftwareVersionRepositoryGetSoftwareVersionIdAssignedToReleasesTests : SoftwareVersionRepositoryTestBase
    {
        private FakeDbSet<SoftwareVersion> _dataSet;
        private string _packagePath;
        private Guid _platformTypeId;
        private SoftwareRelease _softwareReleaseA;
        private SoftwareRelease _softwareReleaseB;
        private SoftwareRelease _softwareReleaseC;
        private SoftwareVersion _SoftwareVersionA;
        private SoftwareVersion _SoftwareVersionB;
        private Guid _softwareVersionIdA;
        private Guid _softwareVersionIdB;
        private Guid _softwareVersionIdC;
        private FakeDbSet<SoftwareRelease> _srData;
        private string _versionNumber;

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersionIdA = Guid.NewGuid();
            _softwareVersionIdB = Guid.NewGuid();
            _versionNumber = "1.2.3.4";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();

            _SoftwareVersionA = new SoftwareVersion
            {
                Id = _softwareVersionIdA,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };
            _SoftwareVersionB = new SoftwareVersion
            {
                Id = _softwareVersionIdB,
                VersionNumber = "4.5.6.7",
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };

            _softwareReleaseA = new SoftwareRelease
            {
                Id = new Guid(),
                Name = "test1",
                SoftwareVersionId = _softwareVersionIdA,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                SoftwareVersion = _SoftwareVersionA
            };
            _softwareReleaseB = new SoftwareRelease
            {
                Id = new Guid(),
                Name = "test2",
                SoftwareVersionId = _softwareVersionIdB,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                SoftwareVersion = _SoftwareVersionB
            };
            _softwareReleaseC = new SoftwareRelease
            {
                Id = new Guid(),
                Name = "test3",
                SoftwareVersionId = _softwareVersionIdB,
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true,
                SoftwareVersion = _SoftwareVersionB
            };

            SetupSoftwareReleaseContext(new[] {_softwareReleaseA, _softwareReleaseB});
            SetupContext(new[] {_SoftwareVersionA, _SoftwareVersionB});
        }

        [TestMethod]
        public void WhenCalledWillReturnSoftwareVersionIdList()
        {
            var result = Repository.GetSoftwareVersionIdsAssignedToReleases();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Any());
        }

        [TestMethod]
        public void WithNoExistingSoftwareRelease_WillReturnEmptyList()
        {
            SetupSoftwareReleaseContext(Enumerable.Empty<SoftwareRelease>());
            var result = Repository.GetSoftwareVersionIdsAssignedToReleases();

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void WithDuplicateSoftwareReleaseVersions_WillReturnDistinctVersionList()
        {
            SetupSoftwareReleaseContext(new[] {_softwareReleaseB, _softwareReleaseC});
            var result = Repository.GetSoftwareVersionIdsAssignedToReleases();

            Assert.IsTrue(result.Count() == 1);
        }

        private void SetupContext(IEnumerable<SoftwareVersion> items)
        {
            _dataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_dataSet.Object);
        }

        private void SetupSoftwareReleaseContext(IEnumerable<SoftwareRelease> items)
        {
            _srData = new FakeDbSet<SoftwareRelease>(items);
            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_srData.Object);
        }
    }
}