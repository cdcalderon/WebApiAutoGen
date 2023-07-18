using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryInsertCaregiverTests : PatientRepositoryTestBase
    {
        protected Guid CareGiverPatientId = Guid.NewGuid();

        [TestInitialize]
        public void Init()
        {
            BasePatients.Add(new Patient() { Id = CareGiverPatientId, PatientNumber = "805" });
        }

        [TestMethod]
        public async Task PatientRepositoryInsertCaregiverTest()
        {
            CareGiver passedInCaregiver = null;

            CareGiverDataset
                .Setup(ds => ds.Add(It.IsAny<CareGiver>()))
                .Callback((CareGiver cg) =>
                {
                    passedInCaregiver = cg;
                });

            var result = await Repository.InsertCareGiver(CareGiverPatientId, BaseCareGiverTypeId);

            Assert.IsTrue(result);

            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(passedInCaregiver);
            Assert.IsFalse(passedInCaregiver.IsDirty);

            MockContext.ResetCalls();
        }

        [TestMethod]
        public async Task PatientRepositoryInsertUpdatePatientInsertInvalidCareGiverTypeTest()
        {
            var result = await Repository.InsertCareGiver(CareGiverPatientId, Guid.Empty);

            Assert.IsFalse(result);

            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Never);
            MockContext.ResetCalls();
        }
    }
}
