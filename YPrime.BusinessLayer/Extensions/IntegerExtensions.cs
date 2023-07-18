using System.Linq;

namespace YPrime.BusinessLayer.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToFormattedString(this int? value, int? decimalPlaces)
        {
            return value.HasValue && decimalPlaces.HasValue ? value.Value.ToString($"0.{string.Join("",Enumerable.Range(1, decimalPlaces.Value).Select(p => "0"))}") : value.ToString();
        }
    }
}