using Newtonsoft.Json;
using System;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.eCOA.DTOLibrary.Patient
{
    public class BYODEmailModel
    {
        public Guid Id { get; set; }
        public string PatientEmail { get; set; }
        public string Subject { get; set; }
        public string EmailContent { get; set; }
        public Guid EmailContentId { get; set; }
        public WebBackupJwtModel WebBackupJwtModel { get; set; }
        public Guid PatientId { get; set;  }
        public string PatientNumber { get; set; }
        public string PatientPin { get; set; }
        public string SiteNumber { get; set; }
        public string SiteName { get; set; }
        public Guid SiteId { get; set; }
        public string Sponsor { get; set; }
        public string StudyId { get; set; }
        public Guid LanguageId { get; set; }
        public string Body { get; set; }
        public bool IsLanguageRightToLeft { get; set; }

        [JsonIgnore]
        [SkipPropertyCopy] public BYODEmailTranslationsModel ConfirmationEmailTranslations { get; set; }
    }
}