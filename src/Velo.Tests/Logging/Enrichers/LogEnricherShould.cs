using System;
using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Writers;
using Velo.Serialization;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging.Enrichers
{
    public class LogEnricherShould : TestClass
    {
        private readonly ConvertersCollection _converters;
        private readonly Mock<ILogEnricher> _logEnricher;
        private readonly ILogger<LogEnricherShould> _logger;
        private readonly Type _sender;
        
        private Action<LogContext, JsonObject> _logWriteCallback = (context, obj) => { };
        
        public LogEnricherShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
            _logEnricher = new Mock<ILogEnricher>();

            var logWriter = new Mock<ILogWriter>();
            logWriter
                .Setup(writer => writer.Write(It.IsAny<LogContext>(), It.IsNotNull<JsonObject>()))
                .Callback(_logWriteCallback);
            
            _logger = new DependencyCollection()
                .AddLogging()
                .AddLogWriter(logWriter.Object)
                .AddLogEnricher(_logEnricher.Object)
                .BuildProvider()
                .GetRequiredService<ILogger<LogEnricherShould>>();

            _sender = typeof(LogEnricherShould);
        }

        [Theory]
        [AutoData]
        public void EnrichMessage(string template, string property, int value)
        {
            _logEnricher
                .Setup(enricher => enricher
                    .Enrich(LogLevel.Debug, _sender, It.IsNotNull<JsonObject>()))
                .Callback<LogLevel, Type, JsonObject>((level, sender, message) => message[property] = JsonValue.Number(value));

            _logWriteCallback = (context, message) => _converters.Read<int>(message[property]).Should().Be(value);
            
            _logger.Debug(template);
        }

        [Theory]
        [AutoData]
        public void EnrichManyMessages((string, string, int)[] values)
        {
            foreach (var (message, property, value) in values)
            {
                EnrichMessage(message, property, value);
            }
        }
    }
}