using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.UnitTests.TestExtensions;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class CorrectionExtensionsTests
    {
        [TestMethod]
        public void CorrectionExtensionShouldShowPreviousValuesTrueTest()
        {
            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id
            };

            var result = correction.ShouldShowPreviousValues();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CorrectionExtensionShouldShowPreviousValuesFalseTest()
        {
            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id
            };

            var result = correction.ShouldShowPreviousValues();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionIsMultiSelectAnswerTrueTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1", 
                TableName = "Answer", 
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId", 
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id, 
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1, 
                    correctionApprovalData2
                }
            };

            var result = correction.IsMultiSelectAnswer(correction.CorrectionApprovalDatas[0]);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CorrectionExtensionIsMultiSelectAnswerFalseTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = Guid.NewGuid().ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.IsMultiSelectAnswer(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionIsMultiSelectAnswerSingleChoiceFalseTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1
                }
            };

            var result = correction.IsMultiSelectAnswer(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionIsMultiSelectAnswerFalseWhenNotAnswerCorrectionTypeTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "DiaryEntry",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1
                }
            };

            var result = correction.IsMultiSelectAnswer(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionIsMultiSelectAnswerNoCorrectionApprovalsTest()
        {

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>()
            };

            var result = correction.IsMultiSelectAnswer(new CorrectionApprovalData());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionShowMultiSelectNewValuesTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.ShowMultiSelectValues(correction.CorrectionApprovalDatas[0]);
            var stringArray = result.Replace(" ", "").Split(',');

            Assert.IsTrue(!String.IsNullOrEmpty(result));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData1.NewDisplayValue.Replace(" ", "")));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData2.NewDisplayValue.Replace(" ", "")));

        }

        [TestMethod]
        public void CorrectionExtensionShowMultiSelectReturnEmptyStringWhenNoMatchingAnswersTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = Guid.NewGuid().ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = Guid.NewGuid().ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var testCorrectionApproval = new CorrectionApprovalData
            {
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var result = correction.ShowMultiSelectValues(testCorrectionApproval);

            Assert.IsTrue(String.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void CorrectionExtensionShowMultiSelectOldValuesTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.ShowMultiSelectValues(correction.CorrectionApprovalDatas[0], true);
            var stringArray = result.Replace(" ", "").Split(',');

            Assert.IsTrue(!String.IsNullOrEmpty(result));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData1.OldDisplayValue.Replace(" ", "")));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData2.OldDisplayValue.Replace(" ", "")));

        }

        [TestMethod]
        public void CorrectionExtensionShowMultiSelectNewValuesRemoveNullsTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData3 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = null,
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2,
                    correctionApprovalData3
                }
            };

            var result = correction.ShowMultiSelectValues(correction.CorrectionApprovalDatas[0]);
            var stringArray = result.Replace(" ", "").Split(',');

            Assert.IsTrue(!String.IsNullOrEmpty(result)) ;
            Assert.IsTrue(stringArray.Length == 2);
            Assert.IsTrue(stringArray.Contains(correctionApprovalData1.NewDisplayValue.Replace(" ", "")));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData2.NewDisplayValue.Replace(" ", "")));

        }

        [TestMethod]
        public void CorrectionExtensionShowMultiSelectOldValuesRemoveNullsTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData3 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = null,
                NewDisplayValue = null,
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2,
                    correctionApprovalData3
                }
            };

            var result = correction.ShowMultiSelectValues(correction.CorrectionApprovalDatas[0], true);
            var stringArray = result.Replace(" ", "").Split(',');

            Assert.IsTrue(!String.IsNullOrEmpty(result));
            Assert.IsTrue(stringArray.Length == 2);
            Assert.IsTrue(stringArray.Contains(correctionApprovalData1.OldDisplayValue.Replace(" ", "")));
            Assert.IsTrue(stringArray.Contains(correctionApprovalData2.OldDisplayValue.Replace(" ", "")));

        }

        [TestMethod]
        public void CorrectionExtensionMultiSelectAnswersUpdatedTrueTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                NewDataPoint = Guid.NewGuid().ToString(),
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                },
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.MultiSelectAnswerUpdated(correction.CorrectionApprovalDatas[0]);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CorrectionExtensionMultiSelectAnswersUpdatedFalseTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                },
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.MultiSelectAnswerUpdated(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionMultiSelectAnswersUpdatedRemoveItemTrueTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                RemoveItem = true,
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                NewDataPoint = Guid.NewGuid().ToString(),
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                },
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.MultiSelectAnswerUpdated(correction.CorrectionApprovalDatas[0]);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CorrectionExtensionMultiSelectAnswersUpdatedFalseWhenNotAnswerCorrectionTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Visit",
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Visit",
                NewDataPoint = Guid.NewGuid().ToString(),
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.MultiSelectAnswerUpdated(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionMultiSelectAnswersUpdatedFalseWhenNoAnswersFoundTest()
        {
            var questionId = Guid.NewGuid();
            var correctionApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 1",
                NewDisplayValue = "new value 1",
                TableName = "Answer",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = questionId.ToString()
                    }
                }
            };

            var correctionApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDisplayValue = "old value 2",
                NewDisplayValue = "new value 2",
                TableName = "Answer",
                NewDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = true,
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnName = "QuestionId",
                        ColumnValue = Guid.NewGuid().ToString()
                    }
                },
            };

            var correction = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    correctionApprovalData1,
                    correctionApprovalData2
                }
            };

            var result = correction.MultiSelectAnswerUpdated(correction.CorrectionApprovalDatas[0]);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CorrectionExtensionToDiaryEntryTest()
        {
            var correctionId = Guid.NewGuid();
            var questionnaireId = Guid.NewGuid();
            var diaryDate = DateTimeOffset.Now.AddDays(-5);
            var visitId = Guid.NewGuid();
            var configId = Guid.NewGuid();

            var freeTextAnswerApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.FreeTextAnswer),
                NewDataPoint = "Test free text answer"
            };

            var choiceAnswerApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = Guid.NewGuid().ToString()
            };

            var visitIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.VisitId),
                NewDataPoint = visitId.ToString()
            };

            var questionnaireIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.QuestionnaireId),
                NewDataPoint = questionnaireId.ToString()
            };

            var diaryDateApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.DiaryDate),
                NewDataPoint = diaryDate.ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    freeTextAnswerApprovalData,
                    choiceAnswerApprovalData,
                    visitIdApprovalData,
                    questionnaireIdApprovalData,
                    diaryDateApprovalData
                }
            };

            var result = correction.ToDiaryEntry();

            Assert.AreEqual(correction.PatientId, result.PatientId);
            Assert.AreEqual(visitId, result.VisitId);
            Assert.AreEqual(questionnaireId, result.QuestionnaireId);
            Assert.AreEqual(correction.StartedByUserId, result.UserId);
            Assert.AreEqual(diaryDate.Date, result.DiaryDate.Date);
            Assert.AreEqual(configId, result.ConfigurationId);
            Assert.AreEqual(DiaryStatus.Modified.Id, result.DiaryStatusId);
            Assert.AreEqual(DataSource.Paper.Id, result.DataSourceId);
            Assert.AreEqual(DateTimeOffset.Now.Date, result.StartedTime);
            Assert.AreEqual(DateTimeOffset.Now.Date, result.CompletedTime);
            Assert.That.AreCloseInSeconds(DateTimeOffset.Now, result.TransmittedTime, 5);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(Answer)),
                result.Answers.Count);

            Assert.IsTrue(result.Answers.All(a => a.DiaryEntryId == result.Id));
            Assert.IsTrue(result.Answers.All(a => a.ConfigurationId == configId));

            var freeTextAnswer = result.Answers.First(a => a.QuestionId == freeTextAnswerApprovalData.RowId);
            Assert.AreEqual(freeTextAnswerApprovalData.NewDataPoint, freeTextAnswer.FreeTextAnswer);
            Assert.IsNull(freeTextAnswer.ChoiceId);

            var choiceAnswer = result.Answers.First(a => a.QuestionId == choiceAnswerApprovalData.RowId);
            Assert.AreEqual(choiceAnswerApprovalData.NewDataPoint, choiceAnswer.ChoiceId.ToString());
            Assert.IsNull(choiceAnswer.FreeTextAnswer);
        }

        [TestMethod]
        public void CorrectionExtensionToDiaryEntryCheckboxAnswerTest()
        {
            var correctionId = Guid.NewGuid();
            var questionnaireId = Guid.NewGuid();
            var diaryDate = DateTimeOffset.Now.AddDays(-5);
            var visitId = Guid.NewGuid();

            var checkboxSelectionOne = Guid.NewGuid();
            var checkboxSelectionTwo = Guid.NewGuid();

            var choiceAnswerApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = $"{checkboxSelectionOne.ToString()},{checkboxSelectionTwo.ToString()}"
            };

            var visitIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.VisitId),
                NewDataPoint = visitId.ToString()
            };

            var questionnaireIdApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.QuestionnaireId),
                NewDataPoint = questionnaireId.ToString()
            };

            var diaryDateApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(DiaryEntry),
                ColumnName = nameof(DiaryEntry.DiaryDate),
                NewDataPoint = diaryDate.ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    choiceAnswerApprovalData,
                    visitIdApprovalData,
                    questionnaireIdApprovalData,
                    diaryDateApprovalData
                }
            };

            var result = correction.ToDiaryEntry();

            Assert.AreEqual(correction.PatientId, result.PatientId);
            Assert.AreEqual(visitId, result.VisitId);
            Assert.AreEqual(questionnaireId, result.QuestionnaireId);
            Assert.AreEqual(correction.StartedByUserId, result.UserId);
            Assert.AreEqual(diaryDate.Date, result.DiaryDate.Date);
            Assert.AreEqual(result.ConfigurationId, Config.Defaults.ConfigurationVersions.InitialVersion.Id);
            Assert.AreEqual(DiaryStatus.Modified.Id, result.DiaryStatusId);
            Assert.AreEqual(DataSource.Paper.Id, result.DataSourceId);
            Assert.AreEqual(DateTimeOffset.Now.Date, result.StartedTime);
            Assert.AreEqual(DateTimeOffset.Now.Date, result.CompletedTime);
            Assert.That.AreCloseInSeconds(DateTimeOffset.Now, result.TransmittedTime, 5);

            Assert.AreEqual(2, result.Answers.Count);

            Assert.IsTrue(result.Answers.All(a => a.DiaryEntryId == result.Id));
            Assert.IsTrue(result.Answers.All(a => a.ConfigurationId == Config.Defaults.ConfigurationVersions.InitialVersion.Id));

            var choiceAnswers = result.Answers.Where(a => a.QuestionId == choiceAnswerApprovalData.RowId);

            Assert.IsTrue(choiceAnswers.All(ca => ca.FreeTextAnswer == null));
            Assert.AreEqual(1, choiceAnswers.Count(ca => ca.ChoiceId == checkboxSelectionOne));
            Assert.AreEqual(1, choiceAnswers.Count(ca => ca.ChoiceId == checkboxSelectionTwo));
        }

        [TestMethod]
        public void CorrectionExtensionToPatientAttributeTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientAttributeConfigDetailId = Guid.NewGuid();

            var patientAttributeApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailId.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientAttributeApprovalData,
                }
            };

            var result = correction.ToPatientAttribute(new List<PatientAttribute>());
            var attribute = result.First();

            Assert.AreEqual(correction.PatientId, attribute.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailId, attribute.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                result.Count);
            
            Assert.AreEqual(patientAttributeApprovalData.NewDataPoint, attribute.AttributeValue);
            Assert.IsTrue(attribute.Id != Guid.Empty);
        }

        [TestMethod]
        public void CorrectionExtensionToPatientAttributeMultipleTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientAttributeConfigDetailIdFirst = Guid.NewGuid();
            var patientAttributeConfigDetailIdSecond = Guid.NewGuid();

            var patientAttributeApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdFirst.ToString()
                    }
                }
            };

            var patientAttributeApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute 2",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdSecond.ToString()
                    }
                }
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientAttributeApprovalDataFirst,
                    patientAttributeApprovalDataSecond
                }
            };

            var result = correction.ToPatientAttribute(new List<PatientAttribute>());
            var attribute = result.First();

            Assert.AreEqual(correction.PatientId, attribute.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdFirst, attribute.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                result.Count);

            Assert.AreEqual(patientAttributeApprovalDataFirst.NewDataPoint, attribute.AttributeValue);
            Assert.IsTrue(attribute.Id != Guid.Empty);

            var attributeSecond = result.Last();

            Assert.AreEqual(correction.PatientId, attributeSecond.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdSecond, attributeSecond.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                result.Count);

            Assert.AreEqual(patientAttributeApprovalDataSecond.NewDataPoint, attributeSecond.AttributeValue);
            Assert.IsTrue(attributeSecond.Id != Guid.Empty);
        }

        [TestMethod]
        public void CorrectionExtensionToPatientAttributeNewAndExistingTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientAttributeConfigDetailIdFirst = Guid.NewGuid();
            var patientAttributeConfigDetailIdSecond = Guid.NewGuid();
            var patientAttributeIdSecond = Guid.NewGuid();
            var existingSyncVersion = 9;
            var expectedSyncVersion = 10;

            var patientAttributeApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdFirst.ToString()
                    }
                }
            };

            var patientAttributeApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = patientAttributeIdSecond,
                TableName = nameof(PatientAttribute),
                ColumnName = nameof(PatientAttribute.AttributeValue),
                NewDataPoint = "Test attribute 2",
                CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                {
                    new CorrectionApprovalDataAdditional
                    {
                        ColumnValue = patientAttributeConfigDetailIdSecond.ToString()
                    }
                }
            };

            var existingMatchingAttribute = new PatientAttribute
            {
                Id = patientAttributeIdSecond,
                SyncVersion = existingSyncVersion
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = Guid.NewGuid(),
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientAttributeApprovalDataFirst,
                    patientAttributeApprovalDataSecond
                }
            };

            var result = correction.ToPatientAttribute(new List<PatientAttribute> { existingMatchingAttribute } );
            var attribute = result.First();

            Assert.AreEqual(correction.PatientId, attribute.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdFirst, attribute.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                result.Count);

            Assert.AreEqual(patientAttributeApprovalDataFirst.NewDataPoint, attribute.AttributeValue);
            Assert.AreEqual(1, attribute.SyncVersion);
            Assert.IsTrue(attribute.Id != Guid.Empty);
            
            var attributeSecond = result.Last();

            Assert.AreEqual(correction.PatientId, attributeSecond.PatientId);
            Assert.AreEqual(patientAttributeConfigDetailIdSecond, attributeSecond.PatientAttributeConfigurationDetailId);

            Assert.AreEqual(
                correction.CorrectionApprovalDatas.Count(cad => cad.TableName == nameof(PatientAttribute)),
                result.Count);

            Assert.AreEqual(patientAttributeApprovalDataSecond.NewDataPoint, attributeSecond.AttributeValue);
            Assert.AreEqual(expectedSyncVersion, attributeSecond.SyncVersion);
            Assert.IsTrue(attributeSecond.Id == patientAttributeIdSecond);
        }

        [TestMethod]
        public void CorrectionExtensionUpdatePatientDataTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var patientStatusTypeId = 99;
            var patientEnrolledDate = new DateTime(2020, 10, 5, 0, 0, 0);
            var patientLanguageId = Guid.NewGuid();

            var patientApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientNumber),
                NewDataPoint = "10001"
            };

            var patientApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientStatusTypeId),
                NewDataPoint = patientStatusTypeId.ToString()
            };

            var patientApprovalDataThird = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.EnrolledDate),
                NewDataPoint = patientEnrolledDate.ToString(CorrectionConstants.EnrolledDateFormat)
            };

            var patientApprovalDataFourth = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.LanguageId),
                NewDataPoint = patientLanguageId.ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientApprovalDataFirst,
                    patientApprovalDataSecond,
                    patientApprovalDataThird,
                    patientApprovalDataFourth
                }
            };

            var patient = new Patient
            {
                Id = patientId
            };

            correction.UpdatePatientData(patient);

            Assert.AreEqual(patientApprovalDataFirst.NewDataPoint, patient.PatientNumber);
            Assert.AreEqual(patientApprovalDataSecond.NewDataPoint, patient.PatientStatusTypeId.ToString());
            Assert.AreEqual(patientEnrolledDate, patient.EnrolledDate);
            Assert.AreEqual(patientLanguageId, patient.LanguageId);
        }

        [TestMethod]
        public void CorrectionExtensionUpdatePatientDataInvalidPatientStatusTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var patientStatusTypeId = "Test";

            var patientApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientNumber),
                NewDataPoint = "10001"
            };

            var patientApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientStatusTypeId),
                NewDataPoint = patientStatusTypeId.ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientApprovalDataFirst,
                    patientApprovalDataSecond
                }
            };

            var patient = new Patient
            {
                Id = patientId,
                PatientStatusTypeId = 1
            };

            correction.UpdatePatientData(patient);

            Assert.AreEqual(patientApprovalDataFirst.NewDataPoint, patient.PatientNumber);
            Assert.AreEqual(1, patient.PatientStatusTypeId);
        }

        [TestMethod]
        public void CorrectionExtensionUpdatePatientDataInvalidModelTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var patientApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Site),
                ColumnName = nameof(Site.SiteNumber),
                NewDataPoint = "10001"
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientApprovalDataFirst
                }
            };

            var patient = new Patient
            {
                Id = patientId,
                PatientStatusTypeId = 1, 
                PatientNumber = "999"
            };

            correction.UpdatePatientData(patient);

            Assert.AreEqual(1, patient.PatientStatusTypeId);
            Assert.AreEqual("999", patient.PatientNumber);
        }

        [TestMethod]
        public void CorrectionExtensionUpdatePatientDataInvalidDataPropertyTest()
        {
            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var patientStatusTypeId = 99;

            var patientApprovalDataFirst = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientNumber),
                NewDataPoint = "10001"
            };

            var patientApprovalDataSecond = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.PatientStatusTypeId),
                NewDataPoint = patientStatusTypeId.ToString()
            };

            var patientApprovalDataThird = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.Id),
                NewDataPoint = Guid.NewGuid().ToString()
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientApprovalDataFirst,
                    patientApprovalDataSecond,
                    patientApprovalDataThird
                }
            };

            var patient = new Patient
            {
                Id = patientId,
                PatientStatusTypeId = 1
            };

            correction.UpdatePatientData(patient);

            Assert.AreEqual(patientApprovalDataFirst.NewDataPoint, patient.PatientNumber);
            Assert.AreEqual(patientApprovalDataSecond.NewDataPoint, patient.PatientStatusTypeId.ToString());
            Assert.AreNotEqual(patientApprovalDataThird.NewDataPoint, patient.Id);
        }

        [TestMethod]
        public void CorrectionExtensionUpdatePatientData_EnrollDateTimezoneTest()
        {
            //ensure that the timezone of the enrollment date is preserved correctly

            var correctionId = Guid.NewGuid();
            var configId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var dateOffset = new TimeSpan(-6, 0, 0);

            var patientEnrolledDate = new DateTimeOffset(2020, 10, 5, 0, 0, 0, dateOffset);

            var patientApprovalDataEnrollDate = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                TableName = nameof(Patient),
                ColumnName = nameof(Patient.EnrolledDate),
                NewDataPoint = patientEnrolledDate.ToString(CorrectionConstants.EnrolledDateFormat + $" 00:00:00 {(dateOffset.Hours > 0 ? "+" : "")}{dateOffset.Hours}:{dateOffset.Minutes:00}")
            };

            var correction = new Correction
            {
                Id = correctionId,
                PatientId = patientId,
                StartedByUserId = Guid.NewGuid(),
                ConfigurationId = configId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    patientApprovalDataEnrollDate
                }
            };

            var patient = new Patient
            {
                Id = patientId
            };

            correction.UpdatePatientData(patient);

            Assert.AreEqual(patientEnrolledDate, patient.EnrolledDate);
        }
    }
}