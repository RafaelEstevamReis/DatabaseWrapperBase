using BenchmarkDotNet.Attributes;
using Simple.DatabaseWrapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[MemoryDiagnoser]
public class DataBufferBenchmark
{
    private const int TotalItems = 500_000; // Meio milhão de itens por teste
    private const int BatchSize = 10_000;   // Flush a cada 10.000 itens

    // Simula o tempo que o SQLite ou uma API leva para processar o lote (I/O Bound)
    // O SpinWait simula carga sem colocar a thread para dormir (o que estragaria o benchmark)
    private readonly Action<IEnumerable<int>> _mockConsumer = batch =>
    {
        Thread.SpinWait(100_000);
    };

    [Benchmark(Baseline = true, Description = "Legacy - Single Thread")]
    public void Legacy_SingleThread()
    {
        using var buffer = new DataBuffer_Legacy<int>(BatchSize, _mockConsumer);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(i);
        }
    }

    [Benchmark(Description = "New - Single Thread")]
    public void New_SingleThread()
    {
        using var buffer = new DataBuffer<int>(BatchSize, _mockConsumer);
        for (int i = 0; i < TotalItems; i++)
        {
            buffer.Add(i);
        }
    }

    [Benchmark(Description = "Legacy - Multi Thread")]
    public void Legacy_MultiThread()
    {
        using var buffer = new DataBuffer_Legacy<int>(BatchSize, _mockConsumer);
        Parallel.For(0, TotalItems, i =>
        {
            buffer.Add(i);
        });
    }

    [Benchmark(Description = "New - Multi Thread")]
    public void New_MultiThread()
    {
        using var buffer = new DataBuffer<int>(BatchSize, _mockConsumer);
        Parallel.For(0, TotalItems, i =>
        {
            buffer.Add(i);
        });
    }
}