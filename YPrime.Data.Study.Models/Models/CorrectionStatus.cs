using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("CorrectionStatus", Schema = "config")]
    public class CorrectionStatus : ModelBase
    {
        public string TranslationKey { get; set; }
        public bool Resolved { get; set; }

        public bool NeedsMoreInformation { get; set; }
    }
}