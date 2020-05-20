using System.Collections;
using System.Linq;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;

namespace Velo.Tests.Collections.Enumerators
{
    public class EmptyEnumeratorShould : TestClass
    {
        private readonly EmptyEnumerator<int> _enumerator;

        public EmptyEnumeratorShould()
        {
            _enumerator = EmptyEnumerator<int>.Instance;
        }

        [Fact]
        public void Empty()
        {
            Assert.Equal(default, _enumerator.Current);
            Assert.False(_enumerator.MoveNext());
        }

        [Fact]
        public void EnumerateAsEnumerator()
        {
            var enumerator = ((IEnumerable) _enumerator).GetEnumerator();

            Assert.Equal(default, enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void EnumerateAsEnumerable()
        {
            _enumerator.Count().Should().Be(0);
        }
    }
}