using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Constants;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Constants;

namespace YPrime.StudyPortal.Controllers
{
    [MenuGroup(MenuGroupType.AnalyticsReports)]
    public class AnalyticsController : BaseController
    {
        private readonly IBIEmbedService _powerBiEmbedService;
        private readonly ISiteRepository _siteRepository;

        public AnalyticsController(
            ISessionService sessionService,
            IBIEmbedService powerBiEmbedService,
            ISiteRepository siteRepository)
            : base(sessionService)
        {
            _powerBiEmbedService = powerBiEmbedService;
            _siteRepository = siteRepository;
        }

        [FunctionAuthorization(nameof(SystemActionTypes.CanViewAnalytics), SystemActionTypeDescriptions.CanViewAnalyticsDescription, true)]
        public ViewResult Index(string ReportType)
        {
            return View();
        }

        [HttpGet]
        public async Task<string> GetAnalyticsInWorkspace()
        {
            var analytics = await _powerBiEmbedService.GetAnalyticsInWorkspace();
            var orderedAnalytics = analytics.OrderBy(a => a.Report.Name);

            var json = new JavaScriptSerializer().Serialize(orderedAnalytics);

            return json;
        }

        [HttpGet]
        public async Task<string> GetEmbedConfig(string analyticsIdString)
        {
            var embedConfig = new EmbedConfig();
            try
            {
                var reportIdGuid = Guid.Parse(analyticsIdString);
                var sites = YPrimeSession.Instance?.CurrentUser.Sites.Select(s => s.Id.ToString()).ToArray();
                if (sites == null || !sites.Any())
                {
                    var allSites = await _siteRepository.GetAllSites(true);
                    sites = allSites.Select(s => s.Id.ToString()).ToArray(); // use all sites if for whatever reason session instance is null or no sites populated
                }

                embedConfig = await _powerBiEmbedService.GetEmbedConfig(reportIdGuid, sites);
                embedConfig.ErrorMessage = null;

                var json = new JavaScriptSerializer().Serialize(embedConfig);
                return json;
            }
            catch (Exception e)
            {
                embedConfig.ErrorMessage = e.ToString();
                var json = new JavaScriptSerializer().Serialize(embedConfig);
                return json;
            }
        }
    }
}