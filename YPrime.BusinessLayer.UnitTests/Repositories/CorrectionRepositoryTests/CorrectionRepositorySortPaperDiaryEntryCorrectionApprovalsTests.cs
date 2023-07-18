using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositorySortPaperDiaryEntryCorrectionApprovalsTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryQuestionsTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var diaryPage2 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 2
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId, 
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestion,
                secondQuestion
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1
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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionCorrectionDataId, testCorrection.CorrectionApprovalDatas[3].Id);
            Assert.AreEqual(secondQuestionCorrectionDataId, testCorrection.CorrectionApprovalDatas[4].Id);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryMultipleQuestionsPerPageTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";;
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var firstQuestionPage1CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage1CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage1CorrectionDataId = Guid.NewGuid();

            var firstQuestionPage2CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage2CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage2CorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var diaryPage2 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 2
            };

            var firstQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestionPage1,
                secondQuestionPage1,
                thirdQuestionPage1
            };

            var firstQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage2.Questions = new List<QuestionModel>
            {
                firstQuestionPage2,
                secondQuestionPage2,
                thirdQuestionPage2
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1,
                    diaryPage2
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
                    Id = secondQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage1.Id.ToString()
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
                    Id = secondQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage2.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage2.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage2.Id.ToString()
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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[3].RowId);
            Assert.AreEqual(secondQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[4].RowId);
            Assert.AreEqual(thirdQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[5].RowId);
            Assert.AreEqual(firstQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[6].RowId);
            Assert.AreEqual(secondQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[7].RowId);
            Assert.AreEqual(thirdQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[8].RowId);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryMultipleQuestionSinglePageTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var firstQuestionPage1CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage1CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage1CorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestionPage1,
                secondQuestionPage1,
                thirdQuestionPage1
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1
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
                    Id = secondQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = diaryDateCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.DiaryDate),
                    NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy"),

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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[3].RowId);
            Assert.AreEqual(secondQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[4].RowId);
            Assert.AreEqual(thirdQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[5].RowId);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryMultipleQuestionPageAndSingleQuestionPageTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var firstQuestionPage1CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage1CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage1CorrectionDataId = Guid.NewGuid();

            var firstQuestionPage2CorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var diaryPage2 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 2
            };

            var firstQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestionPage1,
                secondQuestionPage1,
                thirdQuestionPage1
            };

            var firstQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage2.Questions = new List<QuestionModel>
            {
                firstQuestionPage2
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1,
                    diaryPage2
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
                    Id = secondQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage1.Id.ToString()
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
                    Id = firstQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage2.Id.ToString()
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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[3].RowId);
            Assert.AreEqual(secondQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[4].RowId);
            Assert.AreEqual(thirdQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[5].RowId);
            Assert.AreEqual(firstQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[6].RowId);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryNullQuestionsTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var firstQuestionPage1CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage1CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage1CorrectionDataId = Guid.NewGuid();

            var firstQuestionPage2CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage2CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage2CorrectionDataId = Guid.NewGuid();
            var notFoundQuestionId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var diaryPage2 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 2
            };

            var firstQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestionPage1,
                secondQuestionPage1,
                thirdQuestionPage1
            };

            var firstQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage2.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage2.Questions = new List<QuestionModel>
            {
                firstQuestionPage2,
                secondQuestionPage2,
                thirdQuestionPage2
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1,
                    diaryPage2
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
                    Id = secondQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = notFoundQuestionId,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = notFoundQuestionId.ToString()
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
                    Id = secondQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage2.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage2.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage2CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage2.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage2.Id.ToString()
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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(secondQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[3].RowId);
            Assert.AreEqual(thirdQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[4].RowId);
            Assert.AreEqual(firstQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[5].RowId);
            Assert.AreEqual(secondQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[6].RowId);
            Assert.AreEqual(thirdQuestionPage2.Id, testCorrection.CorrectionApprovalDatas[7].RowId);
            Assert.AreEqual(notFoundQuestionId, testCorrection.CorrectionApprovalDatas[8].RowId);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiarySingleQuestionTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();
            var firstQuestionCorrectionDataId = Guid.NewGuid();
            var secondQuestionCorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestion
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1
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

            var testQuestions = new List<QuestionModel>
            {
                firstQuestion
            };

            var testWorkFlowDbSet = new FakeDbSet<CorrectionWorkflow>(testWorkflows);

            MockContext
                .Setup(c => c.CorrectionWorkflows)
                .Returns(testWorkFlowDbSet.Object);

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(It.Is<Guid>(id => id == testQuestionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testQuestionnaire);

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionCorrectionDataId, testCorrection.CorrectionApprovalDatas[3].Id);
        }

        [TestMethod]
        public async Task CorrectionRepositorySortPaperDiaryEntryNullCorrectionApprovalDataAdditionalTest()
        {
            var expectedCorrectionTypeDisplayName = "Paper Entry DCF";
            var expectedCorrectionDescriptionDisplayName = "Paper Entry DCF Description";
            var testQuestionnaireId = Guid.NewGuid();

            var questionnaireIdCorrectionDataId = Guid.NewGuid();
            var visitIdCorrectionDataId = Guid.NewGuid();
            var diaryDateCorrectionDataId = Guid.NewGuid();

            var firstQuestionPage1CorrectionDataId = Guid.NewGuid();
            var secondQuestionPage1CorrectionDataId = Guid.NewGuid();
            var thirdQuestionPage1CorrectionDataId = Guid.NewGuid();

            var diaryPage1 = new DiaryPageModel
            {
                Id = Guid.NewGuid(),
                Number = 1
            };

            var firstQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var secondQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            var thirdQuestionPage1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 3,
                QuestionnaireId = testQuestionnaireId,
                PageId = diaryPage1.Id,
                QuestionType = InputFieldType.SingleSelectCheckBox.Id
            };

            diaryPage1.Questions = new List<QuestionModel>
            {
                firstQuestionPage1,
                secondQuestionPage1,
                thirdQuestionPage1
            };

            var testQuestionnaire = new QuestionnaireModel
            {
                Id = testQuestionnaireId,
                Pages = new List<DiaryPageModel>
                {
                    diaryPage1
                }
            };

            var outOfOrderCorrectionApprovalDatas = new List<CorrectionApprovalData>
            {
                new CorrectionApprovalData
                {
                    Id = visitIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.VisitId),
                    NewDataPoint = "1",
                    CorrectionApprovalDataAdditionals = null
                },
                new CorrectionApprovalData
                {
                    Id = secondQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = secondQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = secondQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = thirdQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = thirdQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = thirdQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = firstQuestionPage1CorrectionDataId,
                    TableName = nameof(Answer),
                    RowId = firstQuestionPage1.Id,
                    NewDataPoint = "testing",
                    CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                    {
                        new CorrectionApprovalDataAdditional
                        {
                            ColumnName = "QuestionId",
                            ColumnValue = firstQuestionPage1.Id.ToString()
                        }
                    }
                },
                new CorrectionApprovalData
                {
                    Id = diaryDateCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.DiaryDate),
                    NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy"),
                    CorrectionApprovalDataAdditionals = null
                },
                new CorrectionApprovalData
                {
                    Id = questionnaireIdCorrectionDataId,
                    TableName = nameof(DiaryEntry),
                    ColumnName = nameof(DiaryEntry.QuestionnaireId),
                    NewDataPoint = testQuestionnaireId.ToString(),
                    CorrectionApprovalDataAdditionals = null
                },
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

            await Repository.SortDiaryEntryCorrectionApprovals(testCorrection);

            Assert.AreEqual(questionnaireIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[0].Id);
            Assert.AreEqual(diaryDateCorrectionDataId, testCorrection.CorrectionApprovalDatas[1].Id);
            Assert.AreEqual(visitIdCorrectionDataId, testCorrection.CorrectionApprovalDatas[2].Id);
            Assert.AreEqual(firstQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[3].RowId);
            Assert.AreEqual(secondQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[4].RowId);
            Assert.AreEqual(thirdQuestionPage1.Id, testCorrection.CorrectionApprovalDatas[5].RowId);
        }
    }
}
