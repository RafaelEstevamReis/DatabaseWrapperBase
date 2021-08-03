using System.Linq;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface ITable
    {
        string TableName { get; }
        IColumn[] Columns { get; }

#if !NET20_OR_GREATER && !NETSTANDARD
        IColumn GetColumnByName(string name) 
            => Columns.First(c => string.Equals(c.ColumnName, name, System.StringComparison.InvariantCultureIgnoreCase));
#endif

        string ExportCreateTable();
    }
}
