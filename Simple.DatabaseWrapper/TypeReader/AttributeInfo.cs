using System;
using Simple.DatabaseWrapper.Attributes;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class AttributeInfo
    {
        public AttributeInfo(ColumnAttributes columnAttributes, Attribute attribute)
        {
            ColumnAttributes = columnAttributes;
            Attribute = attribute;
        }

        public ColumnAttributes  ColumnAttributes { get; set; }
        public Attribute Attribute { get; set; }

        public override string ToString()
        {
            if (Attribute is DefaultValueAttribute def) return $"DefaultValue({def.DefaultValue})";
            return ColumnAttributes.ToString();
        }
    }
}
