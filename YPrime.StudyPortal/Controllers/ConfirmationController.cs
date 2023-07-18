using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.ManageStudy)]
    public class ConfirmationController : BaseController
    {
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly IRoleRepository _roleRepository;

        public ConfirmationController(
            IConfirmationRepository confirmationRepository, 
            IRoleRepository roleRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _confirmationRepository = confirmationRepository;
            _roleRepository = roleRepository;
        }

        // GET: Confirmation
        public async Task<ActionResult> Index(Guid id)
        {
            if (TempData[id.ToString()] == null)
                RedirectToAction("SavedConfirmation", new Dictionary<string, Guid> {{"id", id}});

            var confirmationDto = (ConfirmationDto) TempData[id.ToString()];

            if (confirmationDto != null)
            {
                var emailInfo = await _confirmationRepository.SendEmail(
                    confirmationDto.ConfirmationTypeId,
                    confirmationDto.ConfirmationData, 
                    confirmationDto.UserId, 
                    confirmationDto.SiteId);

                var vm = new WebConfirmationViewModel
                {
                    BodyText = emailInfo.Body,
                    HeaderText = emailInfo.Subject
                };

                return View(vm);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [FunctionAuthorizationAttribute("CanViewConfirmationList", "Can view confirmation list.")]
        public ActionResult ShowSavedConfirmations()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetEmailGridData()
        {
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            var userId = User.Id;
            var emails = _confirmationRepository
                .GetSavedConfirmations(userId)
                .ToList();

            emails.ForEach(x =>
                x.LinkHTML = htmlHelper
                    .PrimeActionLink(x.Subject, "SavedConfirmation", "Confirmation", new {id = x.Id}, false)
                    .ToHtmlString());

            return Json(emails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavedConfirmation(Guid id)
        {
            var confirmation = _confirmationRepository.GetSavedConfirmation(id);

            return View(confirmation);
        }

        public async Task<SelectList> GetRolesList(string SelectedValue)
        {
            var roles = await _roleRepository.GetUserRoles(User.Id);
            StudyRoleDto all = new StudyRoleDto
            {
                ShortName = "All"
            };
            roles.ToList().Add(all);

            SelectList result;
            result = new SelectList(roles.OrderBy(u => u.ShortName).AsEnumerable(), "ShortName", "ShortName",
                SelectedValue);

            return result;
        }

        public async Task<ActionResult> Resend(Guid id, string bcc)
        {
            var confirmation = await _confirmationRepository.Resend(id, bcc.Split(';'));
            return View("SavedConfirmation", confirmation);
        }
    }
}