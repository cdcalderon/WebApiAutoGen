using System;
using System.Text.RegularExpressions;

namespace YPrime.BusinessLayer.Extensions
{
    public static class StringExtensions
    {
        private const string NumericRegexPattern = @"\d+";

        public static bool ContainsNumbers(this string value)
        {
            bool result;

            if (string.IsNullOrWhiteSpace(value))
            {
                result = false;
            }
            else
            {
                var regexMatch = Regex.Match(value, NumericRegexPattern);

                result = regexMatch.Success;
            }

            return result;
        }

        public static bool TextLengthIsValid(this string value, int? minCharLength, int? maxCharLength)
        {
            bool result = true;

            if (string.IsNullOrWhiteSpace(value))
            {
                return result;
            }

            if (minCharLength == null)
            {
                minCharLength = value.Length;
            }

            if (maxCharLength == null)
            {
                maxCharLength = value.Length;
            }

            result = value.Length >= minCharLength && value.Length <= maxCharLength;

            return result;
        }

        public static bool HasValidNumericValue(
            this string value,
            int? maximumMantissaDigits = null,
            int? minValue = null,
            int? maxValue = null)
        {
            decimal? minVal = null;
            decimal? maxVal = null;
            if (minValue != null)
            {
                minVal = Convert.ToDecimal(minValue);
            }

            if (maxValue != null)
            {
                maxVal = Convert.ToDecimal(maxValue);
            }
            return HasValidNumericValue(value, maximumMantissaDigits) && 
                ValidateValueInRange(value, minVal, maxVal);
        }

        public static bool HasValidNumericValue(
            this string value,
            int? maximumMantissaDigits)
        { 
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var valueParts = value.Split('.');

            var validCharacteristicValue = false;

            if (int.TryParse(valueParts[0], out var integerValue))
            {
                validCharacteristicValue = true;
            }

            var mantissaValue = 0;
            var mantissaDigits = 0;

            if (valueParts.Length == 1 && maximumMantissaDigits > 0)
            {
                return false;
            }

            if (valueParts.Length > 1)
            {
                if (valueParts[1].Length == 0)
                {
                    return false;
                }

                //trim trailing 0s off of mantissa to get correct 
                valueParts[1] = valueParts[1].TrimEnd("0", maximumMantissaDigits ?? 0);

                valueParts[1] = valueParts[1].Length == 0
                    ? "0"
                    : valueParts[1];

                mantissaDigits = valueParts[1].Length;

                // handle scenario where value might be ".987" without a 'valid' characteristic
                if (int.TryParse(valueParts[1], out mantissaValue) && valueParts[0].Length == 0)
                {
                    validCharacteristicValue = true;
                }
            }

            var validMantissa = ValidateMantissaValue(mantissaValue, mantissaDigits, maximumMantissaDigits ?? 0);

            return validCharacteristicValue && validMantissa;
        }

        public static bool HasValidNumericLength(this string value, decimal? maxValue)
        {
            var result = true;

            if (maxValue.HasValue && !string.IsNullOrEmpty(value))
            {
                var valueParts = value.Split('.');
                var maxVal = maxValue.ToString().Split('.');
                var validNumberLength = maxVal[0].Length;
                result = valueParts[0].Length == validNumberLength;
            }

            return result;
        }

        public static DateTime ToLocalDateTime(this string timezone)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            DateTime localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            return localToday;
        }

        public static bool ValidateValueInRange(this string value, decimal? minimumValue, decimal? maximumValue)
        {
            var result = false;

            if (decimal.TryParse(value, out var parsedValue))
            {
                var aboveMinimum = true;
                var belowMaximum = true;

                if (minimumValue.HasValue)
                {
                    aboveMinimum = parsedValue >= minimumValue.Value;
                }

                if (maximumValue.HasValue)
                {
                    belowMaximum = parsedValue <= maximumValue.Value;
                }

                result = aboveMinimum && belowMaximum;
            }

            return result;
        }

        public static bool ValidateValueInRange(this string value, float? minimumValue, float? maximumValue)
        {
            var result = false;

            if (float.TryParse(value, out var parsedValue))
            {
                var aboveMinimum = true;
                var belowMaximum = true;

                if (minimumValue.HasValue)
                {
                    aboveMinimum = parsedValue >= minimumValue.Value;
                }

                if (maximumValue.HasValue)
                {
                    belowMaximum = parsedValue <= maximumValue.Value;
                }

                result = aboveMinimum && belowMaximum;
            }

            return result;
        }


        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        public static string TrimEnd(this string source, string value, int minCharacters)
        {
            while (source.EndsWith(value) && source.Length > minCharacters)
            {
                source = source.Remove(source.Length - 1);
            }

            return source;
        }

        public static string StripHTML(this string source)
        {
            return Regex.Replace(source, "<.*?>", String.Empty);
        }

        private static bool ValidateMantissaValue(int mantissaValue, int mantissaDigits, int maximumLength)
        {
            var result = false;

            if (mantissaValue > 0)
            {
                if (maximumLength > 0 && mantissaDigits == maximumLength)
                {
                    var maxMantissaValue = (int)Math.Pow(10, maximumLength) - 1;

                    if (mantissaValue <= maxMantissaValue)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }
    }
}