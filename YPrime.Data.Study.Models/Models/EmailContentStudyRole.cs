using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("EmailContentStudyRole")]
    public class EmailContentStudyRole : AuditModel
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("EmailContent")]
        public Guid EmailContentId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid StudyRoleId { get; set; }

        [StringLength(150)] public string Notes { get; set; }

        public virtual EmailContent EmailContent { get; set; }
    }
}