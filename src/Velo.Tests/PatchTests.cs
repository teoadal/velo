using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Velo.Patching;
using Velo.Patching.CollectionActions;
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
        public void AddValue(List<int> value, int add)
        {
            var boo = new Boo {Values = value};

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, add)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(add, boo.Values);
        }

        [Theory, AutoData]
        public void AddValue_NotInitialized(int add)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .AddValue(b => b.Values, add)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.Contains(add, boo.Values);
        }

        [Theory, AutoData]
        public void AddValues(List<int> values, int[] add)
        {
            var boo = new Boo {Values = values};

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
        public void Assign_Builder(Boo boo)
        {
            _builder.CreatePatch<Boo>()
                .Assign(b => b.Bool, b => b.Int > b.Id)
                .Apply(boo);

            Assert.Equal(boo.Int > boo.Id, boo.Bool);
        }

        [Theory, AutoData]
        public void Clear(List<int> values)
        {
            var boo = new Boo {Values = values};

            _builder.CreatePatch<Boo>()
                .ClearValues(b => b.Values)
                .Apply(boo);

            Assert.Empty(boo.Values);
        }

        [Fact]
        public void Clear_Notinitialized()
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .ClearValues(b => b.Values)
                .Apply(boo);

            Assert.Null(boo.Values);
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
        public void Drop(Boo boo)
        {
            _builder.CreatePatch<Boo>()
                .Drop(b => b.Bool)
                .Drop(b => b.Double)
                .Drop(b => b.Float)
                .Drop(b => b.Id)
                .Drop(b => b.Int)
                .Drop(b => b.Values)
                .Apply(boo);

            Assert.Equal(default, boo.Bool);
            Assert.Equal(default, boo.Double);
            Assert.Equal(default, boo.Float);
            Assert.Equal(default, boo.Id);
            Assert.Equal(default, boo.Int);
            Assert.Equal(default, boo.Values);
        }

        [Theory, AutoData]
        public void Execute(Boo boo, int remove)
        {
            boo.Values.Add(remove);

            _builder.CreatePatch<Boo>()
                .Execute(new RemoveValuePatch<Boo, int>(b => b.Values, remove))
                .Apply(boo);

            Assert.DoesNotContain(remove, boo.Values);
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
        public void RemoveValue(List<int> values, int remove)
        {
            values.Add(remove);
            var boo = new Boo {Values = values};

            _builder.CreatePatch<Boo>()
                .RemoveValue(b => b.Values, remove)
                .Apply(boo);

            Assert.NotNull(boo.Values);
            Assert.DoesNotContain(remove, boo.Values);
        }

        [Theory, AutoData]
        public void RemoveValue_NotInitialized(int remove)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .RemoveValue(b => b.Values, remove)
                .Apply(boo);

            Assert.Null(boo.Values);
        }

        [Theory, AutoData]
        public void RemoveValues(List<int> values, int[] remove)
        {
            values.AddRange(remove);

            var boo = new Boo {Values = values};

            _builder.CreatePatch<Boo>()
                .RemoveValues(b => b.Values, remove)
                .Apply(boo);

            Assert.NotNull(boo.Values);

            foreach (var item in remove)
            {
                Assert.DoesNotContain(item, boo.Values);
            }
        }

        [Theory, AutoData]
        public void RemoveValues_NotInitialized(int[] remove)
        {
            var boo = new Boo();

            _builder.CreatePatch<Boo>()
                .RemoveValues(b => b.Values, remove)
                .Apply(boo);

            Assert.Null(boo.Values);
        }

        [Theory, AutoData]
        public void ReplaceValue(List<int> values, int oldValue, int newValue)
        {
            values.Add(oldValue);

            var boo = new Boo {Values = values};

            _builder.CreatePatch<Boo>()
                .ReplaceValue(b => b.Values, oldValue, newValue)
                .Apply(boo);

            Assert.DoesNotContain(oldValue, boo.Values);
            Assert.Contains(newValue, boo.Values);
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}