using Config.Enums;
using System;


namespace YPrime.eCOA.DTOLibrary.WebBackup
{
    public class WebBackupJwtModel
    {
        public Guid SiteId { get; set; }

        public Guid DeviceId { get; set; }

        public Guid PatientId { get; set; }

        public Guid? CaregiverId { get; set; }

        public string CultureName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Guid? VisitId { get; set; }

        public WebBackupType WebBackupType { get; set; }
    }
}