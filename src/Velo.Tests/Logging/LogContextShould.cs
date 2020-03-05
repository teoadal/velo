using System;
using System.IO;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.Logging;
using Velo.Logging.Formatters;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public class LogContextShould : TestClass
    {
        private readonly Mock<ILogFormatter> _formatter;
        private readonly Type _sender;

        public LogContextShould(ITestOutputHelper output) : base(output)
        {
            _formatter = new Mock<ILogFormatter>();
            _sender = typeof(LogContextShould);
        }

        [Theory]
        [InlineAutoData(LogLevel.Trace)]
        [InlineAutoData(LogLevel.Debug)]
        [InlineAutoData(LogLevel.Info)]
        [InlineAutoData(LogLevel.Warning)]
        [InlineAutoData(LogLevel.Error)]
        public void HasValidData(LogLevel logLevel, string template)
        {
            var context = new LogContext(logLevel, _sender, _formatter.Object, template);

            context.Level.Should().Be(logLevel);
            context.Sender.Should().Be(_sender);
            context.Template.Should().Be(template);
        }

        [Theory, AutoData]
        public void NotUseFormatterIfNotExists(string template)
        {
            var context = new LogContext(LogLevel.Debug, _sender, null, template);
            var message = new JsonObject();

            context.RenderMessage(message).Should().BeEquivalentTo(template);
        }

        [Fact]
        public void UseFormatter()
        {
            var context = new LogContext(LogLevel.Debug, _sender, _formatter.Object, "Template");
            var message = new JsonObject();

            context.RenderMessage(message);

            _formatter.Verify(formatter => formatter
                .Write(message, It.IsNotNull<TextWriter>()));
        }
    }
}