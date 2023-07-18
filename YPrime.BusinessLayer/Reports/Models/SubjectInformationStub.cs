using System;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class SubjectInformationStub : ConfigStub<Guid>
    {
        public string ChoiceType { get; set; }

        public string DateFormat { get; set; }
    }
}
