using Simple.DatabaseWrapper.Structures;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.StructuresTests.BloomFilterTests
{
    public class BloomBaseTests
    {
        [Fact]
        public void SimpleStringBloomTests()
        {
            var filter = new Bloom<string>(new BloomHash_Murmur2(), 20);

            string[] contains = { "a", "b", "c", "d" };
            string[] absent = { "1", "2", "3", "4", "5", "6", "7", "8" };

            filter.AddRange(contains);

            foreach (var c in contains) Assert.True(filter.Containts(c));
            foreach (var c in absent) Assert.False(filter.Containts(c));
        }
        [Fact]
        public void SimpleIntBloomTests()
        {
            var filter = new Bloom<int>(new BloomHash_Murmur2(), 20);

            int[] contains = { 1, 2, 3, 4 };
            int[] absent = { 11, 12, 13, 14, 16, 17, 18 };

            filter.AddRange(contains);

            foreach (var c in contains) Assert.True(filter.Containts(c));
            foreach (var c in absent) Assert.False(filter.Containts(c));
        }

    }
}
