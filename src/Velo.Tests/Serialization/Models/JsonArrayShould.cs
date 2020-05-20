using System.IO;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.Serialization.Models;
using Xunit;

namespace Velo.Tests.Serialization.Models
{
    public sealed class JsonArrayShould : TestClass
    {
        private readonly int[] _array;
        private readonly IJsonConverter<int> _elementConverter;
        private readonly JsonArray _jsonArray;

        public JsonArrayShould()
        {
            _array = System.Linq.Enumerable.Range(0, 10).ToArray();
            
            _elementConverter = BuildConvertersCollection().Get<int>();

            _jsonArray = new JsonArray(_array.Select(_elementConverter.Write));
        }

        [Fact]
        public void Enumerable()
        {
            var counter = 0;
            foreach (var element in _jsonArray)
            {
                var expected = JsonValue.Number(_array[counter]);
                element.Should().Be(expected);
                counter++;
            }

            counter.Should().Be(_array.Length);
        }

        [Fact]
        public void Empty()
        {
            JsonArray.Empty.Length.Should().Be(0);
        }
        
        [Fact]
        public void Contain()
        {
            foreach (var number in _array)
            {
                var value = JsonValue.Number(number);
                _jsonArray.Contains(value).Should().BeTrue();
            }
        }

        [Theory]
        [AutoData]
        public void CreatedByArray(int[] source)
        {
            var jsonArray = new JsonArray(source.Select(_elementConverter.Write).ToArray());

            for (var i = 0; i < source.Length; i++)
            {
                var expected = JsonValue.Number(source[i]);
                jsonArray[i].Should().Be(expected);
            }
        }

        [Theory]
        [AutoData]
        public void CreatedByEnumerable(int[] source)
        {
            var jsonArray = new JsonArray(source.Select(_elementConverter.Write));

            for (var i = 0; i < source.Length; i++)
            {
                var expected = JsonValue.Number(source[i]);
                jsonArray[i].Should().Be(expected);
            }
        }

        [Fact]
        public void HasValidLength()
        {
            _jsonArray.Length.Should().Be(_array.Length);
        }

        [Fact]
        public void HasValidType()
        {
            _jsonArray.Type.Should().Be(JsonDataType.Array);
            JsonArray.Empty.Type.Should().Be(JsonDataType.Array);
        }

        [Fact]
        public void HasIndexerAccess()
        {
            for (var i = 0; i < _array.Length; i++)
            {
                var value = JsonValue.Number(_array[i]);
                _jsonArray[i].Should().Be(value);
            }
        }

        [Fact]
        public void NotContain()
        {
            var notContainValue = JsonValue.Number(int.MinValue);
            _jsonArray.Contains(notContainValue).Should().BeFalse();
        }

        [Fact]
        public void Serialize()
        {
            var stringWriter = new StringWriter();
            _jsonArray.Serialize(stringWriter);

            var result = stringWriter.ToString();

            result.Should().Be(JsonConvert.SerializeObject(_array));
        }

        [Fact]
        public void UsedViaLinq()
        {
            _jsonArray.Should().Contain(JsonValue.Number(_array[0]));
            _jsonArray.Should().NotContain(JsonValue.Number(int.MinValue));
        }
    }
}