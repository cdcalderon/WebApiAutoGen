using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ScheduledJobRepositoryTests
{
    [TestClass]
    public class ScheduledJobRepositoryTestJobTests : ScheduledJobRepositoryTestBase
    {
        [TestMethod]
        public void IsJustAnExampleJob()
        {
            Repository.TestJob();

            // Assert any logic the job should perform
        }
    }
}
