using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("MissedVisitReason", Schema = "config")]
    public class MissedVisitReason : AuditModel
    {
        public Guid Id { get; set; }
        public string TranslationKey { get; set; }
    }
}