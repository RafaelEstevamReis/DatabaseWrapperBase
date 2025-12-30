using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that this column should not allow nulls
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotNullAttribute : Attribute
    { }
}
