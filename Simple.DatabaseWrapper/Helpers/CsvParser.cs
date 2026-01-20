namespace Simple.DatabaseWrapper.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class CsvParser
{
    /// <summary>
    /// Parses a CSV file into an Enumerable (rows) of string[] (columns)
    /// </summary>
    /// <param name="filePath">File to parse</param>
    /// <param name="encoding">File encoding to use</param>
    /// <param name="quote">Quote char to use</param>
    /// <param name="delimiter">Delimiter char to use</param>
    public static IEnumerable<string[]> ParseCsvFile(string filePath, Encoding? encoding = null, char quote = '"', char delimiter = ',')
    {
        using var reader = new StreamReader(filePath, encoding ?? Encoding.UTF8);
        foreach (var row in ParseCsv(reader, quote, delimiter))
        {
            yield return row;
        }
    }
    /// <summary>
    /// Parses a CSV file into an Enumerable (rows) of string[] (columns)
    /// </summary>
    /// <param name="content">String content to parse</param>
    /// <param name="quote">Quote char to use</param>
    /// <param name="delimiter">Delimiter char to use</param>
    public static IEnumerable<string[]> ParseCsvString(string content, char quote = '"', char delimiter = ',')
    {
        if (string.IsNullOrEmpty(content)) yield break;

        byte[] bytes = Encoding.UTF8.GetBytes(content);
        using MemoryStream stream = new(bytes);
        using StreamReader reader = new(stream, Encoding.UTF8);

        foreach (var row in ParseCsv(reader, quote, delimiter))
        {
            yield return row;
        }
    }
    /// <summary>
    /// Parses a CSV file into an Enumerable (rows) of string[] (columns)
    /// </summary>
    /// <param name="reader">Reader to get rows from</param>
    /// <param name="quote">Quote char to use</param>
    /// <param name="delimiter">Delimiter char to use</param>
    public static IEnumerable<string[]> ParseCsv(StreamReader reader, char quote, char delimiter)
    {
        bool inQuote = false;
        List<string> fields = [];
        StringBuilder fieldBuilder = new StringBuilder();

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuote)
                {
                    if (c == quote)
                    {
                        // Verifica se é uma aspa escapada ("" dentro de campo quotado)
                        if (i + 1 < line.Length && line[i + 1] == quote)
                        {
                            fieldBuilder.Append(quote);
                            i++; // Pula a próxima aspa
                        }
                        else
                        {
                            inQuote = false; // Sai do modo quotado
                        }
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
                else
                {
                    if (c == quote)
                    {
                        inQuote = true; // Entra no modo quotado
                    }
                    else if (c == delimiter)
                    {
                        fields.Add(fieldBuilder.ToString());
#if NET20
                        fieldBuilder = new StringBuilder();
#else
                        fieldBuilder.Clear();
#endif
                    }
                    else
                    {
                        fieldBuilder.Append(c);
                    }
                }
            }

            // Se estamos em um campo quotado, adiciona a quebra de linha
            if (inQuote)
            {
                fieldBuilder.Append('\n');
            }
            else
            {
                // Fim da linha, adiciona o último campo e retorna a linha
                fields.Add(fieldBuilder.ToString());
#if NET20
                fieldBuilder = new StringBuilder();
#else
                fieldBuilder.Clear();
#endif
                yield return fields.ToArray();
                fields.Clear();
            }
        }

        // Processa o último campo, se houver
        if (fieldBuilder.Length > 0 || fields.Count > 0)
        {
            fields.Add(fieldBuilder.ToString());
            yield return fields.ToArray();
        }
    }

    /// <summary>
    /// Converts CSV rows to T instance
    /// </summary>
    /// <typeparam name="T">Target Type</typeparam>
    /// <param name="rows">Parsed CSV Rows</param>
    /// <param name="converter">Mapper function</param>
    public static IEnumerable<T> As<T>(this IEnumerable<string[]> rows, Func<string[], T> converter)
    {
        foreach (var row in rows) yield return converter(row);
    }

#if NET6_0_OR_GREATER || NET48_OR_GREATER
    /// <summary>
    /// Parse Zipped CSV file and get it's rows
    /// </summary>
    /// <param name="zipPath">Zip file path</param>
    /// <param name="entryFilter">Zip Entry filter</param>
    /// <param name="fileRowAction">Action called for each row of each file</param>
    /// <param name="encoding">Encoding to be used to parse</param>
    /// <param name="quote">Quote char to be used to parse</param>
    /// <param name="delimiter">Delimiter char to be used to parse</param>
    public static void GetZippedCsvRows(string zipPath, Func<System.IO.Compression.ZipArchiveEntry, bool> entryFilter, Action<string, string[]> fileRowAction, Encoding encoding = null, char delimiter = ',', char quote = '"')
    {
        using var fs = File.OpenRead(zipPath);
        using var archive = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (!entryFilter(entry)) continue;

            using var zipStream = entry.Open();
            using var reader = new StreamReader(zipStream, encoding ?? Encoding.UTF8);
            var rows = ParseCsv(reader, quote, delimiter);

            foreach (var row in rows)
            {
                fileRowAction(entry.FullName, row);
            }
        }
    }

    /// <summary>
    /// Parse Zipped CSV file and get it's rows
    /// </summary>
    /// <param name="zipPath">Zip file path</param>
    /// <param name="fullNameFilter">Zip Entry FullName filter</param>
    /// <param name="fileRowAction">Action called for each row of each file</param>
    /// <param name="encoding">Encoding to be used to parse</param>
    /// <param name="quote">Quote char to be used to parse</param>
    /// <param name="delimiter">Delimiter char to be used to parse</param>
    public static void GetZippedCsvRows(string zipPath, Func<string, bool> fullNameFilter, Action<string, string[]> fileRowAction, Encoding encoding = null, char delimiter = ',', char quote = '"')
    {
        GetZippedCsvRows(zipPath,
                         e => fullNameFilter(e.FullName),
                         fileRowAction,
                         encoding, delimiter, quote);
    }

    /// <summary>
    /// Parse all CSV Zipped entries and get it's rows
    /// </summary>
    /// <param name="zipPath">Zip file path</param>
    /// <param name="fileRowAction">Action called for each row of each file</param>
    /// <param name="encoding">Encoding to be used to parse</param>
    /// <param name="quote">Quote char to be used to parse</param>
    /// <param name="delimiter">Delimiter char to be used to parse</param>
    public static void GetZippedCsvRows(string zipPath, Action<string, string[]> fileRowAction, Encoding encoding = null, char delimiter = ',', char quote = '"')
    {
        GetZippedCsvRows(zipPath,
                         e => e.Name.Length > 0, // Only files
                         fileRowAction,
                         encoding, delimiter, quote);
    }
#endif

}