using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Velo.Collections;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SequenceAddBenchmark
    {
        [Params(15, 10000)] 
        public int Count;

        private int[] _items;

        [GlobalSetup]
        public void Init()
        {
            _items = Enumerable.Range(0, Count).ToArray();
        }

        [BenchmarkCategory("Regular"), Benchmark(Baseline = true)]
        public int List()
        {
            var list = new List<int>();

            for (var i = 0; i < _items.Length; i++)
            {
                list.Add(_items[i]);
            }

            return list.Count;
        }

        [BenchmarkCategory("Regular"), Benchmark]
        public int Sequence()
        {
            var sequence = new Sequence<int>();

            for (var i = 0; i < _items.Length; i++)
            {
                sequence.Add(_items[i]);
            }

            return sequence.Length;
        }

        [BenchmarkCategory("Fixed"), Benchmark(Baseline = true)]
        public int List_Fixed()
        {
            var list = new List<int>(Count);

            for (var i = 0; i < _items.Length; i++)
            {
                list.Add(_items[i]);
            }

            return list.Count;
        }

        [BenchmarkCategory("Fixed"), Benchmark]
        public int Sequence_Fixed()
        {
            var sequence = new Sequence<int>(Count);

            for (var i = 0; i < _items.Length; i++)
            {
                sequence.Add(_items[i]);
            }

            return sequence.Length;
        }
    }
}