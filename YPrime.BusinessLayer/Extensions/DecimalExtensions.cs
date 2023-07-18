using System.Linq;

namespace YPrime.BusinessLayer.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToFormattedString(this decimal? value, int? decimalPlaces, bool isEQ5D5)
        {
            if (isEQ5D5)
            {
                return value.HasValue && decimalPlaces.HasValue ? value.Value.ToString($"{string.Join("", Enumerable.Range(1, decimalPlaces.Value).Select(p => "0"))}") : value.ToString();
            }
            else
            {
                return value.HasValue && decimalPlaces.HasValue ? value.Value.ToString($"0.{string.Join("", Enumerable.Range(1, decimalPlaces.Value).Select(p => "0"))}") : value.ToString();
            }
        }
    }
}