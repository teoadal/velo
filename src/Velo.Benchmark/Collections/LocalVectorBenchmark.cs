using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Velo.Collections;
using Velo.TestsModels.Boos;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class LocalVectorBenchmark
    {
        private const int ALLOCATIONS = 10;

        [Params(6, 10, 25)] 
        public int Count;

        private int[] _items;

        private Boo[] _booItems;
        private Boo[] _booReversItems;

        private int _threshold;
        private int _modifier;

        [GlobalSetup]
        public void Init()
        {
            _items = Enumerable.Range(0, Count).ToArray();

            _booItems = Enumerable.Range(0, Count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            _booReversItems = _booItems.Reverse().ToArray();

            _threshold = _items.Length / 2;
            _modifier = Count;
        }

//        [BenchmarkCategory("Add"), Benchmark(Baseline = true)]
//        public int List_Add()
//        {
//            var list = new List<int>();
//            for (var j = 0; j < _items.Length; j++)
//            {
//                list.Add(_items[j]);
//            }
//
//            return list.Count;
//        }
//
//        [BenchmarkCategory("Add"), Benchmark]
//        public int LocalVector_Add()
//        {
//            var vector = new LocalVector<int>();
//            for (var j = 0; j < _items.Length; j++)
//            {
//                vector.Add(_items[j]);
//            }
//
//            return vector.Length;
//        }
//
//        [BenchmarkCategory("Add"), Benchmark]
//        public int Span_Add()
//        {
//            Span<int> span = stackalloc int[Count];
//            for (var i = 0; i < _items.Length; i++)
//            {
//                span[i] = _items[i];
//            }
//
//            return span.Length;
//        }
//
//        [BenchmarkCategory("Allocation"), Benchmark(Baseline = true)]
//        public int List_Allocation()
//        {
//            var counter = 0;
//            for (var i = 0; i < ALLOCATIONS; i++)
//            {
//                var list = new List<int>();
//                for (var j = 0; j < _items.Length; j++)
//                {
//                    list.Add(_items[j]);
//                }
//
//                counter += list.Count;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Allocation"), Benchmark]
//        public int LocalVector_Allocation()
//        {
//            var counter = 0;
//            for (var i = 0; i < ALLOCATIONS; i++)
//            {
//                var vector = new LocalVector<int>();
//                for (var j = 0; j < _items.Length; j++)
//                {
//                    vector.Add(_items[j]);
//                }
//
//                counter += vector.Length;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Allocation"), Benchmark]
//        public int Span_Allocation()
//        {
//            var counter = 0;
//            for (var i = 0; i < ALLOCATIONS; i++)
//            {
//                Span<int> span = stackalloc int[Count];
//                for (var j = 0; j < _items.Length; j++)
//                {
//                    span[j] = _items[j];
//                }
//
//                counter += span.Length;
//            }
//
//            return counter;
//        }

        [BenchmarkCategory("Iteration"), Benchmark(Baseline = true)]
        public int List_Iteration()
        {
            var list = new List<int>(_items);

            var counter = 0;
            foreach (var number in list)
            {
                counter += number;
            }

            return counter;
        }

        [BenchmarkCategory("Iteration"), Benchmark]
        public int LocalVector_Iteration()
        {
            var localVector = new LocalVector<int>(_items);

            var counter = 0;
            foreach (var number in localVector)
            {
                counter += number;
            }

            return counter;
        }

//        [BenchmarkCategory("Iteration"), Benchmark]
//        public int LocalVector_Iteration_By_Index()
//        {
//            var localVector = new LocalVector<int>(_items);
//
//            var counter = 0;
//            for (var i = 0; i < localVector.Length; i++)
//            {
//                var number = localVector[i];
//                counter += number;
//            }
//
//            return counter;
//        }
        
//        [BenchmarkCategory("Iteration"), Benchmark]
//        public int Span_Iteration()
//        {
//            var span = new Span<int>(_items);
//            
//            var counter = 0;
//            foreach (var number in span)
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Join_Where_Select_OrderBy"), Benchmark(Baseline = true)]
//        public int List_Join_Where_Select_OrderBy()
//        {
//            var outer = new List<Boo>(_booItems);
//            var inner = new List<Boo>(_booReversItems);
//
//            var counter = 0;
//            foreach (var number in outer
//                .Join(inner, o => o, i => i, (o, i) => i)
//                .Where(b => b.Int > _threshold)
//                .Select(b => b.Id * _modifier)
//                .OrderBy(id => id))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Join_Where_Select_OrderBy"), Benchmark]
//        public int LocalVector_Join_Where_Select_OrderBy()
//        {
//            var outer = new LocalVector<Boo>(_booItems);
//            var inner = new LocalVector<Boo>(_booReversItems);
//
//            var counter = 0;
//            foreach (var number in outer
//                .Join(inner, o => o, i => i, (o, i) => i)
//                .Where((b, threshold) => b.Int > threshold, _threshold)
//                .Select((b, modifier) => b.Id * modifier, _modifier)
//                .OrderBy(id => id))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Select"), Benchmark(Baseline = true)]
//        public int List_Select()
//        {
//            var list = new List<int>(_items);
//
//            var counter = 0;
//            foreach (var number in list.Select(i => i * 2))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Select"), Benchmark]
//        public int LocalVector_Select()
//        {
//            var localVector = new LocalVector<int>(_items);
//
//            var counter = 0;
//            foreach (var number in localVector.Select(i => i * 2))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("ToArray"), Benchmark(Baseline = true)]
//        public int[] List_ToArray()
//        {
//            var list = new List<int>(_items);
//            return list.ToArray();
//        }
//
//        [BenchmarkCategory("ToArray"), Benchmark]
//        public int[] LocalVector_ToArray()
//        {
//            var localVector = new LocalVector<int>(_items);
//            return localVector.ToArray();
//        }
//
//        [BenchmarkCategory("ToArray"), Benchmark]
//        public int[] Span_ToArray()
//        {
//            var localVector = new Span<int>(_items);
//            return localVector.ToArray();
//        }
//
//        [BenchmarkCategory("Where"), Benchmark(Baseline = true)]
//        public int List_Where()
//        {
//            var list = new List<int>(_items);
//
//            var counter = 0;
//            foreach (var number in list.Where(i => i % 2 == 0))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Where"), Benchmark]
//        public int LocalVector_Where()
//        {
//            var localVector = new LocalVector<int>(_items);
//
//            var counter = 0;
//            foreach (var number in localVector.Where(i => i % 2 == 0))
//            {
//                counter += number;
//            }
//
//            return counter;
//        }
//
//        [BenchmarkCategory("Where_ToArray"), Benchmark(Baseline = true)]
//        public int List_Where_ToArray()
//        {
//            var list = new List<int>(_items);
//            return list.Where(i => i % 2 == 0).ToArray().Length;
//        }
//
//        [BenchmarkCategory("Where_ToArray"), Benchmark]
//        public int LocalVector_Where_ToArray()
//        {
//            var localVector = new LocalVector<int>(_items);
//            return localVector.Where(i => i % 2 == 0).ToArray().Length;
//        }
    }
}