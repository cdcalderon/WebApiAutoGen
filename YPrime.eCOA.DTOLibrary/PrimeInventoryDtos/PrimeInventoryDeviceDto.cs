using System;

namespace YPrime.eCOA.DTOLibrary.PrimeInventoryDtos
{
    public class PrimeInventoryDeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AssetTag { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? WarrantyExpirationDate { get; set; }
        public string IMEI { get; set; }
        public string SerialNumber { get; set; }
        public bool HasConnected { get; set; }
        public int? MaxAssetTagUses { get; set; }
        public int? AssetTagUses { get; set; }
    }
}