#if NET20 || NET35 || NET40
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Simple.DatabaseWrapper
{
    internal static class SystemReflectionAttributes
    {
        public static IEnumerable<Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element)
        {
            foreach (var a in element.GetCustomAttributes(false))
            {
                yield return (Attribute)a;
            }
        }
        public static object GetValue(this PropertyInfo propertyInfo, object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }
        public static void SetValue(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value, null);
        }
    }
}
#endif