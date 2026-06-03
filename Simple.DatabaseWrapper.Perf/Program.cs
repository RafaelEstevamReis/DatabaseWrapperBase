using BenchmarkDotNet.Running;
using Simple.DatabaseWrapper.Perf;

var summary = BenchmarkRunner.Run<DataBufferTypeBenchmark>();
