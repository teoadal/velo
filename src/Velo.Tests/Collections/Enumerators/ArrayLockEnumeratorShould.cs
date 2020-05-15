using System;
using System.Linq;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Enumerators
{
    public class ArrayLockEnumeratorShould : TestClass
    {
        private readonly int[] _array;
        private ArrayLockEnumerator<int> _enumerator;
        private readonly ReaderWriterLockSlim _lock;

        public ArrayLockEnumeratorShould(ITestOutputHelper output) : base(output)
        {
            _array = Fixture.CreateMany<int>().ToArray();
            _lock = new ReaderWriterLockSlim();
            _enumerator = new ArrayLockEnumerator<int>(_array, _lock);
        }

        [Fact]
        public void EnterLock()
        {
            _lock.IsReadLockHeld.Should().BeTrue();
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
        public void ExitLock()
        {
            _enumerator.Dispose();
            _lock.IsReadLockHeld.Should().BeFalse();
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