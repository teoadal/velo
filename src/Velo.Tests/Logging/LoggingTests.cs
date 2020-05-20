using Velo.Logging;
using Xunit;

namespace Velo.Tests.Logging
{
    public abstract class LoggingTests : TestClass
    {
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