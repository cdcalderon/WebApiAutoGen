using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    public class CareGiverTypeModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TranslationKey { get; set; }
    }
}
