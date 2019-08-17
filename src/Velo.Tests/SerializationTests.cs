using Newtonsoft.Json;

using Velo.Serialization;

using Xunit;

namespace Velo
{
    public class SerializationTests
    {
        [Fact]
        public void Deserialize()
        {
            var source = new BigObject
            {
                Bool = true,
                Double = 1.5d,
                Foo = null,
                Float = 2.5f,
                Int = 3,
                String = "test \"string\" escape",
                Array = new[] {4, 5, 6},
                Boo = new Boo {Bool = false, Double = 7d, Float = 8f, Int = 9}
            };

            var json = JsonConvert.SerializeObject(source);

            var serializer = new JSerializer();
            var deserialized = serializer.Deserialize<BigObject>(json);

            Assert.Equal(source.Array, deserialized.Array);
            Assert.Equal(source.Boo.Bool, deserialized.Boo.Bool);
            Assert.Equal(source.Boo.Double, deserialized.Boo.Double);
            Assert.Equal(source.Boo.Float, deserialized.Boo.Float);
            Assert.Equal(source.Boo.Int, deserialized.Boo.Int);
            Assert.Equal(source.Bool, deserialized.Bool);
            Assert.Equal(source.Foo, deserialized.Foo);
            Assert.Equal(source.Float, deserialized.Float);
            Assert.Equal(source.Double, deserialized.Double);
            Assert.Equal(source.Int, deserialized.Int);
            Assert.Equal(source.String, deserialized.String);
        }

        [Fact]
        public void Serialize()
        {
        }
    }
}