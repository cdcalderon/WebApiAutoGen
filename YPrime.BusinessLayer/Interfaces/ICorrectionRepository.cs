using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ICorrectionRepository
    {
        Task<Correction> CreateCorrectionObject(Guid StudyUserId, Guid? PatientId);

        Task<Correction> GetCorrection(Guid Id, string CultureName);

        Task<List<CorrectionWorkflow>> GetCorrectionListForUser(Guid StudyUserId, string CultureName);

        Task<List<CorrectionWorkflow>> GetCorrectionListForUserPending(Guid StudyUserId, string CultureName);

        Task<List<CorrectionWorkflow>> GetCorrectionListForUserComplete(Guid StudyUserId, string CultureName);
        
        Task<List<CorrectionWorkflow>> GetUpcomingWorkflowsForPatient(Guid studyUserId, Guid patientId);

        Task UpdateCorrectionWorkFlow(
            Guid workflowId, 
            Guid correctionActionId, 
            Guid studyUserId,
            string discussionComment,
            CorrectionHistory correctionHistoryToAdd);

        Task SaveInitialCorrection(Correction correction);

        List<CorrectionAction> GetCorrectionActions();

        Task<Guid> GetCorrectionConfigurationId(Correction correction);

        Task<CorrectionWorkflow> GetCorrectionWorkflow(
            Guid id, 
            List<StudyRoleModel> roles, 
            Guid userId, 
            string cultureName);

        bool NeedsMoreInformation(Correction correction);

        void UpdateCorrectionWorkFlowNeedsMoreInformation(
            Guid workFlowId, 
            Guid studyUserId, 
            string discussionComment,
            CorrectionHistory correctionHistoryToAdd);

        IEnumerable<CorrectionApprovalData> GetPatientAttributeApprovalDatas(Guid patientId);

        Task<bool> ValidateCorrection(Correction correction, ModelStateDictionary modelState, string cultureCode);

        Task<IEnumerable<PatientDto>> GetAvailableCorrectionPatients(Guid siteId, StudyUserDto user);

        Task SortDiaryEntryCorrectionApprovals(Correction correction);

        Task<Dictionary<string, string>> GetQuestionnaireNameDictionary(Guid patientId);

        Task<QuestionnaireModel> GetPaperDiaryEntryQuestionnaire(
            Guid questionnaireId,
            Guid patientId);

        void CreateDCFRequest(DCFRequestDto DCFRequest);

        Task FormatControlValues(
            Correction correction,
           CorrectionStateInternal currentState);

        DateTime GetLocalDateForCorrection(
            Guid? siteId,
            Guid? patientId);
    }
}