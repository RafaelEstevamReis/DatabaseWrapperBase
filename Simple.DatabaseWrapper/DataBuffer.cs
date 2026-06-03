namespace Simple.DatabaseWrapper;

using System;
using System.Collections.Generic;

/// <summary>
/// A simple buffer for processing data in batches
/// </summary>
public sealed class DataBuffer<T>(int quantity, Action<IEnumerable<T>> flushData) : IDisposable
{
    /// <summary>
    /// Ignore adding NULL values
    /// </summary>
    public bool IgnoreNulls { get; set; } = true;
    /// <summary>
    /// Buffer capacity
    /// </summary>
    public int Capacity { get; } = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
    /// <summary>
    /// Callback for flusing data
    /// </summary>
    public Action<IEnumerable<T>> FlushData { get; } = flushData ?? throw new ArgumentNullException(nameof(flushData));

    private List<T> _activeBuffer = new(quantity);

#if NET9_0_OR_GREATER
    private readonly System.Threading.Lock _swapLock = new();
    private readonly System.Threading.Lock _flushLock = new();
#else
    private readonly object _swapLock = new();
    private readonly object _flushLock = new();
#endif

    /// <summary>
    /// Adds a new Value to the Buffer
    /// </summary>
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

    /// <summary>
    /// Flushes all current data
    /// </summary>
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

    /// <summary>
    /// Disposes all resources and flushes current data
    /// </summary>
    public void Dispose()
    {
        Flush();
    }
}