using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.Metrics.Counters;
using Velo.Metrics.Provider;
using Xunit;

namespace Velo.Tests.Metrics
{
    public class MetricsServiceShould : TestClass
    {
        private readonly Mock<ICounter> _counter;
        private readonly IMetricsProvider _metrics;

        public MetricsServiceShould()
        {
            _counter = new Mock<ICounter>();
            _counter
                .SetupGet(counter => counter.Name)
                .Returns(Fixture.Create<string>());

            _metrics = new MetricsProvider(new[] {_counter.Object});
        }

        [Fact]
        public void ContainsCounter()
        {
            _metrics.Counters.Should().Contain(_counter.Object);
        }

        [Fact]
        public void GetCounter()
        {
            var counter = _metrics.GetCounter(_counter.Object.Name);
            counter.Should().Be(_counter.Object);
        }

        [Fact]
        public void ThrowCounterNotFound()
        {
            _metrics
                .Invoking(metrics => metrics.GetCounter("abc"))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}