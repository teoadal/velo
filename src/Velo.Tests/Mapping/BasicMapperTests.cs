using AutoFixture.Xunit2;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Mapping
{
    public class BasicMapperTests : TestClass
    {
        private readonly BasicMapper<Foo> _mapper;

        public BasicMapperTests(ITestOutputHelper output) : base(output)
        {
            _mapper = new BasicMapper<Foo>();
        }

        [Theory, AutoData]
        public void BasicMapper_Foo_To_Boo(bool boolValue, float floatValue, int intValue, double doubleValue)
        {
            var source = new Boo
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue,
                Double = doubleValue
            };

            var foo = _mapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Theory, AutoData]
        public void BasicMapper_Anonymous_To_Foo(bool boolValue, float floatValue, int intValue)
        {
            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = _mapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }
    }
}