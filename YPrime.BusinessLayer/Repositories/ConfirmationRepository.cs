using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Builders;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.BaseClasses;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.DTOLibrary.ViewModel;
using System.Configuration;
using System.Globalization;
using YPrime.BusinessLayer.Session;
using YPrime.BusinessLayer.Filters;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class ConfirmationRepository : IConfirmationRepository
    {
        private readonly IStudyDbContext _db;
        private readonly ITranslationService _translationService;
        private readonly IStudySettingService _studySettingService;
        private readonly IStudyRoleService _studyRoleService;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IServiceSettings _serviceSettings;
        private readonly IAuthService _authService;

        public ConfirmationRepository(
            IStudyDbContext db, 
            ITranslationService translationService,
            IStudySettingService studySettingService,
            IStudyRoleService studyRoleService,
            ISystemSettingRepository systemSettingRepository,
            IServiceSettings serviceSettings,
            IAuthService authService)
        {
            _db = db;
            _translationService = translationService;
            _studySettingService = studySettingService;
            _studyRoleService = studyRoleService;
            _authService = authService;
            _systemSettingRepository = systemSettingRepository;
            _serviceSettings = serviceSettings;
        }

        public async Task<Dictionary<Guid, string>> GetUsersSubscribedToConfirmation(Guid emailTypeId, Guid? SiteId)
        {
            var studyRoles = await _studyRoleService.GetAll();
            var blindedStudyRoleIds = studyRoles.Where(sr => sr.IsBlinded).Select(r => r.Id);

            var users = _db.EmailContentStudyRoles.Join(_db.StudyUserRoles, x => x.StudyRoleId,
                    y => y.StudyRoleId, (x, y) => new { rbs = x, sur = y })
                .Where(x => x.rbs.EmailContentId == emailTypeId
                            && (!x.rbs.EmailContent.IsSiteSpecific || x.sur.SiteId == SiteId)
                            && (!blindedStudyRoleIds.Contains(x.sur.StudyRoleId) || x.rbs.EmailContent.IsBlinded))
                .Select(x => new { x.sur.StudyUserId, x.sur.StudyUser.Email })
                .Distinct()
                .ToDictionary(x => x.StudyUserId, x => x.Email);

            return users;
        }

        public ConfirmationTypeDto GetConfirmationTypeInfo(Guid emailTypeId)
        {
            var confirmationTypeDto = new ConfirmationTypeDto();
            var emailType = _db.EmailContents.Single(x => x.Id == emailTypeId);
            confirmationTypeDto.CopyPropertiesFromObject(emailType);
            return confirmationTypeDto;
        }

        public EmailSentDto GetSavedConfirmation(Guid id)
        {
            var emailSentFilter = new EmailSentFilter();

            var emailSent = emailSentFilter.Execute(_db.EmailSents).Single(x => x.Id == id);
            var emailSentDto = new EmailSentDto()
            {
                Id = emailSent.Id,
                Body = emailSent.Body,
                Subject = emailSent.Subject,
                DateSent = emailSent.DateSent.Date,
                EmailContentId = emailSent.EmailContentId,
                StudyUserId = emailSent.StudyUserId,
                EmailCOntent = emailSent.EmailContent,
                StudyUser = emailSent.StudyUser,
                Recipients = string.Join(", ", emailSent.EmailRecipients.Select(x => x.EmailAddress))
            };

            return emailSentDto;
        }

        public void SaveConfirmation(Guid id, string subject, string body, string name, Guid emailContentId, Guid? studyUserId, Guid? siteId, ICollection<EmailRecipient> emailRecipient)
        {
            var confirmation = new EmailSent()
            {
                Id = id,
                EmailContentId = emailContentId,
                Body = body,
                StudyUserId = studyUserId,
                Subject = subject,
                DateSent = DateTime.Now,
                SiteId = siteId,
                EmailRecipients = emailRecipient
            };

            _db.EmailSents.Add(confirmation);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public void SaveConfirmation(Guid id, string subject, string body, string name, Guid emailContentId, Guid? siteId, ICollection<EmailRecipient> emailRecipient)
        {
            SaveConfirmation(id, subject, body, name, emailContentId, null, siteId, emailRecipient);
        }

        public async Task SendApiEmail(SendingEmailModel email)
        {
            await _authService.SendEmail(email);
        }

        public IOrderedQueryable<SentEmailViewModel> GetSavedConfirmations(Guid userId)
        {
            var studyUserRole = _db.StudyUserRoles.Where(sur => sur.StudyUserId == userId);
            var studyRoleIds = studyUserRole.Select(x => x.StudyRoleId);

            var subscribedConfirmationTypes = _db.EmailContentStudyRoles.Where(rbs => studyRoleIds.Contains(rbs.StudyRoleId)).Select(x => x.EmailContent);

            var emailSentFilter = new EmailSentFilter();
            var emails = emailSentFilter.Execute(_db.EmailSents).Include(e => e.Site)
                .Where(es => subscribedConfirmationTypes.Contains(es.EmailContent) && (!es.EmailContent.IsSiteSpecific))
                .ToList();

            var sentEmailVms = new List<SentEmailViewModel>();
            foreach (var email in emails)
            {
                var emailContentName = EmailContentType.FirstOrDefault<EmailContentType>(e => e.Id == email.EmailContent.EmailContentTypeId).Name;
                sentEmailVms.Add(new SentEmailViewModel()
                {
                    Id = email.Id,
                    Subject = email.Subject,
                    DateSent = email.DateSent.Date.ToString("dd-MMM-yyyy"),
                    EmailContentType = emailContentName,
                    SiteName = email?.Site?.Name ?? "N/A"
                });
            }

            return sentEmailVms.AsQueryable().OrderByDescending(o => o.DateSentOffset);
        }

        public string ParseEmail(string email)
        {
            string parsed = Regex.Replace(email, @"<span class=""keyword"" contenteditable=""false"">([\s\S]*?)<\/span>", "<=" + @"$1" + "=>", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return parsed;
        }

        public List<ConfirmationTypeDto> GetAllEmailTemplates()
        {
            var result = _db.EmailContents
                .Select(d => new ConfirmationTypeDto()
                {
                    Id = d.Id,
                    Name = d.Name,
                    TranslationKey = d.TranslationKey,
                    IsBlinded = d.IsBlinded,
                    IsSiteSpecific = d.IsSiteSpecific,
                    Notes = d.Notes,
                    LastUpdate = d.LastUpdate,
                    BodyTemplate = d.BodyTemplate,
                    SubjectLineTemplate = d.SubjectLineTemplate,
                    IsEmailSentToPerformingUser = d.IsEmailSentToPerformingUser,
                    EmailContentTypeId = d.EmailContentTypeId
                }).ToList();

            return result;
        }

        public async Task<SendingEmailModel> SendEmail(Guid emailTypeId, EmailDto dto, Dictionary<string, string> additionalFields, Guid? user, Guid? siteId, IEnumerable<string> additionalRecipients = null)
        {
            return await SendEmail(emailTypeId, additionalFields.Union(dto.GetKeyValuesFromProperties(await BuildEmailConfiguration())).ToDictionary(x => x.Key, x => x.Value), user, siteId, additionalRecipients);
        }

        public async Task<SendingEmailModel> SendEmail(Guid emailTypeId, IDictionary<string, string> data, Guid? userId, Guid? siteId, IEnumerable<string> additionalRecipients = null)
        {
            data = data.Union(await GetSharedEmailKeywordValues(userId, siteId)).ToDictionary(x => x.Key, x => x.Value);

            var emailType = GetConfirmationTypeInfo(emailTypeId);

            var body = new StringBuilder(emailType.BodyTemplate);

            body.CompleteTemplate(data);

            var subject = new StringBuilder(emailType.SubjectLineTemplate).Replace("&nbsp;", " ");

            var emailPrependSubjectLine = _systemSettingRepository.GetSystemSettingValue("EmailPrependSubjectLine");

            emailPrependSubjectLine = emailPrependSubjectLine ?? ConfigurationManager.AppSettings["EmailPrependSubjectLine"];

            subject.Insert(0, emailPrependSubjectLine);

            subject.CompleteTemplate(data);

            var recipients = await GetUsersSubscribedToConfirmation(emailTypeId, siteId);
            //Add all recipients emails to bcc field
            var allRecipients = additionalRecipients == null ? new List<string>() : additionalRecipients.ToList();
            allRecipients.AddRange(recipients.Values.ToList());
            allRecipients = allRecipients.Distinct().ToList();

            // can't use session as this may be called from anywhere
            Guid studyId;
            Guid.TryParse(await _studySettingService.GetStringValue("StudyID"), out studyId);

            var emailId = Guid.NewGuid();

            var emailContent = new SendingEmailModel()
            {
                Id = emailId,
                Body = body.ToString(),
                From = "confirmation@yprime.com",
                To = new List<string>(),
                Cc = new List<string>(),
                Bcc = allRecipients,
                BccUsers = recipients.Keys.ToList(),
                ToUsers = new List<Guid>(),
                CcUsers = new List<Guid>(),
                StudyId = studyId,
                Subject = subject.ToString(),
                Attachments = new Dictionary<string, byte[]>(),
                SponsorId = _serviceSettings.SponsorId,
                Environment = _serviceSettings.StudyPortalAppEnvironment
            };

            // attach pdf
            
            if (await _studySettingService.GetStringValue("IsEmailPdfEnabled") == "True")
            {
                var helper = new EmailPdfBuilder(subject.ToString(), body.ToString());
                emailContent.Attachments.Add(helper.GetFileName(), helper.GetBytes());
            }

            var emailRecipients = recipients.Select(r => new EmailRecipient()
            {
                EmailAddress = r.Value,
                EmailRecipientTypeId = EmailRecipientType.Bcc.Id,
                Id = Guid.NewGuid()
            }).ToList();

            SaveConfirmation(emailId, subject.ToString(), body.ToString(), emailType.Name, emailTypeId, siteId, emailRecipients);

            if (recipients.Any())
            {
                await SendApiEmail(emailContent);
            }

            return emailContent;
        }

        public async Task<EmailSentDto> Resend(Guid emailId, IEnumerable<string> bccRecipients)
        {
            var emailSentFilter = new EmailSentFilter();
            var email = emailSentFilter.Execute(_db.EmailSents).Single(e => e.Id == emailId);

            Guid studyId;
            Guid.TryParse(await _studySettingService.GetStringValue("StudyID"), out studyId);

            var emailSentId = Guid.NewGuid();
            var emailContent = new SendingEmailModel()
            {
                Id = Guid.NewGuid(),
                Body = email.Body,
                From = "confirmation@yprime.com",
                To = new List<string>(),
                Cc = new List<string>(),
                Bcc = bccRecipients,
                ToUsers = new List<Guid>(),
                CcUsers = new List<Guid>(),
                StudyId = studyId,
                Subject = email.Subject,
                Attachments = new Dictionary<string, byte[]>(),
                SponsorId = _serviceSettings.SponsorId,
                Environment = _serviceSettings.StudyPortalAppEnvironment
            };

            // attach pdf
            var helper = new EmailPdfBuilder(email.Subject, email.Body);

            emailContent.Attachments.Add(helper.GetFileName(), helper.GetBytes());

            var studyUserRecipients = _db.StudyUsers.Where(s => bccRecipients.Contains(s.Email))
                                .AsEnumerable().Select(x => new EmailRecipient()
                                {
                                    EmailAddress = x.Email,
                                    EmailRecipientTypeId = EmailRecipientType.Bcc.Id,
                                    Id = Guid.NewGuid()
                                }).ToList();

            SaveConfirmation(emailSentId, email.Subject, email.Body, email.EmailContent.Name, email.EmailContentId, null, email.SiteId, studyUserRecipients);

            if (bccRecipients.Any())
            {
                await SendApiEmail(emailContent);
            }

            return GetSavedConfirmation(emailSentId);
        }

        public Dictionary<string, string> GenerateEmailContent(Guid emailTypeId, IDictionary<string, string> data)
        {
            var result = new Dictionary<string, string>();

            var emailType = GetConfirmationTypeInfo(emailTypeId);
            var body = new StringBuilder(emailType.BodyTemplate);
            body.CompleteTemplate(data);
            var subject = new StringBuilder(emailType.SubjectLineTemplate).Replace("&nbsp;", " ");
            subject.CompleteTemplate(data);

            result.Add("SUBJECT", subject.ToString());
            result.Add("BODY", body.ToString());
            return result;
        }

        private async Task<Dictionary<string, string>> GetSharedEmailKeywordValues(Guid? userId, Guid? siteId)
        {
            var user = _db.StudyUsers.SingleOrDefault(x => x.Id == userId);

            var keys = new Dictionary<string, string>()
            {
                { "Sponsor",await _studySettingService.GetStringValue("StudySponsor")},
                { "Protocol", await _studySettingService.GetStringValue("Protocol")},
                { "UserName", user?.UserName },
                { "SystemDate", DateTimeOffset.Now.ToString("dd-MMM-yyyy") },
                { "PatientLabel", (await _translationService.GetByKey("lblPatient")) }
            };

            var siteFilter = new SiteFilter();

            var site = siteFilter.Execute(_db.Sites)
                .SingleOrDefault(x => x.Id == siteId);

            if (site == null)
            {
                keys.Add("LocalDate", "n/a");
            }
            else
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
                var localTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, timeZone);
                keys.Add("LocalDate", localTime.ToString("dd-MMM-yyyy"));
                keys.Add("Investigator", site.Investigator);
                keys.Add("SiteNumber", site.SiteNumber);
            }

            return keys;
        }

        public async Task<EmailDtoConfiguration> BuildEmailConfiguration()
        {
            return new EmailDtoConfiguration()
            {
                GlobalDateFormat = await _studySettingService.GetStringValue("GlobalDateFormat")
            };
        }

        public async Task<string> CreateDeltaTable<TSource, TKey>(IEnumerable<TSource> oldValues, IEnumerable<TSource> newValues, Func<TSource, TKey> keySelector, Dictionary<string, string> columns, string dateFormat, string cultureCode)
        {
            if (!oldValues.Any() && !newValues.Any())
                throw new Exception("Nothing to compare");

            Type operatingType = null;

            if (oldValues.Any())
            {
                operatingType = oldValues.First().GetType();
            }
            else if (newValues.Any())
            {
                operatingType = newValues.First().GetType();
            }

            var properties = operatingType?.GetProperties() ?? Array.Empty<PropertyInfo>();

            var deltaDictionary = oldValues.ToDictionary(item => keySelector(item), item => new Delta() { OldObject = item });

            foreach (var item in newValues)
            {
                var keyValue = keySelector(item);

                if (deltaDictionary.ContainsKey(keyValue))
                {
                    deltaDictionary[keyValue].NewObject = item;

                    foreach (var property in properties)
                    {
                        var oldValue = property.GetValue(deltaDictionary[keyValue].OldObject);
                        var newValue = property.GetValue(deltaDictionary[keyValue].NewObject);

                        // If something set it to true, we want to keep that as true so it knows something changed. 
                        deltaDictionary[keyValue].IsUpdate = deltaDictionary[keyValue].IsUpdate ? deltaDictionary[keyValue].IsUpdate : (oldValue != null && newValue != null &&
                            (oldValue != null && oldValue is IComparable && ((IComparable)oldValue).CompareTo(newValue) != 0));

                        if (deltaDictionary[keyValue].IsUpdate && item.HasProperty("IsRejected") && (bool)item.GetPropertyValue("IsRejected"))
                        {
                            deltaDictionary[keyValue].CustomBackgroundColor = "#FF0000";
                        }
                    }
                }
                else
                {
                    deltaDictionary.Add(keyValue, new Delta() { NewObject = item });
                }
            }

            var yesNo = new YesNoText();          
            yesNo.Yes = (await _translationService.GetByKey("keyYes"));
            yesNo.No = (await _translationService.GetByKey("keyNo"));           

            var propertiesSubset = properties.Where(x => columns.ContainsKey(x.Name)).OrderBy(x => columns.Keys.ToList().IndexOf(x.Name)).ToArray();

            var table = columns.Aggregate("<table border='1'><tr>", (current, column) => current + $"<th style=\"text-align:center;background-color: #314158;color: #ffffff\">{column.Value}</th>");

            table += "</tr>";

            table += string.Join("", deltaDictionary.Where(x => x.Value.IsNew).Select(x => GetDeltaRow(x.Value.NewObject, propertiesSubset, dateFormat, yesNo, "#2ECC71")));

            table += string.Join("", deltaDictionary.Where(x => x.Value.IsRemoved).Select(x => GetDeltaRow(x.Value.OldObject, propertiesSubset, dateFormat, yesNo, "#E74C3C")));
            table += string.Join("", deltaDictionary.Where(x => x.Value.IsUpdate).Select(x => GetDeltaRow(x.Value.NewObject, propertiesSubset, dateFormat, yesNo, string.IsNullOrEmpty(x.Value.CustomBackgroundColor) ? "#3498DB" : x.Value.CustomBackgroundColor)));

            table += string.Join("",
                deltaDictionary.Where(x => !x.Value.IsUpdate && !x.Value.IsNew && !x.Value.IsRemoved)
                    .Select(x => GetDeltaRow(x.Value.NewObject, propertiesSubset, dateFormat, yesNo)));

            table += "</table>";

            return table;
        }

        private string GetDeltaRow(object o, PropertyInfo[] properties, string dateFormat, YesNoText yesNoText, string color = null)
        {
            var row = $"<tr {(color != null ? "style='background-color: " + color + "'" : "")}>";

            row = properties.Aggregate(row, (current, property) => current + $"<td>{FormatData(property.GetValue(o), dateFormat, yesNoText)}</td>");

            row += "</tr>";

            return row;
        }

        private string FormatData(object value, string dateFormat, YesNoText yesNoText)
        {
            if (value == null)
                return string.Empty;

            if (value is DateTimeOffset)
                return ((DateTimeOffset)value).ToString(dateFormat);

            if (value is DateTime)
                return ((DateTime)value).ToString(dateFormat);

            if (value is bool)
                return (bool)value ? yesNoText.Yes : yesNoText.No;

            return value.ToString();
        }

        private class Delta
        {
            public object OldObject { get; set; }

            public object NewObject { get; set; }

            public bool IsRemoved { get { return OldObject != null && NewObject == null; } }

            public bool IsNew { get { return OldObject == null && NewObject != null; } }

            public bool IsUpdate { get; set; }

            public string CustomBackgroundColor { get; set; }
        }

        private class YesNoText
        {
            public string Yes { get; set; }
            public string No { get; set; }
        }
    }
}
