using Simple.DatabaseWrapper.Structures;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.StructuresTests.BloomFilterTests
{
    public class BloomBestValuesTests
    {
        [Theory]
        [InlineData(10, 0.1f)]
        [InlineData(100, 0.01f)]
        [InlineData(15937, 6.27470654E-05)]
        public void BloomFilter_BestValues(int size, float error)
        {
            var rate = Bloom<object>.BestTheoricalErrorRate(size);
            Assert.Equal(error, rate);
        }
        [Theory]
        [InlineData(500, 6_468)]
        [InlineData(1_000, 14_378)]
        [InlineData(10_000_000, 335_477_043)]
        public void BloomFilter_BestSize(int capacity, int bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.01f, 4_793)]
        [InlineData(1_000, 0.01f, 9_586)]
        [InlineData(10_000_000, 0.01f, 95_850_585)]
        public void BloomFilter_BestSizeError_1p(int capacity, float acceptedError, int bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.05f, 3_118)]
        [InlineData(1_000, 0.05f, 6_236)]
        [InlineData(10_000_000, 0.05f, 62_352_242)]
        public void BloomFilter_BestSizeError_5p(int capacity, float acceptedError, int bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.1f, 2_397)]
        [InlineData(1_000, 0.1f, 4_793)]
        [InlineData(10_000_000, 0.1f, 47_925_292)]
        public void BloomFilter_BestSizeError_10p(int capacity, float acceptedError, int bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }
    }
}
