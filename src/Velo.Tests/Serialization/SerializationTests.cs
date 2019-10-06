using System;
using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Velo.TestsModels;
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
        public void Serialize_BigObject(BigObject source)
        {
            var serialized = _converter.Serialize(source);

            BigObject deserialized;
            using (StartStopwatch())
            {
                deserialized = JsonConvert.DeserializeObject<BigObject>(serialized);
            }

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
        
        [Fact]
        public void Serialize_DateTime()
        {
            var source = DateTime.Now;

            var serialized = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<DateTime>(serialized);
            
            Assert.Equal(source, deserialized);
        }
        
        [Fact]
        public void Serialize_Guid()
        {
            var source = Guid.NewGuid();

            var serialized = _converter.Serialize(source);
            var deserialized = JsonConvert.DeserializeObject<Guid>(serialized);
            
            Assert.Equal(source, deserialized);
        }
    }
}