using System;
using System.Collections.Generic;
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

        [Fact]
        public void AddValue()
        {
            const int item = 4;
            var boo = new Boo {Values = new List<int> {1, 2, 3}};

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, item)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(item, boo.Values);
        }

        [Fact]
        public void AddValue_NotInitialized()
        {
            const int item = 4;
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, item)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(item, boo.Values);
        }

        [Theory, AutoData]
        public void Decrement(Boo boo)
        {
            var initValue = boo.Int;

            _builder.CreatePatch<Boo>()
                .Decrement(b => b.Int)
                .Apply(boo);

            Assert.Equal(initValue - 1, boo.Int);
        }

        [Theory, AutoData]
        public void Increment(Boo boo)
        {
            var initValue = boo.Int;

            _builder.CreatePatch<Boo>()
                .Increment(b => b.Int)
                .Apply(boo);

            Assert.Equal(initValue + 1, boo.Int);
        }

        [Theory, AutoData]
        public void SetValue(Boo first, Boo second)
        {
            _builder.CreatePatch<Boo>()
                .SetValue(b => b.Bool, second.Bool)
                .SetValue(b => b.Double, second.Double)
                .SetValue(b => b.Float, second.Float)
                .SetValue(b => b.Id, second.Id)
                .SetValue(b => b.Int, second.Int)
                .Apply(first);

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