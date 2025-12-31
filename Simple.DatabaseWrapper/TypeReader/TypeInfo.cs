using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class TypeInfo
    {
        public string TypeName { get; set; }
        public TypeItemInfo[] Items { get; set; }
        public Attribute[] Attributes { get; set; } = [];
        public Type NativeType { get; set; }
        public bool IsAnonymousType { get; set; }

        public IEnumerable<string> GetNames()
        {
            return Items.Where(o => !o.Is(ColumnAttributes.Ignore))
                        .Select(i => i.Name);
        }
        public IEnumerable<string> GetNamesOf(ColumnAttributes attribute)
        {
            return Items.Where(o => o.Is(attribute)).Select(i => i.Name);
        }
        public string GetFirstNameOf(ColumnAttributes attribute) => GetNamesOf(attribute).FirstOrDefault();

        public static TypeInfo FromType<T>() => FromType(typeof(T));
        public static TypeInfo FromType(Type type)
        {
            return new TypeInfo()
            {
                TypeName = type.Name,
                Items = TypeItemInfo.FromType(type),
                Attributes = type.GetCustomAttributes(false).Select(o => (Attribute)o).ToArray(),
                NativeType = type,
                IsAnonymousType = Helpers.TypeHelper.CheckIfAnonymousType(type),
            };
        }
    }
}
