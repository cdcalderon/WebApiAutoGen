namespace YPrime.Core.BusinessLayer.Models
{
    public class TemperatureConfigModel
    {
        public string MinFahrenheit { get; set; }

        public string MaxFahrenheit { get; set; }

        public string MinCelsius { get; set; }

        public string MaxCelsius { get; set; }

        public bool? PreserveUnits { get; set; }
    }
}