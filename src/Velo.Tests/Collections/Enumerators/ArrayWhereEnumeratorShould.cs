using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Enumerators
{
    public class ArrayWhereEnumeratorShould : TestClass
    {
        private readonly int[] _array;
        private ArrayWhereEnumerator<int, int> _enumerator;
        
        public ArrayWhereEnumeratorShould(ITestOutputHelper output) : base(output)
        {
            _array = Fixture.CreateMany<int>().ToArray();
            _enumerator = new ArrayWhereEnumerator<int, int>(_array, (i, _) => true, 0);
        }
        
        [Fact]
        public void Enumerate()
        {
            var counter = 0;
            while (_enumerator.MoveNext())
            {
                _array[counter++].Should().Be(_enumerator.Current);
            }

            counter.Should().Be(_array.Length);
        }

        [Fact]
        public void EnumerateAsEnumerable()
        {
            var counter = 0;
            foreach (var i in _enumerator)
            {
                _array[counter++].Should().Be(i);
            }

            counter.Should().Be(_array.Length);
        }

        [Fact]
        public void EnumerateAfterReset()
        {
            while (_enumerator.MoveNext())
            {
            }

            _enumerator.Reset();

            var counter = 0;
            while (_enumerator.MoveNext())
            {
                _array[counter++].Should().Be(_enumerator.Current);
            }

            counter.Should().Be(_array.Length);
        }

        [Fact]
        public void FilterValues()
        {
            var greatestValue = _array.Max();
            var enumerator = new ArrayWhereEnumerator<int, int>(_array, (i, arg) => i >= arg, greatestValue);
            enumerator.Should().ContainSingle(v => v == greatestValue);
        }
        
        [Fact]
        public void ThrowDisposed()
        {
            _enumerator.Dispose();

            _enumerator
                .Invoking(enumerator => enumerator.MoveNext())
                .Should().Throw<ObjectDisposedException>();
        }
    }
}