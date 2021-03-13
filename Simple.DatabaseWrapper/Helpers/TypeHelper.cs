using System;
using System.Drawing;
using Simple.DatabaseWrapper.TypeReader;

namespace Simple.DatabaseWrapper.Helpers
{
    /// <summary>
    /// Helper class for Types
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Check if a specified type is 'Simple'
        /// </summary>
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

        /// <summary>
        /// Retrieve the value of a property
        /// </summary>
        [Obsolete]
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

        /// <summary>
        /// Retrieve the value of a field or property
        /// </summary>
        public static object ReadParamValue(TypeItemInfo info, object parameters)
        {
            var objVal = info.GetValue(parameters);
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
