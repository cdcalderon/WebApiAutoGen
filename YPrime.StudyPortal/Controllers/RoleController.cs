using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;
using YPrime.StudyPortal.Models;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.ManageStudy)]
    public class RoleController : BaseController
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IConfirmationRepository _ConfirmationRepository;
        private readonly IRoleRepository _RoleRepository;
        private readonly ISystemActionRepository _SystemActionsRepository;
        private readonly string CurrentRoleIdSessionKey = "CurrentRoleId";

        public RoleController(
            IAnalyticsRepository analyticsRepository,
            IRoleRepository RoleRepository,
            ISystemActionRepository SystemActionRepository,
            IConfirmationRepository ConfirmationRepository,
            IReportRepository reportRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _analyticsRepository = analyticsRepository;
            _RoleRepository = RoleRepository;
            _SystemActionsRepository = SystemActionRepository;
            _ConfirmationRepository = ConfirmationRepository;
            _reportRepository = reportRepository;
        }


        public string CurrentRoleId
        {
            get { return Session[CurrentRoleIdSessionKey].ToString(); }
            set { Session[CurrentRoleIdSessionKey] = value; }
        }


        /**********************************
         * INDEX PAGE
         * *******************************/
        // GET: Roles
        [FunctionAuthorization("CanViewRolesList", "View System Roles", true)]
        public ActionResult Index()
        {
            SetViewData();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetRolesGridData()
        {
            /* Create an instance of HtmlHelper... */
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            var allRoles = await _RoleRepository
                .GetAllRoles();
            var roles = allRoles.ToList();

            foreach (var role in roles)
            {
                var encodedString = ToBase64(role.ShortName);
                role.SetPermissionsButtonHTML = htmlHelper.PrimeActionLink("Set Permissions", "SetPermissions", "Role",
                    new {id = encodedString }, new {@class = "btn btn-primary btn-block"}, false).ToHtmlString();
                role.SetSubscriptionsButtonHTML = htmlHelper.PrimeActionLink("Set Subscriptions", "SetSubscriptions",
                        "Role", new {id = encodedString }, new {@class = "btn btn-primary btn-block"}, false)
                    .ToHtmlString();
                role.SetReportsButtonHTML = htmlHelper.PrimeActionLink("Set Reports", "SetReportStudyRoles", "Role",
                    new {id = encodedString }, new {@class = "btn btn-primary btn-block"}, false).ToHtmlString();
                role.SetAnalyticsButtonHTML = htmlHelper.PrimeActionLink("Set Analytics", "SetAnalyticsStudyRoles", "Role",
                    new { id = encodedString }, new { @class = "btn btn-primary btn-block" }, false).ToHtmlString();
            }

            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        private string ToBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var textAsBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        private string FromBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var bytesAsText = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytesAsText);
        }


        /**********************************
         * END INDEX PAGE
         * *******************************/

        /**********************************
         * SET PERMISSIONS PAGE
         * *******************************/
        [FunctionAuthorization("CanViewRolesList", "View System Roles", true)]
        public async Task<ActionResult> SetPermissions(string id)
        {
            id = FromBase64(id);
            var role = await _RoleRepository.GetRole(id);
            CurrentRoleId = id;
            var replaceString = "Controller";

            var systemActions = _SystemActionsRepository.GetAllSystemActions();
            role.SystemActions = _RoleRepository.GetRoleActions(role.Id, role.IsBlinded);
            systemActions = systemActions.Where(sa => !role.IsBlinded || sa.IsBlinded).ToList();

            ViewData["CurrentRole"] = role;
            ViewData["Roles"] = GetRolesList(role.ShortName);

            var group = systemActions
                .GroupBy(sa => Regex.Replace(sa.ActionLocation.Split(':')[0].Replace(replaceString, ""),
                    "([a-z])([A-Z])", "$1 $2"))
                .ToDictionary(sa => sa.Key, sa => sa.Select(s => s).OrderBy(s => s.Description));
            group.Keys.ToList().ForEach(k =>
            {
                group[k].ToList().ForEach(sa =>
                {
                    sa.AssociatedToUser = role.SystemActions.Any(rsa => rsa.Id == sa.Id);
                });
            });

            SetViewData();
            return View(group);
        }

        public JsonResult UpdateRolePermission(Guid RoleId, Guid SystemActionId, bool Add)
        {
            var result = new AjaxResult();

            if (Add)
            {
                _RoleRepository.AddActionToRole(RoleId, SystemActionId);
            }
            else
            {
                _RoleRepository.RemoveActionFromRole(RoleId, SystemActionId);
            }

            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /**********************************
        * END SET PERMISSIONS PAGE
        * *******************************/

        /**********************************
         * SET SUBSCRIPTIONS PAGE
         * *******************************/
        public async Task<ActionResult> SetSubscriptions(string id)
        {
            id = FromBase64(id);
            //todo: refactor this so the names match
            var role = await _RoleRepository.GetRole(id);
            CurrentRoleId = id;

            var confirmationTypes = _ConfirmationRepository.GetAllEmailTemplates();
            var roleSubscriptions = await _RoleRepository.GetRoleSubscriptions(role.Id, role.IsBlinded);
            confirmationTypes = confirmationTypes.Where(ct => !role.IsBlinded || ct.IsBlinded).OrderBy(ct => ct.Name)
                .ToList();

            confirmationTypes.ForEach(ct => { ct.AssociatedToUser = roleSubscriptions.Any(s => s.Id == ct.Id); });

            ViewData["CurrentRole"] = role;

            SetViewData();
            return View(confirmationTypes);
        }

        public JsonResult UpdateRoleSubscription(Guid RoleId, Guid SubscriptionId, bool Add)
        {
            var result = new AjaxResult();

            if (Add)
            {
                _RoleRepository.AddSubscriptionToRole(RoleId, SubscriptionId);
            }
            else
            {
                _RoleRepository.RemoveSubscriptionFromRole(RoleId, SubscriptionId);
            }

            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /**********************************
        * END SET SUBSCRIPTIONS PAGE
        * *******************************/


        /**********************************
         * SET REPORTS PAGE
         * *******************************/
        public async Task<ActionResult> SetReportStudyRoles(string id)
        {
            id = FromBase64(id);
            var role = await _RoleRepository.GetRole(id);
            CurrentRoleId = id;

            var reportsForRole = _RoleRepository.GetAllReportsByRole(role.Id);

            var allReports = (await _reportRepository.GetAllReportsForDisplay(role))
                .Select(x => new ReportDto
                {
                    Id = x.DisplayId,
                    ReportName = x.ReportTitle,
                    ReportStudyType = "eCOA"
                })
                .ToList();

            allReports.ForEach(r => { r.AssociatedToUser = reportsForRole.Any(rr => rr.Id == r.Id); });

            ViewData["CurrentRole"] = role;

            SetViewData();
            return View(allReports);
        }

        public JsonResult UpdateRoleReport(Guid RoleId, Guid ReportId, bool Add)
        {
            var result = new AjaxResult();

            if (Add)
            {
                _RoleRepository.AddReportToRole(RoleId, ReportId);
            }
            else
            {
                _RoleRepository.RemoveReportFromRole(RoleId, ReportId);
            }

            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /**********************************
        * END SET REPORTS PAGE
        * *******************************/

        /**********************************
         * SET ANALYTICS PAGE
         * *******************************/
        public async Task<ActionResult> SetAnalyticsStudyRoles(string id)
        {
            id = FromBase64(id);
            var role = await _RoleRepository.GetRole(id);
            CurrentRoleId = id;

            var analyticsForRole = await _RoleRepository.GetAllAnalyticsReferencesByRole(role.Id);

            var allAnalytics = _analyticsRepository
                                .GetAllAnalyticsReferences()
                                .Select(a => new AnalyticsDto
                                {
                                    Id = a.Id,
                                    DisplayName = a.DisplayName,
                                    AssociatedToUser = analyticsForRole.Any(ar => ar.Id == a.Id)
                                })
                                .ToList();

            ViewData["CurrentRole"] = role;

            SetViewData();
            return View(allAnalytics);
        }

        public JsonResult UpdateRoleAnalytics(Guid RoleId, Guid ReportId, bool Add)
        {
            var result = new AjaxResult();

            if (Add)
            {
                _RoleRepository.AddAnalyticsToRole(RoleId, ReportId);
            }
            else
            {
                _RoleRepository.RemoveAnalyticsFromRole(RoleId, ReportId);
            }

            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /**********************************
        * END SET ANALYTICS PAGE
        * *******************************/

        private void SetViewData(string selectedLandingPage = null)
        {
            ViewBag.CurrentSiteName = CurrentSiteDto?.Name ?? "No Site Selected";
        }

        public async Task<SelectList> GetRolesList(string SelectedValue)
        {
            var roles = await  _RoleRepository.GetAllRoles();
            SelectList result;
            result = new SelectList(roles.ToList().OrderBy(u => u.ShortName).AsEnumerable(), "ShortName", "ShortName",
                SelectedValue);

            return result;
        }
    }
}