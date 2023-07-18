using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Session;

namespace YPrime.Reports.Reports
{
    public class DCFStatusReport : IReport
    {
        private const string ReportDateFormat = "dd-MMM-yyyy";
        private const string EmptyColumnValue = "N/A";
        private const string StringJoinSeperator = ", ";
        private const int CorrectionNumberPadLength = 4;
        private const char CorrectionNumberPadCharacter = '0';

        private readonly Dictionary<string, string> _columns = new Dictionary<string, string>
        {
            {"DCFNumber", "DCF Number"},
            {"Site", "Site"},
            {"Subject", "Subject"},
            {"DCFType", "DCF Type"},
            {"DCFStatus", "DCF Status"},
            {"DCFOpenedDate", "DCF Opened Date"},
            {"DCFClosedDate", "DCF Closed Date"},
            {"PendingApproverGroup", "Pending Approver Group"},
            {"CompletedApprovals", "Completed Approvals"},
            {"NumberOfDaysOpen", "# of Days DCF Open"},
            {"LastWorkflowId", ""}
        };

        private readonly List<Guid> _pendingDcfStatusIds = new List<Guid>
        {
            CorrectionStatusEnum.InProgress,
            CorrectionStatusEnum.NeedsMoreInformation,
            CorrectionStatusEnum.Pending
        };

        private readonly IStudyDbContext _db;
        private readonly IRoleRepository _roleRepository;
        private readonly ITranslationService _translationService;
        private readonly IApproverGroupService _approverGroupService;
        private readonly ICorrectionTypeService _correctionTypeService;

        private readonly Dictionary<string, string> _statusTranslations = new Dictionary<string, string>();

