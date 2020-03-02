using System.Globalization;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Models
{
    public class JsonObjectShould : TestClass
    {
        private readonly IConvertersCollection _converters;
        
        public JsonObjectShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
        }

        [Theory, AutoData]
        public void Contains(string property, int value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Number(value));

            obj.Contains(property).Should().BeTrue();
        }

        [Theory, AutoData]
        public void Get(string property, bool value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Boolean(value));

            obj[property].Should().Be(JsonValue.Boolean(value));
        }

        [Fact]
        public void HasValidType()
        {
            new JsonObject().Type.Should().Be(JsonDataType.Object);
        }

        [Theory, AutoData]
        public void Read(BigObject source)
        {
            var converter = _converters.Get<BigObject>();

            var jsonObject = (JsonObject) converter.Write(source);
            var result = converter.Read(jsonObject);
            result.Should().BeEquivalentTo(source);
        }

        [Fact]
        public void ReadNull()
        {
            BigObject source = null;
            var converter = _converters.Get<BigObject>();

            // ReSharper disable once ExpressionIsAlwaysNull
            var jsonObject = converter.Write(source);
            var result = converter.Read(jsonObject);
            result.Should().BeNull();
        }
        
        [Theory, AutoData]
        public void Remove(string property)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Null);


            obj.Remove(property).Should().BeTrue();
            obj.Remove(property).Should().BeFalse();
            obj.Contains(property).Should().BeFalse();
        }
        
        [Theory, AutoData]
        public void Write(BigObject source)
        {
            var converter = _converters.Get<BigObject>();

            var jsonObject = converter.Write(source);
            var result = converter.Read(jsonObject);
            result.Should().BeEquivalentTo(source);
        }
        
        [Fact]
        public void WriteNull()
        {
            BigObject source = null;
            var converter = _converters.Get<BigObject>();

            // ReSharper disable once ExpressionIsAlwaysNull
            var jsonObject = converter.Write(source);
            jsonObject.Type.Should().Be(JsonDataType.Null);
        }
    }
}