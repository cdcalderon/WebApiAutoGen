using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareVersionRepositoryTests
{
    [TestClass]
    public class SoftwareVersionRepositoryGetAllSotwareVersionsTest : SoftwareVersionRepositoryTestBase
    {
        private FakeDbSet<SoftwareVersion> _dataSet;
        private SoftwareVersion _dataTestA;
        private SoftwareVersion _dataTestB;
        private string _packagePath;
        private Guid _platformTypeId;
        private Guid _softwareVersionId;
        private string _versionNumber;

        [TestInitialize]
        public void TestInitialize()
        {
            _softwareVersionId = Guid.NewGuid();
            _versionNumber = "1.0.0.0";
            _packagePath = "http://testPath/YPrime_DevelopServices/Packages/YPrime.eCOA.Droid_1.0.0.0.apk";
            _platformTypeId = new Guid();
            _dataTestA = new SoftwareVersion
            {
                Id = _softwareVersionId,
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };
            _dataTestB = new SoftwareVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = _versionNumber,
                PackagePath = _packagePath,
                PlatformTypeId = _platformTypeId
            };
            SetupContext(new[] {_dataTestA, _dataTestB});
        }

        [TestMethod]
        public void WithExistingRecordsInDataBase_WillReturnAllSoftwareVersions()
        {
            var result = Repository.GetAllSoftwareVersions();
            Assert.IsTrue(result.Contains(_dataTestA));
            Assert.IsTrue(result.Contains(_dataTestB));
        }

        [TestMethod]
        public void WhenCalledWillReturnCollectionType_List()
        {
            var result = Repository.GetAllSoftwareVersions();
            Assert.IsTrue(result.GetType() == typeof(List<SoftwareVersion>));
        }

        [TestMethod]
        public void WithNoExistingRecordsInDataBase_WillReturnEmptyList()
        {
            SetupContext(Enumerable.Empty<SoftwareVersion>());
            var result = Repository.GetAllSoftwareVersions();
            Assert.IsFalse(result.Any());
        }

        private void SetupContext(IEnumerable<SoftwareVersion> items)
        {
            _dataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_dataSet.Object);
        }
    }
}