using Config.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Query.Interfaces;
using YPrime.BusinessLayer.Query.Parameters;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessRule.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.DTOLibrary.WebBackup;
using System.Data.Entity;
using System.Text.RegularExpressions;
using YPrime.BusinessLayer.Filters;

namespace YPrime.BusinessLayer.Repositories
{
    public class PatientVisitRepository : BaseRepository, IPatientVisitRepository
    {
        private const string NumericRegexPattern = @"\d+";

        private readonly IDiaryEntryRepository _diaryEntryRepository;
        private readonly IRuleService _ruleService;
        private readonly ITranslationService _translationService;
        private readonly IWebBackupRepository _webBackupRepository;
        private readonly IVisitService _visitService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IStudySettingService _studySettingService;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IPatientVisitSummaryQueryHandler _patientVisitSummaryQueryHandler;

        public PatientVisitRepository(
            IStudyDbContext db,
            IWebBackupRepository webBackupRepository,
            ITranslationService translationService,
            IDiaryEntryRepository diaryEntryRepository,
            IRuleService ruleService,
            IVisitService visitService,
            IPatientStatusService patientStatusService,
            IQuestionnaireService questionnaireService,
            IStudySettingService studySettingService,
            ISiteRepository siteRepository,
            ISystemSettingRepository systemSettingRepository,
            IPatientVisitSummaryQueryHandler patientVisitSummaryQueryHandler)
            : base(db)
        {
            _webBackupRepository = webBackupRepository;
            _translationService = translationService;
            _diaryEntryRepository = diaryEntryRepository;
            _ruleService = ruleService;
            _visitService = visitService;
            _questionnaireService = questionnaireService;
            _studySettingService = studySettingService;
            _siteRepository = siteRepository;
            _patientStatusService = patientStatusService;
            _systemSettingRepository = systemSettingRepository;
            _patientVisitSummaryQueryHandler = patientVisitSummaryQueryHandler;
        }
                
        public async Task<PatientVisitDto> GetPatientVisit(Guid PatientVisitId, string CultureCode)
        {
            var allVisits = await _visitService.GetAll();

            var visits = allVisits.ToDictionary(k => k.Id, v => v.Name);

            PatientVisitDto result = await _db.PatientVisits
                .Where(pv => pv.Id == PatientVisitId)
                .ToList()
                .Select(async pv => await CreatePatientVisitToDropDown(pv, new List<DiaryEntryDto>(), true, CultureCode, visits))
                .Single();

            return result;
        }

        public async Task<List<PatientVisitDto>> GetPatientVisits(Guid PatientId, List<int> visitStatusTypesToInclude, bool IncludeProjectedVisits, string CultureCode)
        {
            var result = _db.PatientVisits
                .Where(pv => pv.PatientId == PatientId && (IncludeProjectedVisits || pv.VisitDate != null || visitStatusTypesToInclude.Contains(pv.PatientVisitStatusTypeId)))
                .ToList();

            var allVisits = await _visitService.GetAll();

            var visits = allVisits.ToDictionary(k => k.Id, v => v.Name);
           
            var resultList = new List<PatientVisitDto>();

            foreach (var pv in result)
            {
                resultList.Add(await CreatePatientVisitToDropDown(pv, new List<DiaryEntryDto>(), false, CultureCode, visits));
            }

            var visitOrders = allVisits.ToDictionary(v => v.Id, v => v.VisitOrder);
            var finalResults = resultList.OrderBy(pv => visitOrders.TryGetValue(pv.VisitId, out var visitOrder) ? visitOrder : resultList.Count())
                .ToList();

            return finalResults;
        }

        public async Task<IEnumerable<PatientVisitDto>> GetById(IEnumerable<Guid> ids, string CultureCode)
        {
            var results = _db.PatientVisits
                .Where(pv => ids.Contains(pv.Id))
                .ToList();

            var resultList = new List<PatientVisitDto>();

            var allVisits = await _visitService.GetAll();

            var visits = allVisits.ToDictionary(k => k.Id, v => v.Name);

            foreach (var pv in results)
            {
                resultList.Add(await CreatePatientVisitDto(pv, new List<DiaryEntryDto>(), true, CultureCode, visits));
            }
               
            return resultList;
        }

