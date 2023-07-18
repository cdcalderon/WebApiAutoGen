using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Helpers;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.Dashboard)]
    public class DashboardController : BaseController
    {
        private readonly IWidgetRepository _widgetRepository;

        public DashboardController(
            IWidgetRepository widgetRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _widgetRepository = widgetRepository;
        }

        public async Task<ActionResult> Index()
        {
            if (YPrimeSession.Instance.CurrentUserAuth0 != null)
            {
                var currentUser = YPrimeSession.Instance.CurrentUser;
                var userId = currentUser.Id;
                var roleId = CurrentStudyRole.Id;

                var userWidgets = await _widgetRepository.GetDashboardWidgets(CurrentSiteUserCultureCode, roleId, userId);

                var dashboardDto = new DashboardDto
                {
                    RenderWidgetApiUrl = Url.Action("RenderWidget", "Dashboard"),
                    SaveDashboardApiUrl = Url.Action("SaveDashboard", "Dashboard"),
                    Widgets = userWidgets,
                    AvailableWidgets = new List<Widget>(),
                    MaxWidgets = 32
                };

                return View(dashboardDto);
            }

            return View(new DashboardDto() { AvailableWidgets = new List<Widget>() });
        }

        [HttpPost]
        public void SaveDashboard(Dictionary<string, string> widgetPositions)
        {
            var currentUser = YPrimeSession.Instance.CurrentUser;
            var studyUserWidgets = new List<StudyUserWidget>();
            var userId = currentUser.Id;
            foreach (var kv in widgetPositions)
            {
                studyUserWidgets.Add(new StudyUserWidget
                {
                    StudyUserId = userId,
                    WidgetId = Guid.Parse(kv.Key),
                    WidgetPosition = int.Parse(kv.Value)
                });
            }

            studyUserWidgets = studyUserWidgets.OrderBy(widget => widget.WidgetPosition)
                .ToList();

            for (var i = 1; i < studyUserWidgets.Count; i++)
            {
                var current = studyUserWidgets[i];
                var previous = studyUserWidgets[i - 1];

                if (current.WidgetPosition <= previous.WidgetPosition)
                {
                    current.WidgetPosition = previous.WidgetPosition + 1;
                }
            }

            _widgetRepository.SaveStudyUserWidgets(userId, studyUserWidgets);
        }

        [HttpPost]
        public async Task<JsonResult> RenderWidget(Guid widgetId)
        {
            var widget = await _widgetRepository.GetWidget(widgetId, CurrentSiteUserCultureCode, User.Id);
            return Json(new
            {
                widget = this.RenderRazorViewToString("~/Views/UI/Widget.cshtml", widget)
            });
        }
    }
}