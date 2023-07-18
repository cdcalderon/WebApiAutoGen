using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.StudyPortal.Models;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Models
{
    [TestClass]
    public class PatientForEditAdapterTests
    {
        private Mock<ICorrectionRepository> _mockCorrectionRepository;
        private Mock<IPatientRepository> _mockPatientRepository;
        private Mock<IVisitService> _mockVisitService;
        private Mock<ILanguageService> _mockLanguageService;
        private Mock<ITranslationService> _mockTranslationService;
        private Mock<ISubjectInformationService> _mockSubjectInformationService;
        private Mock<IPatientStatusService> _mockPatientStastusService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCorrectionRepository = new Mock<ICorrectionRepository>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockVisitService = new Mock<IVisitService>();
            _mockLanguageService = new Mock<ILanguageService>();
            _mockTranslationService = new Mock<ITranslationService>();
            _mockSubjectInformationService = new Mock<ISubjectInformationService>();
            _mockPatientStastusService = new Mock<IPatientStatusService>();
        }

        [TestMethod]
        public void PatientForEditAdapterBuildPatientAttributesTest()
        {
            var adapter = GetAdapter();

            var testDetailId = Guid.NewGuid();
            var testAttributeValue = "Free test text";

            var testAttributes = new List<PatientAttribute>
            {
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = testDetailId,
                    AttributeValue = testAttributeValue
                }
            };

            var testApprovalData = new Dictionary<Guid, IEnumerable<CorrectionDataSummary>>();

            var testSubjectConfig = new SubjectInformationModel
            {
                Id = testDetailId,
                Name = "Test Label",
                Sequence = 1,
                ChoiceType = DataType.TextAttribute.DisplayName,
                Choices = new List<ChoiceModel>()
            };

            var testSubjectInformationModels = new List<SubjectInformationModel>
            {
                testSubjectConfig
            };

            var result = adapter.BuildPatientAttributes(
                testAttributes,
                testApprovalData,
                testSubjectInformationModels);

            Assert.AreEqual(1, result.Count());

            var resultAttribute = result.First();

            Assert.AreEqual(resultAttribute.Label, testSubjectConfig.Name);
            Assert.AreEqual(resultAttribute.Order, testSubjectConfig.Sequence);
            Assert.AreEqual(resultAttribute.Value, testAttributeValue);
            Assert.IsFalse(resultAttribute.IsDateTime);
            Assert.IsFalse(resultAttribute.IsMultipleChoice);
            Assert.AreEqual(0, resultAttribute.CorrectionApprovalDatas.Count());
        }

        [TestMethod]
        public void PatientForEditAdapterBuildPatientAttributesNotMatchingConfigTest()
        {
            var adapter = GetAdapter();

            var testDetailId = Guid.NewGuid();
            var testAttributeValue = "Free test text";

            var testAttributes = new List<PatientAttribute>
            {
                new PatientAttribute
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = testDetailId,
                    AttributeValue = testAttributeValue
                }
            };

            var testApprovalData = new Dictionary<Guid, IEnumerable<CorrectionDataSummary>>();

            var testSubjectConfig = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Name = "Test Label",
                Sequence = 1,
                ChoiceType = DataType.TextAttribute.DisplayName,
                Choices = new List<ChoiceModel>()
            };

            var testSubjectInformationModels = new List<SubjectInformationModel>
            {
                testSubjectConfig
            };

            var result = adapter.BuildPatientAttributes(
                testAttributes,
                testApprovalData,
                testSubjectInformationModels);

            Assert.AreEqual(0, result.Count());
        }

        private PatientForEditAdapter GetAdapter()
        {
            var adapter = new PatientForEditAdapter(
                _mockPatientRepository.Object,
                _mockCorrectionRepository.Object,
                _mockVisitService.Object,
                _mockLanguageService.Object,
                _mockTranslationService.Object,
                _mockSubjectInformationService.Object, 
                _mockPatientStastusService.Object);

            return adapter;
        }
    }
}
