using System.Collections.Generic;

namespace YPrime.Data.Study.Models.Models.DataSync
{
    public class DataSyncResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public List<DataSyncResponseTable> Tables { get; set; } = new List<DataSyncResponseTable>();

        public string Environment { get; set; }

        public string BuilderApiBaseUrl { get; set; }

        public string NotificationSchedulerURL { get; set;  }

        public string NotificationSchedulerApiKey { get; set; }

        public string HmacSigningKey { get; set; }
    }
}