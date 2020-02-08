using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Logging
{
    public class LoggerTests : TestClass
    {
        private readonly DependencyCollection _dependencyCollection;

        public LoggerTests(ITestOutputHelper output) : base(output)
        {
            _dependencyCollection = new DependencyCollection()
                .AddLogging();
        }

        [Theory, AutoData]
        public void DefaultEnrichers(string message)
        {
            var provider = _dependencyCollection
                .AddDefaultLogEnrichers()
                .AddLogWriter<ListLoggerWriter>()
                .BuildProvider();

            var logger = provider.GetRequiredService<ILogger<LoggerTests>>();
            logger.Debug(message);

            var sink = provider.GetRequiredService<ListLoggerWriter>();
            sink.Messages.Should()
                .Contain(m => m.Contains(message) && m.Contains("DBG"));
        }

        [Theory, AutoData]
        public void MessageWithArgs(int arg1, string arg2, Boo arg3)
        {
            var provider = _dependencyCollection
                .AddDefaultLogEnrichers()
                .AddLogWriter<ListLoggerWriter>()
                .BuildProvider();

            var logger = provider.GetRequiredService<ILogger<LoggerTests>>();
            logger.Debug("Test with args {arg1}, {arg2}, {arg3} executed", arg1, arg2, arg3);

            var sink = provider.GetRequiredService<ListLoggerWriter>();
            sink.Messages.Should()
                .Contain(m => m.Contains(arg1.ToString())
                              && m.Contains(arg2)
                              && m.Contains(arg3.String));
        }

        [Theory, AutoData]
        public void SinkDependency(string message)
        {
            var provider = _dependencyCollection
                .AddLogWriter<ListLoggerWriter>()
                .BuildProvider();

            var logger = provider.GetRequiredService<ILogger<LoggerTests>>();
            logger.Debug(message);

            var sink = provider.GetRequiredService<ListLoggerWriter>();
            sink.Messages.Should().Contain(message);
        }
    }
}