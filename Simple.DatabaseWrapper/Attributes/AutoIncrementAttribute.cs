using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that this column has AutoIncrement
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutoIncrementAttribute : Attribute
    { }
}
