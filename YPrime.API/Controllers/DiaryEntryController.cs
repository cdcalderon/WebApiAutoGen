using System;
using System.Web.Http;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class DiaryEntryController : BaseApiController
    {
        private readonly IDiaryEntryRepository _diaryEntryRepository;

        public DiaryEntryController(IDiaryEntryRepository diaryEntryRepository, ISessionService sessionService) : base(sessionService)
        {
            _diaryEntryRepository = diaryEntryRepository;
        }

        // GET: DiaryEntry
        [HttpGet]
        public IHttpActionResult Get(Guid diaryEntryId, string cultureCode = "en-US")
        {
            var entry = _diaryEntryRepository.GetDiaryEntry(diaryEntryId, false, cultureCode);
            return Ok(entry);
        }
    }
}