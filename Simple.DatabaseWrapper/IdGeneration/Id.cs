namespace Simple.DatabaseWrapper.IdGeneration;

using System;

/// <summary>
/// Represets a 64bit unique identifier
/// </summary>
public readonly struct Id(long id) : IComparable, IComparable<Id>, IEquatable<Id>
{
    /// <summary>
    /// Identifier represented value
    /// </summary>
    public long Value => id;

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
    public DateTimeOffset ExtractTimestamp()
    {
        long timestampMs = IdGenerator.extractTimestampMs(id);

#if NET20_OR_GREATER && !NET46_OR_GREATER
        return DateTimeEpoch.FromUnixTimeMilliseconds(timestampMs);
#else
        return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs);
#endif
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int CompareTo(object obj)
    {
        return id.CompareTo(obj);
    }
    public int CompareTo(Id other)
    {
        return id.CompareTo(other.Value);
    }

    public bool Equals(Id other)
    {
        return id.Equals(other.Value);
    }
    public override bool Equals(object obj)
    {
        if (obj is Id idObj) return id.Equals(idObj.Value);
        return id.Equals(obj);
    }
    public override string ToString()
        => $"Id({Value}, Worker={GetWorkerId()}, {ExtractTimestamp():yyyy-MM-dd HH:mm:ss.fff} UTC)";

    public static implicit operator long(Id id) => id.Value;
    public static implicit operator Id(long id) => new(id);

    public static bool operator ==(Id left, Id right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(Id left, Id right)
    {
        return !(left == right);
    }

    public static bool operator >(Id left, Id right)
    {
        return left.Value > right.Value;
    }
    public static bool operator <(Id left, Id right)
    {
        return left.Value < right.Value;
    }

    public static bool operator <=(Id left, Id right)
    {
        return left.CompareTo(right) <= 0;
    }
    public static bool operator >=(Id left, Id right)
    {
        return left.CompareTo(right) >= 0;
    }
}