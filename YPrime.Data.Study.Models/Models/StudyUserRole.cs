using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Models;

namespace YPrime.Data.Study.Models
{
    [Table("StudyUserRole")]
    public class StudyUserRole : AuditModel
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column(Order = 1)] public Guid StudyUserId { get; set; }


        [Column(Order = 2)] public Guid StudyRoleId { get; set; }

        [Column(Order = 3)] public Guid SiteId { get; set; }

        [StringLength(150)] public string Notes { get; set; }

        public virtual StudyUser StudyUser { get; set; }

        public virtual Site Site { get; set; }
    }
}