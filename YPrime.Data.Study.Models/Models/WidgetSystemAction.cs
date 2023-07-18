using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("WidgetSystemAction")]
    public class WidgetSystemAction : AuditModel
    {
        public Guid Id { get; set; }

        [ForeignKey("Widget")] public Guid WidgetId { get; set; }

        [ForeignKey("SystemAction")] public Guid SystemActionId { get; set; }

        public Widget Widget { get; set; }
        public SystemAction SystemAction { get; set; }
    }
}