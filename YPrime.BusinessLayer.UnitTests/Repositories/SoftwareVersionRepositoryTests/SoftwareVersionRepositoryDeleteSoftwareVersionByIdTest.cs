using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareVersionRepositoryTests
{
    [TestClass]
    public class SoftwareVersionRepositoryDeleteSoftwareVersionByIdTest : SoftwareVersionRepositoryTestBase
    {
        private FakeDbSet<SoftwareVersion> _dataSet;
        private SoftwareVersion _dataTestA;
        private SoftwareVersion _dataTestB;
        private SoftwareVersion _dataTestNew;
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

            SetupContext(new[] {_dataTestA});
        }

        [TestMethod]
        public void WhenCalledWithNoExisitingVersionWillThrowException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                Repository.DeleteSoftwareVersionById(_dataTestB.Id));
        }

        [TestMethod]
        public void WhenCalledWithDuplicateVersionIdsWillThrowException()
        {
            _dataTestB.Id = _dataTestA.Id;
            SetupContext(new[] {_dataTestA, _dataTestB});
            Assert.ThrowsException<InvalidOperationException>(() =>
                Repository.DeleteSoftwareVersionById(_dataTestB.Id));
        }

        private void SetupContext(IEnumerable<SoftwareVersion> items)
        {
            _dataSet = new FakeDbSet<SoftwareVersion>(items);
            Context.Setup(ctx => ctx.SoftwareVersions)
                .Returns(_dataSet.Object);
        }
    }
}