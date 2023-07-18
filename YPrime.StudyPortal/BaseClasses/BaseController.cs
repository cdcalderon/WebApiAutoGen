using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        public const int DefaultResultDisplayCount = 50;

        private const string CurrentSiteDtoSessionKey = "CurrentSiteDto";
        private const string CurrentSiteIdSessionKey = "CurrentSiteId";
        private const string DefaultCultureCode = "en-US";
        private const string ViewDataHasLoadupMessageKey = "HasLoadupMessage";
        private const string ViewDataMessageGuid = "PopupMessageGuid";
        private const string ViewDataMessageKey = "ReloadPopupMessage";
        private const string ViewDataMessageTitleKey = "ReloadPopupMessageTitle";

        protected readonly ISessionService _sessionService;

        protected BaseController(
            ISessionService sessionService)
        {
            if (sessionService == null) throw new ArgumentNullException(nameof(ISessionService));

            _sessionService = sessionService;

            SetSessionServiceState();
            SetBaseViewState();
        }

        protected new virtual StudyUserDto User => YPrimeSession.Instance?.CurrentUser;

        public string CurrentSiteUserCultureCode => StudyUserDto.CultureCode;

        public string CurrentPatientCultureCode => DefaultCultureCode;

        public Guid? CurrentSiteId
        {
            get
            {
                return Session == null || Session[CurrentSiteIdSessionKey] == null ||
                       string.IsNullOrWhiteSpace(Session[CurrentSiteIdSessionKey].ToString())
                    ? (Guid?)null
                    : Guid.Parse(Session[CurrentSiteIdSessionKey].ToString());
            }
            set { Session[CurrentSiteIdSessionKey] = value; }
        }

        public SiteDto CurrentSiteDto
        {
            get
            {
                if (Session[CurrentSiteDtoSessionKey] == null)
                {
                    Session[CurrentSiteDtoSessionKey] = new SiteDto(); //this is all available sites
                }

                return (SiteDto)Session[CurrentSiteDtoSessionKey];
            }
            set
            {
                Session[CurrentSiteDtoSessionKey] = value;
                ViewBag.CurrentSiteName = value?.Name ?? null;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public Core.BusinessLayer.Models.StudyRoleModel CurrentStudyRole => User != null ? User.Roles.FirstOrDefault() : null;

        public void SetBaseViewState()
        {
            ViewBag.ApplicationVersion = ApplicationVersion;
            ViewBag.SiteUserCultureCode = CurrentSiteUserCultureCode;
            ViewBag.PatientCultureCode = CurrentPatientCultureCode;
            ViewBag.CurrentStudyRole = CurrentStudyRole;
            ViewBag.CurrentProtocol = YPrimeSession.Instance?.Protocol;
            ViewBag.SupportChatURL = YPrimeSession.Instance?.SupportChatURL;
            ViewBag.IsSupportChatEnabled = (YPrimeSession.Instance?.SupportChatEnabled);
            ViewBag.IsAuthenticated = YPrimeSession.Instance?.CurrentUserAuth0 == null ? false : YPrimeSession.Instance.CurrentUserAuth0.IsAuthenticated ? true : false;
            ViewBag.EnableGridExport = true;
            ViewBag.LandingPageUrl = User?.LandingPageUrl;
            SetMenuGroup();
        }

        protected void SetSessionServiceState()
        {
            _sessionService.UserConfigurationId = YPrimeSession.Instance?.ConfigurationId ?? Config.Defaults.ConfigurationVersions.InitialVersion.Id;
        }

        public async Task<List<SiteDto>> GetUserSites(ISiteRepository _SiteRepository)
        {
            var result = User.Sites;
            if (!result.Any())
            {
                result = await _SiteRepository.GetSitesForUser(User.Id);
            }

            return result;
        }


        public void SetPopUpMessageOnLoad(string Title, string Message, string Id = null)
        {
            Session[ViewDataHasLoadupMessageKey] = true;
            Session[ViewDataMessageTitleKey] = Title;
            Session[ViewDataMessageKey] = Message;

            if (Id != null)
            {
                Session[ViewDataMessageGuid] = Id;
            }
            else
            {
                Session[ViewDataMessageGuid] = Guid.NewGuid();
            }

        }

        private void SetMenuGroup()
        {
            var menuGroupAttribute = GetType().GetCustomAttributes(typeof(MenuGroupAttribute), false).FirstOrDefault();
            if (menuGroupAttribute != null)
            {
                var menuGroupAttr = menuGroupAttribute as MenuGroupAttribute;
                ViewBag.MenuGroup = menuGroupAttr.Type;
            }
        }

        protected T ExecuteAsyncActionSynchronously<T>(Func<Task<T>> taskToExecute)
        {
            var result = Task
                .Run(async () => await taskToExecute.Invoke().ConfigureAwait(false))
                .GetAwaiter()
                .GetResult();

            return result;
        }
    }
}