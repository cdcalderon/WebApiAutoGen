using System;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class StudyCustomModelExtensions
    {
        public static int GetIntValue(this StudyCustomModel model)
        {
            var result = int.Parse(model.Value);

            return result;
        }

        public static Guid GetGuidValue(this StudyCustomModel model)
        {
            var result = Guid.Parse(model.Value);

            return result;
        }

        public static bool? GetBoolValue(this StudyCustomModel model)
        {
            var convertedValue = model.Value;

            if (convertedValue == 1.ToString())
            {
                convertedValue = true.ToString(); 
            }
            else if (convertedValue == 0.ToString())
            {
                convertedValue = false.ToString();
            }

            if (bool.TryParse(convertedValue, out bool result))
            {
                return result;
            }

            return null;
        }
    }
}
