#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
namespace Simple.DatabaseWrapper.Parsers;

using System;
using System.Globalization;
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
    private readonly char _delimiter = delimiter;
    private readonly char _quote = quote;

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
    /// If current fields have scaped quotes
    /// </summary>
    public bool HasScapedQuotes { get; private set; }

    /// <summary>
    /// Reads next row
    /// </summary>
    /// <returns>True if a new row wqas read; False if EOF</returns>
    public bool Read()
    {
        FieldCount = 0;
        HasScapedQuotes = false;

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

            char[] buf = _buffer;
            int pos = _bufferPos;
            int len = _bufferLength;

            while (pos < len)
            {
                var c = buf[pos];

                if (inQuote)
                {
                    if (c == _quote)
                    {
                        if (pos + 1 < len && buf[pos + 1] == _quote)
                        {
                            pos++;
                            HasScapedQuotes = true;
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
                        AddColumn(fieldStart, pos - fieldStart);
                        fieldStart = pos + 1;
                    }
                    else if (c == '\n')
                    {
                        // Windows '\r'
                        var end = pos;
                        if (end > fieldStart && buf[end - 1] == '\r')
                        {
                            end--;
                        }

                        AddColumn(fieldStart, end - fieldStart);

                        _bufferPos = pos + 1;
                        return true;
                    }
                }

                pos++;
            }

            _bufferPos = pos;
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

        if (!HasScapedQuotes)
        {
            return span.ToString();
        }

        if (_unescapeBuffer.Length < span.Length)
        {
            Array.Resize(ref _unescapeBuffer, span.Length);
        }

        int finalLength = 0;
        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];
            _unescapeBuffer[finalLength++] = c;

            if (c == _quote && i < span.Length - 1 && span[i + 1] == _quote)
            {
                i++;
            }
        }

        return new string(_unescapeBuffer, 0, finalLength);
    }

    /// <summary>
    /// Get column value as exact DateTime
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime GetDateTime(int colIndex, params string[] formats)
    {
        var span = GetSpan(colIndex);
        return DateTime.ParseExact(span, formats, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Get column value as double
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetDouble(int colIndex, IFormatProvider provider = null)
    {
        var span = GetSpan(colIndex);
        return double.Parse(span, provider: provider);
    }

    /// <summary>
    /// Get column value as decimal
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal GetDecimal(int colIndex, IFormatProvider provider = null)
    {
        var span = GetSpan(colIndex);
        return decimal.Parse(span, provider: provider);
    }

    /// <summary>
    /// Get column value as int
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt(int colIndex, IFormatProvider provider = null)
    {
        var span = GetSpan(colIndex);
        return int.Parse(span, provider: provider);
    }

    /// <summary>
    /// Get column value as Boolean, accpets as true (case insensitive): 1, t, true, y, s 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBoolean(int colIndex)
    {
        var span = GetSpan(colIndex);

        if (span.Length == 1)
        {
            return span[0] == '1' || span[0] == 'T' || span[0] == 't' || span[0] == 'y' || span[0] == 's';
        }

        return span.Equals("true".AsSpan(), StringComparison.OrdinalIgnoreCase);
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
    public static void ParseCsvZippedFile(string zipFile, Action<string, string[]> onFileRowRead, Func<string, bool> fullNameFilter = null, char delimiter = ',', char quote = '"', Encoding encoding = null)
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