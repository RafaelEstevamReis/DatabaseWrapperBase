namespace Simple.DatabaseWrapper.Interfaces
{
    public interface ITableMapper
    {
        IColumnMapper Add<T>() where T : new();
        ITableCommitResult[] Commit();
    }
}
