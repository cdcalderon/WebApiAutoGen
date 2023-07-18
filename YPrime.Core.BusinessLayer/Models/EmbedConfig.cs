using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    public class EmbedConfig
    {
        public Guid ReportId { get; set; }

        public string EmbedUrl { get; set; }

        public EmbedToken EmbedToken { get; set; }

        public string AccessToken { get; set; }

        public string ErrorMessage { get; set; }

        public string StudyID { get; set; }

        public string SponsorID { get; set; }

        public string[] UserSites { get; set; }
    }
}
