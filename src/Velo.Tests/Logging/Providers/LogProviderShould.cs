using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.Logging.Providers
{
    public class LogProviderShould : LoggingTests
    {
        private readonly IConvertersCollection _converters;
        private readonly ILogger<LogProviderShould> _logger;
        private readonly Mock<ILogWriter> _logWriter;
        private readonly Type _sender;

        private Action<LogContext, JsonObject> _logWriteCallback = (context, obj) => { };

        public LogProviderShould()
        {
            _logWriter = new Mock<ILogWriter>();
            _logWriter
                .Setup(writer => writer.Write(It.IsAny<LogContext>(), It.IsAny<JsonObject>()))
                .Callback(_logWriteCallback);

            var dependencyProvider = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object)
                .BuildProvider();

            _converters = dependencyProvider.GetRequired<IConvertersCollection>();
            _logger = dependencyProvider.GetRequired<ILogger<LogProviderShould>>();
            _sender = typeof(LogProviderShould);
        }

        [Fact]
        public void BeNullIfNoWriters()
        {
            var provider = new DependencyCollection()
                .AddLogging()
                .BuildProvider();

            var logProvider = provider.GetRequired<ILogProvider>();
            logProvider.Should().BeOfType<NullLogProvider>();
        }

        [Fact]
        public void BeScoped()
        {
            var dependencies = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object); // if not add any logWriter - static NullProvider

            dependencies.GetLifetime<ILogProvider>().Should().Be(DependencyLifetime.Scoped);
        }

        [Theory]
        [AutoData]
        public void NotWriteIfLogLevelGreater(string message)
        {
            _logWriter.SetupGet(writer => writer.Level).Returns(LogLevel.Error);

            _logger.Trace(message);

            _logWriter.Verify(writer => writer
                .Write(It.IsAny<LogContext>(), It.IsAny<JsonObject>()), Times.Never);
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void UseEnricher(LogLevel level, string message)
        {
            var enricher = new Mock<ILogEnricher>();

            var logger = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(Mock.Of<ILogWriter>())
                .AddLogEnricher(enricher.Object)
                .BuildProvider()
                .GetRequired<ILogger<LogProviderShould>>();

            logger.Log(level, message);

            enricher.Verify(e => e.Enrich(level, _sender, It.IsNotNull<JsonObject>()));
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void UseEnrichers(LogLevel level, string message)
        {
            var dependencies = new DependencyCollection().AddLogging();
            var enrichers = Many(() => new Mock<ILogEnricher>());

            foreach (var enricher in enrichers)
            {
                dependencies.AddLogEnricher(enricher.Object);
            }

            dependencies
                .AddLogWriter(Mock.Of<ILogWriter>())
                .BuildProvider()
                .GetRequired<ILogger<LogProviderShould>>()
                .Log(level, message);

            foreach (var enricher in enrichers)
            {
                enricher.Verify(e => e
                    .Enrich(level, _sender, It.IsNotNull<JsonObject>()));
            }
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void WriteLogContext(LogLevel level, string message)
        {
            _logger.Log(level, message);

            _logWriter.Verify(writer => writer
                .Write(It.Is<LogContext>(context =>
                    context.Level == level &&
                    context.Sender == _sender &&
                    context.Template == message), It.IsNotNull<JsonObject>()));
        }

        [Theory]
        [AutoData]
        public void WriteLogMessage(int arg1, Guid arg2, Boo arg3)
        {
            _logWriteCallback = (context, message) =>
            {
                _converters.Read<int>(message[nameof(arg1)]).Should().Be(arg1);
                _converters.Read<Guid>(message[nameof(arg2)]).Should().Be(arg2);
                _converters.Read<Boo>(message[nameof(arg3)]).Should().BeEquivalentTo(arg3);
            };

            _logger.Debug("Template {arg1}, {arg2}, {arg3}", arg1, arg2, arg3);

            _logWriter.Verify(writer => writer
                .Write(It.IsAny<LogContext>(), It.IsNotNull<JsonObject>()));
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void WriteWriters(LogLevel level, string message)
        {
            var dependencies = new DependencyCollection().AddLogging();
            var writers = Many(() => new Mock<ILogWriter>());

            foreach (var writer in writers)
            {
                dependencies.AddLogWriter(writer.Object);
            }

            dependencies.BuildProvider()
                .GetRequired<ILogger<LogProviderShould>>()
                .Log(level, message);

            foreach (var writer in writers)
            {
                writer.Verify(w => w
                    .Write(It.Is<LogContext>(context =>
                            context.Level == level &&
                            context.Sender == _sender &&
                            context.Template == message),
                        It.IsNotNull<JsonObject>()));
            }
        }

        [Theory]
        [AutoData]
        public void ThrowIfTemplateUsedWrong(int arg1, string arg2, Boo wrong1, DateTime? wrong2)
        {
            const string template = "Template {arg1} and {arg2}";

            _logger.Debug(template, arg1, arg2);
            Assert.Throws<InvalidCastException>(() => _logger.Error(template, wrong1, wrong2));
        }

        [Theory]
        [AutoData]
        public void ThrowIfTemplateNull(int arg1, Guid arg2, float arg3, Boo arg4)
        {
            Assert.Throws<ArgumentNullException>(() => _logger.Debug(null!));
            Assert.Throws<ArgumentNullException>(() => _logger.Debug(null!, arg1));
            Assert.Throws<ArgumentNullException>(() => _logger.Debug(null!, arg1, arg2));
            Assert.Throws<ArgumentNullException>(() => _logger.Debug(null!, arg1, arg2, arg3));
            Assert.Throws<ArgumentNullException>(() => _logger.Debug(null!, arg1, arg2, arg3, arg4));
        }
    }
}