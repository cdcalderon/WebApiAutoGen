using System;
using System.Net;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    public class DiaryEntriesController : BaseController
    {
        private readonly IDiaryEntryRepository _DiaryEntryRepository;

        public DiaryEntriesController(
            IDiaryEntryRepository DiaryEntryRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _DiaryEntryRepository = DiaryEntryRepository;
        }

        [FunctionAuthorization("CanViewDiaryEntryDetails", "Can View Diary entry details..", true)]
        public async System.Threading.Tasks.Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DiaryEntryDto diaryEntry = await
                _DiaryEntryRepository.GetDiaryEntry((Guid) id, false, CurrentSiteUserCultureCode);

            if (diaryEntry == null)
            {
                 throw new Exception();
            }

            if (diaryEntry.IsCSSRS)
            {
                CSSRSHelper.FilterPlaceholderQuestions(diaryEntry);
            }
           
            return View(diaryEntry);
        }
    }
}