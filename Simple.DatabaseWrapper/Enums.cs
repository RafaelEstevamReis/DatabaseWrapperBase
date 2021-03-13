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
        NotNull,
        PrimaryKey,
        Unique,

        Ignore,
    }
}
