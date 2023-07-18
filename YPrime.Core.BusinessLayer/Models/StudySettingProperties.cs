using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class StudySettingProperties
    {
        public string Key { get; set; }

        public string Section { get; set; }

        public string Group { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }
    }
}
