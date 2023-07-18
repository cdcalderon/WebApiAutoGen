using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessRule.Entities;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.AlarmRepositoryTests
{
    [TestClass]
    public class AlarmRepositoryTests : AlarmRepositoryTestBase
    {
        [TestMethod]
        public async Task ExecuteAlarmBusinessRuleTest()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = TestPatientId
            };

            var execResult = new BusinessRuleResult
            {
                ExecutionResult = true
            };

            var patient = new Patient
            {
                Id = TestPatientId,
                SiteId = TestSiteId
            };

            var alarmModel = new AlarmModel
            {
                NotifyBusinessRuleId = Guid.NewGuid(),
                BusinessRuleTrueFalseIndicator = true
            };

            MockDeviceRepository
                .Setup(s => s.GetLastReportedConfigurationForDevice(It.IsAny<Guid>()))
                .ReturnsAsync(Guid.NewGuid());

            MockPatientRepository
                .Setup(s => s.GetPatientEntity(It.Is<Guid>(pid => pid == TestPatientId)))
                .ReturnsAsync(patient);

            MockAlarmService
                .Setup(s => s.GetTranslatedAlarmModel(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(alarmModel);

            MockRuleService
                .Setup(
                    s => s.ExecuteBusinessRule(
                        It.Is<Guid>(br => br == alarmModel.NotifyBusinessRuleId), 
                        It.Is<Guid>(pid => pid == TestPatientId), 
                        It.Is<Guid>(sid => sid == TestSiteId), 
                        It.Is<bool?>(ind => ind == alarmModel.BusinessRuleTrueFalseIndicator), 
                        It.Is<DateTime?>(dt => dt == TestSiteLocalTime.DateTime)))
                .Returns(execResult);

            var service = GetService();
            var result = await service.ExecuteAlarmBusinessRule(testRequest);

            Assert.IsTrue(result.WasSuccessful);
            MockDeviceRepository.Verify();
            MockPatientRepository.Verify();
            MockAlarmService.Verify();
            MockRuleService.Verify();

            MockSiteRepository
                .Verify(
                    r => r.GetSiteLocalTime(It.Is<Guid>(sid => sid == TestSiteId)),
                    Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAlarmBusinessRule_PatientNotFoundTest()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = TestPatientId
            };

            var execResult = new BusinessRuleResult
            {
                ExecutionResult = true
            };


            var alarmModel = new AlarmModel
            {
                NotifyBusinessRuleId = Guid.NewGuid()
            };

            MockDeviceRepository.Setup(s => s.GetLastReportedConfigurationForDevice(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());
            MockPatientRepository.Setup(s => s.GetPatientEntity(It.IsAny<Guid>())).ReturnsAsync((Patient)null);
            MockAlarmService.Setup(s => s.GetTranslatedAlarmModel(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(alarmModel);
            MockRuleService.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool?>(), It.IsAny<DateTime?>()))
                .Returns(execResult);

            var service = GetService();

            await service
                .Invoking(s=> s.ExecuteAlarmBusinessRule(testRequest))
                .Should()
                .ThrowAsync<PatientNotFoundException>();
        }

        [TestMethod]
        public async Task ExecuteAlarmBusinessRule_AlarmFoundTest()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = TestPatientId
            };

            var execResult = new BusinessRuleResult
            {
                ExecutionResult = true
            };

            var patient = new Patient
            {
                Id = TestPatientId,
                SiteId = TestSiteId
            };

            MockDeviceRepository.Setup(s => s.GetLastReportedConfigurationForDevice(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());
            MockPatientRepository.Setup(s => s.GetPatientEntity(It.IsAny<Guid>())).ReturnsAsync(patient);
            MockAlarmService.Setup(s => s.GetTranslatedAlarmModel(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync((AlarmModel)null);
            MockRuleService.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool?>(), It.IsAny<DateTime?>()))
                .Returns(execResult);

            var service = GetService();

            await service
                .Invoking(s => s.ExecuteAlarmBusinessRule(testRequest))
                .Should()
                .ThrowAsync<AlarmNotFoundException>();
        }

        [TestMethod]
        public async Task ExecuteAlarmBusinessRule_DeviceNotFoundTest()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = TestPatientId
            };

            var execResult = new BusinessRuleResult
            {
                ExecutionResult = true
            };

            var patient = new Patient
            {
                Id = TestPatientId,
                SiteId = TestSiteId
            };

            var alarmModel = new AlarmModel
            {
                NotifyBusinessRuleId = Guid.NewGuid()
            };

            MockDeviceRepository.Setup(s => s.GetLastReportedConfigurationForDevice(It.IsAny<Guid>())).ReturnsAsync((Guid?)null);
            MockPatientRepository.Setup(s => s.GetPatientEntity(It.IsAny<Guid>())).ReturnsAsync(patient);
            MockAlarmService.Setup(s => s.GetTranslatedAlarmModel(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(alarmModel);
            MockRuleService.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool?>(), It.IsAny<DateTime?>()))
                .Returns(execResult);

            var service = GetService();

            await service
                .Invoking(s => s.ExecuteAlarmBusinessRule(testRequest))
                .Should()
                .ThrowAsync<DeviceNotFoundException>();
        }

        [TestMethod]
        public async Task ExecuteAlarmBusinessRule_BusinessRuleErrorTest()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = TestPatientId
            };

            var patient = new Patient
            {
                Id = TestPatientId,
                SiteId = TestSiteId
            };

            var alarmModel = new AlarmModel
            {
                NotifyBusinessRuleId = Guid.NewGuid()
            };

            MockDeviceRepository.Setup(s => s.GetLastReportedConfigurationForDevice(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());
            MockPatientRepository.Setup(s => s.GetPatientEntity(It.IsAny<Guid>())).ReturnsAsync(patient);
            MockAlarmService.Setup(s => s.GetTranslatedAlarmModel(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(alarmModel);
            MockRuleService.Setup(s => s.ExecuteBusinessRule(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool?>(), It.IsAny<DateTime?>()))
                .Returns((BusinessRuleResult)null);

            var service = GetService();

            await service
                .Invoking(s => s.ExecuteAlarmBusinessRule(testRequest))
                .Should()
                .ThrowAsync<BusinessRuleException>();
        }
    }
}
