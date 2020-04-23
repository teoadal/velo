using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Velo.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class ReadLockEnumeratorShould : TestClass
    {
        private readonly ReaderWriterLockSlim _lock;

        private readonly List<int> _list;
        private readonly Dictionary<int, int> _dictionary;

        public ReadLockEnumeratorShould(ITestOutputHelper output) : base(output)
        {
            _lock = new ReaderWriterLockSlim();

            _dictionary = Fixture.Create<Dictionary<int, int>>();
            _list = Fixture.Create<List<int>>();
        }

        [Fact]
        public void EnterDictionaryLock()
        {
            var _ = new ReadLockEnumerator<int, int>(_dictionary.Values, _lock);
            _lock.IsReadLockHeld.Should().BeTrue();
        }

        [Fact]
        public void EnterListLock()
        {
            var _ = new ReadLockEnumerator<int>(_list, _lock);
            _lock.IsReadLockHeld.Should().BeTrue();
        }

        [Fact]
        public void EnumerateDictionary()
        {
            var exists = new HashSet<int>();

            var enumerator = new ReadLockEnumerator<int, int>(_dictionary.Values, _lock);
            while (enumerator.MoveNext())
            {
                exists.Add(enumerator.Current).Should().BeTrue();
            }

            exists.Count.Should().Be(_list.Count);
            enumerator.Dispose();
        }

        [Fact]
        public void EnumerateList()
        {
            var exists = new HashSet<int>();

            var enumerator = new ReadLockEnumerator<int>(_list, _lock);
            while (enumerator.MoveNext())
            {
                exists.Add(enumerator.Current).Should().BeTrue();
            }

            exists.Count.Should().Be(_list.Count);
            enumerator.Dispose();
        }

        [Fact]
        public void ExitDictionaryLock()
        {
            var enumerator = new ReadLockEnumerator<int, int>(_dictionary.Values, _lock);
            enumerator.Dispose();
            _lock.IsReadLockHeld.Should().BeFalse();
        }

        [Fact]
        public void ExitListLock()
        {
            var enumerator = new ReadLockEnumerator<int>(_list, _lock);
            enumerator.Dispose();
            _lock.IsReadLockHeld.Should().BeFalse();
        }
    }
}