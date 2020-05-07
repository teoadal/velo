using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class BoolConverterShould : TestClass
    {
        private readonly IJsonConverter<bool> _converter;

        public BoolConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<bool>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(bool value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(bool value)
        {
            var number = JsonValue.Boolean(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(bool value)
        {
            var jsonValue = JsonValue.Boolean(value);
            _converter.ReadObject(jsonValue).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(bool value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(bool value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(bool value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(bool value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static TheoryData<bool> Values => new TheoryData<bool> {true, false};
    }
}