using System.IO;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Logging.Formatters;
using Velo.Serialization.Models;
using Xunit;

namespace Velo.Tests.Logging.Formatters
{
    public class DefaultStringFormatterShould : TestClass
    {
        private const string Template = "Template {arg1} and {arg2}";
        private readonly ILogFormatter _formatter;

        public DefaultStringFormatterShould()
        {
            _formatter = new DefaultStringFormatter(Template);
        }

        [Theory]
        [InlineAutoData(1)]
        [InlineAutoData(2)]
        [InlineAutoData(3)]
        [InlineAutoData(4)]
        [InlineAutoData(10)]
        public void ParseTemplate(int argsCount, string templateStart)
        {
            var template = templateStart + string.Join(", ", Enumerable
                .Range(0, argsCount)
                .Select(i => $"{{arg{i}}}"));

            var formatter = new DefaultStringFormatter(template);

            formatter.Should().NotBeNull();
        }

        [Theory]
        [AutoData]
        public void Write(int arg1, string arg2)
        {
            var message = new JsonObject
            {
                [nameof(arg1)] = JsonValue.Number(arg1),
                [nameof(arg2)] = JsonValue.String(arg2)
            };

            var stringWriter = new StringWriter();
            _formatter.Write(message, stringWriter);
            var result = stringWriter.ToString();

            result.Should().Be(Template
                .Replace("{arg1}", arg1.ToString())
                .Replace("{arg2}", $"\"{arg2}\""));
        }

        [Theory]
        [AutoData]
        public void WritePrefix(int arg1, string arg2)
        {
            var message = new JsonObject
            {
                [nameof(arg1)] = JsonValue.Number(arg1),
                [nameof(arg2)] = JsonValue.String(arg2),
                ["_prefix"] = new JsonVerbose("PREFIX")
            };

            var stringWriter = new StringWriter();
            _formatter.Write(message, stringWriter);
            var result = stringWriter.ToString();

            result.Should().StartWith("[PREFIX] ");
        }
    }
}