using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
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
    public class DCFStatusReportTests
    {
        private const string PendingStatusTranslationKey = "pending-key";
        private const string PendingStatusText = "Pending";

        private const string CompleteStatusTranslationKey = "complete-key";
        private const string CompleteStatusText = "Complete";

        private const string ChangePatientResponseType = "Change subject Information";
        private const string ExpectedRoleActionName = "CanViewDCFList";

        private Mock<IStudyDbContext> MockContext { get; set; }
        private Mock<IApproverGroupService> MockApproverGroupService { get; set;  }
        private Mock<ITranslationService> MockTranslationService { get; set; }
        private Mock<ICorrectionTypeService> MockCorrectionTypeService { get; set; }
        private Mock<IRoleRepository> MockRoleRepository { get; set; }

        private Patient Patient { get; set; }
        private Guid TestUserId { get; set; }

        private ApproverGroupModel TestApproverGroup { get; set; }
        private Correction PendingCorrection { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockContext = new Mock<IStudyDbContext>();
            MockApproverGroupService = new Mock<IApproverGroupService>();
            MockTranslationService = new Mock<ITranslationService>();
            MockRoleRepository = new Mock<IRoleRepository>();
            MockCorrectionTypeService = new Mock<ICorrectionTypeService>();

            TestUserId = Guid.NewGuid();

            Patient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "123",
                SiteId = Guid.NewGuid()
            };

            TestApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Approver Group"
            };

            MockApproverGroupService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>
                 {
                     TestApproverGroup
                 });

            var studyUserRoles = new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    Id = Guid.NewGuid(),
                    StudyUserId = TestUserId,
                    SiteId = Patient.SiteId
                }
            };

            var studyRoleDbSet = new FakeDbSet<StudyUserRole>(studyUserRoles);

            MockContext
                .Setup(c => c.StudyUserRoles)
                .Returns(studyRoleDbSet.Object);

            var correctionStatusTypes = new List<CorrectionStatus>
            {
                new CorrectionStatus
                {
                    Id = CorrectionStatusEnum.Pending,
                    TranslationKey = PendingStatusTranslationKey
                }
            };

            var correctionStatusDataset = new FakeDbSet<CorrectionStatus>(correctionStatusTypes);

            MockContext.Setup(c => c.CorrectionStatuses).Returns(correctionStatusDataset.Object);

            var patientDataset = new FakeDbSet<Patient>(new List<Patient>
            {
                Patient
            });

            MockContext.Setup(c => c.Patients).Returns(patientDataset.Object);

            PendingCorrection = new Correction
            {
                Id = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireInfo.Id,
                CorrectionStatusId = correctionStatusTypes.First(st => st.Id == CorrectionStatusEnum.Pending).Id,
                PatientId = Patient.Id,
                Patient = Patient,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = correctionStatusTypes.First(st => st.Id == CorrectionStatusEnum.Pending),
                NoApprovalNeeded = true,
                StartedDate = DateTime.Now.AddDays(-4),
                CorrectionWorkflows = new List<CorrectionWorkflow>
                {
                    new CorrectionWorkflow
                    {
                        Id = Guid.NewGuid(),
                        CorrectionActionId = CorrectionActionEnum.Pending,
                        ApproverGroupId = TestApproverGroup.Id
                    }
                }
            };

            var correctionDataset = new FakeDbSet<Correction>(new List<Correction>
            {
                PendingCorrection
            });

            MockContext.Setup(c => c.Corrections).Returns(correctionDataset.Object);

            var pendingTranslation = new TranslationModel
            {
                LanguageId = "en-US",
                Id = PendingStatusTranslationKey,
                LocalText = PendingStatusText
            };

            var completeTranslation = new TranslationModel
            {
                LanguageId = "en-US",
                Id = CompleteStatusTranslationKey,
                LocalText = CompleteStatusText
            };

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(key => key == pendingTranslation.Id), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(pendingTranslation.LocalText);

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(key => key == completeTranslation.Id), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(completeTranslation.LocalText);

            MockRoleRepository
                .Setup(r => r.UserHasRoleAction(It.Is<Guid>(id => id == TestUserId), It.Is<string>(an => an == ExpectedRoleActionName)))
                .ReturnsAsync(true);

            var testCorrectionType = new CorrectionTypeModel
            {
                Id = CorrectionType.ChangeQuestionnaireInfo.Id,
                Name = ChangePatientResponseType
            };

            MockCorrectionTypeService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionTypeModel>
                 {
                     testCorrectionType
                 });
        }

        [TestMethod]
        public async Task DCFStatusReportTest()
        {
            var report = GetReport();

            var result = await report.GetGridData(null, TestUserId);

            Assert.AreEqual(1, result.Count);

            var reportResult = result.First();

            Assert.AreEqual(11, reportResult.Row.Count);
            Assert.AreEqual("0000", reportResult.Row["DCFNumber"]);
            Assert.AreEqual(Patient.PatientNumber, reportResult.Row["Subject"]);
            Assert.AreEqual(PendingStatusText, reportResult.Row["DCFStatus"]);
            Assert.AreEqual(ChangePatientResponseType, reportResult.Row["DCFType"]);
            Assert.AreEqual("N/A", reportResult.Row["DCFClosedDate"]);
            Assert.AreEqual(PendingCorrection.StartedDate.ToString("dd-MMM-yyyy"), reportResult.Row["DCFOpenedDate"]);
            Assert.AreEqual("N/A", reportResult.Row["CompletedApprovals"]);
            Assert.AreEqual(TestApproverGroup.Name, reportResult.Row["PendingApproverGroup"]);
            Assert.AreEqual("5", reportResult.Row["NumberOfDaysOpen"]);

            MockRoleRepository
                .Verify(r => r.UserHasRoleAction(It.Is<Guid>(id => id == TestUserId), It.Is<string>(an => an == ExpectedRoleActionName)), Times.Once);
        }

        [TestMethod]
        public void GetColumnHeadingsTest()
        {
            var report = GetReport();

            var result = report.GetColumnHeadings();

            Assert.AreEqual(11, result.Count);
        }

        [TestMethod]
        public async Task GetReportChartDataTest()
        {
            var report = GetReport();

            var result = await report.GetReportChartData(
                null,
                Guid.NewGuid());

            Assert.IsNull(result);
        }

        private DCFStatusReport GetReport()
        {
            var report = new DCFStatusReport(
                MockContext.Object,
                MockApproverGroupService.Object,
                MockTranslationService.Object,
                MockRoleRepository.Object,
                MockCorrectionTypeService.Object);

            return report;
        }
    }
}

