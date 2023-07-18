using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("EmailContent", Schema = "config")]
    public class EmailContent : AuditModel
    {
        public EmailContent()
        {
            EmailContentStudyRoles = new HashSet<EmailContentStudyRole>();
        }

        public Guid Id { get; set; }

        public int? PatientStatusTypeId { get; set; }

        public string Name { get; set; }

        public string TranslationKey { get; set; }

        public bool IsBlinded { get; set; }

        public bool IsSiteSpecific { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        public string BodyTemplate { get; set; }

        public string SubjectLineTemplate { get; set; }

        public bool IsEmailSentToPerformingUser { get; set; }

        public bool DisplayOnScreen { get; set; }

        public Guid EmailContentTypeId { get; set; }

        public virtual ICollection<EmailContentStudyRole> EmailContentStudyRoles { get; set; }
    }
}