namespace Simple.DatabaseWrapper.IdGeneration;

internal class GeneratorConstants
{
    public const long EPOCH = 1770000000000L;

    public const int WORKER_BITS = 10;
    public const int SEQUENCE_BITS = 12;

    public const long MAX_WORKER = (1L << WORKER_BITS) - 1;
    public const long MAX_SEQUENCE = (1L << SEQUENCE_BITS) - 1;

    public const int WORKER_SHIFT = SEQUENCE_BITS;
    public const int TS_SHIFT = SEQUENCE_BITS + WORKER_BITS;

    public const long WORKER_ID_MASK = (1L << WORKER_BITS) - 1; // 0b1111111111 = 1023
}
