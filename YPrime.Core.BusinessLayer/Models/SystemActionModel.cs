using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class SystemActionModel : IConfigModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBlinded { get; set; }
        public string ActionLocation { get; set; }

        public bool AssociatedToUser { get; set; }

        public override bool Equals(object obj)
        {
            SystemActionModel other = obj as SystemActionModel;
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
