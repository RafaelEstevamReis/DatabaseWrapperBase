using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that this column is PrimaryKey
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrimaryKeyAttribute : Attribute
    { }
}
