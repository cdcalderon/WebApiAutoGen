using System;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SendEmail(SendingEmailModel email);
        Task<AuthUserSignupResponse> CreateSubjectAsync(Guid patientId, string pin);
        Task ChangePasswordAsync(string authUserId, string pin);
        Task<string> GetTokenAsync(string clientId, string clientSecret, string audience);
        Task<bool> UpdateECOALink(Guid participantId, Guid ecoaRecordId, string BYODAssetTag = null);
    }
}
