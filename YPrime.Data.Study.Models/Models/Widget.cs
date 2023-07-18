using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("Widget")]
    public class Widget : AuditModel
    {
        public Widget()
        {
            WidgetSystemActions = new List<WidgetSystemAction>();
            StudyUserWidgets = new HashSet<StudyUserWidget>();
            StudyRoleWidgets = new HashSet<StudyRoleWidget>();
            WidgetLinks = new HashSet<WidgetLink>();
        }

        public Widget(Widget widget, int widgetPosition)
        {
            Id = widget.Id;
            Name = widget.Name;
            TranslationTitleText = widget.TranslationTitleText;
            TranslationButtonText = widget.TranslationButtonText;
            ControllerName = widget.ControllerName;
            ControllerActionName = widget.ControllerActionName;
            WidgetTypeId = widget.WidgetTypeId;
            IconName = widget.IconName;
            ColumnWidth = widget.ColumnWidth;
            ColumnHeight = widget.ColumnHeight;
            ReportId = widget.ReportId;
            StudyRoleWidgets = widget.StudyRoleWidgets;
            StudyUserWidgets = widget.StudyUserWidgets;
            WidgetSystemActions = widget.WidgetSystemActions;
            WidgetCounts = widget.WidgetCounts;
            WidgetLinks = widget.WidgetLinks;
            WidgetPosition = widgetPosition; //All this just to set sequence
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TranslationTitleText { get; set; }
        public string TranslationDescriptionText { get; set; }
        public string TranslationButtonText { get; set; }
        public string ControllerName { get; set; }
        public string ControllerActionName { get; set; }

        public Guid WidgetTypeId { get; set; }

        public int WidgetPosition { get; set; }
        public string IconName { get; set; }
        public int ColumnWidth { get; set; }
        public int ColumnHeight { get; set; }
        public Guid? ReportId { get; set; }

        public virtual ICollection<WidgetSystemAction> WidgetSystemActions { get; set; }

        public virtual ICollection<StudyUserWidget> StudyUserWidgets { get; set; }

        public virtual ICollection<StudyRoleWidget> StudyRoleWidgets { get; set; }

        public virtual ICollection<WidgetCount> WidgetCounts { get; set; }
        public virtual ICollection<WidgetLink> WidgetLinks { get; set; }

        [NotMapped] public string TitleTextDisplay { get; set; }

        [NotMapped] public string DescriptionTextDisplay { get; set; }

        [NotMapped] public string ButtonTextDisplay { get; set; }

        [NotMapped] public List<KeyValuePair<int, string>> Counts { get; set; }
    }
}