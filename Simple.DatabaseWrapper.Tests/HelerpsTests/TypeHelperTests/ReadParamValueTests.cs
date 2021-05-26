using Simple.DatabaseWrapper.Helpers;
using Simple.DatabaseWrapper.TypeReader;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests.TypeHelperTests
{
    public class ReadParamValueTests
    {
        [Fact]
        public void ReadParamValue_Props_NoErrors()
        {
            var testObject = new TestClassProps();
            var typeInfo = TypeInfo.FromType<TestClassProps>();

            foreach (var p in typeInfo.Items)
            {
                if (!p.CanRead) continue;
                _ = TypeHelper.ReadParamValue(p, testObject);
            }
        }
        [Fact]
        public void ReadParamValue_Fields_NoErrors()
        {
            var testObject = new TestClassFields();
            var typeInfo = TypeInfo.FromType<TestClassFields>();

            foreach (var p in typeInfo.Items)
            {
                if (!p.CanRead) continue;
                _ = TypeHelper.ReadParamValue(p, testObject);
            }
        }
    }
}
