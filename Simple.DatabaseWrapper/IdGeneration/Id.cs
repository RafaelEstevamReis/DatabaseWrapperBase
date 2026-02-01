namespace Simple.DatabaseWrapper.IdGeneration;

using System;

/// <summary>
/// Represets a 64bit unique identifier
/// </summary>
public readonly struct Id(long id)
{
    private readonly long id = id;
    /// <summary>
    /// Identifier represented value
    /// </summary>
    public long Value { get => id; }

    /// <summary>
    /// Get the WokerId used to generate this Id
    /// </summary>
    public long GetWorkerId()
    {
        return id >> GeneratorConstants.SEQUENCE_BITS & GeneratorConstants.WORKER_ID_MASK;
    }
    /// <summary>
    /// Get the Timestamp used to generate this Id
    /// </summary>
    public DateTimeOffset ExtractDateTimeUtc()
    {
        long timestampMs = IdGenerator.extractTimestampMs(id);

#if NET20_OR_GREATER && !NET46_OR_GREATER
        return DateTimeEpoch.FromUnixTimeMilliseconds(timestampMs);
#else
        return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs);
#endif
    }

    /// <summary>
    /// Implicit conversion for Long
    /// </summary>
    public static implicit operator long(Id id) => id.Value;
    /// <summary>
    /// Implicit conversion from Long
    /// </summary>
    public static implicit operator Id(long id) => new(id);
}