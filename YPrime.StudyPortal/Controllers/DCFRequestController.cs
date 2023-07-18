using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    public class DCFRequestController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IDiaryEntryRepository diaryEntryRepository;
        private readonly ICorrectionRepository correctionRepository;

        public DCFRequestController(
            IUserRepository userRepository,
            IPatientRepository patientRepository,
            IDiaryEntryRepository diaryEntryRepository,
            ICorrectionRepository correctionRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            this.userRepository = userRepository;
            this.patientRepository = patientRepository;
            this.diaryEntryRepository = diaryEntryRepository;
            this.correctionRepository = correctionRepository;
        }

        private Guid? DiaryEntryId
        {
            get { return (Guid?) Session["DiaryEntryId"]; }
            set { Session["DiaryEntryId"] = value; }
        }

        private string QuestionnaireDisplayName
        {
            get { return Session["QuestionnaireDisplayName"]?.ToString(); }
            set { Session["QuestionnaireDisplayName"] = value; }
        }


        // GET: DCFRequests
        [FunctionAuthorizationAttribute("CanViewDCFList", "Can view the list of DCFs.", true)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: DCFRequests/Create
        [FunctionAuthorizationAttribute("CanCreateDCF", "Can create a new DCF.", true)]
        public async Task<ActionResult> Create(Guid? patientId, Guid? diaryEntryId)
        {
            DiaryEntryId = null;
            QuestionnaireDisplayName = null;

            if (patientId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (diaryEntryId != null)
            {
                var diaryEntryDto =
                   await  diaryEntryRepository.GetDiaryEntry((Guid) diaryEntryId, true, CurrentSiteUserCultureCode);
                DiaryEntryId = diaryEntryId;
                QuestionnaireDisplayName = diaryEntryDto.QuestionnaireDisplayName;
                ViewBag.DiaryEntryId = DiaryEntryId;
                ViewBag.QuestionnaireDisplayName = QuestionnaireDisplayName;
            }

            var patientDto = await patientRepository.GetPatient((Guid) patientId, CurrentSiteUserCultureCode);
            DCFRequestDto dcfRequestDto = new DCFRequestDto
            {
                UserID = User.Id.ToString(),
                Username = User.UserName,
                UserFirstLast = $"{User.FirstName} {User.LastName}",
                PatientNumber = patientDto.PatientNumber,
                PatientId = patientDto.Id,
                SiteId = patientDto.SiteId,
                SiteNumber = patientDto.Site.SiteNumber,
                LastUpdate = DateTime.Now.Date
            };

            return View(dcfRequestDto);
        }

        // POST: DCFRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include =
                "ID,UserID,SiteId,PatientId,Username,TypeOfDataChange,OldValue,NewValue,Notes,LastUpdate,PatientNumber,TicketNumber,SiteNumber,UserFirstLast")]
            DCFRequestDto dcfRequestDto)
        {
            bool success = false;
            ViewBag.DiaryEntryId = DiaryEntryId;
            ViewBag.QuestionnaireDisplayName = QuestionnaireDisplayName;
            if (ModelState.IsValid)
            {
                dcfRequestDto.LastUpdate = DateTime.Now;

                success = ZenDeskHelper.CreateZendDeskTicket(correctionRepository, dcfRequestDto, User);
                if (success)
                {
                    SetPopUpMessageOnLoad("Success", "Data Correction has been submitted successfully.");
                }
                else
                {
                    SetPopUpMessageOnLoad("Failure", "Failed to submit Data Correction.");
                }
            }

            if (success && DiaryEntryId != null)
            {
                return RedirectToAction("Details", "DiaryEntries", new {id = ViewBag.DiaryEntryId});
            }

            if (success && DiaryEntryId == null)
            {
                return RedirectToAction("Edit", "Patient", new {guid = dcfRequestDto.PatientId});
            }

            return View(dcfRequestDto);
        }
    }
}