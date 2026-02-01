#if NET20_OR_GREATER && !NET46_OR_GREATER
using System;

namespace Simple.DatabaseWrapper;

internal static class DateTimeEpoch
{
    public static long ToUnixTimeMilliseconds(this DateTimeOffset utcDateTime)
    {
        // Truncate sub-millisecond precision before offsetting by the Unix Epoch to avoid
        // the last digit being off by one for dates that result in negative Unix times
        long num = utcDateTime.Ticks / 10000;
        return num - 62135596800000L;
    }

    public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
    {
        if (milliseconds < -62135596800000L || milliseconds > 253402300799999L)
        {
            throw new ArgumentOutOfRangeException("milliseconds", string.Format("ArgumentOutOfRange_Range", -62135596800000L, 253402300799999L));
        }

        long ticks = milliseconds * 10000 + 621355968000000000L;
        return new DateTimeOffset(ticks, TimeSpan.Zero);
    }
}
#endif