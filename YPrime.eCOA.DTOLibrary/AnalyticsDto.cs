using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class AnalyticsDto
    {
        public AnalyticsDto()
        {
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public bool AssociatedToUser { get; set; }
    }
}