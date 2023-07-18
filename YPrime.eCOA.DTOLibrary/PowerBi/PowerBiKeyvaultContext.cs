using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.PowerBi
{
    [Serializable]
    public class PowerBiContext : IPowerBiContext
    {
        private const char TermSeparator = '|';

        private HashSet<string> excludedTermMap;

        public string ApiUrl { get; set; }
        
        public string ApplicationId { get; set; }
        
        public string ApplicationSecret { get; set; }
        
        public string AuthorityUrl { get; set; }
        
        public string ResourceUrl { get; set; }
        
        public string TenantId { get; set; }
        
        public Guid WorkspaceId { get; set; }
        
        public string ExcludedTerms { get; set; }
        
        public string ExcludedPrefix { get; set; }

        [JsonIgnore]
        public HashSet<string> ExcludedTermMap 
        { 
            get 
            {
                if (excludedTermMap == null)
                {
                    var splitTerms = ExcludedTerms?.Split(TermSeparator);

                    excludedTermMap = splitTerms != null && splitTerms.Any()
                        ? new HashSet<string>(splitTerms, StringComparer.OrdinalIgnoreCase)
                        : null;
                }

                return excludedTermMap;  
            } 
        }
    }
}