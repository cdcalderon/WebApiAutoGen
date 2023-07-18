using System;
namespace YPrime.Data.Study.Models
{
    public class DeviceData : AuditModel
    {
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public string Fob { get; set; }

        public virtual Device Device { get; set; }
    }
}