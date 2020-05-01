using System;
using System.IO;
using System.Text;
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
    public class DefaultFileWriterShould : LoggingTests
    {
        private readonly string _fileName;
        private readonly Type _sender;
        private readonly DefaultFileWriter _writer;

        public DefaultFileWriterShould(ITestOutputHelper output) : base(output)
        {
            _fileName = $"{nameof(DefaultFileWriterShould)}.log";
            _sender = typeof(DefaultFileWriterShould);
            _writer = new DefaultFileWriter(_fileName);
        }

        [Fact]
        public void DisposeInternalWriter()
        {
            _writer
                .Invoking(writer => writer.Dispose())
                .Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(Levels))]
        public void HasValidLevel(LogLevel level)
        {
            using var writer = new DefaultFileWriter($"{nameof(HasValidLevel)}", level);
            writer.Level.Should().Be(level);
        }

        [Theory]
        [AutoData]
        public void Write(string template)
        {
            var context = new LogContext(LogLevel.Debug, _sender, template);
            _writer.Write(context, new JsonObject());

            CheckFileContains(template);
        }

        [Fact]
        public void WriteMany()
        {
            for (var i = 0; i < 100; i++)
            {
                var template = $"template_{i}";
                var context = new LogContext(LogLevel.Debug, _sender, template);
                _writer.Write(context, new JsonObject());

                CheckFileContains(template);
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

            for (var i = 0; i < 100; i++)
            {
                CheckFileContains(BuildTemplate(i));
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

        private void CheckFileContains(string value)
        {
            using var fileStream = File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            reader.ReadToEnd().Should().Contain(value);
        }
        
        public override void Dispose()
        {
            _writer.Dispose();
            base.Dispose();
        }
    }
}