using Simple.DatabaseWrapper.Helpers;
using Simple.DatabaseWrapper.TypeReader;
using System;
using System.Drawing;
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

        [Fact]
        public void ReadParamValue_SpecialFields_Timespan()
        {
            var testObject = new Special();
            var typeInfo = TypeInfo.FromType<Special>();

            testObject.Span = TimeSpan.Zero;
            Assert.Equal((Int64)0, TypeHelper.ReadParamValue(typeInfo.Items[0], testObject));

            testObject.Span = TimeSpan.FromTicks(21);
            Assert.Equal((Int64)21, TypeHelper.ReadParamValue(typeInfo.Items[0], testObject));

            testObject.Span = TimeSpan.FromHours(2);
            Assert.Equal((Int64)72000000000, TypeHelper.ReadParamValue(typeInfo.Items[0], testObject));
        }
        [Fact]
        public void ReadParamValue_SpecialFields_Color()
        {
            var testObject = new Special();
            var typeInfo = TypeInfo.FromType<Special>();

            testObject.Color = Color.White;
            Assert.Equal(new byte[] { 255, 255, 255, 255 }, TypeHelper.ReadParamValue(typeInfo.Items[1], testObject));

            testObject.Color = Color.Black;
            Assert.Equal(new byte[] { 255, 0, 0, 0 }, TypeHelper.ReadParamValue(typeInfo.Items[1], testObject));

            testObject.Color = Color.FromArgb(10, 20, 30, 40);
            Assert.Equal(new byte[] { 10, 20, 30, 40 }, TypeHelper.ReadParamValue(typeInfo.Items[1], testObject));
        }

        class Special
        {
            public TimeSpan Span { get; set; }
            public Color Color { get; set; }
        }
    }
}
