using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.Shared.Helpers.Data;
using YPrime.StudyPortal.Attributes;
using YPrime.StudyPortal.BaseClasses;
using YPrime.StudyPortal.Models;

namespace YPrime.StudyPortal.Controllers
{
    public class CorrectionController : ControllerWithConfirmations
    {
        private const string ViewAllDcfTabSystemActionName = "CanViewDCFList";
        private const string MergeSubjectsViewName = "MergePatient";
        private const string DateTimePickerSettingFormat = "MM/dd/yyyy";

        private readonly ICorrectionRepository _CorrectionRepository;
        private readonly IPatientRepository _PatientRepository;
        private readonly IDiaryEntryRepository _DiaryEntryRepository;
        private readonly ISiteRepository _SiteRepository;
        private readonly IPatientAttributeRepository _PatientAttributeRepository;
        private readonly IPatientVisitRepository _PatientVisitRepository;
        private readonly IVisitService _visitService;
        private readonly ITranslationService _translationService;
        private readonly ICorrectionTypeService _correctionTypeService;
        private readonly ICorrectionWorkflowService _correctionWorkflowService;
        private readonly IStudySettingService _studySettingService;
        private readonly ICountryService _countryService;

        public CorrectionController(
            ICorrectionRepository CorrectionRepository,
            IPatientRepository PatientRepository,
            IDiaryEntryRepository DiaryEntryRepository,
            IPatientAttributeRepository PatientAttributeRepository,
            ISiteRepository SiteRepository,
            IPatientVisitRepository PatientVisitRepository,
            IVisitService visitService,
            ITranslationService translationService,
            ICorrectionTypeService correctionTypeService,
            ICorrectionWorkflowService correctionWorkflowService,
            IStudySettingService studySettingService,
            ISessionService sessionService,
            ICountryService countryService)
            : base(sessionService)
        {
            _CorrectionRepository = CorrectionRepository;
            _PatientRepository = PatientRepository;
            _DiaryEntryRepository = DiaryEntryRepository;           
            _PatientAttributeRepository = PatientAttributeRepository;
            _SiteRepository = SiteRepository;
            _PatientVisitRepository = PatientVisitRepository;
            _visitService = visitService;
            _translationService = translationService;
            _correctionTypeService = correctionTypeService;
            _correctionWorkflowService = correctionWorkflowService;
            _studySettingService = studySettingService;
            _countryService = countryService;
    }

