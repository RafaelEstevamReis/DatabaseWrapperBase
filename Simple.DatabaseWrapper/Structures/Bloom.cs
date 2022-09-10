using System;
using System.Collections;

namespace Simple.DatabaseWrapper.Structures
{
    public class Bloom<T>
    {
        public IHashFunction Primary { get; private set; }
        public IHashFunction Secondary { get; private set; }
        public BitArray Array { get; private set; }

        /// <summary>
        /// Creates a simple bloom filter implementation using BloomHash_GetHashCode as second hash
        /// </summary>
        public Bloom(IHashFunction primary, int arraySize)
            : this(primary, new BloomHash_GetHashCode(), arraySize)
        { }
        /// <summary>
        /// Creates a simple bloom filter implementation
        /// </summary>
        public Bloom(IHashFunction primary, IHashFunction secondary, int arraySize)
            : this(primary, secondary, new BitArray(arraySize))
        { }
        /// <summary>
        /// Creates a simple bloom filter implementation
        /// </summary>
        public Bloom(IHashFunction primary, IHashFunction secondary, BitArray array)
        {
            Primary = primary;
            Secondary = secondary;
            Array = array;
        }

        public void Add(T item)
        {
            int len = Array.Length;

            int h1b = getIndex(Primary, item, len);
            Array.Set(h1b, true);

            // second hash
            int h2b = getIndex(Secondary, item, len);
            Array.Set(h2b, true);
        }
        public bool Containts(T item)
        {
            int len = Array.Length;

            int h1b = getIndex(Primary, item, len);
            if (!Array.Get(h1b)) return false;

            // second hash
            int h2b = getIndex(Secondary, item, len);
            if (!Array.Get(h2b)) return false;

            return true;
        }

        private static int getIndex(IHashFunction hash, object item, int len)
        {
            int h1 = hash.ComputeHash(item);
            return Math.Abs(h1) % len;
        }

    }
}
