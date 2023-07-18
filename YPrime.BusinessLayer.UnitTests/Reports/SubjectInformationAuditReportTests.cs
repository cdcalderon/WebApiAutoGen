using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Constants;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;


namespace YPrime.BusinessLayer.UnitTests.Reports
{
    [TestClass]
    public class SubjectInformationAuditReportTests
    {
        private const string ExpectedSql = "[dbo].[PatientAuditRecReport_Filtered] @StudyName,@SubjectNumTranslation,@PinSize,@SiteNumber,@PatientId,@PatientAttributeJson,@DataTypeJson,@LanguageJson,@PatientStatusTypeJson,@ChoicesJson";

        private Mock<IStudyDbContext> MockContext;
        private Mock<ITranslationService> MockTranslationService;
        private Mock<ISubjectInformationService> MockSubjectInformationService;
        private Mock<IStudySettingService> MockStudySettingService;
        private Mock<ILanguageService> MockLanguageService;
        private Mock<IPatientStatusService> MockPatientStatusService;

        private object[] UsedParameters = Array.Empty<object>();
        private string TestStudyName;
        private int TestPinLength;
        private string TestNomenclature;
        private Guid TestSiteId;
        private Guid TestUserId;
        private Guid TestCountryId;
        private Patient TestPatient;
        private Site TestSite;
        private SubjectInformationModel TestAttribute;
        private LanguageModel EnglishLanguage;
        private SubjectInformationAuditDto ReturnedDto;

        [TestInitialize]
        public void TestInitialize()
        {
            TestStudyName = Guid.NewGuid().ToString();
            TestPinLength = 4;
            TestNomenclature = "Patient";
            TestSiteId = Guid.NewGuid();
            TestUserId = Guid.NewGuid();
            TestCountryId = Guid.NewGuid();

            EnglishLanguage = new LanguageModel
            {
                Id = Guid.NewGuid(),
                Name = "English",
                CultureName = "en-US"
            };

            TestPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-10001-001",
                SiteId = TestSiteId
            };

            TestSite = new Site
            {
                Id = TestSiteId,
                CountryId = TestCountryId,
                SiteNumber = "S-11001"
            };

            TestAttribute = new SubjectInformationModel
            {
                Id = Guid.NewGuid(),
                Name = "Attribute A",
                ChoiceType = "Radio Button"
            };

