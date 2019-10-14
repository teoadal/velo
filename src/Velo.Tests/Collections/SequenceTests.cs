using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Collections
{
    public class SequenceTests : TestBase
    {
        public SequenceTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Add<T>(T item)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var sequence = new Sequence<T>();
            sequence.Add(item);

            Assert.Contains(item, sequence);
        }

        [Theory, AutoData]
        public void Add_Many(int count)
        {
            count = Math.Abs(count);
            if (count > 10000) count = 10000;

            var sequence = new Sequence<int>();
            var items = Enumerable.Range(0, count).ToArray();

            foreach (var item in items)
            {
                sequence.Add(item);
                Assert.Contains(item, sequence);
            }


            Assert.Equal(items.Length, sequence.Length);
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Any<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            // ReSharper disable once HeapView.CanAvoidClosure
            Assert.True(sequence.Any(e => e.Equals(item)));
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Any_AvoidClosure<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            // ReSharper disable once HeapView.CanAvoidClosure
            Assert.True(sequence.Any((e, i) => e.Equals(i), item));
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Clear<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            sequence.Clear();

            Assert.Empty(sequence);
            Assert.Equal(0, sequence.Length);
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Contains<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            Assert.True(sequence.Contains(item));
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Contains_False<T>(T item)
        {
            var sequence = new Sequence<T> {item};

            sequence.Remove(item);

            Assert.False(sequence.Contains(item));
        }

        [Theory, AutoData]
        public void Count(Boo[] items)
        {
            var sequence = new Sequence<Boo>(items);
            Assert.Equal(items.Count(i => i.Id > 0), sequence.Count(b => b.Id > 0));
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void First<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            // ReSharper disable once HeapView.CanAvoidClosure
            Assert.Equal(item, sequence.First(e => e.Equals(item)));
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void First_AvoidClosure<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            // ReSharper disable once HeapView.CanAvoidClosure
            Assert.Equal(item, sequence.First((e, i) => e.Equals(i), item));
        }

        [Theory, AutoData]
        public void GetEnumerator(int[] items)
        {
            var sequence = new Sequence<int>(items);

            foreach (var element in sequence)
            {
                Assert.Contains(element, items);
            }
        }

        [Fact]
        public void GetEnumerator_Empty()
        {
            var sequence = (IEnumerable<int>) new Sequence<int>();
            using (var enumerator = sequence.GetEnumerator())
                Assert.IsType<EmptyEnumerator<int>>(enumerator);
        }

        [Theory, AutoData]
        public void GetEnumerator_Interface(int[] items)
        {
            var sequence = new Sequence<int>(items);
            var exists = new List<int>(sequence);

            Assert.Equal(items.Length, items.Intersect(exists).Count());
        }

        [Theory, AutoData]
        public void GetUnderlyingArray(int[] items)
        {
            var sequence = new Sequence<int>(items);

            var array = sequence.GetUnderlyingArray(out var length);
            for (var i = 0; i < array.Length; i++)
            {
                if (i == length) break;
                Assert.Equal(items[i], array[i]);
            }
        }

        [Theory, AutoData]
        public void Indexer(int[] items)
        {
            var sequence = new Sequence<int>(items);

            for (var i = 0; i < sequence.Length; i++)
            {
                Assert.Equal(items[i], sequence[i]);
            }
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Initializer<T>(T item)
        {
            var sequence = new Sequence<T> {item};

            Assert.Contains(item, sequence);
        }

        [Theory, AutoData]
        public void Length(int[] items)
        {
            var sequence = new Sequence<int>();

            Assert.Equal(0, sequence.Length);

            for (var i = 0; i < items.Length; i++)
            {
                sequence.Add(items[i]);
                Assert.Equal(i + 1, sequence.Length);
            }

            Assert.Equal(items.Length, sequence.Length);

            for (var i = items.Length - 1; i >= 0; i--)
            {
                sequence.Remove(items[i]);
                Assert.Equal(i, sequence.Length);
            }
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Remove<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            Assert.True(sequence.Remove(item));

            Assert.DoesNotContain(item, sequence);
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Remove_False<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            sequence.Remove(item);

            Assert.False(sequence.Remove(item));
        }

        [Theory, AutoData]
        public void Remove_Many(string[] items)
        {
            var sequence = new Sequence<string>(items);
            foreach (var item in items)
            {
                Assert.True(sequence.Remove(item));
                Assert.DoesNotContain(item, sequence);
            }
        }

        [Theory, AutoData]
        public void Remove_Predicate(string[] items)
        {
            var sequence = new Sequence<string>(items);
            foreach (var item in items)
            {
                // ReSharper disable once HeapView.CanAvoidClosure
                Assert.True(sequence.Remove(element => element == item));
                Assert.DoesNotContain(item, sequence);
            }
        }

        [Theory, AutoData]
        public void Remove_PredicateAvoidClosure(string[] items)
        {
            var sequence = new Sequence<string>(items);
            foreach (var item in items)
            {
                Assert.True(sequence.Remove((element, i) => element == i, item));
                Assert.DoesNotContain(item, sequence);
            }
        }

        [Theory, AutoData]
        public void Select(Boo[] items)
        {
            var sequence = new Sequence<Boo>(items);
            foreach (var id in sequence.Select(b => b.Id))
            {
                Assert.Contains(items, i => i.Id == id);
            }
        }

        [Theory, AutoData]
        public void Select_ToArray(Boo[] items)
        {
            var sequence = new Sequence<Boo>(items);
            foreach (var id in sequence.Select(b => b.Id).ToArray())
            {
                Assert.Contains(items, i => i.Id == id);
            }
        }

        [Theory, AutoData]
        public void Sort(string[] items)
        {
            var sequence = new Sequence<string>(items);

            var comparer = Comparer<string>.Default;
            sequence.Sort(comparer);
            Array.Sort(items, comparer);

            for (var i = 0; i < items.Length; i++)
            {
                Assert.Equal(items[i], sequence[i]);
            }
        }

        [Theory, AutoData]
        public void ToArray(int[] items)
        {
            var sequence = new Sequence<int>(items).ToArray();

            Assert.Equal(items.Length, sequence.Length);

            for (var i = 0; i < items.Length; i++)
            {
                Assert.Equal(items[i], sequence[i]);
            }
        }

        [Fact]
        public void ToArray_Empty()
        {
            var sequence = new Sequence<int>(100);
            Assert.Empty(sequence);
        }

        [Theory, AutoData]
        public void Resize(int count)
        {
            count = Math.Abs(count);
            if (count > 10000) count = 10000;

            var items = Enumerable.Range(0, count).ToArray();

            var sequence = new Sequence<int>(0, SequenceResizeRule.Increase);
            foreach (var item in items)
            {
                sequence.Add(item);
            }

            Assert.Equal(items.Length, sequence.Length);
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Resize_FromZero<T>(T item)
        {
            var sequence = new Sequence<T>(0) {item};
            Assert.True(sequence.Contains(item));
            Assert.Equal(1, sequence.Length);
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void WeakClear<T>(T item)
        {
            var sequence = new Sequence<T> {item};
            sequence.WeakClear();

            Assert.Empty(sequence);
        }

        [Theory, AutoData]
        public void Where(Boo[] items)
        {
            var sequence = new Sequence<Boo>(items);
            Assert.Equal(items.Where(i => i.Id > 0), sequence.Where(b => b.Id > 0));
        }

        [Theory, AutoData]
        public void Where_ToArray(Boo[] items)
        {
            var sequence = new Sequence<Boo>(items);
            Assert.Equal(
                items.Where(i => i.Id > 0).ToArray(),
                sequence.Where(b => b.Id > 0).ToArray());
        }

        [Theory, AutoData]
        public void Where_ToArrayAvoidClosure(Boo[] items, int value)
        {
            var sequence = new Sequence<Boo>(items);
            Assert.Equal(
                items.Where(i => i.Id > value).ToArray(),
                sequence.Where((b, v) => b.Id > v, value).ToArray());
        }

        [Fact]
        public void Throw_First_NotFound()
        {
            var sequence = new Sequence<int>();
            Assert.Throws<KeyNotFoundException>(() => sequence.First(e => e == 5));
        }

        [Fact]
        public void Throw_FirstAvoidClosure_NotFound()
        {
            var sequence = new Sequence<int>();
            Assert.Throws<KeyNotFoundException>(() => sequence.First((e, value) => e == value, 5));
        }

        [Fact]
        public void Throw_IndexOutOfRange()
        {
            var sequence = new Sequence<int>();
            Assert.Throws<IndexOutOfRangeException>(() => sequence[10]);
        }
    }
}