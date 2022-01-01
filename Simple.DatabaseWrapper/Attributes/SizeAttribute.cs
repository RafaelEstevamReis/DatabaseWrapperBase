using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify size for this column
    /// </summary>
    public class SizeAttribute : Attribute
    {
        /// <summary>
        /// Specify column size
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Specify column precision
        /// </summary>
        public int? Precision { get; set; } = null;

        /// <summary>
        /// Specify size for this column
        /// </summary>
        public SizeAttribute(int size)
        {
            Size = size;
        }
        /// <summary>
        /// Specify size and precision for this column
        /// </summary>
        public SizeAttribute(int size, int precision)
        {
            Size = size;
            Precision = precision;
        }
    }
}
