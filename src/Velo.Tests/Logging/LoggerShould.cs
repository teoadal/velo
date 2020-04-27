using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public class LoggerShould : LoggingTests
    {
        private const string Template0 = "Test without args executed";
        private const string Template1 = "Test with arg {arg1} executed";
        private const string Template2 = "Test with args {arg1}, {arg2} executed";
        private const string Template3 = "Test with args {arg1}, {arg2}, {arg3} executed";
        private const string Template4 = "Test with args {arg1}, {arg2}, {arg3}, {arg4} executed";
        
        private readonly ILogger<LoggerShould> _logger;
        private readonly Mock<ILogWriter> _logWriter;
        private readonly Type _sender;

        public LoggerShould(ITestOutputHelper output) : base(output)
        {
            _logWriter = new Mock<ILogWriter>();

            var provider = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object)
                .BuildProvider();

            _logger = provider.GetRequiredService<ILogger<LoggerShould>>();
            _sender = typeof(LoggerShould);
        }

        [Fact]
        public void CreatedWithNullProvider()
        {
            var provider = new DependencyCollection()
                .AddLogging()
                .BuildProvider();

            provider.GetService<ILogger<LoggerShould>>().Should().NotBeNull();
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void Log(LogLevel level, DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Log(level, Template0);
            _logger.Log(level, Template1, arg1);
            _logger.Log(level, Template2, arg1, arg2);
            _logger.Log(level, Template3, arg1, arg2, arg3);
            _logger.Log(level, Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(level, Times.Exactly(5));
        }

        [Theory]
        [AutoData]
        public void LogDebug(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Debug(Template0);
            _logger.Debug(Template1, arg1);
            _logger.Debug(Template2, arg1, arg2);
            _logger.Debug(Template3, arg1, arg2, arg3);
            _logger.Debug(Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(LogLevel.Debug, Times.Exactly(5));
        }

        [Theory]
        [AutoData]
        public void LogError(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Error(Template0);
            _logger.Error(Template1, arg1);
            _logger.Error(Template2, arg1, arg2);
            _logger.Error(Template3, arg1, arg2, arg3);
            _logger.Error(Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(LogLevel.Error, Times.Exactly(5));
        }

        [Theory]
        [AutoData]
        public void LogInfo(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Info(Template0);
            _logger.Info(Template1, arg1);
            _logger.Info(Template2, arg1, arg2);
            _logger.Info(Template3, arg1, arg2, arg3);
            _logger.Info(Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(LogLevel.Info, Times.Exactly(5));
        }

        [Theory]
        [AutoData]
        public void LogTrace(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Trace(Template0);
            _logger.Trace(Template1, arg1);
            _logger.Trace(Template2, arg1, arg2);
            _logger.Trace(Template3, arg1, arg2, arg3);
            _logger.Trace(Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(LogLevel.Trace, Times.Exactly(5));
        }

        [Theory]
        [AutoData]
        public void LogWarning(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            _logger.Warning(Template0);
            _logger.Warning(Template1, arg1);
            _logger.Warning(Template2, arg1, arg2);
            _logger.Warning(Template3, arg1, arg2, arg3);
            _logger.Warning(Template4, arg1, arg2, arg3, arg4);
            
            VerifyLogWrite(LogLevel.Warning, Times.Exactly(5));
        }

        [Fact]
        public void Scoped()
        {
            new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object) // if not add any logWriter - static NullProvider
                .GetLifetime<ILogger<LoggerShould>>().Should().Be(DependencyLifetime.Scoped);
        }
        
        [Fact]
        public void WriteMultiThreading()
        {
            const int callCount = 100;
            var boos = Fixture.CreateMany<Boo>(callCount);

            Parallel.ForEach(boos, boo => _logger
                .Debug(Template3, boo.Id, boo.String, boo));

            VerifyLogWrite(LogLevel.Debug, Times.Exactly(callCount));
        }

        [Theory]
        [AutoData]
        public void WriteManyMessages(DateTime arg1, float arg2, Boo arg3, Guid? arg4)
        {
            LogTrace(arg1, arg2, arg3, arg4);
            LogDebug(arg1, arg2, arg3, arg4);
            LogInfo(arg1, arg2, arg3, arg4);
            LogWarning(arg1, arg2, arg3, arg4);
            LogError(arg1, arg2, arg3, arg4);
        }

        private void VerifyLogWrite(LogLevel level, Times? times = null)
        {
            if (times == null) times = Times.AtLeastOnce();

            _logWriter.Verify(writer => writer.Write(
                It.Is<LogContext>(context => 
                    context.Level == level &&
                    context.Sender == _sender),
                It.IsNotNull<JsonObject>()), times.Value);
        }
    }
}