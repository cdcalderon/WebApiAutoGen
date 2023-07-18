using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("StudyUserWidget")]
    public class StudyUserWidget : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid StudyUserId { get; set; }

        public Guid WidgetId { get; set; }

        public int WidgetPosition { get; set; }

        public virtual StudyUser StudyUser { get; set; }

        public virtual Widget Widget { get; set; }
    }
}