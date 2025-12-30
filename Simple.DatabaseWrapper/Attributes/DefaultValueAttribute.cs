using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify default value for this column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultValueAttribute : Attribute
    {
        /// <summary>
        /// Default value specified
        /// </summary>
        public object DefaultValue { get; }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