        public async Task<List<PatientVisitDto>> GetAllPatientVisit(Guid PatientID, string CultureCode)
        {
            var diaryEntries = _diaryEntryRepository
                .GetDiaryEntries(PatientID, new List<QuestionnaireModel>()).ToList();

            var result = _db.PatientVisits
                .Where(pv => pv.PatientId == PatientID)
                .OrderBy(pv => pv.VisitDate)
                .ToList();

            var resultList = new List<PatientVisitDto>();

            var allVisits = await _visitService.GetAll();

            var visits = allVisits.ToDictionary(k => k.Id, v => v.Name);

            foreach (var pv in result)
            {
                resultList.Add(await CreatePatientVisitDto(pv, diaryEntries, false, CultureCode, visits));
            }

            return resultList;
        }

        public async Task<List<PatientVisitDto>> GetUnassociatedCompletedPatientVisits(List<string> Sites, string CultureCode)
        {
            int completedPatientVisitStatusTypeId = 4;

           var results = _db.PatientVisits
                .Where(pv => pv.PatientVisitStatusTypeId == completedPatientVisitStatusTypeId)
                .ToList();

            var resultList = new List<PatientVisitDto>();

            var allVisits = await _visitService.GetAll();

            var visits = allVisits.ToDictionary(k => k.Id, v => v.Name);

            foreach (var pv in results)
            {
                resultList.Add(await CreatePatientVisitDto(pv, new List<DiaryEntryDto>(), false, CultureCode, visits));
            }

            return resultList;
        }

        public async Task ProjectPatientVisitSchedule(Guid patientId)
        {
            var patient = _db.Patients.SingleOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                var visits = await GetAllScheduledVisits();
                var patientVisits = new List<PatientVisit>();

                foreach (var visit in visits)
                {
                    var patientVisit = new PatientVisit
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.Id,
                        Patient = patient,
                        VisitId = visit.Id,                   
                        PatientVisitStatusTypeId = PatientVisitStatusType.NotStarted.Id,
                        Notes = visit.Notes,
                        OutsideVisitWindow = false,
                        UnscheduledVisitOrder = null,
                        VisitDate = null,
                        ProjectedDate = GetProjectedVisitDate(patient, visit, visits, patientVisits),
                        SyncVersion = 0,
                        ConfigurationId = patient.ConfigurationId
                    };
                    patientVisits.Add(patientVisit);
                }

