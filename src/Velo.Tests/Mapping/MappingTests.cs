using AutoFixture.Xunit2;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Mapping
{
    public class MappingTests : TestBase
    {
        private readonly CompiledMapper<Foo> _mapper;

        public MappingTests(ITestOutputHelper output) : base(output)
        {
            _mapper = new CompiledMapper<Foo>();
        }

        [Theory, AutoData]
        public void CompiledMapper_Foo_To_Boo(bool boolValue, float floatValue, int intValue)
        {
            var source = new Boo
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            Foo foo;
            using (StartStopwatch())
            {
                foo = _mapper.Map(source);
            }

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Theory, AutoData]
        public void CompiledMapper_Anonymous_To_Foo(bool boolValue, float floatValue, int intValue)
        {
            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            Foo foo;
            using (StartStopwatch())
            {
                foo = _mapper.Map(source);
            }

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }
    }
}