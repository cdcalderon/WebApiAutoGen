using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPatientVisitRepository : IBaseRepository
    {
        Task<PatientVisit> ActivatePatientVisit(Guid PatientVisitId, Guid PatientId);
        
        Task<PatientVisitDto> GetPatientVisit(Guid PatientVisitId, string CultureCode);

        Task<List<PatientVisitDto>> GetPatientVisits(Guid PatientId, List<int> visitStatusTypesToInclude, bool IncludeProjectedVisits, string CultureCode);

        Task<IEnumerable<PatientVisitDto>> GetById(IEnumerable<Guid> ids, string CultureCode);

        Task<List<PatientVisitDto>> GetAllPatientVisit(Guid PatientID, string CultureCode);

        Task<List<PatientVisitDto>> GetUnassociatedCompletedPatientVisits(List<string> Sites, string CultureCode);

        Task ProjectPatientVisitSchedule(Guid patientId);

        Task<string> GetPatientVisitDescription(PatientVisit PatientVisit, string CultureCode);

        Task<IEnumerable<PatientVisitSummary>> GetPatientVisitSummary(Guid patientId,
            bool canActivateVisitPortal = false,
            bool canAccessTabletWebBackup = false);

        bool IseCOAVisitInWindow(string visitStop_HSN, int windowBefore, int windowAfter, DateTimeOffset projectedDate, string siteTimeZone);
    }
}