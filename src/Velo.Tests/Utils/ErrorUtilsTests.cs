using System;
using System.Reflection;
using Velo.TestsModels;
using Velo.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Utils
{
    public class ErrorUtilsTests : TestClass
    {
        public ErrorUtilsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Try_Catch()
        {
            var catchExecuted = false;
            Error<InvalidCastException>
                .Catch(e => catchExecuted = true)
                .Action(() => throw Error.Cast(string.Empty));

            Assert.True(catchExecuted);
        }

        [Fact]
        public void Try_Count()
        {
            const int tryCount = 10;

            var counter = 0;
            Assert.Throws<InvalidCastException>(() =>
            {
                Error<InvalidCastException>
                    .Try(tryCount)
                    .Attempt(e => counter++)
                    .Rethrow()
                    .Action(() => throw Error.Cast(string.Empty));
            });

            Assert.Equal(10, counter);
        }

        [Fact]
        public void Try_Disabled()
        {
            const string expected = "abc";

            var actual = Error<InvalidCastException>
                .Try(10)
                .Enabled(false)
                .Return(() => expected);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Try_Map()
        {
            Assert.Throws<AmbiguousMatchException>(() =>
            {
                Error<InvalidCastException>
                    .Map(e => Error.NotSingle(string.Empty))
                    .Rethrow()
                    .Action(() => throw Error.Cast(""));
            });
        }

        [Fact]
        public void Try_NotHandleOtherException()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                Error<InvalidCastException>
                    .Catch(e => { })
                    .Action(() => throw Error.Disposed(string.Empty));
            });
        }
    }
}