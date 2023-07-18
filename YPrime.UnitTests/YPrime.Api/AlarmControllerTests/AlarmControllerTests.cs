using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using YPrime.API.Controllers;
using YPrime.API.Models;
using YPrime.Core.BusinessLayer.Services;
using Newtonsoft.Json;
using YPrime.Data.Study.Models;
using YPrime.Core.BusinessLayer.Models;
using YPrime.BusinessRule.Entities;
using System.Text;
using System.Web.Http.Results;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.UnitTests.YPrime.Api.AlarmControllerTests
{
    [TestClass]
    public class AlarmControllerTests : AlarmControllerTestBase
    {
        [TestMethod]
        public async Task SetupAlarmRequestTest()
        {
            var testRequest = new SetupAlarmDto
            {
                AlarmId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
            };

            var response = new ApiRequestResultViewModel();

            MockAlarmRepository.Setup(s => s.SetupAlarm(It.IsAny<SetupAlarmDto>())).ReturnsAsync(response);

            var controller = GetController();
            var result = await controller.Setup(testRequest);

            Assert.IsTrue(result is OkNegotiatedContentResult<ApiRequestResultViewModel>);
            MockAlarmRepository.Verify();
        }

        private AlarmController GetController()
        {
            return new AlarmController(MockAlarmRepository.Object, new SessionService());
        }
    }
}
