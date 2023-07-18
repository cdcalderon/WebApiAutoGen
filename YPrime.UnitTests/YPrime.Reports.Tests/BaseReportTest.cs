using Moq;
using YPrime.Data.Study;

namespace YPrime.UnitTests.YPrime.Reports.Tests
{
    public abstract class BaseReportTest : BaseTest
    {
        protected readonly Mock<IStudyDbContext> Context;

        public BaseReportTest()
        {
            Context = new Mock<IStudyDbContext>();
        }
    }
}