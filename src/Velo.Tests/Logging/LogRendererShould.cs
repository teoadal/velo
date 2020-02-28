using System;
using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public class LogRendererShould : TestClass
    {
        private const string Template1 = "Test with arg {arg1} executed";
        private const string Template2 = "Test with args {arg1}, {arg2} executed";
        private const string Template3 = "Test with args {arg1}, {arg2}, {arg3} executed";
        private const string Template4 = "Test with args {arg1}, {arg2}, {arg3}, {arg4} executed";

        private readonly ConvertersCollection _converters;
        private readonly ILogger<LogRendererShould> _logger;
        private readonly Mock<ILogWriter> _logWriter;
        private Action<LogContext, JsonObject> _logWriteCallback = (context, obj) => { };

        public LogRendererShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);

            _logWriter = new Mock<ILogWriter>();
            _logWriter
                .Setup(writer => writer.Write(It.IsAny<LogContext>(), It.IsAny<JsonObject>()))
                .Callback(_logWriteCallback);

            var provider = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(_logWriter.Object)
                .BuildProvider();

            _logger = provider.GetRequiredService<ILogger<LogRendererShould>>();
        }

        [Theory, AutoData]
        public void BuildMessage(string template)
        {
            _logger.Debug(template);

            _logWriter.Verify(writer => writer
                .Write(It.IsAny<LogContext>(), It.IsNotNull<JsonObject>()));
        }

        [Theory, AutoData]
        public void BuildMessage1(int arg1)
        {
            _logWriteCallback = (context, message) =>
                _converters.Read<int>(message[nameof(arg1)]).Should().Be(arg1);

            _logger.Debug(Template1, arg1);
        }

        [Theory, AutoData]
        public void BuildMessage2(int arg1, string arg2)
        {
            _logWriteCallback = (context, message) =>
            {
                _converters.Read<int>(message[nameof(arg1)]).Should().Be(arg1);
                _converters.Read<string>(message[nameof(arg2)]).Should().Be(arg2);
            };

            _logger.Debug(Template2, arg1, arg2);
        }

        [Theory, AutoData]
        public void BuildMessage3(int arg1, string arg2, Boo arg3)
        {
            _logWriteCallback = (context, message) =>
            {
                _converters.Read<int>(message[nameof(arg1)]).Should().Be(arg1);
                _converters.Read<string>(message[nameof(arg2)]).Should().Be(arg2);
                _converters.Read<Boo>(message[nameof(arg3)]).Should().BeEquivalentTo(arg3);
            };

            _logger.Debug(Template3, arg1, arg2, arg3);
        }

        [Theory, AutoData]
        public void BuildMessage4(int arg1, string arg2, Boo arg3, TimeSpan? arg4)
        {
            _logWriteCallback = (context, message) =>
            {
                _converters.Read<int>(message[nameof(arg1)]).Should().Be(arg1);
                _converters.Read<string>(message[nameof(arg2)]).Should().Be(arg2);
                _converters.Read<Boo>(message[nameof(arg3)]).Should().BeEquivalentTo(arg3);
                _converters.Read<TimeSpan?>(message[nameof(arg4)]).Should().Be(arg4);
            };

            _logger.Debug(Template4, arg1, arg2, arg3, arg4);
        }

        [Theory, AutoData]
        public void RenderString(int arg1, string arg2, Boo arg3)
        {
            _logWriteCallback = (context, message) => context.RenderMessage(message)
                .Should()
                .ContainAll(arg1.ToString(), arg2, arg3.String);

            _logger.Debug(Template3, arg1, arg2, arg3);
        }
    }
}