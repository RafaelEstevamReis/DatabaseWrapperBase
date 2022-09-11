using System;
using System.Collections;
using System.Collections.Generic;

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

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                Add(i);
            }
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

        public double GetTruthRatio()
        {
            int count = 0;
            foreach (bool bit in Array)
            {
                if (!bit) continue;
                count++;
            }
            return (double)count / Array.Length;
        }

        public static int BestArraySize(int estimatedCapacity)
            => BestArraySize(estimatedCapacity, BestTheoricalErrorRate(estimatedCapacity));
        public static int BestArraySize(int estimatedCapacity, float desiredErrorRate)
        {
            // Error rate is:
            // (1-e^(-kn/m))^k
            // And
            // k=(m/n)ln(2)

            var powLog2 = Math.Pow(2, Math.Log(2.0));
            var invPowLog2 = 1.0 / powLog2;

            return (int)Math.Ceiling(estimatedCapacity * Math.Log(desiredErrorRate, invPowLog2));
        }
        public static float BestTheoricalErrorRate(int estimatedCapacity)
            => 1.0f / estimatedCapacity;

        private static int getIndex(IHashFunction hash, object item, int len)
        {
            uint h1 = hash.ComputeHash(item);
            return (int)(h1 % len);
        }

    }
}
