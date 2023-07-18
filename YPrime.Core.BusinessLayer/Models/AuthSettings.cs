using System;
using System.Collections.Generic;
using System.Text;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    public class AuthSettings : IAuthSettings
    {
        public string BaseUrl { get; set; }
        public string Audience_AAM { get; set; }
        public string Audience_SB { get; set; }
        public string Audience_eConsent { get; set; }
        public string ClientId_M2M { get; set; }
        public string ClientSecret_M2M { get; set; }
        public string ClientId_eConsent { get; set; }
        public string ClientSecret_eConsent { get; set; }
    }
}
