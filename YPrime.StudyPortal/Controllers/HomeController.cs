using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserRepository _UserRepository;

        public HomeController(
            IUserRepository StudyRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _UserRepository = StudyRepository;
        }

        public async Task<ActionResult> Index()
        {
            if (YPrimeSession.Instance.ConfigurationId == Config.Defaults.ConfigurationVersions.InitialVersion.Id)
            {
                return RedirectToAction("Index", "SoftwareRelease");
            }
            else if (YPrimeSession.Instance.CurrentUser != null)
            {
                if (YPrimeSession.Instance.CurrentUser.LandingPageUrl != null)
                {
                    User.LandingPageUrl = YPrimeSession.Instance.CurrentUser.LandingPageUrl;
                }
                else
                {
                    YPrimeSession.Instance.CurrentUser.LandingPageUrl =
                        await _UserRepository.GetLandingPageByUser(User, ControllerContext.RequestContext);
                    User.LandingPageUrl = YPrimeSession.Instance.CurrentUser.LandingPageUrl;
                }

                if (User.LandingPageUrl != null)
                {
                    return Redirect(User.LandingPageUrl);
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}