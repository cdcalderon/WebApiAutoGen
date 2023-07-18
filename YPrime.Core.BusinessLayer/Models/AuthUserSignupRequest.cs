using Newtonsoft.Json;
using System;

namespace YPrime.Core.BusinessLayer.Models
{
    public class AuthUserSignupRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
