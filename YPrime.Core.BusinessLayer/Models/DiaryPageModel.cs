using System;
using System.Collections.Generic;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class DiaryPageModel
    {
        public Guid Id { get; set; }

        public Guid QuestionnaireId { get; set; }

        public int Number { get; set; }

        public string InternalName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string FooterText { get; set; }

        public string TranslationKey { get; set; }

        public string PageDisplayText { get; set; }

        public bool IsHandwritingEnabled { get; set; }

        public bool IsScrollForMoreEnabled { get; set; }

        public bool ShouldDisplayOnScreen { get; set; }

        private bool? shouldDisplayQuestionnaireOnScreen;
        public bool? ShouldDisplayQuestionnaireOnScreen
        {
            get
            {
                return shouldDisplayQuestionnaireOnScreen;
            }
            set 
            {
                shouldDisplayQuestionnaireOnScreen = value ?? true;
            }
        }

        public List<QuestionModel> Questions {get; set;}
    }
}
