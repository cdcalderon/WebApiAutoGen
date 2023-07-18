using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("WidgetCount")]
    public class WidgetCount : AuditModel
    {
        [Key] public Guid Id { get; set; }

        [ForeignKey("Widget")] public Guid WidgetId { get; set; }

        public string FunctionName { get; set; }
        public string TableName { get; set; }
        public string TranslationText { get; set; }

        public Widget Widget { get; set; }

        [NotMapped] public string Text { get; set; }
    }
}