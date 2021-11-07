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
        /// Retrieve the value of a field or property
        /// </summary>
        public static object ReadParamValue(TypeItemInfo info, object parameters)
        {
            var objVal = info.GetValue(parameters);

            if (objVal is Guid guid) return guid.ToByteArray();
            if (objVal is Color color) return new byte[] { color.A, color.R, color.G, color.B };
            if (objVal is TimeSpan span) return span.Ticks;

            return objVal;
        }
        /// <summary>
        /// Check if a type is Anonymous: IsClass, IsGenericType, IsCompillerGenerated, has NotPublic flag,
        /// Name starts with '&lt;&gt;' or 'VB$'
        /// </summary>
        public static bool CheckIfAnonymousType(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            // has a namespace equal to null
            // if (type.Namespace == null) return true; - Not found any official source to specify as Absolute TRUE

            // It will be a class
            if (!type.IsClass) return false;

            // It will be internal
            if (type.IsPublic) return false;

            //if (!type.IsGenericType) return false; // [new { }] is not IsGenericType
            // It will have the CompilerGeneratedAttribute applied to it
            bool isCompilerGenerated = Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
            if (!isCompilerGenerated) return false;

            // Name starts With "<>" (C#) or "VB$" (VB.Net)
            return type.Name.StartsWith("<>") || type.Name.StartsWith("VB$");
        }

    }
}
