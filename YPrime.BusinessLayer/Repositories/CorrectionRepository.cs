using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.Repositories
{
    public class CorrectionRepository : BaseRepository, ICorrectionRepository
    {
        private const string ModelAssemblyName = "YPrime.Data.Study.Models";
        private Dictionary<string, List<string>> RequiredVisitDateTransitions = new Dictionary<string, List<string>>()
        {
            { PatientVisitStatusType.Missed.Id.ToString() , new List<string>(){ PatientVisitStatusType.Complete.Id.ToString(), PatientVisitStatusType.InProgress.Id.ToString(), PatientVisitStatusType.Partial.Id.ToString() }},
            { PatientVisitStatusType.NotStarted.Id.ToString() , new List<string> { PatientVisitStatusType.Complete.Id.ToString(), PatientVisitStatusType.InProgress.Id.ToString(), PatientVisitStatusType.Partial.Id.ToString()}},
            { PatientVisitStatusType.InProgress.Id.ToString() , new List<string> { PatientVisitStatusType.Complete.Id.ToString(), PatientVisitStatusType.Partial.Id.ToString()}}
        };

        private HashSet<CorrectionStateInternal> NonEditCorrectionStates = new HashSet<CorrectionStateInternal> { CorrectionStateInternal.Verify, CorrectionStateInternal.Confirm };

        private readonly ITranslationService _translationService;
        private readonly IPatientAttributeRepository _patientAttributeRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ICorrectionTypeService _correctionTypeService;
        private readonly ICorrectionWorkflowService _correctionWorkflowService;
        private readonly IApproverGroupService _approvalGroupService;
        private readonly ISessionService _sessionService;
        private readonly ICountryService _countryService;

        public CorrectionRepository(
            IStudyDbContext db,
            ITranslationService translationService,
            IPatientRepository patientRepository,
            IPatientAttributeRepository patientAttributeRepository,
            ISiteRepository siteRepository,
            IQuestionnaireService questionnaireService,
            IPatientStatusService patientStatusService,
            ICorrectionTypeService correctionTypeService,
            ICorrectionWorkflowService correctionWorkflowService,
            IApproverGroupService approvalGroupService,
            ISessionService sessionService,
            ICountryService countryService)
            : base(db)
        {
            _translationService = translationService;
            _patientAttributeRepository = patientAttributeRepository;
            _siteRepository = siteRepository;
            _patientRepository = patientRepository;
            _questionnaireService = questionnaireService;
            _patientStatusService = patientStatusService;
            _correctionTypeService = correctionTypeService;
            _correctionWorkflowService = correctionWorkflowService;
            _approvalGroupService = approvalGroupService;
            _sessionService = sessionService;
            _countryService = countryService;
        }

        public async Task UpdateCorrectionWorkFlow(Guid workflowId, Guid correctionActionId, Guid studyUserId, string discussionComment, CorrectionHistory correctionHistoryToAdd)
        {
            var workFlow = _db.CorrectionWorkflows
                .Include(cw => cw.Correction)
                .Include(cw => cw.Correction.CorrectionApprovalDatas)
                .Single(cw => cw.Id == workflowId);

            workFlow.CorrectionActionId = correctionActionId;
            workFlow.WorkflowChangedDate = DateTimeOffset.Now;
            workFlow.StudyUserId = studyUserId;

            if (!workFlow.Correction.NoApprovalNeeded)
            {
                workFlow.Correction.CorrectionDiscussions.Add(
                    new CorrectionDiscussion()
                    {
                        Discussion = discussionComment,
                        DiscussionDate = DateTimeOffset.Now,
                        CorrectionActionId = correctionActionId,
                        StudyUserId = studyUserId
                    }
                );
            }

            var maxWorkFlowOrder = workFlow.Correction.CorrectionWorkflows.Max(cw => cw.WorkflowOrder);

            if (workFlow.WorkflowOrder == maxWorkFlowOrder && correctionActionId != CorrectionActionEnum.NeedsMoreInformation && correctionActionId != CorrectionActionEnum.Rejected)
            {
                var remainingWorkflows = workFlow.Correction.CorrectionWorkflows
                    .Where(cw => cw.Id != workflowId && cw.WorkflowOrder == maxWorkFlowOrder)
                    .ToList();

                var allCompleted = remainingWorkflows
                    .All(cw => cw.CorrectionActionId == CorrectionActionEnum.Approved);

                var allRemainingPending = remainingWorkflows
                    .All(cw => cw.CorrectionActionId == CorrectionActionEnum.Pending);

                if (correctionActionId == CorrectionActionEnum.Approved && allCompleted)
                {
                    workFlow.Correction.CorrectionStatusId = CorrectionStatusEnum.Completed;
                    await FinalCorrectionApproval(workFlow.Correction);
                }
                else if (correctionActionId == CorrectionActionEnum.Approved &&
                         workFlow.Correction.CorrectionStatusId == CorrectionStatusEnum.Pending &&
                         allRemainingPending)
                {
                    workFlow.Correction.CorrectionStatusId = CorrectionStatusEnum.InProgress;
                }
            }
            else
            {
                workFlow.Correction.CurrentWorkflowOrder = GetNextWorkFlowOrder(workFlow);

                if (correctionActionId == CorrectionActionEnum.Rejected)
                {
                    workFlow.Correction.CorrectionStatusId = CorrectionStatusEnum.Rejected;
                    workFlow.Correction.CompletedDate = DateTime.Now;
                }
                else
                {
                    workFlow.Correction.CorrectionStatusId = correctionActionId == CorrectionActionEnum.NeedsMoreInformation
                        ? CorrectionStatusEnum.NeedsMoreInformation
                        : CorrectionStatusEnum.InProgress;
                }
            }

            correctionHistoryToAdd.CorrectionActionId = correctionActionId;
            workFlow.Correction.CorrectionHistory.Add(correctionHistoryToAdd);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public async Task<Guid> GetCorrectionConfigurationId(Correction correction)
        {
            Guid configId;

            if (correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id && correction.PatientId.HasValue)
            {
                configId = await _patientRepository.GetAssignedConfiguration(correction.PatientId.Value);
            }
            else if (correction.CorrectionTypeId == CorrectionType.ChangeQuestionnaireResponses.Id && correction.DiaryEntryId.HasValue)
            {
                var diaryEntry = await _db.DiaryEntries.FindAsync(correction.DiaryEntryId.Value);
                configId = diaryEntry.ConfigurationId;
            }
            else
            {
                configId = _sessionService.UserConfigurationId;
            }

            return configId;
        }

        private int GetNextWorkFlowOrder(CorrectionWorkflow workFlow)
        {
            int Rtn = workFlow.WorkflowOrder;

            // Need to find out if there are any more workflows with the same order that haven't been acted on yet.
            var pendingWorkFlowsWithSameOrder = workFlow.Correction.CorrectionWorkflows.Where(wf => (wf.CorrectionActionId == CorrectionActionEnum.Pending || wf.CorrectionActionId == CorrectionActionEnum.NeedsMoreInformation) && wf.WorkflowOrder == workFlow.WorkflowOrder);

            if (!pendingWorkFlowsWithSameOrder.Any())
            {
                Rtn = workFlow.WorkflowOrder + 1;
            }

            return Rtn;
        }

        public async Task SaveInitialCorrection(Correction correction)
        {
            correction.CorrectionStatus = null;
            correction.CorrectionWorkflowSettings = null;
            correction.StartedDate = DateTimeOffset.Now;
            correction.CurrentWorkflowOrder = 1; // Always starts at one.

            var questions = correction.QuestionnaireId.HasValue
                ? await _questionnaireService.GetQuestions(correction.QuestionnaireId.Value)
                : new List<QuestionModel>();

            correction.CorrectionApprovalDatas = await CleanCorrectionApprovalData(
                correction,
                questions);

            _db.Corrections.Add(correction);

            var workflow = await _correctionWorkflowService.Get(correction.CorrectionTypeId);
            correction.NoApprovalNeeded = workflow.NoApprovalNeeded;

            // Need to save a correction as well as project out the approval workflows.
            if (correction.NoApprovalNeeded)
            {
                correction.CorrectionWorkflows.Add
                    (
                        new CorrectionWorkflow()
                        {
                            ApproverGroupId = null,
                            WorkflowOrder = correction.CurrentWorkflowOrder.Value,
                            CorrectionActionId = CorrectionActionEnum.Pending
                        }
                    );

            }
            else
            {
                foreach (var config in workflow.Configurations)
                {
                    correction.CorrectionWorkflows.Add
                    (
                        new CorrectionWorkflow()
                        {
                            ApproverGroupId = config.ApproverGroupId,
                            WorkflowOrder = config.WorkflowOrder,
                            CorrectionActionId = CorrectionActionEnum.Pending
                        }
                    );
                }
            }

            if (correction.DiaryEntryId != null)
            {
                _db.LoadPropertyFromDb(correction, c => c.DiaryEntry);
                correction.DiaryEntry.DiaryStatusId = DiaryStatus.Queried.Id;
            }

            _db.Corrections.Add(correction);

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            if (correction.NoApprovalNeeded)
            {
                await UpdateCorrectionWorkFlow(correction.CorrectionWorkflows.First().Id, CorrectionActionEnum.Approved, YPrimeSession.Instance.CurrentUser.Id, "", correction.CorrectionHistory.FirstOrDefault());
            }
        }

        public async Task<bool> ValidateCorrection(Correction correction, ModelStateDictionary modelState, string cultureCode)
        {
            var isValid = true;

            switch (correction.CorrectionTypeId)
            {
                case var id when id == CorrectionType.ChangeSubjectInfo.Id:
                    isValid = await ValidateCorrectionPatientAttributes(correction, modelState, cultureCode) && await ValidatePatientNumberLength(correction, modelState);
                    break;
                case var id when id == CorrectionType.ChangeQuestionnaireResponses.Id:
                    isValid = await ValidateCorrectionQuestionResponses(correction, modelState, TranslationConstants.DefaultCultureCode);
                    break;
                case var id when id == CorrectionType.PaperDiaryEntry.Id:
                    isValid = await ValidateCorrectionPaperDiaryEntry(correction, modelState);
                    break;
                case var id when id == CorrectionType.ChangePatientVisit.Id:
                    isValid = await ValidateCorrectionPatientVisit(correction, modelState);
                    break;
            }

            return isValid;
        }

        private async Task<bool> ValidateCorrectionPaperDiaryEntry(Correction correction, ModelStateDictionary modelState)
        {
            var modelErrors = new Dictionary<string, string>();

            var diaryDateCorrectionData = correction.CorrectionApprovalDatas
                .FirstOrDefault(cad => cad.TableName == nameof(DiaryEntry) && cad.ColumnName == nameof(DiaryEntry.DiaryDate));

            if (string.IsNullOrWhiteSpace(diaryDateCorrectionData?.NewDataPoint))
            {
                var errorSuffix = (await _translationService.GetByKey(TranslationConstants.RequiredFieldErrorSuffix));
                modelErrors.Add($"{correction.Id}-InvalidDiaryDate", $"{diaryDateCorrectionData?.Description} {errorSuffix}");
            }

            var questionnaireIdCorrectionData = correction.CorrectionApprovalDatas
                .FirstOrDefault(cad => cad.TableName == nameof(DiaryEntry) && cad.ColumnName == nameof(DiaryEntry.QuestionnaireId));

            var questionnaireId = Guid.Empty;

            if (string.IsNullOrWhiteSpace(questionnaireIdCorrectionData?.NewDataPoint) || !Guid.TryParse(questionnaireIdCorrectionData?.NewDataPoint, out questionnaireId))
            {
                var errorSuffix = (await _translationService.GetByKey(TranslationConstants.RequiredFieldErrorSuffix));
                modelErrors.Add($"{correction.Id}-InvalidQuestionnaireId", $"{questionnaireIdCorrectionData?.Description} {errorSuffix}");
            }

            var matchingQuestionnaire = await _questionnaireService.GetInflatedQuestionnaire(
                questionnaireId,
                configurationId: correction.ConfigurationId);

            matchingQuestionnaire.Questions = matchingQuestionnaire
                .Pages
                .SelectMany(p => p.Questions)
                .ToList();

            foreach (var correctionData in correction.CorrectionApprovalDatas)
            {
                var matchingQuestion = matchingQuestionnaire.Questions.FirstOrDefault(q => q.Id == correctionData.QuestionId);

                if (!string.IsNullOrWhiteSpace(correctionData.NewDataPoint) && (matchingQuestion?.IsNumericValueQuestionType() ?? false))
                {
                    var isInt = !correctionData.NewDataPoint.Contains(".");

                    await ValidateNumericValue(correctionData, matchingQuestion, modelErrors, correction.UseMetricMeasurements, isInt);
                }
            }

            foreach (var modelError in modelErrors)
            {
                modelState.AddModelError(modelError.Key, modelError.Value);
            }

            var result = modelErrors.Count == 0;

            return result;
        }

        private async Task<bool> ValidateCorrectionQuestionResponses(Correction correction, ModelStateDictionary modelState, string cultureCode)
        {
            var dataToValidate = correction
                .CorrectionApprovalDatas
                .Where(cad => !string.IsNullOrWhiteSpace(cad.NewDataPoint));

            var modelErrors = await ValidateQuestionValues(
                correction,
                dataToValidate);

            foreach (var modelError in modelErrors)
            {
                modelState.AddModelError(modelError.Key, modelError.Value);
            }

            var result = modelErrors.Count == 0;

            return result;
        }

        private async Task<Dictionary<string, string>> ValidateQuestionValues(
            Correction correction,
            IEnumerable<CorrectionApprovalData> dataToValidate)
        {
            var modelErrors = new Dictionary<string, string>();

            var questions = correction.QuestionnaireId.HasValue
                ? await _questionnaireService.GetQuestions(correction.QuestionnaireId.Value, correction.ConfigurationId)
                : new List<QuestionModel>();

            foreach (var correctionData in dataToValidate)
            {
                var matchingQuestion = questions.FirstOrDefault(q => q.Id == correctionData.QuestionId);
                if (matchingQuestion?.QuestionType == InputFieldType.NumberSpinner.Id ||
                     matchingQuestion?.QuestionType == InputFieldType.NRS.Id ||
                     matchingQuestion?.QuestionType == InputFieldType.VAS.Id ||
                     matchingQuestion?.QuestionType == InputFieldType.EQ5D5L.Id)
                {                    
                    await ValidateNumericValue(correctionData, matchingQuestion, modelErrors, correction.UseMetricMeasurements);
                }
                else if (matchingQuestion?.QuestionType == InputFieldType.TemperatureSpinner.Id)
                {
                    await ValidateTemperatureValue(correctionData, matchingQuestion, modelErrors, correction.UseMetricMeasurements);
                }             
            }

            return modelErrors;
        }

        private async Task ValidateTemperatureValue(CorrectionApprovalData correctionData, QuestionModel matchingQuestion, Dictionary<string, string> modelErrors, bool useMetric)
        {
            float? minValue = null;
            float? maxValue = null;
            string minMsg = null;
            string maxMsg = null;

            var decimalPlaces = matchingQuestion.GetDecimalValue();

            if (float.TryParse(matchingQuestion.GetMinValue(useMetric), out var parsedMinValue))
            {
                minValue = parsedMinValue;
                minMsg = ConcatDataAndSuffix(minValue.ToFormattedString(decimalPlaces), GetTemperatureDisplaySuffix(useMetric), true);
            }

            if (float.TryParse(matchingQuestion.GetMaxValue(useMetric), out var parsedMaxValue))
            {
                maxValue = parsedMaxValue;
                maxMsg = ConcatDataAndSuffix(maxValue.ToFormattedString(decimalPlaces), GetTemperatureDisplaySuffix(useMetric), true);
            }

            var isValidNumericValue = correctionData.NewDataPoint.HasValidNumericValue(decimalPlaces);
            if (!isValidNumericValue)
            {
                await AddMinMaxModelError(TranslationConstants.InvalidNumericValueErrorSuffix, matchingQuestion, modelErrors, minMsg, maxMsg);
            }

            if (isValidNumericValue && !correctionData.NewDataPoint.ValidateValueInRange(minValue, maxValue))
            {
                await AddMinMaxModelError(TranslationConstants.InvalidRangeValueErrorSuffix, matchingQuestion, modelErrors, minMsg, maxMsg);
            }
        }

        private async Task AddMinMaxModelError(string errorKey, QuestionModel matchingQuestion, Dictionary<string, string> modelErrors, string minMsg, string maxMsg)
        {
            var errorTranslation = await _translationService.GetByKey(errorKey) ?? string.Empty;

            modelErrors.Add($"{matchingQuestion.Id}-{errorKey}", $"{matchingQuestion.QuestionText} {string.Format(errorTranslation, minMsg, maxMsg)}");
        }

        private async Task<bool> ValidateCorrectionPatientVisit(Correction correction, ModelStateDictionary modelState)
        {
            var result = true;

            var visitDate = correction
                .CorrectionApprovalDatas
                .SingleOrDefault(cad => !cad.RemoveItem && cad.ColumnName == CorrectionConstants.PatientVisitVisitDate);

            var visitDateStr = string.Empty;

            if (visitDate != null)
            {
                visitDateStr = string.IsNullOrEmpty(visitDate.NewDisplayValue)
                    ? visitDate.OldDisplayValue
                    : visitDate.NewDisplayValue;
            }

            var activationDate = correction
                .CorrectionApprovalDatas
                .SingleOrDefault(cad => !cad.RemoveItem && cad.ColumnName == CorrectionConstants.PatientVisitActivationDate);
            var activationDateStr = string.IsNullOrEmpty(activationDate?.NewDisplayValue) ? activationDate?.OldDisplayValue : activationDate?.NewDisplayValue;

            var visitStatus = correction
                .CorrectionApprovalDatas
                .SingleOrDefault(cad => cad.ColumnName == nameof(PatientVisit.PatientVisitStatusTypeId));

            if (visitStatus != null && visitDate != null &&
                !string.IsNullOrEmpty(visitStatus.NewDataPoint) &&
                RequiredVisitDateTransitions.TryGetValue(visitStatus.OldDataPoint, out var requiredVisitDateStatuses) &&
                requiredVisitDateStatuses.Contains(visitStatus.NewDataPoint) &&
                string.IsNullOrEmpty(visitDate?.NewDisplayValue))
            {
                modelState.AddModelError(TranslationConstants.RequiredFieldErrorSuffix, $"{await _translationService.GetByKey(visitDate.TranslationKey)} {await _translationService.GetByKey(TranslationConstants.RequiredFieldErrorSuffix)}");
                result = false;
            }

            if (DateTime.TryParse(visitDateStr, out var parsedVisitDate) &&
                DateTime.TryParse(activationDateStr, out var parsedActivationDate) &&
                parsedActivationDate > parsedVisitDate)
            {
                modelState.AddModelError(TranslationConstants.DCFInvalidActivationDate, $"{await _translationService.GetByKey(TranslationConstants.DCFInvalidActivationDate)}");
                result = false;
            }

            return result;
        }

        private async Task ValidateNumericValue(
            CorrectionApprovalData correctionData,
            QuestionModel question,
            Dictionary<string, string> modelErrors,
            bool useMetric,
            bool isInt = true)
        {
            decimal? minValue = null;
            decimal? maxValue = null;
            string minMsg = null;
            string maxMsg = null;
            var decimalPlaces = question.GetDecimalValue();

            if (decimal.TryParse(question.GetMinValue(useMetric), out var parsedMinValue))
            {
                minValue = isInt ? (int)parsedMinValue : parsedMinValue;
                minMsg = minValue.ToFormattedString(decimalPlaces, question.IsEQ5D5LQuestionType());
            }

            if (decimal.TryParse(question.GetMaxValue(useMetric), out var parsedMaxValue))
            {
                maxValue = isInt ? (int)parsedMaxValue : parsedMaxValue;
                maxMsg = maxValue.ToFormattedString(decimalPlaces, question.IsEQ5D5LQuestionType());
            }

            var isValidNumericValue = correctionData.NewDataPoint.HasValidNumericValue(decimalPlaces);
            if (!isValidNumericValue)
            {
                await AddMinMaxModelError(TranslationConstants.InvalidNumericValueErrorSuffix, question, modelErrors, minMsg, maxMsg);
            }

            if (isValidNumericValue && !correctionData.NewDataPoint.ValidateValueInRange(minValue, maxValue))
            {
                await AddMinMaxModelError(TranslationConstants.InvalidRangeValueErrorSuffix, question, modelErrors, minMsg, maxMsg);
            }
        }

        private async Task<bool> ValidateCorrectionPatientAttributes(Correction correction, ModelStateDictionary modelState, string cultureCode)
        {
            var result = false;

            if (correction.PatientId.HasValue)
            {
                var attributes = await _patientAttributeRepository.GetPatientAttributes(correction.PatientId.Value, cultureCode);

                var attributesToValidate = new List<PatientAttributeDto>();

                foreach (var attribute in attributes)
                {
                    var matchingCorrection = new CorrectionApprovalData();
                    if (attribute.NewAttributeData)
                    {
                        matchingCorrection = correction
                        .CorrectionApprovalDatas.FirstOrDefault(c => c.CorrectionApprovalDataAdditionals
                        .Any(cad => cad.ColumnValue == attribute.PatientAttributeConfigurationDetailId.ToString()));
                    }
                    else
                    {
                        matchingCorrection = correction
                        .CorrectionApprovalDatas
                        .FirstOrDefault(cad => cad.TableName == nameof(PatientAttribute) && cad.RowId == attribute.Id);
                    }

                    if (matchingCorrection != null && !string.IsNullOrWhiteSpace(matchingCorrection.NewDataPoint))
                    {
                        attribute.AttributeValue = matchingCorrection.NewDataPoint;

                        attributesToValidate.Add(attribute);
                    }
                }

                var siteLocalTime = correction.Patient?.SiteId != null
                    ? _siteRepository.GetSiteLocalTime(correction.Patient.SiteId)
                    : DateTimeOffset.Now;

                result = await _patientRepository
                    .ValidatePatientAttributesFromDetail(attributesToValidate, modelState, siteLocalTime, correction.PatientId.Value, true, true);
            }

            return result;
        }

        public async Task<bool> ValidatePatientNumberLength(Correction correction, ModelStateDictionary modelState)
        {
            var result = true;

            var subjectNumberCorrection = correction.CorrectionApprovalDatas
                .FirstOrDefault(cad => cad.TableName == nameof(Patient) && cad.ColumnName == nameof(Patient.PatientNumber));

            if (subjectNumberCorrection != null && !string.IsNullOrEmpty(subjectNumberCorrection.NewDataPoint))
            {
                var patientNumberValue = await _patientAttributeRepository.ExtractPatientNumber(subjectNumberCorrection.NewDataPoint);

                result = await _patientRepository.ValidatePatientNumber(patientNumberValue, modelState);
            }

            return result;
        }

        private async Task<List<CorrectionApprovalData>> CleanCorrectionApprovalData(
            Correction correction,
            List<QuestionModel> questions)
        {
            var result = new List<CorrectionApprovalData>();

            //Note: AllowDelete = true means Multiple Choice question
            //CRUD scenarios are:
            //AllowDelete = true/false and newDataPoint has a value (STANDARD UPDATE)
            //AllowDelete = true and rowid is a GUID and new DataPoint is null (REMOVE ITEM)
            //check the RemoveItem attribute
            var multipleChoiceInputs = InputFieldType.Where<InputFieldType>(i => i.MultipleChoice).Select(i => i.Id);
            //filter out non answers
            correction.CorrectionApprovalDatas
                .Where(cad =>
                    (!string.IsNullOrWhiteSpace(cad.NewDataPoint)) //standard update
                    ||
                    (cad.RemoveItem) //standard remove
                    ||
                    (
                        correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id &&
                        !string.IsNullOrWhiteSpace(cad.NewDataPoint)
                    )
                    ||
                    // Remove unselected items for multiple choice questions
                    (cad.AllowDelete && cad.RowId != Guid.Empty && cad.NewDataPoint == null
                        && (questions.Find(x => x.Id == cad.QuestionId) != null &&
                        multipleChoiceInputs.Contains(questions.Find(x => x.Id == cad.QuestionId).QuestionType))
                        && correction.CorrectionApprovalDatas.Any(inner => inner.QuestionId == cad.QuestionId &&
                                                                !string.IsNullOrWhiteSpace(inner.NewDataPoint)))
                ).ToList()
                .ForEach(cad =>
                {
                    if (cad.RemoveItem)
                    {
                        cad.NewDataPoint = null;
                        cad.NewDisplayValue = null;
                    }
                    result.Add(cad);
                });

            //Set NewDataPoint as NewDisplayValue
            if (correction.CorrectionTypeId == CorrectionType.ChangeSubjectInfo.Id)
            {
                foreach (var correctionApprovalData in result.Where(c => string.IsNullOrEmpty(c.NewDataPoint)))
                {
                    correctionApprovalData.NewDataPoint = correctionApprovalData.NewDisplayValue;
                };
            }

            await FormatControlValues(correction, CorrectionStateInternal.Save);
            return result;
        }

        public async Task FormatControlValues(
            Correction correction,
            CorrectionStateInternal correctionState)
        {
            if (correction?.CorrectionApprovalDatas != null)
            {
                if (correction.CorrectionTypeId == CorrectionType.ChangeSubjectInfo.Id)
                {
                    await SetDisplayValuesForAttributes(correction, correctionState);
                }
                else
                {
                    await SetDisplayValuesForQuestionnaire(correction, correctionState);
                }
            }
        }

        private async Task SetDisplayValuesForAttributes(Correction correction, CorrectionStateInternal correctionState)
        {
            var suffixByDetailId = (await _patientAttributeRepository.GetPatientAttributes(correction.PatientId.Value, TranslationConstants.DefaultCultureCode))
                .ToDictionary(key => key.PatientAttributeConfigurationDetailId, value => value.SubjectInformation?.Suffix);

            foreach (var data in correction.CorrectionApprovalDatas)
            {
                var formattedSuffix = string.Empty;
                var detailId = data.CorrectionApprovalDataAdditionals.Any()
                    ? Guid.Parse(data.CorrectionApprovalDataAdditionals[0].ColumnValue)
                    : Guid.Empty;

                var isDefaultAttribute = detailId == Guid.Empty;
                
                // if we are saving DCF, no suffix, else include a space in the front of the suffix here, so that if there is none it wont add extra space                        
                var shouldSkipSuffix = correctionState == CorrectionStateInternal.Save || isDefaultAttribute;
                if (!shouldSkipSuffix)
                {
                    var currentSuffix = suffixByDetailId[detailId];

                    formattedSuffix = string.IsNullOrEmpty(currentSuffix)
                        ? string.Empty
                        : $" {currentSuffix}";
                }

                var oldDisplayValue = string.IsNullOrWhiteSpace(data.OldDisplayValue)
                    ? data.OldDisplayValue
                    : ConcatDataAndSuffix(data.OldDisplayValue, formattedSuffix);

                SetDataDisplayValues(data, data.NewDataPoint, ConcatDataAndSuffix(data.NewDisplayValue, formattedSuffix), data.OldDataPoint, oldDisplayValue);
            }
        }

        private async Task SetDisplayValuesForQuestionnaire(Correction correction, CorrectionStateInternal correctionState)
        {
            var questions = correction.QuestionnaireId.HasValue
                 ? await _questionnaireService.GetQuestions(correction.QuestionnaireId.Value)
                 : new List<QuestionModel>();

            var temperatureQuestions = questions
                ?.Where(x => x.IsTemperatureQuestionType()).Select(i => i.Id)
                .ToList() ?? new List<Guid>();

            foreach (var data in correction.CorrectionApprovalDatas)
            {
                Answer matchingAnswer = null;

                if (correction.CorrectionTypeId == CorrectionType.ChangeQuestionnaireResponses.Id &&
                    data.TableName == nameof(Answer))
                {
                    matchingAnswer = _db.Answers.FirstOrDefault(a => a.Id == data.RowId);
                }

                var isTemperatureData =
                    (data.QuestionId != null && temperatureQuestions.Contains(data.QuestionId.Value)) ||
                    (data.TableName == nameof(Answer) && temperatureQuestions.Contains(data.RowId)) ||
                    (matchingAnswer != null && temperatureQuestions.Contains(matchingAnswer.QuestionId));

                if (isTemperatureData)
                {
                    var matchingQuestionId = data.QuestionId.HasValue
                         ? data.QuestionId.Value
                         : matchingAnswer?.QuestionId ?? data.RowId;

                    var matchingQuestion = questions.Single(q => q.Id == matchingQuestionId);

                    SetTemperatureDataPoints(
                        matchingQuestion,
                        data,
                        correction.UseMetricMeasurements,
                        correctionState);
                }
                else
                {
                    // Paper DCF we will out the description with question text, so link back to that for the question. Else null
                    var idLookUp = data.QuestionId ?? questions?.FirstOrDefault(x => x.QuestionText == data.Description)?.Id;
                    var question = questions?.FirstOrDefault(x => x.Id == idLookUp);
                    if (question != null)
                    {
                        // if we are saving DCF, no suffix, else include a space in the front of the suffix here, so that if there is none it wont add extra space
                        string suffix = correctionState == CorrectionStateInternal.Save || string.IsNullOrWhiteSpace(question.QuestionSettings?.Suffix)
                            ? string.Empty
                            : $" {await _translationService.GetByKey(question.QuestionSettings?.Suffix)}";

                        string oldDisplayValue = string.IsNullOrWhiteSpace(data.OldDisplayValue) ? data.OldDisplayValue : ConcatDataAndSuffix(data.OldDisplayValue, suffix);

                        SetDataDisplayValues(data, data.NewDataPoint, ConcatDataAndSuffix(data.NewDisplayValue, suffix), data.OldDataPoint, oldDisplayValue);
                    }
                }
            }
        }

        public void SetTemperatureDataPoints(
          QuestionModel matchingQuestion,
          CorrectionApprovalData data,
          bool useMetric,
          CorrectionStateInternal correctionState)
        {
            if (NonEditCorrectionStates.Contains(correctionState))
            {
                // Only modify values when adjusted by user or formatted for save
                return;
            }

            if (data == null)
            {
                return;
            }

            var rawNewDataPoint = data.NewDataPoint?
                .TrimEnd(Temperature.Fahrenheit, Temperature.Celsius);
            var rawOldDataPoint = data.OldDataPoint?
                .TrimEnd(Temperature.Fahrenheit, Temperature.Celsius);

            if (correctionState == CorrectionStateInternal.Save)
            {
                data.NewDataPoint = GetConvertedTemperatureDataForSave(matchingQuestion, rawNewDataPoint, useMetric);
                data.OldDataPoint = GetConvertedTemperatureDataForSave(matchingQuestion, rawOldDataPoint, useMetric);
            }

            var displaySuffix = GetTemperatureDisplaySuffix(useMetric);

            data.NewDisplayValue = ConcatDataAndSuffix(rawNewDataPoint, displaySuffix);
            data.OldDisplayValue = ConcatDataAndSuffix(rawOldDataPoint, displaySuffix);
        }

        private void SetDataDisplayValues(CorrectionApprovalData data, string newDataPointValue, string newDisplayValue,
            string oldDataPointValue, string oldDisplayValue)
        {
            data.NewDataPoint = newDataPointValue;
            data.NewDisplayValue = newDisplayValue;
            data.OldDataPoint = oldDataPointValue;
            data.OldDisplayValue = oldDisplayValue;
        }

        private string ConcatDataAndSuffix(string data, string suffix, bool addSpace = false)
        {
            if (string.IsNullOrEmpty(data))
            {
                suffix = string.Empty;
            }

            return addSpace ? $"{data} {suffix}" : $"{data}{suffix}";
        }

        private string GetConvertedTemperatureDataForSave(
              QuestionModel question,
              string data,
              bool useMetric)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            string temperatureString;
            char attachedSuffix;
            var isLegacy = question.TemperatureMinMax == null;

            var convertedValue = float.Parse(data, NumberStyles.Any, CultureInfo.InvariantCulture);

            var shouldPreserveUnits = question.TemperatureMinMax?.PreserveUnits == true;
            if (shouldPreserveUnits)
            {
                attachedSuffix = useMetric ? Temperature.Celsius : Temperature.Fahrenheit;
                temperatureString = convertedValue.ToString(TemperatureControlConstants.DisplayFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                convertedValue = useMetric
                    ? convertedValue
                    : TemperatureConversionHelper.ToCelsius(convertedValue);

                attachedSuffix = Temperature.Celsius;
                temperatureString = convertedValue.ToString(TemperatureControlConstants.NonPreserveSaveFormat, CultureInfo.InvariantCulture);
            }

            return isLegacy
                ? temperatureString
                : $"{temperatureString}{attachedSuffix}";
        }

        private string GetTemperatureDisplaySuffix(bool useMetric)
        {
            return useMetric
                ? Temperature.DegreesCelsius
                : Temperature.DegreesFahrenheit;
        }


        public async Task FinalCorrectionApproval(Correction correction)
        {
            var correctionTypes = await _correctionTypeService
                .GetAll();

            var selectedCorrectionType = correctionTypes
                .First(ct => ct.Id == correction.CorrectionTypeId);

            //retain the correction id for the audit logs
            _db.CorrectionId = correction.Id;

            var patient = _db.Patients.FirstOrDefault(p => p.Id == correction.PatientId);
            int oldPatientStatusTypeId = patient?.PatientStatusTypeId ?? -1;

            correction.CompletedDate = DateTime.Now;

            if (selectedCorrectionType.Id == CorrectionType.PaperDiaryEntry.Id)
            {
                var diaryEntry = correction.ToDiaryEntry();
                _db.DiaryEntries.Add(diaryEntry);
                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }
            else if (selectedCorrectionType.Id == CorrectionType.ChangeSubjectInfo.Id)
            {
                var existingPatientAttributes = new List<PatientAttribute>();

                if (correction.PatientId.HasValue)
                {
                    existingPatientAttributes = _db
                        .PatientAttributes
                        .Where(pa => pa.PatientId == correction.PatientId.Value)
                        .ToList();
                }

                var patientAttributes = correction.ToPatientAttribute(existingPatientAttributes);

                foreach (var attribute in patientAttributes)
                {
                    _db.PatientAttributes.AddOrUpdate(attribute);
                }


                if (patient != null)
                {
                    correction.UpdatePatientData(patient);
                    _db.Patients.AddOrUpdate(patient);
                    await _patientRepository.UpdateNotificationScheduleForPatient(patient.Id, patient.PatientStatusTypeId, oldPatientStatusTypeId);
                }

                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }
            else
            {
                foreach (var correctionApprovalData in correction.CorrectionApprovalDatas)
                {
                    var TypeName = $"{ModelAssemblyName}.{correctionApprovalData.TableName}, {ModelAssemblyName}";
                    var EntityType = Type.GetType(TypeName, true, true);

                    var correctionCRUDType = GetCorrectionCRUDType(correctionApprovalData);
                    var entity = GetCorrectionApprovalDataEntity(correctionApprovalData, EntityType);

                    switch (correctionCRUDType)
                    {
                        case CorrectionCRUDTypes.Insert:
                            InsertRecord(correctionApprovalData, EntityType);
                            break;
                        case CorrectionCRUDTypes.Update:
                            {
                                UpdateProperty(correctionApprovalData.ColumnName, correctionApprovalData.NewDataPoint, entity);
                                // Try to cancel the notification schedule
                                if (correctionApprovalData.ColumnName == nameof(Patient.PatientStatusTypeId))
                                {
                                    if (int.TryParse(correctionApprovalData.NewDataPoint, out var patientStatusTypeId))
                                    {
                                        await _patientRepository.UpdateNotificationScheduleForPatient(correction.PatientId ?? Guid.Empty, patientStatusTypeId, oldPatientStatusTypeId);
                                    }
                                }
                                break;
                            }
                        case CorrectionCRUDTypes.Delete:
                            {
                                if (EntityType == typeof(PatientVisit))
                                {
                                    DeletePatientVisitRecordItems(
                                        (PatientVisit)entity,
                                        correctionApprovalData,
                                        EntityType);
                                }
                                else
                                {
                                    DeleteRecord(correctionApprovalData, EntityType);
                                }

                                break;
                            }
                    }

                    _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

                    _db.LoadPropertyFromDb(correction, c => c.DiaryEntry);

                    if (correction.DiaryEntry != null)
                    {
                        correction.DiaryEntry.DiaryStatusId = DiaryStatus.Modified.Id;
                    }
                }
            }
        }

        private void DeletePatientVisitRecordItems(
            PatientVisit patientVisit,
            CorrectionApprovalData correctionApprovalData,
            Type entityType)
        {
            switch (correctionApprovalData.ColumnName)
            {
                // existing plumbing for object.SetPropertyValue does not work with setting null on these types
                case nameof(PatientVisit.ActivationDate):
                    patientVisit.ActivationDate = null;
                    break;
                case nameof(PatientVisit.VisitDate):
                    patientVisit.VisitDate = null;
                    break;
                default:
                    DeleteRecord(correctionApprovalData, entityType);
                    break;
            }
        }

        private CorrectionCRUDTypes GetCorrectionCRUDType(CorrectionApprovalData dataPoint)
        {
            //CRUD scenarios are:
            //AllowDelete = false and newDataPoint has a value (STANDARD UPDATE)
            //AllowDelete = true and newDataPoint IS A GUID AND rowid is GUID (NO UPDATE keep as is)
            //AllowDelete = true and rowid is empty GUID and new DataPoint has a value (ADD ITEM)
            //AllowDelete = true and rowid is a GUID and new DataPoint is null (REMOVE ITEM)

            var result = CorrectionCRUDTypes.Update;

            //check for insert
            if ((dataPoint.ColumnName == "FreeTextAnswer" && dataPoint.OldDataPoint == null && dataPoint.NewDataPoint != null) ||
                dataPoint.RowId == Guid.Empty && Guid.TryParse(dataPoint.NewDataPoint, out var parsedGuid))
            {
                result = CorrectionCRUDTypes.Insert;
            }
            else
            {
                //check for remove
                if (dataPoint.RowId != Guid.Empty && dataPoint.NewDataPoint == null)
                {
                    result = CorrectionCRUDTypes.Delete;
                }
            }

            return result;
        }

        private void InsertRecord(CorrectionApprovalData correctionApprovalData, Type entityType)
        {
            var entityObject = Activator.CreateInstance(entityType);
            UpdateProperty(nameof(ModelBase.Id), Guid.NewGuid().ToString(), entityObject);
            //loop the additional properties associated to the approval data?
            foreach (var additional in correctionApprovalData.CorrectionApprovalDataAdditionals)
            {
                if (!additional.IgnorePropertyUpdate)
                {
                    UpdateProperty(additional.ColumnName, additional.ColumnValue, entityObject);
                }
            }
            UpdateProperty(correctionApprovalData.ColumnName, correctionApprovalData.NewDataPoint, entityObject);
            ((DbContext)_db).Set(entityType).Add(entityObject);
        }

        private void UpdateProperty(string propertyName, string newValue, object entity)
        {
            var propType = entity.GetPropertyType(propertyName);

            if (Nullable.GetUnderlyingType(propType) != null)
            {
                propType = Nullable.GetUnderlyingType(propType);
            }

            switch (propType.Name)
            {
                case nameof(DateTimeOffset):
                    entity.SetPropertyValue(propertyName, DateTimeOffset.Parse(newValue));
                    break;
                case nameof(Guid):
                    entity.SetPropertyValue(propertyName, Guid.Parse(newValue));
                    break;
                default:
                    entity.SetPropertyValue(propertyName, newValue);
                    break;
            }
        }

        private void DeleteRecord(CorrectionApprovalData correctionApprovalData, Type entityType)
        {
            var entityObject = GetCorrectionApprovalDataEntity(correctionApprovalData, entityType);
            foreach (var additional in correctionApprovalData.CorrectionApprovalDataAdditionals)
            {
                if (!additional.IgnorePropertyUpdate)
                {
                    UpdateProperty(additional.ColumnName, additional.ColumnValue, entityObject);
                }
            }

            ((DbContext)_db).Set(entityType).Remove(entityObject);
        }

        private object GetCorrectionApprovalDataEntity(CorrectionApprovalData correctionApprovalData, Type entityType)
        {
            if (correctionApprovalData.RowId == Guid.Empty && entityType.Name.ToLower() == nameof(Patient).ToLower())
            {
                return ((((DbContext)_db)
                    .Set(entityType))
                    .Find(correctionApprovalData.Correction.PatientId));
            }
            else
            {
                return ((((DbContext)_db)
                    .Set(entityType))
                    .Find(correctionApprovalData.RowId));
            }
        }

        public Task<List<CorrectionWorkflow>> GetCorrectionListForUser(Guid StudyUserId, string CultureName)
        {
            var workFlowListForUserTask = GetCorrectionListData(StudyUserId, false, false);
            return workFlowListForUserTask;
        }

        public Task<List<CorrectionWorkflow>> GetCorrectionListForUserPending(Guid StudyUserId, string CultureName)
        {
            var workFlowListForUserTask = GetCorrectionListData(StudyUserId, true, false);
            return workFlowListForUserTask;
        }

        public Task<List<CorrectionWorkflow>> GetCorrectionListForUserComplete(Guid StudyUserId, string CultureName)
        {
            var workFlowListForUserTask = GetCorrectionListData(StudyUserId, true, true);
            return workFlowListForUserTask;
        }

        public async Task<List<CorrectionWorkflow>> GetUpcomingWorkflowsForPatient(Guid studyUserId, Guid patientId)
        {
            var workflowsForPatient = new List<CorrectionWorkflow>();
            var approverGroups = (await _approvalGroupService.GetAll())
                ?? Enumerable.Empty<ApproverGroupModel>();

            var studyUser = _db.StudyUsers
                    .Include(u => u.StudyUserRoles)
                    .FirstOrDefault(u => u.Id == studyUserId);
            var hasValidUser = studyUser != null && studyUser.StudyUserRoles != null;

            if (hasValidUser && approverGroups.Any())
            {
                var userRoleIds = studyUser.StudyUserRoles.Select(sur => sur.StudyRoleId);
                var roleApproverGroupIds = approverGroups
                    .Where(a => a.Roles.Any(r => userRoleIds.Contains(r.Id)))
                    .Select(a => a.Id);

                var userSiteIds = studyUser.StudyUserRoles.Select(sur => sur.SiteId);
                var workflows = _db.CorrectionWorkflows
                    .Include(c => c.Correction)
                    .Include(c => c.Correction.Patient)
                    .Include(c => c.Correction.CorrectionStatus)
                    .Where(cw =>
                        cw.Correction.PatientId == patientId
                        && roleApproverGroupIds.Contains((Guid)cw.ApproverGroupId)
                        && (cw.Correction.SiteId == null || userSiteIds.Contains((Guid)cw.Correction.SiteId)))
                    .ToList();

                workflowsForPatient = RemoveDuplicateWorkflowsFromList(workflows);
            }

            return workflowsForPatient;
        }

        private async Task<List<CorrectionWorkflow>> GetCorrectionListData(Guid StudyUserId, bool IncludeAllWorkflows, bool ShowAll)
        {
            var studyUser = _db.StudyUsers
                .Include(u => u.StudyUserRoles)
                .FirstOrDefault(u => u.Id == StudyUserId);

            if (studyUser == null
                || studyUser.StudyUserRoles == null)
            {
                return new List<CorrectionWorkflow>();
            }

            var approverGroups = await _approvalGroupService.GetAll();
            if (approverGroups == null)
            {
                return new List<CorrectionWorkflow>();
            }
     
            var userRoleIds = studyUser.StudyUserRoles.Select(sur => sur.StudyRoleId);
            var roleApproverGroupIds = approverGroups
                .Where(a => a.Roles.Any(r => userRoleIds.Contains(r.Id)))
                .Select(a => a.Id);

            var userSiteIds = studyUser.StudyUserRoles.Select(sur => sur.SiteId);
            var pendingWorkflows = _db.CorrectionWorkflows
                .Include(c => c.Correction)
                .Include(c => c.Correction.Patient)
                .Include(c => c.Correction.CorrectionStatus)
                .Where(cw =>
                    (ShowAll || cw.CorrectionActionId == CorrectionActionEnum.Pending) &&
                    (ShowAll || cw.Correction.CorrectionStatusId != CorrectionStatusEnum.Rejected) &&
                    (IncludeAllWorkflows || roleApproverGroupIds.Contains((Guid)cw.ApproverGroupId)) &&
                    (cw.Correction.SiteId == null || userSiteIds.Contains((Guid)cw.Correction.SiteId)));

            var needInformationWorkflows = _db.CorrectionWorkflows
                .Include(c => c.Correction)
                .Include(c => c.Correction.Patient)
                .Include(c => c.Correction.CorrectionStatus)
                .Where(cw =>
                    !ShowAll &&
                    cw.CorrectionActionId == CorrectionActionEnum.NeedsMoreInformation &&
                    cw.Correction.StartedByUserId == StudyUserId &&
                    (cw.Correction.CorrectionStatus.NeedsMoreInformation || !cw.Correction.CorrectionStatus.Resolved));

            var workflowListForUser = pendingWorkflows.Union(needInformationWorkflows).ToList();

            var workflowSettings = workflowListForUser
                .Select(wf => wf.Correction.CorrectionTypeId)
                .Distinct()
                .ToDictionary(correctionTypeId => correctionTypeId, correctionTypeId => _correctionWorkflowService.Get(correctionTypeId));

            foreach (var wf in workflowListForUser)
            {
                wf.ApproverGroupName = approverGroups.FirstOrDefault(a => a.Id == wf.ApproverGroupId)?.Name;
                wf.Correction.CorrectionWorkflowSettings = await workflowSettings[wf.Correction.CorrectionTypeId];
            }

            workflowListForUser = RemoveDuplicateWorkflowsFromList(workflowListForUser);

            return workflowListForUser;
        }

        public async Task<Correction> CreateCorrectionObject(Guid StudyUserId, Guid? PatientId)
        {
            var result = new Correction()
            {
                StartedByUserId = StudyUserId,
                StartedDate = DateTime.Now,
                CorrectionApprovalDatas = new List<CorrectionApprovalData>(),
                CorrectionStatusId = CorrectionStatusEnum.Pending,
                CurrentWorkflowOrder = 1,
                PatientId = PatientId,
                PatientPreLoaded = PatientId != null
            };

            if (result.PatientPreLoaded)
            {
                var patientStatuses = await _patientStatusService.GetAll();
                var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList(), true);
                patientFilter.ById((Guid)PatientId);

                var patient = patientFilter.Execute(_db.Patients).Single();
                result.PatientNumber = patient.PatientNumber;
                result.SiteId = patient.SiteId;
                result.SiteName = patient.Site.Name;
                var country = await _countryService.Get(patient.Site.CountryId);
                result.UseMetricMeasurements = country.UseMetric;
            }

            return result;
        }

        public async Task<Correction> GetCorrection(Guid Id, string CultureName)
        {
            var correctionFilter = new CorrectionFilter();

            var result = correctionFilter.Execute(_db.Corrections)
                .Include(c => c.Patient)
                .Include(c => c.CorrectionApprovalDatas)
                .Include(c => c.CorrectionDiscussions)
                .Include(c => c.CorrectionDiscussions.Select(cd => cd.StudyUser))
                .Include(c => c.CorrectionWorkflows.Select(cw => cw.CorrectionAction))
                .Single(c => c.CorrectionWorkflows.Any(cw => cw.CorrectionId == Id));

            result.CorrectionWorkflowSettings = await _correctionWorkflowService.Get(result.CorrectionTypeId);

            return result;
        }

        public List<CorrectionAction> GetCorrectionActions()
        {
            var result = _db.CorrectionActions.ToList();
            return result;
        }

        public async Task<CorrectionWorkflow> GetCorrectionWorkflow(
            Guid id,
            List<StudyRoleModel> roles,
            Guid userId,
            string cultureName)
        {         
            var result = _db.CorrectionWorkflows
                .Include(cw => cw.Correction)
                .Single(cw => cw.Id == id);

            foreach (var correctionWorkflow in result.Correction.CorrectionWorkflows)
            {
                if (correctionWorkflow.ApproverGroupId.HasValue)
                {
                    var approverGroup = await _approvalGroupService
                        .Get(correctionWorkflow.ApproverGroupId.Value);

                    correctionWorkflow.ApproverGroupName = approverGroup?.Name;
                }
            }

            result.Correction.SiteName = result.Correction.Site.Name;
            result.Correction.CorrectionWorkflowSettings = await _correctionWorkflowService.Get(result.Correction.CorrectionTypeId);

            result.Correction.AllowEdit = await AllowUserEdit(roles, userId, result);
            CheckForRemoveDataItems(result.Correction.CorrectionApprovalDatas);

            // We need description filled out for Paper DCF's and Subject Information
            if (result.Correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id
                || result.Correction.CorrectionTypeId == CorrectionType.ChangeSubjectInfo.Id)
            {
                foreach (var correctionApprovalData in result.Correction.CorrectionApprovalDatas)
                {
                    correctionApprovalData.Description = correctionApprovalData.TranslationKey;
                }
            }

            if (result.Correction.CorrectionApprovalDatas.Any() && (result.Correction.CorrectionTypeId == CorrectionType.PaperDiaryEntry.Id ||
                result.Correction.CorrectionTypeId == CorrectionType.ChangeQuestionnaireResponses.Id))
            {
                await SortDiaryEntryCorrectionApprovals(result.Correction);
            }

            await SortCorrectionApprovalsByType(result.Correction, result.Correction.CorrectionTypeId);

            // set UseMetricMeasurements
            if (result.Correction.SiteId.HasValue && result.Correction.SiteId != Guid.Empty)
            {
                var site = await _siteRepository.GetSite(result.Correction.SiteId.Value);
                var country = await _countryService.Get(site.CountryId);
                result.Correction.UseMetricMeasurements = country.UseMetric;
            }

            await FormatControlValues(result.Correction, CorrectionStateInternal.Verify);

            // Do it this way to avoid lazy loading the patient table,             
            result.PatientNumber =  _db.Patients.Where(p => p.Id == result.Correction.PatientId).Select(p => p.PatientNumber).FirstOrDefault() ?? string.Empty;
            return result;
        }

        private async Task SortCorrectionApprovalsByType(Correction correction, Guid correctionAction)
        {
            if (correctionAction == CorrectionType.ChangePatientVisit.Id)
            {
                correction.CorrectionApprovalDatas = correction.CorrectionApprovalDatas.OrderBy(d =>
                {
                    int order = 0;
                    switch (d.ColumnName)
                    {
                        case "PatientVisitStatusTypeId":
                            order = 1;
                            break;
                        case "VisitDate":
                            order = 2;
                            break;
                        case "ActivationDate":
                            order = 3;
                            break;
                    }
                    return order;
                }).ToList();
            }
            else if (correctionAction == CorrectionType.ChangeSubjectInfo.Id)
            {
                var patientAttributes = await _patientAttributeRepository.GetPatientAttributesForCorrection(correction.PatientId.Value, correction.Id,
                    StudyUserDto.CultureCode, new List<CorrectionApprovalData>());

                if (patientAttributes != null)
                {
                    var orderDictionary = patientAttributes
                    .ToDictionary(a => a.PatientAttribute.PatientAttributeConfigurationDetailId != Guid.Empty ? a.PatientAttribute.PatientAttributeConfigurationDetailId.ToString() : a.ColumnName,
                                  a => patientAttributes.IndexOf(a));

                    correction.CorrectionApprovalDatas = correction.CorrectionApprovalDatas.OrderBy(d =>
                    {
                        var order = orderDictionary.Count();
                        if (d.CorrectionApprovalDataAdditionals.Any() &&
                        orderDictionary.TryGetValue(d.CorrectionApprovalDataAdditionals.First().ColumnValue, out order))
                        {// configurable attributes
                            return order;
                        }

                        if (orderDictionary.TryGetValue(d.ColumnName, out order))
                        {// hardcoded attribute (Subject Number, Subject Status, Enrolled Date, Language)
                            return order;
                        }

                        return order;
                    }).ToList();
                }
            }
        }

        public async Task SortDiaryEntryCorrectionApprovals(Correction correction)
        {
            const string QuestionIdKey = "QuestionId";

            if (!correction.QuestionnaireId.HasValue)
            {
                return;
            }

            var questionnaire = await _questionnaireService.GetInflatedQuestionnaire(
                correction.QuestionnaireId.Value,
                configurationId: correction.ConfigurationId);

            correction.CorrectionApprovalDatas.Sort((left, right) =>
            {
                // Diary entries should be at the top of the list
                if (left.TableName == nameof(DiaryEntry) || right.TableName == nameof(DiaryEntry))
                {
                    if (left.TableName != nameof(DiaryEntry))
                    {
                        return 1;
                    }

                    if (right.TableName != nameof(DiaryEntry))
                    {
                        return -1;
                    }

                    if (left.ColumnName == nameof(DiaryEntry.VisitId))
                    {
                        return 1;
                    }

                    if (right.ColumnName == nameof(DiaryEntry.QuestionnaireId))
                    {
                        return 1;
                    }

                    return -1;
                }

                // Anything that is not an answer or diary entry should be at the bottom of the list
                if (left.TableName != nameof(Answer))
                {
                    return 1;
                }

                if (right.TableName != nameof(Answer))
                {
                    return -1;
                }

                var leftQuestionId = Guid.Parse(left.CorrectionApprovalDataAdditionals.FirstOrDefault(a => a.ColumnName == QuestionIdKey).ColumnValue);
                var rightQuestionId = Guid.Parse(right.CorrectionApprovalDataAdditionals.FirstOrDefault(a => a.ColumnName == QuestionIdKey).ColumnValue);

                var leftPage = questionnaire
                    .Pages
                    .FirstOrDefault(p => p.Questions.Any(q => q.Id == leftQuestionId));

                if (leftPage == null)
                {
                    return 1;
                }

                var rightPage = questionnaire
                    .Pages
                    .FirstOrDefault(p => p.Questions.Any(q => q.Id == rightQuestionId));

                if (rightPage == null)
                {
                    return -1;
                }

                var leftQuestion = leftPage?.Questions.FirstOrDefault(q => q.Id == leftQuestionId);
                var rightQuestion = rightPage?.Questions.FirstOrDefault(q => q.Id == rightQuestionId);

                if (leftQuestion == null)
                {
                    return 1;
                }

                var leftMultipleChoice = leftQuestion.GetInputFieldType().MultipleChoice;
                var rightMultipleChoice = rightQuestion != null && rightQuestion.GetInputFieldType().MultipleChoice;

                if (leftMultipleChoice && rightMultipleChoice && leftQuestion.Id == rightQuestion.Id)
                {
                    if (left.NewDataPoint == null || right.NewDataPoint == null)
                    {
                        return 1;
                    }

                    var leftChoice = questionnaire.Pages
                        .SelectMany(p => p.Questions)
                        .SelectMany(q => q.Choices)
                        .FirstOrDefault(c => c.Id.ToString() == left.NewDataPoint);

                    var rightChoice = questionnaire.Pages
                        .SelectMany(p => p.Questions)
                        .SelectMany(q => q.Choices)
                        .FirstOrDefault(c => c.Id.ToString() == right.NewDataPoint);

                    if (leftChoice.Sequence > rightChoice.Sequence)
                    {
                        return 1;
                    }
                }

                if (leftQuestion.Sequence == 0 && leftPage.Number > rightPage.Number)
                {
                    return 1;
                }

                if (rightQuestion == null)
                {
                    return -1;
                }

                if (rightQuestion.Sequence == 0 && leftPage.Number < rightPage.Number)
                {
                    return -1;
                }

                if (leftQuestion.Sequence > rightQuestion.Sequence && leftPage.Number == rightPage.Number)
                {
                    return 1;
                }

                if (leftQuestion.Sequence < rightQuestion.Sequence && leftPage.Number == rightPage.Number)
                {
                    return -1;
                }

                return leftPage.Number > rightPage.Number ?
                1 : -1;
            });
        }

        private List<CorrectionWorkflow> RemoveDuplicateWorkflowsFromList(List<CorrectionWorkflow> workflows)
        {
            var idsToRemoveFromList = new List<Guid>();
            workflows
                .GroupBy(c => c.CorrectionId)
                .ToList()
                .ForEach(groupedCorrections =>
                {
                    if (groupedCorrections.Count() > 1)
                    {
                        var orderedCorrections = groupedCorrections
                            .OrderBy(gc => gc.WorkflowOrder)
                            .ToList();

                        var firstCorrectionStatusId = orderedCorrections.First().Correction.CorrectionStatusId;
                        if (firstCorrectionStatusId == CorrectionStatusEnum.Completed)
                        {
                            var concatenatedApprovers = string.Join(",", orderedCorrections.Select(d => d.ApproverGroupName));
                            orderedCorrections.First().ApproverGroupName = concatenatedApprovers;
                            idsToRemoveFromList.AddRange(orderedCorrections.Skip(1).Select(c => c.Id));
                        }
                        else if (firstCorrectionStatusId == CorrectionStatusEnum.Pending || firstCorrectionStatusId == CorrectionStatusEnum.NeedsMoreInformation)
                        {
                            idsToRemoveFromList.AddRange(orderedCorrections.Skip(1).Select(c => c.Id));
                        }
                        else if (firstCorrectionStatusId == CorrectionStatusEnum.Rejected)
                        {
                            var rejectedCorrection = orderedCorrections
                                .FirstOrDefault(oc => oc.CorrectionActionId == CorrectionActionEnum.Rejected);

                            if (rejectedCorrection != null)
                            {
                                idsToRemoveFromList.AddRange(orderedCorrections.Where(oc => oc.Id != rejectedCorrection.Id).Select(c => c.Id));
                            }
                        }
                        else if (firstCorrectionStatusId == CorrectionStatusEnum.InProgress)
                        {
                            var firstNotApproved = orderedCorrections
                                .FirstOrDefault(oc => oc.CorrectionActionId != CorrectionActionEnum.Approved);

                            if (firstNotApproved != null)
                            {
                                idsToRemoveFromList.AddRange(orderedCorrections.Where(oc => oc.Id != firstNotApproved.Id).Select(c => c.Id));
                            }
                        }
                    }
                });

            if (idsToRemoveFromList.Any())
            {
                workflows.RemoveAll(c => idsToRemoveFromList.Contains(c.Id));
            }

            return workflows;
        }

        private void CheckForRemoveDataItems(List<CorrectionApprovalData> correctionApprovalDatas)
        {
            correctionApprovalDatas.ForEach(cad =>
            {
                cad.RemoveItem = string.IsNullOrEmpty(cad.NewDataPoint);
            });
        }

        private async Task<bool> AllowUserEdit(List<Core.BusinessLayer.Models.StudyRoleModel> roles, Guid userId, CorrectionWorkflow correctionWorkflow)
        {
            var result = false;

            if (!correctionWorkflow.Correction.CorrectionStatus.Resolved || correctionWorkflow.Correction.CorrectionStatus.Id == CorrectionStatusEnum.NeedsMoreInformation)
            {
                if (correctionWorkflow.CorrectionActionId != null && NeedsMoreInformation(correctionWorkflow.Correction))
                {
                    result = correctionWorkflow.Correction.StartedByUserId == userId;
                }
                else if (correctionWorkflow.CorrectionActionId == null || !correctionWorkflow.CorrectionAction.Actionable)
                {
                    var approvalGroups = await _approvalGroupService.GetAll();
                    var roleIds = roles.Select(r => r.Id);
                    var authorizedApproverGroups = approvalGroups
                        .Where(a => a.Roles.Any(r => roleIds.Contains(r.Id)));
                    if (authorizedApproverGroups.Any())
                    {
                        //check for the current workflow order and then allow edit
                        var workflow = await _correctionWorkflowService.Get(correctionWorkflow.Correction.CorrectionTypeId);
                        if (workflow.Configurations.Any(wc =>
                            wc.WorkflowOrder == correctionWorkflow.Correction.CurrentWorkflowOrder &&
                            authorizedApproverGroups.Any(ag => ag.Id == wc.ApproverGroupId)))

                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool NeedsMoreInformation(Correction correction)
        {
            var result = correction.CorrectionWorkflows.Any(cw => cw.CorrectionAction.Id == CorrectionActionEnum.NeedsMoreInformation);
            return result;
        }

        public void UpdateCorrectionWorkFlowNeedsMoreInformation(Guid workFlowId, Guid studyUserId, string discussionComment, CorrectionHistory correctionHistoryToAdd)
        {
            var workFlow = _db.CorrectionWorkflows.Single(w => w.Id == workFlowId);
            workFlow.CorrectionActionId = CorrectionActionEnum.Pending;
            workFlow.Correction.CorrectionStatusId = CorrectionStatusEnum.Pending;

            workFlow.Correction.CorrectionDiscussions.Add
            (
                new CorrectionDiscussion()
                {
                    StudyUserId = studyUserId,
                    DiscussionDate = DateTimeOffset.Now,
                    Discussion = discussionComment,
                    CorrectionActionId = CorrectionActionEnum.Pending
                }
            );

            workFlow.Correction.CorrectionHistory.Add(correctionHistoryToAdd);

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public IEnumerable<CorrectionApprovalData> GetPatientAttributeApprovalDatas(Guid patientId)
        {
            return _db.CorrectionApprovalDatas
                .Join(_db.PatientAttributes, c => c.RowId, a => a.Id, (c, a) => new { CorrectionApprovalData = c, PatientAttribute = a })
                .Where(x => x.PatientAttribute.PatientId == patientId)
                .Select(x => x.CorrectionApprovalData);
        }

        public async Task<IEnumerable<PatientDto>> GetAvailableCorrectionPatients(Guid siteId, StudyUserDto user)
        {
            var sites = new List<Guid>();

            if (!siteId.Equals(Guid.Empty))
            {
                sites.Add(siteId);
            }
            else
            {
                sites.AddRange(user.Sites.Select(s => s.Id));
            }

            var patients = await _patientRepository
                .GetAllPatients(sites);

            return patients;
        }

        public void CreateDCFRequest(DCFRequestDto DCFRequest)
        {
            var dcfRequestEntity = new DCFRequest();
            dcfRequestEntity.CopyPropertiesFromObject(DCFRequest);
            _db.DCFRequest.Add(dcfRequestEntity);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public async Task<Dictionary<string, string>> GetQuestionnaireNameDictionary(Guid patientId)
        {
            var configId = await _patientRepository.GetAssignedConfiguration(patientId);

            var questionnaires = await _questionnaireService.GetAllWithPages(configId);

            var dictionary = questionnaires
                .ToDictionary(q => q.Id.ToString(), q => q.DisplayName);

            return dictionary;
        }

        public async Task<QuestionnaireModel> GetPaperDiaryEntryQuestionnaire(
            Guid questionnaireId,
            Guid patientId)
        {
            var configId = await _patientRepository
                .GetAssignedConfiguration(patientId);

            var questionnaire = await _questionnaireService
                .GetInflatedQuestionnaire(questionnaireId, configurationId: configId);

            if (questionnaire != null)
            {
                questionnaire.Questions = questionnaire.Pages
                                            .OrderBy(p => p.Number)
                                            .SelectMany(p =>
                                            {
                                                p.Questions.ForEach(q => q.Choices = q.Choices.OrderBy(c => c.Sequence).ToList());
                                                return p.Questions.OrderBy(q => q.Sequence);
                                            }).ToList();
            }

            return questionnaire;
        }

        public DateTime GetLocalDateForCorrection(
            Guid? siteId,
            Guid? patientId)
        {
            var siteIdToLookup = siteId ?? Guid.Empty;

            if (siteIdToLookup == Guid.Empty && patientId.HasValue)
            {
                siteIdToLookup = _db
                    .Patients
                    .Where(p => p.Id == patientId)
                    .Select(p => p.SiteId)
                    .First();
            }

            var localDateTime = _siteRepository.GetSiteLocalTime(siteIdToLookup);

            var result = localDateTime.Date.AddDays(1).AddSeconds(-1);

            return result;
        }

        public async Task<int> GetNextDataCorrectionNumber()
        {
            int lastCorrection = 0;
            lastCorrection = (await _db.Corrections
            .OrderByDescending(c => c.DataCorrectionNumber)
            .Select(c => c.DataCorrectionNumber)
            .ToListAsync())
            .FirstOrDefault();
            return lastCorrection + 1;
        }

        public async Task<int> GetDataCorrectionNumberFromId(Guid id)
        {
            int dataCorrectionNumber = 0;
            dataCorrectionNumber = (await _db.Corrections
            .Where(c => c.Id == id)
            .Select(c => c.DataCorrectionNumber)
            .ToListAsync())
            .FirstOrDefault();
            return dataCorrectionNumber;
        }
    }
}
