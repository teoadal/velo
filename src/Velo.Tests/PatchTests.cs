using System;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Velo.Patching;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo
{
    public class PatchTests : IDisposable
    {
        private readonly PatchBuilder _builder;
        private readonly ITestOutputHelper _output;
        private readonly Stopwatch _stopwatch;

        public PatchTests(ITestOutputHelper output)
        {
            _builder = new PatchBuilder();

            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }
        
        [Theory, AutoData]
        public void Decrement(Boo boo)
        {
            var initValue = boo.Int;

            var path = _builder.CreatePatch<Boo>()
                .Decrement(b => b.Int);

            path.Apply(boo);
            Assert.Equal(initValue - 1, boo.Int);
        }

        [Theory, AutoData]
        public void Increment(Boo boo)
        {
            var initValue = boo.Int;

            var path = _builder.CreatePatch<Boo>()
                .Increment(b => b.Int);

            path.Apply(boo);
            Assert.Equal(initValue - 1, boo.Int);
        }

        [Theory, AutoData]
        public void SetValue(Boo first, Boo second)
        {
            var patch = _builder.CreatePatch<Boo>()
                .SetValue(b => b.Bool, second.Bool)
                .SetValue(b => b.Double, second.Double)
                .SetValue(b => b.Float, second.Float)
                .SetValue(b => b.Id, second.Id)
                .SetValue(b => b.Int, second.Int);

            patch.Apply(first);

            Assert.Equal(second.Bool, first.Bool);
            Assert.Equal(second.Double, first.Double);
            Assert.Equal(second.Float, first.Float);
            Assert.Equal(second.Id, first.Id);
            Assert.Equal(second.Int, first.Int);
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}