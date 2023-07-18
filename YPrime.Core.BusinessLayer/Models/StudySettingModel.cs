using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class StudySettingModel : IConfigModel
    {
        public string Value { get; set; }

        public StudySettingProperties Properties { get; set; }

        public string Key => Properties?.Key;

        public string Group => Properties?.Group;
    }
}
