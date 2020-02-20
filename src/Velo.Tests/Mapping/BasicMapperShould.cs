using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Mapping
{
    public class BasicMapperShould : TestClass
    {
        private readonly BasicMapper<Foo> _mapper;

        public BasicMapperShould(ITestOutputHelper output) : base(output)
        {
            _mapper = new BasicMapper<Foo>();
        }

        [Theory, AutoData]
        public void ConvertBooToFoo(Boo source)
        {
            var foo = _mapper.Map(source);

            foo.Bool.Should().Be(source.Bool);
            foo.Float.Should().Be(source.Float);
            foo.Int.Should().Be(source.Int);
        }

        [Theory, AutoData]
        public void ConvertAnonymousToFoo(bool boolValue, float floatValue, int intValue)
        {
            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = _mapper.Map(source);

            foo.Bool.Should().Be(source.Bool);
            foo.Float.Should().Be(source.Float);
            foo.Int.Should().Be(source.Int);
        }
    }
}