        [FunctionAuthorization("CanViewDataCorrections", "Can View Datacorrections", true)]
        public ActionResult Index()
        {
            ViewBag.CanViewAllDcfsTab = User
                ?.Roles
                .Any(r => r.SystemActions.Any(sa => sa.Name == ViewAllDcfTabSystemActionName)) ?? false;

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetCorrectionGridData(string correctionCode)
        {
            List<CorrectionWorkflow> dcfs;
            switch (correctionCode)
            {
                case "C":
                case "S":
                    dcfs = await _CorrectionRepository.GetCorrectionListForUserComplete(User.Id,
                        CurrentSiteUserCultureCode);
                    break;
                case "P":
                    dcfs = await _CorrectionRepository.GetCorrectionListForUserPending(User.Id,
                        CurrentSiteUserCultureCode);
                    break;
                default:
                    dcfs = await _CorrectionRepository.GetCorrectionListForUser(User.Id,
                        CurrentSiteUserCultureCode);
                    break;
            }

            var correctionTypes = await _correctionTypeService.GetAll();
            var htmlHelper =
                new HtmlHelper(
                    new ViewContext(ControllerContext, new WebFormView(ControllerContext, "fakeView"),
                        new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());
            var results = dcfs.Select(x => new
            {
                EditLinkHTML = htmlHelper.PrimeActionLink(x.Correction.DataCorrectionNumber.ToString().PadLeft(4, '0'),
                    "Workflow", "CorrectionWorkflow", new {x.Id}, false, true).ToHtmlString(),
                ApproverGroup = x.Correction.NoApprovalNeeded
                    ? htmlHelper.TranslationLabel("[N/A]", CurrentSiteUserCultureCode)
                    : x.ApproverGroupName,
                CorrectionType = correctionTypes.FirstOrDefault(c => c.Id == x.Correction.CorrectionTypeId)?.Name,
                CorrectionStatus = htmlHelper.TranslationLabel(x.Correction.CorrectionStatus.TranslationKey,
                    CurrentSiteUserCultureCode),
                x.Correction.Patient.PatientNumber,
                Site = x.Correction.Site.Name,
                x.Correction.StartedDate
            });

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [FunctionAuthorization("CanCreateDataCorrections", "Can Create Datacorrections", true)]
        public async Task<ActionResult> Create(Guid? PatientId)
        {
            var correction = await _CorrectionRepository.CreateCorrectionObject(User.Id, PatientId);
            SetCorrectionsViewbag(null);
            ModelState.Clear();
            return View(correction);
        }

        [HttpPost]
        [ValidateInput(false)]
        [FunctionAuthorization("CanCreateDataCorrections", "Can Create Datacorrections", true)]
        public async Task<ActionResult> Create(Correction correction, bool? edit = false)
        {
            // The back button returns here, with the edit flag set to true
            // We do not need to check for existing patient if returning to edit mode
            if (!(bool) edit)
            {
                await UpdateCorrectionForPatientFields(correction);
            }
            
            ActionResult result = View(correction);
            if (correction.CorrectionTypeId != Guid.Empty && ModelState.IsValid && !(bool) edit)
            {
                await CheckCorrectionProperties(correction);

                correction.ConfigurationId = await _CorrectionRepository.GetCorrectionConfigurationId(correction);

                var isValidCorrection = await _CorrectionRepository
                    .ValidateCorrection(correction, ModelState, CurrentSiteUserCultureCode);

                if (isValidCorrection)
                {
                    if (correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id)
                    {
                        await _CorrectionRepository.SortDiaryEntryCorrectionApprovals(correction);
                    }

                    // Don't modify user input until persistence
                    await _CorrectionRepository.FormatControlValues(correction, CorrectionStateInternal.Create);

                    result = View("~/Views/Correction/Submit.cshtml", correction);
                }
                else
                {
                    SetCorrectionsViewbag(correction.SiteId);
                    result = View(correction);
                }
            }
            else
            {
                if (correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id)
                {
                    await _CorrectionRepository.SortDiaryEntryCorrectionApprovals(correction);
                }
                // Returning to create screen, using Javascript to reload variables
                SetCorrectionsViewbag(correction.SiteId);
                // Transfer what user entered into view bag variables.
                ViewBag.SubjectVal = correction.CorrectionApprovalDatas[0].NewDisplayValue;

                // not all dcf types have a status
                if (correction.CorrectionApprovalDatas.Count > 1)
                {
                    ViewBag.StatusVal = correction.CorrectionApprovalDatas[1].NewDisplayValue;
                }
            }

            // make sure the UseMetricMeasurements is set correctly
            if (correction.SiteId != null && correction.SiteId != Guid.Empty)
            {
                var site = await _SiteRepository.GetSite(correction.SiteId.Value);
                var country = await _countryService.Get(site.CountryId);
                correction.UseMetricMeasurements = country.UseMetric;
            }

            return result;
        }

        public async Task<JsonResult> DoesPatientExist(string siteID, string subjectNo)
        {
            var isFound = false;
            Guid siteGuid;

            if (Guid.TryParse(siteID, out siteGuid))
            {
                var patNo = await _PatientRepository.GeneratePatientNumber(siteGuid, subjectNo);
                isFound = _PatientRepository.AnyPatients(patNo);
            }

            return Json(isFound, JsonRequestBehavior.AllowGet);
        }

        private async Task UpdateCorrectionForPatientFields(Correction correction)
        {
            // First, if patient number was entered, we need to make two adjustments
            // Zero fill the display value, and compute the new subject number
            // Look up the id code for the patient status (if changed), and set the value instead of the text
            var patUpdate = correction.CorrectionApprovalDatas.FirstOrDefault(p =>
                p.TableName.ToLower() == "patient" && p.ColumnName.ToLower() == "patientnumber");
            if (patUpdate != null && !string.IsNullOrEmpty(patUpdate.NewDisplayValue))
            {
                // Make tweaks
                int pLen = await _studySettingService.GetIntValue("PatientNumberLength");
                patUpdate.NewDisplayValue = patUpdate.NewDisplayValue.PadLeft(pLen, '0');

                patUpdate.NewDataPoint = await _PatientRepository.GeneratePatientNumber(
                    (Guid) correction.SiteId, 
                    patUpdate.NewDisplayValue);

                patUpdate.OldDataPoint = correction.PatientNumber;
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetCorrectionView(Correction correction)
        {
            correction.ConfigurationId = Guid.NewGuid();
            string url = ExecuteAsyncActionSynchronously(() => CreateCorrectionRedirect(correction));
            return PartialView(url, correction);
        }

        private async Task CheckCorrectionProperties(Correction correction)
        {
            await CheckCorrectionType(correction);

            if (correction.PatientId != null && correction.Patient == null)
            {
                correction.Patient = await _PatientRepository.GetPatientEntity((Guid) correction.PatientId);
            }

            if (correction.SiteId != null && correction.SiteId == Guid.Empty && correction.Site == null)
            {
                var siteId = ((Guid) correction.SiteId != null && correction.SiteId != Guid.Empty)
                    ? (Guid) correction.SiteId
                    : correction.Patient.SiteId;

                correction.Site = _SiteRepository.GetSiteEntity(siteId);
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> Submit(Correction correction)
        {
            if (ModelState.IsValid)
            {
                if (correction.SiteId != null && correction.SiteId == Guid.Empty)
                {
                    var patient = _PatientRepository.GetPatientForEditAsync((Guid) correction.PatientId).Result;
                    correction.SiteId = patient.SiteId;
                }

                correction.CorrectionHistory.Add(new CorrectionHistory()
                    {
                        CorrectionActionId = CorrectionActionEnum.Pending,
                        UserName = User.UserName,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        DateTimeStamp = DateTime.UtcNow.ToString("dd'-'MMM'-'yyyy hh':'mm tt 'UTC'")
                    }
                );

                await _CorrectionRepository.SaveInitialCorrection(correction);

                var successTranslation = await _translationService.GetByKey("Success");
                var correctionAddedTranslation = await _translationService.GetByKey("CorrectionAddedSuccessfully");

                SetPopUpMessageOnLoad(
                    successTranslation,
                    correctionAddedTranslation);

                // Format Correction correctly
                await _CorrectionRepository.FormatControlValues(correction, CorrectionStateInternal.Verify);

                var table = RenderRazorViewToString("~/Views/Correction/CorrectionDataDisplay.cshtml", correction);            

                var data = new Dictionary<string, string>();
                data.Add("VisitQuestionnaireConfirmation", table);

                var workflow = await _correctionWorkflowService.Get(correction.CorrectionTypeId);
                var emailType = workflow.NoApprovalNeeded ? EmailTypes.DataCorrectionApproved : EmailTypes.DataCorrectionPendingApproval;

                return DoConfirmation(data, emailType);
            }

            return View(correction);
        }

        public async Task<JsonResult> GetPatientsBySite(Guid siteId)
        {
            ModelState.Clear();

            var patients = await _CorrectionRepository.GetAvailableCorrectionPatients(siteId, User);

            var result = new AjaxResult
            {
                JsonData = JSONHelper.SerializeObject(patients),
                Success = true
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCountryDetailsFromSiteId(Guid siteId)
        {
            var site = await _SiteRepository.GetSite(siteId);
            var country = await _countryService.Get(site.CountryId);

            var result = new AjaxResult
            {
                JsonData = JSONHelper.SerializeObject(country),
                Success = true
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> CreateCorrectionRedirect(Correction correction)
        {
            string result;

            await CheckCorrectionType(correction);
            SetRemoveStatusViewBag();
            ModelState.Clear();

            result = GetCorrectionTypeUrl(correction.CorrectionTypeId);

            if (correction.SiteId != null)
            {
                SetTodayDate(
                    correction.SiteId,
                    correction.PatientId);
            }

            return result;
        }

        private void SetRemoveStatusViewBag()
        {
            ViewBag.RemovedStatusTypeId = 99;
            ViewBag.RemovedStatusTypeDisplay = "Removed";
        }

        private async Task CheckCorrectionType(Correction correction)
        {
            var correctionType = correction.CorrectionWorkflowSettings;
            if (correctionType == null)
            {
                correctionType = await _correctionWorkflowService.Get(correction.CorrectionTypeId);
                correction.CorrectionWorkflowSettings = correctionType;
            }
        }

        private string GetCorrectionTypeUrl(Guid correctionTypeId)
        {
            if (correctionTypeId == Guid.Empty)
            {
                return "Create";
            }

            var correctionType = CorrectionType.GetAll<CorrectionType>().FirstOrDefault(c => c.Id == correctionTypeId);

            var viewName = correctionType?.Id == CorrectionType.MergeSubjects.Id
                ? MergeSubjectsViewName
                : correctionType?.Name;

            var result = $"~/Views/Correction/{viewName}.cshtml";
            return result;
        }

        /*----------------------------------------
         * Correction Type HELPERS
         * -------------------------------------*/

        /*****************************************************
         * Change PatientVisit
         * **************************************************/
        public async Task<JsonResult> GetPatientVisits(Guid PatientId)
        {
            var result = new AjaxResult();

            var visitStatusTypesToInclude = new List<int>()
            {
                PatientVisitStatusType.NotStarted.Id,
                PatientVisitStatusType.InProgress.Id,
                PatientVisitStatusType.Partial.Id,
                PatientVisitStatusType.Complete.Id,
                PatientVisitStatusType.Missed.Id,
            };

            var patientVisits =  await _PatientVisitRepository.GetPatientVisits(PatientId, visitStatusTypesToInclude, false, CurrentSiteUserCultureCode);
            
            patientVisits.ForEach(pv =>
            {
                pv.VisitDateDisplay = pv.VisitDate?.ToString(YPrimeSession.Instance.GlobalDateFormat);
                pv.VisitActivationDateDisplay = pv.ActivationDate?.ToString(YPrimeSession.Instance.GlobalDateFormat);
            });

            var visitStatusTypes = PatientVisitStatusType.GetAll<PatientVisitStatusType>().Where(t => visitStatusTypesToInclude.Contains(t.Id));

            var data = new
            {
                PatientVisits = patientVisits,
                VisitStatusTypes = visitStatusTypes
            };

            result.JsonData = JSONHelper.SerializeObject<object>(data);
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPatientVisit(Guid PatientVisitId)
        {
            //note: this is used for the edit questionnaire information DCF
            var result = new AjaxResult();
            var patientVisit = await _PatientVisitRepository.GetPatientVisit(PatientVisitId, CurrentSiteUserCultureCode);
            patientVisit.VisitDateDisplay =
                patientVisit.VisitDate?.ToString(YPrimeSession.Instance.GlobalDateFormat);
            patientVisit.VisitActivationDateDisplay =
                patientVisit.ActivationDate?.ToString(YPrimeSession.Instance.GlobalDateFormat);
            result.JsonData = JSONHelper.SerializeObject(patientVisit);
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /*****************************************************
        * Change Questionnaire Info
        * **************************************************/
        public async Task<JsonResult> GetDiaryEntries(Guid PatientId)
        {
            var result = new AjaxResult();

            var userBlinded = false;

            var diaryEntries = await _DiaryEntryRepository
                .GetDiaryEntriesInflated(null, false, userBlinded, PatientId);

            diaryEntries = diaryEntries
                .OrderBy(de => de.QuestionnaireName)
                .ThenBy(de => de.DiaryDate)
                .ToList();

            diaryEntries.ForEach(de =>
            {
                de.DiaryEntryDateDisplay = de.DiaryDate.ToString(YPrimeSession.Instance.GlobalDateFormat);
            });

            var visits = await _visitService.GetAll();

            var data = new
            {
                DiaryEntries = diaryEntries,
                Visits = visits
            };

            result.JsonData = JSONHelper.SerializeObject<object>(data);
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDiaryEntry(Guid DiaryEntryId)
        {
            //note: this is used for the edit questionnaire information DCF
            var result = new AjaxResult();
            var diaryEntry = await _DiaryEntryRepository.GetDiaryEntry(DiaryEntryId, true, CurrentSiteUserCultureCode);
            diaryEntry.DiaryEntryDateDisplay = diaryEntry.DiaryDate.ToString(YPrimeSession.Instance.GlobalDateFormat);
            result.JsonData = JSONHelper.SerializeObject(diaryEntry);
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /*****************************************************
          * Change Questionnaire Responses
          * **************************************************/
        public async Task<ActionResult> GetDiaryEntryView(Guid DiaryEntryId, Guid CorrectionId)
        {
            //note: this is used for the edit questionnaire responses DCF
            var diaryEntry = await _DiaryEntryRepository.GetDiaryEntry(DiaryEntryId, true, CurrentSiteUserCultureCode);
            diaryEntry.DiaryEntryDateDisplay = diaryEntry.DiaryDate.ToString(YPrimeSession.Instance.GlobalDateFormat);
            ViewBag.CorrectionId = CorrectionId;

            var patient = await _PatientRepository.GetPatientEntity(diaryEntry.PatientId);
            var country = await _countryService.Get(patient.Site.CountryId);
            ViewBag.UseMetric = country.UseMetric;

            SetDateTimeFormatForPicker(country, diaryEntry.QuestionAnswers.Select(qa => qa.Question).ToList());

            SetTodayDate(
                patient.Site.Id,
                patient.Id);

            return PartialView(diaryEntry);
        }

        /*****************************************************
          * Create Diary Entry 
          * **************************************************/
        [HttpGet]
        public async Task<JsonResult> GetQuestionnaires(Guid patientId)
        {
            var result = new AjaxResult();
            var data = await _CorrectionRepository.GetQuestionnaireNameDictionary(patientId);

            result.JsonData = JSONHelper.SerializeObject(data);
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPaperDiaryEntryView(Guid questionnaireId, Guid correctionId, Guid patientId)
        {
            var questionnaire = await _CorrectionRepository.GetPaperDiaryEntryQuestionnaire(
                questionnaireId,
                patientId);
            
            var visits = await _visitService.GetAll();         

            ViewBag.CorrectionId = correctionId;
            ViewBag.Visits = visits.Where(v => v.IsScheduled);

            var patient = await _PatientRepository.GetPatientEntity(patientId);
            var country = await _countryService.Get(patient.Site.CountryId);
            ViewBag.UseMetric = country.UseMetric;

            SetTodayDate(
                patient.SiteId,
                patient.Id);

            SetDateTimeFormatForPicker(country, questionnaire.Questions);
            
            return PartialView(questionnaire);
        }

        public async Task<ActionResult> GetMergePatientView(
            Guid patientId, 
            Guid correctionId)
        {
            var currentCultureCode = CurrentPatientCultureCode;
            var dateformat = YPrimeSession.Instance.GlobalDateFormat;

            var merges = await _PatientRepository.GetDuplicatePatientsById(
                patientId, 
                currentCultureCode,
                dateformat);

            ViewBag.PatientNumber = (await _PatientRepository.GetPatients(new List<Guid>() { patientId })).FirstOrDefault().PatientNumber;
            ViewBag.CorrectionId = correctionId;
            ViewBag.MergeLabel = await _translationService.GetByKey("Merge");
            SetRemoveStatusViewBag();
            return PartialView(merges);
        }

        public async Task<JsonResult> GetPatientInformation(Guid PatientId)
        {
            var result = new AjaxResult();
            var patient = await _PatientRepository.GetPatient(PatientId, CurrentSiteUserCultureCode);
            result.JsonData = JSONHelper.SerializeObject(patient);
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void SetCorrectionsViewbag(Guid? siteId)
        {
            ViewBag.CorrectionTypes = ExecuteAsyncActionSynchronously(() => _correctionTypeService
                .GetAll())
                .Where(ct => ct.HasConfiguration)
                .ToList();

            ViewBag.Sites = User.Sites.OrderBy(s => s.Name);

            ViewBag.Patients = siteId.HasValue
                ? ExecuteAsyncActionSynchronously(() => _PatientRepository.GetAllPatients(new List<Guid> { (Guid)siteId }))
                : new List<PatientDto>();
        }

        public async Task<ActionResult> GetPatientAttributes(Guid patientId, Guid correctionId,
            IList<CorrectionApprovalData> correctionApprovalData)
        {
            correctionApprovalData = correctionApprovalData ?? new List<CorrectionApprovalData>();
            var patient = await _PatientRepository.GetPatient(patientId, CurrentSiteUserCultureCode);
            ViewBag.SiteId = patient.SiteId;
            ViewBag.IsDCF = true;
            SetTodayDate(
                patient.SiteId,
                patient.Id);

            var results = await _PatientAttributeRepository.GetPatientAttributesForCorrection(patientId, correctionId,
                CurrentSiteUserCultureCode, correctionApprovalData);
            
            return PartialView(results.ToArray());
        }

        private void SetDateTimeFormatForPicker(CountryModel country, List<QuestionModel> questions)
        {
            var timeFormat = $" {country.GetTimeFormat()}";
            questions.ForEach(q =>
            {
                q.QuestionSettings.DateTimeFormat = q.QuestionSettings.DateTimeFormat?.ToUpper();
                if (q.InputFieldTypeId == InputFieldType.DateTime.Id ||
                    q.InputFieldTypeId == InputFieldType.Time.Id)
                {
                    q.QuestionSettings.DateTimeFormat = (q.QuestionSettings.DateTimeFormat + timeFormat).Trim();
                }
            });
        }

        private void SetTodayDate(
            Guid? siteId, 
            Guid? patientId)
        {
            var localDate = _CorrectionRepository.GetLocalDateForCorrection(siteId, patientId);
            ViewBag.TodayMaxDateTime = localDate;
            ViewBag.Today = localDate.ToString(DateTimePickerSettingFormat, CultureInfo.InvariantCulture);
        }
    }
}