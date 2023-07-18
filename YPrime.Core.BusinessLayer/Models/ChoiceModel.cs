using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class ChoiceModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public string AlternateId { get; set; }
    }
}
