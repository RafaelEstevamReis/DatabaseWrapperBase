using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.DatabaseWrapper.Attributes;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class TypeInfo
    {
        public string TypeName { get; set; }
        public TypeItemInfo[] Items { get; set; }

        public static TypeInfo FromType<T>() => FromType(typeof(T));
        public static TypeInfo FromType(Type type)
        {
            return new TypeInfo()
            {
                 TypeName = type.Name,
                 Items = TypeItemInfo.FromType(type)
            };
        }
    }
}
