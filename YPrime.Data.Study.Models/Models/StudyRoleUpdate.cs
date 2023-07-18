using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("StudyRoleUpdate")]
    public class StudyRoleUpdate : AuditModel
    {
        [Key]
        public Guid StudyRoleId { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}