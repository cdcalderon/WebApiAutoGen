using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.Shared.Helpers.Data;
using YPrime.Core.BusinessLayer.Models;
using System.Data.Entity;

namespace YPrime.BusinessLayer.Repositories
{
    public class SiteRepository : BaseRepository, ISiteRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly ILanguageService _languageService;
        private readonly ICountryService _countryService;
        private readonly IStudyRoleService _studyRoleService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly IStudySettingService _studySettingService;

        public SiteRepository(
            IStudyDbContext db,
            IUserRepository userRepository,
            ILanguageService languageService,
            IStudyRoleService studyRoleService,
            IPatientStatusService patientStatusService,
            ICountryService countryService,
            IStudySettingService studySettingService)
            : base(db)
        {
            _userRepository = userRepository;
            _languageService = languageService;
            _studyRoleService = studyRoleService;
            _patientStatusService = patientStatusService;
            _countryService = countryService;
            _studySettingService = studySettingService;
        }

        public async Task<List<SiteDto>> GetSitesForUser(Guid UserId)
        {
            var results = (await GetUserSites(UserId)).ToList();

            var countries = await _countryService.GetAll();
            return results
                .Select(s =>
                {
                    var result = new SiteDto();
                    result.CopyPropertiesFromObject(s);
                    result.CountryName = countries.FirstOrDefault(c => c.Id == s.CountryId).Name;
                    return result;
                }
                ).ToList();
        }

        public async Task<IOrderedEnumerable<SiteDto>> GetAllSites(bool? siteActive = null)
        {
            var countries = await _countryService.GetAll();
            var siteFilter = new SiteFilter();

            var filteredSites = siteFilter.Execute(_db.Sites)
                .Where(s => siteActive == null || s.IsActive == siteActive)
                .ToList();

            var result = filteredSites
                .Select(s =>
                    new SiteDto
                    {
                        Id = s.Id,
                        SiteNumber = s.SiteNumber,
                        Name = s.Name,
                        Address1 = s.Address1,
                        Address2 = s.Address2,
                        Address3 = s.Address3,
                        TimeZone = s.TimeZone,
                        Notes = s.Notes,
                        PatientDOBFormatId = s.PatientDOBFormatId,
                        City = s.City,
                        State = s.State,
                        Zip = s.Zip,
                        CountryId = s.CountryId,
                        CountryName = countries.FirstOrDefault(c => c.Id == s.CountryId)?.Name,
                        PrimaryContact = s.PrimaryContact,
                        PhoneNumber = s.PhoneNumber,
                        FaxNumber = s.FaxNumber,
                        IsActive = s.IsActive,
                        LastUpdate = s.LastUpdate,
                        SiteDisplayLanguageId = s.SiteDisplayLanguageId
                    }).OrderBy(s => s.Name);

            return result;
        }

        public async Task<IEnumerable<Site>> GetAllSiteModels(Guid UserId)
        {
            var sites = await GetUserSites(UserId);
            return sites.ToList();
        }

        public dynamic GetAllSitesAsDynamic()
        {
            var siteFilter = new SiteFilter();

            var sites = siteFilter.Execute(_db.Sites)
                .OrderBy(s => s.Name)
                .Select(s =>
                    new
                    {
                        s.Id,
                        SiteId = s.SiteNumber,
                        SiteName = s.Name,
                        SiteDisplayLanguageId = s.SiteDisplayLanguageId,
                        IsActive = s.IsActive
                    }).ToList();

            return sites;
        }

        public async Task<SiteDto> GetSite(Guid siteId)
        {         
            SiteDto result = new SiteDto();
            var siteFilter = new SiteFilter();

            var site = siteFilter.Execute(_db.Sites).Include("SiteLanguages").Single(s => s.Id == siteId);

            result.CopyPropertiesFromObject(site);

            var language = await _languageService.GetAll();
            result.Languages = site.SiteLanguages.Select(l => new LanguageDto
                {Id = l.LanguageId, Name = language.FirstOrDefault(x => x.Id == l.LanguageId)?.DisplayName, Selected = true}).ToList();

            return result;
        }

     

        public Site GetSiteEntity(Guid Id)
        {
            var siteFilter = new SiteFilter();

            var result = siteFilter.Execute(_db.Sites)
                .Single(s => s.Id == Id);
            return result;
        }

