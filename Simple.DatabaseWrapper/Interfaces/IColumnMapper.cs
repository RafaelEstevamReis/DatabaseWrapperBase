using System;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface IColumnMapper : ITableMapper
    {
        [Obsolete($"Use {nameof(EditTable)} instead", error: true)]
        ITableMapper ConfigureTable(Action<ITable> Options);
        ITableMapper EditTable(Action<ITable> Options);
    }
}
