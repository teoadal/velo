using System;
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
    public class DateTimeConverterShould : TestClass
    {
        private readonly IJsonConverter<DateTime> _converter;

        public DateTimeConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<DateTime>();
        }

        [Fact]
        public void BePrimitive() => _converter.IsPrimitive.Should().BeTrue();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(DateTime value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(DateTime value)
        {
            var number = JsonValue.DateTime(value);
            _converter.Read(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(DateTime value)
        {
            var number = JsonValue.DateTime(value);
            _converter.ReadObject(number).Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(DateTime value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(DateTime value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(DateTime value)
        {
            var jsonValue = (JsonValue) _converter.Write(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(DateTime value)
        {
            var jsonValue = (JsonValue) _converter.WriteObject(value);
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static IEnumerable<object[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new object[] {DateTime.MaxValue};
                yield return new object[] {DateTime.MinValue};
                yield return new object[] {DateTime.Now};
                yield return new object[] {DateTime.UtcNow};
                yield return new object[] {DateTime.Now.Date};
                yield return new object[] {new DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Local)};
                yield return new object[] {new DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Unspecified)};
                yield return new object[] {new DateTime()};
            }
        }
    }
}