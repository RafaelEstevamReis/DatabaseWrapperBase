using System;
using System.Collections.Generic;

namespace Simple.DatabaseWrapper
{
    public sealed class DataBuffer<T> : IDisposable
    {
        public bool IgnoreNulls { get; set; } = true;
        public int Capacity { get; }
        public Action<IEnumerable<T>> FlushData { get; }

        private List<T> _activeBuffer;

#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _swapLock = new();
        private readonly System.Threading.Lock _flushLock = new();
#else
        private readonly object _swapLock = new();
        private readonly object _flushLock = new();
#endif

        public DataBuffer(int quantity, Action<IEnumerable<T>> flushData)
        {
            Capacity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
            FlushData = flushData ?? throw new ArgumentNullException(nameof(flushData));
            _activeBuffer = new List<T>(Capacity);
        }

        public void Add(T value)
        {
            if (IgnoreNulls && value is null) return;

            List<T> bufferToFlush = null;

            lock (_swapLock)
            {
                _activeBuffer.Add(value);

                if (_activeBuffer.Count >= Capacity)
                {
                    bufferToFlush = _activeBuffer;
                    _activeBuffer = new List<T>(Capacity);
                }
            }

            if (bufferToFlush != null)
            {
                // Lock other flush threads when second buffer fills before this ends
                lock (_flushLock)
                {
                    FlushData(bufferToFlush);
                }
            }
        }

        public void Flush()
        {
            List<T> bufferToFlush = null;

            lock (_swapLock)
            {
                if (_activeBuffer.Count > 0)
                {
                    bufferToFlush = _activeBuffer;
                    _activeBuffer = new List<T>(Capacity);
                }
            }

            if (bufferToFlush != null)
            {
                lock (_flushLock)
                {
                    FlushData(bufferToFlush);
                }
            }
        }

        public void Dispose()
        {
            Flush();
        }
    }
}