using Velo.Mapping;

using Xunit;

namespace Velo
{
    public class MappingTests
    {
        [Fact]
        public void BasicMapper_Foo_To_Boo()
        {
            var basicMapper = new BasicMapper<Foo>();

            var source = new Boo
            {
                Bool = true,
                Float = 1f,
                Int = 11,
                Double = 123d
            };

            var foo = basicMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Fact]
        public void BasicMapper_Anonymous_To_Foo()
        {
            var basicMapper = new BasicMapper<Foo>();

            var source = new
            {
                Bool = true,
                Float = 2f,
                Int = 22
            };

            var foo = basicMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Fact]
        public void CompiledMapper_Foo_To_Boo()
        {
            var compiledMapper = new CompiledMapper<Foo>();

            var source = new Boo
            {
                Bool = true,
                Float = 1f,
                Int = 11
            };

            var foo = compiledMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Fact]
        public void CompiledMapper_Anonymous_To_Foo()
        {
            var compiledMapper = new CompiledMapper<Foo>();

            var source = new
            {
                Bool = true,
                Float = 2f,
                Int = 22
            };

            var foo = compiledMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }
    }
}