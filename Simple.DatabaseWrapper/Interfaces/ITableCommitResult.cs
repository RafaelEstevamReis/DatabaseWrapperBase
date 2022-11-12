namespace Simple.DatabaseWrapper.Interfaces
{
    public interface ITableCommitResult
    {
        string TableName { get; }
        bool WasTableCreated { get; }
        string[] ColumnsAdded { get; }
        string[] IndexesAdded { get; }
    }
}
