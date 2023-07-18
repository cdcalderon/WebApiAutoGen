using System;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class MarginsModel
    {
        public float MarginTop { get; set; }

        public float MarginLeft { get; set; }

        public float MarginBottom { get; set; }

        public float MarginRight { get; set; }
    }
}
