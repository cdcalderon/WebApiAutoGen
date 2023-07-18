using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class CorrectionTypeModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasConfiguration { get; set; }
    }
}