        public async Task UpdateSiteLanguages(SiteDto siteDto)
        {
            var siteId = siteDto.Id;
            var siteDtoLanguageCollection = siteDto.Languages;

            var IDListToAdd = siteDtoLanguageCollection.Where(l => l.Selected).Select(l => l.Id);
            var IDListToRemove = siteDtoLanguageCollection.Where(l => !l.Selected).Select(l => l.Id);

            var languages = await _languageService.GetAll();

            var LanguagesToAdd = languages.Where(l => IDListToAdd.Contains(l.Id));
            var LanguagesToRemove = languages.Where(l => IDListToRemove.Contains(l.Id));

            // no needd to do site filtering here, since we accessed the site already. 
            // and are making changes to it
            var site = _db.Sites
                .SingleOrDefault(s => s.Id == siteId) ?? new Site();

            var siteLanguageCollection = site?.SiteLanguages;

            if (siteLanguageCollection != null && siteLanguageCollection.Any())
            {
                foreach (var language in LanguagesToRemove)
                {
                    var siteLanguage = siteLanguageCollection
                        .FirstOrDefault(sl => sl.LanguageId == language.Id);

                    if (siteLanguage != null)
                    {
                        _db.SiteLanguages.Remove(siteLanguage);
                        siteLanguageCollection.Remove(siteLanguage);
                    }
                }
            }

            if (siteLanguageCollection == null)
            {
                siteLanguageCollection = new List<SiteLanguage>();
            }

            foreach (var language in LanguagesToAdd)
            {
                var siteLanguage = siteLanguageCollection.FirstOrDefault(sl => sl.LanguageId == language.Id);
                if (siteLanguage == null)
                {
                    siteLanguage = new SiteLanguage {SiteId = siteId, LanguageId = language.Id};
                    siteLanguageCollection.Add(siteLanguage);
                }
            }

            site.SiteLanguages = siteLanguageCollection;
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public async Task<IEnumerable<LanguageModel>> GetLanguagesForSite(
            Guid siteId,
            Guid? configurationId)
        {
            var allLanguages = await _languageService
                .GetAll(configurationId);

            var siteLanguageIds = _db.SiteLanguages
                 .Where(sl => sl.SiteId == siteId)
                 .Select(sl => sl.LanguageId)
                 .ToList();

            var siteLanguages = allLanguages
                .Where(al => siteLanguageIds.Contains(al.Id));

            return siteLanguages;
        }

        public async Task<int> GetSiteCompliancePercent(Guid SiteId)
        {
            var siteIds = new List<Guid>();
            int result;
            int totalCompliantSubjects;
            int totalSubjects;

            siteIds.Add(SiteId);

            totalCompliantSubjects = GetPatientComplianceListByPatientStatusCurrentAsync(siteIds).Result.Count;
            totalSubjects = await GetEnrolledPatientCount(siteIds);
            result = totalCompliantSubjects > 0 ? (int)((decimal)totalCompliantSubjects / (decimal)totalSubjects * 100) : 0;

            return result;
        }

        public async Task UpsertSite(SiteDto siteDto)
        {
            var site = _db.Sites
                .SingleOrDefault(s => s.Id == siteDto.Id);

            if (site == null)
            {
                await InsertSite(siteDto);
            }
            else
            {
                await EditSite(siteDto, site);
            }
        }

        public bool CheckSiteNumberIsUsed(string number, Guid? ignoreSiteId = null)
        {
            return _db.Sites.Any(x => x.SiteNumber == number && (ignoreSiteId == null || x.Id != ignoreSiteId.Value));
        }

        public bool CheckSiteNameIsUsed(string name, Guid? ignoreSiteId = null)
        {
            return _db.Sites.Any(x => x.Name == name && (ignoreSiteId == null || x.Id != ignoreSiteId.Value));
        }

        public IEnumerable<SiteDto> GetById(IEnumerable<Guid> ids)
        {
            var siteFilter = new SiteFilter();

            var sites = siteFilter.Execute(_db.Sites)
                .Include("SiteLanguages")
                .Where(s => ids.Contains(s.Id))
                .ToList();

            var result = sites
                .Select(s =>
                {
                    var dto = new SiteDto();
                    dto.CopyPropertiesFromObject(s);

                    return dto;
                });

            return result;
        }

        public async Task ValidateSiteImport(FileImport<SiteDto> import)
        {
            bool AllowSiteEdit = false;
            bool SiteInUse;

            var languages = await _languageService.GetAll();
            var countries = await _countryService.GetAll();
            //Init & loop sites

            var siteFilter = new SiteFilter();

            var siteNumbers = siteFilter.Execute(_db.Sites).ToDictionary(s => s.Id, s => s.SiteNumber.ToLower());
            var siteNames = siteFilter.Execute(_db.Sites).ToDictionary(s => s.Id, s => s.Name.ToLower());
            var countryNames = countries.ToDictionary(c => c.Id, c => c.Name.ToLower());
            var languageCodes = languages.ToDictionary(l => l.Id, l => l.CultureName);
            var languageDesc = languages.ToDictionary(l => l.Id, l => l.Notes);

            foreach (var siteImport in import.ImportedObjects)
            {
                //Cast entity as Site
                var site = (SiteDto) siteImport.Entity;

                //SiteNumber
                SiteInUse = false;
                var siteNumber = site.SiteNumber.ToLower();
                var siteNumberLength = await _studySettingService.GetStringValue("SiteNumberLength");
                if (!siteNumber.Length.Equals(int.Parse(siteNumberLength)))
                {
                    siteImport.ValidationErrors.Add($"site number must be exactly '{siteNumberLength}' numbers");
                }

                if (siteNumbers.ContainsValue(siteNumber))
                {
                    if (AllowSiteEdit)
                    {
                        site.Id = siteNumbers.Single(c => c.Value == siteNumber).Key; //Update
                    }
                    else
                    {
                        siteImport.ValidationErrors.Add($"The site number '{siteNumber}' is already in use");
                        SiteInUse = true;
                    }
                }

                //SiteName
                var siteName = site.Name.ToLower();
                if (siteNames.ContainsValue(siteName) && !SiteInUse)
                {
                    siteImport.ValidationErrors.Add($"A site already exists with the name '{site.Name}'");
                }

                //Country
                var countryName = site.CountryName.ToLower();
                if (countryNames.ContainsValue(countryName))
                {
                    site.CountryId = countryNames.Single(c => c.Value == countryName).Key;
                }
                else
                {
                    siteImport.ValidationErrors.Add($"Unable to find a Country with the name '{site.CountryName}'");
                }

                //Timezone
                string timeZone = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(tz => tz.Id.ToLower() == site.TimeZone.ToLower())?.Id;
                if (string.IsNullOrEmpty(timeZone))
                {
                    siteImport.ValidationErrors.Add($"Unable to find a TimeZone with the name '{site.TimeZone}'");
                }
                else
                {
                    site.TimeZone = timeZone;
                }

                if (!string.IsNullOrWhiteSpace(site.AllowedLanguages))
                {
                    string[] SiteLanguageCodes = site.AllowedLanguages.Trim().Split(',');
                    bool AllLanguagesValid = true;
                    // Confirm each one exists in table

                    foreach (string languageCode in SiteLanguageCodes)
                    {
                        if (!languageCodes.ContainsValue(languageCode) && !languageDesc.ContainsValue(languageCode))
                        {
                            siteImport.ValidationErrors.Add(
                                $"Unable to find a language code with the name '{languageCode}'");
                            AllLanguagesValid = false;
                        }
                    }

                    // If all exists, update site.languages accordingly
                    if (AllLanguagesValid)
                    {
                        site.Languages = new List<LanguageDto>();
                        foreach (string languageCode in SiteLanguageCodes)
                        {
                            Guid LangCodeKey = Guid.Empty;
                            if (languageCodes.ContainsValue(languageCode))
                            {
                                LangCodeKey = languageCodes.FirstOrDefault(x => x.Value == languageCode).Key;
                            }

                            if (languageDesc.ContainsValue(languageCode))
                            {
                                LangCodeKey = languageDesc.FirstOrDefault(x => x.Value == languageCode).Key;
                            }

                            if (LangCodeKey != Guid.Empty)
                            {
                                LanguageDto langRecord = new LanguageDto
                                {
                                    Name = languageCode,
                                    Id = LangCodeKey,
                                    Selected = true
                                };
                                site.Languages.Add(langRecord);
                            }
                        }
                    }
                }
                else
                {
                    siteImport.ValidationErrors.Add(
                                $"At least one site language must be included");
                }
            }
        }

        public List<Site> BulkUpdateSites(List<Site> sites)
        {
            var siteIds = sites.Select(x => x.Id).ToList();
            var allSites = _db.Sites.Where(x => siteIds.Contains(x.Id)).ToList();

            allSites.ForEach(x =>
            {
                var siteData = sites.FirstOrDefault(y => y.Id == x.Id);

                bool previousActive = x?.IsActive ?? false;
                x.IsActive = siteData.IsActive;
                AddActivationHistoryToSite(x, previousActive);

                _db.Sites.AddOrUpdate(x);
            });

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            return allSites;
        }

        public async Task<DateTime?> CalculateWebBackupExpireDate(string timeZone)
        {
            DateTime? WebBackupExpireDate = null;
            if (!string.IsNullOrEmpty(timeZone))
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                    var localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

                    string NumDays = await _studySettingService.GetStringValue("WebBackupTabletEnabled");

                    if (double.TryParse(NumDays, out double NumberDays) && NumberDays > 0)
                    {
                        WebBackupExpireDate = localToday.Date.AddDays(NumberDays).AddSeconds(86399);
                    }
                }
                catch
                {
                    WebBackupExpireDate = null;
                }
            }

