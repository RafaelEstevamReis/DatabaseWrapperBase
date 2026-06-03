namespace Simple.DatabaseWrapper.Tests.HelerpsTests;

using Simple.DatabaseWrapper.Parsers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

public class FastCsvReaderTests
{
    // Helper method to create a StreamReader from an in-memory string
    private StreamReader CreateStream(string content)
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new StreamReader(memoryStream);
    }

    [Fact]
    public void Read_ShouldParseSimpleCsvCorrectly()
    {
        var csv = "Id,Name,Age\n1,John,30\n2,Mary,25";
        using var reader = new FastCsvReader(CreateStream(csv));

        // Line 1 (Header)
        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("Id", reader.GetString(0));
        Assert.Equal("Name", reader.GetString(1));
        Assert.Equal("Age", reader.GetString(2));

        // Line 2
        Assert.True(reader.Read());
        Assert.Equal("1", reader.GetString(0));
        Assert.Equal("John", reader.GetString(1));
        Assert.Equal("30", reader.GetString(2));

        // Line 3
        Assert.True(reader.Read());
        Assert.Equal("2", reader.GetString(0));
        Assert.Equal("Mary", reader.GetString(1));
        Assert.Equal("25", reader.GetString(2));

        // EOF
        Assert.False(reader.Read());
    }

    [Fact]
    public void Read_ShouldHandleInternalQuotesAndDelimiters()
    {
        // Field 2 has a comma INSIDE the quotes. It should not split the column.
        var csv = "1,\"Doe, John\",30";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("1", reader.GetString(0));
        Assert.Equal("Doe, John", reader.GetString(1)); // External quotes must be stripped
        Assert.Equal("30", reader.GetString(2));
    }

    [Fact]
    public void Read_ShouldHandleInternallyEscapedQuotes()
    {
        // Original CSV: 1,"John ""The Boss"" Doe",30
        var csv = "1,\"John \"\"The Boss\"\" Doe\",30";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("1", reader.GetString(0));

        // Double quotes ("") must be converted to single quotes (") in GetString
        Assert.Equal("John \"The Boss\" Doe", reader.GetString(1));
    }

    [Fact]
    public void Read_ShouldHandleEmptyFields()
    {
        // CSV: a,,c,
        var csv = "a,,c,";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());
        Assert.Equal(4, reader.FieldCount);
        Assert.Equal("a", reader.GetString(0));
        Assert.Equal("", reader.GetString(1)); // Empty field between commas
        Assert.Equal("c", reader.GetString(2));
        Assert.Equal("", reader.GetString(3)); // Empty field at the end of the line
    }

    [Fact]
    public void Read_ShouldHandleCustomDelimiters()
    {
        // CSV separated by Semicolon (;)
        var csv = "1;Peter;Active";
        using var reader = new FastCsvReader(CreateStream(csv), delimiter: ';');

        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("1", reader.GetString(0));
        Assert.Equal("Peter", reader.GetString(1));
        Assert.Equal("Active", reader.GetString(2));
    }

    [Fact]
    public void Read_ShouldHandleMixedLineEndings()
    {
        // Mixing \n (Linux) and \r\n (Windows)
        var csv = "Line1\nLine2\r\nLine3";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());
        Assert.Equal("Line1", reader.GetString(0));

        Assert.True(reader.Read());
        Assert.Equal("Line2", reader.GetString(0));

        Assert.True(reader.Read());
        Assert.Equal("Line3", reader.GetString(0));
    }

    [Fact]
    public void Read_ShouldHandleEofWithoutNewLine()
    {
        // The file ends strictly after "c", without a trailing \n (Abrupt EOF)
        var csv = "a,b,c";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("a", reader.GetString(0));
        Assert.Equal("b", reader.GetString(1));
        Assert.Equal("c", reader.GetString(2));

        Assert.False(reader.Read());
    }

    [Fact]
    public void Read_ShouldResizeColumnsArrayAutomatically()
    {
        // FastCsvReader initially supports 16 columns.
        // Forcing 20 columns to trigger internal Array.Resize.
        var sb = new StringBuilder();
        for (int i = 0; i < 20; i++)
        {
            sb.Append($"col{i}");
            if (i < 19) sb.Append(',');
        }

        using var reader = new FastCsvReader(CreateStream(sb.ToString()));

        Assert.True(reader.Read());
        Assert.Equal(20, reader.FieldCount);
        Assert.Equal("col0", reader.GetString(0));
        Assert.Equal("col15", reader.GetString(15));
        Assert.Equal("col19", reader.GetString(19));
    }

    [Fact]
    public void GetSpan_ShouldReturnSpanWithoutAllocatingString()
    {
        var csv = "Test1,\"Test2\"";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());

        var span0 = reader.GetSpan(0);
        var span1 = reader.GetSpan(1);

        Assert.True(span0.SequenceEqual("Test1".AsSpan()));
        // GetSpan should strip external quotes ("...")
        Assert.True(span1.SequenceEqual("Test2".AsSpan()));
    }

    [Fact]
    public void GetString_ShouldThrowExceptionIfIndexIsInvalid()
    {
        var csv = "a,b";
        using var reader = new FastCsvReader(CreateStream(csv));

        Assert.True(reader.Read());

        // Valid access
        Assert.Equal("a", reader.GetString(0));

        // Out of bounds access
        Assert.Throws<IndexOutOfRangeException>(() => reader.GetString(2));
        Assert.Throws<IndexOutOfRangeException>(() => reader.GetString(-1));
    }

    [Fact]
    public void Read_ShouldHandleComplexMultiLineAndEmptyFields()
    {
        // Arrange: A highly complex CSV string combining multiple edge cases
        string csv = "Name,Age,Description\r\nJohn,30,\"Software Engineer, Senior\"\r\n,,\r\nJane,25,\"Data Scientist\r\nwith multiple projects\"";
        using var reader = new FastCsvReader(CreateStream(csv));

        // Act & Assert

        // Line 1: Header
        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("Name", reader.GetString(0));
        Assert.Equal("Age", reader.GetString(1));
        Assert.Equal("Description", reader.GetString(2));

        // Line 2: Commas inside quotes
        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("John", reader.GetString(0));
        Assert.Equal("30", reader.GetString(1));
        Assert.Equal("Software Engineer, Senior", reader.GetString(2)); // Quotes stripped, comma preserved

        // Line 3: Empty fields (just delimiters)
        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("", reader.GetString(0));
        Assert.Equal("", reader.GetString(1));
        Assert.Equal("", reader.GetString(2));

        // Line 4: Line break INSIDE the quoted field + EOF without trailing \n
        Assert.True(reader.Read());
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal("Jane", reader.GetString(0));
        Assert.Equal("25", reader.GetString(1));
        // The inner \r\n must be preserved exactly as it was written, and external quotes stripped
        Assert.Equal("Data Scientist\r\nwith multiple projects", reader.GetString(2));

        // EOF
        Assert.False(reader.Read());
    }
}