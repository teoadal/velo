using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization
{
    public class DeserializationTests : TestClass
    {
        private readonly JConverter _converter;

        public DeserializationTests(ITestOutputHelper output) : base(output)
        {
            _converter = new JConverter();
        }

        [Theory, AutoData]
        public void Deserialize_Array_Objects(Boo[] array)
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
        public void Deserialize_Array_Floats(float[] array)
        {
            var json = JsonConvert.SerializeObject(array);
            var deserialized = _converter.Deserialize<float[]>(json);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }

        [Theory, AutoData]
        public void Deserialize_Array_Ints(int[] array)
        {
            var json = JsonConvert.SerializeObject(array);
            var deserialized = _converter.Deserialize<int[]>(json);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }

        [Fact]
        public void Deserialize_Array_Null()
        {
            bool[] source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<bool[]>(json);

            Assert.Null(deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_Array_MultiThreading(int[][] arrays)
        {
            var jsons = arrays.Select(JsonConvert.SerializeObject).ToArray();

            var tasks = new Task[arrays.Length];
            for (var i = 0; i < jsons.Length; i++)
            {
                var array = arrays[i];
                var json = jsons[i];
                tasks[i] = Task.Run(() =>
                {
                    var deserialized = _converter.Deserialize<int[]>(json);
                    for (var index = 0; index < array.Length; index++)
                    {
                        Assert.Equal(array[index], deserialized[index]);
                    }
                });
            }
        }

        [Theory, AutoData]
        public void Deserialize_Array_Strings(string[] array)
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

        [Fact]
        public void Deserialize_DateTime()
        {
            var source = DateTime.Now;

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<DateTime>(json);

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
        public void Deserialize_Enum(ModelType modelType)
        {
            var json = JsonConvert.SerializeObject(modelType);
            var deserialized = _converter.Deserialize<ModelType>(json);

            Assert.Equal(modelType, deserialized);
        }

        [Fact]
        public void Deserialize_Guid()
        {
            var source = Guid.NewGuid();

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<Guid>(json);

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
        public void Deserialize_File(BigObject[] source)
        {
            var json = JsonConvert.SerializeObject(source, Formatting.Indented);

            const string fileName = nameof(Deserialize_File) + ".json";

            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllText(fileName, json, Encoding.UTF8);

            var fileStream = File.OpenRead(fileName);
            BigObject[] deserialized;
            try
            {
                deserialized = _converter.Deserialize<BigObject[]>(fileStream);
            }
            finally
            {
                fileStream.Dispose();
            }

            for (var i = 0; i < source.Length; i++)
            {
                var expected = source[i];
                var actual = deserialized[i];

                Assert.Equal(expected.Double, actual.Double);
                Assert.Equal(expected.Float, actual.Float);
                Assert.Equal(expected.Int, actual.Int);
                Assert.Equal(expected.String, actual.String);
                Assert.Equal(expected.Foo.Bool, actual.Foo.Bool);
                Assert.Equal(expected.Foo.Type, actual.Foo.Type);
            }
        }

        [Theory, AutoData]
        public void Deserialize_Int(int source)
        {
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<int>(json);

            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_List(List<bool?> source)
        {
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<List<bool?>>(json);

            for (var i = 0; i < deserialized.Count; i++)
            {
                Assert.Equal(source[i], deserialized[i]);
            }
        }

        [Fact]
        public void Deserialize_List_Null()
        {
            List<ModelType> source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<List<ModelType>>(json);

            Assert.Null(deserialized);
        }

        [Fact]
        public void Deserialize_Object()
        {
            var source = new Foo {Int = 123, Type = ModelType.Foo};

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<Foo>(json);

            Assert.Equal(source.Int, deserialized.Int);
            Assert.Equal(source.Type, deserialized.Type);
        }

        [Fact]
        public void Deserialize_Object_Array()
        {
            var source = new Foo {Array = new[] {1, 2, 3}};

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<Foo>(json);

            Assert.Equal(source.Int, deserialized.Int);
            Assert.Equal(source.Type, deserialized.Type);
            for (var i = 0; i < source.Array.Length; i++)
            {
                Assert.Equal(source.Array[i], deserialized.Array[i]);
            }
        }

        [Fact]
        public void Deserialize_Object_Empty()
        {
            var source = new Foo();

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<Foo>(json);

            Assert.Equal(source.Bool, deserialized.Bool);
            Assert.Equal(source.Int, deserialized.Int);
            Assert.Equal(source.Float, deserialized.Float);
            Assert.Equal(source.Type, deserialized.Type);
        }

        [Fact]
        public void Deserialize_Object_NullProperty()
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

            Assert.Null(deserialized.Array);
            Assert.Null(deserialized.Boo);
            Assert.Null(deserialized.Foo);
            Assert.Null(deserialized.String);
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

        [Fact]
        public void Deserialize_Nullable_NotNull()
        {
            ModelType? source = ModelType.Foo;

            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<ModelType?>(json);

            Assert.Equal(source, deserialized);
        }

        [Fact]
        public void Deserialize_Nullable_Null()
        {
            int? source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<int?>(json);

            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Deserialize_String(string source)
        {
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<string>(json);

            Assert.Equal(source, deserialized);
        }
        
        [Theory, AutoData]
        public void Deserialize_TimeSpan(TimeSpan source)
        {
            var json = JsonConvert.SerializeObject(source);
            var deserialized = _converter.Deserialize<TimeSpan>(json);

            Assert.Equal(source, deserialized);
        }
    }
}