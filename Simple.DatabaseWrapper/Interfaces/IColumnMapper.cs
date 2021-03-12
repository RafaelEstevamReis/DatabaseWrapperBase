using System;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface IColumnMapper
    {
        ITableMapper ConfigureTable(Action<ITable> Options);
    }
}
