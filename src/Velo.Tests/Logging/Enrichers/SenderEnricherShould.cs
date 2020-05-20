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
    public class SenderEnricherShould : TestClass
    {
        private readonly SenderEnricher _enricher;
        private readonly JsonObject _message;
        private readonly Type _sender;

        public SenderEnricherShould()
        {
            _enricher = new SenderEnricher();
            _message = new JsonObject();
            _sender = typeof(SenderEnricherShould);
        }

        [Theory]
        [AutoData]
        public void AddProperty(LogLevel level)
        {
            _enricher.Enrich(level, _sender, _message);
            _message.Contains(SenderEnricher.Name);
        }

        [Theory]
        [AutoData]
        public void AddVerbose(LogLevel level)
        {
            _enricher.Enrich(level, _sender, _message);
            _message[SenderEnricher.Name].Type.Should().Be(JsonDataType.Verbose);
        }

        [Theory]
        [AutoData]
        public void AddMany(LogLevel level)
        {
            var messages = Enumerable.Range(0, 100).Select(_ => new JsonObject());
            foreach (var message in messages)
            {
                _enricher.Enrich(level, _sender, message);
                message.Contains(SenderEnricher.Name).Should().BeTrue();
            }
        }

        [Theory]
        [AutoData]
        public void AddMultiThreading(LogLevel level)
        {
            var messages = Many(() => new JsonObject());

            Parallel.ForEach(messages, message => _enricher.Enrich(level, _sender, message));

            foreach (var message in messages)
            {
                message.Contains(SenderEnricher.Name).Should().BeTrue();
            }
        }

        [Theory]
        [AutoData]
        public void HaveOneVerboseInstance(LogLevel level)
        {
            var messages = Many(() => new JsonObject());

            JsonVerbose instance = null;
            foreach (var message in messages)
            {
                _enricher.Enrich(level, _sender, message);

                if (instance == null) instance = (JsonVerbose) message[SenderEnricher.Name];
                else instance.Should().Be(message[SenderEnricher.Name]);
            }
        }
    }
}