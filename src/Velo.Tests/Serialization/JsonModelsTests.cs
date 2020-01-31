using System.Linq;
using AutoFixture.Xunit2;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class JsonModelsTests : TestClass
    {
        public JsonModelsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
        public void Array_Contains(int[] values)
        {
            var array = new JsonArray(values.Select(JsonValue.Number));
            for (var i = 0; i < values.Length; i++)
            {
                var value = JsonValue.Number(values[i]);
                Assert.True(array.Contains(value));
                Assert.Equal(value, array[i]);
            }
        }

        [Theory, AutoData]
        public void Object_Contains(string property, int value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Number(value));

            Assert.True(obj.Contains(property));
        }

        [Theory, AutoData]
        public void Object_Get(string property, bool value)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Boolean(value));

            Assert.Equal(JsonValue.Boolean(value), obj[property]);
        }

        [Theory, AutoData]
        public void Object_Remove(string property)
        {
            var obj = new JsonObject();
            obj.Add(property, JsonValue.Null);

            Assert.True(obj.Remove(property));
            Assert.False(obj.Remove(property));
            Assert.False(obj.Contains(property));
        }

        [Fact]
        public void Value_Equals()
        {
            Assert.Equal(JsonValue.False, JsonValue.Boolean(false));
            Assert.Equal(JsonValue.True, JsonValue.Boolean(true));

            Assert.Equal(JsonValue.Zero, JsonValue.Number(0));
            Assert.Equal(JsonValue.Number(25), JsonValue.Number(25f));

            Assert.Equal(JsonValue.StringEmpty, JsonValue.String(string.Empty));
            Assert.Equal(JsonValue.String("abc"), JsonValue.String("abc"));
        }

        [Theory, AutoData]
        public void Value_GetHashCode(string value)
        {
            Assert.Equal(value.GetHashCode(), JsonValue.String(value).GetHashCode());
        }

        [Fact]
        public void Types()
        {
            Assert.Equal(JsonDataType.Array, JsonArray.Empty.Type);
            Assert.Equal(JsonDataType.Object, new JsonObject().Type);

            Assert.Equal(JsonDataType.True, JsonValue.Boolean(true).Type);
            Assert.Equal(JsonDataType.False, JsonValue.Boolean(false).Type);

            Assert.Equal(JsonDataType.Null, JsonValue.Null.Type);

            Assert.Equal(JsonDataType.Number, JsonValue.Zero.Type);
            Assert.Equal(JsonDataType.Number, JsonValue.Number(25).Type);
            Assert.Equal(JsonDataType.Number, JsonValue.Number(25f).Type);

            Assert.Equal(JsonDataType.String, JsonValue.String("abc").Type);
            Assert.Equal(JsonDataType.String, JsonValue.StringEmpty.Type);
        }
    }
}