using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class EmailSent : AuditModel
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public Guid EmailContentId { get; set; }
        public Guid? StudyUserId { get; set; }
        [ForeignKey("Site")] public Guid? SiteId { get; set; }
        public virtual EmailContent EmailContent { get; set; }
        public virtual StudyUser StudyUser { get; set; }
        public virtual Site Site { get; set; }
        public virtual ICollection<EmailRecipient> EmailRecipients { get; set; }
    }
}