using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections.Local;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Local
{
    public class LocalListShould : TestClass
    {
        private readonly Boo[] _items;

        public LocalListShould(ITestOutputHelper output) : base(output)
        {
            _items = Fixture.CreateMany<Boo>(10).ToArray();
        }

        [Theory]
        [InlineData(25)]
        [InlineData("test")]
        [InlineData(typeof(TestClass))]
        public void AddValue<T>(T item)
        {
            var localList = new LocalList<T>();
            localList.Add(item);

            localList.Contains(item).Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public void AddMany(int count)
        {
            var localList = new LocalList<int>();

            var items = Many(EnsureValid(count), i => i);

            foreach (var item in items)
            {
                localList.Add(item);
                localList.Contains(item).Should().BeTrue();
            }

            localList.Length.Should().Be(items.Length);
        }

        [Fact]
        public void AddRangeFromArray()
        {
            var localList = new LocalList<Boo>();
            localList.AddRange(_items);

            foreach (var item in _items)
            {
                localList.Contains(item).Should().BeTrue();
            }

            localList.Length.Should().Be(_items.Length);
        }

        [Theory]
        [AutoData]
        public void AddRangeFromLocalList(Boo[] items)
        {
            var localList = new LocalList<Boo>();
            localList.AddRange(new LocalList<Boo>(items));

            foreach (var item in items)
            {
                localList.Contains(item).Should().BeTrue();
            }

            localList.Length.Should().Be(items.Length);
        }

        [Fact]
        public void CheckAll()
        {
            var localList = new LocalList<Boo>(_items);
            localList.All(element => element.Bool);
        }

        [Fact]
        public void CheckAllWithoutClosure()
        {
            var localList = new LocalList<Boo>(_items);
            localList.All((element, v) => element.Bool == v, true);
        }

        [Fact]
        public void CheckAny()
        {
            var localList = new LocalList<Boo>(_items);

            foreach (var item in _items)
            {
                // ReSharper disable once HeapView.CanAvoidClosure
                localList.Any(element => element == item);
            }
        }

        [Fact]
        public void CheckAnyWithoutClosure()
        {
            var localList = new LocalList<Boo>(_items);

            foreach (var item in _items)
            {
                localList.Any((element, i) => element == i, item);
            }
        }

        [Fact]
        public void CheckContains()
        {
            var localList = new LocalList<Boo>(_items);
            localList.Contains(_items[0]).Should().BeTrue();
            localList.Contains(new Boo(), EqualityComparer<Boo>.Default).Should().BeFalse();
        }

        [Fact]
        public void Clear()
        {
            var localList = new LocalList<Boo>(_items);

            localList.Clear();

            localList.Length.Should().Be(0);
        }

        [Fact]
        public void First()
        {
            var localList = new LocalList<Boo>();

            foreach (var item in _items)
            {
                localList.Add(item);

                // ReSharper disable once HeapView.CanAvoidClosure
                localList.First(e => e == item).Should().Be(item);
            }
        }

        [Fact]
        public void FirstWithoutClosure()
        {
            var localList = new LocalList<Boo>();

            foreach (var item in _items)
            {
                localList.Add(item);
                localList.First((e, i) => e == i, item).Should().Be(item);
            }
        }

        [Fact]
        public void Enumerate()
        {
            var localList = new LocalList<Boo>(_items);

            foreach (var element in localList)
            {
                _items.Contains(element).Should().BeTrue();
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
                groupEnumerator.MoveNext().Should().BeTrue();

                var expectedGroup = groupEnumerator.Current;

                // ReSharper disable once PossibleNullReferenceException
                localListGroup.Key.Should().Be(expectedGroup.Key);

                var expectedGroupEnumerator = expectedGroup.GetEnumerator();
                foreach (var boo in localListGroup.Values)
                {
                    expectedGroupEnumerator.MoveNext().Should().BeTrue();
                    boo.Should().Be(expectedGroupEnumerator.Current);
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
                .Select(gr => gr.Values.Sum(b => b.Id)))
            {
                Assert.True(groupEnumerator.MoveNext());
                actual.Should().Be(groupEnumerator.Current);
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
                localListGroup.Key.Should().Be(expectedGroup.Key);

                var expectedGroupEnumerator = expectedGroup.Where(b => b.Id > 1).GetEnumerator();
                foreach (var boo in localListGroup.Values.Where(b => b.Id > 1))
                {
                    expectedGroupEnumerator.MoveNext().Should().BeTrue();
                    boo.Should().Be(expectedGroupEnumerator.Current);
                }

                expectedGroupEnumerator.Dispose();
            }

            groupEnumerator.Dispose();
        }

        [Fact]
        public void IndexOf()
        {
            var localList = new LocalList<Boo>(_items);
            for (var index = 0; index < _items.Length; index++)
            {
                var boo = _items[index];
                localList.IndexOf(boo).Should().Be(index);
            }
        }

        [Fact]
        public void Indexer()
        {
            var localList = new LocalList<Boo>(_items);

            for (var i = 0; i < localList.Length; i++)
            {
                localList[i].Should().Be(_items[i]);
            }
        }

        [Fact]
        public void InitializeFromItems()
        {
            var list1 = new LocalList<int>(item0: 1);
            list1.Contains(1).Should().BeTrue();
            list1.Length.Should().Be(1);

            var list2 = new LocalList<int>(1, 2);
            list2.Contains(1).Should().BeTrue();
            list2.Contains(2).Should().BeTrue();
            list2.Length.Should().Be(2);

            var list3 = new LocalList<int>(1, 2, 3);
            list3.Contains(1).Should().BeTrue();
            list3.Contains(2).Should().BeTrue();
            list3.Contains(3).Should().BeTrue();
            list3.Length.Should().Be(3);

            var list4 = new LocalList<int>(1, 2, 3, 4);
            list4.Contains(1).Should().BeTrue();
            list4.Contains(2).Should().BeTrue();
            list4.Contains(3).Should().BeTrue();
            list4.Contains(4).Should().BeTrue();
            list4.Length.Should().Be(4);

            var list5 = new LocalList<int>(1, 2, 3, 4, 5);
            list5.Contains(1).Should().BeTrue();
            list5.Contains(2).Should().BeTrue();
            list5.Contains(3).Should().BeTrue();
            list5.Contains(4).Should().BeTrue();
            list5.Contains(5).Should().BeTrue();
            list5.Length.Should().Be(5);
        }

        [Theory, AutoData]
        public void InitializeFromCollection(Boo[] collection)
        {
            var localList = new LocalList<Boo>(collection);
            localList.Length.Should().Be(collection.Length);

            var collectionList = new LocalList<Boo>((ICollection<Boo>) collection);
            collectionList.Length.Should().Be(collection.Length);
        }

        [Fact]
        public void Join()
        {
            static int OuterKeySelector(Boo b) => b.Id;
            static int InnerKeySelector(Foo f) => f.Int;
            static float ResultBuilder(Boo b, Foo f) => b.Float;

            var outer = new LocalList<Boo>(_items);

            var innerItems = _items.Select(i => new Foo {Int = i.Id}).ToArray();
            var inner = new LocalList<Foo>(innerItems);

            var join = _items.Join(innerItems, OuterKeySelector, InnerKeySelector, ResultBuilder).ToArray();

            var counter = 0;
            foreach (var number in outer.Join(inner, OuterKeySelector, InnerKeySelector, ResultBuilder))
            {
                number.Should().Be(join[counter++]);
            }
        }

        [Fact]
        public void Length()
        {
            var localList = new LocalList<Boo>(_items);
            localList.Length.Should().Be(_items.Length);
        }

        [Theory]
        [AutoData]
        public void MixOfAddRemove(int count)
        {
            var items = new LocalList<int>();

            for (var i = 0; i < EnsureValid(count); i++)
            {
                items.Add(i);
                items.Contains(i).Should().BeTrue();

                if (i % 2 != 0) continue;

                items.Remove(i).Should().BeTrue();
                items.Contains(i).Should().BeFalse();
            }
        }

        [Fact]
        public void NotRemoveNoyContains()
        {
            var localList = new LocalList<int>();
            localList.Remove(1).Should().BeFalse();
        }
        
        [Fact]
        public void Remove()
        {
            var localList = new LocalList<Boo>(_items);
            var item = _items[_items.Length / 2];

            localList.Remove(item).Should().BeTrue();
            localList.Contains(item).Should().BeFalse();
        }

        [Fact]
        public void RemoveAll()
        {
            var localList = new LocalList<Boo>(_items);

            foreach (var item in _items)
            {
                localList.Remove(item).Should().BeTrue();
                localList.Contains(item).Should().BeFalse();
            }
        }

        [Fact]
        public void RemoveAt()
        {
            var localList = new LocalList<Boo>(_items);

            for (var i = 0; i < _items.Length; i++)
            {
                var element = localList[0];
                localList.RemoveAt(0);
                localList.Contains(element).Should().BeFalse();
            }
        }

        [Fact]
        public void RemoveLast()
        {
            var localList = new LocalList<Boo>(_items);
            var item = _items[^1];

            localList.Remove(item).Should().BeTrue();
            localList.Contains(item).Should().BeFalse();
        }

        [Fact]
        public void Reverse()
        {
            var localList = new LocalList<Boo>(_items);

            var items = _items.Reverse().ToArray();
            localList.Reverse();

            for (var i = 0; i < localList.Length; i++)
            {
                localList[i].Should().Be(items[i]);
            }
        }

        [Fact]
        public void Select()
        {
            var select = _items.Select(i => i.Id).ToArray();

            var localList = new LocalList<Boo>(_items);

            var counter = 0;
            foreach (var number in localList.Select(i => i.Id))
            {
                number.Should().Be(select[counter++]);
            }
        }

        [Fact]
        public void SelectWithoutClosure()
        {
            // ReSharper disable once ConvertToConstant.Local
            var threshold = 1;
            var select = _items.Select(i => i.Id > threshold ? 5 : 10).ToArray();

            var localList = new LocalList<Boo>(_items);

            var counter = 0;
            foreach (var number in localList.Select((i, t) => i.Id > t ? 5 : 10, threshold))
            {
                number.Should().Be(select[counter++]);
            }
        }

        [Fact]
        public void Sort()
        {
            var sorted = _items.OrderBy(b => b.Id).ToArray();

            var localList = new LocalList<Boo>(_items);

            localList.Sort(b => b.Id);

            for (var i = 0; i < sorted.Length; i++)
            {
                localList[i].Should().Be(sorted[i]);
            }
        }

        [Theory]
        [AutoData]
        public void Sum(int count)
        {
            count = EnsureValid(count);

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

            int localListSum;
            using (Measure())
            {
                localListSum = outerLocalList
                    .Join(innerLocalList, o => o.Id, i => i.Id, (o, i) => i)
                    .GroupBy(boo => boo.Id)
                    .Select(gr => gr.Values.First())
                    .Where((b, t) => b.Int > t, threshold)
                    .Select((b, m) => b.Id * m, modifier)
                    .OrderBy(id => id)
                    .Sum();
            }

            localListSum.Should().Be(sum);
        }

        [Fact]
        public void ToArray()
        {
            var array = new LocalList<Boo>(_items).ToArray();

            for (var i = 0; i < _items.Length; i++)
            {
                array[i].Should().Be(_items[i]);
            }
        }

        [Fact]
        public void ToEmptyArray()
        {
            var localList = new LocalList<Boo>();
            localList.ToArray().Should().BeEmpty();
        }

        [Fact]
        public void ToArrayGranularity()
        {
            var localList = new LocalList<int>();

            for (var i = 0; i < 100; i++)
            {
                localList.Add(i);
                localList.Length.Should().Be(localList.ToArray().Length);
            }
        }

        [Fact]
        public void ToList()
        {
            var list = new LocalList<Boo>(_items).ToList();

            for (var i = 0; i < _items.Length; i++)
            {
                list[i].Should().Be(_items[i]);
            }
        }

        [Fact]
        public void ToListEmpty()
        {
            var list = new LocalList<Boo>().ToList();
            list.Should().BeEmpty();
        }
        
        [Fact]
        public void Where()
        {
            var localList = new LocalList<Boo>(_items);

            var where = _items.Where(i => i.Id % 2 == 0).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where(i => i.Id % 2 == 0))
            {
                boo.Should().Be(where[counter++]);
            }
        }

        [Fact]
        public void WhereWithoutClosure()
        {
            var localList = new LocalList<Boo>(_items);

            // ReSharper disable once ConvertToConstant.Local
            var number = 2;

            var where = _items.Where(i => i.Id % number == 0).ToArray();

            var counter = 0;
            foreach (var boo in localList.Where((i, n) => i.Id % n == 0, number))
            {
                boo.Should().Be(where[counter++]);
            }
        }

        [Fact]
        public void Throw_First()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var localList = new LocalList<int>();
                localList.First();
            });
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
        public void Throw_RemoveAt()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var localList = new LocalList<int>();
                localList.RemoveAt(1);
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