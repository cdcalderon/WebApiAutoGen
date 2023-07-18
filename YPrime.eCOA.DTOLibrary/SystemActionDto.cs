using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SystemActionDto : DtoBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBlinded { get; set; }
        public string ActionLocation { get; set; }

        public bool AssociatedToUser { get; set; }

        public override bool Equals(object obj)
        {
            SystemActionDto other = obj as SystemActionDto;
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}