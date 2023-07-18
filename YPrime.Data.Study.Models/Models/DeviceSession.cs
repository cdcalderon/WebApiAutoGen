using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class DeviceSession : AuditModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }
        public string Token { get; set; }
        public int AvailableCount { get; set; }
        public bool Expired { get; set; }
    }
}