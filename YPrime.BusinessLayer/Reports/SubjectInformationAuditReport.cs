using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.BusinessLayer.Reports;
using YPrime.Data.Study.Constants;

namespace YPrime.Reports.Reports
{
    public class SubjectInformationAuditReport : ReportBase, IReport
    {
        private const string StoredProcedureName = "[dbo].[PatientAuditRecReport_Filtered]";

        private Dictionary<string, string> _columns = null;
        private string _nomenclature = null;

        private readonly ITranslationService _translationService;
        private readonly ISubjectInformationService _subjectInformationService;
        private readonly IStudySettingService _studySettingService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ILanguageService _languageService;

        public SubjectInformationAuditReport(
            IStudyDbContext db,
            ITranslationService translationService,
            ISubjectInformationService subjectInformationService,
            IStudySettingService studySettingService,
            ILanguageService languageService,
            IPatientStatusService patientStatusService)
            : base(db)
        {
            _translationService = translationService;
            _subjectInformationService = subjectInformationService;
            _studySettingService = studySettingService;
            _languageService = languageService;
            _patientStatusService = patientStatusService;
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var pSites = "ALL";
            var pSubj = "ALL";
            var sites = "";
            var patiendId = Guid.Empty;

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
                var SiteList = await _db.StudyUserRoles.Where(x => x.StudyUserId == userId).Select(x => x.SiteId).ToListAsync();
                sites = string.Join(",", SiteList);
            }
            else
            {
                sites = pSites;
            }

            if (!string.IsNullOrEmpty(pSubj) || pSubj != "ALL")
            {
                patiendId = new Guid(pSubj);
            }

            var sqlParameters = await BuildSqlparameters(
                patiendId,
                sites);

            var procLabel = $"{StoredProcedureName} {string.Join(",", sqlParameters.Select(p => $"@{p.ParameterName}"))}";

            var rawData = _db.ExecuteSqlToList<SubjectInformationAuditDto>(
                    procLabel,
                    sqlParameters[0],
                    sqlParameters[1],
                    sqlParameters[2],
                    sqlParameters[3],
                    sqlParameters[4],
                    sqlParameters[5],
                    sqlParameters[6],
                    sqlParameters[7],
                    sqlParameters[8],
                    sqlParameters[9]);

            return rawData.ToReportData(GetColumnHeadings().Keys.ToList());
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            if (_columns == null)
            {
                var nomenclature = GetNomenclature();

                _columns = new Dictionary<string, string>
                {
                    {nameof(SubjectInformationAuditDto.Protocol), "Protocol"},
                    {nameof(SubjectInformationAuditDto.SiteNumber), "Site Number"},
                    {nameof(SubjectInformationAuditDto.SubjectNumber), nomenclature + " Number"},
                    {nameof(SubjectInformationAuditDto.SubjectAttribute), nomenclature + " Attribute"},
                    {nameof(SubjectInformationAuditDto.OldValue), "Old Value"},
                    {nameof(SubjectInformationAuditDto.NewValue), "New Value"},
                    {nameof(SubjectInformationAuditDto.ChangeReasonType), "Change Reason Type"},
                    {nameof(SubjectInformationAuditDto.ChangedBy), "Changed By"},
                    {nameof(SubjectInformationAuditDto.ChangedDate), "Changed Date"},
                    {nameof(SubjectInformationAuditDto.CorrectionReason), "Correction Reason"},
                    {nameof(SubjectInformationAuditDto.DCFNumber), "DCF Number"},
                    {nameof(SubjectInformationAuditDto.AssetTag), "Asset Tag"}
                };
            }

            return _columns;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        private async Task<List<SqlParameter>> BuildSqlparameters(
            Guid patientId,
            string sites)
        {
            var patient = await _db
               .Patients
               .FirstOrDefaultAsync(p => p.Id == patientId);

            var patientSite = await _db
                .Sites
                .FirstAsync(s => s.Id == patient.SiteId);

            var studyName = await _studySettingService
                .GetStringValue("StudyName");

            var pinLength = await _studySettingService
                .GetIntValue("PatientPINLength");

            var attributes = await _subjectInformationService
                .GetForCountry(patientSite.CountryId);

            var attributeStubs = attributes
                .Select(a => new SubjectInformationStub
                {
                    Id = a.Id,
                    Name = a.Name,
                    ChoiceType = a.ChoiceType.ToUpper(),
                    DateFormat = a.DateFormat
                });

            var attributeJson = SerializeObject(attributeStubs);

            var dataTypeStubs = DataType
                .GetAll<DataType>()
                .Select(dt => new ConfigStub<string>
                {
                    Id = dt.DisplayName.ToUpper(),
                    Name = dt.DisplayName
                });

            var dataTypeJson = SerializeObject(dataTypeStubs);

            var languages = await _languageService
                .GetAll();

            var languageStubs = languages
                .Select(l => new ConfigStub<Guid>
                {
                    Id = l.Id,
                    Name = l.Name
                });

            var languageJson = SerializeObject(languageStubs);

            var patientStatusTypes = await _patientStatusService.GetAll();
            var patientStatusTypeStubs = patientStatusTypes
                .Select(pst => new ConfigStub<int>
                {
                    Id = pst.Id,
                    Name = pst.Name
                });

            var statusTypeJson = SerializeObject(patientStatusTypeStubs);

            var choiceStubs = attributes
                .SelectMany(a => a.Choices)
                .Select(c => new ConfigStub<Guid>
                {
                    Id = c.Id,
                    Name = c.Name
                });

            var choiceJson = SerializeObject(choiceStubs);

            var nomenclature = GetNomenclature();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("StudyName", studyName),
                new SqlParameter("SubjectNumTranslation", nomenclature),
                new SqlParameter("PinSize", pinLength),
                new SqlParameter("SiteNumber", sites),
                new SqlParameter("PatientId", patient.Id),
                new SqlParameter("PatientAttributeJson", attributeJson),
                new SqlParameter("DataTypeJson", dataTypeJson),
                new SqlParameter("LanguageJson", languageJson),
                new SqlParameter("PatientStatusTypeJson", statusTypeJson),
                new SqlParameter("ChoicesJson", choiceJson),
            };

            return parameters;
        }

        private string GetNomenclature()
        {
            if (string.IsNullOrWhiteSpace(_nomenclature))
            {
                _nomenclature = Task.Run(async () => await _translationService.GetByKey(TranslationKeyTypes.lblPatient).ConfigureAwait(false)).Result
                    ?? "Subject";
            }

            return _nomenclature;
        }
    }
}