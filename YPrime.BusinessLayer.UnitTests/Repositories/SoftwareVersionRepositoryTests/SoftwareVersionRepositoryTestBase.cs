using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SoftwareVersionRepositoryTests
{
    public abstract class SoftwareVersionRepositoryTestBase
    {
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly SoftwareVersionRepository Repository;

        protected SoftwareVersionRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();
            Repository = new SoftwareVersionRepository(Context.Object);
        }
    }
}