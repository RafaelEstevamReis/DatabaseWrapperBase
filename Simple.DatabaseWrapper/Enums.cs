namespace Simple.DatabaseWrapper
{
    public enum ItemType
    {
        Property,
        Field,
    }
    public enum ColumnAttributes
    {
        AllowNull,
        DefaultValue,
        AutoIncrement,
        Size,
        NotNull,
        PrimaryKey,
        Unique,

        Ignore,
    }
}
