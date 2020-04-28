using System;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class TimeSpanConverterShould : TestClass
    {
        private readonly IJsonConverter<TimeSpan> _converter;

        public TimeSpanConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<TimeSpan>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(TimeSpan value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(TimeSpan value)
        {
            var number = JsonValue.TimeSpan(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(TimeSpan value)
        {
            var number = JsonValue.TimeSpan(value);
            _converter.ReadObject(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(TimeSpan value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(TimeSpan value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(TimeSpan value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(TimeSpan value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static TheoryData<TimeSpan> Values => new TheoryData<TimeSpan>()
        {
            TimeSpan.Zero,
            TimeSpan.MaxValue,
            TimeSpan.MinValue,
            TimeSpan.FromMilliseconds(21),
            TimeSpan.FromDays(3),
            TimeSpan.FromMinutes(3),
            -TimeSpan.FromHours(3),
        };
    }
}