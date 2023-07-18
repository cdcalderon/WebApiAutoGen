using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class QuestionSettingsModel : IHasCustomExtensions
    {
        public MarginsModel Margins { get; set; }

        public bool IsSticky { get; set; }

        public bool IsActive { get; set; }

        public bool IsRequired { get; set; }

        public string SummaryDisplayTextKey { get; set; }

        public Guid? EnabledWhen { get; set; }

        public Guid? VisibleWhen { get; set; }

        public string ExportDisplayName { get; set; }

        public int? ExportDisplayOrder { get; set; }

        public string SASLabelType { get; set; }

        public string SASLabel { get; set; }

        public string SDTMCode { get; set; }

        public string SDTMText { get; set; }

        public bool AutoResizeChoices { get; set; }

        public string ChoiceLocation { get; set; }

        public string MinValue { get; set; }

        public string MaxValue { get; set; }

        public string DateTimeFormat { get; set; }

        public string MinValueTextKey { get; set; }

        public string MaxValueTextKey { get; set; }

        public string MidValueTextKey { get; set; }

        public string DecimalValue { get; set; }

        public string Suffix { get; set; }

        public bool LanguageSpecificHotSpotRequired { get; set; }

        public Guid? HotSpotImage { get; set; }

        public List<StudyFileDataModel> LanguageHotSpotImages { get; set; } = new List<StudyFileDataModel>();

        public List<CustomExtensionModel> CustomExtensions { get; set; } = new List<CustomExtensionModel>();

        public bool? EnabledWhenBusinessRuleTrueFalseIndicator { get; set; }

        public bool? VisibleWhenBusinessRuleTrueFalseIndicator { get; set; }

        // Portal Specific Properties

        public string MinValueText { get; set; }

        public string MaxValueText { get; set; }

        public TemperatureConfigModel TemperatureMinMax { get; set; }
    }
}
