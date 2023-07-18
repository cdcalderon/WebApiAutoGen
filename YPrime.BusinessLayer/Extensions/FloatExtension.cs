using System.Linq;

namespace YPrime.BusinessLayer.Extensions
{
    public static class FloatExtensions
    {
        public static string ToFormattedString(this float? value, int? decimalPlaces)
        {
            return value.HasValue && decimalPlaces.HasValue
                ? value.Value.ToString(
                    $"0.{string.Join("", Enumerable.Range(1, decimalPlaces.Value).Select(p => "0"))}")
                : value.ToString();
        }
    }
}