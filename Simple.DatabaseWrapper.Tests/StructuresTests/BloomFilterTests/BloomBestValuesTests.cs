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
        [InlineData(200_000_000, 7_956_585_713)]
        public void BloomFilter_BestSize(int capacity, long bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.01f, 4_793)]
        [InlineData(1_000, 0.01f, 9_586)]
        [InlineData(10_000_000, 0.01f, 95_850_585)]
        [InlineData(200_000_000, 0.01f, 1_917_011_685)]
        public void BloomFilter_BestSizeError_1p(int capacity, float acceptedError, long bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.05f, 3_118)]
        [InlineData(1_000, 0.05f, 6_236)]
        [InlineData(10_000_000, 0.05f, 62_352_242)]
        [InlineData(200_000_000, 0.05f, 1_247_044_840)]
        public void BloomFilter_BestSizeError_5p(int capacity, float acceptedError, long bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }
        [Theory]
        [InlineData(500, 0.1f, 2_397)]
        [InlineData(1_000, 0.1f, 4_793)]
        [InlineData(10_000_000, 0.1f, 47_925_292)]
        [InlineData(200_000_000, 0.1f, 958_505_832)]
        public void BloomFilter_BestSizeError_10p(int capacity, float acceptedError, long bestSize)
        {
            var size = Bloom<object>.BestArraySize(capacity, acceptedError);
            Assert.Equal(bestSize, size);
        }

        [Fact]
        public void BloomFilter_BestSizeEmpirical()
        {
            float acceptedError = 0.05f; // 5%
            var size = Bloom<int>.BestArraySize(100, acceptedError); // 5%
            var bloom = new Bloom<int>((int)size);

            for (int i = 0; i < 100; i++)
            {
                bloom.Add(i);
            }
            int matches = 0;
            for(int i = 1000; i< 21_000; i++) // 20k tests
            {
                if (bloom.Contains(i)) matches++;
            }

            float actualError = matches / 20_000.0f;
            Assert.Equal(acceptedError, actualError, 0.05); // 3% tolerance
        }


    }
}
