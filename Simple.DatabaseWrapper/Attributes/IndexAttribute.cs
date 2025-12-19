using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that an index must be created for this column
    /// All columns with same indexName will be indexed together
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// Index Name
        /// </summary>
        public string IndexName { get; }
        /// <summary>
        /// Order or column on index
        /// </summary>
        public int ColumnOrder { get; } = 0;

        /// <summary>
        /// Defines an index
        /// </summary>
        public IndexAttribute(string indexName)
        {
            if (FrameworkCompatibility.String_NullOrWhitespace.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException($"'{nameof(indexName)}' cannot be null or whitespace.", nameof(indexName));
            }

            IndexName = indexName;
        }

        /// <summary>
        /// Defines an index
        /// </summary>
        public IndexAttribute(string indexName, int columnOrder)
        {
            if (FrameworkCompatibility.String_NullOrWhitespace.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException($"'{nameof(indexName)}' cannot be null or whitespace.", nameof(indexName));
            }

            IndexName = indexName;
            ColumnOrder = columnOrder;
        }
    }
}
