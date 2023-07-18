using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IAuthSettings
    {
        string BaseUrl { get; set; }
        string Audience_AAM { get; set; }
        string Audience_SB { get; set; }
        string Audience_eConsent { get; set; }
        string ClientId_M2M { get; set; }
        string ClientSecret_M2M { get; set; } // TODO: move to key vault
        string ClientId_eConsent { get; set; }
        string ClientSecret_eConsent { get; set; }
    }
}
