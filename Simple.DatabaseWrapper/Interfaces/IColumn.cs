using Simple.DatabaseWrapper.TypeReader;
using System;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface IColumn
    {
        string ColumnName { get; }
        Type NativeType { get; }
        bool IsPK { get; set; }
        bool IsAI { get; set; }
        bool IsUnique { get; set; }
        bool AllowNulls { get; set; }
        object DefaultValue { get; set; }
        string[] Indexes { get; set; }
        AttributeInfo[] Attributes { get; set; }

        string ExportColumnDefinitionAsStatement();
        string ExportAddColumnAsStatement();
    }
}
