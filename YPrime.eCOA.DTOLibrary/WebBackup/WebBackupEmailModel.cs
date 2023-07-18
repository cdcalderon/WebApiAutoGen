using System;

namespace YPrime.eCOA.DTOLibrary.WebBackup
{
    public class WebBackupEmailModel
    {
        public Guid Id { get; set; }
        public string PatientEmail { get; set; }
        public string Subject { get; set; }
        public string EmailContent { get; set; }
        public Guid EmailContentId { get; set; }
        public WebBackupJwtModel WebBackupJwtModel { get; set; }

    }
}