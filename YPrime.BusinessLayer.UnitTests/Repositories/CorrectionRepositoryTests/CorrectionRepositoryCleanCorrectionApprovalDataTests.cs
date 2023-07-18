using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryCleanCorrectionApprovalDataTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryCleanCorrectionApprovalDataRemoveItemTest()
        {
            var dbSet = new FakeDbSet<Answer>(Enumerable.Empty<Answer>());
            MockContext.Setup(ctx => ctx.Answers)
                .Returns(dbSet.Object);
            var correctionId = Guid.NewGuid();

            // removed correction data with new data
            var approvalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                NewDataPoint = Guid.NewGuid().ToString(),
                NewDisplayValue = "NewDisplayValue",
                RemoveItem = true,
            };

            var correction = new Correction
            {
                Id = correctionId,
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    approvalData,
                }
            };

            PrivateObject obj = new PrivateObject(Repository);
            var cleanCorrectionApprovalDataTask = obj.Invoke("CleanCorrectionApprovalData", correction, new List<QuestionModel>()) as Task<List<CorrectionApprovalData>>;
            await cleanCorrectionApprovalDataTask;

            Assert.IsNull(approvalData.NewDataPoint);
            Assert.IsNull(approvalData.NewDisplayValue);
        }

        [TestMethod]
        public async Task CorrectionRepositoryCleanCorrectionApprovalDataSingleSelectTest()
        {
            var dbSet = new FakeDbSet<Answer>(Enumerable.Empty<Answer>());
            MockContext.Setup(ctx => ctx.Answers)
                .Returns(dbSet.Object);
            var correctionId = Guid.NewGuid();

            // correction data with no change
            var singleSelectApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = null,
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = false,
                AllowDelete = false
            };

            // correction data to remove
            var singleSelectApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = null,
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = true,
                AllowDelete = false
            };

            // correction data with new data
            var singleSelectApprovalData3 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = Guid.NewGuid().ToString(),
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = false,
                AllowDelete = false
            };

            var correction = new Correction
            {
                Id = correctionId,
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    singleSelectApprovalData1,
                    singleSelectApprovalData2,
                    singleSelectApprovalData3,
                }
            };

            PrivateObject obj = new PrivateObject(Repository);
            var cleanCorrectionApprovalDataTask = obj.Invoke("CleanCorrectionApprovalData", correction, new List<QuestionModel>()) as Task<List<CorrectionApprovalData>>;
            var correctionDataToSave = await cleanCorrectionApprovalDataTask;

            var expectedCorrectionDataToSave = new List<CorrectionApprovalData>
            {
                singleSelectApprovalData2,
                singleSelectApprovalData3
            };

            CollectionAssert.AreEqual(expectedCorrectionDataToSave, correctionDataToSave);
        }

        [TestMethod]
        public async Task CorrectionRepositoryCleanCorrectionApprovalDataMultiSelectTest()
        {
            var dbSet = new FakeDbSet<Answer>(Enumerable.Empty<Answer>());
            MockContext.Setup(ctx => ctx.Answers)
                .Returns(dbSet.Object);

            var correctionId = Guid.NewGuid();
            var questionId = Guid.NewGuid();

            var questionModels = new List<QuestionModel>
            {
                new QuestionModel
                {
                    Id = questionId,
                    QuestionType = InputFieldType.HotSpotMultipleSelect.Id,
                }
            };

            // previously selected option which will be removed
            var multiSelectApprovalData1 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.NewGuid(),
                QuestionId = questionId,
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = null,
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = false, 
                AllowDelete = true
            };

            // previously not selected option which will be ignored
            var multiSelectApprovalData2 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                RowId = Guid.Empty,
                QuestionId = questionId,
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = null,
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = false,
                AllowDelete = true
            };

            // correction data to remove
            var multiSelectApprovalData3 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                QuestionId = questionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = null,
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = true,
                AllowDelete = true
            };

            // reselect previously selected option which will added to the new correction data
            var multiSelectApprovalData4 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                QuestionId = questionId,
                RowId = Guid.NewGuid(),
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = Guid.NewGuid().ToString(),
                OldDataPoint = Guid.NewGuid().ToString(),
                RemoveItem = false,
                AllowDelete = true
            };

            // new selected option
            var multiSelectApprovalData5 = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = correctionId,
                QuestionId = questionId,
                RowId = Guid.Empty,
                TableName = nameof(Answer),
                ColumnName = nameof(Answer.ChoiceId),
                NewDataPoint = Guid.NewGuid().ToString(),
                OldDataPoint = null,
                RemoveItem = false,
                AllowDelete = true
            };

            var correction = new Correction
            {
                Id = correctionId,
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    multiSelectApprovalData1,
                    multiSelectApprovalData2,
                    multiSelectApprovalData3,
                    multiSelectApprovalData4,
                    multiSelectApprovalData5
                }
            };

            PrivateObject obj = new PrivateObject(Repository);
            var cleanCorrectionApprovalDataTask = obj.Invoke("CleanCorrectionApprovalData", correction, questionModels) as Task<List<CorrectionApprovalData>>;
            var correctionDataToSave = await cleanCorrectionApprovalDataTask;

            var expectedCorrectionDataToSave = new List<CorrectionApprovalData>
            {
                multiSelectApprovalData1,
                multiSelectApprovalData3,
                multiSelectApprovalData4,
                multiSelectApprovalData5
            };

            CollectionAssert.AreEqual(expectedCorrectionDataToSave, correctionDataToSave);
        }
    }
}