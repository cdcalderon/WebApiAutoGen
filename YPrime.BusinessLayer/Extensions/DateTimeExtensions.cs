using System;

namespace YPrime.BusinessLayer.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ConvertToTimeZone(this DateTimeOffset value, string timeZone)
        {
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch (InvalidTimeZoneException ex)
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }
            catch (TimeZoneNotFoundException ex)
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }

            return TimeZoneInfo.ConvertTime(value, timeZoneInfo);
        }
    }
}