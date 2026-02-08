namespace Simple.DatabaseWrapper.IdGeneration;

using System;

/// <summary>
/// Implements a simple Snowflake like Id Generator
/// Trade-off: Uniqueness and speed it more important than timestamp accuracy
/// </summary>
public class IdGenerator
{
    private readonly long workerId;
    private long sequence;

#if NET9_0_OR_GREATER
    private readonly System.Threading.Lock lockObject;
#else
    private readonly object lockObject;
#endif


    /// <summary>
    /// Creates a new generator fot a WorkerID
    /// </summary>
    public IdGenerator(long workerId)
    {
        if (workerId <= 0 || workerId > GeneratorConstants.MAX_WORKER)
        {
            throw new ArgumentOutOfRangeException(nameof(workerId), $"Worker ID must be between 1 and {GeneratorConstants.MAX_WORKER}");
        }

        this.workerId = workerId;
        sequence = 0;
        lockObject = new();
    }

    /// <summary>
    /// Generates a new ID
    /// </summary>
    public Id GetNextId() => next();

    private long next()
    {
        long thisSequence;

#if NET9_0_OR_GREATER
        using (lockObject.EnterScope())
#else
        lock (lockObject)
#endif
        {
            sequence = ++sequence % GeneratorConstants.MAX_SEQUENCE;
            thisSequence = sequence;
        }

        long now = currentMillis();
        return (now - GeneratorConstants.EPOCH) << GeneratorConstants.TS_SHIFT
             | workerId << GeneratorConstants.WORKER_SHIFT
             | thisSequence;

    }

    private static long currentMillis()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    internal static long extractTimestampMs(long id)
    {
        // Desloca para a direita removendo worker + sequence
        return (id >> GeneratorConstants.TS_SHIFT) + GeneratorConstants.EPOCH;
    }
}
