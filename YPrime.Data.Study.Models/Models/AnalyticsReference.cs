using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    public class AnalyticsReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string InternalName { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool? SponsorReport { get; set; }
    }
}
