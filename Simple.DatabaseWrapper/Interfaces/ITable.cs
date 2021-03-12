namespace Simple.DatabaseWrapper.Interfaces
{
    public interface ITable
    {
        string TableName { get; }
        IColumn[] Columns { get; }

        string ExportCreateTable();
    }
}
