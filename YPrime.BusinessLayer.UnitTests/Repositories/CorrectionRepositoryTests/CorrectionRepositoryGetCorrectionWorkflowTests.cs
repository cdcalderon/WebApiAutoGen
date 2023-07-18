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
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryGetCorrectionWorkflowTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryGetPaperDCFCorrectionWorkflowTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var diaryPage = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id, 
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage.Questions = new List<QuestionModel>
            {
                firstQuestion,
                secondQuestion
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage
                }
            };

            var outOfOrderCorrectionApprovalDatas = new List<CorrectionApprovalData>
            {
                new CorrectionApprovalData
                {
                    Id = visitIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.VisitId),
                    NewDataPoint = "1"
                },
                new CorrectionApprovalData
                {
                    Id = secondQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestion.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = diaryDateCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.DiaryDate),
                    NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy")
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestion.Id,
                    NewDataPoint = "testing", 
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId", 
                            ColumnValue = firstQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = questionnaireIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.QuestionnaireId),
                    NewDataPoint = testQuestionnaireId.ToString()
                }
            };

            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = outOfOrderCorrectionApprovalDatas,
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                QuestionnaireId = testQuestionnaireId,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                }
            };

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testWorkflow
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testQuestionnaire);

            var result = await Repository
                .GetCorrectionWorkflow(testWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;

            Assert.AreEqual(testCorrection.Site.Name, resultCorrection.SiteName);
            Assert.AreEqual(expectedCorrectionTypeDisplayName, resultCorrection.CorrectionWorkflowSettings.Name);
            Assert.AreEqual(expectedCorrectionDescriptionDisplayName, resultCorrection.CorrectionWorkflowSettings.Description);
            Assert.IsFalse(resultCorrection.AllowEdit);

            Assert.AreEqual(5, resultCorrection.CorrectionApprovalDatas.Count);
            Assert.AreEqual(questionnaireIdCorrectionDataId, resultCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, resultCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, resultCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionCorrectionDataId, resultCorrection.CorrectionApprovalDatas[3].Id);
            Assert.AreEqual(secondQuestionCorrectionDataId, resultCorrection.CorrectionApprovalDatas[4].Id);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetChangeQuestionnaireResponseCorrectionWorkflowTest()
        {
            var expectedCorrectionTypeDisplayName = "Change Questionnaire Response DCF";
            var expectedCorrectionDescriptionDisplayName = "Change Questionnaire Response DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var diaryPage = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage.Questions = new List<QuestionModel>
            {
                firstQuestion,
                secondQuestion
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage
                }
            };

            var outOfOrderCorrectionApprovalDatas = new List<CorrectionApprovalData>
            {
                new CorrectionApprovalData
                {
                    Id = visitIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.VisitId),
                    NewDataPoint = "1"
                },
                new CorrectionApprovalData
                {
                    Id = secondQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestion.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = diaryDateCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.DiaryDate),
                    NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy")
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestion.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = questionnaireIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.QuestionnaireId),
                    NewDataPoint = testQuestionnaireId.ToString()
                },
            };

            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = outOfOrderCorrectionApprovalDatas,
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                QuestionnaireId = testQuestionnaireId,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                }
            };

            var dbSet = new FakeDbSet<Answer>(Enumerable.Empty<Answer>());

            MockContext.Setup(ctx => ctx.Answers)
                .Returns(dbSet.Object);

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testWorkflow
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testQuestionnaire);

            var result = await Repository
                .GetCorrectionWorkflow(testWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;

            Assert.AreEqual(testCorrection.Site.Name, resultCorrection.SiteName);
            Assert.AreEqual(expectedCorrectionTypeDisplayName, resultCorrection.CorrectionWorkflowSettings.Name);
            Assert.AreEqual(expectedCorrectionDescriptionDisplayName, resultCorrection.CorrectionWorkflowSettings.Description);
            Assert.IsFalse(resultCorrection.AllowEdit);

            Assert.AreEqual(5, resultCorrection.CorrectionApprovalDatas.Count);
            Assert.AreEqual(questionnaireIdCorrectionDataId, resultCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, resultCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, resultCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionCorrectionDataId, resultCorrection.CorrectionApprovalDatas[3].Id);
            Assert.AreEqual(secondQuestionCorrectionDataId, resultCorrection.CorrectionApprovalDatas[4].Id);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetNoApprovalDataWorkflowTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var diaryPage = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
            };

            var secondQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
            };


            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>(),
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                }
            };

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testWorkflow
            };

            var testQuestions = new List<QuestionModel>
            {
                firstQuestion,
                secondQuestion
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockQuestionnaireService
                .Setup(s => s.GetQuestions(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(testQuestions);

            var result = await Repository
                .GetCorrectionWorkflow(testWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;

            MockQuestionnaireService
                .Verify(s => s.GetQuestions(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>()), Times.Never);
        }

        [TestMethod]
        public async Task CorrectionRepositoryOtherCorrectionTypeWorkflowTest()
        {
            var expectedCorrectionTypeDisplayName = "Subject Information DCF";
            var expectedCorrectionDescriptionDisplayName = "Subject Information DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();
         

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var testPatientAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                SubjectInformation = new SubjectInformationModel
                {
                    Id = Guid.NewGuid(),
                    Suffix = "Test"
                }
            };

            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        Id = visitIdCorrectionDataId,
                        TableName = nameof(PatientAttribute),
                        ColumnName = nameof(PatientAttribute.AttributeValue),
                        NewDataPoint = "test",
                        RowId = testPatientAttribute.Id
                    }
                },
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                },
                PatientId = TestPatient.Id
            };

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testWorkflow
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockPatientAttributeRepository
                .Setup(q => q.GetPatientAttributes(TestPatient.Id, testLocale))
                .ReturnsAsync(new List<PatientAttributeDto> { testPatientAttribute });

            var result = await Repository
                .GetCorrectionWorkflow(testWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;

            MockQuestionnaireService
                .Verify(s => s.GetQuestions(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>()), Times.Never);
        }

        [TestMethod]
        public async Task CorrectionRepositorySubjectInfoTypeSuffixWorkflowTest()
        {
            var expectedCorrectionTypeDisplayName = "Subject Information DCF";
            var expectedCorrectionDescriptionDisplayName = "Subject Information DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();
            var testPatientId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var configurationDetailId = Guid.NewGuid();

            var testPatientAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                AttributeValue = "Test Data",
                PatientAttributeConfigurationDetailId = configurationDetailId,
                SubjectInformation = new SubjectInformationModel
                {
                    Id = Guid.NewGuid(),
                    Suffix = "Test Suffix"
                }
            };

            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        Id = visitIdCorrectionDataId,
                        TableName = nameof(PatientAttribute),
                        ColumnName = nameof(PatientAttribute.AttributeValue),
                        NewDataPoint = "test",
                        NewDisplayValue = testPatientAttribute.AttributeValue,
                        RowId = testPatientAttribute.Id,
                        CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                        {
                            new CorrectionApprovalDataAdditional
                            {
                                ColumnName = "PatientAttributeConfigurationDetailId",
                                ColumnValue = configurationDetailId.ToString()
                            }
                        }
                    }
                },
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                },
                PatientId = testPatientId
            };

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testWorkflow
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockPatientAttributeRepository
                .Setup(q => q.GetPatientAttributes(testPatientId, testLocale))
                .ReturnsAsync(new List<PatientAttributeDto> { testPatientAttribute });

            var result = await Repository
                .GetCorrectionWorkflow(testWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;
            var correctionDisplayValue = $"{testPatientAttribute.AttributeValue} {testPatientAttribute.SubjectInformation.Suffix}";

            MockQuestionnaireService
                .Verify(s => s.GetQuestions(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>()), Times.Never);
            Assert.AreEqual(resultCorrection.CorrectionApprovalDatas[0].NewDisplayValue, correctionDisplayValue);
        }

        [TestMethod]
        public async Task CorrectionRepositoryGetCorrectionWorkflowMultipleTest()
        {
            var expectedCorrectionTypeDisplayName = "Change Questionnaire Response DCF";
            var expectedCorrectionDescriptionDisplayName = "Change Questionnaire Response DCF Description";
            var testLocale = "en-US";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var firstApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = "First Approver Group"
            };

            var secondApproverGroup = new ApproverGroupModel
            {
                Id = Guid.NewGuid(),
                Name = "Second Approver Group"
            };

            MockApproverGroupService
                .Setup(s => s.Get(It.Is<Guid>(id => id == firstApproverGroup.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(firstApproverGroup);

            MockApproverGroupService
                .Setup(s => s.Get(It.Is<Guid>(id => id == secondApproverGroup.Id), It.IsAny<Guid?>()))
                .ReturnsAsync(secondApproverGroup);

            var diaryPage = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage.Questions = new List<QuestionModel>
            {
                firstQuestion,
                secondQuestion
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage
                }
            };

            var outOfOrderCorrectionApprovalDatas = new List<CorrectionApprovalData>
            {
                new CorrectionApprovalData
                {
                    Id = visitIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.VisitId),
                    NewDataPoint = "1"
                },
                new CorrectionApprovalData
                {
                    Id = secondQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestion.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = diaryDateCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.DiaryDate),
                    NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy")
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionCorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestion.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestion.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = questionnaireIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.QuestionnaireId),
                    NewDataPoint = testQuestionnaireId.ToString()
                },
            };

            var testCorrection = new Correction
            {
                CorrectionDiscussions = new List<CorrectionDiscussion>(),
                CorrectionApprovalDatas = outOfOrderCorrectionApprovalDatas,
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                QuestionnaireId = testQuestionnaireId,
                Site = new Site
                {
                    Name = "Test Site"
                },
                CorrectionStatus = new CorrectionStatus
                {
                    Resolved = true
                }
            };

            MockCorrectionWorkflowService
                .Setup(r => r.Get(It.Is<Guid>(s => s == testCorrection.CorrectionTypeId), It.IsAny<Guid?>()))
                .ReturnsAsync(new CorrectionWorkflowSettingsModel()
                {
                    Name = expectedCorrectionTypeDisplayName,
                    Description = expectedCorrectionDescriptionDisplayName
                });

            var testFirstWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection,
                ApproverGroupId = firstApproverGroup.Id
            };

            var testSecondWorkflow = new CorrectionWorkflow
            {
                Id = Guid.NewGuid(),
                Correction = testCorrection,
                ApproverGroupId = secondApproverGroup.Id
            };

            var testWorkflows = new List<CorrectionWorkflow>
            {
                testFirstWorkflow,
                testSecondWorkflow
            };

            testCorrection.CorrectionWorkflows = testWorkflows;

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            var dbSet = new FakeDbSet<Answer>(Enumerable.Empty<Answer>());

            MockContext.Setup(ctx => ctx.Answers)
                .Returns(dbSet.Object);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testQuestionnaire);

            var result = await Repository
                .GetCorrectionWorkflow(testFirstWorkflow.Id, new List<StudyRoleModel>(), Guid.NewGuid(), testLocale);

            var resultCorrection = result.Correction;

            Assert.AreEqual(testCorrection.Site.Name, resultCorrection.SiteName);
            Assert.AreEqual(expectedCorrectionTypeDisplayName, resultCorrection.CorrectionWorkflowSettings.Name);
            Assert.AreEqual(expectedCorrectionDescriptionDisplayName, resultCorrection.CorrectionWorkflowSettings.Description);

            Assert.AreEqual(2, resultCorrection.CorrectionWorkflows.Count);
            Assert.AreEqual(firstApproverGroup.Name, resultCorrection.CorrectionWorkflows.First(wf => wf.ApproverGroupId == firstApproverGroup.Id).ApproverGroupName);
            Assert.AreEqual(secondApproverGroup.Name, resultCorrection.CorrectionWorkflows.First(wf => wf.ApproverGroupId == secondApproverGroup.Id).ApproverGroupName);
        }
    }
}