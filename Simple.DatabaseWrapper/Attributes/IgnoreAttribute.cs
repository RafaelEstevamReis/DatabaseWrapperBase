using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that this column should be ignored
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    { }
}
