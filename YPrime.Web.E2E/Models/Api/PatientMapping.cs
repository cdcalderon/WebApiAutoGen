using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Models.Api
{
    public class PatientMapping
    {
        public string MappingName { get; set; }

        public string PatientNumber { get; set; }

        public string SiteName { get; set; }

        public Patient Patient { get; set; }
    }
}
