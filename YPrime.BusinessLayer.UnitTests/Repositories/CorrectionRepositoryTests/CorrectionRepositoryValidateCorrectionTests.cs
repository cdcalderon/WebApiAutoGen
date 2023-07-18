using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryValidateCorrectionTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionNonValidatedTypeTest()
        {
            SetPatientNumberValidationResult(true);

            var testCorrection = new Correction
            {
                CorrectionTypeId = Guid.NewGuid()
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionNoPatientIdSubjectAttributeTest()
        {
            SetPatientNumberValidationResult(true);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                PatientId = null
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionSubjectAttributeValidTest()
        {
            SetPatientNumberValidationResult(true);

            const string firstDataPointNewData = "First Data Updated";
            const string secondDataPointNewData = "Second Data Updated";

            var passedInPatientAttributes = new List<PatientAttributeDto>();

            MockPatientRepository
                .Setup(r => r.ValidatePatientAttributesFromDetail(It.IsAny<List<PatientAttributeDto>>(), It.IsAny<ModelStateDictionary>(), It.IsAny<DateTimeOffset>(), It.IsAny<Guid>(), It.Is<bool>(b => b), It.Is<bool>(b => b)))
                .ReturnsAsync(true)
                .Callback<List<PatientAttributeDto>, ModelStateDictionary, DateTimeOffset, Guid, bool, bool>((passedInAttributes, passedInModelState, passedInSiteTime, passedInIsCorrection, patientId, useConfig) =>
                {
                    passedInPatientAttributes = passedInAttributes;
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                PatientId = Guid.NewGuid(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = firstDataPointNewData,
                        RowId = FirstAttribute.Id
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = secondDataPointNewData,
                        RowId = SecondAttribute.Id
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(2, passedInPatientAttributes.Count);

            var firstPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == FirstAttribute.Id);

            Assert.AreEqual(firstDataPointNewData, firstPassedInAttribute.AttributeValue);

            var secondPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == SecondAttribute.Id);

            Assert.AreEqual(secondDataPointNewData, secondPassedInAttribute.AttributeValue);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionSubjectAttributeNewAttributeTest()
        {
            SetPatientNumberValidationResult(true);

            const string firstDataPointNewData = "First Data Updated";
            const string secondDataPointNewData = "Second Data Updated";
            const string thirdDataPointNewData = "Third Data Updated";

            var passedInPatientAttributes = new List<PatientAttributeDto>();

            MockPatientRepository
                .Setup(r => r.ValidatePatientAttributesFromDetail(It.IsAny<List<PatientAttributeDto>>(), It.IsAny<ModelStateDictionary>(), It.IsAny<DateTimeOffset>(), It.IsAny<Guid>(), It.Is<bool>(b => b), It.Is<bool>(b => b)))
                .ReturnsAsync(true)
                .Callback<List<PatientAttributeDto>, ModelStateDictionary, DateTimeOffset, Guid, bool, bool>((passedInAttributes, passedInModelState, passedInSiteTime, passedInIsCorrection, patientId, useConfig) =>
                {
                    passedInPatientAttributes = passedInAttributes;
                });

            var ThirdAttribute = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                NewAttributeData = true,
                PatientAttributeConfigurationDetailId = Guid.NewGuid()
            };

            BaseAttributes.Add(ThirdAttribute);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                PatientId = Guid.NewGuid(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = firstDataPointNewData,
                        RowId = FirstAttribute.Id
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = secondDataPointNewData,
                        RowId = SecondAttribute.Id
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = thirdDataPointNewData,
                        RowId = SecondAttribute.Id,
                        CorrectionApprovalDataAdditionals = new List<CorrectionApprovalDataAdditional>
                        {
                            new CorrectionApprovalDataAdditional
                            {
                                ColumnValue = ThirdAttribute.PatientAttributeConfigurationDetailId.ToString()
                            }
                        }
                    }
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(3, passedInPatientAttributes.Count);

            var firstPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == FirstAttribute.Id);

            Assert.AreEqual(firstDataPointNewData, firstPassedInAttribute.AttributeValue);

            var secondPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == SecondAttribute.Id);

            Assert.AreEqual(secondDataPointNewData, secondPassedInAttribute.AttributeValue);

            var thirdPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == ThirdAttribute.Id);

            Assert.AreEqual(thirdDataPointNewData, thirdPassedInAttribute.AttributeValue);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionSubjectAttributeSkipEmptyTest()
        {
            SetPatientNumberValidationResult(true);

            const string firstDataPointNewData = "First Data Updated";

            var passedInPatientAttributes = new List<PatientAttributeDto>();

            MockPatientRepository
                .Setup(r => r.ValidatePatientAttributesFromDetail(It.IsAny<List<PatientAttributeDto>>(), It.IsAny<ModelStateDictionary>(), It.IsAny<DateTimeOffset>(), It.IsAny<Guid>(), It.Is<bool>(b => b), It.Is<bool>(b => b)))
                .ReturnsAsync(true)
                .Callback<List<PatientAttributeDto>, ModelStateDictionary, DateTimeOffset, Guid, bool, bool>((passedInAttributes, passedInModelState, passedInSiteTime, passedInIsCorrection, patientId, useConfig) =>
                {
                    passedInPatientAttributes = passedInAttributes;
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeSubjectInfo.Id,
                PatientId = Guid.NewGuid(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = firstDataPointNewData,
                        RowId = FirstAttribute.Id
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(PatientAttribute),
                        NewDataPoint = string.Empty,
                        RowId = SecondAttribute.Id
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, passedInPatientAttributes.Count);

            var firstPassedInAttribute = passedInPatientAttributes.Single(a => a.Id == FirstAttribute.Id);

            Assert.AreEqual(firstDataPointNewData, firstPassedInAttribute.AttributeValue);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseValidTest()
        {
            const string firstDataPointNewData = "08.25";
            const string secondDataPointNewData = "097";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = "2",
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,

                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseInvalidTest()
        {
            const string firstDataPointNewData = "8.225";
            const string secondDataPointNewData = "97.1";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = "2",
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(2, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireInvalidDecimalsTest()
        {
            const string firstDataPointNewData = "00.0";
            const string secondDataPointNewData = "11.0";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "11",
                    MaxValue = "99",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseNrsValidTest()
        {
            const string firstDataPointNewData = "6";
            const string secondDataPointNewData = "9";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NRS.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NRS.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseNrsInvalidTest()
        {
            const string firstDataPointNewData = "12";
            const string secondDataPointNewData = "a";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NRS.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "10",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NRS.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(2, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseVasValidTest()
        {
            const string firstDataPointNewData = "98.00";
            const string secondDataPointNewData = "5";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = "2",
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.VAS.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.VAS.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseVasInvalidTest()
        {
            const string firstDataPointNewData = "8.22";
            const string secondDataPointNewData = "101";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.VAS.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.VAS.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(2, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseEqValidTest()
        {
            const string firstDataPointNewData = "100.00";
            const string secondDataPointNewData = "0";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = "2",
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1, testQuestion2 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseEqInvalidTest()
        {
            const string firstDataPointNewData = "8.2";
            const string secondDataPointNewData = "9";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "10",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, modelState.Count);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }



        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseEQ5InvalidTest()
        {
            const string firstDataPointNewData = "8.2";
            const string secondDataPointNewData = "9";

            var questionnaireId = Guid.NewGuid();

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 1",
                QuestionType = InputFieldType.EQ5D5L.Id
            };

            var testQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "10",
                    MaxValue = "100",
                    DecimalValue = null,
                },
                QuestionText = "Question 2",
                QuestionType = InputFieldType.EQ5D5L.Id
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = secondDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion2.Id,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, modelState.Count);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));

            MockQuestionnaireService
                .Verify(qs => qs.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateCorrectionQuestionnaireResponseMissingTranslationTest()
        {
            MockTranslationService.Reset();

            var questionnaireId = Guid.NewGuid();

            const string firstDataPointNewData = "51";

            var questionnaireIdsPassedToQuestionnaireService = new List<Guid>();

            var testQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionText = "Question 1",
                QuestionType = InputFieldType.NumberSpinner.Id,
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = "0",
                    MaxValue = "100",
                    DecimalValue = null,
                },
            };

            MockQuestionnaireService
                .Setup(r => r.GetQuestions(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<QuestionModel> { testQuestion1 })
                .Callback((Guid passedInQuestionId, Guid? passedInConfigId) =>
                {
                    questionnaireIdsPassedToQuestionnaireService.Add(passedInQuestionId);
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangeQuestionnaireResponses.Id,
                PatientId = Guid.NewGuid(),
                SiteId = TestSite.Id,
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        NewDataPoint = firstDataPointNewData,
                        Id = Guid.NewGuid(),
                        QuestionId = testQuestion1.Id,
                    }
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(1, questionnaireIdsPassedToQuestionnaireService.Count);
            Assert.AreEqual(0, modelState.Count);

            Assert.IsTrue(questionnaireIdsPassedToQuestionnaireService.Contains(questionnaireId));
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateDiaryEntryValidTest()
        {
            MockTranslationService.Reset();

            var questionnaireId = Guid.NewGuid();

            var requiredTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.TextArea.Id,
                QuestionText = "Test Required Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = true
                }
            };

            var notRequiredTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.NumberSpinner.Id,
                QuestionText = "Test Not Required Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = false
                }
            };

            var noneTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.None.Id,
                QuestionText = "Test None Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = true
                }
            };

            var testQuestions = new List<QuestionModel>
            {
                requiredTestQuestion,
                notRequiredTestQuestion,
                noneTestQuestion
            };

            MockQuestionnaireService
                .Setup(r => r.GetInflatedQuestionnaire(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(new QuestionnaireModel
                {
                    QuestionnaireTypeId = QuestionnaireType.Clinician.Id,
                    Questions = testQuestions,
                    Pages = new List<DiaryPageModel>
                    {
                        new DiaryPageModel
                        {
                            Questions = testQuestions
                        }
                    }
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                PatientId = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.VisitId),
                        NewDataPoint = "98",
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.DiaryDate),
                        NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy"),
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.QuestionnaireId),
                        NewDataPoint = questionnaireId.ToString(),
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(Answer),
                        RowId = requiredTestQuestion.Id,
                        NewDataPoint = "Test response",
                        Id = Guid.NewGuid(),
                    }
                },
                SiteId = TestSite.Id
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(0, modelState.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateDiaryEntryInvalidTest()
        {
            MockTranslationService.Reset();

            var questionnaireId = Guid.NewGuid();

            var requiredTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.TextArea.Id,
                QuestionText = "Test Required Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = true
                }
            };

            var notRequiredTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.NumberSpinner.Id,
                QuestionText = "Test Not Required Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = true
                }
            };

            var noneTestQuestion = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionType = InputFieldType.None.Id,
                QuestionText = "Test None Question",
                QuestionSettings = new QuestionSettingsModel
                {
                    IsRequired = true
                }
            };

            var testQuestions = new List<QuestionModel>
            {
                requiredTestQuestion,
                notRequiredTestQuestion,
                noneTestQuestion
            };

            MockQuestionnaireService
                .Setup(r => r.GetInflatedQuestionnaire(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(new QuestionnaireModel
                {
                    Id = questionnaireId,
                    QuestionnaireTypeId = QuestionnaireType.Clinician.Id,
                    Questions = testQuestions,
                    Pages = new List<DiaryPageModel>
                    {
                        new DiaryPageModel
                        {
                            Questions = testQuestions
                        }
                    }
                });

            var testRequiredFieldErrorText = "Field is required";

            MockTranslationService
                .Setup(r => r.GetByKey(It.Is<string>(s => s == "RequiredFieldErrorSuffix"), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(testRequiredFieldErrorText);

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                PatientId = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.QuestionnaireId),
                        Id = Guid.NewGuid(),
                        NewDataPoint = questionnaireId.ToString(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.VisitId),
                        NewDataPoint = string.Empty,
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(Answer),
                        RowId = requiredTestQuestion.Id,
                        NewDataPoint = string.Empty,
                        Id = Guid.NewGuid(),
                    }
                },
                SiteId = TestSite.Id
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsFalse(result);
            Assert.AreEqual(1, modelState.Count);
            Assert.IsTrue(modelState.All(ms => ms.Value.Errors.All(e => e.ErrorMessage.Contains(testRequiredFieldErrorText))));
            Assert.IsTrue(!modelState.Any(ms => ms.Key.Contains(requiredTestQuestion.Id.ToString())));
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateDiaryEntryValidNoVisitQuestionnaireTest()
        {
            MockTranslationService.Reset();

            var questionnaireId = Guid.NewGuid();

            MockQuestionnaireService
                .Setup(r => r.GetInflatedQuestionnaire(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(new QuestionnaireModel
                {
                    Id = questionnaireId,
                    QuestionnaireTypeId = QuestionnaireType.PatientHandheld.Id,
                    Pages = new List<DiaryPageModel>
                    {
                        new DiaryPageModel
                        {
                            Questions = new List<QuestionModel>()
                        }
                    }
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                PatientId = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.VisitId),
                        NewDataPoint = string.Empty,
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.DiaryDate),
                        NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy"),
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.QuestionnaireId),
                        NewDataPoint = questionnaireId.ToString(),
                        Id = Guid.NewGuid(),
                    },
                },
                SiteId = TestSite.Id
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(0, modelState.Count);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidateDiaryEntryIsValidWhenQuestionnaireHasVisitButNotSelectedTest()
        {
            MockTranslationService.Reset();

            var questionnaireId = Guid.NewGuid();

            MockQuestionnaireService
                .Setup(r => r.GetInflatedQuestionnaire(It.Is<Guid>(id => id == questionnaireId), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(new QuestionnaireModel
                {
                    Id = questionnaireId,
                    QuestionnaireTypeId = QuestionnaireType.Patient.Id,
                    Pages = new List<DiaryPageModel>
                    {
                        new DiaryPageModel
                        {
                            Questions = new List<QuestionModel>()
                        }
                    }
                });

            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.PaperDiaryEntry.Id,
                PatientId = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.VisitId),
                        NewDataPoint = string.Empty,
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.DiaryDate),
                        NewDataPoint = DateTime.Now.ToString("dd/MMMM/yyyy"),
                        Id = Guid.NewGuid(),
                    },
                    new CorrectionApprovalData
                    {
                        TableName = nameof(DiaryEntry),
                        ColumnName = nameof(DiaryEntry.QuestionnaireId),
                        NewDataPoint = questionnaireId.ToString(),
                        Id = Guid.NewGuid(),
                    },
                },
                SiteId = TestSite.Id
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.IsTrue(result);
            Assert.AreEqual(0, modelState.Count);
        }

        [TestMethod]
        [DataRow("10-Aug-2022", "10-Aug-2022", "15-Aug-2022", "15-Aug-2022", true)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "15-Aug-2022", "05-Aug-2022", true)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "05-Aug-2022", "15-Aug-2022", false)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "", "05-Aug-2022", true)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "", "15-Aug-2022", false)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "15-Aug-2022", "", true)]
        [DataRow("10-Aug-2022", "10-Aug-2022", "05-Aug-2022", "", false)]
        public async Task CorrectionRepositoryValidateCorrectionPatientVisitActivationDateTest(
            string oldVisitDate,
            string oldActivationDate,
            string newVisitDate,
            string newActivationDate,
            bool expectedIsValid)
        {
            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangePatientVisit.Id,
                PatientId = Guid.NewGuid(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        ColumnName = CorrectionConstants.PatientVisitVisitDate,
                        NewDisplayValue = newVisitDate,
                        OldDisplayValue = oldVisitDate,
                    },
                    new CorrectionApprovalData
                    {
                        ColumnName = CorrectionConstants.PatientVisitActivationDate,
                        NewDisplayValue = newActivationDate,
                        OldDisplayValue = oldActivationDate,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var actualIsValid = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.AreEqual(expectedIsValid, actualIsValid);
            var containsValidationMessage = modelState.ContainsKey(TranslationConstants.DCFInvalidActivationDate);
            Assert.IsTrue(actualIsValid ? !containsValidationMessage : containsValidationMessage);    
        }

        [TestMethod]
        [DataRow("1", "4", "15-Aug-2022", true)]
        [DataRow("1", "4", "", false)]
        [DataRow("2", "4", "05-Aug-2022", true)]
        [DataRow("2", "4", "", false)]
        [DataRow("4", "1", "15-Aug-2022", true)]
        [DataRow("4", "2", "", true)]
        public async Task CorrectionRepositoryValidateCorrectionPatientVisitRequiredVisitDateTest(
            string oldVisitStatus,
            string newVisitStatus,
            string newVisitDate,
            bool expectedIsValid)
        {
            var testCorrection = new Correction
            {
                CorrectionTypeId = CorrectionType.ChangePatientVisit.Id,
                PatientId = Guid.NewGuid(),
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        ColumnName = CorrectionConstants.PatientVisitVisitDate,
                        NewDisplayValue = newVisitDate
                    },
                    new CorrectionApprovalData
                    {
                        ColumnName = nameof(PatientVisit.PatientVisitStatusTypeId),
                        NewDataPoint = newVisitStatus,
                        OldDataPoint = oldVisitStatus,
                    },
                }
            };

            var modelState = new ModelStateDictionary();

            var actualIsValid = await Repository.ValidateCorrection(testCorrection, modelState, DefaultCultureCode);

            Assert.AreEqual(expectedIsValid, actualIsValid);
            var containsValidationMessage = modelState.ContainsKey(TranslationConstants.RequiredFieldErrorSuffix);
            Assert.IsTrue(actualIsValid ? !containsValidationMessage : containsValidationMessage);
        }
    }
}
