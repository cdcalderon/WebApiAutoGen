using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class CountryModelExtension
    {
        public static string GetTimeFormat(this CountryModel country)
        {
            return country.Use12HourTime ? "hh:mm A" : "HH:mm";
        }
    }
}
