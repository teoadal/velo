using System.IO;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class ObjectConverterShould : TestClass
    {
        private readonly IJsonConverter<Boo> _converter;
        private readonly IConvertersCollection _converters;

        public ObjectConverterShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection();
            _converter = _converters.Get<Boo>();
        }

        [Theory]
        [AutoData]
        public void IgnorePropertyIfIgnoreAttributeExists(IgnoreModel model)
        {
            var converter = (IJsonConverter) _converters.Get<IgnoreModel>();

            var stringWriter = new StringWriter();
            converter.SerializeObject(model, stringWriter);

            stringWriter.ToString().Should()
                .NotContain(nameof(IgnoreModel.Ignored))
                .And.Contain(nameof(IgnoreModel.Value));
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

        [Fact]
        public void UseCustomConverterForDeserialize()
        {
            var converter = (IJsonConverter) _converters.Get<CustomConverterModel>();

            var serialized = new JsonObject();
            serialized.Add(nameof(CustomConverterModel.Value), JsonValue.String("one"));
            var model = (CustomConverterModel) converter.ReadObject(serialized);

            model.Value.Should().Be(1);
        }

        [Fact]
        public void UseCustomConverterForSerialize()
        {
            var converter = (IJsonConverter) _converters.Get<CustomConverterModel>();

            var stringWriter = new StringWriter();
            converter.SerializeObject(new CustomConverterModel(), stringWriter);

            stringWriter.ToString().Should().Contain("one");
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

        public static TheoryData<Boo> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                var fixture = new Fixture();

                return new TheoryData<Boo>
                {
                    fixture.Create<Boo>(),
                    fixture.Build<Boo>().OmitAutoProperties().Create(),
                    fixture.Build<Boo>()
                        .Without(b => b.String)
                        .Without(b => b.Values)
                        .Create(),
                    null
                };
            }
        }
    }
}