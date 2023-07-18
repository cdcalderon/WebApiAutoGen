using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class TranslationModel : IConfigModel
    {
        public string Id { get; set; }

        public string LanguageId { get; set; }

        public string EnglishText { get; set; }

        public string LocalText { get; set; }

        public string PatientFacing { get; set; }

        public string Source { get; set; }

        public string Questionnaire { get; set; }

        public string Page { get; set; }

        public string Category { get; set; }

        public bool Locked { get; set; }

        public Guid? PageId { get; set; }

        public Guid? QuestionnaireId { get; set; }

        public Guid LanguageKey { get; set; }
    }
}
