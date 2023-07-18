namespace YPrime.Web.E2E.Models.Api
{
    public class CheckForUpdatesRequestMap
    {
        public string DeviceType { get; set; }
        public string AssetTag { get; set; }
        public string Site { get; set; }
        public string SoftwareVersion { get; set; }
        public string ConfigurationVersion { get; set; }
    }
}
