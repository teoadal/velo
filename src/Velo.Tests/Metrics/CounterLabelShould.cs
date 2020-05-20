using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Metrics.Counters;
using Xunit;

namespace Velo.Tests.Metrics
{
    public class CounterLabelShould : TestClass
    {
        private readonly ICounterLabel _label;

        public CounterLabelShould()
        {
            _label = new CounterLabel(Fixture.Create<string>());
        }

        [Theory]
        [AutoData]
        public void HasName(string name)
        {
            ICounterLabel label = new CounterLabel(name);
            label.Name.Should().Be(name);
        }

        [Fact]
        public void HasValue()
        {
            _label.IncrementTo(100d);
            _label.Value.Should().Be(100d);
        }

        [Fact]
        public void Increment()
        {
            var lastValue = _label.Value;
            var actual = _label.Increment();

            actual.Should().Be(lastValue + 1d);
        }

        [Theory]
        [AutoData]
        public void IncrementValue(double value)
        {
            var lastValue = _label.Value;
            var actual = _label.Increment(value);

            actual.Should().Be(lastValue + value);
        }

        [Fact]
        public void IncrementValueTo()
        {
            var lastValue = _label.Value;
            const double to = 500d;
            var actual = _label.IncrementTo(to);

            actual.Should().Be(lastValue + to);
        }

        [Fact]
        public void FlushValue()
        {
            _label.IncrementTo(100d);
            
            var value = _label.Flush();
            
            value.Should().Be(100d);

            _label.Value.Should().Be(0d);
        }
        
        [Fact]
        public void NotIncrementValueToIfLess()
        {
            _label.IncrementTo(500d);

            var actual = _label.IncrementTo(300d);

            actual.Should().Be(500d);
        }

        [Theory]
        [InlineData("")]
        [InlineData((string) null)]
        public void ThrowIfNameNullOrEmpty(string name)
        {
            Assert.Throws<ArgumentNullException>(() => new CounterLabel(name));
        }

        [Fact]
        public void ThrowIncrementLessZero()
        {
            _label
                .Invoking(label => label.Increment(-10d))
                .Should().Throw<InvalidOperationException>();
        }
    }
}