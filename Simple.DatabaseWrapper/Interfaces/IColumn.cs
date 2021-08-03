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

        string ExportColumnDefinitionAsStatement();
        string ExportAddColumnAsStatement();
    }
}
