using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer;
using Moq;
using System.Net.Http;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.UnitTests.Services;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.NotificationScheduleServiceTests
{
    [TestClass]
    public class NotificationScheduleServiceTests : ConfigServiceTestBase<object>
    {
        public NotificationScheduleServiceTests()
            : base(string.Empty)
        {

        }

        [TestMethod]
        public async Task WhenPatientIdIsEmptyShouldReturnFalse()
        {
            var service = GetService();
            var result = await service.CancelScheduleAsync(Guid.Empty);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task WhenPatientIdIsValidShouldReturnTrue()
        {
            var service = GetService();
            var result = await service.CancelScheduleAsync(Guid.NewGuid());
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        protected INotificationScheduleService GetService()
        {
            var service = new NotificationScheduleService(
                MockHttpFactory.Object, TestServiceSettings);

            return service;
        }
    }
}
