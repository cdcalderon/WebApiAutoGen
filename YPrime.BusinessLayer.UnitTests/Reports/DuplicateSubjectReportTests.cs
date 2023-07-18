using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.UnitTests.Reports
{
    [TestClass]
    public class DuplicateSubjectReportTests
    {
        [TestMethod]
        public async Task GetGridDataWithDupes()
        {
            var mockContext = new Mock<IStudyDbContext>();
            var mockTranslationService = new Mock<ITranslationService>();
            var mockPatientStatusService = new Mock<IPatientStatusService>();
            var testUserId = Guid.NewGuid();
            var testSiteAId = Guid.NewGuid();
            var testSiteBId = Guid.NewGuid();

            var studyUserRoles = new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    SiteId = testSiteAId,
                    StudyUserId = testUserId
                },
                new StudyUserRole
                {
                    SiteId = testSiteBId,
                    StudyUserId = testUserId
                }
            };

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Removed"
                }
            };

            var studyUserRoleDataset = new FakeDbSet<StudyUserRole>(studyUserRoles);

            mockContext
                .Setup(ctx => ctx.StudyUserRoles)
                .Returns(studyUserRoleDataset.Object);

            mockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            var testSiteA = new Site
            {
                Id = testSiteAId,
                Name = "Site A",
                SiteNumber = "001"
            };

            var testSiteB = new Site
            {
                Id = testSiteBId,
                Name = "Site B",
                SiteNumber = "002"
            };

            var patientA = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteA.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-4),
                SiteId = testSiteA.Id,
                Site = testSiteA,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 12, 30, 30)
                    }
                }
            };

            var duplicatePatientA = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteA.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-5),
                SiteId = testSiteA.Id,
                Site = testSiteA,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 12, 30, 30)
                    }
                }
            };

            var patientC = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteB.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now,
                SiteId = testSiteB.Id,
                Site = testSiteB,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 19, 4, 30)
                    }
                }
            };

            var patientD = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteB.SiteNumber}-002",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-2),
                SiteId = testSiteB.Id,
                Site = testSiteB,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 2, 4, 6, 30)
                    }
                }
            };

            var patients = new List<Patient>
            {
                patientD,
                patientC,
                patientA,
                duplicatePatientA
            };

            var patientDataset = new FakeDbSet<Patient>(patients);
            mockContext
                .Setup(ctx => ctx.Patients)
                .Returns(patientDataset.Object);

            var report = new DuplicateSubjectReport(mockContext.Object, mockTranslationService.Object, mockPatientStatusService.Object);
            var results = await report.GetGridData(null, testUserId);

            Assert.AreEqual(2, results.Count);
            var dupePatientAResults = results.First();

            Assert.AreEqual(testSiteA.SiteNumber, dupePatientAResults.Row["Site"].ToString());
            Assert.AreEqual(duplicatePatientA.PatientNumber, dupePatientAResults.Row["SubjectNumber"].ToString());
            Assert.AreEqual(duplicatePatientA.EnrolledDate.ToString(DateTimeFormatConstants.DefaultDateTime), dupePatientAResults.Row["EnrolledDate"].ToString());

            var patientAResults = results.Skip(1).First();
            Assert.AreEqual(testSiteA.SiteNumber, patientAResults.Row["Site"].ToString());
            Assert.AreEqual(patientA.PatientNumber, patientAResults.Row["SubjectNumber"].ToString());
            Assert.AreEqual(patientA.EnrolledDate.ToString(DateTimeFormatConstants.DefaultDateTime), patientAResults.Row["EnrolledDate"].ToString());
        }

        [TestMethod]
        public async Task GetGridDataWithoutDupes()
        {
            var mockContext = new Mock<IStudyDbContext>();
            var mockTranslationService = new Mock<ITranslationService>();
            var mockPatientStatusService = new Mock<IPatientStatusService>();
            var testUserId = Guid.NewGuid();
            var testSiteAId = Guid.NewGuid();
            var testSiteBId = Guid.NewGuid();

            var studyUserRoles = new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    SiteId = testSiteAId,
                    StudyUserId = testUserId
                },
                new StudyUserRole
                {
                    SiteId = testSiteBId,
                    StudyUserId = testUserId
                }
            };

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Removed"
                }
            };

            var studyUserRoleDataset = new FakeDbSet<StudyUserRole>(studyUserRoles);

            mockContext
                .Setup(ctx => ctx.StudyUserRoles)
                .Returns(studyUserRoleDataset.Object);

            mockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            var testSiteA = new Site
            {
                Id = testSiteAId,
                Name = "Site A",
                SiteNumber = "001"
            };

            var testSiteB = new Site
            {
                Id = testSiteBId,
                Name = "Site B",
                SiteNumber = "002"
            };

            var patientA = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteA.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-4),
                SiteId = testSiteA.Id,
                Site = testSiteA,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 12, 30, 30)
                    }
                }
            };

            var patientC = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteB.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now,
                SiteId = testSiteB.Id,
                Site = testSiteB,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 19, 4, 30)
                    }
                }
            };

            var patientD = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteB.SiteNumber}-002",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-2),
                SiteId = testSiteB.Id,
                Site = testSiteB,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 2, 4, 6, 30)
                    }
                }
            };

            var patients = new List<Patient>
            {
                patientD,
                patientC,
                patientA
            };

            var patientDataset = new FakeDbSet<Patient>(patients);
            mockContext
                .Setup(ctx => ctx.Patients)
                .Returns(patientDataset.Object);

            var report = new DuplicateSubjectReport(mockContext.Object, mockTranslationService.Object, mockPatientStatusService.Object);
            var results = await report.GetGridData(null, testUserId);

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public async Task GetGridDataWithoutRemoved()
        {
            var mockContext = new Mock<IStudyDbContext>();
            var mockTranslationService = new Mock<ITranslationService>();
            var mockPatientStatusService = new Mock<IPatientStatusService>();
            var testUserId = Guid.NewGuid();
            var testSiteAId = Guid.NewGuid();
            var testSiteBId = Guid.NewGuid();

            var studyUserRoles = new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    SiteId = testSiteAId,
                    StudyUserId = testUserId
                },
                new StudyUserRole
                {
                    SiteId = testSiteBId,
                    StudyUserId = testUserId
                }
            };

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Removed", 
                    IsRemoved = true,
                }
            };

            var studyUserRoleDataset = new FakeDbSet<StudyUserRole>(studyUserRoles);

            mockContext
                .Setup(ctx => ctx.StudyUserRoles)
                .Returns(studyUserRoleDataset.Object);

            mockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            var testSiteA = new Site
            {
                Id = testSiteAId,
                Name = "Site A",
                SiteNumber = "001"
            };

            var testSiteB = new Site
            {
                Id = testSiteBId,
                Name = "Site B",
                SiteNumber = "002"
            };

            var patientA = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteA.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First().Id,
                EnrolledDate = DateTime.Now.AddDays(-4),
                SiteId = testSiteA.Id,
                Site = testSiteA,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 12, 30, 30)
                    }
                }
            };

            var patientB = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = $"S-{testSiteA.SiteNumber}-001",
                PatientStatusTypeId = statusTypes.First(s => s.Id == 3).Id,
                EnrolledDate = DateTime.Now.AddDays(-4),
                SiteId = testSiteA.Id,
                Site = testSiteA,
                Devices = new List<Device>
                {
                    new Device
                    {
                        LastSyncDate = new DateTime(2020, 9, 3, 12, 30, 30)
                    }
                }
            };

            var patients = new List<Patient>
            {
                patientA,
                patientB,
            };

            var patientDataset = new FakeDbSet<Patient>(patients);

            mockContext
                .Setup(ctx => ctx.Patients)
                .Returns(patientDataset.Object);

            var report = new DuplicateSubjectReport(mockContext.Object, mockTranslationService.Object, mockPatientStatusService.Object);
            var results = await report.GetGridData(null, testUserId);

            Assert.AreEqual(0, results.Count);
        }
    }
}
