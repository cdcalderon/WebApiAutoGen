using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    public class SendEmailRequest
    {
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public IEnumerable<string> To { get; set; }
        [JsonProperty("cc")]
        public IEnumerable<string> Cc { get; set; }
        [JsonProperty("bcc")]
        public IEnumerable<string> Bcc { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("attachments")]
        public Dictionary<string, byte[]> Attachments { get; set; }
        [JsonProperty("studyId")]
        public Guid StudyId { get; set; }
        [JsonProperty("sponsorId")]
        public Guid SponsorId { get; set; }
        [JsonProperty("environment")]
        public string Environment { get; set; }
    }
}
