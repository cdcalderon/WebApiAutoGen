using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("SecurityQuestion")]
    public class SecurityQuestion : AuditModel
    {
        public Guid Id { get; set; }

        [StringLength(255)] public string TranslationKey { get; set; }
    }
}