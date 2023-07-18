using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YPrime.eCOA.DTOLibrary.Extensions
{
    public static class ReflectionExtensions
    {
        //reflection helpers
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static Type GetPropertyType(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).PropertyType;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null && CanChangeType(value, propertyInfo.PropertyType))
            {
                propertyInfo.SetValue(obj,
                                Convert.ChangeType(value, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType), //account for nullable types
                               null);
            }
        }

        public static bool CanChangeType(object value, Type conversionType)
        {
            var result = true;
            //TODO: Need to clean this up
            if (conversionType != Guid.NewGuid().GetType())
            {

                if (conversionType == null)
                {
                    result = false;
                }

                if (result && value == null)
                {
                    result = false;
                }

                IConvertible convertible = value as IConvertible;

                if (result && convertible == null && conversionType.Name != "DateTimeOffset")
                {
                    result = false;
                }
            }
            return result;
        }

        public static void CopyPropertiesFromObject(this object obj, object source)
        {
            //loop the dto properties
            obj.GetType().GetProperties().ToList().ForEach(prop =>
            {
                if (source.HasProperty(prop.Name))
                {
                    var value = source.GetPropertyValue(prop.Name);
                    obj.SetPropertyValue(prop.Name, value);
                }
            });
        }
    }
}
