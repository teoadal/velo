using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class ObjectConverterShould : TestClass
    {
        private readonly IJsonConverter<Boo> _converter;

        public ObjectConverterShould(ITestOutputHelper output) : base(output)
        {
            _converter = new ConvertersCollection(CultureInfo.InvariantCulture).Get<Boo>();
        }

        [Fact]
        public void NotBePrimitive() => _converter.IsPrimitive.Should().BeFalse();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(Boo value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            _converter.Deserialize(serialized).Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(Boo value)
        {
            var jsonObject = _converter.Write(value);
            _converter.Read(jsonObject).Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(Boo value)
        {
            var jsonObject = _converter.Write(value);
            _converter.ReadObject(jsonObject).Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(Boo value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(Boo value)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(value, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(Boo value)
        {
            var jsonValue = value == null 
                ? JsonValue.Null 
                : _converter.Write(value);
            
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(Boo value)
        {
            var jsonValue = value == null 
                ? JsonValue.Null 
                : _converter.WriteObject(value);
            
            jsonValue.Serialize().Should().Be(JsonConvert.SerializeObject(value));
        }

        public static IEnumerable<object[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                var fixture = new Fixture();
                yield return new object[] {fixture.Create<Boo>()};
                yield return new object[] {fixture.Build<Boo>().OmitAutoProperties().Create()};
                yield return new object[]
                {
                    fixture.Build<Boo>()
                        .Without(b => b.String)
                        .Without(b => b.Values)
                        .Create()
                };
                yield return new object[] {null};
            }
        }
    }
}