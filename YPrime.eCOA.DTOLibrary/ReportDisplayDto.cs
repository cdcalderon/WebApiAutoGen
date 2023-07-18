using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ReportDisplayDto : DtoBase
    {
        public string ReportTitle { get; set; }
        public Guid DisplayId { get; set; }
        public string UserId { get; set; }
    }
}