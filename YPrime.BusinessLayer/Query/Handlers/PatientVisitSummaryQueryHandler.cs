using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Query.Interfaces;
using YPrime.BusinessLayer.Query.Parameters;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Query.Handlers
{

    public class PatientVisitSummaryQueryHandler : IPatientVisitSummaryQueryHandler
    { 
        public IStudyDbContext DbContext { get; }
        public PatientVisitSummaryQueryHandler(IStudyDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Read the <see cref="SiteDto"/> with only the columns necessary for generating the PatientVisitSummary
        /// </summary>
        /// <param name="parameters"><see cref="PatientVisitSummaryQueryParameters"/>Parameters for PatientVisitSummary Queries</param>
        /// <returns><see cref="SiteDto"/></returns>
        public async Task<SiteDto> ReadSiteForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters)
        {
            return await this.DbContext.Patients
                .AsNoTracking()
                .Where(w => w.Id == parameters.PatientId)
                .Select(s => new SiteDto 
                {
                    Id = s.Site.Id,
                    WebBackupExpireDate = s.Site.WebBackupExpireDate,
                    TimeZone = s.Site.TimeZone
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Read the <see cref="IEnumerable{PatientVisitDto}"/> with only the columns necessary for generating the PatientVisitSummary
        /// </summary>
        /// <param name="parameters"><see cref="PatientVisitSummaryQueryParameters"/>Parameters for PatientVisitSummary Queries</param>
        /// <returns><see cref="IEnumerable{PatientVisitDto}"/></returns>
        public async Task<IEnumerable<PatientVisitDto>> ReadPatientVisitsForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters)
        {
            return await this.DbContext.PatientVisits
                .AsNoTracking()
                .Include(i => i.Patient.Site)                
                .Where(p => p.PatientId == parameters.PatientId)
                .Select(s => new PatientVisitDto
                {
                    Id = s.Id,
                    VisitId = s.VisitId,
                    PatientVisitStatusTypeId = s.PatientVisitStatusTypeId,
                    PatientId = s.PatientId,
                    ActivationDate = s.ActivationDate,
                    ProjectedDate = s.ProjectedDate,
                    VisitDate = s.VisitDate

                }).ToListAsync();
            
        }

        /// <summary>
        /// Read the <see cref="IEnumerable{DiaryEntryDto}"/> with only the columns necessary for generating the PatientVisitSummary
        /// </summary>
        /// <param name="parameters"><see cref="PatientVisitSummaryQueryParameters"/>Parameters for PatientVisitSummary Queries</param>
        /// <returns><see cref="IEnumerable{DiaryEntryDto}"/></returns>
        public async Task<IEnumerable<DiaryEntryDto>> ReadDiaryEntriesForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters)
        {
            var diaryEntriesForPatient = await DbContext.DiaryEntries
                .AsNoTracking()
                .Where(w => w.PatientId == parameters.PatientId)
                .Select(s => new DiaryEntryDto
                {
                    Id = s.Id,
                    VisitId = s.VisitId,
                    QuestionnaireId = s.QuestionnaireId
                }).ToListAsync();

            return diaryEntriesForPatient;
        }

        public async Task<List<CareGiver>> ReadCareGiversForPatientVisitSummary(PatientVisitSummaryQueryParameters parameters)
        {
            return await DbContext.CareGivers
                .AsNoTracking()
                .Where(c => c.PatientId == parameters.PatientId)
                .ToListAsync();
        }
    }
}
