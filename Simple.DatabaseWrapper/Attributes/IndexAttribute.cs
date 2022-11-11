using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify that an index must be created for this column
    /// All columns with same indexName will be indexed together
    /// </summary>
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// Index Name
        /// </summary>
        public string IndexName { get; }
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

    }
}
