namespace Simple.DatabaseWrapper.IdGeneration;

using System;

/// <summary>
/// Implements a Snowflake like Id Generator
/// </summary>
public class IdGenerator
{
    private readonly long workerId;
    private long sequence;
    private object lockObject;

    /// <summary>
    /// Creates a new generator fot a WorkerID
    /// </summary>
    public IdGenerator(long workerId)
    {
        if (workerId <= 0 || workerId > GeneratorConstants.MAX_WORKER)
            throw new ArgumentOutOfRangeException(nameof(workerId), $"Worker ID must be between 1 and {GeneratorConstants.MAX_WORKER}");

        this.workerId = workerId;
        sequence = 0;
        lockObject = new object();
    }

    /// <summary>
    /// Generates a new ID
    /// </summary>
    public Id GetNextId() => next();

    private long next()
    {
        long now = CurrentMillis();
        long thisSequence;
        lock (lockObject)
        {
            sequence = ++sequence % GeneratorConstants.MAX_SEQUENCE;
            thisSequence = sequence;
        }

        return now - GeneratorConstants.EPOCH << GeneratorConstants.TS_SHIFT
             | workerId << GeneratorConstants.WORKER_SHIFT
             | thisSequence;

    }

    private long CurrentMillis()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    internal static long extractTimestampMs(long id)
    {
        // Desloca para a direita removendo worker + sequence
        return (id >> GeneratorConstants.TS_SHIFT) + GeneratorConstants.EPOCH;
    }
}
