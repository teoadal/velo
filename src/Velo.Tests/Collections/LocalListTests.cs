using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Velo.Collections.Local;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class LocalVectorTests : TestClass
    {
        private static int OuterKeySelector(Boo b) => b.Id;
        private static int InnerKeySelector(Foo f) => f.Int;
        private static float ResultBuilder(Boo b, Foo f) => b.Float;

        public LocalVectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(25)]
        [InlineData("test")]
        [InlineData(typeof(TestClass))]
        public void Add<T>(T item)
        {
            var localList = new LocalList<T>();
            localList.Add(item);

            Assert.True(localList.Contains(item));
        }

        [Theory]
        [AutoData]
        public void Add_Many(int count)
        {
            count = FixCount(count);

            var localList = new LocalList<int>();
            var items = Enumerable.Range(0, count).ToArray();

            foreach (var item in items)
            {
                localList.Add(item);
                Assert.True(localList.Contains(item));
            }

            Assert.Equal(items.Length, localList.Length);
        }

        [Theory]
        [AutoData]
        public void AddRange(Boo[] items)
        {
            var localList = new LocalList<Boo>();
            localList.AddRange(new LocalList<Boo>(items));

            Assert.Equal(items.Length, localList.Length);
        }

        [Theory]
        [AutoData]
        public void Any(int[] items)
        {
            var localList = new LocalList<int>();

            foreach (var item in items)
            {
                localList.Add(item);
                // ReSharper disable once HeapView.CanAvoidClosure
                Assert.True(localList.Any(e => e == item));
            }
        }

        [Fact]
        public void Any_False()
        {
            var localList = new LocalList<int>();
            Assert.False(localList.Any(i => i > 2));
        }

        [Theory]
        [AutoData]
        public void Any_AvoidClosure(int[] items)
        {
            var localList = new LocalList<int>();

            foreach (var item in items)
            {
                localList.Add(item);
                Assert.True(localList.Any((e, i) => e == i, item));
            }
        }

        [Theory]
        [InlineData(25)]
        [InlineData("test")]
        [InlineData(typeof(TestClass))]
        public void Contains<T>(T item)
        {
            var localList = new LocalList<T>();
            localList.Add(item);

            Assert.True(localList.Contains(item));
        }

        [Fact]
        public void Contains_False()
        {
            var localList = new LocalList<int>();
            Assert.False(localList.Contains(5));
        }

        [Theory]
        [InlineData(25)]
        [InlineData("test")]
        [InlineData(typeof(TestClass))]
        public void Contains_WithComparer<T>(T item)
        {
            var localList = new LocalList<T>();
            localList.Add(item);

            Assert.True(localList.Contains(item, EqualityComparer<T>.Default));
        }

        [Theory]
        [AutoData]
        public void Clear(int[] items)
        {
            var localList = new LocalList<int>();
            foreach (var item in items)
                localList.Add(item);

            localList.Clear();

            Assert.True(localList.Length == 0);
            Assert.Equal(0, localList.Length);
        }

        [Theory]
        [AutoData]
        public void First(int[] items)
        {
            var localList = new LocalList<int>();

            foreach (var item in items)
            {
                localList.Add(item);
                // ReSharper disable once HeapView.CanAvoidClosure
                Assert.Equal(item, localList.First(e => e == item));
            }
        }

        [Theory]
        [AutoData]
        public void First_AvoidClosure(int[] items)
        {
            var localList = new LocalList<int>();

            foreach (var item in items)
            {
                localList.Add(item);
                Assert.Equal(item, localList.First((e, i) => e == i, item));
            }
        }

        [Theory]
        [AutoData]
        public void GetEnumerator(int[] items)
        {
            var localList = new LocalList<int>();
            foreach (var item in items)
                localList.Add(item);

            foreach (var element in localList)
            {
                Assert.Contains(element, items);
            }
        }

        [Theory]
        [AutoData]
        public void GroupBy(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items.GroupBy(i => i.Id).GetEnumerator();

            foreach (var localListGroup in new LocalList<Boo>(items).GroupBy(i => i.Id))
            {
                Assert.True(groupEnumerator.MoveNext());
                var expectedGroup = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                Assert.Equal(expectedGroup.Key, localListGroup.Key);

                var expectedGroupEnumerator = expectedGroup.GetEnumerator();
                foreach (var boo in localListGroup)
                {
                    Assert.True(expectedGroupEnumerator.MoveNext());
                    var expectedBoo = expectedGroupEnumerator.Current;

                    Assert.Equal(expectedBoo, boo);
                }

                expectedGroupEnumerator.Dispose();
            }

            groupEnumerator.Dispose();
        }

        [Theory]
        [AutoData]
        public void GroupBy_Select(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items
                .GroupBy(i => i.Id)
                .Select(gr => gr.Sum(b => b.Id))
                .GetEnumerator();

            foreach (var actual in new LocalList<Boo>(items)
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

        [Theory]
        [AutoData]
        public void GroupBy_Where(List<Boo> items)
        {
            items.AddRange(items.Select(i => new Boo {Id = i.Id}).ToArray());

            var groupEnumerator = items.GroupBy(i => i.Id).GetEnumerator();

            foreach (var localListGroup in new LocalList<Boo>(items).GroupBy(i => i.Id))
            {
                Assert.True(groupEnumerator.MoveNext());
                var expectedGroup = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                Assert.Equal(expectedGroup.Key, localListGroup.Key);

                var expectedGroupEnumerator = expectedGroup.Where(b => b.Id > 1).GetEnumerator();
                foreach (var boo in localListGroup.Where(b => b.Id > 1))
                {
                    Assert.True(expectedGroupEnumerator.MoveNext());
                    var expectedBoo = expectedGroupEnumerator.Current;

                    Assert.Equal(expectedBoo, boo);
                }

                expectedGroupEnumerator.Dispose();
            }

            groupEnumerator.Dispose();
        }

        [Theory]
        [AutoData]
        public void IndexOf(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);
            for (var index = 0; index < items.Length; index++)
            {
                var boo = items[index];
                Assert.Equal(index, localList.IndexOf(boo));
            }
        }

        [Theory]
        [AutoData]
        public void Indexer(int count)
        {
            count = FixCount(count);

            var items = Enumerable.Range(0, count).ToArray();
            var localList = new LocalList<int>(items);

            for (var i = 0; i < localList.Length; i++)
            {
                Assert.Equal(items[i], localList[i]);
            }
        }

        [Theory]
        [AutoData]
        public void Join(Boo[] items)
        {
            var outer = new LocalList<Boo>(items);

            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            var inner = new LocalList<Foo>(innerItems);

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray();

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Join_Many(int count)
        {
            count = FixCount(count);

            var outer = Enumerable.Range(0, count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            var inner = outer.Reverse().ToArray();

            var join = outer.Join(inner, o => o.Id, i => i.Id, (o, i) => i.Int).ToArray();

            var outerLocalList = new LocalList<Boo>(outer);
            var innerLocalList = new LocalList<Boo>(inner);
            var localList = outerLocalList.Join(innerLocalList, o => o.Id, i => i.Id, (o, i) => i.Int);

            var counter = 0;
            foreach (var element in localList)
            {
                Assert.Equal(join[counter++], element);
            }
        }

        [Fact]
        public void Join_WithManyInner()
        {
            var outer = new[] {new Boo {Id = 1, Int = 1}};
            var inner = new[] {new Foo {Int = 1}, new Foo {Int = 1}, new Foo {Int = 1}};

            var joinSum = outer
                .Join(inner, o => o.Int, i => i.Int, (o, i) => i.Int)
                .Sum();

            var outerLocalList = new LocalList<Boo>(outer);
            var innerLocalList = new LocalList<Foo>(inner);

            var localListSum = outerLocalList
                .Join(innerLocalList, o => o.Int, i => i.Int, (o, i) => i.Int)
                .Sum(i => i);

            Assert.Equal(joinSum, localListSum);
        }

        [Theory]
        [AutoData]
        public void Join_OrderBy(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .OrderBy(f => f)
                .ToArray();

            var outer = new LocalList<Boo>(items);
            var inner = new LocalList<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .OrderBy(f => f))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Join_Select(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Select(f => f * 10)
                .ToArray();

            var outer = new LocalList<Boo>(items);
            var inner = new LocalList<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Select(f => f * 10))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Join_ToArray(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray();

            var outer = new LocalList<Boo>(items);
            var inner = new LocalList<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray())
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Join_Where(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var join = items
                .Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where(f => f > 10f)
                .ToArray();

            var outer = new LocalList<Boo>(items);
            var inner = new LocalList<Foo>(innerItems);

            var counter = 0;
            foreach (var number in outer
                .Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder)
                .Where(f => f > 10f))
            {
                Assert.Equal(join[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Join_Where_AvoidClosure(Boo[] items)
        {
            var outer = new LocalList<Boo>(items);

            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();
            var inner = new LocalList<Foo>(innerItems);

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

        [Theory]
        [AutoData]
        public void Length(int[] items)
        {
            var localList = new LocalList<int>();

            Assert.Equal(0, localList.Length);

            for (var i = 0; i < items.Length; i++)
            {
                localList.Add(items[i]);
                Assert.Equal(i + 1, localList.Length);
            }

            Assert.Equal(items.Length, localList.Length);
        }

        [Fact]
        public void Mix()
        {
            var items = new LocalList<int>();

            for (var i = 0; i < 100; i++)
            {
                items.Add(i);
                Assert.True(items.Contains(i));

                if (i % 2 != 0) continue;

                Assert.True(items.Remove(i));
                Assert.False(items.Contains(i));
            }
        }

        [Theory]
        [AutoData]
        public void Mix_Many(int count)
        {
            count = FixCount(count);

            var localList = new LocalList<int>();

            for (var i = 0; i < count; i++)
            {
                localList.Add(i);
                Assert.True(localList.Contains(i));

                if (i % 2 != 0) continue;

                Assert.True(localList.Remove(i));
                Assert.False(localList.Contains(i));
            }
        }

        [Theory]
        [AutoData]
        public void Remove(int[] items)
        {
            var localList = new LocalList<int>(items);
            var item = items[items.Length / 2];

            Assert.True(localList.Remove(item));
            Assert.False(localList.Contains(item));
        }

        [Theory]
        [AutoData]
        public void Remove_All(int[] items)
        {
            var localList = new LocalList<int>(items);

            foreach (var item in items)
            {
                Assert.True(localList.Remove(item));
                Assert.False(localList.Contains(item));
            }
        }

        [Theory]
        [AutoData]
        public void Remove_At(int[] items)
        {
            var localList = new LocalList<int>(items);

            for (var i = 0; i < items.Length; i++)
            {
                var element = localList[0];
                localList.RemoveAt(0);
                Assert.False(localList.Contains(element));
            }
        }

        [Theory]
        [AutoData]
        public void Remove_First(int[] items)
        {
            var localList = new LocalList<int>(items);
            var item = items[0];

            Assert.True(localList.Remove(item));
            Assert.False(localList.Contains(item));
        }

        [Theory]
        [AutoData]
        public void Remove_Last(int[] items)
        {
            var localList = new LocalList<int>(items);
            var item = items[^1];

            Assert.True(localList.Remove(item));
            Assert.False(localList.Contains(item));
        }

        [Theory]
        [AutoData]
        public void RemoveAt(int[] items)
        {
            var localList = new LocalList<int>(items);

            foreach (var element in items)
            {
                var index = localList.IndexOf(element);

                localList.RemoveAt(index);

                Assert.False(localList.Contains(element));
            }
        }

        [Theory]
        [AutoData]
        public void Select(Boo[] items)
        {
            var select = items.Select(i => i.Id).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList.Select(i => i.Id))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_AvoidClosure(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList.Select((i, t) => i.Id > t ? 5 : 10, threshold))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_AvoidClosure_ToArray(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .ToArray())
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_AvoidClosure_OrderBy(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).OrderBy(id => id).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .OrderBy(id => id))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_AvoidClosure_Where(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = items.Select(i => i.Id > threshold ? 5 : 10).Where(id => id == 5).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList
                .Select((i, t) => i.Id > t ? 5 : 10, threshold)
                .Where(id => id == 5))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_OrderBy(Boo[] items)
        {
            var select = items.Select(i => i.Id).OrderBy(n => n).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList.Select(i => i.Id).OrderBy(n => n))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_Where(Boo[] items)
        {
            var select = items.Select(i => i.Id).Where(id => id > 1).ToArray();

            var localList = new LocalList<Boo>(items);

            var counter = 0;
            foreach (var number in localList.Select(i => i.Id).Where(id => id > 1))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_Where_AvoidClosure(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var select = items.Select(i => i.Id).Where(id => id > 1).ToArray();

            var counter = 0;
            foreach (var number in localList.Select(i => i.Id).Where((id, n) => id > n, 1))
            {
                Assert.Equal(select[counter++], number);
            }
        }

        [Theory]
        [AutoData]
        public void Select_ToArray(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var select = items.Select(i => i.Id).ToArray();
            var localListSelect = localList.Select(i => i.Id).ToArray();

            for (var i = 0; i < select.Length; i++)
            {
                Assert.Equal(select[i], localListSelect[i]);
            }
        }

        [Theory]
        [AutoData]
        public void Sort(Boo[] items)
        {
            var sorted = items.OrderBy(b => b.Id).ToArray();

            var localList = new LocalList<Boo>(items);

            using (Measure())
                localList.Sort(b => b.Id);

            for (var i = 0; i < sorted.Length; i++)
            {
                Assert.Equal(sorted[i], localList[i]);
            }
        }

        [Theory]
        [AutoData]
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

            var outerLocalList = new LocalList<Boo>(outer);
            var innerLocalList = new LocalList<Boo>(inner);

            int vectorSum;
            using (Measure())
            {
                vectorSum = outerLocalList
                    .Join(innerLocalList, o => o.Id, i => i.Id, (o, i) => i)
                    .GroupBy(boo => boo.Id)
                    .Select(gr => gr.First())
                    .Where((b, t) => b.Int > t, threshold)
                    .Select((b, m) => b.Id * m, modifier)
                    .OrderBy(id => id)
                    .Sum();
            }

            Assert.Equal(sum, vectorSum);
        }

        [Theory]
        [AutoData]
        public void ToArray(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);
            var localListArray = localList.ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                Assert.Equal(items[i], localListArray[i]);
            }
        }

        [Fact]
        public void ToArray_Empty()
        {
            var localList = new LocalList<Boo>();
            var localListArray = localList.ToArray();

            Assert.Empty(localListArray);
        }

        [Fact]
        public void ToArray_Granularity()
        {
            var localList = new LocalList<int>();

            for (var i = 0; i < 100; i++)
            {
                localList.Add(i);
                var localListArray = localList.ToArray();
                Assert.Equal(localListArray.Length, localList.Length);
            }
        }

        [Theory]
        [AutoData]
        public void Where(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where(i => i.Id % 2 == 0))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_AvoidClosure(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where((i, n) => i.Id % n == 0, number))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_AvoidClosure_Select(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).Select(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where((i, n) => i.Id % n == 0, number).Select(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_AvoidClosure_Join(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var localList = new LocalList<Boo>(items);
            var innerLocalList = new LocalList<Foo>(innerItems);

            var where = items
                .Where(i => i.Id % number == 0)
                .Join(innerItems, b => b.Id, f => f.Int, (b, f) => b)
                .ToArray();

            var counter = 0;
            foreach (var boo in localList
                .Where((i, n) => i.Id % n == 0, number)
                .Join(innerLocalList, b => b.Id, f => f.Int, (b, f) => b))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_AvoidClosure_OrderBy(Boo[] items)
        {
            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var localList = new LocalList<Boo>(items);

            var where = items.Where(i => i.Id % number == 0).OrderBy(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in localList
                .Where((i, n) => i.Id % 2 == 0, number)
                .OrderBy(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_Join(Boo[] items)
        {
            var innerItems = items.Select(i => new Foo {Int = i.Id}).ToArray();

            var localList = new LocalList<Boo>(items);
            var innerLocalList = new LocalList<Foo>(innerItems);

            var where = items
                .Where(i => i.Id % 2 == 0)
                .Join(innerItems, b => b.Id, f => f.Int, (b, f) => b).ToArray();

            var counter = 0;
            foreach (var boo in localList
                .Where(i => i.Id % 2 == 0)
                .Join(innerLocalList, b => b.Id, f => f.Int, (b, f) => b))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_OrderBy(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).OrderBy(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where(i => i.Id % 2 == 0).OrderBy(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_Select(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).Select(b => b.Id).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where(i => i.Id % 2 == 0).Select(b => b.Id))
            {
                Assert.Equal(where[counter++], boo);
            }
        }

        [Theory]
        [AutoData]
        public void Where_ToArray(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            var where = items.Where(i => i.Id % 2 == 0).ToArray();
            var localListWhere = localList.Where(i => i.Id % 2 == 0).ToArray();

            for (var i = 0; i < where.Length; i++)
            {
                Assert.Equal(where[i], localListWhere[i]);
            }
        }

        [Theory]
        [AutoData]
        public void Where_ToArray_AvoidClosure(Boo[] items)
        {
            var localList = new LocalList<Boo>(items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = items.Where(i => i.Id % number == 0).ToArray();
            var localListWhere = localList.Where((i, n) => i.Id % n == 0, number).ToArray();

            for (var i = 0; i < where.Length; i++)
            {
                Assert.Equal(where[i], localListWhere[i]);
            }
        }

        [Fact]
        public void Throw_First_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var localList = new LocalList<int>();
                localList.First(e => e > 5);
            });
        }

        [Fact]
        public void Throw_FirstAvoidClosure_NotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var localList = new LocalList<int>();
                localList.First((e, value) => e == value, 5);
            });
        }

        [Fact]
        public void Throw_IndexOutOfRange()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var localList = new LocalList<int>();
                return localList[10];
            });
        }
    }
}