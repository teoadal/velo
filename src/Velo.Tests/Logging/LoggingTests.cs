using System.Collections.Generic;
using Velo.Logging;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public abstract class LoggingTests : TestClass
    {
        protected LoggingTests(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<object[]> Levels => new[]
        {
            new object[] {LogLevel.Trace},
            new object[] {LogLevel.Debug},
            new object[] {LogLevel.Info},
            new object[] {LogLevel.Warning},
            new object[] {LogLevel.Error},
        };
    }
}