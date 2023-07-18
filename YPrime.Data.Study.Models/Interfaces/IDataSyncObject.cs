using System;

namespace YPrime.Data.Study.Models.Interfaces
{
    public interface IDataSyncObject
    {
        Guid Id { get; set; }
        int SyncVersion { get; set; }
        bool IsDirty { get; set; }
    }
}