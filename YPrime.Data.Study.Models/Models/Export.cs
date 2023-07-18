using System;

namespace YPrime.Data.Study.Models
{
    public class Export : AuditModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public Guid? SiteId { get; set; }

        public Guid? PatientId { get; set; }

        public int? PatientStatusTypeId { get; set; }

        public int? QuestionnaireTypeId { get; set; }

        public int ExportStatusId { get; set; }

        public DateTimeOffset? DiaryStartDate { get; set; }

        public DateTimeOffset? DiaryEndDate { get; set; }

        public DateTimeOffset? ScheduledStartTime { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset? StartedTime { get; set; }

        public DateTimeOffset? CompletedTime { get; set; }

        public virtual Site Site { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual ExportStatus ExportStatus { get; set; }
    }
}