namespace YPrime.Web.E2E.Extensions
{
    public static class StringExtensions
    {
        private const string NullText = "null";

        public static string ConvertNullStringToNull(this string instance)
        {
            var result = instance;

            if (instance?.ToLower()?.Trim() == NullText)
            {
                result = null;
            }

            return result;
        }
    }
}
