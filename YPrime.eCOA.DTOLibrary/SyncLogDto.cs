using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SyncLogDto : DtoBase
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public Guid? SoftwareVersionId { get; set; }
        public Guid? ConfigurationVersionId { get; set; }
        public string SoftwareVersionNumber { get; set; }
        public string ConfigurationVersionNumber { get; set; }
        public string SyncAction { get; set; }
        public DateTimeOffset SyncDate { get; set; }
    }
}