using System;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class DataSyncBase : AuditModel
    {
        public DataSyncBase()
        {
            SyncVersion = 1;
        }

        public int SyncVersion { get; set; }

        public bool IsDirty { get; set; }
    }
}
