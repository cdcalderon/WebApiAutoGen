using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ExportDto : DtoBase
    {
        public string SiteName { get; set; }
        public string PatientNumber { get; set; }
        public string ExportStatus { get; set; }
        public Guid Id { get; set; }

        [DisplayName("Export Name")]
        [Required]
        public string Name { get; set; }

        [DisplayName("User")] [Required] public Guid UserId { get; set; }

        [DisplayName("Site")] public Guid? SiteId { get; set; }

        [DisplayName("Patient")] public Guid? PatientId { get; set; }

        [DisplayName("From")] public DateTimeOffset? DiaryStartDate { get; set; }

        [DisplayName("To")] public DateTimeOffset? DiaryEndDate { get; set; }

        [DisplayName("Export Status")] public int ExportStatusId { get; set; }

        public DateTimeOffset? ScheduledStartTime { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset? StartedTime { get; set; }

        public DateTimeOffset? CompletedTime { get; set; }

        public string DiaryStartDateGridData { get; set; }
        public string DiaryEndDateGridData { get; set; }

        public string CreatedTimeGridData { get; set; }
        public string StartedTimeGridData { get; set; }

        public string CompletedTimeGridData { get; set; }

        public string ActionButtonsHTML { get; set; }
    }
}