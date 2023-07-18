using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Defaults;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryRemovePatientTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task RemovePatient_HasDiariesDoesNotDeleteTest()
        {
            var testPatientId = Guid.NewGuid();
            var testPatient = new Patient 
            { 
                Id = testPatientId,
                DiaryEntries = new List<DiaryEntry> { new DiaryEntry { Id = Guid.NewGuid() } }
            };

            BasePatients.Add(testPatient);

            PatientDataset
                .Setup(q => q.Remove(testPatient))
                .Returns((Patient removedPatient) =>
                {
                    BasePatients.Remove(removedPatient);

                    return removedPatient;
                });

            await Repository.RemovePatient(testPatientId);

            MockContext.Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RemovePatient_PatientInitializedDoesNotDeleteTest()
        {
            var testPatientId = Guid.NewGuid();
            var testPatient = new Patient
            {
                Id = testPatientId,
                PatientStatusTypeId = PatientStatusTypes.Screened.Id
            };

            BasePatients.Add(testPatient);

            PatientDataset
                .Setup(q => q.Remove(testPatient))
                .Returns((Patient removedPatient) =>
                {
                    BasePatients.Remove(removedPatient);

                    return removedPatient;
                });

            await Repository.RemovePatient(testPatientId);

            MockContext.Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RemovePatient_PatientIsRemovedTest()
        {
            var testPatientId = Guid.NewGuid();
            var testPatient = new Patient
            {
                Id = testPatientId,
                PatientStatusTypeId = PatientStatusTypes.Enrolled.Id
            };

            BasePatients.Add(testPatient);

            PatientDataset
                .Setup(q => q.Remove(testPatient))
                .Returns((Patient removedPatient) =>
                {
                    BasePatients.Remove(removedPatient);

                    return removedPatient;
                });

            var patient = BasePatients.FirstOrDefault(q => q.Id == testPatientId);

            Assert.IsNotNull(patient);
            Assert.AreEqual(patient.Id, testPatientId);

            await Repository.RemovePatient(testPatientId);

            patient = BasePatients.FirstOrDefault(q => q.Id == testPatientId);

            Assert.IsNull(patient);

            MockContext.Verify(c => c.SaveChangesAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
