using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("QuestionInputFieldTypeResult")]
    public class QuestionInputFieldTypeResult : AuditModel
    {
        [Key] public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Guid? ChoiceId { get; set; }

        public int InputFieldTypeResultId { get; set; }

        public bool DisplayInApp { get; set; }

        public bool SaveInPortal { get; set; }

        public virtual InputFieldTypeResult InputFieldTypeResult { get; set; }
    }
}