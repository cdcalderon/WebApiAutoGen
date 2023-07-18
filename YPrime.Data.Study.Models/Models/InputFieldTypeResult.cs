using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Models;

namespace YPrime.Data.Study.Models
{
    [Table("InputFieldTypeResult")]
    public class InputFieldTypeResult : AuditModel
    {
        [Key] public int Id { get; set; }

        public Guid? PatientAttributeConfigurationDetailId { get; set; }

        public int InputFieldTypeId { get; set; }

        [StringLength(20)] [Required] public string ResultCode { get; set; }

        [StringLength(300)] public string Description { get; set; }

        public bool? IsPatientAttribute { get; set; }

        [StringLength(20)] public string UnitOfMeasure { get; set; }

        public virtual ICollection<QuestionInputFieldTypeResult> QuestionInputFieldTypeResults { get; set; }
    }
}