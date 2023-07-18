using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.Reports.Factory
{
    public class ReportFactory : IReportFactory
    {
        private readonly IStudyDbContext _db;
        private readonly IAuthenticationUserRepository _authenticationUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStudyRoleService _studyRoleService;
        private readonly ITranslationService _translationService;
        private readonly ICountryService _countryService;
        private readonly IApproverGroupService _approverGroupService;
        private readonly ICorrectionTypeService _correctionTypeService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IStudySettingService _studySettingService;
        private readonly IVisitService _visitService;
        private readonly ILanguageService _languageService;
        private readonly ISubjectInformationService _subjectInformationService;
        private readonly IDiaryEntryRepository _diaryEntryRepository;

        public ReportFactory(
            IStudyDbContext db,
            IAuthenticationUserRepository authenticationUserRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IStudyRoleService studyRoleService,
            IApproverGroupService approverGroupService,
            ITranslationService translationService,
            ICountryService countryService,
            ICorrectionTypeService correctionTypeService,
            IPatientStatusService patientStatusService,
            IQuestionnaireService questionnaireService,
            IStudySettingService studySettingService,
            IVisitService visitService,
            ILanguageService languageService,
            ISubjectInformationService subjectInformationService,
            IDiaryEntryRepository diaryEntryRepository)
        {
            _db = db;
            _authenticationUserRepository = authenticationUserRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _studyRoleService = studyRoleService;
            _approverGroupService = approverGroupService;
            _translationService = translationService;
            _correctionTypeService = correctionTypeService;
            _patientStatusService = patientStatusService;
            _questionnaireService = questionnaireService;
            _studySettingService = studySettingService;
            _countryService = countryService;
            _visitService = visitService;
            _languageService = languageService;
            _subjectInformationService = subjectInformationService;
            _diaryEntryRepository = diaryEntryRepository;
        }

        public IReport CreateReport(ReportType reportType)
        {
            IReport report = null;
            switch (reportType.Id)
            {
                case var r when r == ReportType.AnswerAuditRecReportFiltered.Id:
                    report = new AnswerAuditReport(_db, _translationService, _questionnaireService, _studySettingService);
                    break;
                case var r when r == ReportType.AverageDiaryDuration.Id:
                    report = new AverageDiaryDurationReport(_diaryEntryRepository);
                    break;
                case var r when r == ReportType.DailyDiaryComplianceReport.Id:
                    report = new eCOASubjectComplianceReport(_db, _translationService, _patientStatusService);
                    break;
                case var r when r == ReportType.DCFStatusReport.Id:
                    report = new DCFStatusReport(_db, _approverGroupService, _translationService, _roleRepository, _correctionTypeService);
                    break;
                case var r when r == ReportType.PatientAuditRecReportFiltered.Id:
                    report = new SubjectInformationAuditReport(_db, _translationService, _subjectInformationService, _studySettingService, _languageService, _patientStatusService);
                    break;
                case var r when r == ReportType.PatientDuplicateReportByUser.Id:
                    report = new DuplicateSubjectReport(_db, _translationService, _patientStatusService);
                    break;
                case var r when r == ReportType.PatientVisitAuditRecReportFiltered.Id:
                    report = new SubjectVisitAuditReport(_db, _translationService, _visitService, _studySettingService);
                    break;
                case var r when r == ReportType.SiteDetailsReport.Id:
                    report = new SiteDetailsReport(_db, _countryService, _visitService, _userRepository);
                    break;
                case var r when r == ReportType.StudyUserReportByUser.Id:
                    report = new StudyUserReport(_db, _authenticationUserRepository, _studyRoleService);
                    break;
                case var r when r == ReportType.SubjectComplianceReportByUser.Id:
                    report = new eCOAComplianceReport(_db, _visitService, _patientStatusService, _translationService);
                    break;
                case var r when r == ReportType.TotalEnrollmentReport.Id:
                    report = new TotalEnrollmentReport(_db, _translationService, _patientStatusService);
                    break;
                case var r when r == ReportType.VisitComplianceReport.Id:
                    report = new VisitComplianceReport(_db, _visitService, _translationService, _questionnaireService, _patientStatusService);
                    break;
            }
            return report;
        }

    }
}
