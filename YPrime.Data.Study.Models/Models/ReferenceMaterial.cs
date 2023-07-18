using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class ReferenceMaterial : AuditModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid StudyUserId { get; set; }

        public Guid ReferenceMaterialTypeId { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset? UpdatedTime { get; set; }

        public virtual StudyUser StudyUser { get; set; }
    }
}