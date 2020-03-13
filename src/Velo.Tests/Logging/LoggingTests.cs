using Velo.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public abstract class LoggingTests : TestClass
    {
        protected LoggingTests(ITestOutputHelper output) : base(output)
        {
        }

        public static TheoryData<LogLevel> Levels => new TheoryData<LogLevel>()
        {
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error
        };
    }
}