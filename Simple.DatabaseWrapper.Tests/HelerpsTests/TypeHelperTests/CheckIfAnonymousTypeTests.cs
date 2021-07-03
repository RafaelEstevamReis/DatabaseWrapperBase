using Simple.DatabaseWrapper.Helpers;
using System;
using Xunit;

namespace Simple.DatabaseWrapper.Tests.HelerpsTests.TypeHelperTests
{
    public class CheckIfAnonymousTypeTests
    {
        [Fact]
        public void CheckIfAnonymousType_BaseChecks_Anonymous()
        {
            var valueA = new { a = 1 };
            var valueNone = new { };
            Assert.True(TypeHelper.CheckIfAnonymousType(valueA.GetType()));
            Assert.True(TypeHelper.CheckIfAnonymousType(valueNone.GetType()));
        }

        [Fact]
        public void CheckIfAnonymousType_BaseChecks_Struct()
        {
            // Public
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(TestStructProps)));
            // local
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(stA)));
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(stB)));
        }

        [Fact]
        public void CheckIfAnonymousType_BaseChecks_Classes()
        {
            // Public
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(CheckIfAnonymousTypeTests))); // This test class
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(TestClassProps)));
            // local
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(cA)));
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(cB)));
            Assert.False(TypeHelper.CheckIfAnonymousType(typeof(cC)));
        }

        [Fact]
        public void CheckIfAnonymousType_Null()
        {
            Assert.Throws<ArgumentNullException>(() => TypeHelper.CheckIfAnonymousType(null));
        }

        struct stA
        {
        }
        struct stB
        {
            int A;
        }

        class cA { }
        class cB { int a { get; set; } }
        class cC { public int a { get; set; } }
    }
}
