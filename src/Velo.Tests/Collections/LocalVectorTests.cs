using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Collections
{
    public class LocalVectorTests : TestBase
    {
        private static int OuterKeySelector(Boo b) => b.Id;
        private static int InnerKeySelector(Foo f) => f.Int;
        private static float ResultBuilder(Boo b, Foo f) => b.Float;

        public LocalVectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Add<T>(T item)
        {
            var vector = new LocalVector<T>();
            vector.Add(item);

            Assert.True(vector.Contains(item));
        }

        [Theory, AutoData]
        public void Add_Many(int count)
        {
            count = FixCount(count);

            var vector = new LocalVector<int>();
            var items = Enumerable.Range(0, count).ToArray();

            foreach (var item in items)
            {
                vector.Add(item);
                Assert.True(vector.Contains(item));
            }

            Assert.Equal(items.Length, vector.Length);
        }

        [Theory, AutoData]
        public void AddRange(Boo[] items)
        {
            var vector = new LocalVector<Boo>();
            vector.AddRange(new LocalVector<Boo>(items));

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

        [Fact]
        public void Any_False()
        {
            var vector = new LocalVector<int>();
            Assert.False(vector.Any(i => i > 2));
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

        [Theory]
        [InlineData(25), InlineData("test"), InlineData(typeof(TestBase))]
        public void Contains_WithComparer<T>(T item)
        {
            var vector = new LocalVector<T>();
            vector.Add(item);

            Assert.True(vector.Contains(item, EqualityComparer<T>.Default));
        }

        [Theory, AutoData]
        public void Clear(int[] items)
        {
            var vector = new LocalVector<int>();
            foreach (var item in items)
                vector.Add(item);

            vector.Clear();

            Assert.True(vector.Length == 0);
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

        [Theory, AutoData]
        public void GroupBy(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items.GroupBy(i => i.Id).GetEnumerator();

            foreach (var vectorGroup in new LocalVector<Boo>(items).GroupBy(i => i.Id))
            {
                Assert.True(groupEnumerator.MoveNext());
                var expectedGroup = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                Assert.Equal(expectedGroup.Key, vectorGroup.Key);

                var expectedGroupEnumerator = expectedGroup.GetEnumerator();
                foreach (var boo in vectorGroup)
                {
                    Assert.True(expectedGroupEnumerator.MoveNext());
                    var expectedBoo = expectedGroupEnumerator.Current;

                    Assert.Equal(expectedBoo, boo);
                }

                expectedGroupEnumerator.Dispose();
            }

            groupEnumerator.Dispose();
        }

        [Theory, AutoData]
        public void GroupBy_Select(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items
                .GroupBy(i => i.Id)
                .Select(gr => gr.Sum(b => b.Id))
                .GetEnumerator();

            foreach (var actual in new LocalVector<Boo>(items)
                .GroupBy(i => i.Id)
                .Select(gr => gr.Sum(b => b.Id)))
            {
                Assert.True(groupEnumerator.MoveNext());
                var expected = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                Assert.Equal(expected, actual);
            }

            groupEnumerator.Dispose();
        }

        [Theory, AutoData]
        public void GroupBy_Where(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items.GroupBy(i => i.Id).GetEnumerator();

            foreach (var vectorGroup in new LocalVector<Boo>(items).GroupBy(i => i.Id))
            {
                Assert.True(groupEnumerator.MoveNext());
                var expectedGroup = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                Assert.Equal(expectedGroup.Key, vectorGroup.Key);

                var expectedGroupEnumerator = expectedGroup.Where(b => b.Id > 1).GetEnumerator();
                foreach (var boo in vectorGroup.Where(b => b.Id > 1))
                {
                    Assert.True(expectedGroupEnumerator.MoveNext());
                    var expectedBoo = expectedGroupEnumerator.Current;

                    Assert.Equal(expectedBoo, boo);
                }

                expectedGroupEnumerator.Dispose();
            }

            groupEnumerator.Dispose();
        }

        [Theory, AutoData]
        public void IndexOf(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);
            for (var index = 0; index < items.Length; index++)
            {
                var boo = items[index];
                Assert.Equal(index, vector.IndexOf(boo));
            }
        }

        [Theory, AutoData]
        public void Indexer(int count)
        {
            count = FixCount(count);

            var items = Enumerable.Range(0, count).ToArray();
            var vector = new LocalVector<int>(items);

            for (var i = 0; i < vector.Length; i++)
            {
                Assert.Equal(items[i], vector[i]);
            }
        }

        [Theory, AutoData]
        public void Join(Boo[] items)
        {
            var outer = new LocalVector<Boo>(items);

            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            var inner = new LocalVector<Foo>(innerItems);

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray();

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Join_Many(int count)
        {
            count = FixCount(count);

            var outer = Enumerable.Range(0, count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            var inner = outer.Reverse().ToArray();

            var join = outer.Join(inner, o => o.Id, i => i.Id, (o, i) => i.Int).ToArray();

            var outerVector = new LocalVector<Boo>(outer);
            var innerVector = new LocalVector<Boo>(inner);
            var vector = outerVector.Join(innerVector, o => o.Id, i => i.Id, (o, i) => i.Int);

            var counter = 0;
            foreach (var element in vector)
            {
                Assert.Equal(join[counter++], element);
            }
        }

        [Fact]
        public void Join_WithManyInner()
        {
            var outer = new [] { new Boo {Id = 1, Int = 1} };
            var inner = new[] {new Foo {Int = 1}, new Foo {Int = 1}, new Foo {Int = 1}};
            
            var joinSum = outer
                .Join(inner, o => o.Int, i => i.Int, (o, i) => i.Int)
                .Sum();
            
            var outerVector = new LocalVector<Boo>(outer);
            var innerVector = new LocalVector<Foo>(inner);

            var vectorSum = outerVector
                .Join(innerVector, o => o.Int, i => i.Int, (o, i) => i.Int)
                .Sum(i => i);

            Assert.Equal(joinSum, vectorSum);
        }
        
        [Theory, AutoData]
        public void Join_OrderBy(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .OrderBy(f => f)
                .ToArray();

            var outer = new LocalVector<Boo>(items);
            var inner = new LocalVector<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .OrderBy(f => f))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Join_Select(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Select(f => f * 10)
                .ToArray();

            var outer = new LocalVector<Boo>(items);
            var inner = new LocalVector<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Select(f => f * 10))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Join_ToArray(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray();

            var outer = new LocalVector<Boo>(items);
            var inner = new LocalVector<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray())
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Join_Where(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items
                .Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where(f => f > 10f)
                .ToArray();

            var outer = new LocalVector<Boo>(items);
            var inner = new LocalVector<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer
                .Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where(f => f > 10f))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Join_Where_AvoidClosure(Boo[] items)
        {
            var outer = new LocalVector<Boo>(items);

            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            var inner = new LocalVector<Foo>(innerItems);

            var join = items
                .Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where(f => f > 10f)
                .ToArray();

            var counter = 0;
            foreach (var number in outer
                .Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where((f, n) => f > n, 10f))
            {
                Assert.Equal(join[counter++], number);
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
        public void Mix()
        {
            var items = new LocalVector<int>();

            for (var i = 0; i < 100; i++)
            {
                items.Add(i);
                Assert.True(items.Contains(i));

                if (i % 2 != 0) continue;

                Assert.True(items.Remove(i));
                Assert.False(items.Contains(i));
            }
        }

        [Theory, AutoData]
        public void Mix_Many(int count)
        {
            count = FixCount(count);

            var vector = new LocalVector<int>();

            for (var i = 0; i < count; i++)
            {
                vector.Add(i);
                Assert.True(vector.Contains(i));

                if (i % 2 != 0) continue;

                Assert.True(vector.Remove(i));
                Assert.False(vector.Contains(i));
            }
        }

        [Theory, AutoData]
        public void Remove(int[] items)
        {
            var vector = new LocalVector<int>(items);
            var item = items[items.Length / 2];

            Assert.True(vector.Remove(item));
            Assert.False(vector.Contains(item));
        }

        [Theory, AutoData]
        public void Remove_All(int[] items)
        {
            var vector = new LocalVector<int>(items);

            foreach (var item in items)
            {
                Assert.True(vector.Remove(item));
                Assert.False(vector.Contains(item));
            }
        }

        [Theory, AutoData]
        public void Remove_At(int[] items)
        {
            var vector = new LocalVector<int>(items);

            for (var i = 0; i < items.Length; i++)
            {
                var element = vector[0];
                vector.RemoveAt(0);
                Assert.False(vector.Contains(element));
            }
        }

        [Theory, AutoData]
        public void Remove_First(int[] items)
        {
            var vector = new LocalVector<int>(items);
            var item = items[0];

            Assert.True(vector.Remove(item));
            Assert.False(vector.Contains(item));
        }

        [Theory, AutoData]
        public void Remove_Last(int[] items)
        {
            var vector = new LocalVector<int>(items);
            var item = items[^1];

            Assert.True(vector.Remove(item));
            Assert.False(vector.Contains(item));
        }

        [Theory, AutoData]
        public void RemoveAt(int[] items)
        {
            var vector = new LocalVector<int>(items);

            foreach (var element in items)
            {
                var index = vector.IndexOf(element);

                vector.RemoveAt(index);

                Assert.False(vector.Contains(element));
            }
        }

        [Theory, AutoData]
        public void Select(Boo[] items)
        {
            var select = items.Select(i => i.Id).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector.Select(i => i.Id))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_AvoidClosure(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector.Select((i, t) => i.Id > t ? 5 : 10, threshold))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_AvoidClosure_ToArray(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .ToArray())
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_AvoidClosure_OrderBy(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).OrderBy(id => id).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .OrderBy(id => id))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_AvoidClosure_Where(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).Where(id => id == 5).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .Where(id => id == 5))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_OrderBy(Boo[] items)
        {
            var select = items.Select(i => i.Id).OrderBy(n => n).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector.Select(i => i.Id).OrderBy(n => n))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_Where(Boo[] items)
        {
            var select = items.Select(i => i.Id).Where(id => id > 1).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            
            var counter = 0;
            foreach (var number in vector.Select(i => i.Id).Where(id => id > 1))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_Where_AvoidClosure(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var select = items.Select(i => i.Id).Where(id => id > 1).ToArray();

            var counter = 0;
            foreach (var number in vector.Select(i => i.Id).Where((id, n) => id > n, 1))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory, AutoData]
        public void Select_ToArray(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var select = items.Select(i => i.Id).ToArray();
            var vectorSelect = vector.Select(i => i.Id).ToArray();

            for (var i = 0; i < select.Length; i++)
            {
                Assert.Equal(select[i], vectorSelect[i]);
            }
        }

        [Theory, AutoData]
        public void Sort(Boo[] items)
        {
            var sorted = items.OrderBy(b => b.Id).ToArray();
            
            var vector = new LocalVector<Boo>(items);

            using (Measure())
                vector.Sort(b => b.Id);
            
            for (var i = 0; i < sorted.Length; i++)
            {
                Assert.Equal(sorted[i], vector[i]);
            }
        }

        [Theory, AutoData]
        public void Sum(int count)
        {
            count = FixCount(count);

            var outer = Enumerable.Range(0, count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            var inner = outer.Reverse().ToArray();

            // ReSharper disable ConvertToConstant.Local
            var threshold = 2;
            var modifier = 7;
            // ReSharper restore ConvertToConstant.Local

            var sum = outer
                .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
                .GroupBy(boo => boo.Id)
                .Select(gr => gr.First())
                .Where(b => b.Int > threshold)
                .Select(b => b.Id * modifier)
                .OrderBy(id => id)
                .Sum();

            var outerVector = new LocalVector<Boo>(outer);
            var innerVector = new LocalVector<Boo>(inner);

            int vectorSum;
            using (Measure())
            {
                vectorSum = outerVector
                    .Join(innerVector, o => o.Id, i => i.Id, (o, i) => i)
                    .GroupBy(boo => boo.Id)
                    .Select(gr => gr.First())
                    .Where((b, t) => b.Int > t, threshold)
                    .Select((b, m) => b.Id * m, modifier)
                    .OrderBy(id => id)
                    .Sum();
            }

            Assert.Equal(sum, vectorSum);
        }

        [Theory, AutoData]
        public void ToArray(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);
            var vectorArray = vector.ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                Assert.Equal(items[i], vectorArray[i]);
            }
        }

        [Fact]
        public void ToArray_Empty()
        {
            var vector = new LocalVector<Boo>();
            var vectorArray = vector.ToArray();

            Assert.Empty(vectorArray);
        }

        [Fact]
        public void ToArray_Granularity()
        {
            var vector = new LocalVector<int>();

            for (var i = 0; i < 100; i++)
            {
                vector.Add(i);
                var vectorArray = vector.ToArray();
                Assert.Equal(vectorArray.Length, vector.Length);
            }
        }

        [Theory, AutoData]
        public void Where(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).ToArray();

            var counter = 0;
            foreach (var boo in vector.Where(i => i.Id % 2 == 0))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory, AutoData]
        public void Where_AvoidClosure(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).ToArray();

            var counter = 0;
            foreach (var boo in vector.Where((i, n) => i.Id % n == 0, number))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory, AutoData]
        public void Where_AvoidClosure_Select(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).Select(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in vector.Where((i, n) => i.Id % n == 0, number).Select(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }
        
        [Theory, AutoData]
        public void Where_AvoidClosure_Join(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var number = 2;
            
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            var innerVector = new LocalVector<Foo>(innerItems);
            
            var where = items
                .Where(i => i.Id % number == 0)
                .Join(innerItems, b => b.Id, f => f.Int, (b, f) => b)
                .ToArray();

            var counter = 0;
            foreach (var boo in vector
                .Where((i, n) => i.Id % n == 0, number)
                .Join(innerVector, b => b.Id, f => f.Int, (b, f) => b))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory, AutoData]
        public void Where_AvoidClosure_OrderBy(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var number = 2;
            
            var vector = new LocalVector<Boo>(items);

            var where = items.Where(i => i.Id % number == 0).OrderBy(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in vector
                .Where((i, n) => i.Id % 2 == 0, number)
                .OrderBy(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }
        
        [Theory, AutoData]
        public void Where_Join(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            
            var vector = new LocalVector<Boo>(items);
            var innerVector = new LocalVector<Foo>(innerItems);
            
            var where = items
                .Where(i => i.Id % 2 == 0)
                .Join(innerItems, b => b.Id, f => f.Int, (b, f) => b).ToArray();

            var counter = 0;
            foreach (var boo in vector
                .Where(i => i.Id % 2 == 0)
                .Join(innerVector, b => b.Id, f => f.Int, (b, f) => b))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory, AutoData]
        public void Where_OrderBy(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).OrderBy(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in vector.Where(i => i.Id % 2 == 0).OrderBy(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }
        
        [Theory, AutoData]
        public void Where_Select(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).Select(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in vector.Where(i => i.Id % 2 == 0).Select(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory, AutoData]
        public void Where_ToArray(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).ToArray();
            var vectorWhere = vector.Where(i => i.Id % 2 == 0).ToArray();

            for (var i = 0; i < where.Length; i++)
            {
                Assert.Equal(where[i], vectorWhere[i]);
            }
        }

        [Theory, AutoData]
        public void Where_ToArray_AvoidClosure(Boo[] items)
        {
            var vector = new LocalVector<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).ToArray();
            var vectorWhere = vector.Where((i, n) => i.Id % n == 0, number).ToArray();

            for (var i = 0; i < where.Length; i++)
            {
                Assert.Equal(where[i], vectorWhere[i]);
            }
        }

        [Fact]
        public void Throw_First_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var vector = new LocalVector<int>();
                vector.First(e => e > 5);
            });
        }

        [Fact]
        public void Throw_FirstAvoidClosure_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var vector = new LocalVector<int>();
                vector.First((e, value) => e == value, 5);
            });
        }

        [Fact]
        public void Throw_IndexOutOfRange()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var vector = new LocalVector<int>();
                return vector[10];
            });
        }
    }
}