namespace Simple.DatabaseWrapper.Tests.HelerpsTests;

using Simple.DatabaseWrapper.Helpers;
using Xunit;

public class ConnectionStringParserTests
{
    [Fact]
    public void Parse_EmptyString_ReturnsEmptyDictionary()
    {
        var result = ConnectionStringParser.Parse(string.Empty);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_WhitespaceString_ReturnsEmptyDictionary()
    {
        string connectionString = "   ";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_SqlServerConnectionString_ParsesCorrectly()
    {
        string connectionString = "Server=localhost;Database=myDB;User Id=sa;Password=pass123;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(4, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("myDB", result["Database"]);
        Assert.Equal("sa", result["User Id"]);
        Assert.Equal("pass123", result["Password"]);
    }

    [Fact]
    public void Parse_MySqlConnectionStringWithQuotes_ParsesCorrectly()
    {
        string connectionString = "Server=127.0.0.1;Port=3306;Database=test_db;Uid=\"john doe\";Pwd=\"secret pass\";";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(5, result.Count);
        Assert.Equal("127.0.0.1", result["Server"]);
        Assert.Equal("3306", result["Port"]);
        Assert.Equal("test_db", result["Database"]);
        Assert.Equal("john doe", result["Uid"]);
        Assert.Equal("secret pass", result["Pwd"]);
    }

    [Fact]
    public void Parse_PostgreSqlConnectionStringWithEscapedSemicolon_ParsesCorrectly()
    {
        string connectionString = "Host=localhost;Port=5432;Database=app_db;Username=postgres;Password=secret\\;pass";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(5, result.Count);
        Assert.Equal("localhost", result["Host"]);
        Assert.Equal("5432", result["Port"]);
        Assert.Equal("app_db", result["Database"]);
        Assert.Equal("postgres", result["Username"]);
        Assert.Equal("secret;pass", result["Password"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithTrailingSemicolon_ParsesCorrectly()
    {
        string connectionString = "Server=localhost;Database=myDB;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(2, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("myDB", result["Database"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithEmptyValue_ParsesCorrectly()
    {
        string connectionString = "Server=localhost;Database=;User Id=sa;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(3, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("", result["Database"]);
        Assert.Equal("sa", result["User Id"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithEscapedQuotes_ParsesCorrectly()
    {
        string connectionString = "Server=localhost;Database=myDB;Password=\"pass\\\"with\\\"quotes\";";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(3, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("myDB", result["Database"]);
        Assert.Equal("pass\"with\"quotes", result["Password"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithMultipleSemicolons_IgnoresEmptyPairs()
    {
        string connectionString = "Server=localhost;;;Database=myDB;;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(2, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("myDB", result["Database"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithNoValueAfterEquals_ParsesEmptyValue()
    {
        string connectionString = "Server=localhost;Database=;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(2, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("", result["Database"]);
    }

    [Fact]
    public void Parse_ConnectionStringWithKeyOnly_IgnoresKeyWithoutValue()
    {
        string connectionString = "Server=localhost;Database;User Id=sa;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(2, result.Count);
        Assert.Equal("localhost", result["Server"]);
        Assert.Equal("sa", result["User Id"]);
    }

    //[Fact] // Unsupported
    //public void Parse_OleDbConnectionString_ParsesCorrectly()
    //{
    //    string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\txtFilesFolder\;Extended Properties=""text;HDR=Yes;FMT=Fixed"";";
    //    var result = ConnectionStringParser.Parse(connectionString);
    //
    //    Assert.Equal(3, result.Count);
    //    Assert.Equal("Microsoft.Jet.OLEDB.4.0", result["Provider"]);
    //    Assert.Equal(@"c:\txtFilesFolder\", result["Data Source"]);
    //    Assert.Equal("text;HDR=Yes;FMT=Fixed", result["Extended Properties"]);
    //}

    [Fact]
    public void Parse_ZimOdbcConnectionString_ParsesCorrectly()
    {
        string connectionString = "Driver={Zim Technologies Inc. ODBC Driver 32};Remote=False;Dbq=PathOrDbName;Uid=myUsername;Pwd=myPassword;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(5, result.Count);
        Assert.Equal("{Zim Technologies Inc. ODBC Driver 32}", result["Driver"]);
        Assert.Equal("False", result["Remote"]);
        Assert.Equal("PathOrDbName", result["Dbq"]);
        Assert.Equal("myUsername", result["Uid"]);
        Assert.Equal("myPassword", result["Pwd"]);
    }

    [Fact]
    public void Parse_SqliteConnectionString_ParsesCorrectly()
    {
        string connectionString = @"Data Source=c:\\mydb.db;Version=3;Pooling=True;Max Pool Size=100;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(4, result.Count);
        Assert.Equal(@"c:\mydb.db", result["Data Source"]);
        Assert.Equal("3", result["Version"]);
        Assert.Equal("True", result["Pooling"]);
        Assert.Equal("100", result["Max Pool Size"]);
    }

    [Fact]
    public void Parse_FeedConnectionStringWithUrls_ParsesCorrectly()
    {
        string connectionString = "Tables=MicrosoftFeed=http://blogs.technet.com/microsoft_blog/rss.xml,GoogleNews=http://news.google.com/news?pz=1&cf=all&ned=us&hl=en&output=rss;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Single(result);
        Assert.Equal("MicrosoftFeed=http://blogs.technet.com/microsoft_blog/rss.xml,GoogleNews=http://news.google.com/news?pz=1&cf=all&ned=us&hl=en&output=rss", result["Tables"]);
    }

    [Fact]
    public void Parse_AtomFeedConnectionString_ParsesCorrectly()
    {
        string connectionString = "URL=http://someservice.com/atom-feed/;User=myUsername;Password=myPassword;Firewall Server=fireWallIPorDNSname;Firewall User=fwUserName;Firewall Password=fwPassword;";
        var result = ConnectionStringParser.Parse(connectionString);

        Assert.Equal(6, result.Count);
        Assert.Equal("http://someservice.com/atom-feed/", result["URL"]);
        Assert.Equal("myUsername", result["User"]);
        Assert.Equal("myPassword", result["Password"]);
        Assert.Equal("fireWallIPorDNSname", result["Firewall Server"]);
        Assert.Equal("fwUserName", result["Firewall User"]);
        Assert.Equal("fwPassword", result["Firewall Password"]);
    }
}
