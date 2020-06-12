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
    public class TimeStampEnricherShould : TestClass
    {
        private readonly TimeStampEnricher _enricher;
        private readonly JsonObject _message;
        
        public TimeStampEnricherShould()
        {
            _enricher = new TimeStampEnricher("s");
            _message = new JsonObject();
        }

        [Theory]
        [AutoData]
        public void AddProperty(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);
            _message.Contains(TimeStampEnricher.Name);
        }

        [Theory]
        [AutoData]
        public void AddDateTime(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);
            DateTime.TryParse(((JsonVerbose)_message[TimeStampEnricher.Name]).Value, out _).Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public void AddVerbose(LogLevel level, Type sender)
        {
            _enricher.Enrich(level, sender, _message);
            _message[TimeStampEnricher.Name].Type.Should().Be(JsonDataType.Verbose);
        }

        [Theory]
        [AutoData]
        public void AddMany(LogLevel level, Type sender)
        {
            var messages = Enumerable.Range(0, 100).Select(_ => new JsonObject());
            foreach (var message in messages)
            {
                _enricher.Enrich(level, sender, message);
                message.Contains(TimeStampEnricher.Name).Should().BeTrue();
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
                message.Contains(TimeStampEnricher.Name).Should().BeTrue();
            }
        }
    }
}