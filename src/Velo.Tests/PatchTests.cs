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

        [Theory, AutoData]
        public void AddValue(List<int> list, int item)
        {
            var boo = new Boo {Values = list};

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, item)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(item, boo.Values);
        }

        [Theory, AutoData]
        public void AddValue_NotInitialized(int item)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, item)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(item, boo.Values);
        }

        [Theory, AutoData]
        public void AddValues(List<int> list, int[] add)
        {
            var boo = new Boo {Values = list};

            _builder.CreatePatch<Boo>()
                .AddValues(b => b.Values, add)
                .Apply(boo);

            Assert.NotNull(boo.Values);

            foreach (var number in add)
            {
                Assert.Contains(number, boo.Values);
            }
        }

        [Theory, AutoData]
        public void AddValues_NotInitialized(int[] add)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .AddValues(b => b.Values, add)
                .Apply(boo);

            Assert.NotNull(boo.Values);

            foreach (var number in add)
            {
                Assert.Contains(number, boo.Values);
            }
        }

        [Theory, AutoData]
        public void Assign(Boo first, Boo second)
        {
            _builder.CreatePatch<Boo>()
                .Assign(b => b.Bool, second.Bool)
                .Assign(b => b.Double, second.Double)
                .Assign(b => b.Float, second.Float)
                .Assign(b => b.Id, second.Id)
                .Assign(b => b.Int, second.Int)
                .Apply(first);

            Assert.Equal(second.Bool, first.Bool);
            Assert.Equal(second.Double, first.Double);
            Assert.Equal(second.Float, first.Float);
            Assert.Equal(second.Id, first.Id);
            Assert.Equal(second.Int, first.Int);
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
        public void RemoveValue(List<int> list, int item)
        {
            list.Add(item);
            var boo = new Boo {Values = list};

            _builder.CreatePatch<Boo>()
                .RemoveValue(b => b.Values, item)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.DoesNotContain(item, boo.Values);
        }

        [Theory, AutoData]
        public void RemoveValue_NotInitialized(int item)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .RemoveValue(b => b.Values, item)
                .Apply(boo);

            Assert.Null(boo.Values);
        }

        [Theory, AutoData]
        public void RemoveValues(List<int> list, int[] items)
        {
            list.AddRange(items);

            var boo = new Boo {Values = list};

            _builder.CreatePatch<Boo>()
                .RemoveValues(b => b.Values, items)
                .Apply(boo);

            Assert.NotNull(boo.Values);

            foreach (var item in items)
            {
                Assert.DoesNotContain(item, boo.Values);
            }
        }

        [Theory, AutoData]
        public void RemoveValues_NotInitialized(int[] items)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .RemoveValues(b => b.Values, items)
                .Apply(boo);

            Assert.Null(boo.Values);
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}