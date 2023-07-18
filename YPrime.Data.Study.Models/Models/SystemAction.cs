using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("SystemAction")]
    public class SystemAction : AuditModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBlinded { get; set; }
        public string ActionLocation { get; set; }
        public bool DeviceAction { get; set; }

        public virtual ICollection<SystemActionStudyRole> SystemActionStudyRoles { get; set; } =
            new HashSet<SystemActionStudyRole>();
    }
}