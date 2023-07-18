using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Models.Api
{
    public class DiaryEntryMapping
    {
        public string MappingName { get; set; }

        public string PatientMappingName { get; set; }

        public DiaryEntry DiaryEntry { get; set; }
    }
}
