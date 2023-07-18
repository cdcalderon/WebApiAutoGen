using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientVisitRepositoryTests
{
    [TestClass]
    public class PatientVisitRepositoryProjectPatientVisitScheduleTests : PatientVisitRepositoryTestBase
    {
        [TestMethod]
        public async Task PatientVisitServiceProjectPatientVisitScheduleTest_WithExistingPatient()
        {
            var addedPatientVisits = new List<PatientVisit>();

            PatientVisitDataset
                .Setup(d => d.Add(It.IsAny<PatientVisit>()))
                .Callback((PatientVisit patientVisit) =>
                {
                    addedPatientVisits.Add(patientVisit);
                });

            await Repository.ProjectPatientVisitSchedule(BasePatientId);

            MockContext.Verify(c => c.Patients, Times.Once);
            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);

            Assert.IsTrue(addedPatientVisits.All(pv => !pv.IsDirty));
        }

        [TestMethod]
        public async Task PatientVisitServiceProjectPatientVisitScheduleTest_WithNullPatient()
        {
            await Repository.ProjectPatientVisitSchedule(Guid.NewGuid());

            MockContext.Verify(c => c.Patients, Times.Once);
            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task PatientVisitServiceProjectPatientVisitScheduleTest_VisitDateTimeZone()
        {
            var insertedPatientVisits = new List<PatientVisit>();

            const int enrollDateYear = 2020;
            const int enrollDateMonth = 7;
            const int enrollDateDay = 9;
            const int enrollDateHour = 2;
            const int enrollDateMinute = 33;
            const int enrollDateSecond = 15;
            const int enrollDateHourOffset = 9;

            var enrollDateInGmtPlus9 = new DateTimeOffset(
                enrollDateYear,
                enrollDateMonth,
                enrollDateDay,
                enrollDateHour,
                enrollDateMinute,
                enrollDateSecond,
                new TimeSpan(enrollDateHourOffset, 0, 0));

            var testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                EnrolledDate = enrollDateInGmtPlus9
            };

            SetupPatients(new List<Patient> { testPatient });

            PatientVisitDataset
                .Setup(ds => ds.AddRange(It.IsAny<IEnumerable<PatientVisit>>()))
                .Callback((IEnumerable<PatientVisit> inserted) =>
                {
                    insertedPatientVisits.AddRange(inserted);
                });

            await Repository.ProjectPatientVisitSchedule(testPatient.Id);

            MockContext.Verify(c => c.Patients, Times.Once);
            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);

            Assert.AreEqual(BaseVisits.Count, insertedPatientVisits.Count);

            foreach (var insertedPatientVisit in insertedPatientVisits)
            {
                Assert.AreEqual(enrollDateHourOffset, insertedPatientVisit.ProjectedDate.Offset.Hours);
                Assert.IsFalse(insertedPatientVisit.IsDirty);
            }
        }
    }
}