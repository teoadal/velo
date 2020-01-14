using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Velo.Collections;
using Velo.TestsModels.Boos;

namespace Velo.Benchmark.Collections
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class LocalVectorBenchmark
    {
        private const int InvokeCount = 10;

        [Params(10)] 
        public int Count;

        private Boo[] _items;
        private Boo[] _reversItems;

        private int _threshold;
        private int _modifier;

        [GlobalSetup]
        public void Init()
        {
            _items = Enumerable.Range(0, Count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            _reversItems = _items.Reverse().ToArray();

            _threshold = _items.Length / 2;
            _modifier = Count;
        }

        [BenchmarkCategory("Add")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Add()
        {
            var list = new List<Boo>();
            for (var j = 0; j < _items.Length; j++)
            {
                list.Add(_items[j]);
            }
        
            return list.Count;
        }
        
        [BenchmarkCategory("Add")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Add()
        {
            var localList = new LocalList<Boo>();
            for (var j = 0; j < _items.Length; j++)
            {
                localList.Add(_items[j]);
            }
        
            return localList.Length;
        }
        
        [BenchmarkCategory("Add")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int Span_Add()
        {
            var span = new Span<Boo>(new Boo[Count]);
            for (var i = 0; i < _items.Length; i++)
            {
                span[i] = _items[i];
            }
        
            return span.Length;
        }
        
        [BenchmarkCategory("Allocation")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Allocation()
        {
            var list = new List<Boo>();
        
            foreach (var boo in _items)
            {
                list.Add(boo);
            }
        
            return list.Count;
        }
        
        [BenchmarkCategory("Allocation")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Allocation()
        {
            var localList = new LocalList<Boo>();
        
            foreach (var boo in _items)
            {
                localList.Add(boo);
            }
        
            return localList.Length;
        }
        
        [BenchmarkCategory("Allocation")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int Span_Allocation()
        {
            var span = new Span<Boo>(new Boo[Count]);
        
            for (var j = 0; j < _items.Length; j++)
            {
                span[j] = _items[j];
            }
        
            return span.Length;
        }

        [BenchmarkCategory("Iteration")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Iteration()
        {
            var list = new List<Boo>(_items);

            var counter = 0;
            foreach (var boo in list)
            {
                counter += boo.Id;
            }

            return counter;
        }

        [BenchmarkCategory("Iteration")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Iteration()
        {
            var localList = new LocalList<Boo>(_items);

            var counter = 0;
            foreach (var boo in localList)
            {
                counter += boo.Id;
            }

            return counter;
        }

        [BenchmarkCategory("Iteration")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int Span_Iteration()
        {
            var span = new Span<Boo>(_items.ToArray());
        
            var counter = 0;
            foreach (var boo in span)
            {
                counter += boo.Id;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("GroupBy")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_GroupBy()
        {
            var list = new List<Boo>(_items);
        
            var counter = 0;
            foreach (var group in list.GroupBy(b => b.Id % 2 == 0))
            {
                foreach (var boo in group)
                {
                    counter += boo.Id;
                }
            }
        
            return counter;
        }
        
        [BenchmarkCategory("GroupBy")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_GroupBy()
        {
            var localList = new LocalList<Boo>(_items);
        
            var counter = 0;
            foreach (var group in localList.GroupBy(b => b.Id % 2 == 0))
            {
                foreach (var boo in group)
                {
                    counter += boo.Id;
                }
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Join")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Join()
        {
            var outer = new List<Boo>(_items);
            var inner = new List<Boo>(_reversItems);
        
            return outer
                .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
                .Sum(i => i.Id);
        }
        
        [BenchmarkCategory("Join")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Join()
        {
            var outer = new LocalList<Boo>(_items);
            var inner = new LocalList<Boo>(_reversItems);
        
            return outer
                .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
                .Sum(i => i.Id);
        }
        
        [BenchmarkCategory("ManyLinq")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_ManyLinq()
        {
            var outer = new List<Boo>(_items);
            var inner = new List<Boo>(_reversItems);
        
            return outer
                .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
                .GroupBy(boo => boo.Id)
                .Select(gr => gr.First())
                .Where(b => b.Int > _threshold)
                .Select(b => b.Id * _modifier)
                .OrderBy(id => id)
                .Sum();
        }
        
        [BenchmarkCategory("ManyLinq")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_ManyLinq()
        {
            var outer = new LocalList<Boo>(_items);
            var inner = new LocalList<Boo>(_reversItems);
        
            return outer
                .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
                .GroupBy(boo => boo.Id)
                .Select(gr => gr.First())
                .Where((b, threshold) => b.Int > threshold, _threshold)
                .Select((b, modifier) => b.Id * modifier, _modifier)
                .OrderBy(id => id)
                .Sum();
        }
        
        
        [BenchmarkCategory("Remove")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Remove()
        {
            var list = new List<Boo>(_items);
        
            var counter = 0;
            foreach (var item in _items)
            {
                list.Remove(item);
                counter += list.Count;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Remove")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Remove()
        {
            var localList = new LocalList<Boo>(_items);
        
            var counter = 0;
            foreach (var item in _items)
            {
                localList.Remove(item);
                counter += localList.Length;
            }
        
            return counter;
        }
        
        
        [BenchmarkCategory("Select")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Select()
        {
            var list = new List<Boo>(_items);
        
            var counter = 0;
            foreach (var number in list.Select(b => b.Id))
            {
                counter += number;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Select")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Select()
        {
            var localList = new LocalList<Boo>(_items);
        
            var counter = 0;
            foreach (var number in localList.Select(b => b.Id))
            {
                counter += number;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("ToArray")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public Boo[] List_ToArray()
        {
            var list = new List<Boo>(_items);
            return list.ToArray();
        }
        
        [BenchmarkCategory("ToArray")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public Boo[] LocalVector_ToArray()
        {
            var localList = new LocalList<Boo>(_items);
            return localList.ToArray();
        }
        
        [BenchmarkCategory("ToArray")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public Boo[] Span_ToArray()
        {
            var span = new Span<Boo>(_items);
            return span.ToArray();
        }
        
        [BenchmarkCategory("Where")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Where()
        {
            var list = new List<Boo>(_items);
        
            var counter = 0;
            foreach (var boo in list.Where(b => b.Id > 1))
            {
                counter += boo.Id;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Where")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int List_FindAll()
        {
            var list = new List<Boo>(_items);
        
            var counter = 0;
            foreach (var boo in list.FindAll(b => b.Id > 1))
            {
                counter += boo.Id;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Where")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Where()
        {
            var localList = new LocalList<Boo>(_items);
        
            var counter = 0;
            foreach (var boo in localList.Where(b => b.Id > 1))
            {
                counter += boo.Id;
            }
        
            return counter;
        }
        
        [BenchmarkCategory("Where_ToArray")]
        [Benchmark(Baseline = true, OperationsPerInvoke = InvokeCount)]
        public int List_Where_ToArray()
        {
            var list = new List<Boo>(_items);
            return list.Where(b => b.Id > 1).ToArray().Length;
        }
        
        [BenchmarkCategory("Where_ToArray")]
        [Benchmark(OperationsPerInvoke = InvokeCount)]
        public int LocalVector_Where_ToArray()
        {
            var localList = new LocalList<Boo>(_items);
            return localList.Where(b => b.Id > 1).ToArray().Length;
        }
    }
}