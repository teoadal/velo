using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Models;
using Xunit;

namespace Velo.Tests.Serialization.Converters
{
    public class LongConverterShould : TestClass
    {
        private readonly IJsonConverter<long> _converter;

        public LongConverterShould()
        {
            _converter = BuildConvertersCollection().Get<long>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(long value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(long value)
        {
            var number = JsonValue.Number(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(long value)
        {
            var jsonValue = JsonValue.Number(value);
            _converter.ReadObject(jsonValue).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(long value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(long value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(long value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(long value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static TheoryData<long> Values = new TheoryData<long>
        {
            long.MaxValue,
            long.MinValue,
            1L,
            -1L,
            100L,
            -100L,
            0L
        };
    }
}