using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.BusinessLayer.Repositories;
using YPrime.eCOA.DTOLibrary;
using Moq.Protected;
using YPrime.BusinessLayer.Query;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientVisitRepositoryTests
{
    [TestClass]
    public class PatientRepositoryVisitManagementTests : PatientVisitRepositoryTestBase
    {
        protected const string WebBackupTabletPublicKeyName = "WebBackupTabletPublicKey";
        protected const string TestKeyValue = "XYZ-Test-Key";

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(1),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });
            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsBusinessRuleNotValidTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(1),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = false
                });
            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });
            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasHardStopNoMessageTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
            
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(1),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });
            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var hardStopMessage = "hard stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitHardStop"), null, null))
                .ReturnsAsync(hardStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(null, patientVisit.PatientVisitHardStop.HardStopMessage);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowYesNo);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasHardStopOutOfWindowTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1     
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var hardStopMessage = "hard stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitHardStop"), null, null))
                .ReturnsAsync(hardStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, hardStopMessage);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowYesNo);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasHardStopInWindowNotAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-555556745624"),
                AlwaysAvailable = false,
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
                       
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testPatientVisitDto2 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit2.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,
                testPatientVisitDto2
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1, baseVisit2 });

            var hardStopMessage = "hard stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitHardStop"), null, null))
                .ReturnsAsync(hardStopMessage);
            
            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault(r => r.VisitId == baseVisit2.Id);
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, hardStopMessage);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowYesNo);

        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasSoftStopOutOfWindowIsAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,                
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var softStopMessage = "soft stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitSoftStop"), null, null))
                .ReturnsAsync(softStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, softStopMessage);
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowYesNo);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasSoftStopOutOfWindowNotAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-555556745624"),
                AlwaysAvailable = false,
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
                        
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testPatientVisitDto2 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit2.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,
                testPatientVisitDto2
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1, baseVisit2 });

            var softStopMessage = "soft stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitSoftStopWithWarning"), null, null))
                .ReturnsAsync(softStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault(r => r.VisitId == baseVisit2.Id);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, softStopMessage);
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowYesNo);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasSoftStopInWindowNotAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-555556745624"),
                AlwaysAvailable = false,
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
                        
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(1),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testPatientVisitDto2 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit2.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(1),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,
                testPatientVisitDto2
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            
            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });
            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1, baseVisit2 });

            var softStopMessage = "soft stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisit"), null, null))
                .ReturnsAsync(softStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault(r => r.VisitId == baseVisit2.Id);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, softStopMessage);
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowYesNo);

        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasNoStopInWindowNotAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-555556745624"),
                AlwaysAvailable = false,
                VisitOrder = 2,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "N",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testPatientVisitDto2 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit2.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,
                testPatientVisitDto2
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });
            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1, baseVisit2 });

            var softStopMessage = "soft stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisit"), null, null))
                .ReturnsAsync(softStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault(r => r.VisitId == baseVisit2.Id);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(patientVisit.PatientVisitHardStop.HardStopMessage, softStopMessage);
            Assert.IsTrue(patientVisit.PatientVisitHardStop.ShowYesNo);

        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsHasNoStopOutOfWindowIsAvailableTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "N",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
            
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };
            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });
            
            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var hardStopMessage = "hard stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitHardStop"), null, null))
                .ReturnsAsync(hardStopMessage);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowOk);
            Assert.AreEqual(null, patientVisit.PatientVisitHardStop.HardStopMessage);
            Assert.IsFalse(patientVisit.PatientVisitHardStop.ShowYesNo);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsCanActivateVisitTrueTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var subjectQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireTaker = new QuestionnaireTakerModel
                {
                    QuestionnaireTypeId = QuestionnaireType.PatientHandheld.Id,
                }
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true,
                Questionnaires = new List<VisitQuestionnaireModel>
                {
                    new VisitQuestionnaireModel
                    {
                        QuestionnaireId = subjectQuestionnaire.Id,
                    }
                }
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };
            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };
                        
            SetupQuestionnaires(new List<QuestionnaireModel> { subjectQuestionnaire });
            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsTrue(patientVisit.CanActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsCanActivateVisitFalseTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
                        
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.InProgress.Id,
            };
            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
            
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsFalse(patientVisit.CanActivateVisit);
        }

        [TestMethod]        
        public async Task PatientVisitServiceGetPatientVisitsAlwaysAvailableTrueTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = true,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.InProgress.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsTrue(patientVisit.ShowActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsAlwaysAvailableFalseTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 1,
                WindowBefore = -1,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
            
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.InProgress.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };
                        
            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };

            
            var patientVisitData = new List<PatientVisitDto>
            { 
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsFalse(patientVisit.ShowActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsShowActivateVisitInWindowTrueTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 10,
                WindowBefore = 10,
                VisitStop_HSN = "S",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1          
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);
                        
            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
                {
                    ExecutionResult = true
                });

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault();
            Assert.IsTrue(patientVisit.ShowActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsShowActivateVisitInWindowUnscheduledTest()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                WindowAfter = 0,
                WindowBefore = 1,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };

            var baseVisit2 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-555556745624"),
                AlwaysAvailable = false,
                VisitOrder = 2,
                IsScheduled = false,
                DaysExpected = 1,
                WindowAfter = 10,
                WindowBefore = 10,
                VisitStop_HSN = "H",
                VisitAvailableBusinessRuleId = Guid.NewGuid(),
                VisitAvailableBusinessRuleTrueFalseIndicator = true
            };
            
            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testPatientVisitDto2 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit2.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now,
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1,
                testPatientVisitDto2
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<VisitModel> { baseVisit1, baseVisit2 });

            var hardStopMessage = "hard stop warning";
            MockTranslationService.Setup(s => s.GetByKey(It.Is<string>(k => k == "ContinueCompletingVisitHardStopWithWarning"), null, null))
                .ReturnsAsync(hardStopMessage);

            MockRuleRepository.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(),
               It.IsAny<Guid>(),
               It.IsAny<Guid>(),
               It.IsAny<bool>(),
               It.IsAny<DateTime?>())).Returns(new BusinessRule.Entities.BusinessRuleResult
               {
                   ExecutionResult = true
               });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var patientVisit = result.FirstOrDefault(r => r.VisitId == baseVisit2.Id);
            Assert.IsTrue(patientVisit.ShowActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsWebBackUpEnabled()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };
                        
            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1, BaseCaregivers);
            
            MockSystemSettingRepository
                .Setup(r => r.GetSystemSettingValue(It.Is<string>(k => k == WebBackupTabletPublicKeyName)))
                .Returns(TestKeyValue);

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, true);
            var expiration = result.FirstOrDefault().ValidTo;
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(expiration));
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsWebBackUpNotEnabledPermissionFalse()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);            

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, false);
            var expiration = result.FirstOrDefault().ValidTo;
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(expiration));
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsWebBackUpNotEnabledStudyCustomFalse()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms(0);
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);            

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, true);
            var expiration = result.FirstOrDefault().ValidTo;
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(expiration));
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsClincianOnlyQuestionnaires()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var questionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireTaker = new QuestionnaireTakerModel
                {
                    QuestionnaireTypeId = QuestionnaireType.Clinician.Id
                }
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                Questionnaires = new List<VisitQuestionnaireModel>
                {
                    new VisitQuestionnaireModel
                    {
                        QuestionnaireId = questionnaire.Id,
                    }
                }
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, true);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.First().CanActivateVisit);
        }

        [TestMethod]
        public async Task PatientVisitServiceGetPatientVisitsClincianAndSubjectQuestionnaires()
        {
            var testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            var clinicianQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireTaker = new QuestionnaireTakerModel
                {
                    QuestionnaireTypeId = QuestionnaireType.Clinician.Id
                }
            };

            var subjectQuestionnaire = new QuestionnaireModel
            {
                Id = Guid.NewGuid(),
                QuestionnaireTaker = new QuestionnaireTakerModel
                {
                    QuestionnaireTypeId = QuestionnaireType.PatientHandheld.Id
                }
            };

            var baseVisit1 = new VisitModel()
            {
                Id = Guid.Parse("12345998-7777-4545-1111-123456745624"),
                AlwaysAvailable = false,
                VisitOrder = 1,
                IsScheduled = true,
                DaysExpected = 1,
                Questionnaires = new List<VisitQuestionnaireModel>
                {
                    new VisitQuestionnaireModel
                    {
                        QuestionnaireId = clinicianQuestionnaire.Id,
                    },
                    new VisitQuestionnaireModel
                    {
                        QuestionnaireId = subjectQuestionnaire.Id,
                    }
                }
            };

            var testPatientVisitDto1 = new PatientVisitDto
            {
                Id = Guid.NewGuid(),
                PatientId = testPatient.Id,
                VisitId = baseVisit1.Id,
                ActivationDate = DateTime.Now,
                ProjectedDate = DateTime.Now.AddDays(-10),
                VisitDate = DateTime.Now,
                PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
            };

            var testSiteDto1 = new SiteDto
            {
                Id = Guid.NewGuid(),
                WebBackupExpireDate = DateTime.Now.AddDays(4),
                TimeZone = "Eastern Standard Time",
            };

            var DiaryEntry1 = new DiaryEntryDto
            {
                Id = Guid.NewGuid(),
                VisitId = baseVisit1.Id,
                QuestionnaireId = Guid.NewGuid()
            };


            var patientVisitData = new List<PatientVisitDto>
            {
                testPatientVisitDto1
            };

            var diaryEntryData = new List<DiaryEntryDto> { DiaryEntry1 };

            SetupStudyCustoms();
            SetupQuestionnaires(new List<QuestionnaireModel> { clinicianQuestionnaire, subjectQuestionnaire });
            SetupPatientVisitSummaryQueryHandlers(patientVisitData, diaryEntryData, testSiteDto1);

            MockVisitService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel> { baseVisit1 });

            var result = await Repository.GetPatientVisitSummary(testPatient.Id, true, true);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.First().CanActivateVisit);
        }
    }
}