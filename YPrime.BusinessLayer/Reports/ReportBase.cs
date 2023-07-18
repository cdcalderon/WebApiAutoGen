using Newtonsoft.Json;
using YPrime.Data.Study;

namespace YPrime.Reports.Reports
{
    public abstract class ReportBase
    {
        protected readonly JsonSerializerSettings DefaultSerializerSettings;
        protected readonly IStudyDbContext _db;

        protected ReportBase(IStudyDbContext db)
        {
            _db = db;

            DefaultSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
            };
        }

        protected string SerializeObject(object value)
        {
            var result = JsonConvert.SerializeObject(
                value,
                Formatting.None,
                DefaultSerializerSettings);

            return result;
        }
    }
}
