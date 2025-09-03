using Simple.DatabaseWrapper.Helpers;
using System.Linq;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests
{
    public class CsvParserTests
    {
        [Fact]
        public void Parse_EmptyString_ReturnsEmpty()
        {
            var result = CsvParser.ParseCsvString(string.Empty);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public void Parse_Complex()
        {
            string content = "Name,Age,Description\r\nJohn,30,\"Software Engineer, Senior\"\r\n,,\r\nJane,25,\"Data Scientist\r\nwith multiple projects\"";
            var result = CsvParser.ParseCsvString(content).ToArray();
            Assert.NotNull(result);

            Assert.Equal(4, result.Length);
            // Row 1
            Assert.Equal(4, result[1][0].Length);
            Assert.Contains(",", result[1][2]);
            // Row 3
            Assert.Equal(0, result[2][2].Length);
            // Row 4
            Assert.Contains("\n", result[3][2]);
        }
    }
}
