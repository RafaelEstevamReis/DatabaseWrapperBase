using Simple.DatabaseWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.DatabaseWrapper
{
    /// <summary>
    /// Object cloning tool
    /// </summary>
    public class DataClone
    {
#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// Clone an Object using serialization, slow but versatile
        /// This one, uses Net5 JsonSerializer
        /// </summary>
        public static T CopyWithSerialization<T>(object source)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
#else
        /// <summary>
        /// Clone an Object using serialization, slow but versatile
        /// This one, uses old XmlSerializer
        /// </summary>
        public static T CopyWithSerialization<T>(object source)
        {
            using var stream = new System.IO.MemoryStream();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            serializer.Serialize(stream, source);
            stream.Position = 0;
            return (T)serializer.Deserialize(stream);
        }
#endif

        /// <summary>
        /// Map properties from a model to another. Property name is used for matching
        /// </summary>
        public static TOut MapModel<TIn, TOut>(TIn source)
        {
            var tIn = typeof(TIn);
            var tOut = typeof(TOut);

            var inProps = tIn.GetProperties();
            var outProps = tOut.GetProperties();

            var newOut = (TOut)Activator.CreateInstance(tOut);

            foreach (var currProp in inProps)
            {
                if (!currProp.CanRead) continue;

                var destProp = outProps.Where(f => f.Name == currProp.Name) // NET20 does not support predicate on firstOrDefault
                                       .FirstOrDefault();
                if (destProp == null) continue;
                if (!destProp.CanWrite) continue;

                var newValue = processValue(currProp.PropertyType,
                                            destProp.PropertyType,
                                            currProp.GetValue(source));

                destProp.SetValue(newOut, newValue);
            }

            return newOut;
        }
        private static object processValue(Type srcType, Type dstType, object val)
        {
            if (val == null) return null;
            // direct copy/reference
            if (srcType == dstType) return val;
            // different type
            if (isPrimitiveOrString(srcType) && isPrimitiveOrString(dstType))
            {
                // try convert
                try
                {
                    return Convert.ChangeType(val, dstType);
                }
                catch { }
            }

            return val;
        }
        private static bool isPrimitiveOrString(Type type)
            => type.IsPrimitive || type == typeof(string);

        /// <summary>
        /// Creates a recursive copy of an object
        /// </summary>
        public static T CopyObject<T>(T original)
        {
            return (T)copyObject(original);
        }
        /// <summary>
        /// Creates a recursive copy of an object
        /// </summary>
        public static object CopyObject(object original)
            => copyObject(original);

        private static object copyObject(object original)
        {
            if (original == null) return null;
            var type = original.GetType();
            // a simple type can be returned that will be copied
            if (Helpers.TypeHelper.CheckIfSimpleType(type)) return original;

            // first, call the object's shallowCopy
            var copyMethod = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            var newCopy = copyMethod.Invoke(original, null);
            // then copy fields
            foreach (var field in type.GetFields())
            {
                // skip already copied simple types
                if (Helpers.TypeHelper.CheckIfSimpleType(field.FieldType)) continue;
                // copy
                var newValue = copyObject(field.GetValue(original));
                field.SetValue(newCopy, newValue);
            }

            // is array?
            if (type.IsArray)
            {
                var arrayType = type.GetElementType();
                if (!Helpers.TypeHelper.CheckIfSimpleType(arrayType))
                {
                    var elements = copyArray((Array)newCopy);
                    newCopy = elements.ToArray();
                }
            }
            // collections as List<T>
            // ??

            return newCopy;
        }
        private static IEnumerable<object> copyArray(Array arr)
        {
            foreach (var e in arr)
            {
                yield return copyObject(e);
            }
        }

    }
}
