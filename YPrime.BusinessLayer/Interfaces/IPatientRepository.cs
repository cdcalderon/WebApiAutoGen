using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Responses;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPatientRepository : IBaseRepository
    {
        Task<PatientDto> GetPatient(Patient patient, string cultureCode);

        Task<PatientDto> GetPatient(Guid id, string cultureCode);

        Task<Patient> GetPatientForEditAsync(Guid id);

        string DecryptPin(PatientDto patient);
        string DecryptPin(Patient patient);

        Task<IEnumerable<PatientDto>> GetAllPatients(IEnumerable<Guid> allowedSites, bool? showActive = null);

        Task<Guid> GetAssignedConfiguration(Guid patientId);

        Task<int> GetEnrolledPatientCount(List<Guid> AllowedSites);

        Task<PatientDto> CreateNewPatientObject(Guid SiteId);

        Task<IEnumerable<PatientDto>> GetPatients(IEnumerable<Guid> ids);

        Task<Patient> GetPatientEntity(Guid id);

        void UpsertPatientEntity(Patient patient);

        Task<PatientResponse> InsertUpdatePatient(
            PatientDto patient,
            bool insert,
            ModelStateDictionary modelState);

        Task<bool> InsertCareGiver(Guid patientId, Guid careGiverTypeId);

        Task UpdatePatientStatusTypeId(Guid patientId, int patientStatusTypeId);

        Task UpdateNotificationScheduleForPatient(Guid patientId, int patientStatusTypeId, int oldPatientStatusTypeId);

        Task<string> GeneratePin();

        Task<string> GenerateDefaultPin();

        Task<string> GeneratePatientNumber(Guid siteId, string patientNumber, Guid? configurationId = null);

        Task<bool> ResetPatientPin(Guid Id, string NewPin);

        bool ResetCareGiverPin(Guid Id, string NewPin);

        Task<string> GetCareGiverTypeName(Guid Id);

        Task<Device> AddBringYourOwnDeviceAssetTag(string PrimeInventoryAPIUrl, string Environment, Guid PatientId,
            int NumberOfUses);

        Task<List<MergePatientDto>> GetDuplicatePatientsById(
            Guid patientId,
            string cultureCode,
            string globalDateFormat);

        Task<IOrderedQueryable<CareGiverDto>> GetCareGivers(Guid patientId);

        bool AnyPatients(string patientNumber);

        Task<bool> ValidatePatientNumber(string patientNumber, ModelStateDictionary modelState);

        List<string> GetPatientComplianceListByPatientStatusCurrent(List<Guid> SiteIds);

        Task<bool> ValidatePatientAttributesFromDetail(List<PatientAttributeDto> patientAttributes,
           ModelStateDictionary modelState,
           DateTimeOffset currentSiteTime,
           Guid PatientId,
           bool validationIsForCorrection = false,
           bool usePatientConfig = false);
        Task AddAuthUserIdAsync(Guid patientId, string authUserId);
        Task RemovePatient(Guid patientId);
    }
}