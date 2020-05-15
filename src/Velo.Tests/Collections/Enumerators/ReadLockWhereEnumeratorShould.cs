using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Enumerators;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Enumerators
{
    public class ReadLockWhereEnumeratorShould : TestClass
    {
        private readonly ReaderWriterLockSlim _lock;

        private readonly Dictionary<int, int> _dictionary;
        private readonly List<int> _list;
        private readonly Func<int, int, bool> _stub;

        public ReadLockWhereEnumeratorShould(ITestOutputHelper output) : base(output)
        {
            _lock = new ReaderWriterLockSlim();

            _dictionary = Fixture.Create<Dictionary<int, int>>();
            _list = Fixture.Create<List<int>>();

            _stub = (a, b) => true;
        }

        [Fact]
        public void EnterDictionaryLock()
        {
            var _ = new ReadLockWhereEnumerator<int, int, int>(_dictionary.Values, _stub, 0, _lock);
            _lock.IsReadLockHeld.Should().BeTrue();
        }

        [Fact]
        public void EnterListLock()
        {
            var _ = new ReadLockWhereEnumerator<int, int>(_list, _stub, 0, _lock);
            _lock.IsReadLockHeld.Should().BeTrue();
        }

        [Fact]
        public void EnumerateDictionary()
        {
            var exists = new HashSet<int>();

            var enumerator = new ReadLockWhereEnumerator<int, int, int>(_dictionary.Values, _stub, 0, _lock);
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

            var enumerator = new ReadLockWhereEnumerator<int, int>(_list, _stub, 0, _lock);
            while (enumerator.MoveNext())
            {
                exists.Add(enumerator.Current).Should().BeTrue();
            }

            exists.Count.Should().Be(_list.Count);
            enumerator.Dispose();
        }

        [Fact]
        public void FilterDictionaryValues()
        {
            var exists = new HashSet<int>();

            var greatestValue = _dictionary.Values.Max();
            var enumerator = new ReadLockWhereEnumerator<int, int, int>(_dictionary.Values,
                (v, arg) => v >= arg, greatestValue, _lock);

            while (enumerator.MoveNext())
            {
                exists.Add(enumerator.Current).Should().BeTrue();
            }

            exists.Count.Should().Be(1);
            exists.Should().ContainSingle(v => v == greatestValue);
            enumerator.Dispose();
        }

        [Fact]
        public void FilterListValues()
        {
            var exists = new HashSet<int>();

            var greatestValue = _list.Max();
            var enumerator = new ReadLockWhereEnumerator<int, int>(_list, (v, arg) => v >= arg, greatestValue, _lock);
            while (enumerator.MoveNext())
            {
                exists.Add(enumerator.Current).Should().BeTrue();
            }

            exists.Count.Should().Be(1);
            exists.Should().ContainSingle(v => v == greatestValue);
            enumerator.Dispose();
        }

        [Fact]
        public void ExitDictionaryLock()
        {
            var enumerator = new ReadLockWhereEnumerator<int, int, int>(_dictionary.Values, _stub, 0, _lock);
            enumerator.Dispose();
            _lock.IsReadLockHeld.Should().BeFalse();
        }

        [Fact]
        public void ExitListLock()
        {
            var enumerator = new ReadLockWhereEnumerator<int, int>(_list, _stub, 0, _lock);
            enumerator.Dispose();
            _lock.IsReadLockHeld.Should().BeFalse();
        }
    }
}