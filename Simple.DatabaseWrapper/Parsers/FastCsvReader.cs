#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
namespace Simple.DatabaseWrapper.Parsers;

using System;
using System.IO;

public class FastCsvReader(StreamReader reader, char delimiter = ',', char quote = '"') : IDisposable
{
    private readonly StreamReader _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    private readonly char _quote = quote;
    private readonly char _delimiter = delimiter;

    private char[] _buffer = new char[65536];
    private int _bufferLength = 0;
    private int _bufferPos = 0;

    private (int Offset, int Length)[] _columns = new (int, int)[16];

    public int FieldCount { get; private set; }

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
                int leftover = _bufferLength - rowStart;

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
                    int finalColumnLength = _bufferPos - fieldStart;
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

            char c = _buffer[_bufferPos];

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
                    // Windows \r
                    int end = _bufferPos;
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

    public ReadOnlySpan<char> GetSpan(int colIndex)
    {
        if (colIndex < 0 || colIndex >= FieldCount)
            throw new IndexOutOfRangeException($"Coluna {colIndex} não existe.");

        var (offset, length) = _columns[colIndex];
        var span = new ReadOnlySpan<char>(_buffer, offset, length);

        if (span.Length >= 2 && span[0] == _quote && span[span.Length - 1] == _quote)
        {
            span = span.Slice(1, span.Length - 2);
        }

        return span;
    }

    public string GetString(int colIndex)
    {
        var span = GetSpan(colIndex);
        string text = span.ToString();

        if (text.Contains(new string(_quote, 2)))
        {
            return text.Replace(new string(_quote, 2), _quote.ToString());
        }

        return text;
    }

    private void AddColumn(int offset, int length)
    {
        if (FieldCount >= _columns.Length)
        {
            Array.Resize(ref _columns, _columns.Length * 2);
        }
        _columns[FieldCount++] = (offset, length);
    }

    public void Dispose()
    {
        _reader?.Dispose();
    }
}
#endif