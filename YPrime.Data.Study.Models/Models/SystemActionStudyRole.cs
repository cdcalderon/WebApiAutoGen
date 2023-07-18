using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("SystemActionStudyRole")]
    public class SystemActionStudyRole : AuditModel
    {
        [Key] [Column(Order = 0)] public Guid SystemActionId { get; set; }

        [Key] [Column(Order = 1)] public Guid StudyRoleId { get; set; }

        public virtual SystemAction SystemAction { get; set; }
    }
}