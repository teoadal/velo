using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Converters
{
    public class ArrayConverterShould : TestClass
    {
        private readonly IJsonConverter<Boo[]> _converter;
        private readonly IJsonConverter<Boo> _elementConverter;

        public ArrayConverterShould(ITestOutputHelper output) : base(output)
        {
            var converters = BuildConvertersCollection();
            _converter = converters.Get<Boo[]>();
            _elementConverter = converters.Get<Boo>();
        }

        [Fact]
        public void NotBePrimitive() => _converter.IsPrimitive.Should().BeFalse();

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize(Boo[] array)
        {
            var serialized = JsonConvert.SerializeObject(array);
            var deserialized = _converter.Deserialize(serialized);

            for (var i = 0; i < array.Length; i++)
            {
                deserialized[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Theory]
        [AutoData]
        public void DeserializeICollection(ICollection<Boo> collection)
        {
            var serialized = JsonConvert.SerializeObject(collection);
            var deserialized = _converter.Deserialize(serialized);

            var array = collection.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                deserialized[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Theory]
        [AutoData]
        public void DeserializeEnumerable(IEnumerable<Boo> collection)
        {
            var serialized = JsonConvert.SerializeObject(collection);
            var deserialized = _converter.Deserialize(serialized);

            var array = collection.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                deserialized[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Theory]
        [AutoData]
        public void DeserializeIReadonlyCollection(IReadOnlyCollection<Boo> collection)
        {
            var serialized = JsonConvert.SerializeObject(collection);
            var deserialized = _converter.Deserialize(serialized);

            var array = collection.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                deserialized[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Fact]
        public void DeserializeNull()
        {
            var serialized = JsonConvert.SerializeObject(null);
            _converter.Deserialize(serialized).Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void DeserializeValidLength(Boo[] array)
        {
            var serialized = JsonConvert.SerializeObject(array);
            var deserialized = _converter.Deserialize(serialized);

            deserialized.Length.Should().Be(array.Length);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read(Boo[] array)
        {
            var jsonData = new JsonArray(array.Select(_elementConverter.Write));
            var result = _converter.Read(jsonData);

            for (var i = 0; i < array.Length; i++)
            {
                result[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Fact]
        public void ReadNull()
        {
            _converter.Read(JsonValue.Null).Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject(Boo[] array)
        {
            var jsonData = new JsonArray(array.Select(_elementConverter.Write));
            var result = _converter.Read(jsonData);

            for (var i = 0; i < array.Length; i++)
            {
                result[i].Should().BeEquivalentTo(array[i]);
            }
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize(Boo[] array)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(array, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(array));
        }

        [Fact]
        public void SerializeNull()
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(null, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(null));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject(Boo[] array)
        {
            var stringWriter = new StringWriter();
            _converter.SerializeObject(array, stringWriter);

            var result = stringWriter.ToString();
            result.Should().Be(JsonConvert.SerializeObject(array));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Write(Boo[] array)
        {
            var jsonArray = _converter.Write(array);
            jsonArray.Serialize().Should().Be(JsonConvert.SerializeObject(array));
        }

        [Fact]
        public void WriteNull()
        {
            var jsonArray = (JsonValue) _converter.Write(null);
            jsonArray.Serialize().Should().Be(JsonConvert.SerializeObject(null));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject(Boo[] array)
        {
            var jsonArray = _converter.WriteObject(array);
            jsonArray.Serialize().Should().Be(JsonConvert.SerializeObject(array));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteValidLength(Boo[] array)
        {
            var jsonData = (JsonArray) _converter.Write(array);
            jsonData.Length.Should().Be(array.Length);
        }

        public static TheoryData<Boo[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                var fixture = new Fixture();

                return new TheoryData<Boo[]>
                {
                    Array.Empty<Boo>(),
                    fixture.CreateMany<Boo>(1).ToArray(),
                    fixture.CreateMany<Boo>(5).ToArray(),
                    new[] {fixture.Create<Boo>(), null, fixture.Create<Boo>()}
                };
            }
        }
    }
}