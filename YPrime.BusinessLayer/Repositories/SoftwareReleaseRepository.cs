using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class SoftwareReleaseRepository : BaseRepository, ISoftwareReleaseRepository
    {
        private readonly ICountryService _countryService;
        private readonly ITranslationService _translationService;
        private readonly IConfigurationVersionService _configurationVersionService;

        public SoftwareReleaseRepository(
            IStudyDbContext db,
            ICountryService countryService,
            ITranslationService translationService,
            IConfigurationVersionService configurationVersionService)
            : base(db)
        {
            _countryService = countryService;
            _translationService = translationService;
            _configurationVersionService = configurationVersionService;
        }

        public async Task<string> GetSoftwareReleaseDeviceConfirmation(SoftwareReleaseDto softwareRelease)
        {
            var newVersionNumber = Version.Parse(softwareRelease.VersionNumber);

            var configVersions = await _configurationVersionService.GetAll();
            var selectedConfigVersion = configVersions.FirstOrDefault(c => c.Id == softwareRelease.ConfigurationId)?.ConfigurationVersionNumber;
            var newConfigNumber = Version.Parse(selectedConfigVersion);

            var deviceList = GetDevicesForSoftwareRelease(softwareRelease);

            var eligibleDevices = GetDevicesEligibleForReleaseUpgrade(
                deviceList,
                newVersionNumber,
                newConfigNumber);

            var eligibleSoftwareVersionCount = eligibleDevices.Count(d => Version.Parse(d.SoftwareRelease.SoftwareVersion.VersionNumber) < newVersionNumber);
            var eligibleConfigVersionCount = eligibleDevices.Count(d => d.SoftwareRelease.ConfigurationVersion == null || Version.Parse(d.SoftwareRelease.ConfigurationVersion) < newConfigNumber);
            var ineligibleSoftwareVersionCount = deviceList.Count - eligibleSoftwareVersionCount;
            var ineligibleConfigVersionCount = deviceList.Count - eligibleConfigVersionCount;

            var translationResource = await _translationService.GetByKey("SoftwareReleaseConfirmationMessage");

            var message = translationResource?.Replace("{{eligibleSoftwareVersions}}", eligibleSoftwareVersionCount.ToString())
                    .Replace("{{eligibleConfigVersions}}", eligibleConfigVersionCount.ToString())
                    .Replace("{{ineligibleSoftwareVersions}}", ineligibleSoftwareVersionCount.ToString())
                    .Replace("{{ineligibleConfigVersions}}", ineligibleConfigVersionCount.ToString());

            return message;
        }

        public async Task CreateSoftwareRelease(SoftwareReleaseDto softwareRelease)
        {
            await AddSoftwareRelease(softwareRelease);

            var deviceList = GetDevicesForSoftwareRelease(softwareRelease);

            var eligibleDevices = GetEligibleDevices(
                deviceList,
                softwareRelease.Id);

            await UpdateDeviceSoftwareRelease(eligibleDevices, softwareRelease);
        }

        public bool DeactivateSoftwareRelease(Guid id)
        {
            var success = false;
            var entity = _db.SoftwareReleases.SingleOrDefault(s => s.Id == id);

            if (entity != null)
            {
                entity.IsActive = false;
                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
                success = true;
            }

            return success;
        }

        public async Task<List<DeviceType>> GetProvisionalDeviceTypesForStudy()
        {
            var usedDeviceTypeIds = await _db
                .Devices
                .Where(d => d.DeviceTypeId != DeviceType.BYOD.Id)
                .Select(d => d.DeviceTypeId)
                .Distinct()
                .ToListAsync();

            var deviceTypes = DeviceType
                .Where<DeviceType>(dt => usedDeviceTypeIds.Contains(dt.Id))
                .ToList();

            return deviceTypes;
        }

        public List<Device> GetDevicesForSoftwareRelease(SoftwareReleaseDto softwareRelease)
        {
            var deviceList = new List<Device>();

            if (softwareRelease.StudyWide)
            {
                var studyDeviceList = _db.Devices.ToList();
                deviceList.AddRange(studyDeviceList);
            }
            else
            {
                var siteIds = softwareRelease.SiteIds ?? new List<Guid>();
                var countryIds = softwareRelease.CountryIds ?? new List<Guid>();
                var deviceTypeIds = softwareRelease.DeviceTypeIds ?? new List<Guid>();

                if (deviceTypeIds.Contains(DeviceType.Phone.Id))
                {
                    deviceTypeIds.Add(DeviceType.BYOD.Id);
                }

                deviceList.AddRange(_db.Devices.Where(d =>
                    (siteIds.Count == 0 || (d.SiteId != null && siteIds.Contains((Guid)d.SiteId))) &&
                    (countryIds.Count == 0 || (d.Site != null && countryIds.Contains(d.Site.CountryId))) &&
                    (deviceTypeIds.Count == 0 || deviceTypeIds.Contains(d.DeviceTypeId))));
            }

            return deviceList;
        }

        public IEnumerable<Device> GetEligibleDevices(
            List<Device> devices,
            Guid softwareReleaseId)
        {
            var softwareReleaseVersions = _db
                .SoftwareReleases
                .Where(sr => sr.Id == softwareReleaseId)
                .Select(sr => new
                {
                    sr.SoftwareVersion.VersionNumber,
                    sr.ConfigurationVersion
                })
                .First();

            var newVersionNumber = Version.Parse(softwareReleaseVersions.VersionNumber);
            var newConfigNumber = Version.Parse(softwareReleaseVersions.ConfigurationVersion);

            var eligibleDevices = GetDevicesEligibleForReleaseUpgrade(
                devices,
                newVersionNumber,
                newConfigNumber);

            return eligibleDevices;
        }

        private IEnumerable<Device> GetDevicesEligibleForReleaseUpgrade(
            List<Device> devices,
            Version newVersionNumber,
            Version newConfigNumber)
        {
            // include devices that:
            //  - have either a lower version or a lower config
            //  - downgrade software version if a higher config version is assigned
            //  - do not have both a version and config that match the new release

            var eligibleDevices = devices
                .Where(d =>
                    (
                        (
                            string.IsNullOrWhiteSpace(d.SoftwareRelease.ConfigurationVersion) ||
                            Version.Parse(d.SoftwareRelease.ConfigurationVersion) < newConfigNumber
                        ) ||
                        (
                            Version.Parse(d.SoftwareRelease.SoftwareVersion.VersionNumber) <= newVersionNumber &&
                            (
                                string.IsNullOrWhiteSpace(d.SoftwareRelease.ConfigurationVersion) ||
                                Version.Parse(d.SoftwareRelease.ConfigurationVersion) <= newConfigNumber
                            )
                        )
                    ) &&
                    !(
                        Version.Parse(d.SoftwareRelease.SoftwareVersion.VersionNumber) == newVersionNumber &&
                        !string.IsNullOrWhiteSpace(d.SoftwareRelease.ConfigurationVersion) &&
                        Version.Parse(d.SoftwareRelease.ConfigurationVersion) == newConfigNumber
                    ));

            return eligibleDevices;
        }

        public SoftwareRelease GetLatestSoftwareRelease(Guid? SiteId, Guid DeviceTypeId)
        {
            var associatedCountryId = SiteId.HasValue
                ? _db.Sites.First(s => s.Id == SiteId.Value).CountryId
                : (Guid?)null;

            var releases = _db.SoftwareReleases
                .GroupJoin(_db.SoftwareReleaseCountry,
                    sr => sr.Id,
                    src => src.SoftwareReleaseId,
                    (sr, countryReleases) => new { SoftwareRelease = sr, SoftwareReleaseCountry = countryReleases })
                .GroupJoin(_db.SoftwareReleaseDeviceTypes,
                    sr => sr.SoftwareRelease.Id,
                    dt => dt.SoftwareRelease.Id,
                    (sr, dt) => new { sr.SoftwareRelease, sr.SoftwareReleaseCountry, SoftwareReleaseDeviceTypes = dt })
                .Where(s => (s.SoftwareRelease.StudyWide ||
                    s.SoftwareRelease.Sites.Any(st => st.Id == SiteId) ||
                    (
                        associatedCountryId.HasValue &&
                        s.SoftwareReleaseCountry.Any(c => c.CountryId == associatedCountryId)
                    ) ||
                    s.SoftwareReleaseDeviceTypes.Any(sr => sr.DeviceTypeId == DeviceTypeId
                    || (sr.DeviceTypeId == DeviceType.Phone.Id && DeviceTypeId == DeviceType.BYOD.Id)))
                    && s.SoftwareRelease.IsActive)
                .ToList();

            var result = releases
                .OrderByDescending(s => Version.Parse(s.SoftwareRelease.SoftwareVersion.VersionNumber))
                .ThenByDescending(s => string.IsNullOrWhiteSpace(s.SoftwareRelease.ConfigurationVersion)
                    ? Version.Parse("0.0")
                    : Version.Parse(s.SoftwareRelease.ConfigurationVersion))
                .FirstOrDefault();

            return result.SoftwareRelease;
        }

        public async Task<List<SoftwareReleaseDto>> GetSoftwareReleaseGridData()
        {
            var releases = await _db.SoftwareReleases.OrderByDescending(r => r.DateCreated).ToListAsync();
            var versions = await _db.SoftwareVersions.ToDictionaryAsync(s => s.Id, s => s);
            var devices = await _db.Devices.ToListAsync();
            var softwareReleaseCountries = _db.SoftwareReleaseCountry as IEnumerable<SoftwareReleaseCountry>;
            var countries = await _countryService.GetAll();
            var configVersions = await _configurationVersionService.GetAll();
            var softwareReleaseDeviceTypes = await _db.SoftwareReleaseDeviceTypes.ToListAsync();

            return releases.Select(s => new SoftwareReleaseDto
            {
                Id = s.Id,
                ReleaseDate = s.DateCreated.ToString("dd-MMM-yy"),
                Name = s.Name,
                VersionNumber = versions[s.SoftwareVersionId].VersionNumber,
                ConfigVersionNumber = configVersions.FirstOrDefault(c => c.Id == s.ConfigurationId)?.DisplayVersion,
                IsActive = s.IsActive,
                Required = s.Required,
                ConfigurationId = s.ConfigurationId,
                StudyWide = s.StudyWide,
                CountryNameList = GetCountryNameList(s.Id, countries, softwareReleaseCountries),
                DeviceTypeNames = GetDeviceNames(s.Id, softwareReleaseDeviceTypes),
                SiteNameList = s.Sites.Any() ? string.Join(", ", s.Sites.Select(c => c.Name).ToArray()) : "",
                AssetTagList = string.Join(", ",
                    devices.Where(d => d.SoftwareReleaseId == s.Id).Select(d => d.AssetTag)),
                AssignedReportedConfigCount = SetAssignReportConfigCount(devices, s),
                AssignedReportedVersionCount = SetAssignReportVersionCount(devices, s)
            }).ToList();
        }

        public async Task<Guid> FindLatestConfigurationVersion(List<Guid> siteIds, List<Guid> countryIds)
        {
            var highestConfiguration = Config.Defaults.ConfigurationVersions.InitialVersion.Id;

            var possibleVersions = new Dictionary<Guid, Version>();

            var studyWideReleases = await _db
                .SoftwareReleases
                .Where(sr => sr.StudyWide && sr.IsActive)
                .ToListAsync();

            var latestStudyWide = studyWideReleases
                .Select(r => new { Release = r, ConfigurationVersion = Version.Parse(r.ConfigurationVersion) })
                .OrderByDescending(r => r.ConfigurationVersion)
                .Select(r => r.Release)
                .FirstOrDefault();

            if (latestStudyWide != null && Version.TryParse(latestStudyWide?.ConfigurationVersion, out var parsedSiteWideVersion))
            {
                possibleVersions[latestStudyWide.ConfigurationId] = parsedSiteWideVersion;
            }

            var siteAssignedReleases = await _db
                .SoftwareReleases
                .Where(sr => sr.Sites.Any(s => siteIds.Contains(s.Id)) && sr.IsActive)
                .ToListAsync();

            var latestSiteAssigned = siteAssignedReleases
                .Select(r => new { Release = r, ConfigurationVersion = Version.Parse(r.ConfigurationVersion) })
                .OrderByDescending(r => r.ConfigurationVersion)
                .Select(r => r.Release)
                .FirstOrDefault();

            if (latestSiteAssigned != null && Version.TryParse(latestSiteAssigned?.ConfigurationVersion, out var parsedSiteVersion))
            {
                possibleVersions[latestSiteAssigned.ConfigurationId] = parsedSiteVersion;
            }

            var countryAssignedReleases = await _db
                .SoftwareReleaseCountry
                .Include(src => src.SoftwareRelease)
                .Where(src => countryIds.Contains(src.CountryId) && src.SoftwareRelease.IsActive)
                .ToListAsync();

            var latestCountryAssigned = countryAssignedReleases
                .Select(src => new { Release = src.SoftwareRelease, ConfigurationVersion = Version.Parse(src.SoftwareRelease.ConfigurationVersion) })
                .OrderByDescending(r => r.ConfigurationVersion)
                .Select(r => r.Release)
                .FirstOrDefault();

            if (latestCountryAssigned != null && Version.TryParse(latestCountryAssigned?.ConfigurationVersion, out var parsedCountryVersion))
            {
                possibleVersions[latestCountryAssigned.ConfigurationId] = parsedCountryVersion;
            }

            var tabletAssignedReleases = await _db
                .SoftwareReleaseDeviceTypes
                .Include(src => src.SoftwareRelease)
                .Where(src => src.DeviceTypeId == DeviceType.Tablet.Id && src.SoftwareRelease.IsActive)
                .ToListAsync();

            var latestTabletAssigned = tabletAssignedReleases
                .Select(srdt => new { Release = srdt.SoftwareRelease, ConfigurationVersion = Version.Parse(srdt.SoftwareRelease.ConfigurationVersion) })
                .OrderByDescending(r => r.ConfigurationVersion)
                .Select(r => r.Release)
                .FirstOrDefault();

            if (latestTabletAssigned != null && Version.TryParse(latestTabletAssigned?.ConfigurationVersion, out var parsedDeviceTypeVersion))
            {
                possibleVersions[latestTabletAssigned.ConfigurationId] = parsedDeviceTypeVersion;
            }

            if (possibleVersions.Any())
            {
                highestConfiguration = possibleVersions
                    .OrderByDescending(pc => pc.Value)
                    .Select(pc => pc.Key)
                    .First();
            }

            return highestConfiguration;
        }

        public Guid GetLatestGlobalConfigurationVersionId()
        {
            var latest = _db.SoftwareReleases
                .Where(s => (s.StudyWide || s.SoftwareReleaseDeviceTypes.Any()) && s.IsActive)
                .Select(s => new { s.ConfigurationId, s.ConfigurationVersion })
                .ToList()
                .OrderByDescending(s => Version.Parse(s.ConfigurationVersion))
                .FirstOrDefault()?.ConfigurationId;

            return latest ?? Config.Defaults.ConfigurationVersions.InitialVersion.Id;
        }

        public async Task<Dictionary<string, string>> GetCountryDictionaryByDeviceType(List<Guid> deviceTypeIds)
        {
            var countries = await GetCountryModelsByDeviceType(deviceTypeIds);

            var countryDictionary = countries
                .ToDictionary(
                    c => c.Id.ToString(),
                    c => c.Name);

            return countryDictionary;
        }

        public async Task<Dictionary<string, string>> GetSiteDictionaryByCountry(List<Guid> deviceTypeIds, List<Guid> countryIds)
        {
            var sites = await GetSitesByCountry(deviceTypeIds, countryIds);

            var siteDictionary = sites
                .ToDictionary(
                    s => s.Id.ToString(),
                    s => s.Name);

            return siteDictionary;
        }

        public async Task UpdateDeviceSoftwareRelease(IEnumerable<Device> devices, SoftwareReleaseDto softwareRelease)
        {
            var ids = devices
                .Select(x => x.Id)
                .ToList();

            var devicesToUpdate = _db.Devices.Where(s => ids.Contains(s.Id));

            foreach (var device in devicesToUpdate)
            {
                device.SoftwareReleaseId = softwareRelease.Id;
            }

            await _db.SaveChangesAsync(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        private async Task<List<CountryModel>> GetCountryModelsByDeviceType(List<Guid> deviceTypeIds)
        {
            var allCountries = await _countryService
                .GetAll();

            if (deviceTypeIds == null || !deviceTypeIds.Any())
            {
                return allCountries;
            }

            if (deviceTypeIds.Contains(DeviceType.Phone.Id))
            {
                deviceTypeIds.Add(DeviceType.BYOD.Id);
            }

            var countryIds = await _db.Devices
                .Where(d => deviceTypeIds.Contains(d.DeviceTypeId))
                .Select(d => d.Site.CountryId)
                .ToListAsync();

            var matchedCountries = allCountries
                .Where(c => countryIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .ToList();

            return matchedCountries;
        }

        private async Task<List<Site>> GetSitesByCountry(List<Guid> deviceTypeIds, List<Guid> countryIds)
        {
            var filteredCountries = await GetCountryModelsByDeviceType(deviceTypeIds);

            var filteredCountryIds = filteredCountries
                .Select(c => c.Id);

            var sitesQuery = _db
                .Sites
                .Where(s => filteredCountryIds.Contains(s.CountryId));

            if (deviceTypeIds != null && deviceTypeIds.Any())
            {
                if (deviceTypeIds.Contains(DeviceType.Phone.Id))
                {
                    deviceTypeIds.Add(DeviceType.BYOD.Id);
                }

                var deviceSiteIds = await _db.Devices
                    .Where(d => deviceTypeIds.Contains(d.DeviceTypeId))
                    .Select(d => d.SiteId)
                    .ToListAsync();

                sitesQuery = sitesQuery
                    .Where(s => deviceSiteIds.Contains(s.Id));
            }

            if (countryIds != null && countryIds.Any())
            {
                sitesQuery = sitesQuery
                    .Where(s => countryIds.Contains(s.CountryId));
            }

            var result = await sitesQuery
                .ToListAsync();

            return result;
        }

        private async Task<List<Device>> GetDevicesBySite(List<Guid> deviceTypeIds, List<Guid> countryIds, List<Guid> siteIds)
        {
            var deviceQuery = _db
                .Devices
                .AsQueryable();

            if (siteIds != null && siteIds.Any())
            {
                deviceQuery = deviceQuery
                    .Where(d => d.SiteId.HasValue && siteIds.Contains(d.SiteId.Value));
            }
            else if (countryIds != null && countryIds.Any())
            {
                var filteredSites = await GetSitesByCountry(deviceTypeIds, countryIds);

                var filteredSiteIds = filteredSites
                    .Select(s => s.Id);

                deviceQuery = deviceQuery
                    .Where(d => d.SiteId.HasValue && filteredSiteIds.Contains(d.SiteId.Value));
            }

            if (deviceTypeIds != null && deviceTypeIds.Any())
            {
                deviceQuery = deviceQuery
                    .Where(d => deviceTypeIds.Contains(d.DeviceTypeId));
            }

            var devices = await deviceQuery
                .ToListAsync();

            return devices;
        }

        private async Task AddSoftwareRelease(SoftwareReleaseDto softwareRelease)
        {
            var configVersions = await _configurationVersionService
                .GetAll();

            var selectedConfigVersion = configVersions
                .First(cv => cv.Id == Guid.Parse(softwareRelease.ConfigVersionNumber));

            var entity = new SoftwareRelease
            {
                Id = Guid.NewGuid(),
                SoftwareVersionId = _db.SoftwareVersions.First(v => v.VersionNumber == softwareRelease.VersionNumber).Id,
                DateCreated = DateTime.Now,
                Name = softwareRelease.Name,
                Required = softwareRelease.Required,
                ConfigurationId = selectedConfigVersion.Id,
                ConfigurationVersion = selectedConfigVersion.ConfigurationVersionNumber,
                SRDVersion = selectedConfigVersion.SrdVersion,
                IsActive = true,
                StudyWide = softwareRelease.StudyWide
            };

            if (!softwareRelease.StudyWide)
            {
                if (softwareRelease.CountryIds != null)
                {
                    var countries = await _countryService.GetAll();
                    foreach (var country in countries.Where(c => softwareRelease.CountryIds.Contains(c.Id)))
                    {
                        var softwareReleaseCountry = new SoftwareReleaseCountry
                        {
                            SoftwareReleaseId = entity.Id,
                            CountryId = country.Id
                        };

                        _db.SoftwareReleaseCountry.Add(softwareReleaseCountry);
                    }
                }

                if (softwareRelease.SiteIds != null)
                {
                    var Sites = _db.Sites.Where(s => softwareRelease.SiteIds.Contains(s.Id));
                    foreach (var site in Sites)
                    {
                        entity.Sites.Add(site);
                    }
                }

                if (softwareRelease.DeviceTypeIds != null)
                {
                    foreach (var deviceTypeId in softwareRelease.DeviceTypeIds)
                    {
                        var releaseDeviceType = new SoftwareReleaseDeviceType
                        {
                            SoftwareReleaseId = entity.Id,
                            DeviceTypeId = deviceTypeId
                        };

                        _db.SoftwareReleaseDeviceTypes.Add(releaseDeviceType);
                    }
                }
            }

            _db.SoftwareReleases.Add(entity);

            await _db.SaveChangesAsync(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            softwareRelease.Id = entity.Id;

            var userSiteIds = YPrimeSession.Instance.CurrentUser.Sites.Select(s => s.Id).ToList();
            var userCountryIds = YPrimeSession.Instance.CurrentUser.Sites.Select(s => s.CountryId).Distinct().ToList();

            YPrimeSession.Instance.ConfigurationId = await FindLatestConfigurationVersion(userSiteIds, userCountryIds);
        }

        private string GetCountryNameList(Guid Id, List<CountryModel> countries, IEnumerable<SoftwareReleaseCountry> softwareReleaseCountries)
        {
            var countryIds = softwareReleaseCountries.Where(sc => sc.SoftwareReleaseId == Id).Select(sc => sc.CountryId);
            var countryList = countryIds.Any() ?
               string.Join(", ", countries.Where(c => countryIds.Contains(c.Id)).Select(c => c.Name).ToArray()) : "";

            return countryList;
        }

        private string GetDeviceNames(Guid id, IEnumerable<SoftwareReleaseDeviceType> softwareReleaseDeviceTypes)
        {
            var deviceTypeIds = softwareReleaseDeviceTypes
                .Where(dt => dt.SoftwareReleaseId == id)
                .Select(dt => dt.DeviceTypeId);

            var deviceTypeList = deviceTypeIds.Any()
                ? string.Join(",", DeviceType.Where<DeviceType>(dt => deviceTypeIds.Contains(dt.Id)).Select(dt => dt.Name))
                : string.Empty;

            return deviceTypeList;
        }

        private string SetAssignReportVersionCount(List<Device> devices, SoftwareRelease softwareRelease)
        {
            var reportedCount = devices.Count(d =>
                d.LastReportedSoftwareVersionId == softwareRelease.SoftwareVersionId &&
                d.SoftwareReleaseId == softwareRelease.Id);

            var result = GetDeviceAssignedCount(devices, softwareRelease, reportedCount);

            return result;
        }

        private string SetAssignReportConfigCount(List<Device> devices, SoftwareRelease softwareRelease)
        {
            var reportedCount = devices.Count(d =>
                d.LastReportedConfigurationId == softwareRelease.ConfigurationId &&
                d.SoftwareReleaseId == softwareRelease.Id);

            var result = GetDeviceAssignedCount(devices, softwareRelease, reportedCount);

            return result;
        }

        private string GetDeviceAssignedCount(List<Device> devices, SoftwareRelease softwareRelease, int reportedCount)
        {
            var assignCount = devices.Count(d =>
                d.SoftwareReleaseId == softwareRelease.Id);

            return $"{assignCount}/{reportedCount}";
        }
    }
}