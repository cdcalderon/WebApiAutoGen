using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models.Models
{
    public class AdditionalTablesToSync : AuditModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string TableName { get; set; }
        public bool DoSync { get; set; }
    }
}