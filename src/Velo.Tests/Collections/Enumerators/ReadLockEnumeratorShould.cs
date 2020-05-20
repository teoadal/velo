using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;

namespace Velo.Tests.Collections.Enumerators
{
    public class ReadLockEnumeratorShould : TestClass
    {
        private readonly ReaderWriterLockSlim _lock;

        private readonly List<int> _list;
        private readonly Dictionary<int, int> _dictionary;

        public ReadLockEnumeratorShould()
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

            exists.Count.Should().Be(_dictionary.Count);
            enumerator.Dispose();
        }

        [Fact]
        public void EnumerateDictionaryAsEnumerator()
        {
            var exists = new HashSet<int>();

            var enumerator = (IEnumerator) new ReadLockEnumerator<int, int>(_dictionary.Values, _lock);
            while (enumerator.MoveNext())
            {
                exists.Add((int) enumerator.Current!).Should().BeTrue();
            }

            exists.Count.Should().Be(_dictionary.Count);
        }
        
        [Fact]
        public void EnumerateList()
        {
            var enumerator = new ReadLockEnumerator<int>(_list, _lock);
            var counter = 0;
            while (enumerator.MoveNext())
            {
                _list[counter++].Should().Be(enumerator.Current);
            }

            counter.Should().Be(_list.Count);
            enumerator.Dispose();
        }

        [Fact]
        public void EnumerateListAsEnumerator()
        {
            var counter = 0;
            var enumerator = (IEnumerator) new ReadLockEnumerator<int>(_list, _lock);
            while (enumerator.MoveNext())
            {
                _list[counter++].Should().Be((int) enumerator.Current!);
            }

            counter.Should().Be(_list.Count);
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