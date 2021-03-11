using System;
using System.Drawing;

namespace Simple.DatabaseWrapper.Helpers
{
    public static class TypeHelper
    {
        public static bool CheckIfSimpleType(this Type typeT)
        {
            if (typeT.IsPrimitive) return true;
            if (typeT == typeof(byte[])) return true;
            if (typeT == typeof(string)) return true;
            if (typeT == typeof(decimal)) return true;
            if (typeT == typeof(DateTime)) return true;
            if (typeT == typeof(TimeSpan)) return true;
            if (typeT == typeof(Guid)) return true;
            // Drawing
            if (typeT == typeof(Color)) return true;

            if (IsNullableSimpleType(typeT)) return true;

            return false;
        }
        private static bool IsNullableSimpleType(Type typeT)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeT);
            return underlyingType != null && CheckIfSimpleType(underlyingType);
        }

        public static object ReadParamValue(System.Reflection.PropertyInfo p, object parameters)
        {
            var objVal = p.GetValue(parameters);
            if (objVal is Color color)
            {
                return new byte[] { color.A, color.R, color.G, color.B };
            }
            if (objVal is TimeSpan span)
            {
                objVal = span.Ticks;
            }

            return objVal;
        }
    }
}