                _db.PatientVisits.AddRange(patientVisits);
                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }
        }

        public bool IseCOAVisitInWindow(string visitStop_HSN, int windowBefore, int windowAfter,
            DateTimeOffset projectedDate, string siteTimeZone)
        {
            var isVisitInWindow = false;
            if (visitStop_HSN == "N")
            {
                isVisitInWindow = true;
            }
            else
            {
                var expectedDate = projectedDate.Date;
                var maxDate = expectedDate.AddDays(windowAfter);
                var minDate = expectedDate.AddDays(-windowBefore);
                var siteTime = DateTimeOffset.Now.ConvertToTimeZone(siteTimeZone).Date;

                if ((siteTime >= minDate) &&
                    (siteTime <= maxDate))
                {
                    isVisitInWindow = true;
                }
            }

            return isVisitInWindow;
        }

        public async Task<string> GetPatientVisitDescription(PatientVisit PatientVisit, string CultureCode)
        {
            var visit = await _visitService.Get(PatientVisit.VisitId);  

            var result = await _translationService.GetByKey("lblVisit") + " " + visit?.Name + " " +
                         PatientVisit.VisitDate;

            return result;
        }
                
        public async Task<IEnumerable<PatientVisitSummary>> GetPatientVisitSummary(Guid patientId,
            bool canActivateVisitPortal = false,
            bool canAccessTabletWebBackup = false)
        {
            List<PatientVisitSummary> patientVisitSummaries = new List<PatientVisitSummary>();

            var studyCustoms = await _studySettingService.GetAllStudyCustoms(YPrimeSession.Instance.ConfigurationId);            
            var queryParameters = new PatientVisitSummaryQueryParameters(patientId);            
            var patientVisits = await _patientVisitSummaryQueryHandler.ReadPatientVisitsForPatientVisitSummary(queryParameters);
            var diaryEntries = await _patientVisitSummaryQueryHandler.ReadDiaryEntriesForPatientVisitSummary(queryParameters);
            var site = await _patientVisitSummaryQueryHandler.ReadSiteForPatientVisitSummary(queryParameters);
            var visits = await _visitService.GetAll();
            var questionnaires = await _questionnaireService.GetAll();
            var handHeldVisitActivationEnabled = studyCustoms.FirstOrDefault(s => s.Key == "ActivateSubjectForms")?.GetIntValue() == 1;
            var caregiverPatientFormsEnabled = studyCustoms.FirstOrDefault(s => s.Key == "CaregiverPatientFormsEnabled")?.GetIntValue() == 1;
            var ignoreVisitOrder = studyCustoms.FirstOrDefault(s => s.Key == "IgnoreVisitOrder")?.GetIntValue() > 0;
            var eligibleForWebBackup = EligibleForWebBackUp(studyCustoms, canAccessTabletWebBackup, site.WebBackupExpireDate, site.TimeZone);

            foreach (var pv in patientVisits)
            {   
                var visitQuestionnaireIds = visits.Where(v => v.Id == pv.VisitId).SelectMany(v => v.Questionnaires.Select(q => q.QuestionnaireId));
                var clinicianOnlyVisit = questionnaires.Where(q => visitQuestionnaireIds.Contains(q.Id)).All(q => q.QuestionnaireTaker.QuestionnaireTypeId == QuestionnaireType.Clinician.Id);
                var visit = visits.First(x => x.Id == pv.VisitId);
                var visitStatusName = PatientVisitStatusType.FirstOrDefault<PatientVisitStatusType>(v => v.Id == pv.PatientVisitStatusTypeId)?.Name;
                var isAvailable = false;

                if (!visit.VisitAvailableBusinessRuleId.HasValue ||
                    BusinessRuleIsValid(visit.VisitAvailableBusinessRuleId,
                        visit.VisitAvailableBusinessRuleTrueFalseIndicator, patientId, site.Id))
                {
                    isAvailable = true;
                }

                if (isAvailable)
                {
                    var pvSum = new PatientVisitSummary()
                    {
                        Id = pv.Id,
                        PatientId = pv.PatientId,
                        CanActivateVisit = canActivateVisitPortal && handHeldVisitActivationEnabled && !clinicianOnlyVisit &&
                                            pv.PatientVisitStatusTypeId == PatientVisitStatusType.NotStarted.Id,
                        VisitId = pv.VisitId,
                        PatientVisitStatusTypeId = pv.PatientVisitStatusTypeId,
                        PatientVisitStatus = visitStatusName,
                        ActivationDate = pv.ActivationDate,
                        ProjectedDate = pv.ProjectedDate,
                        VisitName = visit.Name,
                        VisitOrder = visit.VisitOrder,
                        VisitDate = pv.VisitDate,
                        VisitStop_HSN = visit.VisitStop_HSN,
                        IsScheduled = visit.IsScheduled,
                        AlwaysAvailable = visit.AlwaysAvailable,
                        WindowAfter = visit.WindowAfter,
                        WindowBefore = visit.WindowBefore,
                        VisitAvailableBusinessRuleId = visit.VisitAvailableBusinessRuleId,
                        VisitAvailableBusinessRuleTrueFalseIndicator =
                           visit.VisitAvailableBusinessRuleTrueFalseIndicator,
                        DiaryEntries = diaryEntries.Where(x => x.VisitId == pv.VisitId).ToList()
                    };

                    if (eligibleForWebBackup)
                    {
                        await UpdateSummaryForWebBackup(pvSum,
                            patientId,
                            site.Id,
                            (DateTime)site.WebBackupExpireDate,
                            visits,
                            questionnaires,
                            caregiverPatientFormsEnabled);
                    }

                    patientVisitSummaries.Add(pvSum);
                }
            }

            patientVisitSummaries = patientVisitSummaries.OrderBy(pv => pv.VisitOrder).ToList();

            var firstScheduledVisit = patientVisitSummaries
                .FirstOrDefault(pvs =>
                    pvs.PatientVisitStatusTypeId == PatientVisitStatusType.NotStarted.Id ||
                    pvs.PatientVisitStatusTypeId == PatientVisitStatusType.InProgress.Id);

            if (firstScheduledVisit != null)
            {
                firstScheduledVisit.IsNextScheduled = true;
            }

            await SetPatientVisitHardStops(patientVisitSummaries, ignoreVisitOrder, site.TimeZone);

            return patientVisitSummaries;
        }

        public async Task<PatientVisit> ActivatePatientVisit(Guid PatientVisitId, Guid PatientId)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patient = patientFilter.Execute(_db.Patients)
                .FirstOrDefault(p => p.Id == PatientId);

            var patientVisits = _db.PatientVisits.Where(pv => pv.PatientId == PatientId).ToList();
            var patientVisitToActivate = patientVisits.FirstOrDefault(p => p.Id == PatientVisitId);
            var siteFilter = new SiteFilter();

            var site = siteFilter.Execute(_db.Sites)
                .FirstOrDefault(s => s.Id == patient.SiteId);

            if (patientVisitToActivate != null)
            {
                if (!patientVisitToActivate.ActivationDate.HasValue)
                {
                    patientVisitToActivate.ActivationDate = DateTimeOffset.Now.ConvertToTimeZone(site.TimeZone);
                }

                var allVisits = await _visitService.GetAll();
                var currentVisitOrder = allVisits.FirstOrDefault(v => v.Id == patientVisitToActivate.VisitId)?.VisitOrder;
                var previousVisits = allVisits.Where(v => v.VisitOrder < currentVisitOrder).OrderBy(pv => pv.VisitOrder);

                foreach (var previousVisit in previousVisits)
                {
                    var previousPatientVisit = patientVisits.FirstOrDefault(pv => pv.VisitId == previousVisit.Id);
                    if (previousPatientVisit?.PatientVisitStatusTypeId == PatientVisitStatusType.InProgress.Id
                        || previousPatientVisit?.PatientVisitStatusTypeId == PatientVisitStatusType.NotStarted.Id)

                    {
                        previousPatientVisit.PatientVisitStatusTypeId = (previousPatientVisit.PatientVisitStatusTypeId == 
                            PatientVisitStatusType.InProgress.Id) ?
                            PatientVisitStatusType.Partial.Id : PatientVisitStatusType.Missed.Id;
                    }
                }

                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }

            return patientVisitToActivate;
        }

        private bool EligibleForWebBackUp(List<StudyCustomModel> studyCustoms, bool canAccessTabletWebBackup
            , DateTime? siteWebBackupExpireDate, string siteTimeZone)
        {
            var daysIsValid = studyCustoms.FirstOrDefault(s => s.Key == "WebBackupTabletEnabled")?.GetIntValue() > 0;
            var webbackupKey = _systemSettingRepository.GetSystemSettingValue("WebBackupTabletPublicKey");
            var keyIsValid = !string.IsNullOrEmpty(webbackupKey);

            var siteIsValid = siteWebBackupExpireDate != null && siteWebBackupExpireDate >= siteTimeZone.ToLocalDateTime();

            return daysIsValid && keyIsValid && canAccessTabletWebBackup && siteIsValid;
        }

        private async Task UpdateSummaryForWebBackup(
            PatientVisitSummary patientVisitSummary,
            Guid patientId,
            Guid siteId,
            DateTime siteExpiration,
            List<VisitModel> visits,
            List<QuestionnaireModel> questionnaires,
            bool caregiverPatientForms)
        {
            var visitQuestionnaires = new Dictionary<Guid, IEnumerable<QuestionnaireModel>>();
            foreach (var q in visits)
            {
                visitQuestionnaires.Add(q.Id, questionnaires
                    .Where(x => q.Questionnaires
                    .Select(y => y.QuestionnaireId).Contains(x.Id)));
            }

            var caregiverQuestionnaireTypes =
                caregiverPatientForms
                    ? new List<int> { QuestionnaireType.Patient.Id, QuestionnaireType.Observer.Id }
                    : new List<int> { QuestionnaireType.Observer.Id };

            patientVisitSummary.ValidTo = $"Expires {(siteExpiration).ToString("d-MMM")}";

            var visitIsInVisitQuestionnaires = visitQuestionnaires.ContainsKey(patientVisitSummary.VisitId);
            var visitStatusNotCompleteMissedPartial = patientVisitSummary.PatientVisitStatus != PatientVisitStatusType.Complete.Name
                && patientVisitSummary.PatientVisitStatus != PatientVisitStatusType.Missed.Name
                && patientVisitSummary.PatientVisitStatus != PatientVisitStatusType.Partial.Name;

            patientVisitSummary.Caregivers =await _patientVisitSummaryQueryHandler
                .ReadCareGiversForPatientVisitSummary(new PatientVisitSummaryQueryParameters(patientId));
            
            patientVisitSummary.ShowTabletCaregiverWebBackup = 
                visitIsInVisitQuestionnaires &&
                visitStatusNotCompleteMissedPartial &&
                patientVisitSummary.Caregivers.Any() &&
                HasRemainingQuestionnairesForVisit(
                    patientVisitSummary.PatientId,
                    siteId,
                    patientVisitSummary.DiaryEntries,
                    visitQuestionnaires[patientVisitSummary.VisitId],
                    caregiverQuestionnaireTypes);

            patientVisitSummary.ShowTabletPatientWebBackup = 
                visitIsInVisitQuestionnaires &&
                visitStatusNotCompleteMissedPartial &&
                HasRemainingQuestionnairesForVisit(
                    patientVisitSummary.PatientId,
                    siteId,
                    patientVisitSummary.DiaryEntries,
                    visitQuestionnaires[patientVisitSummary.VisitId],
                    new List<int> { QuestionnaireType.Patient.Id });

            patientVisitSummary.PatientWebBackUpEmailModel = (patientVisitSummary.ShowTabletPatientWebBackup)
                ? CreateWebBackupEmailModel(patientId, siteId, patientVisitSummary.VisitId, WebBackupType.TabletPatient)
                : default;

            patientVisitSummary.CaregiverWebBackUpEmailModel = (patientVisitSummary.ShowTabletCaregiverWebBackup)
                ? CreateWebBackupEmailModel(patientId, siteId, patientVisitSummary.VisitId, WebBackupType.TabletCaregiver)
                : default;
        }

        private WebBackupEmailModel CreateWebBackupEmailModel(Guid patientId, Guid siteId, Guid visitId, WebBackupType webBackupType)
        {
            return new WebBackupEmailModel
            {
                Id = Guid.NewGuid(),
                EmailContentId = EmailTypes.SubjectHandheldWebBackup,
                WebBackupJwtModel = new WebBackupJwtModel
                {
                    PatientId = patientId,
                    SiteId = siteId,
                    VisitId = visitId,
                    WebBackupType = webBackupType
                }
            };
        }

        private async Task<PatientVisitDto> CreatePatientVisitToDropDown(PatientVisit pv, List<DiaryEntryDto> DiaryEntries,
           bool IncludeAnswers, string CultureCode, Dictionary<Guid, string> visits)
        {
            var visit = visits.FirstOrDefault(x => x.Key == pv.VisitId);

            var obj = new PatientVisitDto()
            {
                Id = pv.Id,
                SyncVersion = pv.SyncVersion,
                VisitReasonId = pv?.VisitReasonId,
                ProjectedDate = pv.ProjectedDate,
                SystemDate = pv?.SystemDate != null ? (DateTimeOffset)pv.SystemDate : DateTimeOffset.MinValue,
                OutsideVisitWindow = pv.OutsideVisitWindow,
                UnscheduledVisitOrder = pv?.UnscheduledVisitOrder,
                Notes = pv?.Notes,
                IRTPatientVisitStatusTypeId = pv.IRTPatientVisitStatusTypeId,
                PatientId = pv.PatientId,
                VisitId = pv.VisitId,
                PatientVisitStatusTypeId = pv.PatientVisitStatusTypeId,
                VisitDate = pv.VisitDate,
                ActivationDate = pv.ActivationDate,
                PatientNumber = pv?.Patient?.PatientNumber,
                VisitName = visit.Value
            };

            if (obj.PatientVisitStatus == null)
            {
                var patientVisitStatuses = PatientVisitStatusType.GetAll<PatientVisitStatusType>();
                obj.PatientVisitStatus = patientVisitStatuses
                    .FirstOrDefault(p => p.Id == obj.PatientVisitStatusTypeId)?.Name;
            }

            return obj;
        }

        private async Task<PatientVisitDto> CreatePatientVisitDto(PatientVisit pv, List<DiaryEntryDto> DiaryEntries,
            bool IncludeAnswers, string CultureCode, Dictionary<Guid, string> visits)
        {
            var obj = await CreatePatientVisitToDropDown(pv, DiaryEntries, IncludeAnswers, CultureCode, visits);

            if (DiaryEntries.Any())
            {
                obj.DiaryEntries = DiaryEntries.Where(e => e.VisitId == pv.VisitId).ToList();
            }
            else
            {
                var entriesByVisit = await _diaryEntryRepository
                     .GetAllPatientDiaryEntriesByVisit(pv.PatientId, pv.VisitId, IncludeAnswers, null, CultureCode);

                obj.DiaryEntries = entriesByVisit.ToList();
            }

            return obj;
        }

        private async Task<List<VisitModel>> GetAllScheduledVisits()
        {            
            var visits = await _visitService.GetAll();
            return visits.Where(v => v.IsScheduled).OrderBy(v => v.VisitOrder).ToList();
        }

        private DateTimeOffset GetProjectedVisitDate(Patient patient, VisitModel visit, List<VisitModel> visits,
            List<PatientVisit> patientVisits)
        {
            var defaultVisitId = visits.FirstOrDefault().Id;
            var anchorVisit = visits.FirstOrDefault(v => v.Id == visit.VisitAnchor);
            var visitId = anchorVisit != null ? anchorVisit.Id : defaultVisitId;
            var patientVisit = patientVisits.FirstOrDefault(pv => pv.PatientId == patient.Id && pv.VisitId == visitId);
            var visitDate = new DateTimeOffset();

            if (patientVisit != null)
            {
                visitDate = patientVisit.VisitDate ?? patientVisit.ProjectedDate;
            }
            else
            {
                visitDate = patient.EnrolledDate;
            }

            visitDate = visitDate.AddDays(visit.DaysExpected);

            //subtract time of day from visitDate instead of using visitDate.Date in order to preserve the site timezone
            //  DateTimeOffset.Date causes an implicit cast to the local timezone of the server
            visitDate = visitDate.AddTicks(-visitDate.TimeOfDay.Ticks);

            return visitDate;
        }

        private async Task SetPatientVisitHardStops(List<PatientVisitSummary> patientVisits, bool ignoreVisitOrder, string siteTimeZone)
        {
            foreach (var pv in patientVisits)
            {
                pv.PatientVisitHardStop = new PatientVisitHardStop();
                pv.VisitInWindow =
                    IseCOAVisitInWindow(pv.VisitStop_HSN, pv.WindowBefore, pv.WindowAfter, pv.ProjectedDate, siteTimeZone);
                pv.PatientVisitHardStop.HardStop = pv.VisitStop_HSN;
                var visitIsAvailable = pv.IsNextScheduled || !pv.IsScheduled;
                var visitIsComplete =
                    pv.PatientVisitStatusTypeId == PatientVisitStatusType.Complete.Id ||
                    pv.PatientVisitStatusTypeId == PatientVisitStatusType.Missed.Id ||
                    pv.PatientVisitStatusTypeId == PatientVisitStatusType.Partial.Id;

                if (!visitIsComplete)
                {
                    if (pv.AlwaysAvailable || (pv.VisitInWindow && (ignoreVisitOrder || visitIsAvailable)))
                    {
                        pv.ShowActivateVisit = true;
                    }
                    else if (!pv.VisitInWindow)
                    {
                        if (pv.PatientVisitHardStop.HardStop == "H")
                        {
                            pv.PatientVisitHardStop.ShowOk = true;
                            pv.PatientVisitHardStop.HardStopMessage =
                                await _translationService.GetByKey("ContinueCompletingVisitHardStop");
                        }

                        else if (pv.PatientVisitHardStop.HardStop == "S")
                        {
                            pv.PatientVisitHardStop.ShowYesNo = true;
                            pv.PatientVisitHardStop.HardStopMessage = visitIsAvailable
                                ? await _translationService.GetByKey("ContinueCompletingVisitSoftStop")
                                : await _translationService.GetByKey("ContinueCompletingVisitSoftStopWithWarning");
                        }
                    }
                    else if (pv.VisitInWindow && !visitIsAvailable)
                    {
                        if (pv.PatientVisitHardStop.HardStop == "H")
                        {
                            pv.PatientVisitHardStop.ShowOk = true;
                            pv.PatientVisitHardStop.HardStopMessage =
                                await _translationService.GetByKey("ContinueCompletingVisitHardStopWithWarning");
                        }

                        else if (pv.PatientVisitHardStop.HardStop == "S" ||
                            pv.PatientVisitHardStop.HardStop == "N")
                        {
                            pv.PatientVisitHardStop.ShowYesNo = true;
                            pv.PatientVisitHardStop.HardStopMessage =
                                await _translationService.GetByKey("ContinueCompletingVisit");
                        }
                    }
                }
            }
        }

        private bool HasRemainingQuestionnairesForVisit(
            Guid patientId,
            Guid siteId,
            List<DiaryEntryDto> diaryEntries,
            IEnumerable<QuestionnaireModel> questionnaires,
            List<int> questionnaireTypeIds)
        {
            var visitQuestionnaireCount = 0;
            var completedCount = 0;

            foreach (var q in questionnaires.Where(q => questionnaireTypeIds.Contains(q.QuestionnaireTaker.QuestionnaireTypeId)))
            {
                completedCount += diaryEntries.Count(d => d.QuestionnaireId == q.Id);

                if (BusinessRuleIsValid(q.EnableVisitQuestionnaireId, q.EnableBusinessRuleTrueFalseIndicator, patientId,
                        siteId) &&
                    BusinessRuleIsValid(q.VisibleBusinessRuleId, q.VisibleBusinessRuleTrueFalseIndicator, patientId,
                        siteId))
                {
                    visitQuestionnaireCount++;
                }
            }

            return completedCount < visitQuestionnaireCount;
        }

        private bool BusinessRuleIsValid(Guid? businessRuleId, bool? trueFalseIndicator, Guid patientId, Guid siteId)
        {
            var isValid = businessRuleId == null;

            if (businessRuleId.HasValue)
            {
                isValid = _ruleService
                    .ExecuteBusinessRule(businessRuleId.Value, patientId, siteId, trueFalseIndicator)
                    .ExecutionResult;
            }

            return isValid;
        }
    }
}