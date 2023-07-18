using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.Data.Study.Constants;
using System.Data.Entity;

namespace YPrime.Reports.Reports
{
    public class SubjectVisitAuditReport : IReport
    {
        private readonly Dictionary<string, string> _columns;
        private readonly IStudyDbContext _db;
        private readonly ITranslationService _translationService;
        private readonly IVisitService _visitService;
        private readonly IStudySettingService _studySettingService;

        public SubjectVisitAuditReport(
            IStudyDbContext db,
            ITranslationService translationService,
            IVisitService visitService,
            IStudySettingService studySettingService)
        {
            _db = db;
            _translationService = translationService;
            _visitService = visitService;
            _studySettingService = studySettingService;

            var nomenclature = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatient).ConfigureAwait(false)).Result
                 ?? "Subject";

            _columns = new Dictionary<string, string>
            {
                {nameof(PatientVisitAuditDto.Protocol), "Protocol"},
                {nameof(PatientVisitAuditDto.SiteNumber), "Site Number"},
                {nameof(PatientVisitAuditDto.SubjectNumber), nomenclature + " Number"},
                {nameof(PatientVisitAuditDto.VisitName), "Visit Name"},
                {nameof(PatientVisitAuditDto.SubjectVisitAttribute), "Subject Visit Attribute"},
                {nameof(PatientVisitAuditDto.OldValue), "Old Value"},
                {nameof(PatientVisitAuditDto.NewValue), "New Value"},
                {nameof(PatientVisitAuditDto.ChangeReasonType), "Change Reason Type"},
                {nameof(PatientVisitAuditDto.ChangedBy), "Changed By"},
                {nameof(PatientVisitAuditDto.ChangedDate), "Changed Date"},
                {nameof(PatientVisitAuditDto.CorrectionReason), "Correction Reason"},
                {nameof(PatientVisitAuditDto.DCFNumber), "DCF Number"},
                {nameof(PatientVisitAuditDto.AssetTag), "Asset Tag"}
            };
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var pSites = "ALL";
            var pSubj = "ALL";
            var patientId = Guid.Empty;

            if (parameters.ContainsKey("SITES"))
            {
                pSites = parameters["SITES"].ToString();
            }

            if (parameters.ContainsKey("SUBJ"))
            {
                pSubj = parameters["SUBJ"].ToString();
            }

            if (string.IsNullOrEmpty(pSites) || pSites == "ALL")
            {
                var SiteList = _db.StudyUserRoles.Where(x => x.StudyUserId == userId).Select(x => x.SiteId).ToList();
                pSites = string.Join(",", SiteList);
            }

            var protocol = await _studySettingService.GetStringValue("Protocol");

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,

            };

            var visits = await _visitService.GetAll();
            var visitJson = JsonConvert.SerializeObject(
                visits,
                Formatting.None,
                serializerSettings);

            if (!string.IsNullOrEmpty(pSubj) && pSubj != "ALL")
            {
                patientId = new Guid(pSubj);
            }

            var subject = await GetPatientNumberById(patientId);
            var patientVisitStatusTypes = PatientVisitStatusType.GetAll<PatientVisitStatusType>();
            var patientVisitStatusJson = JsonConvert.SerializeObject(
                patientVisitStatusTypes,
                Formatting.None,
                serializerSettings);

            var spName = "[dbo].[PatientVisitAuditRecReport_Filtered] @p1,@p2,@p3,@p4,@p5";

            var rawData = await _db.Database
            .SqlQuery<PatientVisitAuditDto>(
                spName,
                new SqlParameter("p1", pSites),
                new SqlParameter("p2", subject),
                new SqlParameter("p3", protocol),
                new SqlParameter("p4", visitJson),
                new SqlParameter("p5", patientVisitStatusJson))
            .ToListAsync();

            return rawData.ToReportData(GetColumnHeadings().Keys.ToList());
        }

        private async Task<string> GetPatientNumberById(Guid patientId)
        {
            var patient = await _db
               .Patients
               .FirstOrDefaultAsync(p => p.Id == patientId);

            return patient?.PatientNumber;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return _columns;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }
    }
}