            var testStudyUserRoles = new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    Id = Guid.NewGuid(),
                    StudyUserId = TestUserId,
                    SiteId = TestSiteId
                }
            };

            MockTranslationService = new Mock<ITranslationService>();

            MockTranslationService
                .Setup(s => s.GetByKey(It.Is<string>(key => key == TranslationKeyTypes.lblPatient), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(TestNomenclature);

            MockSubjectInformationService = new Mock<ISubjectInformationService>();

            MockSubjectInformationService
                .Setup(s => s.GetForCountry(It.Is<Guid>(cid => cid == TestCountryId), It.IsAny<Guid?>()))
                .ReturnsAsync(new List<SubjectInformationModel>
                {
                    TestAttribute
                });

            MockStudySettingService = new Mock<IStudySettingService>();

            MockStudySettingService
                .Setup(s => s.GetStringValue(It.Is<string>(key => key == "StudyName"), It.IsAny<Guid?>()))
                .ReturnsAsync(TestStudyName);

            MockStudySettingService
                .Setup(s => s.GetIntValue(It.Is<string>(key => key == "PatientPINLength"), It.IsAny<int>(), It.IsAny<Guid?>()))
                .ReturnsAsync(TestPinLength);

            MockLanguageService = new Mock<ILanguageService>();

            MockLanguageService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<LanguageModel>
                {
                    EnglishLanguage
                });

            MockPatientStatusService = new Mock<IPatientStatusService>();

            MockPatientStatusService
                .Setup(s => s.GetAll(null))
                .ReturnsAsync(new List<PatientStatusModel>
                {
                    new PatientStatusModel
                    {
                        Id = 1,
                        Name = "test status"
                    }
                });

            MockContext = new Mock<IStudyDbContext>();

            var studyUserRoleDbSet = new FakeDbSet<StudyUserRole>(testStudyUserRoles);
            var patientDbSet = new FakeDbSet<Patient>(new List<Patient> { TestPatient });
            var siteDbSet = new FakeDbSet<Site>(new List<Site> { TestSite });

            MockContext
                .Setup(c => c.StudyUserRoles)
                .Returns(studyUserRoleDbSet.Object);

            MockContext
                    .Setup(c => c.Patients)
                    .Returns(patientDbSet.Object);

            MockContext
                .Setup(c => c.Sites)
                .Returns(siteDbSet.Object);

            ReturnedDto = new SubjectInformationAuditDto
            {
                Protocol = TestStudyName,
                SiteNumber = TestSite.SiteNumber,
                SubjectNumber = TestPatient.PatientNumber,
                SubjectAttribute = TestAttribute.Name,
                OldValue = "Old",
                NewValue = "New",
                ChangedBy = "Test User",
                ChangedDate = DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy"),
                CorrectionReason = "CR",
                DCFNumber = null,
                AuditSeq = string.Empty,
                AuditSource = "A",
                AssetTag = string.Empty
            };

            MockContext
                .Setup(c => c.ExecuteSqlToList<SubjectInformationAuditDto>(
                    It.Is<string>(sql => sql == ExpectedSql),
                    It.IsAny<object[]>()))
                .Returns(new List<SubjectInformationAuditDto>
                {
                    ReturnedDto
                })
                .Callback((string passedInSql, object[] passedInParameters) =>
                {
                    UsedParameters = passedInParameters;
                });
        }

        [TestMethod]
        public async Task GetGridDataTest()
        {
            var report = GetReport();

            var reportParams = new Dictionary<string, object>
            {
                { "SUBJ", TestPatient.Id}
            };

            var results = await report
                .GetGridData(reportParams, TestUserId);

            Assert.AreEqual(1, results.Count);

            var result = results.First();

            Assert.AreEqual(ReturnedDto.Protocol, result[$"{nameof(SubjectInformationAuditDto.Protocol)}"]);
            Assert.AreEqual(ReturnedDto.SiteNumber, result[$"{nameof(SubjectInformationAuditDto.SiteNumber)}"]);
            Assert.AreEqual(ReturnedDto.SubjectNumber, result[$"{nameof(SubjectInformationAuditDto.SubjectNumber)}"]);
            Assert.AreEqual(ReturnedDto.SubjectAttribute, result[$"{nameof(SubjectInformationAuditDto.SubjectAttribute)}"]);
            Assert.AreEqual(ReturnedDto.OldValue, result[$"{nameof(SubjectInformationAuditDto.OldValue)}"]);
            Assert.AreEqual(ReturnedDto.NewValue, result[$"{nameof(SubjectInformationAuditDto.NewValue)}"]);
            Assert.AreEqual(ReturnedDto.ChangeReasonType, result[$"{nameof(SubjectInformationAuditDto.ChangeReasonType)}"]);
            Assert.AreEqual(ReturnedDto.ChangedBy, result[$"{nameof(SubjectInformationAuditDto.ChangedBy)}"]);
            Assert.AreEqual(ReturnedDto.ChangedDate, result[$"{nameof(SubjectInformationAuditDto.ChangedDate)}"]);
            Assert.AreEqual(ReturnedDto.CorrectionReason, result[$"{nameof(SubjectInformationAuditDto.CorrectionReason)}"]);
            Assert.AreEqual(ReturnedDto.DCFNumber, result[$"{nameof(SubjectInformationAuditDto.DCFNumber)}"]);
            Assert.AreEqual(ReturnedDto.AssetTag, result[$"{nameof(SubjectInformationAuditDto.AssetTag)}"]);

            Assert.AreEqual(10, UsedParameters.Count());
        }

        [TestMethod]
        public void GetColumnHeadingsTest()
        {
            var report = GetReport();

            var result = report.GetColumnHeadings();

            Assert.AreEqual(12, result.Count);
            Assert.AreEqual($"{TestNomenclature} Number", result["SubjectNumber"]);
            Assert.AreEqual($"{TestNomenclature} Attribute", result["SubjectAttribute"]);
        }

        [TestMethod]
        public void GetColumnHeadingsNoNomenclatureResultTest()
        {
            MockTranslationService.Reset();

            MockTranslationService
                .Setup(s => s.GetByKey(It.Is<string>(key => key == TranslationKeyTypes.lblPatient), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync((string)null);

            var report = GetReport();

            var result = report.GetColumnHeadings();

            Assert.AreEqual(12, result.Count);
            Assert.AreEqual($"Subject Number", result[$"{nameof(SubjectInformationAuditDto.SubjectNumber)}"]);
            Assert.AreEqual($"Subject Attribute", result[$"{nameof(SubjectInformationAuditDto.SubjectAttribute)}"]);
        }

        [TestMethod]
        public async Task GetReportChartDataTest()
        {
            var report = GetReport();

            var result = await report.GetReportChartData(
                null,
                Guid.NewGuid());

            Assert.IsNull(result);
        }


        private SubjectInformationAuditReport GetReport()
        {
            var report = new SubjectInformationAuditReport(
                MockContext.Object,
                MockTranslationService.Object,
                MockSubjectInformationService.Object,
                MockStudySettingService.Object,
                MockLanguageService.Object,
                MockPatientStatusService.Object);

            return report;
        }
    }
}
