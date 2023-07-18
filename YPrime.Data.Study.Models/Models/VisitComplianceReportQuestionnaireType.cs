using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Table("VisitComplianceReportQuestionnaireType")]
    public class VisitComplianceReportQuestionnaireType : AuditModel
    {
        [Key] public Guid Id { get; set; }

        [Index("UIX_VisitComplianceReportQuestionnaireType", 1, IsUnique = true)]
        public int VisitId { get; set; }

        [Index("UIX_VisitComplianceReportQuestionnaireType", 2, IsUnique = true)]
        public int QuestionnaireTypeId { get; set; }
    }
}