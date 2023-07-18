using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Models.Api
{
    public class AnswerMapping
    {
        public string MappingName { get; set; }

        public string DiaryEntryMappingName { get; set; }

        public Answer Answer { get; set; }
    }
}
