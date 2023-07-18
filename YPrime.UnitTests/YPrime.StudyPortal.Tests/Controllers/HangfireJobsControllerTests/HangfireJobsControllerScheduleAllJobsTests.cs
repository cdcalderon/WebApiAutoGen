using Hangfire;
using Hangfire.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.UnitTests;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.ScheduledJobControllerTests
{
    [TestClass]
    public class HangfireJobsControllerScheduleAllJobsTests : HangfireJobsControllerTestBase
    {
        [TestMethod]
        public void WhenCalled_SchedulesAllJobs()
        {
            var result = Controller.ScheduleAllJobs();

            _recurringJobManager.Verify(m => m.AddOrUpdate(It.Is<string>(s => s == nameof(IScheduledJobRepository.TestJob)), It.IsAny<Job>(), It.Is<string>(s => s == Cron.Daily()), It.IsAny<RecurringJobOptions>()), Times.Once);
        }

        [TestMethod]
        public void WhenCalled_RedirectsToHangfire()
        {
            var result = Controller.ScheduleAllJobs();

            YAssert.IsType<RedirectToRouteResult>(result);
            Assert.AreEqual("hangfire", result.RouteName);
        }
    }
}
