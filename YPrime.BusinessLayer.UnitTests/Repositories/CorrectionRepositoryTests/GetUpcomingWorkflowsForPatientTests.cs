using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class GetUpcomingWorkflowsForPatientTests : CorrectionRepositoryTestBase
    {
        private Guid studyUserId = Guid.NewGuid();
        private Guid studyRoleId = Guid.NewGuid();
        private Guid secondStudyRoleId = Guid.NewGuid();
        private Guid siteId = Guid.NewGuid();
        private Guid patientId = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            var studyUsers = new FakeDbSet<StudyUser>(new List<StudyUser>()
                {
                    new StudyUser()
                    {
                        Id = studyUserId,
                        StudyUserRoles = new List<StudyUserRole>()
                        {
                            new StudyUserRole()
                            {
                                Id = Guid.NewGuid(),
                                StudyUserId = studyUserId,
                                StudyRoleId = studyRoleId,
                                SiteId = siteId
                            }
                        }
                    }
                });
            MockContext
                .Setup(c => c.StudyUsers)
                .Returns(studyUsers.Object);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_InvalidUserReturnsEmptyCollection()
        {
            var result = await Repository.GetUpcomingWorkflowsForPatient(Guid.NewGuid(), patientId);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_NoApproverGroupsReturnsEmptyCollection()
        {
            var result = await Repository.GetUpcomingWorkflowsForPatient(studyUserId, patientId);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_MultipleWorkflowsReturnsOneMatchPerCorrection()
        {
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testFirstCorrectionId = Guid.NewGuid();
            var testFirstWorkFlowId = Guid.NewGuid();
            var testSecondWorkFlowId = Guid.NewGuid();

            var firstApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = firstApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var secondApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = secondApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var testFirstCorrection = new Correction
            {
                Id = testFirstCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = patientId,
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    WorkflowOrder = 1
                }
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkFlows);

            MockApproverGroupService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>()
                {
                    firstApproverGroup,
                    secondApproverGroup,
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetUpcomingWorkflowsForPatient(studyUserId, patientId);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_NoWorkflowsForRoleReturnsEmptyCollection()
        {
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testFirstCorrectionId = Guid.NewGuid();
            var testFirstWorkFlowId = Guid.NewGuid();
            var testSecondWorkFlowId = Guid.NewGuid();

            var firstApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = firstApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            var secondApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = secondApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            var testFirstCorrection = new Correction
            {
                Id = testFirstCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = patientId,
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    WorkflowOrder = 1
                }
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkFlows);

            MockApproverGroupService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>()
                {
                    firstApproverGroup,
                    secondApproverGroup,
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetUpcomingWorkflowsForPatient(studyUserId, patientId);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_OneWorkflowForSiteReturnsSingleResult()
        {
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testFirstCorrectionId = Guid.NewGuid();
            var testSecondCorrectionId = Guid.NewGuid();
            var testFirstWorkFlowId = Guid.NewGuid();
            var testSecondWorkFlowId = Guid.NewGuid();

            var firstApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = firstApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var secondApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = secondApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var testFirstCorrection = new Correction
            {
                Id = testFirstCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.InProgress,
                PatientId = patientId,
                SiteId = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var testSecondCorrection = new Correction
            {
                Id = testSecondCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = patientId,
                SiteId = siteId,
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testSecondCorrectionId,
                    Correction = testSecondCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    WorkflowOrder = 1
                }
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkFlows);

            MockApproverGroupService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>()
                {
                    firstApproverGroup,
                    secondApproverGroup,
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetUpcomingWorkflowsForPatient(studyUserId, patientId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Id, testSecondWorkFlowId);
        }

        [TestMethod]
        public async Task GetUpcomingWorkflowsForPatient_OneWorkflowForPatientReturnsSingleResult()
        {
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testFirstCorrectionId = Guid.NewGuid();
            var testSecondCorrectionId = Guid.NewGuid();
            var testFirstWorkFlowId = Guid.NewGuid();
            var testSecondWorkFlowId = Guid.NewGuid();

            var firstApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = firstApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var secondApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = secondApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = studyRoleId
                    }
                }
            };

            var testFirstCorrection = new Correction
            {
                Id = testFirstCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.InProgress,
                PatientId = patientId,
                SiteId = siteId,
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var testSecondCorrection = new Correction
            {
                Id = testSecondCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = Guid.NewGuid(),
                SiteId = siteId,
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testFirstCorrectionId,
                    Correction = testFirstCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testSecondCorrectionId,
                    Correction = testSecondCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    WorkflowOrder = 1
                }
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkFlows);

            MockApproverGroupService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>()
                {
                    firstApproverGroup,
                    secondApproverGroup,
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetUpcomingWorkflowsForPatient(studyUserId, patientId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Id, testFirstWorkFlowId);
        }
    }
}