using BenchmarkDotNet.Running;
using Simple.DatabaseWrapper.Perf;

BenchmarkRunner.Run<DataBufferTypeBenchmark>();
BenchmarkRunner.Run<CsvParserBenchmark>();
