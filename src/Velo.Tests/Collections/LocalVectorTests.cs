using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Collections
{
    public class LocalVectorTests : TestBase
    {
        public LocalVectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Add<T>(T item)
        {
            var vector = new LocalVector<T>();
            vector.Add(item);

            Assert.Contains(item, vector);
        }

        [Theory, AutoData]
        public void Add_Many(int count)
        {
            count = Math.Abs(count);
            if (count > 10000) count = 10000;

            var vector = new LocalVector<int>();
            var items = Enumerable.Range(0, count).ToArray();

            foreach (var item in items)
            {
                vector.Add(item);
                Assert.Contains(item, vector);
            }

            Assert.Equal(items.Length, vector.Length);
        }

        [Theory, AutoData]
        public void Any(int[] items)
        {
            var vector = new LocalVector<int>();

            foreach (var item in items)
            {
                vector.Add(item);
                // ReSharper disable once HeapView.CanAvoidClosure
                Assert.True(vector.Any(e => e == item));
            }
        }

        [Theory, AutoData]
        public void Any_AvoidClosure(int[] items)
        {
            var vector = new LocalVector<int>();

            foreach (var item in items)
            {
                vector.Add(item);
                Assert.True(vector.Any((e, i) => e == i, item));
            }
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Contains<T>(T item)
        {
            var vector = new LocalVector<T>();
            vector.Add(item);

            Assert.True(vector.Contains(item));
        }

        [Fact]
        public void Contains_False()
        {
            var vector = new LocalVector<int>();
            Assert.False(vector.Contains(5));
        }

        [Theory, AutoData]
        public void Clear(int[] items)
        {
            var vector = new LocalVector<int>();
            foreach (var item in items)
                vector.Add(item);

            vector.Clear();
            
            Assert.Empty(vector);
            Assert.Equal(0, vector.Length);
        }

        [Theory, AutoData]
        public void First(int[] items)
        {
            var vector = new LocalVector<int>();

            foreach (var item in items)
            {
                vector.Add(item);
                // ReSharper disable once HeapView.CanAvoidClosure
                Assert.Equal(item, vector.First(e => e == item));
            }
        }

        [Theory, AutoData]
        public void First_AvoidClosure(int[] items)
        {
            var vector = new LocalVector<int>();

            foreach (var item in items)
            {
                vector.Add(item);
                Assert.Equal(item, vector.First((e, i) => e == i, item));
            }
        }

        [Theory, AutoData]
        public void GetEnumerator(int[] items)
        {
            var vector = new LocalVector<int>();
            foreach (var item in items)
                vector.Add(item);

            foreach (var element in vector)
            {
                Assert.Contains(element, items);
            }
        }

        [Fact]
        public void GetEnumerator_Empty()
        {
            var vector = (IEnumerable<int>) new LocalVector<int>();
            using (var enumerator = vector.GetEnumerator())
                Assert.IsType<EmptyEnumerator<int>>(enumerator);
        }

        [Theory, AutoData]
        public void GetEnumerator_Interface(int[] items)
        {
            var vector = new LocalVector<int>();
            foreach (var item in items)
                vector.Add(item);

            var exists = new List<int>(vector);

            Assert.Equal(items.Length, items.Intersect(exists).Count());
        }

        [Theory, AutoData]
        public void Indexer(int[] items)
        {
            var vector = new LocalVector<int>();
            foreach (var item in items)
                vector.Add(item);

            for (var i = 0; i < vector.Length; i++)
            {
                Assert.Equal(items[i], vector[i]);
            }
        }


        [Theory, AutoData]
        public void Length(int[] items)
        {
            var vector = new LocalVector<int>();

            Assert.Equal(0, vector.Length);

            for (var i = 0; i < items.Length; i++)
            {
                vector.Add(items[i]);
                Assert.Equal(i + 1, vector.Length);
            }

            Assert.Equal(items.Length, vector.Length);
        }

        [Fact]
        public void Throw_First_NotFound()
        {
            var sequence = new LocalVector<int>();
            Assert.Throws<KeyNotFoundException>(() => sequence.First(e => e > 5));
        }

        [Fact]
        public void Throw_FirstAvoidClosure_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var sequence = new LocalVector<int>();
                sequence.First((e, value) => e == value, 5);
            });
        }

        [Fact]
        public void Throw_IndexOutOfRange()
        {
            var vector = new LocalVector<int>();
            Assert.Throws<IndexOutOfRangeException>(() => vector[10]);
        }
    }
}