using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class QuestionChoiceModel : IHasCustomExtensions, IChoice
    {
        public Guid QuestionId { get; set; }

        public float ChoiceFloatValue { get; set; }

        public string DisplayText { get; set; }

        public bool? ClearOtherResponses { get; set; }

        public MarginsModel Margins { get; set; }

        public string SDTMValue { get; set; }

        public Guid Id { get; set; }

        public int Sequence { get; set; }

        public Guid? ChoiceHotSpotImage { get; set; }

        public List<StudyFileDataModel> LanguageChoiceHotSpotImages { get; set; } = new List<StudyFileDataModel>();

        public List<CustomExtensionModel> CustomExtensions { get; set; } = new List<CustomExtensionModel>();

        [JsonIgnore]
        public string HtmlFreeDisplayText => string.IsNullOrWhiteSpace(DisplayText) ?
            DisplayText :
            WebUtility.HtmlDecode(Regex.Replace(DisplayText, "<[^>]*(>|$)", string.Empty));

        [JsonIgnore]
        float IChoice.Value
        {
            get => ChoiceFloatValue;
            set => ChoiceFloatValue = value;
        }
    }
}
