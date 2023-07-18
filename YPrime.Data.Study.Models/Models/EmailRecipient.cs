using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class EmailRecipient : AuditModel
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required] [StringLength(200)] public string EmailAddress { get; set; }

        public Guid EmailSentId { get; set; }

        public Guid EmailRecipientTypeId { get; set; }
        public virtual EmailSent EmailSent { get; set; }    }
}