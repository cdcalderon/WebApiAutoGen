using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class LanguageModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string CultureName { get; set; }

        public bool IsRightToLeft { get; set; }

        public bool IsDefault { get; set; }

        public bool TranslationApproved { get; set; }

        public string Notes { get; set; }
    }
}
