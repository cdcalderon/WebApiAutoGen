using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YPrime.eCOA.DTOLibrary.Utils
{
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("memberExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("property");

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException("static method");

            return memberExpression.Member.Name;
        }

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
            var setMethod = propertyInfo.GetSetMethod();
            if (propertyInfo != null && setMethod != null && CanChangeType(value, propertyInfo.PropertyType))
            {
                var canBeNull = propertyInfo.PropertyType.IsValueType ||
                                (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null);

                if (value == null && canBeNull)
                {
                    propertyInfo.SetValue(obj, value, null);
                }
                else
                {
                    propertyInfo.SetValue(obj,
                        Convert.ChangeType(value,
                            Nullable.GetUnderlyingType(propertyInfo.PropertyType) ??
                            propertyInfo.PropertyType), //account for nullable types
                        null);
                }
            }
        }

        public static bool CanChangeType(object value, Type conversionType)
        {
            var result = true;
            var includeList = new List<string>
            {
                "DateTimeOffset"
            };

            //TODO: Need to clean this up
            if (conversionType != Guid.NewGuid().GetType() && conversionType != typeof(Guid?))
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

                ////if (result && convertible == null && conversionType.Name != "DateTimeOffset" && !conversionType.IsClass)
                if (result && (!includeList.Contains(conversionType.Name) &&
                               !includeList.Contains(Nullable.GetUnderlyingType(conversionType)?.Name)) &&
                    !conversionType.IsClass)
                {
                    if (convertible == null)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public static void CopyPropertiesFromObject(this object obj, object source, bool deepCopy = true)
        {
            var objectTypeName = source.GetType().Name;
            //NOTE: use this to debug the code
#if DEBUG
            Debug.WriteLine("--------------------------");
            Debug.WriteLine("**" + objectTypeName);
#endif
            //loop the dto properties
            obj.GetType().GetProperties().ToList().ForEach(prop =>
            {
                //this handles issues with reflection
                var hasSkipPropertyCopy = Attribute.IsDefined(prop, typeof(SkipPropertyCopyAttribute));
                if (!hasSkipPropertyCopy && source.HasProperty(prop.Name))
                {
                    var value = source.GetPropertyValue(prop.Name);
                    var targetValue = obj.GetPropertyValue(prop.Name);
                    var targetPropertyType = obj.GetPropertyType(prop.Name);
                    var isList = value is IEnumerable<object>;
#if DEBUG
                    Debug.WriteLine("--(" + objectTypeName + ")" + prop.Name + "  =  " + value);
#endif
                    if (value != null && (targetPropertyType.GetConstructor(Type.EmptyTypes) != null || isList))
                    {
                        if (isList)
                        {
                            var objType = obj.GetPropertyType(prop.Name);
                            var objValue = obj.GetPropertyValue(prop.Name);
                            var underlyingObjType = objType.GetGenericArguments()[0];

                            //create the list object
                            if (targetValue == null)
                            {
                                if (!objType.IsInterface)
                                {
                                    //instantiate the array
                                    var newList = Activator.CreateInstance(targetPropertyType);
                                    obj.SetPropertyValue(prop.Name, newList);
                                    targetValue = newList;
                                }
                                //left this for reference to load an interfaced list
                                //else
                                //{
                                //    var listType = typeof(List<>);
                                //    var constructedListType = listType.MakeGenericType(underlyingObjType);
                                //    var newInterfaceList = Activator.CreateInstance(constructedListType);

                                //    foreach (var item in (IEnumerable)value)
                                //    {
                                //        var newItem = Activator.CreateInstance(underlyingObjType);
                                //        newItem.CopyPropertiesFromObject(item);
                                //        //obj.SetPropertyValue(prop.Name, newItem);                               
                                //        newInterfaceList.GetType().GetMethod("Add").Invoke(targetValue, new[] { newItem }); //Add
                                //    }

                                //    obj.SetPropertyValue(prop.Name, newInterfaceList);
                                //}
                            }
                            else
                            {
                                if (!objType.IsInterface)
                                {
                                    obj.GetPropertyType(prop.Name).GetMethod("Clear").Invoke(targetValue, null);
                                }
                            }

                            foreach (var item in (IEnumerable) value)
                            {
                                var newItem = Activator.CreateInstance(underlyingObjType);
                                newItem.CopyPropertiesFromObject(item, deepCopy);
                                //obj.SetPropertyValue(prop.Name, newItem);                               
                                obj.GetPropertyType(prop.Name).GetMethod("Add")
                                    .Invoke(targetValue, new[] {newItem}); //Add
                            }
                        }
                        else
                        {
                            if (deepCopy)
                            {
                                var obj2 = Activator.CreateInstance(targetPropertyType);
                                obj2.CopyPropertiesFromObject(value);
                                obj.SetPropertyValue(prop.Name, obj2);
                            }
                        }
                    }
                    else
                    {
                        obj.SetPropertyValue(prop.Name, value);
                    }
                }
            });
        }

        //private static bool IsComplexProperty (object destinationObject, string propertyName, object value)
        //{
        //    var targetPropertyType = destinationObject.GetPropertyType(propertyName);
        //    if (targetPropertyType.IsClass && targetPropertyType.GetConstructor(Type.EmptyTypes) != null)
        //    {
        //        var obj2 = Activator.CreateInstance(targetPropertyType);
        //        obj2.CopyPropertiesFromObject(value);
        //        destinationObject.SetPropertyValue(propertyName, obj2);
        //    }
        //}
    }
}