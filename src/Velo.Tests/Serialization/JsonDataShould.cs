using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class JsonDataShould : TestClass
    {
        private readonly IConvertersCollection _converters;

        public JsonDataShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
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
    }
}