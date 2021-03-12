using System;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface IColumn
    {
        string ColumnName { get; }
        Type NativeType { get; }
        bool IsPK { get; }
        bool IsAI { get; }
        bool IsUnique { get; }
        bool AllowNulls { get; }
        object DefaultValue { get; }

        string ExportColumnDefinitionAsStatement();
    }
}
