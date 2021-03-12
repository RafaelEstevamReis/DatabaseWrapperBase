using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.DatabaseWrapper.Attributes;

namespace Simple.DatabaseWrapper.Helpers
{
    public class TypeReader
    {
        public string TypeName { get; set; }
        public TypeItemInfo[] Items { get; set; }

        public static TypeReader FromType<T>() => FromType(typeof(T));
        public static TypeReader FromType(Type type)
        {
            return new TypeReader()
            {
                 TypeName = type.Name,
                 Items = TypeItemInfo.FromType(type)
            };
        }
    }
    public class TypeItemInfo
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public AttributeInfo[] DBAttributes { get; set; }

        public static TypeItemInfo[] FromType(Type type)
        {
            var fields = type.GetFields()
                             .Select(f => FromFields(f));

            return type.GetProperties()
                       .Select(p => FromProp(p))
                       .Union(fields)
                       .ToArray();
        }

        private static TypeItemInfo FromFields(FieldInfo f)
        {
            return new TypeItemInfo()
            {
                ItemType = ItemType.Field,
                Type = f.FieldType,
                Name = f.Name,
                DBAttributes = getAttributes(f.GetCustomAttributes()).ToArray(),
            };
        }
        private static TypeItemInfo FromProp(PropertyInfo p)
        {
            return new TypeItemInfo()
            {
                ItemType = ItemType.Property,
                Type = p.PropertyType,
                Name = p.Name,
                DBAttributes = getAttributes(p.GetCustomAttributes()).ToArray(),
            };
        }
        private static IEnumerable<AttributeInfo> getAttributes(IEnumerable<Attribute> attributes)
        {
            foreach (var att in attributes)
            {
                if (att is AllowNullAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.AllowNull, att);
                }
                else if (att is NotNullAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.NotNull, att);
                }
                else if (att is DefaultValueAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.DefaultValue, att);
                }
                else if (att is PrimaryKeyAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.PrimaryKey, att);
                }
                else if (att is System.ComponentModel.DataAnnotations.KeyAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.PrimaryKey, att);
                }
                else if (att is UniqueAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Unique, att);
                }
            }
        }

    }
    public class AttributeInfo
    {
        public AttributeInfo(ColumnAttributes columnAttributes, Attribute attribute)
        {
            ColumnAttributes = columnAttributes;
            Attribute = attribute;
        }

        public ColumnAttributes  ColumnAttributes { get; set; }
        public Attribute Attribute { get; set; }
    }
}
