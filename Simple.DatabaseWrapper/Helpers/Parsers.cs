namespace Simple.DatabaseWrapper.Helpers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class ConnectionStringParser
{
    public static Dictionary<string, string> Parse(string connectionString)
    {
#if NET20
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim().Length == 0) return [];
#else
        if (string.IsNullOrWhiteSpace(connectionString)) return [];
#endif

        if (connectionString.Contains("\"\""))
        {
            throw new ArgumentException("Double-quotted quotes are not supported");
        }

        var result = new Dictionary<string, string>();
        var currentKey = new StringBuilder();
        var currentValue = new StringBuilder();
        bool inKey = true;
        bool inQuotes = false;
        bool escapeNext = false;

        for (int i = 0; i < connectionString.Length; i++)
        {
            char c = connectionString[i];

            if (escapeNext)
            {
                (inKey ? currentKey : currentValue).Append(c);
                escapeNext = false;
                continue;
            }

            if (c == '\\')
            {
                escapeNext = true;
                continue;
            }

            if (c == '"' && !inKey)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == '=' && !inQuotes && inKey)
            {
                inKey = false;
                continue;
            }

            if (c == ';' && !inQuotes)
            {
                // If [inKey], no Equal sign

                if (currentKey.Length > 0 && !inKey)
                {
                    result[currentKey.ToString().Trim()] = currentValue.ToString().Trim();
                }

#if NET20
                currentKey = new StringBuilder();
                currentValue = new StringBuilder();
#else
                currentKey.Clear();
                currentValue.Clear();
#endif

                inKey = true;
                continue;
            }

            (inKey ? currentKey : currentValue).Append(c);
        }

        // Handle the last key-value pair
        if (currentKey.Length > 0)
        {
            result[currentKey.ToString().Trim()] = currentValue.ToString().Trim();
        }

        return result;
    }
}
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
        using var sr = new StreamReader(filePath, encoding ?? Encoding.UTF8);
        return ParseCsv(sr, quote, delimiter);
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
        foreach(var row in rows) yield return converter(row);
    }
}