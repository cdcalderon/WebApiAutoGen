using System;
using System.Collections.Generic;
using YPrime.BusinessRule.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class QuestionModel : IConfigModel, IQuestion
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid QuestionnaireId { get; set; }

        public int QuestionType { get; set; }

        public string DisplayName { get; set; }

        public Guid PageId { get; set; }

        public string QuestionText { get; set; }

        public int Sequence { get; set; }

        public QuestionSettingsModel QuestionSettings { get; set; }

        public List<QuestionChoiceModel> Choices { get; set; } = new List<QuestionChoiceModel>();

        public bool IsRequired => QuestionSettings != null && QuestionSettings.IsRequired;

        public string MinValue => QuestionSettings?.MinValue ?? string.Empty;

        public string MaxValue => QuestionSettings?.MaxValue ?? string.Empty;

        public string EQ5DScoreTextTranslationKey { get; set; }

        public TemperatureConfigModel TemperatureMinMax => QuestionSettings?.TemperatureMinMax;

        public int InputFieldTypeId
        {
            get => QuestionType;
            set => QuestionType = value;
        }
    }
}
