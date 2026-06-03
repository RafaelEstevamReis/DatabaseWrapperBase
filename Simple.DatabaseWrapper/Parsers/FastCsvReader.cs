#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
namespace Simple.DatabaseWrapper.Parsers;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// A Fast CSV reader
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Unavailable on older frameworks")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper", Justification = "Unavailable on older frameworks")]
public sealed class FastCsvReader(StreamReader reader, char delimiter = ',', char quote = '"') : IDisposable
{
    private readonly StreamReader _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    private readonly char _quote = quote;
    private readonly char _delimiter = delimiter;

    private char[] _buffer = new char[65536];
    private char[] _unescapeBuffer = new char[1024];
    private int _bufferLength = 0;
    private int _bufferPos = 0;

    private (int Offset, int Length)[] _columns = new (int, int)[16];

    /// <summary>
    /// Current row field count
    /// </summary>
    public int FieldCount { get; private set; }

    /// <summary>
    /// Reads next row
    /// </summary>
    /// <returns>True if a new row wqas read; False if EOF</returns>
    public bool Read()
    {
        FieldCount = 0;
        var inQuote = false;
        var rowStart = _bufferPos;
        var fieldStart = _bufferPos;

        while (true)
        {
            // End of buffer?
            if (_bufferPos >= _bufferLength)
            {
                var leftover = _bufferLength - rowStart;

                if (leftover > 0 && fieldStart > 0)
                {
                    Array.Copy(_buffer, rowStart, _buffer, 0, leftover);

                    for (int i = 0; i < FieldCount; i++)
                    {
                        _columns[i].Offset -= rowStart;
                    }

                    fieldStart -= rowStart;
                }

                // Line bigger then buffer?
                if (leftover == _buffer.Length)
                {
                    Array.Resize(ref _buffer, _buffer.Length * 2);
                }

                _bufferPos = leftover;
                rowStart = 0;

                int read = _reader.Read(_buffer, _bufferPos, _buffer.Length - _bufferPos);
                if (read == 0) // EOF
                {
                    var finalColumnLength = _bufferPos - fieldStart;
                    if (finalColumnLength > 0 || (FieldCount > 0 && _bufferPos == fieldStart))
                    {
                        AddColumn(fieldStart, finalColumnLength);
                        _bufferLength = 0;
                        _bufferPos = 0;
                        return true;
                    }
                    return false;
                }
                _bufferLength = _bufferPos + read;
            }

            var c = _buffer[_bufferPos];

            if (inQuote)
            {
                if (c == _quote)
                {
                    if (_bufferPos + 1 < _bufferLength && _buffer[_bufferPos + 1] == _quote)
                    {
                        _bufferPos++;
                    }
                    else
                    {
                        inQuote = false;
                    }
                }
            }
            else
            {
                if (c == _quote)
                {
                    inQuote = true;
                }
                else if (c == _delimiter)
                {
                    AddColumn(fieldStart, _bufferPos - fieldStart);
                    fieldStart = _bufferPos + 1;
                }
                else if (c == '\n')
                {
                    // Windows '\r'
                    var end = _bufferPos;
                    if (end > fieldStart && _buffer[end - 1] == '\r')
                    {
                        end--;
                    }

                    AddColumn(fieldStart, end - fieldStart);
                    _bufferPos++;
                    return true;
                }
            }

            _bufferPos++;
        }
    }

    /// <summary>
    /// Get a coumn span
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Index outside of column count</exception>
    public ReadOnlySpan<char> GetSpan(int colIndex)
    {
        if (colIndex < 0 || colIndex >= FieldCount)
        {
            throw new IndexOutOfRangeException($"Column {colIndex} does not exist");
        }

        var (offset, length) = _columns[colIndex];
        var span = new ReadOnlySpan<char>(_buffer, offset, length);

        if (span.Length >= 2 && span[0] == _quote && span[^1] == _quote)
        {
            span = span[1..^1];
        }

        return span;
    }

    /// <summary>
    /// Get a Column string value
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Index outside of column count</exception>
    public string GetString(int colIndex)
    {
        var span = GetSpan(colIndex);

        if (span.IsEmpty) return string.Empty;

        bool hasEscaped = false;
        for (int i = 0; i < span.Length - 1; i++)
        {
            if (span[i] == _quote && span[i + 1] == _quote)
            {
                hasEscaped = true;
                break;
            }
        }

        if (!hasEscaped)
        {
            return span.ToString();
        }

        // Checks buffer len
        if (_unescapeBuffer.Length < span.Length)
        {
            Array.Resize(ref _unescapeBuffer, span.Length);
        }

        // Copy
        int finalLength = 0;
        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];
            _unescapeBuffer[finalLength++] = c;

            // Se for uma aspa e a próxima também for, pula a próxima
            if (c == _quote && i < span.Length - 1 && span[i + 1] == _quote)
            {
                i++;
            }
        }

        return new string(_unescapeBuffer, 0, finalLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddColumn(int offset, int length)
    {
        if (FieldCount >= _columns.Length)
        {
            Array.Resize(ref _columns, _columns.Length * 2);
        }
        _columns[FieldCount++] = (offset, length);
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        _reader?.Dispose();
    }

    /// <summary>
    /// Process a zipped CSV file
    /// Caution: Row string[] is reused internally, do not store
    /// </summary>
    /// <exception cref="ArgumentNullException">Row Action should be defined</exception>
    public static void ParseCsvZippedFile(string zipFile, Action<string, string[]> onFileRowRead, Func<string, bool> fullNameFilter, char delimiter = ',', char quote = '"', Encoding encoding = null)
    {
        if (onFileRowRead == null)
        {
            throw new ArgumentNullException(nameof(onFileRowRead));
        }

        fullNameFilter ??= fullName => !string.IsNullOrEmpty(fullName);

        using var fs = File.OpenRead(zipFile);
        using var archive = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name)) continue; // Folders
            if (!fullNameFilter(entry.FullName)) continue;

            using var zipStream = entry.Open();
            using var reader = encoding == null
                    ? new StreamReader(zipStream)
                    : new StreamReader(zipStream, encoding);

            ParseCsvLines(reader, row => onFileRowRead(entry.FullName, row), delimiter, quote);
        }
    }

    /// <summary>
    /// Process a CSV file
    /// </summary>
    public static void ParseCsvFile(string csvFile, Action<string[]> onRowRead, char delimiter = ',', char quote = '"', Encoding encoding = null)
    {
        using var reader = new StreamReader(csvFile, encoding ?? Encoding.UTF8);
        ParseCsvLines(reader, onRowRead, delimiter, quote);
    }

    /// <summary>
    /// Process a CSV reader
    /// Caution: Row string[] is reused internally, do not store
    /// </summary>
    /// <exception cref="ArgumentNullException">Row Action should be defined</exception>
    public static void ParseCsvLines(StreamReader reader, Action<string[]> onRowRead, char delimiter = ',', char quote = '"')
    {
        if (onRowRead == null)
        {
            throw new ArgumentNullException(nameof(onRowRead));
        }

        using var csvReader = new FastCsvReader(reader, delimiter, quote);

        string[] row = null;
        while (csvReader.Read())
        {
            if (row == null || row.Length != csvReader.FieldCount)
            {
                row = new string[csvReader.FieldCount];
            }

            for (int i = 0; i < csvReader.FieldCount; i++)
            {
                row[i] = csvReader.GetString(i);
            }

            onRowRead(row);
        }
    }

}
#endif