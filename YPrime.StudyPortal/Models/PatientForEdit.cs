using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.StudyPortal.Models
{
    public class PatientForEdit
    {
        public Guid Id { get; set; }
        public string PatientNumber { get; set; }
        public string PatientStatus { get; set; }

        [DisplayName("Patient Status")]
        public int PatientStatusTypeId { get; set; }
        public bool Compliance { get; set; }
        public bool IsSiteActive { get; set; }
        public bool HasCareGivers { get; set; }
        public bool IsActive { get; set; }
        public Guid SiteId { get; set; }
        public bool CanEnableWebBackup { get; set; }
        public bool PatientHasDevice { get; set; }

        [DisplayName("Handheld Training Complete")]
        public bool IsHandheldTrainingComplete { get; set; }

        [DisplayName("Tablet Training Complete")]
        public bool IsTabletTrainingComplete { get; set; }

        public WebBackupEmailModel WebBackUpEmail { get; set; }
        public IEnumerable<PatientAttributeForEdit> Attributes { get; set; }
        public IEnumerable<UnscheduledVisitSummary> UnscheduledVisits { get; set; }
        public Guid LanguageId { get; set; }
        public string Language { get; set; }
    }

    public class UnscheduledVisitSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class QuestionnaireSummary
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class PatientAttributeForEdit
    {
        public int Order { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string ValueDisplay { get; set; }
        public IEnumerable<CorrectionDataSummary> CorrectionApprovalDatas { get; set; }
        public bool IsDateTime { get; set; }
        public bool IsMultipleChoice { get; set; }
    }

    public class CorrectionDataSummary
    {
        public int DataCorrectionNumber { get; set; }
        public Guid Id { get; set; }
        public Guid CorrectionId { get; set; }
        public string OldDisplayValue { get; set; }
        public Guid? NextWorkflowId { get; set; }
    }

    public interface IPatientForEditAdapter
    {
        Task<PatientForEdit> Adapt(Patient entity, string cultureCode, bool webBackupIsEnabled, Guid? deviceId, Guid? userId);
    }

    public class PatientForEditAdapter : IPatientForEditAdapter
    {
        private readonly ICorrectionRepository _correctionRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IVisitService _visitService;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ISubjectInformationService _subjectInformationService;

        public PatientForEditAdapter(
            IPatientRepository patientRepository, 
            ICorrectionRepository correctionRepository,
            IVisitService visitService, 
            ILanguageService languageService,
            ITranslationService translationService,
            ISubjectInformationService subjectInformationService,
            IPatientStatusService patientStatusService)
        {
            _patientRepository = patientRepository;
            _correctionRepository = correctionRepository;
            _visitService = visitService;
            _languageService = languageService;
            _translationService = translationService;
            _subjectInformationService = subjectInformationService;
            _patientStatusService = patientStatusService;
        }

        public async Task<PatientForEdit> Adapt(Patient entity, string cultureCode, bool webBackupIsEnabled, Guid? deviceId, Guid? userId)
        {
            var nextWorkflowIdByCorrectionId = new Dictionary<Guid, Guid>();
                
            if(userId.HasValue)
            {
                nextWorkflowIdByCorrectionId = (await _correctionRepository.GetUpcomingWorkflowsForPatient(userId.Value, entity.Id))
                    .ToDictionary(key => key.CorrectionId, value => value.Id);
            }

            var attributeCorrectionApprovalData = _correctionRepository.GetPatientAttributeApprovalDatas(entity.Id)
                .GroupBy(x => x.RowId)
                .ToArray()
                .ToDictionary(x => x.Key, x => x.Select(cad => new CorrectionDataSummary
                {
                    CorrectionId = cad.CorrectionId,
                    Id = cad.Id,
                    DataCorrectionNumber = cad.Correction.DataCorrectionNumber,
                    OldDisplayValue = cad.OldDisplayValue,
                    NextWorkflowId = nextWorkflowIdByCorrectionId.ContainsKey(cad.CorrectionId)
                        ? nextWorkflowIdByCorrectionId[cad.CorrectionId]
                        : (Guid?)null
                }));

            var patientStatuses = await _patientStatusService.GetAll();
            var statusType = entity.GetPatientStatusType(patientStatuses);

            var language = new LanguageModel();

            if (entity != null)
            {
                language = await _languageService.Get(entity.LanguageId);
            }

            var subjectInformationModels = await _subjectInformationService
                .GetForCountry(entity.Site.CountryId);

            var patient = new PatientForEdit
            {
                Id = entity.Id,
                PatientNumber = entity.PatientNumber,
                Compliance = _patientRepository
                    .GetPatientComplianceListByPatientStatusCurrent(new List<Guid> {entity.Site.Id})
                    .Any(c => c == entity.PatientNumber),
                IsSiteActive = entity.Site.IsActive,
                HasCareGivers = entity.CareGivers.Any(),
                IsHandheldTrainingComplete = entity.IsHandheldTrainingComplete,
                IsTabletTrainingComplete = entity.IsTabletTrainingComplete,
                PatientStatusTypeId = entity.PatientStatusTypeId,
                PatientStatus = statusType.Name,
                IsActive = statusType.IsActive,
                SiteId = entity.Site.Id,
                CanEnableWebBackup = statusType.IsActive && webBackupIsEnabled,
                PatientHasDevice = deviceId.HasValue,
                LanguageId = language.Id,
                Language = language?.DisplayName,
                Attributes = BuildPatientAttributes(
                    entity.PatientAttributes, 
                    attributeCorrectionApprovalData,
                    subjectInformationModels),      
                UnscheduledVisits = new List<UnscheduledVisitSummary>()
            };

           return patient;
        }

        public IEnumerable<PatientAttributeForEdit> BuildPatientAttributes(
            ICollection<PatientAttribute> attrs,
            Dictionary<Guid, IEnumerable<CorrectionDataSummary>> attributeCorrectionApprovalData,
            List<SubjectInformationModel> subjectInformationModels)
        {
            var patientAttributes = new List<PatientAttributeForEdit>();

            foreach (var a in attrs)
            {
                var config = subjectInformationModels
                    .FirstOrDefault(si => si.Id == a.PatientAttributeConfigurationDetailId);

                if (config != null)
                {
                    var selectedChoice = config
                        ?.Choices
                        ?.FirstOrDefault(c => c.Id.ToString().ToUpper() == a.AttributeValue?.ToUpper());

                    var dataType = config.GetDataType();

                    patientAttributes.Add(new PatientAttributeForEdit
                    {
                        Label = config.Name,
                        Order = config.Sequence ?? 0,
                        Value = a.AttributeValue,
                        ValueDisplay = selectedChoice?.Name ?? a.AttributeValue,
                        IsDateTime = dataType.IsDateTime,
                        IsMultipleChoice = dataType.IsMultipleChoice,
                        CorrectionApprovalDatas = attributeCorrectionApprovalData.ContainsKey(a.Id)
                            ? attributeCorrectionApprovalData[a.Id]
                            : new List<CorrectionDataSummary>()
                    });
                }
            }

            return patientAttributes;
        }
    }
}