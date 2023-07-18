using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.UnitTests.TestObjects;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.AnalyticsRepositoryTests
{
    [TestClass]
    public class AnalyticsRepositoryTests : RepositoryTestBase
    {
        protected FakeDbSet<AnalyticsReference> BIAnalyticsDataset;
        protected FakeDbSet<AnalyticsReferenceStudyRole> BIAnalyticsStudyRoleDataset;
        protected Mock<IStudyDbContext> Context;
        protected AnalyticsRepository Repository;

        private readonly Guid TestAnalyticId = Guid.NewGuid();
        private readonly Guid TestAnalyticTwoId = Guid.NewGuid();
        private readonly Guid TestStudyRoleId = Guid.NewGuid();

        private List<AnalyticsReference> BaseBIAnalytics;
        private List<AnalyticsReferenceStudyRole> BIAnalyticsStudyRoleList;

        [TestMethod]
        public void AddAnalyticsReferenceDuplicate_ThrowsException()
        {
            var testAnalytic = BaseBIAnalytics.First();

            Assert.ThrowsException<DuplicateAnalyticsException>(() => Repository.AddAnalyticsReference(testAnalytic));
        }

        [TestMethod]
        public void AddAnalyticsReferenceNewReference_ReturnsNewId()
        {
            var newAnalytic = new AnalyticsReference { Id = Guid.NewGuid(), InternalName = "GHI", DisplayName = "GHI Test Name 3" };

            BIAnalyticsDataset.Setup(q => q.Add(It.IsAny<AnalyticsReference>())).Returns(newAnalytic);

            var result = Repository.AddAnalyticsReference(newAnalytic);

            Assert.AreEqual(newAnalytic.Id, result);
        }

        [TestMethod]
        public void GetAllAnalyticsReferences_ReturnsOrderedReferences()
        {
            var result = Repository.GetAllAnalyticsReferences();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result[0].Id, TestAnalyticTwoId);
            Assert.AreEqual(result[1].Id, TestAnalyticId);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new Mock<IStudyDbContext>();
            Repository = new AnalyticsRepository(Context.Object);

            BaseBIAnalytics = new List<AnalyticsReference>()
            {
                new AnalyticsReference() {
                    Id = TestAnalyticId,
                    InternalName = "DEF",
                    DisplayName = "DEF Test Name 1"
                },
                new AnalyticsReference()
                {
                    Id = TestAnalyticTwoId,
                    InternalName = "ABC",
                    DisplayName = "ABC Test Name 2"
                }
            };

            BIAnalyticsDataset = new FakeDbSet<AnalyticsReference>(BaseBIAnalytics);
            Context.Setup(c => c.AnalyticsReferences).Returns(BIAnalyticsDataset.Object);

            BIAnalyticsStudyRoleList = new List<AnalyticsReferenceStudyRole>()
            {
                new AnalyticsReferenceStudyRole() {
                    AnalyticsReferenceId = TestAnalyticId,
                    StudyRoleId = TestStudyRoleId
                }
            };
            BIAnalyticsStudyRoleDataset = new FakeDbSet<AnalyticsReferenceStudyRole>(BIAnalyticsStudyRoleList);
            Context.Setup(c => c.AnalyticsReferenceStudyRoles).Returns(BIAnalyticsStudyRoleDataset.Object);

            BIAnalyticsStudyRoleDataset
                .Setup(asr => asr.Add(It.IsAny<AnalyticsReferenceStudyRole>()))
                .Callback((AnalyticsReferenceStudyRole addedars) =>
                {
                    BIAnalyticsStudyRoleList.Add(addedars);
                });

            BIAnalyticsStudyRoleDataset
                .Setup(asr => asr.Remove(It.IsAny<AnalyticsReferenceStudyRole>()))
                .Callback((AnalyticsReferenceStudyRole removedars) =>
                {
                    BIAnalyticsStudyRoleList.Remove(removedars);
                });
        }
    }
}