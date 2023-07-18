using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class CountryBaseModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
