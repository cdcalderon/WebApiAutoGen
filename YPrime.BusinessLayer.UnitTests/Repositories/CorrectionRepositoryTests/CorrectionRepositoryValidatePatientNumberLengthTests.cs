using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositoryValidatePatientNumberLengthTests : CorrectionRepositoryTestBase
    {
        private const string SubjectTableName = "Patient";
        private const string SubjectNumberColumnName = "PatientNumber";

        [TestMethod]
        public async Task CorrectionRepositoryValidatePatientNumberLengthValidTest()
        {
            const string testPatientNumberValue = "321";
            var testFullPatientNumberValue = $"S-10001-{testPatientNumberValue}";

            MockPatientAttributeRepository
                .Setup(r => r.ExtractPatientNumber(It.Is<string>(s => s == testFullPatientNumberValue)))
                .ReturnsAsync(testPatientNumberValue);

            MockPatientRepository
                .Setup(r => r.ValidatePatientNumber(It.Is<string>(s => s == testPatientNumberValue),
                    It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(true);

            var testCorrection = new Correction
            {
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = SubjectTableName,
                        ColumnName = SubjectNumberColumnName,
                        NewDataPoint = testFullPatientNumberValue
                    }
                }
            };

            var result = await Repository.ValidatePatientNumberLength(testCorrection, new ModelStateDictionary());

            Assert.IsTrue(result);

            MockPatientAttributeRepository
                .Verify(r => r.ExtractPatientNumber(It.Is<string>(s => s == testFullPatientNumberValue)), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidatePatientNumberLengthNotValidTest()
        {
            const string testPatientNumberValue = "456";
            var testFullPatientNumberValue = $"S-10001-{testPatientNumberValue}";

            MockPatientAttributeRepository
                .Setup(r => r.ExtractPatientNumber(It.Is<string>(s => s == testFullPatientNumberValue)))
                .ReturnsAsync(testPatientNumberValue);

            MockPatientRepository
                .Setup(r => r.ValidatePatientNumber(It.Is<string>(s => s == testPatientNumberValue),
                    It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(false);

            var testCorrection = new Correction
            {
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = SubjectTableName,
                        ColumnName = SubjectNumberColumnName,
                        NewDataPoint = testFullPatientNumberValue
                    }
                }
            };

            var result = await Repository.ValidatePatientNumberLength(testCorrection, new ModelStateDictionary());

            Assert.IsFalse(result);

            MockPatientAttributeRepository
                .Verify(r => r.ExtractPatientNumber(It.Is<string>(s => s == testFullPatientNumberValue)), Times.Once);
        }

        [TestMethod]
        public async Task CorrectionRepositoryValidatePatientNumberLengthNoSubjectNumberTest()
        {
            var testCorrection = new Correction
            {
                CorrectionApprovalDatas = new List<CorrectionApprovalData>
                {
                    new CorrectionApprovalData
                    {
                        TableName = "NotSubject",
                        ColumnName = "NotSubjectNumber",
                        NewDataPoint = "Some other data point"
                    }
                }
            };

            var result = await Repository.ValidatePatientNumberLength(testCorrection, new ModelStateDictionary());

            Assert.IsTrue(result);

            MockPatientAttributeRepository
                .Verify(r => r.ExtractPatientNumber(It.IsAny<string>()), Times.Never);
        }
    }
}