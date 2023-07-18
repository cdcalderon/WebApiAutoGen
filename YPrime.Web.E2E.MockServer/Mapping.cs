using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.MockServer
{
    public class Mapping
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MappingFile { get; set; }
        public string ResponseBody { get; set; }
        public string PatternFormat { get; set; }
        public List<string> LookupEndpointForBody { get; set; }
        public string NodeToPullDataFrom { get; set; }

        public Mapping(string mappingFile, string patternFormat = null,
            List<string> lookupEndpointForBody = null,
            string nodeToPullDataFrom = "")
        {
            MappingFile = mappingFile;
            PatternFormat = patternFormat;
            LookupEndpointForBody = lookupEndpointForBody;
            NodeToPullDataFrom = nodeToPullDataFrom;
        }
    }
}
