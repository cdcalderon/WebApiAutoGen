namespace YPrime.BusinessLayer.Helpers
{
    public interface IConfigurationSettings
    {
        string YPrimeEnvironment { get; }
        string YPrimeInventoryEnvironment { get; }
        string StudyApiBaseUrl { get; }
    }
}