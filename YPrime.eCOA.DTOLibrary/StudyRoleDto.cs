using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class StudyRoleDto : DtoBase
    {

        public static readonly string LandingPageDelimiter = "~";

        public Guid Id { get; set; }

        [Required] [Display(Name = "Name")] public string ShortName { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Is Blinded")]
        public bool IsBlinded { get; set; }

        [Required]
        [Display(Name = "Auto Assign New Sites")]
        public bool AutoAssignNewSites { get; set; }

        [Display(Name = "Notes")] public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Landing Page")]
        public string LandingPageId { get; set; }
        public string LandingPageName { get; set; }

        public bool Selected { get; set; }

        public ICollection<SystemActionDto> SystemActions { get; set; } = new HashSet<SystemActionDto>();

        public ICollection<ConfirmationTypeDto> Subscriptions { get; set; } = new HashSet<ConfirmationTypeDto>();

        public ICollection<ReportDto> Reports { get; set; } = new HashSet<ReportDto>();
        public string SetDashboardButtonHTML { get; set; }
        public string SetPermissionsButtonHTML { get; set; }
        public string SetSubscriptionsButtonHTML { get; set; }
        public string SetReportsButtonHTML { get; set; }
        public string SetAnalyticsButtonHTML { get; set; }
    }
}