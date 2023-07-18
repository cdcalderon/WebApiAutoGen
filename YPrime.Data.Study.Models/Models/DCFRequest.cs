using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("DCFRequest")]
    public class DCFRequest : AuditModel
    {
        public int ID { get; set; }

        [Required] [StringLength(10)] public string UserID { get; set; }

        public string TypeOfDataChange { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        [StringLength(255)] public string PatientNumber { get; set; }

        [StringLength(50)] public string TicketNumber { get; set; }

        [ForeignKey("Patient")] public Guid? PatientId { get; set; }

        public virtual Patient Patient { get; set; }
    }
}