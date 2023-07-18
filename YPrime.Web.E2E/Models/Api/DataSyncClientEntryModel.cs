using System.Collections.Generic;

namespace YPrime.Web.E2E.Models.Api
{
    public class DataSyncClientEntryModel
    {
        public string TableName { get; set; }

        public List<object> Rows { get; set; }
    }
}
