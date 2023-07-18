using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.BusinessLayer.UnitTests;
using YPrime.StudyPortal.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.ScheduledJobControllerTests
{
    [TestClass]
    public class HangfireJobsControllerConstructorTests : HangfireJobsControllerTestBase
    {
        [TestMethod]
        public void WithMissingParameter_SessionService_WillThrowException()
        {
            YAssert.DoesThrow<ArgumentNullException>(() => new HangfireJobsController(null, _recurringJobManager.Object));
        }

        [TestMethod]
        public void WithMissingParameter_RecurringJobManager_WillThrowException()
        {
            YAssert.DoesThrow<ArgumentNullException>(() => new HangfireJobsController(_sessionService.Object, null));
        }

        [TestMethod]
        public void WithAllParameters_WillNotThrowException()
        {
            YAssert.DoesNotThrow<Exception>(() => new HangfireJobsController(_sessionService.Object, _recurringJobManager.Object));
        }
    }
}
