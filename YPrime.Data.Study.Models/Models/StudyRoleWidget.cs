using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("StudyRoleWidget")]
    public class StudyRoleWidget : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid StudyRoleId { get; set; }

        public Guid WidgetId { get; set; }

        public int WidgetPosition { get; set; }

        public virtual Widget Widget { get; set; }
    }
}