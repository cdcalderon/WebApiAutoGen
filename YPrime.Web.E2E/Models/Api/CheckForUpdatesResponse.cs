using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models.Api
{
    public class CheckForUpdatesResponse
    {
        public string PackagePath { get; set; }
        public string ServerName { get; set; }
        public bool Priority { get; set; }
        public string ConfigCDNUrl { get; set; }
        public bool RunSQL { get; set; }
        public List<string> DeleteFiles { get; set; }
    }
}
