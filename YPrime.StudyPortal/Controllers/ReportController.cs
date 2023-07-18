using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.Models.DataTables;

namespace YPrime.StudyPortal.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportRepository _reportRepository;
        private new readonly int DefaultResultDisplayCount = 10;

        public ReportController(
            IReportRepository reportRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet]
        [FunctionAuthorization(nameof(SystemActionTypes.CanViewReports), SystemActionTypeDescriptions.CanViewReportsDescription, true)]
        public async Task<ActionResult> Index(string ReportType)
        {
            SetUpViewBag();

            var model = await CreateReportList();
            SetBaseViewState();
            ViewBag.UserId = User.Id;

            if (ReportType == null)
            {
                ReportType = "report";
            }

            ViewBag.ReportType = ReportType;

            return View(model);
        }

        [FunctionAuthorization(nameof(SystemActionTypes.CanViewReports), SystemActionTypeDescriptions.CanViewReportsDescription, true)]
        public async Task<ActionResult> Display(
            Guid id, 
            string sites = "", 
            string subjNo = "")
        {
            Dictionary<string, object> reportParameters = null;

            if (!string.IsNullOrEmpty(sites) || !string.IsNullOrEmpty(subjNo))
            {
                reportParameters = new Dictionary<string, object>
                {
                    { "SITES", sites },
                    { "SUBJ", subjNo }
                };
            }

            var report = await _reportRepository.GetReportDataObject(id, User.Id, reportParameters);
            report.Charts.Add(await _reportRepository.GetReportChartData(id, User.Id, reportParameters));

            ViewBag.ResultDisplayCount = DefaultResultDisplayCount;
            ViewBag.SiteNumber = sites;
            ViewBag.SubjectNumber = subjNo;
            ViewBag.EnableGridExportPDF = false;
            ViewBag.SiteUserCultureCode = CurrentSiteUserCultureCode;

            return PartialView(report);
        }

        [HttpPost]
        public async Task<ActionResult> GetReportGridData(
            Guid id, 
            DataTableAjaxPostModel settings, 
            string sites = "",
            string subjNo = "")
        {
            var queryParams = new Dictionary<string, object>
            {
                { "DatatablesSettings", settings }
            };

            if (!string.IsNullOrEmpty(sites) || !string.IsNullOrEmpty(subjNo))
            {
                queryParams.Add("SITES", sites);
                queryParams.Add("SUBJ", subjNo);
            }

            ReportDto report = await _reportRepository.GetReportDataObject(id, User.Id, queryParams);

            var response = TranslateReportDataToJsonResponse(report, settings);

            return Content(response.ToString(), "application/json");
        }

        private JObject TranslateReportDataToJsonResponse(ReportDto report, DataTableAjaxPostModel settings)
        {
            JObject result = new JObject();
            JProperty draw = new JProperty("draw", settings.Draw);
            JProperty tot = new JProperty("recordsTotal", report.TotalRecords);
            JProperty filt = new JProperty("recordsFiltered", report.TotalRecordsFiltered);
            JArray datums = new JArray();
            foreach (var row in report.ReportData)
            {
                JObject rowValue = new JObject();
                foreach (var key in row.Row.Keys)
                {
                    JProperty cellValue = new JProperty(key, FormatCell(row, key));
                    rowValue.Add(cellValue);
                }

                datums.Add(rowValue);
            }

            JProperty data = new JProperty("data", datums);
            result.Add(draw);
            result.Add(tot);
            result.Add(filt);
            result.Add(data);

            return result;
        }

        private string FormatCell(ReportDataDto dto, string key)
        {
            if (dto.Row[key] == null)
                return string.Empty;

            if (dto.Row[key].GetType() == typeof(DateTime))
                return ((DateTime) dto.Row[key]).ToString("dd/MMM/yyyy");

            if (dto.Row[key].GetType() == typeof(DateTimeOffset))
                return ((DateTimeOffset) dto.Row[key]).ToString("dd/MMM/yyyy");

            return dto.Row[key]?.ToString();
        }

        private async Task<List<ReportDisplayDto>> CreateReportList()
        {
            var result = new List<ReportDisplayDto>();
            var reportDto = await _reportRepository.GetAllReportsForDisplay(User.Id);
            result.AddRange(reportDto);

            return result;
        }

        private void SetUpViewBag()
        {
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

            var siteList = User.Sites
                .Select(x => x.Id + "|" + x.Name)
                .Distinct()
                .ToList();

            var nomenclature = htmlHelper.TranslationLabel(TranslationKeyTypes.lblPatient);
            var displayMsg = htmlHelper.TranslationLabel("DisplayAuditReport");

            ViewBag.Nomenclature = nomenclature ?? "Subject";
            ViewBag.Sites = string.Join(",", siteList);
            ViewBag.DisplayMsg = displayMsg;
            ViewBag.SiteUserCultureCode = CurrentSiteUserCultureCode;
        }
    }
}