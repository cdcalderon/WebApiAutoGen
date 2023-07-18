using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Repositories;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryGetAllPatientsTests : PatientRepositoryTestBase
    {
        [TestMethod]
        public async Task GetAllPatientsTest()
        {
            var siteFilterList = new List<Guid>();

            var lastDiaryDate = DateTime.Now.AddDays(-1);

            var testPatient1 = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First().Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            var testDiaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient1.Id,
                QuestionnaireId = Guid.Parse(PatientRepository.DailyDiaryId),
                TransmittedTime = lastDiaryDate,
                DiaryDate = lastDiaryDate
            };

            testPatient1.DiaryEntries.Add(testDiaryEntry);

            var testPatientVisit1 = new PatientVisit
            {
                Id = Guid.NewGuid(),
                VisitId = BaseVisits.OrderBy(v => v.VisitOrder).First().Id,
                VisitDate = testPatient1.EnrolledDate
            };

            testPatient1.PatientVisits.Add(testPatientVisit1);

            var testPatientVisit2 = new PatientVisit
            {
                Id = Guid.NewGuid(),
                VisitId = BaseVisits.OrderBy(v => v.VisitOrder).Skip(1).First().Id,
                VisitDate = testPatient1.EnrolledDate.AddDays(5)
            };

            testPatient1.PatientVisits.Add(testPatientVisit2);

            BasePatients.Add(testPatient1);
            BasePatientVisits.Add(testPatientVisit1);
            BasePatientVisits.Add(testPatientVisit2);
            BaseDiaryEntries.Add(testDiaryEntry);

            var results = await Repository.GetAllPatients(siteFilterList);

            Assert.AreEqual(1, results.Count());

            var result = results.First();

            Assert.AreEqual(testPatient1.Id, result.Id);
            Assert.AreEqual(testPatient1.IsHandheldTrainingComplete, result.IsHandheldTrainingComplete);
            Assert.AreEqual(testPatient1.IsTabletTrainingComplete, result.IsTabletTrainingComplete);
            Assert.AreEqual(testPatient1.IsTempPin, result.IsTempPin);
            Assert.AreEqual(testPatient1.SiteId, result.SiteId);
            Assert.AreEqual(BaseSite.IsActive, result.IsSiteActive);
            Assert.AreEqual(testPatient1.PatientNumber, result.PatientNumber);
            Assert.AreEqual(testPatient1.EnrolledDate, result.EnrolledDate);
            Assert.AreEqual(testDiaryEntry.TransmittedTime, result.LastDeviceSyncDate);
            Assert.AreEqual(testDiaryEntry.DiaryDate, result.LastDiaryEntryDate);
            Assert.AreEqual(testPatient1.PatientStatusTypeId, result.PatientStatusTypeId);
            Assert.AreEqual(BasePatientStatuses.First().Name, result.PatientStatus);
            Assert.AreEqual(BasePatientStatuses.First().IsActive, result.IsActive);
            Assert.IsFalse(result.Compliance);

            var expectedLastVisit = BaseVisits.OrderByDescending(v => v.VisitOrder).First();

            Assert.AreEqual(expectedLastVisit.Name, result.LastVisit);
            Assert.AreEqual(testPatient1.PatientVisits.First(pv => pv.VisitId == expectedLastVisit.Id).VisitDate, result.LastVisitDate);
        }

        [TestMethod]
        public async Task GetAllPatientsCompliantTest()
        {
            var siteFilterList = new List<Guid>();
            const int enrolledDaysAgo = 10;

            var testPatient1 = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-enrolledDaysAgo).Date,
                PatientStatusTypeId = BasePatientStatuses.First().Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            BasePatients.Add(testPatient1);

            for (var i = 0; i < enrolledDaysAgo; i++)
            {
                var testDiaryEntry = new DiaryEntry
                {
                    Id = Guid.NewGuid(),
                    PatientId = testPatient1.Id,
                    QuestionnaireId = Guid.Parse(PatientRepository.DailyDiaryId),
                    TransmittedTime = DateTime.Now.AddDays(-i),
                    DiaryDate = DateTime.Now.AddDays(-i).Date
                };

                testPatient1.DiaryEntries.Add(testDiaryEntry);
                BaseDiaryEntries.Add(testDiaryEntry);
            }

            var expectedLastDiary = testPatient1
                .DiaryEntries
                .OrderByDescending(d => d.DiaryDate)
                .First();

            var results = await Repository.GetAllPatients(siteFilterList);

            Assert.AreEqual(1, results.Count());

            var result = results.First();

            Assert.AreEqual(expectedLastDiary.TransmittedTime, result.LastDeviceSyncDate);
            Assert.AreEqual(expectedLastDiary.DiaryDate, result.LastDiaryEntryDate);
            Assert.IsTrue(result.Compliance);
        }

        [TestMethod]
        public async Task GetAllPatientsFilterBySiteTest()
        {
            var siteFilterList = new List<Guid>
            {
                Guid.NewGuid()
            };

            var testPatient1 = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First().Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            BasePatients.Add(testPatient1);

            var results = await Repository.GetAllPatients(siteFilterList);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetAllPatientsFilterOnlyActiveTest()
        {
            var siteFilterList = new List<Guid>();

            var activeTestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First().Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            var inactiveTestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First(p => p.IsRemoved).Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            BasePatients.Add(activeTestPatient);
            BasePatients.Add(inactiveTestPatient);

            var results = await Repository.GetAllPatients(siteFilterList, true);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(activeTestPatient.Id, results.First().Id);
        }

        [TestMethod]
        public async Task GetAllPatientsFilterOnlyInactiveTest()
        {
            var siteFilterList = new List<Guid>();

            var activeTestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First().Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            var inactiveTestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123457",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                PatientStatusTypeId = BasePatientStatuses.First(p => p.Name == "Discontinued").Id,
                SiteId = BaseSite.Id,
                Site = BaseSite,
                DiaryEntries = new List<DiaryEntry>(),
                PatientVisits = new List<PatientVisit>()
            };

            BasePatients.Add(activeTestPatient);
            BasePatients.Add(inactiveTestPatient);

            var results = await Repository.GetAllPatients(siteFilterList, false);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(inactiveTestPatient.Id, results.First().Id);
        }
    }
}
