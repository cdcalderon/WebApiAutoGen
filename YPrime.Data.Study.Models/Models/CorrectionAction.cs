using System;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class CorrectionAction : ModelBase
    {
        public string TranslationKey { get; set; }
        public int DisplayOrder { get; set; }

        public string IconCss { get; set; }
        public string StatusCss { get; set; }
        public bool Actionable { get; set; }
    }
}