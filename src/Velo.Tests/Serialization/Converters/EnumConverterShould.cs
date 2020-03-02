using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class EnumConverterShould : TestClass
    {
        private readonly IJsonConverter<DependencyLifetime> _converter;

        public EnumConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<DependencyLifetime>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(DependencyLifetime value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(DependencyLifetime value)
        {
            var number = JsonValue.Number((int) value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(DependencyLifetime value)
        {
            var jsonValue = JsonValue.Number((int) value);
            _converter.ReadObject(jsonValue).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(DependencyLifetime value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(DependencyLifetime value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(DependencyLifetime value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(DependencyLifetime value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Fact]
        public void ThrowIfValueNotExists()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _converter.Deserialize(25.ToString()));
        }

        public static IEnumerable<object[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                foreach (var value in Enum.GetValues(typeof(DependencyLifetime)))
                {
                    yield return new[] {value};
                }
            }
        }
    }
}