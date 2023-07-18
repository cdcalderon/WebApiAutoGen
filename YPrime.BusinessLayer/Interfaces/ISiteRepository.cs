using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.Shared.Helpers.Data;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISiteRepository : IBaseRepository
    {
        Task<List<SiteDto>> GetSitesForUser(Guid UserId);

        Task<SiteDto> GetSite(Guid SiteId);

        Site GetSiteEntity(Guid Id);

        Task<int> GetSiteCompliancePercent(Guid SiteId);

        dynamic GetAllSitesAsDynamic();

        Task<IOrderedEnumerable<SiteDto>> GetAllSites(bool? siteActive = null);

        Task UpdateSiteLanguages(SiteDto siteDto);

        Task UpsertSite(SiteDto siteDto);

        bool CheckSiteNumberIsUsed(string number, Guid? ignoreSiteId = null);

        bool CheckSiteNameIsUsed(string name, Guid? ignoreSiteId = null);

        IEnumerable<SiteDto> GetById(IEnumerable<Guid> ids);

        Task<IEnumerable<Site>> GetAllSiteModels(Guid UserId);

        List<Site> BulkUpdateSites(List<Site> sites);

        Task ValidateSiteImport(FileImport<SiteDto> import);

        Task<DateTime?> CalculateWebBackupExpireDate(string timeZone);

        DateTimeOffset GetSiteLocalTime(Guid siteId);

        Task<Site> GetPatientSiteAsync(Guid patientId);

        Task<IEnumerable<LanguageModel>> GetLanguagesForSite(
            Guid siteId,
            Guid? configurationId);

        
    }
}