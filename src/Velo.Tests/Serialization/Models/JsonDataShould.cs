using System;
using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Models
{
    public sealed class JsonDataShould : TestClass
    {
        private readonly IConvertersCollection _converters;

        public JsonDataShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
        }

        [Theory, AutoData]
        public void ParseArrayFromString(int[] array)
        {
            var serialized = JsonConvert.SerializeObject(array);
            var result = (JsonArray) JsonData.Parse(serialized);

            result.Length.Should().Be(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                var expected = array[i];
                var element = (JsonValue) result[i];

                element.Type.Should().Be(JsonDataType.Number);
                element.Value.Should().Be(expected.ToString());
            }
        }

        [Theory, AutoData]
        public void ParseDateTimeFromString(DateTime source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var converter = _converters.Get<DateTime>();
            var result = converter.Read(JsonData.Parse(serialized));

            result.Should().Be(source);
        }

        [Theory, AutoData]
        public void ParseGuidFromString(Guid source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var converter = _converters.Get<Guid>();
            var result = converter.Read(JsonData.Parse(serialized));

            result.Should().Be(source);
        }

        [Theory, AutoData]
        public void ParseIntFromString(int source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var converter = _converters.Get<int>();
            var result = converter.Read(JsonData.Parse(serialized));

            result.Should().Be(source);
        }

        [Theory, AutoData]
        public void ParseObjectFromString(BigObject source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var converter = _converters.Get<BigObject>();
            var result = converter.Read((JsonObject) JsonData.Parse(serialized));

            result.Should().BeEquivalentTo(source);
        }
        
        [Theory, AutoData]
        public void ParseObjectArrayFromString(BigObject[] array)
        {
            var serialized = JsonConvert.SerializeObject(array);
            var result = (JsonArray) JsonData.Parse(serialized);

            result.Length.Should().Be(array.Length);

            var converter = _converters.Get<BigObject>();
            for (var i = 0; i < array.Length; i++)
            {
                var expected = array[i];
                var element = converter.Read(result[i]);
                
                element.Should().BeEquivalentTo(expected);
            }
        }
    }
}