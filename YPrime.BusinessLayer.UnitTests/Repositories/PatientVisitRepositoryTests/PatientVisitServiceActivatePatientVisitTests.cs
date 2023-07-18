using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientVisitRepositoryTests
{
    [TestClass]
    public class PatientVisitServiceActivatePatientVisitTests : PatientVisitRepositoryTestBase
    {
        [TestMethod]
        public async Task PatientVisitServiceActivatePatientVisitActivationDateTest()
        {    
            
            var insertedPatientVisits = new List<PatientVisit>();

            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var testPatientVisit = new PatientVisit
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now,
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id
            };

            /* Diary setup */
            SetupDiaryEntries(testPatient);
            SetupPatients(new List<Patient> { testPatient });
            SetupPatientVisits(new List<PatientVisit> { testPatientVisit });
            
            BasePatients.Add(testPatient);
            BaseVisits.Add(baseVisit1);

            var testPatientVisits = new List<PatientVisit>() {testPatientVisit };
            var testPatientVisitDataset = new FakeDbSet<PatientVisit>(testPatientVisits);
            MockContext.Setup(c => c.PatientVisits).Returns(testPatientVisitDataset.Object);

            await Repository.ActivatePatientVisit(testPatientVisit.Id, testPatient.Id);
            Assert.IsNotNull(testPatientVisit.ActivationDate);
            Assert.IsFalse(testPatientVisit.IsDirty);
        }

        [TestMethod]
        public async Task PatientVisitServiceActivatePatientVisitActivationDateNotUpdateTest()
        {
            var testActivationDate = DateTime.Now.AddDays(-1);

            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                VisitAvailableBusinessRuleTrueFalseIndicator = true,
            };

            var testPatientVisit = new PatientVisit
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = testActivationDate,
                ProjectedDate = DateTime.Now,
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id
            };

            /* Diary setup */
            SetupDiaryEntries(testPatient);
            SetupPatients(new List<Patient> { testPatient });
            SetupPatientVisits(new List<PatientVisit> { testPatientVisit });


            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel> { baseVisit1 });


            BasePatientVisits.Add(testPatientVisit);
            BasePatients.Add(testPatient);
            BaseVisits.Add(baseVisit1);

            await Repository.ActivatePatientVisit(testPatientVisit.Id, testPatient.Id);

            Assert.AreEqual(testActivationDate, testPatientVisit.ActivationDate);
            Assert.IsFalse(testPatientVisit.IsDirty);
        }

        [TestMethod]
        public async Task PatientVisitServiceActivatePatientVisitActivationDateUpdateStatusTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid(), 
                SiteId = BaseSiteId
            };

            var testVisit1 = new Core.BusinessLayer.Models.VisitModel
            {
                Id = Guid.NewGuid(),
                VisitOrder = 1,
                Name = "SV1",
                VisitStop_HSN = "visit stop1"
            };

            var testVisit2 = new Core.BusinessLayer.Models.VisitModel
            {
                Id = Guid.NewGuid(),
                VisitOrder = 2,
                Name = "SV2",
                VisitStop_HSN = "visit stop2"
            };

            var testPatientVisit1 = new PatientVisit
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = testVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now,
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id
            };

            var testPatientVisit2 = new PatientVisit
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = testVisit2.Id,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id
            };

            /* Diary setup */
            SetupDiaryEntries(testPatient);
            SetupPatients(new List<Patient> { testPatient });
            SetupPatientVisits(new List<PatientVisit> { testPatientVisit1, testPatientVisit2 });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
               .ReturnsAsync(new List<VisitModel> { testVisit1, testVisit2 });

            BaseVisits.Add(testVisit1);
            BaseVisits.Add(testVisit2);
            
            testPatientVisit2.VisitId = testVisit2.Id;

            await Repository.ActivatePatientVisit(testPatientVisit2.Id, testPatient.Id);

            var firstVisit = BasePatientVisits.Find(p => p.Id == testPatientVisit1.Id);

            Assert.AreEqual(PatientVisitStatusType.Missed.Id, firstVisit.PatientVisitStatusTypeId);
            Assert.IsFalse(testPatientVisit1.IsDirty);
            Assert.IsFalse(testPatientVisit2.IsDirty);
        }
    }
}
