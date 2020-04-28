using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.Metrics.Counters;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Metrics
{
    public class CounterShould : TestClass
    {
        private readonly ICounter _counter;
        private readonly Mock<ICounterLabel> _counterLabel;

        public CounterShould(ITestOutputHelper output) : base(output)
        {
            _counterLabel = new Mock<ICounterLabel>();
            _counterLabel
                .SetupGet(counter => counter.Name)
                .Returns(Fixture.Create<string>());

            _counter = new Counter<CounterShould>(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                new[] {_counterLabel.Object});
        }

        [Fact]
        public void ContainsLabel()
        {
            _counter.Labels.Should().Contain(_counterLabel.Object);
        }

        [Fact]
        public void Indexer()
        {
            var labelName = _counterLabel.Object.Name;

            var value = _counter.Increment(labelName);
            _counter[labelName].Should().Be(value);
        }

        [Theory]
        [AutoData]
        public void HasName(string name)
        {
            ICounter counter = new Counter<CounterShould>(name, null, Array.Empty<ICounterLabel>());
            counter.Name.Should().Be(name);
        }

        [Theory]
        [AutoData]
        public void HasDescription(string name, string description)
        {
            ICounter counter = new Counter<CounterShould>(name, description, Array.Empty<ICounterLabel>());
            counter.Description.Should().Be(description);
        }

        [Fact]
        public void IncrementLabel()
        {
            var labelName = _counterLabel.Object.Name;
            _counter.Increment(labelName);

            _counterLabel
                .Verify(label => label
                    .Increment(It.Is<double>(value => Math.Abs(value - 1d) < 0.0001)));
        }

        [Fact]
        public void IncrementLabelTo()
        {
            var labelName = _counterLabel.Object.Name;
            _counter.IncrementTo(labelName, 500d);

            _counterLabel
                .Verify(label => label
                    .IncrementTo(It.Is<double>(value => Math.Abs(value - 500d) < 0.0001d)));
        }

        [Theory]
        [InlineData("")]
        [InlineData((string) null)]
        public void ThrowIfNameNullOrEmpty(string name)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Counter<CounterShould>(name, null, Array.Empty<ICounterLabel>()));
        }

        [Fact]
        public void ThrowIfLabelsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Counter<CounterShould>("abc", null, null));
        }

        [Fact]
        public void ThrowIfLabelsNameNotUnique()
        {
            Assert.Throws<InvalidOperationException>(() => new Counter<CounterShould>("abc", null, new ICounterLabel[]
            {
                new CounterLabel("abc"),
                new CounterLabel("abc")
            }));
        }
    }
}