        public DCFStatusReport(
            IStudyDbContext db,
            IApproverGroupService approverGroupService,
            ITranslationService translationService,
            IRoleRepository roleRepository,
            ICorrectionTypeService correctionTypeService)
        {
            _db = db;
            _roleRepository = roleRepository;
            _translationService = translationService;
            _approverGroupService = approverGroupService;
            _correctionTypeService = correctionTypeService;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return _columns;
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var data = await GetData(userId);
            var results = data.ToReportData(_columns.Keys.ToList());
            return results;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        private async Task<List<DcfStatusReportDto>> GetData(Guid userId)
        {
            var corrections = await GetCorrections(userId);

            var reportData = new List<DcfStatusReportDto>();

            var showDcfLink = await _roleRepository.UserHasRoleAction(userId, "CanViewDCFList");

            var correctionTypes = await _correctionTypeService
                .GetAll();

            var approverGroups = await _approverGroupService
                .GetAll();

            foreach (var correction in corrections)
            {
                var reportItem = new DcfStatusReportDto
                {
                    CorrectionId = correction.Id.ToString(),
                    Site = correction?.Site?.SiteNumber,
                    DCFNumber = correction.DataCorrectionNumber
                        .ToString()
                        .PadLeft(CorrectionNumberPadLength, CorrectionNumberPadCharacter),
                    Subject = correction.Patient.PatientNumber,
                    DCFOpenedDate = correction.StartedDate.ToString(ReportDateFormat)
                };

                var correctionWorkflows = correction
                    ?.CorrectionWorkflows
                    ?.OrderBy(wf => wf.WorkflowOrder)
                    ?.ToList() ?? new List<CorrectionWorkflow>();

                reportItem.LastWorkflowId = GetDcfWorkflowId(
                    correction,
                    correctionWorkflows,
                    showDcfLink);

                var correctionType = correctionTypes
                    .FirstOrDefault(ct => ct.Id == correction.CorrectionTypeId);

                reportItem.DCFType = correctionType?.Name ?? string.Empty;

                var dcfStatusTranslation = await GetStatusTranslation(correction.CorrectionStatus.TranslationKey);

                reportItem.DCFStatus = dcfStatusTranslation ?? string.Empty;

                var completedUserNames = correction.CorrectionWorkflows
                    .Where(cw => cw.CorrectionActionId == CorrectionActionEnum.Approved)
                    .Select(cw => cw?.StudyUser?.UserName);

                reportItem.CompletedApprovals = string
                    .Join(StringJoinSeperator, completedUserNames.Where(un => !string.IsNullOrWhiteSpace(un)));

                if (string.IsNullOrWhiteSpace(reportItem.CompletedApprovals) || correction.NoApprovalNeeded)
                {
                    reportItem.CompletedApprovals = EmptyColumnValue;
                }

                var pendingApproverGroup = EmptyColumnValue;

                if (_pendingDcfStatusIds.Contains(correction.CorrectionStatusId))
                {
                    var workflow = correctionWorkflows
                        .FirstOrDefault(cw =>
                            cw.CorrectionActionId == CorrectionActionEnum.Pending ||
                            cw.CorrectionActionId == CorrectionActionEnum.NeedsMoreInformation);
                    if (workflow?.ApproverGroupId != null)
                    {
                        var pendingGroup = approverGroups.FirstOrDefault(ag => ag.Id == workflow.ApproverGroupId.Value);
                        pendingApproverGroup = pendingGroup?.Name;
                    }
                }

                reportItem.PendingApproverGroup = pendingApproverGroup;

                DateTimeOffset? completionDate = null;

                if (correction.CorrectionWorkflows.All(cw => cw.CorrectionActionId == CorrectionActionEnum.Approved))
                {
                    completionDate = correctionWorkflows
                        .LastOrDefault()
                        ?.WorkflowChangedDate;
                }
                else if (correction.CorrectionWorkflows.Any(
                    cw => cw.CorrectionActionId == CorrectionActionEnum.Rejected))
                {
                    completionDate = correctionWorkflows
                        .FirstOrDefault(cw => cw.CorrectionActionId == CorrectionActionEnum.Rejected)
                        ?.WorkflowChangedDate;
                }

                var dayPeriodEndingValue = completionDate ?? DateTime.Now;

                reportItem.DCFClosedDate = completionDate?.ToString(ReportDateFormat) ?? EmptyColumnValue;

                reportItem.NumberOfDaysOpen = ((dayPeriodEndingValue - correction.StartedDate).Days + 1).ToString();

                reportData.Add(reportItem);
            }

            reportData = reportData
                .OrderBy(rd => rd.DCFNumber)
                .ToList();

            return reportData;
        }

        private async Task<string> GetStatusTranslation(string key)
        {
            if (_statusTranslations.ContainsKey(key))
            {
                return _statusTranslations[key];
            }

            var translation = await _translationService.GetByKey(key);

            if (!string.IsNullOrWhiteSpace(translation))
            {
                _statusTranslations.Add(key, translation);
            }

            return translation;
        }

        private async Task<List<Correction>> GetCorrections(Guid? userId)
        {
            var siteIds = GetUserSiteIds(userId);
            var correctionFilter = new CorrectionFilter();
         
            var corrections = await correctionFilter.Execute(_db.Corrections)
                .Include(c => c.CorrectionWorkflows)
                .Include(c => c.CorrectionStatus)
                .Include(c => c.Site)
                .Include(c => c.Patient)
                .Where(c => siteIds.Contains(c.Patient.SiteId))
                .OrderBy(c => c.DataCorrectionNumber)
                .ToListAsync();

            return corrections;
        }

        private List<Guid> GetUserSiteIds(Guid? userId)
        {
            var ids = new List<Guid>();

            if (userId.HasValue)
            {
                ids = _db.StudyUserRoles
                    .Where(sur => sur.StudyUserId == userId.Value)
                    .Select(sur => sur.SiteId)
                    .ToList();
            }

            return ids;
        }

        private string GetDcfWorkflowId(
            Correction correction,
            List<CorrectionWorkflow> correctionWorkflows,
            bool showLink)
        {
            string link = null;

            if (showLink)
            {
                CorrectionWorkflow workflowToLink;

                if (correction.CorrectionStatusId == CorrectionStatusEnum.Completed)
                {
                    workflowToLink =
                        correctionWorkflows.FirstOrDefault(wf =>
                            wf.CorrectionActionId == CorrectionActionEnum.Approved);
                }
                else if (correction.CorrectionStatusId == CorrectionStatusEnum.Rejected)
                {
                    workflowToLink =
                        correctionWorkflows.FirstOrDefault(wf =>
                            wf.CorrectionActionId == CorrectionActionEnum.Rejected);
                }
                else if (correction.CorrectionStatusId == CorrectionStatusEnum.InProgress)
                {
                    workflowToLink =
                        correctionWorkflows.FirstOrDefault(oc =>
                            oc.CorrectionActionId != CorrectionActionEnum.Approved);
                }
                else
                {
                    workflowToLink = correction.CorrectionWorkflows.FirstOrDefault();
                }

                link = workflowToLink?.Id.ToString();
            }

            return link;
        }

        private class DcfStatusReportDto
        {
            public string CorrectionId { get; set; }

            public string Site { get; set; }

            public string DCFNumber { get; set; }

            public string Subject { get; set; }

            public string DCFType { get; set; }

            public string DCFStatus { get; set; }

            public string DCFOpenedDate { get; set; }

            public string DCFClosedDate { get; set; }

            public string PendingApproverGroup { get; set; }

            public string CompletedApprovals { get; set; }

            public string NumberOfDaysOpen { get; set; }

            public string LastWorkflowId { get; set; }
        }
    }
}