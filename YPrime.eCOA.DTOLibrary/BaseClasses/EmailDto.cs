using System;
using System.Collections.Generic;
using System.Linq;

namespace YPrime.eCOA.DTOLibrary.BaseClasses
{
    [Serializable]
    public abstract class EmailDto : DtoBase
    {
        public Dictionary<string, string> GetKeyValuesFromProperties(EmailDtoConfiguration emailDtoConfiguration)
        {
            var properties = GetType().GetProperties();

            return properties.ToDictionary(property => property.Name, property =>
            {
                var propertyType = property.PropertyType;
                var propertyValue = property.GetValue(this);

                if (propertyValue == null)
                    return null;

                if (propertyType == typeof(DateTime))
                    return ((DateTime) propertyValue).Date.ToString(emailDtoConfiguration.GlobalDateFormat);

                if (propertyType == typeof(DateTime?))
                    return ((DateTime?) propertyValue).Value.Date.ToString(emailDtoConfiguration.GlobalDateFormat);

                if (propertyType == typeof(DateTimeOffset))
                    return ((DateTimeOffset) propertyValue).Date.ToString(emailDtoConfiguration.GlobalDateFormat);

                if (propertyValue == typeof(DateTimeOffset?))
                    return ((DateTimeOffset?) propertyValue).Value.Date.ToString(emailDtoConfiguration
                        .GlobalDateFormat);

                return property.GetValue(this)?.ToString();
            });
        }
    }
}