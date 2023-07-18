using System;
using System.Collections.Generic;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class SubjectInformationModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? BusinessRuleId { get; set; }

        public string BusinessRuleName { get; set; }

        public bool? BusinessRuleTrueFalseIndicator { get; set; }

        public string ChoiceType { get; set; }

        public int? Sequence { get; set; }

        public string Suffix { get; set; }

        public string Min { get; set; }

        public string Max { get; set; }

        public string DateFormat { get; set; }

        public bool DisableNumeric { get; set; }

        public int? Decimal { get; set; }

        public List<CountryBaseModel> Countries { get; set; } = new List<CountryBaseModel>();

        public List<ChoiceModel> Choices { get; set; } = new List<ChoiceModel>();
    }
}
