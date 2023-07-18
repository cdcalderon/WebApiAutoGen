using System;

namespace YPrime.eCOA.DTOLibrary.ApiDtos
{
    [Serializable]
    public class AddUpdateDeviceDto
    {
        public Guid DeviceId { get; set; }
        public Guid SiteId { get; set; }
        public string DeviceType { get; set; }
        public string AssetTag { get; set; }
        public string Type { get; set; }
    }
}