using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization.Models
{
    public class JsonObjectShould : TestClass
    {
        private IConvertersCollection _converters;
        
        public JsonObjectShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
        }

        [Theory, AutoData]
        public void Contains(string property, int value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Number(value));

            Assert.True(obj.Contains(property));
        }

        [Theory, AutoData]
        public void Get(string property, bool value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Boolean(value));

            Assert.Equal(JsonValue.Boolean(value), obj[property]);
        }

        [Fact]
        public void HasValidType()
        {
            new JsonObject().Type.Should().Be(JsonDataType.Object);
            JsonObject.Null.Type.Should().Be(JsonDataType.Null);
        }

        [Theory, AutoData]
        public void Read(BigObject source)
        {
            var converter = _converters.Get<BigObject>();

            var jsonObject = (JsonObject) converter.Write(source);
            var result = converter.Read(jsonObject);
            result.Should().BeEquivalentTo(source);
        }
        
        [Theory, AutoData]
        public void Remove(string property)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Null);

            Assert.True(obj.Remove(property));
            Assert.False(obj.Remove(property));
            Assert.False(obj.Contains(property));
        }
        
        [Theory, AutoData]
        public void Write(BigObject source)
        {
            var converter = _converters.Get<BigObject>();

            var jsonArray = (JsonObject) converter.Write(source);
            var result = converter.Read(jsonArray);
            result.Should().BeEquivalentTo(source);
        }
    }
}