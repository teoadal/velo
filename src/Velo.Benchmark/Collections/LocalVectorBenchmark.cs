using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Velo.Collections;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class LocalVectorBenchmark
    {
        private const int AllocationsCount = 100;
        
        [Params(2, 5, 1000)] 
        public int ElementsCount;
        
        private int[] _items;

        [GlobalSetup]
        public void Init()
        {
            _items = Enumerable.Range(0, ElementsCount).ToArray();
        }
        
        [Benchmark(Baseline = true)]
        public int List()
        {
            var counter = 0;
            for (var i = 0; i < AllocationsCount; i++)
            {
                var list = new List<int>();
                for (var j = 0; j < _items.Length; j++)
                {
                    list.Add(_items[j]);
                }

                counter += list.Count;
            }

            return counter;
        }
        
        [Benchmark]
        public int LocalVector()
        {
            var counter = 0;
            for (var i = 0; i < AllocationsCount; i++)
            {
                var vector = new LocalVector<int>();
                for (var j = 0; j < _items.Length; j++)
                {
                    vector.Add(_items[j]);
                }

                counter += vector.Length;
            }

            return counter;
        }
    }
}