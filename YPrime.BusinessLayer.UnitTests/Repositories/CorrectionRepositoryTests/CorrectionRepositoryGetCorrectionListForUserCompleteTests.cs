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
    public class CorrectionRepositoryGetCorrectionListForUserCompleteTests : CorrectionRepositoryTestBase
    {
        private Guid studyUserId = Guid.NewGuid();
        private Guid studyRoleId = Guid.NewGuid();
        private Guid secondStudyRoleId = Guid.NewGuid();
        private Guid siteId = Guid.NewGuid();

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
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteAllCompleteTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";
            var expectedApproverGroupName = $"{firstApproverGroupName},{secondApproverGroupName}";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testFirstWorkFlowId, result.First().Id);
            Assert.AreEqual(expectedApproverGroupName, result.First().ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteOneCompleteTest()
        {
            const string testCultureName = "en-US";
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
                CorrectionStatusId = CorrectionStatusEnum.Completed,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.ChangePatientVisit.Id
            };

            var testSecondCorrection = new Correction
            {
                Id = testSecondCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = CorrectionType.ChangePatientVisit.Id
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testFirstCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    },
                        new CorrectionWorkflowSettingsModel
                    {
                        Id = testSecondCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    },
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(2, result.Count);

            var firstWorkflowResult = result.First(r => r.Id == testFirstWorkFlowId);

            Assert.AreEqual(firstApproverGroupName, firstWorkflowResult.ApproverGroupName);

            var secondWorkflow = result.First(r => r.Id == testSecondWorkFlowId);

            Assert.AreEqual(secondApproverGroupName, secondWorkflow.ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteInProgressTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.InProgress,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testSecondWorkFlowId, result.First().Id);
            Assert.AreEqual(secondApproverGroupName, result.First().ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompletePendingTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testFirstWorkFlowId, result.First().Id);
            Assert.AreEqual(firstApproverGroupName, result.First().ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteNeedsMoreInfoTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.NeedsMoreInformation,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.NeedsMoreInformation,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testFirstWorkFlowId, result.First().Id);
            Assert.AreEqual(firstApproverGroupName, result.First().ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteRejectedTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Rejected,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Rejected,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testFirstWorkFlowId, result.First().Id);
            Assert.AreEqual(firstApproverGroupName, result.First().ApproverGroupName);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserCompleteRejectedBadDataTest()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Rejected,
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 1
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUserComplete(studyUserId, testCultureName);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserNeedsInfo()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.NeedsMoreInformation,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.NeedsMoreInformation
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.NeedsMoreInformation,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserNeedsInfoResolved()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.NeedsMoreInformation,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.NeedsMoreInformation,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.NeedsMoreInformation,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserNeedsInfoNotResolved()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.NeedsMoreInformation,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.NeedsMoreInformation,
                    Resolved = false
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.NeedsMoreInformation,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserUnauthorizedUser()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.NeedsMoreInformation,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.NeedsMoreInformation
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.NeedsMoreInformation,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserPending()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserPendingNullApprover()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserAllPending()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";
            const string thirdApproverGroupName = "Test Group 3";

            var testCorrectionId = Guid.NewGuid();
            var testCorrection2Id = Guid.NewGuid();
            var testFirstWorkFlowId = Guid.NewGuid();
            var testSecondWorkFlowId = Guid.NewGuid();
            var testThirdWorkFlowId = Guid.NewGuid();

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

            var thirdApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = thirdApproverGroupName,
                Roles = new List<StudyRoleModel>()
                {
                    new StudyRoleModel()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2, 
                SiteId = siteId
            };

            var testCorrection2 = new Correction
            {
                Id = testCorrection2Id,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                CurrentWorkflowOrder = 2,
                SiteId = siteId
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
                },
                new CorrectionWorkflow
                {
                    Id = testThirdWorkFlowId,
                    CorrectionId = testCorrection2Id,
                    Correction = testCorrection2,
                    ApproverGroupId = thirdApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 3
                }
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkFlows);

            MockApproverGroupService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<ApproverGroupModel>()
                {
                    firstApproverGroup,
                    secondApproverGroup,
                    thirdApproverGroup
                });

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(1, result.Count);

            var allPending = await Repository.GetCorrectionListForUserPending(studyUserId, testCultureName);

            Assert.AreEqual(2, allPending.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserPendingNoApprover()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2,
                NoApprovalNeeded = true,
                SiteId = siteId
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = Guid.NewGuid(),
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = Guid.NewGuid(),
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserPendingUnauthorizedSite()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Pending,
                    Resolved = true
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2,
                SiteId = Guid.NewGuid()
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Approved,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionListForUserRejectedCorrection()
        {
            const string testCultureName = "en-US";
            const string firstApproverGroupName = "Test Group 1";
            const string secondApproverGroupName = "Test Group 2";

            var testCorrectionId = Guid.NewGuid();
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
                        Id = secondStudyRoleId
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

            var testCorrection = new Correction
            {
                Id = testCorrectionId,
                CorrectionStatusId = CorrectionStatusEnum.Rejected,
                CorrectionStatus = new CorrectionStatus()
                {
                    Id = CorrectionStatusEnum.Rejected
                },
                PatientId = Guid.NewGuid(),
                CorrectionTypeId = Guid.NewGuid(),
                StartedByUserId = studyUserId,
                CurrentWorkflowOrder = 2,
            };

            var testWorkFlows = new List<CorrectionWorkflow>
            {
                new CorrectionWorkflow
                {
                    Id = testFirstWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = firstApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Rejected,
                    WorkflowOrder = 1,
                },
                new CorrectionWorkflow
                {
                    Id = testSecondWorkFlowId,
                    CorrectionId = testCorrectionId,
                    Correction = testCorrection,
                    ApproverGroupId = secondApproverGroup.Id,
                    CorrectionActionId = CorrectionActionEnum.Pending,
                    WorkflowOrder = 2
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

            MockCorrectionWorkflowService
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CorrectionWorkflowSettingsModel>()
                {
                    new CorrectionWorkflowSettingsModel
                    {
                        Id = testCorrection.CorrectionTypeId,
                        Name = "Name",
                        Description = "Description"
                    }
                });

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            var result = await Repository.GetCorrectionListForUser(studyUserId, testCultureName);

            Assert.AreEqual(0, result.Count);
        }
    }
}