using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class AnalyticsReferenceStudyRole : ModelBase
    {
        [Required]
        [ForeignKey("AnalyticsReference")]
        public Guid AnalyticsReferenceId { get; set; }

        [Required]
        public Guid StudyRoleId { get; set; }

        public virtual AnalyticsReference AnalyticsReference { get; set; }
    }
}