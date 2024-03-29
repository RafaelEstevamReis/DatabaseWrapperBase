﻿using System;

namespace Simple.DatabaseWrapper.Interfaces
{
    public interface IColumnMapper : ITableMapper
    {
        ITableMapper ConfigureTable(Action<ITable> Options);
    }
}
