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
    public class FloatConverterShould : TestClass
    {
        private readonly IJsonConverter<float> _converter;

        public FloatConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<float>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(float value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(float value)
        {
            var number = JsonValue.Number(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(float value)
        {
            var number = JsonValue.Number(value);
            _converter.ReadObject(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(float value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(float value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(float value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(float value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static TheoryData<float> Values => new TheoryData<float>
        {
            0f,
            -1.0f,
            1.0f,
            -1.1f,
            1.1f,
            -1.12f,
            1.12f,
            1.123456f,
            -1.123456f,
            0.123456f,
            -0.123456f,
            50f,
            -50f,
            50.1234f,
            -50.1234f
        };
    }
}