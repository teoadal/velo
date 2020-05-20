using System;
using System.Collections;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;

namespace Velo.Tests.Collections.Enumerators
{
    public class ArrayEnumeratorShould : TestClass
    {
        private readonly int[] _array;
        private ArrayEnumerator<int> _enumerator;

        public ArrayEnumeratorShould()
        {
            _array = Fixture.CreateMany<int>().ToArray();
            _enumerator = new ArrayEnumerator<int>(_array);
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
        public void EnumerateAsEnumerator()
        {
            var counter = 0;
            var enumerator = ((IEnumerable) _enumerator).GetEnumerator();
            while (enumerator.MoveNext())
            {
                _array[counter++].Should().Be((int) enumerator.Current!);
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
        public void ThrowDisposed()
        {
            _enumerator.Dispose();

            _enumerator
                .Invoking(enumerator => enumerator.MoveNext())
                .Should().Throw<ObjectDisposedException>();
        }
    }
}