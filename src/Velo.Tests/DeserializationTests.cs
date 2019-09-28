using AutoFixture.Xunit2;

using Newtonsoft.Json;

using Velo.Serialization;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo
{
    public class DeserializationTests
    {
        private readonly JConverter _converter;

        public DeserializationTests()
        {
            _converter = new JConverter();
        }

        [Theory, AutoData]
        public void Deserialize_Array_Object(Boo[] array)
        {
            var json = JsonConvert.SerializeObject(array);

            var deserialized = _converter.Deserialize<Boo[]>(json);
            for (var i = 0; i < array.Length; i++)
            {
                var element = array[i];
                var deserializedElement = deserialized[i];

                Assert.Equal(element.Bool, deserializedElement.Bool);
                Assert.Equal(element.Double, deserializedElement.Double);
                Assert.Equal(element.Float, deserializedElement.Float);
                Assert.Equal(element.Int, deserializedElement.Int);
            }
        }

        [Theory, AutoData]
        public void Deserialize_Array_Float(float[] array)
        {
            var json = JsonConvert.SerializeObject(array);

            var deserialized = _converter.Deserialize<float[]>(json);
            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }

        [Theory, AutoData]
        public void Deserialize_Array_Int(int[] array)
        {
            var json = JsonConvert.SerializeObject(array);

            var deserialized = _converter.Deserialize<int[]>(json);
            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }

        [Theory, AutoData]
        public void Deserialize_Array_String(string[] array)
        {
            var json = JsonConvert.SerializeObject(array);

            var deserialized = _converter.Deserialize<string[]>(json);
            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }

        [Theory, AutoData]
        public void Deserialize_BigObject(BigObject source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<BigObject>(json);

            if (source.Boo != null)
            {
                Assert.NotNull(deserialized.Boo);
                Assert.Equal(source.Boo.Bool, deserialized.Boo.Bool);
                Assert.Equal(source.Boo.Double, deserialized.Boo.Double);
                Assert.Equal(source.Boo.Float, deserialized.Boo.Float);
                Assert.Equal(source.Boo.Int, deserialized.Boo.Int);
            }

            if (source.Foo != null)
            {
                Assert.NotNull(deserialized.Foo);
                Assert.Equal(source.Foo.Bool, deserialized.Foo.Bool);
                Assert.Equal(source.Foo.Float, deserialized.Foo.Float);
                Assert.Equal(source.Foo.Int, deserialized.Foo.Int);
            }

            Assert.Equal(source.Array, deserialized.Array);
            Assert.Equal(source.Bool, deserialized.Bool);
            Assert.Equal(source.Float, deserialized.Float);
            Assert.Equal(source.Double, deserialized.Double);
            Assert.Equal(source.Int, deserialized.Int);
            Assert.Equal(source.String, deserialized.String);
        }

        [Theory, AutoData]
        public void Deserialize_Boolean(bool source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<bool>(json);
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_Double(double source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<double>(json);
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_Float(float source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<double>(json);
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_Int(int source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<int>(json);
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_String(string source)
        {
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<string>(json);
            Assert.Equal(source, deserialized);
        }

        [Fact]
        public void Deserialize_Null_Property()
        {
            var source = new BigObject
            {
                Array = null,
                Boo = null,
                Foo = null,
                String = null
            };

            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<BigObject>(json);

            Assert.Equal(source.Array, deserialized.Array);
            Assert.Equal(source.Boo, deserialized.Boo);
            Assert.Equal(source.Foo, deserialized.Foo);
            Assert.Equal(source.String, deserialized.String);
        }

        [Fact]
        public void Deserialize_Null()
        {
            BigObject source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = JsonConvert.SerializeObject(source);

            var deserialized = _converter.Deserialize<BigObject>(json);
            Assert.Null(deserialized);
        }
    }
}