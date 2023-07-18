using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IDiaryEntryRepository : IBaseRepository
    {
        Task<List<DiaryEntryDto>> GetDiaryEntriesInflated(
            Guid? questionnaireId,
            bool includeAnswers, 
            bool? isUserBlinded,
            Guid? patientId = null);

        Task<List<DiaryEntryDto>> GetAllPatientDiaryEntriesByVisit(
            Guid? patientId,
            Guid? visitId,
            bool includeAnswers, 
            bool? isBlinded, 
            string cultureCode);

        IQueryable<DiaryEntryDto> GetDiaryEntries(Guid patientId, List<QuestionnaireModel> questionnaires);

        Task<DiaryEntryDto> GetDiaryEntry(Guid Id, bool includeDCFData, string CultureCode);

        DiaryEntry GetDiaryEntry(Guid Id);

        Task<DiaryEntryDto> GetLastDiaryEntryByDevice(Guid DeviceId, string CultureCode);

        Task SaveDiaryEntryImageToDisk(Guid DiaryEntryId, string Base64, string FileName);
    }
}