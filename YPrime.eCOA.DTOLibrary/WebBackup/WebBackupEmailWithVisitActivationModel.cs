using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.eCOA.DTOLibrary.WebBackup
{
    public class WebBackupEmailWithVisitActivationModel
    {
        public Guid Id { get; set; }
        public string PatientEmail { get; set; }
        public string Subject { get; set; }
        public string EmailContent { get; set; }
        public Guid EmailContentId { get; set; }
        public WebBackupJwtModel WebBackupJwtModel { get; set; }
        public Guid VisitId { get; set; }
        public Guid PatientId { get; set; }
    }
}
