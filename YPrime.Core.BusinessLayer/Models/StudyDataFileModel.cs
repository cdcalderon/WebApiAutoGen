using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class StudyFileDataModel
    {
        public string LanguageId { get; set; }

        public Guid ImageId { get; set; }

        public string LanguageName { get; set; }

        public string ImageName { get; set; }
    }
}
