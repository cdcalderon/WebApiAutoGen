using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    public class KeyvaultClientIdAndSecret
    {
        [JsonProperty("ClientId")]
        public string ClientId { get; set; }

        [JsonProperty("ClientSecret")]
        public string ClientSecret { get; set; }
    }
}
