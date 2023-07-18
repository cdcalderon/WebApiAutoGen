using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareReleaseRepositoryTests
{
    [TestClass]
    public class SoftwareReleaseRepositoryDeactivateSoftwareReleaseTests : SoftwareReleaseRepositoryTestBase
    {
        private FakeDbSet<SoftwareRelease> _dataSet;
        private SoftwareRelease _model;
        private Guid releaseId;

        [TestInitialize]
        public void TestInitialize()
        {
            releaseId = Guid.NewGuid();
            _model = new SoftwareRelease
            {
                Id = releaseId,
                Name = "test",
                SoftwareVersionId = new Guid(),
                DateCreated = new DateTimeOffset(),
                IsActive = true,
                Required = true,
                ConfigurationId = new Guid(),
                StudyWide = true
            };

            SetupContext(new[] {_model});
        }

        [TestMethod]
        public void WhenCalledWillNotUpdateSoftwareReleaseModelIfRecordNotFound()
        {
            Repository.DeactivateSoftwareRelease(Guid.Empty);

            Assert.IsTrue(_model.IsActive);
        }

        [TestMethod]
        public void WhenCalledWillSetSoftwareReleaseIsActiveFalse()
        {
            Repository.DeactivateSoftwareRelease(_model.Id);

            Assert.IsFalse(_model.IsActive);
        }

        private void SetupContext(IEnumerable<SoftwareRelease> items)
        {
            _dataSet = new FakeDbSet<SoftwareRelease>(items);
            Context.Setup(ctx => ctx.SoftwareReleases)
                .Returns(_dataSet.Object);
        }
    }
}