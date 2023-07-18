using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using YPrime.API.Controllers;
using YPrime.Core.BusinessLayer.Services;
using System.Web.Http.Results;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.BusinessLayer.Exceptions;

namespace YPrime.UnitTests.YPrime.Api.BusinessRuleControllerTests
{
    [TestClass]
    public class BusinessRuleControllerTests : BusinessRuleControllerTestBase
    {
        [TestMethod]
        public async Task RunAlarmRuleTest()
        {
            var request = GetAlarmRuleDto();

            var response = new ApiRequestResultViewModel();

            MockAlarmRepository.Setup(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>())).ReturnsAsync(response);

            var controller = GetController();
            var result = await controller.RunAlarmRule(request);

            Assert.IsTrue(result is OkNegotiatedContentResult<ApiRequestResultViewModel>);
            MockAlarmRepository.Verify(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()), Times.Once);
        }

        [TestMethod]
        public async Task PatientNotFoundTest()
        {
            var request = GetAlarmRuleDto();

            MockAlarmRepository
                .Setup(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()))
                .ThrowsAsync(new PatientNotFoundException());

            var controller = GetController();
            var result = await controller.RunAlarmRule(request);

            Assert.IsTrue(result is BadRequestErrorMessageResult);
            Assert.AreEqual("Patient Id not found", ((BadRequestErrorMessageResult)result).Message);
            MockAlarmRepository.Verify(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()), Times.Once);
        }

        [TestMethod]
        public async Task DeviceNotFoundTest()
        {
            var request = GetAlarmRuleDto();

            MockAlarmRepository
                .Setup(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()))
                .ThrowsAsync(new DeviceNotFoundException());

            var controller = GetController();
            var result = await controller.RunAlarmRule(request);

            Assert.IsTrue(result is BadRequestErrorMessageResult);
            Assert.AreEqual("Device not found", ((BadRequestErrorMessageResult)result).Message);
            MockAlarmRepository.Verify(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()), Times.Once);
        }

        [TestMethod]
        public async Task AlarmNotFoundTest()
        {
            var request = GetAlarmRuleDto();

            MockAlarmRepository
                .Setup(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()))
                .ThrowsAsync(new AlarmNotFoundException());

            var controller = GetController();
            var result = await controller.RunAlarmRule(request);

            Assert.IsTrue(result is BadRequestErrorMessageResult);
            Assert.AreEqual("Alarm not found", ((BadRequestErrorMessageResult)result).Message);
            MockAlarmRepository.Verify(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()), Times.Once);
        }

        [TestMethod]
        public async Task BusinessRuleErrorTest()
        {
            var request = GetAlarmRuleDto();

            MockAlarmRepository
                .Setup(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()))
                .ThrowsAsync(new BusinessRuleException());

            var controller = GetController();
            var result = await controller.RunAlarmRule(request);

            Assert.IsTrue(result is BadRequestErrorMessageResult);
            Assert.AreEqual("Business Rule Execution Failed", ((BadRequestErrorMessageResult)result).Message);
            MockAlarmRepository.Verify(s => s.ExecuteAlarmBusinessRule(It.IsAny<AlarmRuleDto>()), Times.Once);
        }

        public AlarmRuleDto GetAlarmRuleDto()
        {
            var testRequest = new AlarmRuleDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
            };

            return testRequest;
        }

        private BusinessRuleController GetController()
        {
            return new BusinessRuleController(MockAlarmRepository.Object, new SessionService());
        }
    }
}
