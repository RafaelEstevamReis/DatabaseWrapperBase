namespace Simple.DatabaseWrapper.Perf;

using BenchmarkDotNet.Attributes;
using Simple.DatabaseWrapper;
using System;
using System.Collections.Generic;

// 8 Bytes na Stack
public struct SmallStruct
{
    public int V1, V2;
}

// 128 Bytes na Stack
public struct LargeStruct
{
    public decimal V1, V2, V3, V4, V5, V6, V7, V8;
}

// 8 Bytes de ponteiro + 24 Bytes no Heap (Object Header + 2 ints)
public class SmallClass
{
    public int V1, V2;
}

// 8 Bytes de ponteiro + 144 Bytes no Heap (Object Header + 8 decimais)
public class LargeClass
{
    public decimal V1, V2, V3, V4, V5, V6, V7, V8;
}

// --- BENCHMARK ---

[MemoryDiagnoser]
public class DataBufferTypeBenchmark
{
    private const int TotalItems = 1_000_000;
    private const int BatchSize = 10_000;

    // Callbacks vazios para isolarmos APENAS o custo da Memória/CPU no Add e no Buffer
    private readonly Action<IEnumerable<SmallStruct>> _consumeSmallStruct = b => { };
    private readonly Action<IEnumerable<LargeStruct>> _consumeLargeStruct = b => { };
    private readonly Action<IEnumerable<SmallClass>> _consumeSmallClass = b => { };
    private readonly Action<IEnumerable<LargeClass>> _consumeLargeClass = b => { };

    [Benchmark(Description = "Struct Pequena (8 bytes)")]
    public void Add_SmallStruct()
    {
        using var buffer = new DataBuffer<SmallStruct>(BatchSize, _consumeSmallStruct);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(new SmallStruct { V1 = i, V2 = i });
        }
    }

    [Benchmark(Description = "Struct Grande (128 bytes)")]
    public void Add_LargeStruct()
    {
        using var buffer = new DataBuffer<LargeStruct>(BatchSize, _consumeLargeStruct);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(new LargeStruct { V1 = i, V2 = i, V3 = i, V4 = i, V5 = i, V6 = i, V7 = i, V8 = i });
        }
    }

    [Benchmark(Description = "Class Pequena")]
    public void Add_SmallClass()
    {
        using var buffer = new DataBuffer<SmallClass>(BatchSize, _consumeSmallClass);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(new SmallClass { V1 = i, V2 = i });
        }
    }

    [Benchmark(Description = "Class Grande")]
    public void Add_LargeClass()
    {
        using var buffer = new DataBuffer<LargeClass>(BatchSize, _consumeLargeClass);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(new LargeClass { V1 = i, V2 = i, V3 = i, V4 = i, V5 = i, V6 = i, V7 = i, V8 = i });
        }
    }
}