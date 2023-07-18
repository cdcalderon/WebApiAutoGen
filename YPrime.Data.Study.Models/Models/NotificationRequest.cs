using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models.Models
{    
    [Serializable]
    [Table("NotificationRequest")]
    public class NotificationRequest : AuditModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public string AuthenticationHeader { get; set; }

        public string RequestBody { get; set; }

        public int ReponseCode { get; set; }
    }
}