using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{
    [TestClass]
    public class PatientRepositoryValidatePatientAttributesFromDetailTests : PatientRepositoryTestBase
    {
        private const string InvalidTextLengthPlaceholder = "Invalid Text Length";
        private const string InvalidDateFormatPlaceholder = "Invalid Date Format";
        private const string InvalidDateRangePlaceholder = "Invalid Date Range";
        private const string AttributeRequiredFieldPlaceholder = "Required Field";
        private const string AttributeContainsNumbersPlaceholder = "Contains Numbers";

        private Patient _testPatient;

        [TestInitialize]
        public void TestInitialize()
        {
            _testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                SiteId = BaseSite.Id,
                Site = BaseSite,
            };

            BasePatients.Add(_testPatient);

            MockTranslationService
                .Setup(q => q.GetByKey(TranslationConstants.SubjectAttributeInvalidTextLengthErrorSuffix, null, null))
                .ReturnsAsync(InvalidTextLengthPlaceholder);

            MockTranslationService
                .Setup(q => q.GetByKey(TranslationConstants.SubjectAttributeInvalidDateFormatErrorSuffix, null, null))
                .ReturnsAsync(InvalidDateFormatPlaceholder);

            MockTranslationService
                .Setup(q => q.GetByKey(TranslationConstants.SubjectAttributeInvalidDateRangeErrorSuffix, null, null))
                .ReturnsAsync(InvalidDateRangePlaceholder);

            MockTranslationService
                .Setup(q => q.GetByKey(TranslationConstants.SubjectAttributeRequiredFieldErrorSuffix, null, null))
                .ReturnsAsync(AttributeRequiredFieldPlaceholder);

            MockTranslationService
                .Setup(q => q.GetByKey(TranslationConstants.SubjectAttributeContainsNumbersErrorSuffix, null, null))
                .ReturnsAsync(AttributeContainsNumbersPlaceholder);
        }
        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailTest()
        {
            var testAttributes = new List<PatientAttributeDto>
            {
                new PatientAttributeDto
                {
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "fdsao"
                },
                new PatientAttributeDto
                {
                    PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                    AttributeValue = "29/07/1990"
                }
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsTrue(result);
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, modelErrors);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailInValidTextLengthMinimumTest()
        {
            var textAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                AttributeValue = "a"
            };

            var testAttributes = new List<PatientAttributeDto>
            {
                new PatientAttributeDto
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "abcdefg"
                },
                textAttributeDto
            };

            var expectedErrorMessage = $"{LettersOnlyConfigDetail.Name} {InvalidTextLengthPlaceholder}";
            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailInValidTextLengthMaximumTest()
        {
            var textAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                AttributeValue = "abcdefghiklmnop"
            };

            var testAttributes = new List<PatientAttributeDto>
            {
                new PatientAttributeDto
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "abcdefg"
                },
                textAttributeDto
            };

            var expectedErrorMessage = $"{LettersOnlyConfigDetail.Name} {InvalidTextLengthPlaceholder}";
            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes,
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailValidAttributeBadDateTest()
        {
            var dateAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = "02/30/1990"
            };

            var expectedErrorMessage = $"{DateFormatConfigDetail.Name} {InvalidDateFormatPlaceholder}";

            var testAttributes = new List<PatientAttributeDto>
            {
                new PatientAttributeDto
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                    AttributeValue = "fdsao"
                },
                dateAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailValidAttributeDateOutOfRangeTest()
        {
            var dateAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = "09/09/1950"
            };

            var expectedErrorMessage = $"{DateFormatConfigDetail.Name} {InvalidDateRangePlaceholder}";

            var testAttributes = new List<PatientAttributeDto>
            {
                dateAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState,
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailValidAttributeDateOutOfRangeCorrectionTest()
        {
            var dateAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = "09/09/1950"
            };

            var testAttributes = new List<PatientAttributeDto>
            {
                dateAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow, 
                _testPatient.Id,
                true);

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsTrue(result);
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, modelErrors);
        }

        [TestMethod]
        public async Task ValidateAttributesFromDetailsValidAttributeLettersOnlyHasNumbersTest()
        {
            var textAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                AttributeValue = "fdsa2"
            };

            var expectedErrorMessage = $"{LettersOnlyConfigDetail.Name} {AttributeContainsNumbersPlaceholder}";

            var testAttributes = new List<PatientAttributeDto>
            {
                textAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailValidAttributeMissingDataTest()
        {
            var textAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                AttributeValue = "fdsa"
            };

            var missingTextAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = string.Empty
            };

            var expectedErrorMessage = $"{DateFormatConfigDetail.Name} {AttributeRequiredFieldPlaceholder}";

            var testAttributes = new List<PatientAttributeDto>
            {
                textAttributeDto,
                missingTextAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes, 
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var firstError = modelState.First().Value;

            var errorMessage = firstError
                .Errors[0]
                .ErrorMessage;

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelErrors);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailValidAttributeValidNumericLengthTest()
        {
            var numberAttributeDto = new PatientAttributeDto
            {
                Id = Guid.NewGuid(),
                PatientAttributeConfigurationDetailId = NumberAttributeConfigDetail.Id,
                AttributeValue = "22"
            };

            var testAttributes = new List<PatientAttributeDto>
            {
                numberAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes,
                modelState, 
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            Assert.IsTrue(result);
            Assert.IsTrue(modelState.IsValid);
        }

        [TestMethod]
        public async Task ValidatePatientAttributesFromDetailNewAttributesDuplicateMessageTest()
        {
            var textAttributeDto = new PatientAttributeDto
            {
                PatientAttributeConfigurationDetailId = LettersOnlyConfigDetail.Id,
                AttributeValue = string.Empty
            };

            var missingTextAttributeDto = new PatientAttributeDto
            {
                PatientAttributeConfigurationDetailId = DateFormatConfigDetail.Id,
                AttributeValue = string.Empty
            };

            var testAttributes = new List<PatientAttributeDto>
            {
                textAttributeDto,
                missingTextAttributeDto
            };

            var modelState = new ModelStateDictionary();

            var result = await Repository.ValidatePatientAttributesFromDetail(
                testAttributes,
                modelState,
                DateTimeOffset.UtcNow,
                _testPatient.Id);

            var modelErrors = modelState.Values.SelectMany(q => q.Errors).Count();

            Assert.IsFalse(result);
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(2, modelErrors);
        }
    }
}
