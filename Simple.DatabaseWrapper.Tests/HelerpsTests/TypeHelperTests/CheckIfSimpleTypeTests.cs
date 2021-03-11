using System;
using Simple.DatabaseWrapper.Helpers;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests.TypeHelperTests
{
    public class CheckIfSimpleTypeTests
    {

        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(long))]
        [InlineData(typeof(int))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        public void CheckIfSimpleType_Primitive(Type t)
        {
            Assert.True(TypeHelper.CheckIfSimpleType(t));
        }

        [Theory]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(string))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(Guid))]
        public void CheckIfSimpleType_NotPrimitive(Type t)
        {
            Assert.True(TypeHelper.CheckIfSimpleType(t));
        }

        [Theory]
        [InlineData(typeof(long?))]
        [InlineData(typeof(int?))]
        [InlineData(typeof(byte?))]
        [InlineData(typeof(short?))]
        [InlineData(typeof(ulong?))]
        [InlineData(typeof(uint?))]
        [InlineData(typeof(ushort?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        public void CheckIfSimpleType_NullablePrimitive(Type t)
        {
            Assert.True(TypeHelper.CheckIfSimpleType(t));
        }

        [Theory]
        [InlineData(typeof(decimal?))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(TimeSpan?))]
        [InlineData(typeof(Guid?))]
        public void CheckIfSimpleType_NullableNotPrimitive(Type t)
        {
            Assert.True(TypeHelper.CheckIfSimpleType(t));
        }

        [Theory]
        [InlineData(typeof(Exception))]
        [InlineData(typeof(NotSimpleClass))]
        [InlineData(typeof(NotSimpleStruct))]
        public void CheckIfSimpleType_NotSimple(Type t)
        {
            Assert.False(TypeHelper.CheckIfSimpleType(t));
        }

        public class NotSimpleClass
        {
            public int Prop1 { get; set; }
        }
        public struct NotSimpleStruct
        {
            public int Prop1 { get; set; }
        }
    }
}
