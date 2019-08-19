using AutoFixture.Xunit2;

using Newtonsoft.Json;

using Velo.Serialization;

using Xunit;

namespace Velo
{
    public class SerializationTests
    {
        private readonly JSerializer _serializer;

        public SerializationTests()
        {
            _serializer = new JSerializer();
        }

        [Theory, AutoData]
        public void Serialize_BigObject(BigObject source)
        {
            var serialized = _serializer.Serialize(source);

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
    }
}