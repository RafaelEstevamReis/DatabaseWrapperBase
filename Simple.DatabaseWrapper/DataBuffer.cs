using System;
using System.Collections.Generic;

namespace Simple.DatabaseWrapper
{
    /// <summary>
    /// Bufferizes data to flush in batches
    /// </summary>
    public class DataBuffer<T> : IDisposable
    {
        /// <summary>
        /// Batch size
        /// </summary>
        public int Quantity { get; }
        /// <summary>
        /// Flush action
        /// </summary>
        public Action<IEnumerable<T>> FlushData { get; }

        List<T> queue;
        /// <summary>
        /// Creates a new buffer instance
        /// </summary>
        /// <param name="quantity">Batch size</param>
        /// <param name="flushData">Flush action</param>
        public DataBuffer(int quantity, Action<IEnumerable<T>> flushData)
        {
            Quantity = quantity;
            FlushData = flushData ?? throw new ArgumentNullException(nameof(flushData));
            queue = new List<T>(quantity + 2);
        }
        /// <summary>
        /// Adds a new value to the buffer
        /// </summary>
        public void Add(T value)
        {
            queue.Add(value);

            if (queue.Count > Quantity)
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
