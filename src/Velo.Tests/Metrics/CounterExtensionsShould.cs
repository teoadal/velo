using System.Globalization;
using System.IO;
using AutoFixture;
using FluentAssertions;
using Velo.Metrics.Counters;
using Velo.Serialization.Primitives;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Metrics
{
    public class CounterExtensionsShould : TestClass
    {
        private readonly ICounter _counter;
        private readonly ICounterLabel _counterLabel;
        private readonly double _value;

        public CounterExtensionsShould(ITestOutputHelper output) : base(output)
        {
            _value = 100d;
            _counterLabel = new CounterLabel("abc");
            _counterLabel.IncrementTo(_value);

            _counter = new Counter<CounterExtensionsShould>(Fixture.Create<string>(), null, new[]
            {
                _counterLabel
            });
        }

        [Fact]
        public void FlushToTextWriter()
        {
            var stringWriter = new StringWriter();
            _counter.Flush(stringWriter);

            var str = stringWriter.ToString();

            str.Should().Contain(_counterLabel.Name);

            var expected = _value.ToString(DoubleConverter.Pattern, CultureInfo.InvariantCulture);
            str.Should().Contain(expected);
        }

        [Fact]
        public void FlushToDictionary()
        {
            var dictionary = _counter.Flush();

            dictionary.Should().ContainKey(_counterLabel.Name);
            dictionary.Should().ContainValue(_value);
        }

        [Fact]
        public void ReadToTextWriter()
        {
            var stringWriter = new StringWriter();
            _counter.Read(stringWriter);

            var str = stringWriter.ToString();

            str.Should().Contain(_counterLabel.Name);

            var expected = _counterLabel.Value.ToString(DoubleConverter.Pattern, CultureInfo.InvariantCulture);
            str.Should().Contain(expected);
        }

        [Fact]
        public void ReadToDictionary()
        {
            var dictionary = _counter.Read();

            dictionary.Should().ContainKey(_counterLabel.Name);
            dictionary.Should().ContainValue(_counterLabel.Value);
        }
    }
}