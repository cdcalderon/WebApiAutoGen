using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Auth.Data.Models.JSON;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.BaseClasses;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IConfirmationRepository
    {
        Task<Dictionary<Guid, string>> GetUsersSubscribedToConfirmation(Guid emailTypeId, Guid? SiteId);
        ConfirmationTypeDto GetConfirmationTypeInfo(Guid emailTypeId);
        EmailSentDto GetSavedConfirmation(Guid id);

        void SaveConfirmation(Guid id, string subject, string body, string name, Guid emailContentId, Guid? studyUserId,
            Guid? siteId, ICollection<EmailRecipient> emailRecipient);

        void SaveConfirmation(Guid id, string subject, string body, string name, Guid emailContentId, Guid? siteId,
            ICollection<EmailRecipient> emailRecipient);

        IOrderedQueryable<SentEmailViewModel> GetSavedConfirmations(Guid userId);

        Task<SendingEmailModel> SendEmail(
            Guid emailTypeId, 
            IDictionary<string, string> data, 
            Guid? userId, 
            Guid? siteId,
            IEnumerable<string> additionalRecipients = null);

        Task<SendingEmailModel> SendEmail(Guid emailTypeId, EmailDto dto, Dictionary<string, string> additionalFields, Guid? user,
            Guid? siteId, IEnumerable<string> additionalRecipients = null);

        Task SendApiEmail(SendingEmailModel email);
        List<ConfirmationTypeDto> GetAllEmailTemplates();
        Task<EmailDtoConfiguration> BuildEmailConfiguration();
        Task<EmailSentDto> Resend(Guid emailId, IEnumerable<string> bccRecipients);

        Task<string> CreateDeltaTable<TSource, TKey>(IEnumerable<TSource> oldValues, IEnumerable<TSource> newValues,
            Func<TSource, TKey> keySelector, Dictionary<string, string> columns, string dateFormat, string cultureCode);

        Dictionary<string, string> GenerateEmailContent(Guid emailTypeId, IDictionary<string, string> data);
    }
}