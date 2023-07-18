using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class LanguageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}