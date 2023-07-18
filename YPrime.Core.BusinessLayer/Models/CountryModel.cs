using System;
using System.Collections.Generic;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class CountryModel : CountryBaseModel, IConfigModel
    {
        public string ShortName { get; set; }

        public string Notes { get; set; }

        public bool Use12HourTime { get; set; }

        public Guid? DefaultId { get; set; }

        public List<LanguageModel> Languages { get; set; } = new List<LanguageModel>();

        /// <summary>
        /// If true, means that site Temperatures should be shown in Celsius
        /// </summary>
        public bool UseMetric { get; set; }
    }
}
