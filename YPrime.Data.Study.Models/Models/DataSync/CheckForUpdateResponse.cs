using System.Collections.Generic;

namespace YPrime.Data.Study.Models.Models.DataSync
{
    public class CheckForUpdateResponse
    {
        public string PackagePath { get; set; }
        public string ServerName { get; set; }
        public bool Priority { get; set; }
        public string ConfigCDNUrl { get; set; }
        public string TranslationCDNUrl { get; set; }
        public bool RunSQL { get; set; }
        public List<string> DeleteFiles { get; set; }
    }
}

