using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging.Writers
{
    public class DefaultConsoleWriterShould : LoggingTests
    {
        private readonly TextWriter _output;
        private readonly Type _sender;
        private readonly DefaultConsoleWriter _writer;

        public DefaultConsoleWriterShould(ITestOutputHelper output) : base(output)
        {
            _output = new StringWriter();
            Console.SetOut(_output);
            _sender = typeof(DefaultConsoleWriterShould);
            _writer = new DefaultConsoleWriter();
        }

        [Fact]
        public void DisposeInternalWriters()
        {
            _writer
                .Invoking(writer => writer.Dispose())
                .Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(Levels))]
        public void HasValidLevel(LogLevel level)
        {
            var writer = new DefaultConsoleWriter(level);
            writer.Level.Should().Be(level);
        }

        [Theory]
        [AutoData]
        public void Write(string template)
        {
            var context = new LogContext(LogLevel.Debug, _sender, template);
            _writer.Write(context, new JsonObject());

            _output.ToString().Should().Contain(template);
        }

        [Fact]
        public void WriteMany()
        {
            for (var i = 0; i < 100; i++)
            {
                var template = $"template_{i}";
                var context = new LogContext(LogLevel.Debug, _sender, template);
                _writer.Write(context, new JsonObject());

                _output.ToString().Should().Contain(template);
            }
        }

        [Fact]
        public void WriteParallel()
        {
            static string BuildTemplate(int i) => $"template_{i}";

            Parallel.For(0, 100, i =>
            {
                var context = new LogContext(LogLevel.Debug, _sender, BuildTemplate(i));
                _writer.Write(context, new JsonObject());
            });

            var data = _output.ToString();

            for (var i = 0; i < 100; i++)
            {
                data.Should().Contain(BuildTemplate(i));
            }
        }
        
        [Fact]
        public void ThrowDisposed()
        {
            _writer.Dispose();
            
            var context = new LogContext(LogLevel.Debug, _sender, "123");
            _writer
                .Invoking(writer => writer.Write(context, new JsonObject()))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}