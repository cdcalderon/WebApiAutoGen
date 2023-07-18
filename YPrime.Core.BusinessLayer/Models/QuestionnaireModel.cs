using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class QuestionnaireModel : IConfigModel, IHasCustomExtensions
    {
        public string URL { get; set; }

        public Guid? VisibleBusinessRuleId { get; set; }

        public Guid? EnableBusinessRuleId { get; set; }

        public bool? VisibleBusinessRuleTrueFalseIndicator { get; set; }

        public bool? EnableBusinessRuleTrueFalseIndicator { get; set; }

        public Guid? OpenEndedBusinessRuleId { get; set; }

        public bool? CanBlindedSeeAnswers { get; set; }

        public bool? DisplaySummaryScore { get; set; }

        public bool? IsDropdownNavigationEnabled { get; set; }

        public bool? ValidateQuestionnaireOnSave { get; set; }

        public bool? PromptIncompletePagesOnSave { get; set; }

        public Guid? EnableVisitQuestionnaireId { get; set; }

        public int PreviousDaysEntry { get; set; }

        public int PreviousDaysEdit { get; set; }

        public bool? EnforcePreviousDaysEntry { get; set; }

        public bool? AllowEdit { get; set; }

        public Guid? AdaptestSettingsFileId { get; set; }

        public int? QuestionnaireTypeId { get; set; }

        public string DisplayName { get; set; }

        public Guid Id { get; set; }

        public string InternalName { get; set; }

        public int? Sequence { get; set; }

        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();

        public List<DiaryPageModel> Pages { get; set; } = new List<DiaryPageModel>();

        public List<CustomExtensionModel> CustomExtensions { get; set; } = new List<CustomExtensionModel>();

        public QuestionnaireTakerModel QuestionnaireTaker { get; set; } = new QuestionnaireTakerModel();

    }
}
