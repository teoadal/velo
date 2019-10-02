using System;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Velo.Mapping;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo
{
    public class MappingTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly Stopwatch _stopwatch;

        public MappingTests(ITestOutputHelper output)
        {
            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }

        [Theory, AutoData]
        public void BasicMapper_Foo_To_Boo(bool boolValue, float floatValue, int intValue, double doubleValue)
        {
            var basicMapper = new BasicMapper<Foo>();

            var source = new Boo
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue,
                Double = doubleValue
            };

            var foo = basicMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Theory, AutoData]
        public void BasicMapper_Anonymous_To_Foo(bool boolValue, float floatValue, int intValue)
        {
            var basicMapper = new BasicMapper<Foo>();

            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = basicMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Theory, AutoData]
        public void CompiledMapper_Foo_To_Boo(bool boolValue, float floatValue, int intValue)
        {
            var compiledMapper = new CompiledMapper<Foo>();

            var source = new Boo
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = compiledMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        [Theory, AutoData]
        public void CompiledMapper_Anonymous_To_Foo(bool boolValue, float floatValue, int intValue)
        {
            var compiledMapper = new CompiledMapper<Foo>();

            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = compiledMapper.Map(source);

            Assert.Equal(source.Bool, foo.Bool);
            Assert.Equal(source.Float, foo.Float);
            Assert.Equal(source.Int, foo.Int);
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}