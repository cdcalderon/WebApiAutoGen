using System;
using System.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Data.Entity;
using YPrime.Core.BusinessLayer.Models;
using System.Collections.Generic;

namespace YPrime.BusinessLayer.Repositories
{
    public class SyncLogRepository : BaseRepository, ISyncLogRepository
    {
        private readonly IConfigurationVersionService _configurationVersionService;
        public SyncLogRepository(IStudyDbContext db, IConfigurationVersionService configurationVersionService) : base(db)
        {
            _configurationVersionService = configurationVersionService;            
        }

        public async Task<IEnumerable<SyncLogDto>> GetSyncLogsByDevice(Guid deviceId, bool Descending)
        {
            List<SyncLogDto> result;

            var versions = await _configurationVersionService.GetAll();            
            var query = _db.SyncLogs.Include(x => x.Device)
                .Where(sl => sl.DeviceId == deviceId)
                .Select(sl =>  new SyncLogDto {                 
                    Id = sl.Id,
                    DeviceId = sl.DeviceId,
                    SoftwareVersionId = sl.SoftwareVersionId,
                    SoftwareVersionNumber = sl.SoftwareVersion.VersionNumber,                   
                    ConfigurationVersionNumber = sl.ConfigurationVersionNumber,
                    SyncAction = sl.SyncAction,
                    SyncDate = sl.SyncDate                
                });

            result = (Descending ? query.OrderByDescending(sl => sl.SyncDate) : query.OrderBy(sl => sl.SyncDate)).ToList();

            foreach (var sync in result)
            {
                sync.ConfigurationVersionNumber = ((sync.ConfigurationVersionNumber ?? "") + "-" + versions.FirstOrDefault(x => x.ConfigurationVersionNumber == sync.ConfigurationVersionNumber)?.SrdVersion ?? "").TrimEnd('-');
            }
            
            return result;
        }

        public DateTimeOffset? GetLastSyncDateFromLogsByDevice(string assetTag)
        {
            const string syncClientDataString = "SyncClientData";

            var dateTime = _db.SyncLogs
                .Where(x => x.Device.AssetTag.ToLower() == assetTag.ToLower() &&
                            x.SyncAction.ToLower() == syncClientDataString.ToLower() && x.SyncSuccess)
                .Max(x => (DateTimeOffset?) x.SyncDate);

            return dateTime;
        }
    }
}