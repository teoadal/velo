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
        private const int ALLOCATIONS = 10;

        [Params(6)] 
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

        // [BenchmarkCategory("Add"), Benchmark(Baseline = true)]
        // public int List_Add()
        // {
        //     var list = new List<Boo>();
        //     for (var j = 0; j < _items.Length; j++)
        //     {
        //         list.Add(_items[j]);
        //     }
        //
        //     return list.Count;
        // }
        //
        // [BenchmarkCategory("Add"), Benchmark]
        // public int LocalVector_Add()
        // {
        //     var vector = new LocalVector<Boo>();
        //     for (var j = 0; j < _items.Length; j++)
        //     {
        //         vector.Add(_items[j]);
        //     }
        //
        //     return vector.Length;
        // }
        //
        // [BenchmarkCategory("Add"), Benchmark]
        // public int Span_Add()
        // {
        //     var span = new Span<Boo>(new Boo[Count]);
        //     for (var i = 0; i < _items.Length; i++)
        //     {
        //         span[i] = _items[i];
        //     }
        //
        //     return span.Length;
        // }
        //
        // [BenchmarkCategory("Allocation"), Benchmark(Baseline = true, OperationsPerInvoke = ALLOCATIONS)]
        // public int List_Allocation()
        // {
        //     var list = new List<Boo>();
        //
        //     foreach (var boo in _items)
        //     {
        //         list.Add(boo);
        //     }
        //
        //     return list.Count;
        // }
        //
        // [BenchmarkCategory("Allocation"), Benchmark(OperationsPerInvoke = ALLOCATIONS)]
        // public int LocalVector_Allocation()
        // {
        //     var vector = new LocalVector<Boo>();
        //
        //     foreach (var boo in _items)
        //     {
        //         vector.Add(boo);
        //     }
        //
        //     return vector.Length;
        // }
        //
        // [BenchmarkCategory("Allocation"), Benchmark(OperationsPerInvoke = ALLOCATIONS)]
        // public int Span_Allocation()
        // {
        //     var span = new Span<Boo>(new Boo[Count]);
        //
        //     for (var j = 0; j < _items.Length; j++)
        //     {
        //         span[j] = _items[j];
        //     }
        //
        //     return span.Length;
        // }

        // [BenchmarkCategory("Iteration"), Benchmark(Baseline = true)]
        // public int List_Iteration()
        // {
        //     var list = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var boo in list)
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("Iteration"), Benchmark]
        // public int LocalVector_Iteration()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var boo in vector)
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }

        // [BenchmarkCategory("Iteration"), Benchmark]
        // public int Span_Iteration()
        // {
        //     var span = new Span<Boo>(_items.ToArray());
        //
        //     var counter = 0;
        //     foreach (var boo in span)
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("GroupBy"), Benchmark(Baseline = true)]
        // public int List_GroupBy()
        // {
        //     var list = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var group in list.GroupBy(b => b.Id % 2 == 0))
        //     {
        //         foreach (var boo in group)
        //         {
        //             counter += boo.Id;
        //         }
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("GroupBy"), Benchmark]
        // public int LocalVector_GroupBy()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var group in vector.GroupBy(b => b.Id % 2 == 0))
        //     {
        //         foreach (var boo in group)
        //         {
        //             counter += boo.Id;
        //         }
        //     }
        //
        //     return counter;
        // }

        // [BenchmarkCategory("Join"), Benchmark(Baseline = true)]
        // public int List_Join()
        // {
        //     var outer = new List<Boo>(_items);
        //     var inner = new List<Boo>(_reversItems);
        //
        //     return outer
        //         .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
        //         .Sum(i => i.Id);
        // }
        //
        // [BenchmarkCategory("Join"), Benchmark]
        // public int LocalVector_Join()
        // {
        //     var outer = new LocalVector<Boo>(_items);
        //     var inner = new LocalVector<Boo>(_reversItems);
        //
        //     return outer
        //         .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
        //         .Sum(i => i.Id);
        // }
        
        // [BenchmarkCategory("ManyLinq"), Benchmark(Baseline = true)]
        // public int List_ManyLinq()
        // {
        //     var outer = new List<Boo>(_items);
        //     var inner = new List<Boo>(_reversItems);
        //
        //     return outer
        //         .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
        //         .GroupBy(boo => boo.Id)
        //         .Select(gr => gr.First())
        //         .Where(b => b.Int > _threshold)
        //         .Select(b => b.Id * _modifier)
        //         .OrderBy(id => id)
        //         .Sum();
        // }
        //
        // [BenchmarkCategory("ManyLinq"), Benchmark]
        // public int LocalVector_ManyLinq()
        // {
        //     var outer = new LocalVector<Boo>(_items);
        //     var inner = new LocalVector<Boo>(_reversItems);
        //
        //     return outer
        //         .Join(inner, o => o.Id, i => i.Id, (o, i) => i)
        //         .GroupBy(boo => boo.Id)
        //         .Select(gr => gr.First())
        //         .Where((b, threshold) => b.Int > threshold, _threshold)
        //         .Select((b, modifier) => b.Id * modifier, _modifier)
        //         .OrderBy(id => id)
        //         .Sum();
        // }

        //
        // [BenchmarkCategory("Remove"), Benchmark(Baseline = true)]
        // public int List_Remove()
        // {
        //     var list = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var item in _items)
        //     {
        //         list.Remove(item);
        //         counter += list.Count;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("Remove"), Benchmark]
        // public int LocalVector_Remove()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var item in _items)
        //     {
        //         vector.Remove(item);
        //         counter += vector.Length;
        //     }
        //
        //     return counter;
        // }
        //
        //
        // [BenchmarkCategory("Select"), Benchmark(Baseline = true)]
        // public int List_Select()
        // {
        //     var list = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var number in list.Select(b => b.Id))
        //     {
        //         counter += number;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("Select"), Benchmark]
        // public int LocalVector_Select()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var number in vector.Select(b => b.Id))
        //     {
        //         counter += number;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("ToArray"), Benchmark(Baseline = true)]
        // public Boo[] List_ToArray()
        // {
        //     var list = new List<Boo>(_items);
        //     return list.ToArray();
        // }
        //
        // [BenchmarkCategory("ToArray"), Benchmark]
        // public Boo[] LocalVector_ToArray()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //     return vector.ToArray();
        // }
        //
        // [BenchmarkCategory("ToArray"), Benchmark]
        // public Boo[] Span_ToArray()
        // {
        //     var span = new Span<Boo>(_items);
        //     return span.ToArray();
        // }
        //
        // [BenchmarkCategory("Where"), Benchmark(Baseline = true)]
        // public int List_Where()
        // {
        //     var list = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var boo in list.Where(b => b.Id > 1))
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("Where"), Benchmark]
        // public int List_FindAll()
        // {
        //     var vector = new List<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var boo in vector.FindAll(b => b.Id > 1))
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }
        //
        //
        // [BenchmarkCategory("Where"), Benchmark]
        // public int LocalVector_Where()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //
        //     var counter = 0;
        //     foreach (var boo in vector.Where(b => b.Id > 1))
        //     {
        //         counter += boo.Id;
        //     }
        //
        //     return counter;
        // }
        //
        // [BenchmarkCategory("Where_ToArray"), Benchmark(Baseline = true)]
        // public int List_Where_ToArray()
        // {
        //     var list = new List<Boo>(_items);
        //     return list.Where(b => b.Id > 1).ToArray().Length;
        // }
        //
        // [BenchmarkCategory("Where_ToArray"), Benchmark]
        // public int LocalVector_Where_ToArray()
        // {
        //     var vector = new LocalVector<Boo>(_items);
        //     return vector.Where(b => b.Id > 1).ToArray().Length;
        // }
    }
}