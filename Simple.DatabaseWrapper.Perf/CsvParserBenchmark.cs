namespace Simple.DatabaseWrapper.Perf;

using BenchmarkDotNet.Attributes;
using Simple.DatabaseWrapper.Parsers;
using System.IO;
using System.Text;

[MemoryDiagnoser]
public class CsvParserBenchmark
{
    private byte[] _csvData;

    [GlobalSetup]
    public void Setup()
    {
        // Gera um CSV com 100.000 linhas e 10 colunas em memória
        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,Age,City,Country,Occupation,Salary,Status,Date,Notes");
        for (int i = 0; i < 100000; i++)
        {
            sb.AppendLine($"{i},John Doe,{i % 100},Metropolis,Countryland,Engineer,{i * 10},Active,2026-06-03,None");
        }
        _csvData = Encoding.UTF8.GetBytes(sb.ToString());
    }

    [Benchmark(Baseline = true, Description = "CsvParser.ParseCsv")]
    public void Parse_Old()
    {
        using var ms = new MemoryStream(_csvData);
        using var reader = new StreamReader(ms);

        // O foreach obriga a máquina de estados do yield a trabalhar
        foreach (var row in CsvParser.ParseCsv(reader, '"', ','))
        {
            // Apenas consome o dado para testar o parser
        }
    }

    [Benchmark(Description = "FastCsvReader.ParseCsvLines")]
    public void Parse_New()
    {
        using var ms = new MemoryStream(_csvData);
        using var reader = new StreamReader(ms);

        // O callback é chamado injetando o mesmo array reciclado
        FastCsvReader.ParseCsvLines(reader, row =>
        {
            // Apenas consome o dado para testar o parser
        }, ',', '"');
    }
}
