using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.BusinessLayer.Repositories;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ScheduledJobRepositoryTests
{
    [TestClass]
    public class ScheduledJobRepositoryConstructorTests : ScheduledJobRepositoryTestBase
    {

        [TestMethod]
        public void WithMissingParameter_DbContext_WillThrowException()
        {
            YAssert.DoesThrow<ArgumentNullException>(() => new ScheduledJobRepository(null));
        }

        [TestMethod]
        public void WithAllParameters_WillNotThrowException()
        {
            YAssert.DoesNotThrow<Exception>(() => new ScheduledJobRepository(_context.Object));
        }
    }
}
