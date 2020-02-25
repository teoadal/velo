using System;
using System.Globalization;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization.Models
{
    public sealed class JsonArrayShould : TestClass
    {
        private readonly IConvertersCollection _converters;

        public JsonArrayShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
        }

        [Theory, AutoData]
        public void Contains(int[] values)
        {
            var array = new JsonArray(values.Select(JsonValue.Number));
            for (var i = 0; i < values.Length; i++)
            {
                var value = JsonValue.Number(values[i]);
                array.Should().Contain(value);
                array.Contains(value).Should().BeTrue();
                array[i].Should().Be(value);
            }
        }

        [Fact]
        public void HasValidType()
        {
            JsonArray.Empty.Type.Should().Be(JsonDataType.Array);
        }

        [Fact]
        public void NotContains()
        {
            var value = new JsonValue(2.ToString(), JsonDataType.Number);
            var notContainValue = new JsonValue(3.ToString(), JsonDataType.Number);
            var array = new JsonArray(new JsonData[] {value});

            array.Contains(notContainValue).Should().BeFalse();
            array.Should().NotContain(notContainValue);
        }

        [Theory, AutoData]
        public void Read(int[] source)
        {
            var converter = _converters.Get<int[]>();

            var jsonArray = (JsonArray) converter.Write(source);
            var result = converter.Read(jsonArray);
            result.Should().BeEquivalentTo(source);
        }

        [Fact]
        public void ReadEmptyArray()
        {
            var source = Array.Empty<int>();
            var converter = _converters.Get<int[]>();

            var jsonArray = (JsonArray) converter.Write(source);
            var result = converter.Read(jsonArray);
            result.Should().BeEquivalentTo(source);
        }

        [Fact]
        public void ReadNull()
        {
            var converter = _converters.Get<int[]>();

            var jsonArray = JsonValue.Null;
            var result = converter.Read(jsonArray);
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void ReadObjects(BigObject[] source)
        {
            var converter = _converters.Get<BigObject[]>();

            var jsonArray = (JsonArray) converter.Write(source);
            var result = converter.Read(jsonArray);
            // ReSharper disable once CoVariantArrayConversion
            result.Should().BeEquivalentTo(source);
        }

        [Theory, AutoData]
        public void Write(int[] source)
        {
            var converter = _converters.Get<int[]>();

            var jsonArray = (JsonArray) converter.Write(source);
            jsonArray.Should().Contain(source.Select(JsonValue.Number));
        }

        [Fact]
        public void WriteNull()
        {
            int[] source = null;
            var converter = _converters.Get<int[]>();

            // ReSharper disable once ExpressionIsAlwaysNull
            var jsonArray = converter.Write(source);
            jsonArray.Type.Should().Be(JsonDataType.Null);
        }

        [Theory, AutoData]
        public void WriteObjects(BigObject[] source)
        {
            var converter = _converters.Get<BigObject>();

            var jsonArray = (JsonArray) _converters.Get<BigObject[]>().Write(source);
            jsonArray.Should().BeEquivalentTo(source.Select(s => converter.Write(s)));
        }
    }
}