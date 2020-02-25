using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Logging
{
    public class LoggerShould : TestClass
    {
        private const string Template0 = "Test without args executed";
        private const string Template1 = "Test with arg {arg1} executed";
        private const string Template2 = "Test with args {arg1}, {arg2} executed";
        private const string Template3 = "Test with args {arg1}, {arg2}, {arg3} executed";
        private const string Template4 = "Test with args {arg1}, {arg2}, {arg3}, {arg4} executed";

        private readonly IConvertersCollection _convertersCollection;
        private readonly ILogger<LoggerShould> _logger;

        private readonly Mock<ILogEnricher> _logEnricher;
        private readonly Mock<ILogWriter> _logWriter;
        private readonly TestLogWriter _testLogWriter;

        public LoggerShould(ITestOutputHelper output) : base(output)
        {
            _logEnricher = new Mock<ILogEnricher>();
            _logWriter = new Mock<ILogWriter>();
            _testLogWriter = new TestLogWriter();

            var provider = new DependencyCollection()
                .AddLogging()
                .AddJsonConverter() // for register IConvertersCollection
                .AddDefaultLogEnrichers()
                .AddLogWriter(_testLogWriter)
                .AddLogWriter(_logWriter.Object)
                .AddLogWriter<ConsoleLogWriter>()
                .AddLogEnricher(_logEnricher.Object)
                .AddLogEnricher<TestLogEnricher>()
                .BuildProvider();

            _convertersCollection = provider.GetRequiredService<IConvertersCollection>();
            _logger = provider.GetRequiredService<ILogger<LoggerShould>>();
        }

        [Fact]
        public void BuildMessage0()
        {
            _testLogWriter.Verify = (context, message) => context.Template.Should().Be(Template0);

            _logger.Debug(Template0);
        }

        [Theory, AutoData]
        public void BuildMessage1(DateTime arg1)
        {
            var converter = _convertersCollection.Get<DateTime>();

            _testLogWriter.Verify = (context, message) => converter.Read(message["arg1"]).Should().Be(arg1);

            _logger.Debug(Template1, arg1);
        }

        [Theory, AutoData]
        public void BuildMessage2(TimeSpan arg1, Guid arg2)
        {
            var converter1 = _convertersCollection.Get<TimeSpan>();
            var converter2 = _convertersCollection.Get<Guid>();

            _testLogWriter.Verify = (context, message) =>
            {
                converter1.Read(message["arg1"]).Should().Be(arg1);
                converter2.Read(message["arg2"]).Should().Be(arg2);
            };

            _logger.Debug(Template2, arg1, arg2);
        }

        [Theory, AutoData]
        public void BuildMessage3(int arg1, string arg2, Boo arg3)
        {
            var converter1 = _convertersCollection.Get<int>();
            var converter2 = _convertersCollection.Get<string>();
            var converter3 = _convertersCollection.Get<Boo>();

            _testLogWriter.Verify = (context, message) =>
            {
                converter1.Read(message["arg1"]).Should().Be(arg1);
                converter2.Read(message["arg2"]).Should().Be(arg2);
                converter3.Read(message["arg3"]).Should().BeEquivalentTo(arg3);
            };

            _logger.Debug(Template3, arg1, arg2, arg3);
        }

        [Theory, AutoData]
        public void BuildMessage4(int arg1, string arg2, Boo arg3, TimeSpan? arg4)
        {
            var converter1 = _convertersCollection.Get<int>();
            var converter2 = _convertersCollection.Get<string>();
            var converter3 = _convertersCollection.Get<Boo>();
            var converter4 = _convertersCollection.Get<TimeSpan?>();

            _testLogWriter.Verify = (context, message) =>
            {
                converter1.Read(message["arg1"]).Should().Be(arg1);
                converter2.Read(message["arg2"]).Should().Be(arg2);
                converter3.Read(message["arg3"]).Should().BeEquivalentTo(arg3);
                converter4.Read(message["arg4"]).Should().Be(arg4);
            };

            _logger.Debug(Template4, arg1, arg2, arg3, arg4);
        }

        [Theory, AutoData]
        public void BuildStringWithArgs(int arg1, string arg2, Boo arg3)
        {
            _logger.Debug(Template3, arg1, arg2, arg3);

            _testLogWriter.Messages.Should()
                .Contain(m => m
                    .Contains(arg1.ToString()) && m
                    .Contains(arg2) && m
                    .Contains(arg3.String));
        }

        [Theory, AutoData]
        public void LogWithDifferentLevels(Guid arg1, float arg2, TimeSpan arg3, Boo arg4)
        {
            _testLogWriter.Verify = (context, message) => context.Level.Should().Be(LogLevel.Trace);

            _logger.Trace(Template0);
            _logger.Trace(Template1, arg1);
            _logger.Trace(Template2, arg1, arg2);
            _logger.Trace(Template3, arg1, arg2, arg3);
            _logger.Trace(Template4, arg1, arg2, arg3, arg4);

            _testLogWriter.Verify = (context, message) => context.Level.Should().Be(LogLevel.Debug);

            WriteDebug(_logger, arg1, arg2, arg3, arg4);

            _testLogWriter.Verify = (context, message) => context.Level.Should().Be(LogLevel.Info);

            _logger.Info(Template0);
            _logger.Info(Template1, arg1);
            _logger.Info(Template2, arg1, arg2);
            _logger.Info(Template3, arg1, arg2, arg3);
            _logger.Info(Template4, arg1, arg2, arg3, arg4);

            _testLogWriter.Verify = (context, message) => context.Level.Should().Be(LogLevel.Warning);

            _logger.Warning(Template0);
            _logger.Warning(Template1, arg1);
            _logger.Warning(Template2, arg1, arg2);
            _logger.Warning(Template3, arg1, arg2, arg3);
            _logger.Warning(Template4, arg1, arg2, arg3, arg4);

            _testLogWriter.Verify = (context, message) => context.Level.Should().Be(LogLevel.Error);

            _logger.Error(Template0);
            _logger.Error(Template1, arg1);
            _logger.Error(Template2, arg1, arg2);
            _logger.Error(Template3, arg1, arg2, arg3);
            _logger.Error(Template4, arg1, arg2, arg3, arg4);
        }

        [Theory, AutoData]
        public void NotWriteIfLogLevelLess(Guid arg1, float arg2, TimeSpan arg3, Boo arg4)
        {
            _logWriter.SetupGet(w => w.Level).Returns(LogLevel.Error);

            WriteDebug(_logger, arg1, arg2, arg3, arg4);

            VerifyLogWriting(Times.Never());
        }

        [Theory, AutoData]
        public void NotWriteWithoutWriters(Guid arg1, float arg2, TimeSpan arg3, Boo arg4)
        {
            var provider = new DependencyCollection()
                .AddLogging()
                .BuildProvider();

            var logProvider = provider.GetRequiredService<ILogProvider>();
            logProvider.Should().BeOfType<NullProvider>();

            var logger = provider.GetRequiredService<ILogger<TestClass>>();

            WriteDebug(logger, arg1, arg2, arg3, arg4);
        }

        [Fact]
        public void UsedCustomEnricher()
        {
            const string property = "TEST";

            _logEnricher.Setup(e => e.Enrich(
                    It.IsAny<LogLevel>(), It.IsAny<Type>(), It.IsAny<JsonObject>()))
                .Callback<LogLevel, Type, JsonObject>((level, sender, message) => message.Add(property, JsonValue.Null));

            _testLogWriter.Verify = (context, message) => message[property].Should().Be(JsonValue.Null);

            _logger.Debug("abc");
        }

        [Fact]
        public void WriteMultiThreading()
        {
            const int callCount = 100;
            var boos = Fixture.CreateMany<Boo>(callCount);

            Parallel.ForEach(boos, b => _logger
                .Debug(Template3, b.Id, b.String, b));

            VerifyLogWriting(Times.Exactly(callCount));
        }

        [Theory, AutoData]
        public void WriteManyMessages(Boo[] boos)
        {
            foreach (var boo in boos)
            {
                _logger.Debug(Template3, boo.Id, boo.String, boo);
            }

            VerifyLogWriting();
        }

        [Theory, AutoData]
        public void WriteWithoutEnrichers(Guid arg1, float arg2, TimeSpan arg3, Boo arg4)
        {
            var provider = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object)
                .BuildProvider();

            var logger = provider.GetRequiredService<ILogger<TestClass>>();

            WriteDebug(logger, arg1, arg2, arg3, arg4);

            _logWriter.Verify(w => w.Write(
                It.IsAny<LogContext>(),
                It.IsAny<JsonObject>()), Times.Exactly(5));
        }

        [Theory, AutoData]
        public void ThrowIfTemplateUsedWrong(int arg1, string arg2, Boo wrong1, DateTime? wrong2)
        {
            _logger.Debug(Template2, arg1, arg2);
            Assert.Throws<InvalidCastException>(() => _logger.Error(Template2, wrong1, wrong2));
        }

        private void VerifyLogWriting(Times? times = null)
        {
            if (times == null) times = Times.AtLeastOnce();

            _logWriter.Verify(w => w.Write(
                It.IsAny<LogContext>(),
                It.IsAny<JsonObject>()), times.Value);
        }

        private static void WriteDebug<T>(ILogger<T> logger, Guid arg1, float arg2, TimeSpan arg3, Boo arg4)
        {
            logger.Debug(Template0);
            logger.Debug(Template1, arg1);
            logger.Debug(Template2, arg1, arg2);
            logger.Debug(Template3, arg1, arg2, arg3);
            logger.Debug(Template4, arg1, arg2, arg3, arg4);
        }
    }
}