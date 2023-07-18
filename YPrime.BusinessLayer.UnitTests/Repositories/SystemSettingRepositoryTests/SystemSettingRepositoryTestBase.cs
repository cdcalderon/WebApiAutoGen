using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SystemSettingRepositoryTests
{
    public abstract class SystemSettingRepositoryTestBase
    {
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly SystemSettingRepository Repository;

        protected SystemSettingRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();
            Repository = new SystemSettingRepository(Context.Object);
        }
    }
}
