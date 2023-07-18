using System.Configuration;

namespace YPrime.BusinessLayer.Helpers
{
    public class ConfigurationSettings : IConfigurationSettings
    {
        public string YPrimeEnvironment => ConfigurationManager.AppSettings["AppEnvironment"] ?? string.Empty;
        public string YPrimeInventoryEnvironment => ConfigurationManager.AppSettings["AppInventoryEnvironmentName"] ?? string.Empty;
        public string StudyApiBaseUrl => ConfigurationManager.AppSettings["YPrime.StudyAPIBaseURL"] ?? string.Empty;
    }
}