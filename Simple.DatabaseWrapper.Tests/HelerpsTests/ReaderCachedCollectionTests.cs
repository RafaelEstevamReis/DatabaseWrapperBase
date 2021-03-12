using Simple.DatabaseWrapper.TypeReader;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests
{
    public class ReaderCachedCollectionTests
    {
        [Fact]
        public void ReaderCachedCollection_BaseTests()
        {
            var col = new ReaderCachedCollection();

            Assert.False(col.HasCacheOf<TestClassProps>());
            var info1 = col.GetInfo<TestClassProps>();
            Assert.Equal("TestClassProps", info1.TypeName);
            Assert.True(col.HasCacheOf<TestClassProps>());

            Assert.False(col.HasCacheOf<TestStructProps>());
            var info2 = col.GetInfo<TestStructProps>();
            Assert.Equal("TestStructProps", info2.TypeName);
            Assert.True(col.HasCacheOf<TestStructProps>());

            col.Remove<TestClassProps>();
            Assert.False(col.HasCacheOf<TestClassProps>());
            Assert.True(col.HasCacheOf<TestStructProps>());
        }
    }
}
