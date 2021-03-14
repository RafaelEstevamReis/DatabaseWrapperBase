﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class TypeInfo
    {
        public string TypeName { get; set; }
        public TypeItemInfo[] Items { get; set; }

        public IEnumerable<string> NamesOf(ColumnAttributes attribute)
        {
            return Items.Where(o => o.Is(attribute)).Select(i => i.Name);
        }
        public string FirstNameOf(ColumnAttributes attribute) => NamesOf(attribute).FirstOrDefault();

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
