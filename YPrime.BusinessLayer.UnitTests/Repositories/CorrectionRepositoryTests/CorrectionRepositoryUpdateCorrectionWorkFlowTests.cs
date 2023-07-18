using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.UnitTests.TestExtensions;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryUpdateCorrectionWorkFlowTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryUpdateCorrectionWorkFlowApprovedMoreWorkflowsTest()
        {
            const int workflowLevelOrder = 1;

            var testWorkFlowId = Guid.NewGuid();
            var approvedCorrectionActionId = CorrectionActionEnum.Approved;
            var testStudyUserId = Guid.NewGuid();
            var testComment = Guid.NewGuid().ToString();
            var testUserName = "test@yprime.com";
            var testFirstName = "First";
            var testLastName = "Last";
            var testDateTimeStamp = DateTime.UtcNow.ToString("dd'-'MMM'-'yyyy hh':'mm tt 'UTC'");

            var testCorrection = new Correction
            {
                Id = Guid.NewGuid(),
                NoApprovalNeeded = false,
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>()
            };

            var testCorrectionHistory = new CorrectionHistory
            {
                Id = Guid.NewGuid(),
                UserName = testUserName,
                FirstName = testFirstName,
                LastName = testLastName,
                DateTimeStamp = testDateTimeStamp
            };

            var testApprovedWorkflow = new CorrectionWorkflow
            {
                Id = testWorkFlowId,
                WorkflowOrder = workflowLevelOrder,
                CorrectionActionId = CorrectionActionEnum.Pending,
                Correction = testCorrection,
                CorrectionId = testCorrection.Id
            };

            var testOtherWorkFlow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                WorkflowOrder = workflowLevelOrder,
                CorrectionActionId = CorrectionActionEnum.Pending,
                Correction = testCorrection,
                CorrectionId = testCorrection.Id
            };

            var correctionWorkflows = new List<CorrectionWorkflow>
            {
                testApprovedWorkflow,
                testOtherWorkFlow
            };

            testCorrection.CorrectionWorkflows = correctionWorkflows;

            var workflowDbSet = new FakeDbSet<CorrectionWorkflow>(correctionWorkflows);

            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));

            MockContext
                .Setup(db => db.CorrectionWorkflows)
                .Returns(workflowDbSet.Object);

            await Repository.UpdateCorrectionWorkFlow(
                testWorkFlowId,
                approvedCorrectionActionId,
                testStudyUserId,
                testComment,
                testCorrectionHistory);

            Assert.AreEqual(CorrectionActionEnum.Approved, testApprovedWorkflow.CorrectionActionId);
            Assert.AreEqual(testStudyUserId, testApprovedWorkflow.StudyUserId);
            Assert.AreEqual(CorrectionStatusEnum.InProgress, testApprovedWorkflow.Correction.CorrectionStatusId);
            Assert.AreEqual(1, testApprovedWorkflow.Correction.CorrectionDiscussions.Count);
            Assert.AreEqual(testComment, testApprovedWorkflow.Correction.CorrectionDiscussions.First().Discussion);

            MockContext.Verify(db => db.SaveChanges(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryUpdateCorrectionWorkFlowRejectedTest()
        {
            const int workflowLevelOrder = 1;

            var testWorkFlowId = Guid.NewGuid();
            var rejectedCorrectionActionId = CorrectionActionEnum.Rejected;
            var testStudyUserId = Guid.NewGuid();
            var testComment = Guid.NewGuid().ToString();
            var testUserName = "test@yprime.com";
            var testFirstName = "First";
            var testLastName = "Last";
            var testDateTimeStamp = DateTime.UtcNow.ToString("dd'-'MMM'-'yyyy hh':'mm tt 'UTC'");

            var testCorrection = new Correction
            {
                Id = Guid.NewGuid(),
                NoApprovalNeeded = false,
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>()
            };

            var testCorrectionHistory = new CorrectionHistory
            {
                Id = Guid.NewGuid(),
                UserName = testUserName,
                FirstName = testFirstName,
                LastName = testLastName,
                DateTimeStamp = testDateTimeStamp
            };

            var testRejectedWorkflow = new CorrectionWorkflow
            {
                Id = testWorkFlowId,
                WorkflowOrder = workflowLevelOrder,
                CorrectionActionId = CorrectionActionEnum.Pending,
                Correction = testCorrection,
                CorrectionId = testCorrection.Id
            };

            var testOtherWorkFlow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                WorkflowOrder = workflowLevelOrder,
                CorrectionActionId = CorrectionActionEnum.Pending,
                Correction = testCorrection,
                CorrectionId = testCorrection.Id
            };

            var correctionWorkflows = new List<CorrectionWorkflow>
            {
                testRejectedWorkflow,
                testOtherWorkFlow
            };

            testCorrection.CorrectionWorkflows = correctionWorkflows;

            var workflowDbSet = new FakeDbSet<CorrectionWorkflow>(correctionWorkflows);

            MockContext
                .Setup(db => db.CorrectionWorkflows)
                .Returns(workflowDbSet.Object);

            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));

            await Repository.UpdateCorrectionWorkFlow(
                testWorkFlowId,
                rejectedCorrectionActionId,
                testStudyUserId,
                testComment,
                testCorrectionHistory);

            Assert.AreEqual(testStudyUserId, testRejectedWorkflow.StudyUserId);
            Assert.AreEqual(CorrectionStatusEnum.Rejected, testRejectedWorkflow.Correction.CorrectionStatusId);
            Assert.AreEqual(1, testRejectedWorkflow.Correction.CorrectionDiscussions.Count);
            Assert.AreEqual(testComment, testRejectedWorkflow.Correction.CorrectionDiscussions.First().Discussion);
            Assert.That.AreCloseInSeconds(DateTimeOffset.Now, testRejectedWorkflow.Correction.CompletedDate, 5);

            MockContext.Verify(db => db.SaveChanges(It.IsAny<string>()), Times.Once);
        }
    }
}