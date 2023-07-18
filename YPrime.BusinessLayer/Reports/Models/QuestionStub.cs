using System;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class QuestionStub : ConfigStub<Guid>
    {
        public Guid QuestionnaireId { get; set; }
        public int InputFieldTypeId { get; set; }
        public int Sequence { get; set; }
        public string ExportDisplayName { get; set; }
        public int? ExportDisplayOrder { get; set; }
    }
}
