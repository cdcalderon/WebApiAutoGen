using Newtonsoft.Json;

namespace YPrime.Core.BusinessLayer.Models
{
    public class AuthUserSignupResponse
    { 
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
