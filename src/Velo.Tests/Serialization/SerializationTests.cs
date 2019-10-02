using System;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class SerializationTests : IDisposable
    {
        private readonly JConverter _converter;
        private readonly ITestOutputHelper _output;
        private readonly Stopwatch _stopwatch;

        public SerializationTests(ITestOutputHelper output)
        {
            _converter = new JConverter();
            _output = output;
            _stopwatch = Stopwatch.StartNew();
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


        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}