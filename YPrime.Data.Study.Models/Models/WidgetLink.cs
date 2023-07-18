using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("WidgetLink")]
    public class WidgetLink : AuditModel
    {
        [Key] public Guid Id { get; set; }

        [ForeignKey("Widget")] public Guid WidgetId { get; set; }

        public string ControllerName { get; set; }
        public string ControllerActionName { get; set; }
        public string TranslationText { get; set; }

        public Widget Widget { get; set; }

        [NotMapped] public string Text { get; set; }
    }
}