using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that this column should allow nulls
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowNullAttribute : Attribute
    { }
}