            return WebBackupExpireDate;
        }

        public DateTimeOffset GetSiteLocalTime(Guid siteId)
        {
            var siteFilter = new SiteFilter();

            var site = siteFilter.Execute(_db.Sites)
                .Single(s => s.Id == siteId);
            return DateTimeOffset.Now.ConvertToTimeZone(site.TimeZone);
        }

        public async Task<Site> GetPatientSiteAsync(Guid patientId)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            return patientFilter.Execute(_db.Patients)
                .FirstOrDefault(p => p.Id == patientId)?.Site;
        }

        private async Task<IQueryable<Site>> GetUserSites(Guid userId)
        {
            var allowAllSites = false;

            var roles = await _userRepository.GetRolesForUser(userId, null);
            allowAllSites = roles.Any(r => r == "YP");

            var siteIds = _db.StudyUserRoles.Where(x => x.StudyUser.Id == userId).Select(x => x.SiteId);

            var results = _db.Sites.
                Where(s => allowAllSites || siteIds.Contains(s.Id))
                .OrderBy(s => s.Name)
                .AsQueryable();

            return results;
        }

        private void SetWebBackupExpiration(SiteDto siteDto, Site site)
        {
            if (!siteDto.IsWebBackupEnabled)
            {
                site.WebBackupExpireDate = null;
            }
        }

        private async Task<List<string>> GetPatientComplianceListByPatientStatusCurrentAsync(List<Guid> SiteIds)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patientList = patientFilter.Execute(_db.Patients)
                .Where(p => !SiteIds.Any() || SiteIds.Contains(p.SiteId))
                .ToList();

            var result = patientList.Where(p => p.PatientStatusTypeId != 1)
                .Select(p => p.PatientNumber)
                .ToList();

            return result;
        }

        private async Task<int> GetEnrolledPatientCount(List<Guid> AllowedSites)
        {
            int result;
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            if (AllowedSites == null || !AllowedSites.Any())
            {
                result = patientFilter.Execute(_db.Patients).Count();
            }
            else
            {
                result = patientFilter.Execute(_db.Patients).Count(p => AllowedSites.Contains(p.SiteId));
            }

            return result;
        }
        private async Task EditSite(SiteDto siteDto, Site site)
        {
            AddActivationHistoryToSite(siteDto, site);
            SetWebBackupExpiration(siteDto, site);
            site.CopyPropertiesFromObject(siteDto);

            if (siteDto.Languages.Count > 0)
            {
                await UpdateSiteLanguages(siteDto);
            }

            _db.Sites.AddOrUpdate(site);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            siteDto.Id = site.Id;
        }

        private async Task InsertSite(SiteDto siteDto)
        {
            var site = new Site();
            AddActivationHistoryToSite(siteDto, site);
            SetWebBackupExpiration(siteDto, site);
            site.CopyPropertiesFromObject(siteDto);
            site.Id = Guid.NewGuid();
            site.ConfigurationId = YPrimeSession.Instance.ConfigurationId;

            _db.Sites.AddOrUpdate(site);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            if (siteDto.Languages.Count > 0)
            {
                siteDto.Id = site.Id;
                await UpdateSiteLanguages(siteDto);
            }

            // Need to add all users to this site that belong to a Role that has the AutoAssignNewSites flag tripped
            var allRoles = await _studyRoleService.GetAll();
            var allDbRoles = _db.StudyUserRoles.ToList();
            foreach (var Role in allRoles.Where(sr => sr.AutoAssignNewSites))
            {
                var autoAssignUserRoles = allDbRoles.Where(sur => sur.StudyRoleId == Role.Id);
                foreach (var SUR in autoAssignUserRoles.GroupBy(x => new { x.StudyUserId, x.StudyRoleId }))
                {
                    SUR.First().StudyUser.StudyUserRoles.Add(new StudyUserRole
                    {
                        Id = Guid.NewGuid(),
                        StudyRoleId = SUR.Key.StudyRoleId,
                        SiteId = site.Id,
                        StudyUserId = SUR.Key.StudyUserId
                    });
                }
            }      

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            siteDto.Id = site.Id;
        }

        private void AddActivationHistoryToSite(Site site, bool previous)
        {
            var history = new SiteActiveHistory
            {
                Id = Guid.NewGuid(),
                Current = site.IsActive,
                Previous = previous
            };

            //-- only post modifications.
            if (history.Previous != history.Current)
            {
                site.SiteActiveHistory.Add(history);
            }
        }

        private void AddActivationHistoryToSite(SiteDto siteDto, Site site)
        {
            if (siteDto.IsActive != site.IsActive)
            {
                site.SiteActiveHistory.Add(new SiteActiveHistory
                {
                    Current = siteDto.IsActive,
                    Previous = site.IsActive,
                    Id = Guid.NewGuid()
                });
            }
        }
    }
}