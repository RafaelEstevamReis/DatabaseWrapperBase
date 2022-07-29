using System;
using System.Collections.Generic;

#if NET6_0 || NET5_0 || NETCOREAPP3_1
using System.Collections.Concurrent;
#endif

namespace Simple.DatabaseWrapper
{
#if NET6_0 || NET5_0 || NETCOREAPP3_1
    /// <summary>
    /// Bufferizes data to flush in batches.
    /// Using ConcurrentBag (Net5 and NetCore3.1)
    /// </summary>
#else
    /// <summary>
    /// Bufferizes data to flush in batches.
    /// Using List+Lock (Ueses ConcurrentBag on Net5 and NetCore3.1)
    /// </summary>
#endif
    public sealed class DataBuffer<T> : IDisposable
    {
        /// <summary>
        /// Ignore Nulls additions
        /// </summary>
        public bool IgnoreNulls { get; set; } = true;

        /// <summary>
        /// Batch size
        /// </summary>
        public int Quantity { get; }
        /// <summary>
        /// Flush action
        /// </summary>
        public Action<IEnumerable<T>> FlushData { get; }

#if NET6_0 || NET5_0 || NETCOREAPP3_1
        readonly ConcurrentBag<T> queue = new ConcurrentBag<T>();
#else
        readonly List<T> queue = new List<T>();
#endif

        /// <summary>
        /// Creates a new buffer instance
        /// </summary>
        /// <param name="quantity">Batch size</param>
        /// <param name="flushData">Flush action</param>
        public DataBuffer(int quantity, Action<IEnumerable<T>> flushData)
        {
            Quantity = quantity;
            FlushData = flushData ?? throw new ArgumentNullException(nameof(flushData));
        }
        /// <summary>
        /// Adds a new value to the buffer
        /// </summary>
        public void Add(T value)
        {
            if (IgnoreNulls && value is null) return;

#if NET6_0 || NET5_0 || NETCOREAPP3_1
            queue.Add(value);
#else
            lock (queue)
            {
                queue.Add(value);
            }
#endif

            if (queue.Count >= Quantity)
            {
                Flush();
            }
        }
        /// <summary>
        /// Flushes the buffer
        /// </summary>
        public void Flush()
        {
            lock (queue)
            {
                var arr = queue.ToArray();
                queue.Clear();
                FlushData(arr);
            }
        }
        /// <summary>
        /// Disposes the object and flushes all remaining data
        /// </summary>
        public void Dispose()
        {
            Flush();
        }
    }
}
