using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class SerializationTests : TestBase
    {
        private readonly JConverter _converter;

        public SerializationTests(ITestOutputHelper output) : base(output)
        {
            _converter = new JConverter();
        }

        [Theory, AutoData]
        public void Serialize_Array_Object(Boo[] array)
        {
            var json = _converter.Serialize(array);
            var deserialized = JsonConvert.DeserializeObject<Boo[]>(json);

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
        public void Serialize_Array_Float(float[] array)
        {
            var json = _converter.Serialize(array);
            var deserialized = JsonConvert.DeserializeObject<float[]>(json);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }
        
        [Theory, AutoData]
        public void Serialize_Array_MultiThreading(int[][] arrays)
        {
            var jsons = arrays.Select(_converter.Serialize).ToArray();

            var tasks = new Task[arrays.Length];
            for (var i = 0; i < jsons.Length; i++)
            {
                var array = arrays[i];
                var json = jsons[i];
                tasks[i] = Task.Run(() =>
                {
                    var deserialized = JsonConvert.DeserializeObject<int[]>(json);
                    for (var index = 0; index < array.Length; index++)
                    {
                        Assert.Equal(array[index], deserialized[index]);
                    }
                });
            }
        }
        
        [Fact]
        public void Serialize_Array_Null()
        {
            bool[] source = null;
            
            // ReSharper disable once ExpressionIsAlwaysNull
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<bool[]>(json);
            
            Assert.Null(deserialized);
        }
        
        [Theory, AutoData]
        public void Serialize_Array_String(string[] array)
        {
            var json = _converter.Serialize(array);
            var deserialized = JsonConvert.DeserializeObject<string[]>(json);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], deserialized[i]);
            }
        }
        
        [Theory, AutoData]
        public void Serialize_BigObject(BigObject source)
        {
            var serialized = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<BigObject>(serialized);

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
        public void Serialize_Boolean(bool source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<bool>(json);
            
            Assert.Equal(source, deserialized);
        }
        
        [Fact]
        public void Serialize_DateTime()
        {
            var source = DateTime.Now;

            var serialized = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<DateTime>(serialized);
            
            Assert.Equal(source, deserialized);
        }
        
        [Theory, AutoData]
        public void Serialize_Double(double source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<double>(json);
            
            Assert.Equal(source, deserialized);
        }
        
        [Theory, AutoData]
        public void Serialize_Enum(ModelType modelType)
        {
            var json = _converter.Serialize(modelType);
            var deserialized = JsonConvert.DeserializeObject<ModelType>(json);
            
            Assert.Equal(modelType, deserialized);
        }
        
        [Fact]
        public void Serialize_Guid()
        {
            var source = Guid.NewGuid();

            var serialized = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<Guid>(serialized);
            
            Assert.Equal(source, deserialized);
        }
        
        [Theory, AutoData]
        public void Serialize_Float(float source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<double>(json);
            
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Serialize_Int(int source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<int>(json);
            
            Assert.Equal(source, deserialized);
        }

        [Theory, AutoData]
        public void Serialize_List(List<bool?> source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<List<bool?>>(json);

            for (var i = 0; i < deserialized.Count; i++)
            {
                Assert.Equal(source[i], deserialized[i]);
            }
        }
        
        [Fact]
        public void Serialize_List_Null()
        {
            List<ModelType> source = null;
            
            // ReSharper disable once ExpressionIsAlwaysNull
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<List<ModelType>>(json);
            
            Assert.Null(deserialized);
        }
        
        [Fact]
        public void Serialize_Null_Property()
        {
            var source = new BigObject
            {
                Array = null,
                Boo = null,
                Foo = null,
                String = null
            };

            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<BigObject>(json);

            Assert.Null(deserialized.Array);
            Assert.Null(deserialized.Boo);
            Assert.Null(deserialized.Foo);
            Assert.Null(deserialized.String);
        }

        [Fact]
        public void Serialize_Null()
        {
            BigObject source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<BigObject>(json);
            
            Assert.Null(deserialized);
        }

        [Fact]
        public void Serialize_Nullable_NotNull()
        {
            ModelType? source = ModelType.Boo;

            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<ModelType?>(json);
            
            Assert.Equal(source, deserialized);
        }

        [Fact]
        public void Serialize_Nullable_Null()
        {
            int? source = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<int?>(json);
            
            Assert.Equal(source, deserialized);
        }
        
        [Theory, AutoData]
        public void Serialize_String(string source)
        {
            var json = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<string>(json);
            
            Assert.Equal(source, deserialized);
        }
    }
}