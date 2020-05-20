using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Serialization.Models;
using Xunit;

namespace Velo.Tests.Logging.Enrichers
{
    public class LogLevelEnricherShould : LoggingTests
    {
        private readonly LogLevelEnricher _enricher;
        private readonly JsonObject _message;

        public LogLevelEnricherShould()
        {
            _enricher = new LogLevelEnricher();
            _message = new JsonObject();
        }

        [Theory]
        [MemberAutoData(nameof(Levels))]
        public void AddLogLevel(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);

            var verb = (JsonVerbose) _message[LogLevelEnricher.Name];

            switch (level)
            {
                case LogLevel.Trace:
                    verb.Value.Should().Be("TRA");
                    break;
                case LogLevel.Debug:
                    verb.Value.Should().Be("DBG");
                    break;
                case LogLevel.Info:
                    verb.Value.Should().Be("INF");
                    break;
                case LogLevel.Warning:
                    verb.Value.Should().Be("WRN");
                    break;
                case LogLevel.Error:
                    verb.Value.Should().Be("ERR");
                    break;
            }
        }

        [Theory]
        [AutoData]
        public void AddProperty(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);
            _message.Contains(LogLevelEnricher.Name);
        }

        [Theory]
        [AutoData]
        public void AddVerbose(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);
            _message[LogLevelEnricher.Name].Type.Should().Be(JsonDataType.Verbose);
        }

        [Theory]
        [AutoData]
        public void AddMany(LogLevel level, Type sender)
        {
            var messages = Enumerable.Range(0, 100).Select(_ => new JsonObject());
            foreach (var message in messages)
            {
                _enricher.Enrich(level, sender, message);
                message.Contains(LogLevelEnricher.Name).Should().BeTrue();
            }
        }

        [Theory]
        [AutoData]
        public void AddMultiThreading(LogLevel level, Type sender)
        {
            var messages = Enumerable.Range(0, 100).Select(_ => new JsonObject()).ToArray();

            Parallel.ForEach(messages, message => _enricher.Enrich(level, sender, message));

            foreach (var message in messages)
            {
                message.Contains(LogLevelEnricher.Name).Should().BeTrue();
            }
        }

        [Theory]
        [AutoData]
        public void HaveOneVerboseInstance(LogLevel level, Type sender)
        {
            var messages = Many(() => new JsonObject());

            JsonVerbose instance = null;
            foreach (var message in messages)
            {
                _enricher.Enrich(level, sender, message);

                if (instance == null) instance = (JsonVerbose) message[LogLevelEnricher.Name];
                else instance.Should().Be(message[LogLevelEnricher.Name]);
            }
        }
        
        [Fact]
        public void ThrowInvalidLogLevel()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _enricher
                .Enrich((LogLevel) 25, null!, new JsonObject()));
        }
    }
}