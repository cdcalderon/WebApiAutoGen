using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Models;

namespace YPrime.Data.Study.Models
{
    [Table("ReportStudyRole")]
    public class ReportStudyRole : AuditModel
    {
        [Column(Order = 0)]
        [Key] 
        public Guid ReportId { get; set; }

        public string ReportName { get; set; }

        [Column(Order = 2)]
        [Key]
        public Guid StudyRoleId { get; set; }
    }
}