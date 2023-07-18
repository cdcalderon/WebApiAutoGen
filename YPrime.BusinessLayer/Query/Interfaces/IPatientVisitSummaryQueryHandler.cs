using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Query.Parameters;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Query.Interfaces
{
    public interface IPatientVisitSummaryQueryHandler
    {
        Task<SiteDto> ReadSiteForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters);
        Task<IEnumerable<PatientVisitDto>> ReadPatientVisitsForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters);
        Task<IEnumerable<DiaryEntryDto>> ReadDiaryEntriesForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters);
        Task<List<CareGiver>> ReadCareGiversForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters);
    }
}
