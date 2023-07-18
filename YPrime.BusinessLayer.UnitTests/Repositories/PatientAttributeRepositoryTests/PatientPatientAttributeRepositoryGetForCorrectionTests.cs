using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientAttributeRepositoryTests
{
    [TestClass]
    public class PatientPatientAttributeRepositoryGetForCorrectionTests : PatientAttributeRepositoryTestBase
    {
        private const string TestPatientNumberDisplayValue = "100";
        private const string TestPatientNumberDisplayPoint = "S-10001-100";

        [TestMethod]
        public async Task PatientAttributeRepositoryGetPatientAttributesForCorrectionMultipleDigitPatientLengthTest()
        {
            for (var i = 0; i < 9; i++)
            {
                var testPatientNumberLength = i + 1;

                var testStudySetting = new StudySettingModel
                {
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberLength"
                    },
                    Value = testPatientNumberLength.ToString()
                };

                MockStudySettingService.Reset();
                MockStudySettingService
                    .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testStudySetting.Key), It.IsAny<Guid?>()))
                    .ReturnsAsync(testStudySetting.Value);

                var testCorrectionId = Guid.NewGuid();

                MockPatientRepository.Setup(r => r.GetPatient(It.Is<Guid>(y => y == TestPatient.Id), It.Is<string>(x => x == "en-US")))
                    .ReturnsAsync(TestPatientDto);

                var result = await Repository.GetPatientAttributesForCorrection(
                    TestPatient.Id,
                    testCorrectionId,
                    "en-US",
                    new List<CorrectionApprovalData>());

                Assert.AreEqual(5, result.Count);

                var patientNumberAttribute = result.FirstOrDefault(r =>
                    r.PatientAttribute.SubjectInformation.Name == SubjectNumberTranslation);

                Assert.IsNotNull(patientNumberAttribute);
                Assert.AreEqual(testPatientNumberLength, patientNumberAttribute.PatientAttribute.AttributeValue.Length);
            }
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryGetPatientAttributesForCorrectionOverMaxDigitPatientLengthTest()
        {
            const int testPatientNumberLength = 10;
            const int expectedPatientNumberLength = 9;

            var testStudySetting = new StudySettingModel
            {
                Properties = new StudySettingProperties()
                {
                    Key = "PatientNumberLength"
                },
                Value = testPatientNumberLength.ToString()
            };

            MockStudySettingService.Reset();
            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testStudySetting.Key), It.IsAny<Guid?>()))
                .ReturnsAsync(testStudySetting.Value);

            var testCorrectionId = Guid.NewGuid();

            MockPatientRepository.Setup(r => r.GetPatient(It.Is<Guid>(y => y == TestPatient.Id), It.Is<string>(x => x == "en-US")))
             .ReturnsAsync(TestPatientDto);

            var result = await Repository.GetPatientAttributesForCorrection(
                TestPatient.Id,
                testCorrectionId,
                "en-US",
                new List<CorrectionApprovalData>());

            Assert.AreEqual(5, result.Count);

            var patientNumberAttribute = result.FirstOrDefault(r =>
                r.PatientAttribute.SubjectInformation.Name == SubjectNumberTranslation);

            Assert.IsNotNull(patientNumberAttribute);
            Assert.AreEqual(expectedPatientNumberLength, patientNumberAttribute.PatientAttribute.AttributeValue.Length);
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryGetPatientAttributesForCorrectionSubjectNumberSeperatorTest()
        {
            const int testPatientNumberLength = 8;
            const int expectedPatientNumberLength = 6;
            const string testPatientSeperator = "-";

            var testPatientLengthStudySetting = new StudySettingModel
            {
                Properties = new StudySettingProperties()
                {
                    Key = "PatientNumberLength"
                },
                Value = testPatientNumberLength.ToString()
            };

            var testPatientSeperatorStudySetting = new StudySettingModel
            {
                Properties = new StudySettingProperties()
                {
                    Key = "PatientNumberSiteSubjectNumberSeparator"
                },
                Value = testPatientSeperator
            };

            MockStudySettingService.Reset();
            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testPatientLengthStudySetting.Key), It.IsAny<Guid?>()))
                .ReturnsAsync(testPatientLengthStudySetting.Value);

            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testPatientSeperatorStudySetting.Key), It.IsAny<Guid?>()))
                .ReturnsAsync(testPatientSeperatorStudySetting.Value);

            var testCorrectionId = Guid.NewGuid();

            MockPatientRepository.Setup(r => r.GetPatient(It.Is<Guid>(y => y == TestPatient.Id), It.Is<string>(x => x == "en-US")))
                .ReturnsAsync(TestPatientDto);

            var result = await Repository.GetPatientAttributesForCorrection(
                TestPatient.Id,
                testCorrectionId,
                "en-US",
                new List<CorrectionApprovalData>());

            Assert.AreEqual(5, result.Count);

            var patientNumberAttribute = result.FirstOrDefault(r =>
                r.PatientAttribute.SubjectInformation.Name == SubjectNumberTranslation);

            Assert.IsNotNull(patientNumberAttribute);
            Assert.AreEqual(expectedPatientNumberLength, patientNumberAttribute.PatientAttribute.AttributeValue.Length);
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryGetPatientAttributesForCorrection_ValueSavedBetweenPageLoadsTest()
        {
            const int testPatientNumberLength = 8;

            var testPatientLengthStudySetting = new StudySettingModel
            {
                Properties = new StudySettingProperties()
                {
                    Key = "PatientNumberLength"
                },
                Value = testPatientNumberLength.ToString()
            };

            MockStudySettingService.Reset();
            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testPatientLengthStudySetting.Key), It.IsAny<Guid?>()))
                .ReturnsAsync(testPatientLengthStudySetting.Value);

            var testCorrectionId = Guid.NewGuid();

            MockPatientRepository.Setup(r => r.GetPatient(It.Is<Guid>(y => y == TestPatient.Id), It.Is<string>(x => x == "en-US")))
                .ReturnsAsync(TestPatientDto);

            var testCorrectionApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = testCorrectionId,
                ColumnName = CorrectionConstants.PatientNumberColumnName,
                NewDisplayValue = TestPatientNumberDisplayValue,
                NewDataPoint = TestPatientNumberDisplayPoint
            };

            var result = await Repository.GetPatientAttributesForCorrection(
                TestPatient.Id,
                testCorrectionId,
                "en-US",
                new List<CorrectionApprovalData> { testCorrectionApprovalData });

            Assert.AreEqual(5, result.Count);

            var patientNumberAttribute = result.FirstOrDefault(r =>
                r.PatientAttribute.SubjectInformation.Name == SubjectNumberTranslation);

            Assert.IsNotNull(patientNumberAttribute);
            Assert.AreEqual(TestPatientNumberDisplayPoint, patientNumberAttribute.NewDataPoint);
            Assert.AreEqual(TestPatientNumberDisplayValue, patientNumberAttribute.NewDisplayValue);
        }

        [TestMethod]
        public async Task PatientAttributeRepositoryGetPatientAttributesForCorrection_LanguageIdTest()
        {
            const int testPatientNumberLength = 8;

            var testPatientLengthStudySetting = new StudySettingModel
            {
                Properties = new StudySettingProperties()
                {
                    Key = "PatientNumberLength"
                },
                Value = testPatientNumberLength.ToString()
            };

            MockStudySettingService.Reset();
            MockStudySettingService
                .Setup(r => r.GetStringValue(It.Is<string>(sc => sc == testPatientLengthStudySetting.Key), It.IsAny<Guid?>()))
                .ReturnsAsync(testPatientLengthStudySetting.Value);

            var testCorrectionId = Guid.NewGuid();

            MockPatientRepository.Setup(r => r.GetPatient(It.Is<Guid>(y => y == TestPatient.Id), It.Is<string>(x => x == "en-US")))
                .ReturnsAsync(TestPatientDto);

            var testCorrectionApprovalData = new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                CorrectionId = testCorrectionId,
                ColumnName = CorrectionConstants.PatientLanguageColumnName,
                NewDisplayValue = Japanese.DisplayName,
                NewDataPoint = Japanese.Id.ToString()
            };

            var result = await Repository.GetPatientAttributesForCorrection(
                TestPatient.Id,
                testCorrectionId,
                "en-US",
                new List<CorrectionApprovalData> { testCorrectionApprovalData });

            Assert.AreEqual(5, result.Count);

            var languageAttribute = result.FirstOrDefault(r =>
                r.ColumnName == nameof(Patient.LanguageId));

            Assert.IsNotNull(languageAttribute);
            Assert.AreEqual(Japanese.Id.ToString(), languageAttribute.NewDataPoint);
            Assert.AreEqual(Japanese.DisplayName, languageAttribute.NewDisplayValue);
        }
    }
}