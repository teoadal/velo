using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class DoubleConverterShould : TestClass
    {
        private readonly IJsonConverter<double> _converter;

        public DoubleConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<double>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(double value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(double value)
        {
            var number = JsonValue.Number(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(double value)
        {
            var number = JsonValue.Number(value);
            _converter.ReadObject(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(double value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(double value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(double value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(double value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static IEnumerable<object[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new object[] {0d};
                yield return new object[] {-1.0d};
                yield return new object[] {1.0d};
                yield return new object[] {-1.1d};
                yield return new object[] {1.1d};
                yield return new object[] {-1.12d};
                yield return new object[] {1.12d};
                yield return new object[] {1.1234567890d};
                yield return new object[] {-1.1234567890d};
                yield return new object[] {0.1234567890d};
                yield return new object[] {-0.1234567890d};
                yield return new object[] {50d};
                yield return new object[] {-50d};
                yield return new object[] {50.12356789d};
                yield return new object[] {-50.123456789d};
            }
        }
    }
}