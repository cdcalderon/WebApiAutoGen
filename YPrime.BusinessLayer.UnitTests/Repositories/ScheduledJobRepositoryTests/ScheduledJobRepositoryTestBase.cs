using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.UnitTests.Repositories.ScheduledJobRepositoryTests
{
    [TestClass]
    public abstract class ScheduledJobRepositoryTestBase
    {
        protected Mock<IStudyDbContext> _context;
        protected ScheduledJobRepository Repository;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _context = new Mock<IStudyDbContext>();
            Repository = new ScheduledJobRepository(_context.Object);
        }
    }
}
