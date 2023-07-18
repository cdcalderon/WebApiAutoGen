using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISyncLogRepository
    {
        Task<IEnumerable<SyncLogDto>> GetSyncLogsByDevice(Guid deviceId, bool Descending);
        DateTimeOffset? GetLastSyncDateFromLogsByDevice(string assetTag);
    }
}