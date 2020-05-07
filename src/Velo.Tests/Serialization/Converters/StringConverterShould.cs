using System;
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
    public class StringConverterShould : TestClass
    {
        private readonly IJsonConverter<string> _converter;

        public StringConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<string>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(string value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(string value)
        {
            var str = JsonValue.String(value);
            _converter.Read(str).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(string value)
        {
            var str = JsonValue.String(value);
            _converter.ReadObject(str).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(string value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(string value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(string value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(string value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static TheoryData<string> Values => new TheoryData<string>
        {
            string.Empty,
            null,
            Guid.NewGuid().ToString("N")
        };
    }
}