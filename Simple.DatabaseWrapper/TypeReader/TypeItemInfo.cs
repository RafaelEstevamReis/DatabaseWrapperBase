using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.DatabaseWrapper.Attributes;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class TypeItemInfo
    {
        private TypeItemInfo() { }

        private FieldInfo fieldInfo;
        private PropertyInfo propertyInfo;

        public ItemType ItemType { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public AttributeInfo[] DBAttributes { get; private set; }

        public bool CanRead => ItemType == ItemType.Field || propertyInfo.CanRead;
        public bool CanWrite => ItemType == ItemType.Field || propertyInfo.CanWrite;

        public bool Is(ColumnAttributes attribute) => DBAttributes.Any(o => o.ColumnAttributes == attribute);
        public bool HasDefaultValue(out object DefaultValue)
        {
            foreach (var attr in DBAttributes)
            {
                if (attr.Attribute is DefaultValueAttribute def)
                {
                    DefaultValue = def;
                    return true;
                }
            }

            DefaultValue = null;
            return false;
        }

        public object GetValue(object Object)
        {
            if (fieldInfo != null) return fieldInfo.GetValue(Object);
            return propertyInfo.GetValue(Object);
        }
        public void SetValue(object Object, object Value)
        {
            if (fieldInfo != null) fieldInfo.SetValue(Object, Value);
            else propertyInfo.SetValue(Object, Value);
        }

        public T GetAttribute<T>(ColumnAttributes attribute)
            where T : Attribute
        {
            return DBAttributes.Where(a => a.ColumnAttributes == attribute)
                               .Select(a => a.Attribute as T)
                               .Where(o => o is not null)
                               .FirstOrDefault();
        }
        public T[] GetAttributes<T>(ColumnAttributes attribute)
            where T : Attribute
        {
            return DBAttributes.Where(a => a.ColumnAttributes == attribute)
                               .Select(a => a.Attribute as T)
                               .Where(o => o is not null)
                               .ToArray();
        }

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
                fieldInfo = f,
                propertyInfo = null,

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
                fieldInfo = null,
                propertyInfo = p,

                Type = p.PropertyType,
                Name = p.Name,
                DBAttributes = getAttributes(p.GetCustomAttributes()).ToArray(),
            };
        }

        private static IEnumerable<AttributeInfo> getAttributes(IEnumerable<Attribute> attributes)
        {
            foreach (var att in attributes)
            {
                // Keys
                if (att is PrimaryKeyAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.PrimaryKey, att);
                }
                else if (att is System.ComponentModel.DataAnnotations.KeyAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.PrimaryKey, att);
                }
                // Nulls
                else if (att is AllowNullAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.AllowNull, att);
                }
                else if (att is NotNullAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.NotNull, att);
                }
                // Other Column' attribute
                else if (att is DefaultValueAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.DefaultValue, att);
                }
                else if (att is UniqueAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Unique, att);
                }
                else if (att is IndexAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Indexed, att);
                }
                else if (att is IgnoreAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Ignore, att);
                }
                else if (att is SizeAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Size, att);
                }
                else if (att is AutoIncrementAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.AutoIncrement, att);
                }
                // Other Attribtes
                else if (att is EnumPolicyAttribute)
                {
                    yield return new AttributeInfo(ColumnAttributes.Other, att);
                }
                else
                {
                    yield return new AttributeInfo(ColumnAttributes.Other, att);
                }
            }
        }

        public override string ToString()
        {
            return $"[{(ItemType == ItemType.Property ? 'P' : 'F')}] {Type.Name} {Name}";
        }
    }
}
