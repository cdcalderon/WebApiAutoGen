using System;
using System.Globalization;
using System.Text.RegularExpressions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class SubjectInformationModelExtensions
    {
        private const string DefaultDateFormat = "dd/MMMM/yyyy";

        private const string MinMaxDateRegex =
            @"^{{(TODAY|\d?\d\/\d?\d\/\d{4})(\+|-)(\d{1,3})d:(\d{1,3})m:(\d{1,3})y}}$";

        private const string TodayValue = "TODAY";
        private const string ParsingCultureInfo = "en-US";

        private static readonly string[] DefaultMinMaxDateFormats =
        {
            "MM/dd/yyyy",
            "M/dd/yyyy",
            "MM/d/yyyy",
            "M/d/yyyy"
        };

        public static DataType GetDataType(this SubjectInformationModel model)
        {
            DataType result = null;

            switch (model)
            {
                case var m when m.ChoiceType?.ToUpper() == DataType.NumberAttribute.DisplayName.ToUpper():
                    result = m.Decimal.HasValue && m.Decimal.Value > 0
                        ? DataType.DecimalNumberAttribute
                        : DataType.NumberAttribute;
                    break;
                case var m when m.ChoiceType?.ToUpper() == DataType.TextAttribute.DisplayName.ToUpper():
                    result = m.DisableNumeric
                        ? DataType.LettersOnlyAttribute
                        : DataType.TextAttribute;
                    break;
                case var m when m.ChoiceType?.ToUpper() == DataType.DateAttribute.DisplayName.ToUpper():
                    result = DataType.DateAttribute;
                    break;
                case var m when m.ChoiceType?.ToUpper() == DataType.ChoicesAttribute.DisplayName.ToUpper():
                    result = DataType.ChoicesAttribute;
                    break;
            }

            return result;
        }

        public static int? GetMininumValue(this SubjectInformationModel model)
        {
            return ParseNumericString(model.Min);
        }

        public static int? GetMaximumValue(this SubjectInformationModel model)
        {
            return ParseNumericString(model.Max);
        }

        public static SubjectInformationModel ParseMinAndMaxDateFormat(this SubjectInformationModel model,
            DateTimeOffset currentSiteTime)
        {           
            model.Min = ParseMinMaxDate(model.Min, DateTimeOffset.MinValue, currentSiteTime).ToString();
            model.Max = ParseMinMaxDate(model.Max, DateTimeOffset.MaxValue, currentSiteTime).ToString();

            return model;
        }

        public static bool HasValidDateFormat(this SubjectInformationModel model, string value)
        {
            var dateFormat = string.IsNullOrWhiteSpace(model.DateFormat)
                ? DefaultDateFormat
                : model.DateFormat;

            var formats = new[]
            {
                dateFormat
            };

            var result = DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedValue);

            return result;
        }

        public static bool HasDateWithinRangeOfMinMax(
            this SubjectInformationModel model,
            string value, 
            DateTimeOffset currentSiteTime)
        {
            var result = false;
            var defaultMinDate = currentSiteTime.AddYears(-100);
            var defaultMaxDate = currentSiteTime.AddYears(100);
            var dateFormat = string.IsNullOrWhiteSpace(model.DateFormat)
                ? DefaultDateFormat
                : model.DateFormat;

            var formats = new[]
            {
                dateFormat
            };

            var minDate = ParseMinMaxDate(model.Min, defaultMinDate, currentSiteTime).Date;
            var maxDate = ParseMinMaxDate(model.Max, defaultMaxDate, currentSiteTime).Date;

            if (DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
            {
                var standardizedDateFormat = dateFormat.ToUpper();
   
                if (standardizedDateFormat.Contains("D"))
                {
                    minDate = new DateTime(minDate.Year, minDate.Month, minDate.Day);
                    maxDate = new DateTime(maxDate.Year, maxDate.Month, maxDate.Day).AddDays(1).AddTicks(-1);
                }
                else if (standardizedDateFormat.Contains("M"))
                {
                    minDate = new DateTime(minDate.Year, minDate.Month, 1);
                    maxDate = new DateTime(maxDate.Year, maxDate.Month, 1).AddMonths(1).AddTicks(-1);
                }
                else
                {
                    minDate = new DateTime(minDate.Year, 1, 1);
                    maxDate = new DateTime(maxDate.Year, 1, 1).AddYears(1).AddTicks(-1);
                }

                result = parsedDate >= minDate && parsedDate <= maxDate;
            }

            return result;
        }

        private static DateTime ParseMinMaxDate(
            string dateString, 
            DateTimeOffset defaultDate,
            DateTimeOffset currentSiteTime)
        {
            var parsedDate = defaultDate.DateTime;

            if (string.IsNullOrEmpty(dateString))
            {
                return parsedDate;
            }

            var match = Regex.Match(dateString, MinMaxDateRegex);

            if (match.Success && match.Groups.Count == 6)
            {
                var groups = match.Groups;

                // Group[1] -> TODAY or 05/10/2001
                var startDateString = groups[1].Value;
                if (startDateString != TodayValue)
                {
                    if (!DateTime.TryParseExact(startDateString, DefaultMinMaxDateFormats,
                        new CultureInfo(ParsingCultureInfo), DateTimeStyles.None, out parsedDate))
                    {
                        return parsedDate;
                    }
                }
                else
                {
                    // This is a TODAY based value, so repopulate parsedDate with the current date/time as a baseline
                    parsedDate = currentSiteTime.DateTime;
                }

                // Group[2] -> "+" or "-"
                var subtractDate = groups[2].Value == "-";

                // Group[5] -> years
                if (int.TryParse(groups[5].Value, out int years) && years != 0)
                {
                    years = subtractDate ? years * -1 : years;
                    parsedDate = parsedDate.AddYears(years);
                }

                // Group[4] -> months
                if (int.TryParse(groups[4].Value, out int months) && months != 0)
                {
                    months = subtractDate ? months * -1 : months;
                    parsedDate = parsedDate.AddMonths(months);
                }

                // Group[3] -> days
                if (int.TryParse(groups[3].Value, out int days) && days != 0)
                {
                    days = subtractDate ? days * -1 : days;
                    parsedDate = parsedDate.AddDays(days);
                }
            }         

            return parsedDate;
        }

        private static int? ParseNumericString(string value)
        {
            int? result = null;

            if (int.TryParse(value, out var parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

    }
}
