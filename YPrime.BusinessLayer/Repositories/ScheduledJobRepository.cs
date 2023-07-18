using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.Repositories
{
    public class ScheduledJobRepository : BaseRepository, IScheduledJobRepository
    {
        // Hangfire requires a parameterless constructor in order to run job
        public ScheduledJobRepository(IStudyDbContext db) : base(db)
        {

        }

        public void TestJob()
        {
            // this is just an example job to make sure the jobs gets schedule
            // so it doesn't need any logic
        }
    }
}