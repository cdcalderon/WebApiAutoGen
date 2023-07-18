using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.UnitTests.Reports
{
    [TestClass]
    public class eCOASubjectComplianceReportTests
    {
        [TestMethod]
        public async Task GetGridDataTest()
        {
            var mockContext = new Mock<IStudyDbContext>();
            var mockTranslationService = new Mock<ITranslationService>();
            var mockPatientStatusService = new Mock<IPatientStatusService>();
            var testUserId = Guid.NewGuid();
            var testSiteAId = Guid.NewGuid();
            var testSiteBId = Guid.NewGuid();
            var dailyDiaryQuestionnaireId = Guid.Parse("D63F9B76-A70A-4529-BD64-0A654FC4D9FA");

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

            ConfigurationManager.AppSettings["QuestionnaireComplianceId"] = "D63F9B76-A70A-4529-BD64-0A654FC4D9FA";
            var studyUserRoleDataset = new FakeDbSet<StudyUserRole>(studyUserRoles);

            mockContext
                .Setup(ctx => ctx.StudyUserRoles)
                .Returns(studyUserRoleDataset.Object);


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
                },
                DiaryEntries = new List<DiaryEntry>
                {
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-1)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-2)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = Guid.NewGuid(),
                        DiaryDate = DateTime.Now.AddDays(-2)
                    },
                }
            };

            var duplicatePatientB = new Patient
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
                },
                DiaryEntries = new List<DiaryEntry>
                {
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-1)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-2)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = Guid.NewGuid(),
                        DiaryDate = DateTime.Now.AddDays(-2)
                    },
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
                },
                DiaryEntries = new List<DiaryEntry>
                {
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now
                    },
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
                },
                DiaryEntries = new List<DiaryEntry>
                {
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-1)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-2)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-3)
                    },
                    new DiaryEntry
                    {
                        Id = Guid.NewGuid(),
                        QuestionnaireId = dailyDiaryQuestionnaireId,
                        DiaryDate = DateTime.Now.AddDays(-4)
                    },
                }
            };

            var patients = new List<Patient>
            {
                patientD,
                patientC,
                patientA,
                duplicatePatientB
            };

            var patientDataset = new FakeDbSet<Patient>(patients);

            mockPatientStatusService
                .Setup(pvr => pvr.GetAll(null))
                .ReturnsAsync(statusTypes);

            mockContext
                .Setup(ctx => ctx.Patients)
                .Returns(patientDataset.Object);

            var report = new eCOASubjectComplianceReport(
                mockContext.Object,
                mockTranslationService.Object,
                mockPatientStatusService.Object);

            var results = await report.GetGridData(null, testUserId);

            Assert.AreEqual(3, results.Count);

            var patientAResults = results.First();

            Assert.AreEqual(testSiteA.SiteNumber, patientAResults.Row["SiteNumber"].ToString());
            Assert.AreEqual(patientA.PatientNumber, patientAResults.Row["PatientNumber"].ToString());
            Assert.AreEqual("2", patientAResults.Row["DiariesCompleted"].ToString());
            Assert.AreEqual("1", patientAResults.Row["DiariesMissed"].ToString());
            Assert.AreEqual("66.67", patientAResults.Row["CompliancePercentage"].ToString());

            var patientCResults = results.Skip(1).First();

            Assert.AreEqual(testSiteB.SiteNumber, patientCResults.Row["SiteNumber"].ToString());
            Assert.AreEqual(patientC.PatientNumber, patientCResults.Row["PatientNumber"].ToString());
            Assert.AreEqual("0", patientCResults.Row["DiariesCompleted"].ToString());
            Assert.AreEqual("0", patientCResults.Row["DiariesMissed"].ToString());
            Assert.AreEqual("0.00", patientCResults.Row["CompliancePercentage"].ToString());

            var patientDResults = results.Skip(2).First();

            Assert.AreEqual(testSiteB.SiteNumber, patientDResults.Row["SiteNumber"].ToString());
            Assert.AreEqual(patientD.PatientNumber, patientDResults.Row["PatientNumber"].ToString());
            Assert.AreEqual("4", patientDResults.Row["DiariesCompleted"].ToString());
            Assert.AreEqual("0", patientDResults.Row["DiariesMissed"].ToString());
            Assert.AreEqual("100.00", patientDResults.Row["CompliancePercentage"].ToString());
        }
    }
}
